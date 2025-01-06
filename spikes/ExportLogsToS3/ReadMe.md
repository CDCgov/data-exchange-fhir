

This lambda function exports log groups to a S3 Bucket on the AWS account.

It will only export each log group if it has the tag ExportToS3=true, if the last export was more than 24 hours ago it creates 
an export task to the S3_BUCKET defined saving the current timestamp in a SSM parameter.


At a glance:
Env variable S3_BUCKET needs to be set. It’s the bucket to export the logs.
Creates a Cloudwatch Logs Export Task
It only exports logs from Log Groups that have a tag ExportToS3=true
It will use the log group name as the prefix folder when exporting
Saves a checkpoint in SSM so it exports from that timestamp next time
Only exports if 24 hours have passed from the last checkpoint

List Bundle Policy IAM Role:

{
    "Version": "2012-10-17",
    "Statement": [
        {
            "Effect": "Allow",
            "Action": [
                "logs:DescribeLogGroups",
                "logs:DescribeLogStreams",
                "logs:FilterLogEvents",
                "logs:CreateExportTask",
                "logs:DescribeExportTasks"
            ],
            "Resource": ""
        },
        {
            "Effect": "Allow",
            "Action": [
                "s3:PutObject",
                "s3:GetBucketAcl",
                "s3:ListAllMyBuckets"
            ],
            "Resource": [
                "arn:aws:s3:::cw-log-export-02",
                "arn:aws:s3:::cw-log-export-02/"
            ]
        },
        {
            "Effect": "Allow",
            "Action": [
                "s3:ListAllMyBuckets"
            ],
            "Resource": "arn:aws:s3:::*"
        }
    ]
}

Bucket Policy:
{
    "Version": "2012-10-17",
    "Statement": [
        {
            "Sid": "AllowCloudWatchLogsExport",
            "Effect": "Allow",
            "Principal": {
                "Service": "logs.us-east-1.amazonaws.com"
            },
            "Action": "s3:PutObject",
            "Resource": "arn:aws:s3:::cw-log-export-02/*"
        },
        {
            "Sid": "AllowCloudWatchLogsGetBucketAcl",
            "Effect": "Allow",
            "Principal": {
                "Service": "logs.us-east-1.amazonaws.com"
            },
            "Action": "s3:GetBucketAcl",
            "Resource": "arn:aws:s3:::cw-log-export-02"
        }
    ]
}