import boto3
import os
import uuid
import time
import datetime
import asyncio
import logging
from botocore.exceptions import ClientError

# Initialize Logger
logger = logging.getLogger()
logger.setLevel(logging.INFO)

def set_global_vars():
    """
    Set global variables and override defaults if provided by the user.
    
    :return: Dictionary containing global environment variables.
    """
    global_vars = {"status": False}
    try:
        global_vars.update({
            "Owner": "arn:aws:iam::457115063442:role/ListBucketPolicy",
            "Environment": "Prod",
            "aws_region": "us-east-1",
            "tag_name": "cloudwatch_logs_exporter",
            "retention_days": 5,
            "cw_logs_to_export": ["/aws/bundle-logs"],
            "time_out": 300,
            "tsk_back_off": 2,
            "log_group_name": "/aws/bundle-logs",
            "bucket_name": "cw-log-export-02"
        })
        global_vars["status"] = True
    except Exception as e:
        logger.error(f"Unable to set Global Environment variables: {str(e)}")
        global_vars["error_message"] = str(e)
    return global_vars

def gen_uuid():
    """Generate and return a unique UUID string."""
    return str(uuid.uuid4())

def gen_ymd_from_epoch(t):
    """
    Generates a string of the format 'YYYY-MM-DD' from the given epoch time.
    
    :param t: Epoch time in milliseconds.
    :return: Date string formatted as 'YYYY-MM-DD'.
    """
    return datetime.datetime.utcfromtimestamp(t / 1000).strftime("%Y-%m-%d")

def gen_ymd(t, d="-"):
    """
    Generates a string of the format 'YYYY-MM-DD' from the given datetime.
    
    :param t: datetime object.
    :param d: Delimiter to separate the year, month, and day.
    :return: Date string formatted as 'YYYY{delimiter}MM{delimiter}DD'.
    """
    return f"{t.year}{d}{t.month:02d}{d}{t.day:02d}"

def does_bucket_exist(bucket_name):
    """
    Checks if an S3 bucket exists by using a HEAD request.
    
    :param bucket_name: Name of the S3 bucket to check.
    :return: Dictionary with status and error message (if any).
    """
    bucket_exists_status = {'status': False, 'error_message': ''}
    try:
        s3 = boto3.resource('s3')
        bucket_exists = any(bucket.name == bucket_name for bucket in s3.buckets.all())
        if bucket_exists:
            bucket_exists_status['status'] = True
        else:
            bucket_exists_status['error_message'] = f"Bucket '{bucket_name}' does not exist."
    except ClientError as e:
        bucket_exists_status['error_message'] = str(e)
    return bucket_exists_status

def get_cloudwatch_log_groups():
    """
    Retrieves the list of CloudWatch Log Groups.
    
    :return: Dictionary containing status, log groups, and error message (if any).
    """
    resp_data = {'status': False, 'log_groups': [], 'error_message': ''}
    client = boto3.client('logs')
    try:
        resp = client.describe_log_groups(limit=50)
        resp_data['log_groups'].extend(resp.get('logGroups', []))
        while 'nextToken' in resp:
            resp = client.describe_log_groups(nextToken=resp['nextToken'], limit=50)
            resp_data['log_groups'].extend(resp.get('logGroups', []))
        resp_data['status'] = True
    except ClientError as e:
        resp_data['error_message'] = str(e)
    return resp_data

def filter_logs_to_export(global_vars, log_groups):
    """
    Filters the CloudWatch log groups based on the global variables.
    
    :param global_vars: Global configuration variables.
    :param log_groups: List of CloudWatch log groups to filter.
    :return: Dictionary containing status, filtered log groups, and error message (if any).
    """
    filtered_logs = [lg for lg in log_groups if lg['logGroupName'] in global_vars.get('log_group_name', [])]
    return {'status': bool(filtered_logs), 'log_groups': filtered_logs, 'error_message': ''}

async def export_cw_logs_to_s3(global_vars, log_group_name, retention_days, bucket_name, obj_prefix=None):
    """
    Exports CloudWatch logs to an S3 bucket with a specified retention period.
    
    :param global_vars: Global configuration variables.
    :param log_group_name: Name of the CloudWatch log group to export.
    :param retention_days: Number of days to retain logs.
    :param bucket_name: S3 bucket name for storing the logs.
    :param obj_prefix: Optional prefix for the exported objects in S3.
    :return: Dictionary containing status, task information, and error message (if any).
    """
    resp_data = {'status': False, 'task_info': {}, 'error_message': ''}
    if not retention_days:
        retention_days = 90
    obj_prefix = obj_prefix or log_group_name.split('/')[-1]
    now_time = datetime.datetime.now()
    n1_day = now_time - datetime.timedelta(days=retention_days + 1)
    n_day = now_time
    f_time = int(n1_day.timestamp())
    t_time = int(n_day.timestamp())
    d_prefix = log_group_name.replace("/", "-")[1:]

    resp = does_bucket_exist(bucket_name)
    if not resp['status']:
        resp_data['error_message'] = resp['error_message']
        return resp_data

    try:
        client = boto3.client('logs')
        task_response = client.create_export_task(
            taskName=gen_uuid(),
            logGroupName=log_group_name,
            fromTime=f_time,
            to=t_time,
            destination=bucket_name,
            destinationPrefix=d_prefix
        )

        task_status = get_tsk_status(task_response['taskId'], global_vars['time_out'], global_vars['tsk_back_off'])
        resp_data['task_info'] = task_status['tsk_info']
        resp_data['status'] = task_status['status']
        if not task_status['status']:
            resp_data['error_message'] = task_status['error_message']
    except ClientError as e:
        resp_data['error_message'] = str(e)
    
    return resp_data

def get_tsk_status(tsk_id, time_out, tsk_back_off):
    """
    Retrieves the status of a CloudWatch export task.
    
    :param tsk_id: Task ID of the export task.
    :param time_out: Maximum time to wait for the task to complete.
    :param tsk_back_off: Time between retries in case of failure.
    :return: Dictionary containing status, task information, and error message (if any).
    """
    resp_data = {'status': False, 'tsk_info': {}, 'error_message': ''}
    client = boto3.client('logs')
    if not time_out:
        time_out = 300
    t = tsk_back_off
    try:
        while True:
            time.sleep(t)
            resp = client.describe_export_tasks(taskId=tsk_id)
            tsk_info = resp['exportTasks'][0]
            if t > time_out:
                resp_data['error_message'] = f"Task:{tsk_id} is still running. Status: {tsk_info['status']['code']}"
                resp_data['tsk_info'] = tsk_info
                break
            if tsk_info['status']['code'] != "COMPLETED":
                t *= 2  # Exponential backoff
            else:
                resp_data['tsk_info'] = tsk_info
                resp_data['status'] = True
                break
    except ClientError as e:
        resp_data['error_message'] = f"Unable to verify status of task:{tsk_id}. ERROR:{str(e)}"
    resp_data['tsk_info']['time_taken'] = t
    logger.info(f"Export task '{tsk_id}' took {t} seconds.")
    return resp_data

def export_logs_to_s3_handler(event, context):
    """
    Main handler for processing log export tasks.
    
    :param event: The event that triggers the lambda function (not used).
    :param context: The context for the lambda function (not used).
    :return: Dictionary containing status and results of the export tasks.
    """
    global_vars = set_global_vars()
    resp_data = {"status": False, "error_message": ''}

    if not global_vars['status']:
        logger.error(f"ERROR: {global_vars['error_message']}")
        resp_data['error_message'] = global_vars['error_message']
        return resp_data

    log_groups = get_cloudwatch_log_groups()
    if not log_groups['status']:
        logger.error(f"Unable to get list of CloudWatch Logs: {log_groups['error_message']}")
        resp_data['error_message'] = log_groups['error_message']
        return resp_data

    filtered_logs = filter_logs_to_export(global_vars, log_groups['log_groups'])
    if not filtered_logs['status'] or not filtered_logs['log_groups']:
        logger.error(f"No log groups match the filter or unable to filter: {filtered_logs['error_message']}")
        resp_data['error_message'] = filtered_logs['error_message']
        return resp_data

    resp_data['export_tasks'] = []
    loop = asyncio.new_event_loop()
    asyncio.set_event_loop(loop)

    # Begin the export process for each log group
    for lg in filtered_logs['log_groups']:
        export_resp = loop.run_until_complete(export_cw_logs_to_s3(
            global_vars, lg['logGroupName'], global_vars['retention_days'], global_vars['bucket_name']))
        resp_data['export_tasks'].append(export_resp)
    
    loop.close()
    resp_data['status'] = True
    return resp_data

if __name__ == '__main__':
    lambda_handler(None, None)
