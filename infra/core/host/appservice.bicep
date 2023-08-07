@minLength(3)
@maxLength(40)
param name string
param location string = resourceGroup().location
param tags object
param kind string = 'app,linux'
param appServicePlanId string
param applicationInsightsName string
// param keyVaultName string = ''

@allowed([ 'DOTNET', 'DOTNET-ISOLATED', 'NODE', 'Python', 'Java', 'Powershell' ])
param runtimeName string
param runtimeVersion string

// Microsoft.Web/sites/config
param alwaysOn bool = true
param appCommandLine string = ''
param appSettings array = []
param clientAffinityEnabled bool = false
// param enableOryxBuild bool = contains(kind, 'linux')
param functionAppScaleLimit int = 0
param minimumElasticInstanceCount int = -1
param numberOfWorkers int = -1
param scmDoBuildDuringDeployment bool = false
param use32BitWorkerProcess bool = false
param healthCheckPath string = ''
param allowedOrigins array = []
param serviceName string

var runtimeNameAndVersion = '${runtimeName}|${runtimeVersion}'
var ftpsState = 'FtpsOnly'
var http20Enabled = true
var httpLoggingEnabled = true
var httpsOnly = true

resource appService 'Microsoft.Web/sites@2022-03-01' = {
  // checkov:skip=CKV_AZURE_17: Enable client certs
  name: name
  location: location
  tags: union(tags, { 'azd-service-name': serviceName })
  kind: kind
  properties: {
    serverFarmId: appServicePlanId
    reserved: true

    siteConfig: {
      linuxFxVersion: runtimeNameAndVersion
      minTlsVersion: '1.2'
      alwaysOn: alwaysOn
      ftpsState: ftpsState
      appCommandLine: appCommandLine
      numberOfWorkers: numberOfWorkers != -1 ? numberOfWorkers : null
      minimumElasticInstanceCount: minimumElasticInstanceCount != -1 ? minimumElasticInstanceCount : null
      use32BitWorkerProcess: use32BitWorkerProcess
      functionAppScaleLimit: functionAppScaleLimit != -1 ? functionAppScaleLimit : null
      healthCheckPath: healthCheckPath
      http20Enabled: http20Enabled
      httpLoggingEnabled: httpLoggingEnabled

      cors: {
        allowedOrigins: union([ 'https://portal.azure.com', 'https://ms.portal.azure.com' ], allowedOrigins)
      }
      appSettings: union(appSettings, [
          {
            name: 'APPINSIGHTS_INSTRUMENTATIONKEY'
            value: applicationInsights.properties.InstrumentationKey
          }
          {
            name: 'APPLICATIONINSIGHTS_CONNECTION_STRING'
            value: applicationInsights.properties.ConnectionString
          }
          {
            name: 'SCM_DO_BUILD_DURING_DEPLOYMENT'
            value: string(scmDoBuildDuringDeployment)
          }
        ])
    }

    clientAffinityEnabled: clientAffinityEnabled
    httpsOnly: httpsOnly
    clientCertMode: 'Optional'
    // clientCertEnabled: true
  }

  identity: {
    type: 'SystemAssigned'
  }

  resource configLogs 'config' = {
    // checkov:skip=CKV_AZURE_13: Azure App Service Web app authentication is Off
    name: 'logs'
    properties: {
      applicationLogs: { fileSystem: { level: 'Verbose' } }
      detailedErrorMessages: { enabled: true }
      failedRequestsTracing: { enabled: true }
      httpLogs: { fileSystem: { enabled: true, retentionInDays: 1, retentionInMb: 35 } }
    }
  }
}

// resource keyVault 'Microsoft.KeyVault/vaults@2022-07-01' existing = if (!(empty(keyVaultName))) {
//   name: keyVaultName
// }

resource applicationInsights 'Microsoft.Insights/components@2020-02-02' existing = if (!empty(applicationInsightsName)) {
  name: applicationInsightsName
}

output id string = appService.id
output identityPrincipalId string = appService.identity.principalId
output name string = appService.name
output uri string = 'https://${appService.properties.defaultHostName}'
