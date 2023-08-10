@minLength(3)
@maxLength(60)
param name string
param hostingPlanName string = ''
param location string = resourceGroup().location
param tags object
param kind string = 'functionapp,linux'
param applicationInsightsName string
param keyVaultName string = ''
param storageAccountName string
param zoneRedundant bool = false
param functionAppScaleLimit int = 100
param allowedOrigins array = []
param appSettings array = []
param serviceName string = 'backend'
param runtimeName string
param runtimeVersion string
param alwaysOn bool = false
param use32BitWorkerProcess bool = false
param numberOfWorkers int = 1
param minimumElasticInstanceCount int = 0

var hostingPlanNameVar = empty(hostingPlanName) ? replace(name, 'func-', 'func-plan-') : hostingPlanName
var runtimeNameAndVersion = '${toUpper(runtimeName)}|${runtimeVersion}'
var scmDoBuildDuringDeployment = contains(kind, 'linux') ? true : false
var enableOryxBuild = contains(kind, 'linux') ? true : false
var websiteRunFromPackage = contains(kind, 'linux') ? 1 : 0

resource storageAccount 'Microsoft.Storage/storageAccounts@2022-09-01' existing = if (!empty(storageAccountName)) {
  name: storageAccountName
  scope: resourceGroup()
}

resource applicationInsights 'Microsoft.Insights/components@2020-02-02' existing = if (!empty(applicationInsightsName)) {
  name: applicationInsightsName
  scope: resourceGroup()
}

resource keyVault 'Microsoft.KeyVault/vaults@2022-07-01' existing = if (!(empty(keyVaultName))) {
  name: keyVaultName
  scope: resourceGroup()
}

resource hostingPlan 'Microsoft.Web/serverfarms@2022-03-01' = {
  name: hostingPlanNameVar
  location: location
  tags: tags
  kind: kind
  sku: {
    name: 'Y1'
    tier: 'Dynamic'
    size: 'Y1'
    family: 'Y'
  }

  properties: {
    reserved: true
    zoneRedundant: zoneRedundant
  }
}

resource functionApp 'Microsoft.Web/sites@2022-03-01' = {
  name: name
  location: location
  tags: union(tags, { 'azd-service-name': serviceName })
  kind: kind
  properties: {
    reserved: true
    serverFarmId: hostingPlan.id

    siteConfig: {
      linuxFxVersion: runtimeNameAndVersion
      alwaysOn: alwaysOn
      // appCommandLine: appCommandLine
      numberOfWorkers: numberOfWorkers < 0 ? 1 : numberOfWorkers
      minimumElasticInstanceCount: minimumElasticInstanceCount < 0 ? 0 : minimumElasticInstanceCount
      ftpsState: 'FtpsOnly'
      use32BitWorkerProcess: use32BitWorkerProcess
      minTlsVersion: '1.2'
      functionAppScaleLimit: functionAppScaleLimit != -1 ? functionAppScaleLimit : null
      // healthCheckPath: healthCheckPath
      http20Enabled: false

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
          // {
          //   name: 'AzureWebJobsFeatureFlags'
          //   value: 'EnableWorkerIndexing'
          // }
          {
            name: 'AzureWebJobsStorage'
            value: 'DefaultEndpointsProtocol=https;AccountName=${storageAccount.name};EndpointSuffix=${environment().suffixes.storage};AccountKey=${listKeys(storageAccount.id, storageAccount.apiVersion).keys[0].value}'
          }
          {
            name: 'FUNCTIONS_EXTENSION_VERSION'
            value: '~4'
          }
          {
            name: 'FUNCTIONS_WORKER_RUNTIME'
            value: toLower(runtimeName)
          }
          {
            name: 'WEBSITE_CONTENTAZUREFILECONNECTIONSTRING'
            value: 'DefaultEndpointsProtocol=https;AccountName=${storageAccount.name};EndpointSuffix=${environment().suffixes.storage};AccountKey=${listKeys(storageAccount.id, storageAccount.apiVersion).keys[0].value}'
          }
          {
            name: 'WEBSITE_CONTENTSHARE'
            value: name
          }
          {
            name: 'WEBSITE_HTTPLOGGING_RETENTION_DAYS'
            value: '1'
          }
          {
            name: 'ENABLE_ORYX_BUILD'
            value: string(enableOryxBuild)
          }
          {
            name: 'SCM_DO_BUILD_DURING_DEPLOYMENT'
            value: string(scmDoBuildDuringDeployment)
          }
          // {
          //   name: 'WEBSITE_RUN_FROM_PACKAGE'
          //   value: string(websiteRunFromPackage)
          // }
        ])
    }
    httpsOnly: true

    // clientAffinityEnabled: clientAffinityEnabled
  }

  identity: {
    type: 'SystemAssigned'
  }

  resource configLogs 'config' = {
    name: 'logs'
    properties: {
      applicationLogs: { fileSystem: { level: 'Verbose' } }
      detailedErrorMessages: { enabled: true }
      failedRequestsTracing: { enabled: true }
      httpLogs: { fileSystem: { enabled: true, retentionInDays: 1, retentionInMb: 35 } }
    }
  }
}

resource secret 'Microsoft.KeyVault/vaults/secrets@2022-07-01' = {
  parent: keyVault
  name: '${name}-key'
  properties: {
    value: listkeys('${functionApp.id}/host/default', '2022-03-01').masterKey //functionAppHost.listKeys().masterKey
    contentType: 'function app key'

    attributes: {
      enabled: true
      exp: 0
    }
  }
}

output id string = functionApp.id
output identityPrincipalId string = functionApp.identity.principalId
output name string = functionApp.name
output uri string = 'https://${functionApp.properties.defaultHostName}'
output functionKeyName string = '${name}-key'

#disable-next-line outputs-should-not-contain-secrets
output functionKey string = listkeys('${functionApp.id}/host/default', '2022-03-01').masterKey
//https://stackoverflow.com/questions/59880218/azure-functions-getting-403-error-while-accessing-the-storage-account
