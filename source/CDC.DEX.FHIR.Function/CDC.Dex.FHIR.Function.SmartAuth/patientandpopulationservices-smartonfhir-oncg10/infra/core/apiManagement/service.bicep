param apiManagementServiceName string
param location string
param sku string
param skuCount int
param publisherName string
param publisherEmail string
param appInsightsInstrumentationKey string

resource apim 'Microsoft.ApiManagement/service@2021-12-01-preview' = {
  name: apiManagementServiceName
  location: location
  sku: {
    name: sku
    capacity: skuCount
  }

  identity: {
    type: 'SystemAssigned'
  }
  resource portalSettings 'portalsettings@2022-09-01-preview' = {
    name: 'signin'
    properties: {
        enabled: true // Compliant: Sign-in is enabled for portal access
    }
  }
  properties: {
    publisherEmail: publisherEmail
    publisherName: publisherName
    authenticationSettings: { // Compliant: API has authentication enabled
    openid: {
        bearerTokenSendingMethods: ['authorizationHeader']
        openidProviderId: '<an OpenID provider ID>'
        }
    }
  }

  resource apimLogger 'loggers' = {
    name: 'appinsights'
    properties: {
      loggerType: 'applicationInsights'
      credentials: {
        appInsightsInstrumentationKey: appInsightsInstrumentationKey
        instrumentationKey: appInsightsInstrumentationKey
      }
      isBuffered: true
    }
  }

  resource apimDiagnostics 'diagnostics' = {
    name: 'applicationinsights'
    properties: {
      alwaysLog: 'allErrors'
      httpCorrelationProtocol: 'W3C'
      logClientIp: true
      loggerId: apimLogger.id
      sampling: {
        samplingType: 'fixed'
        percentage: 100
      }
      frontend: {
        request: {
          dataMasking: {
            queryParams: [
              {
                value: '*'
                mode: 'Hide'
              }
            ]
          }
        }
      }
      backend: {
        request: {
          dataMasking: {
            queryParams: [
              {
                value: '*'
                mode: 'Hide'
              }
            ]
          }
        }
      }
    }
  }
}

output name string = apim.name
output serviceLoggerId string = apim::apimLogger.id
