targetScope = 'subscription'

@minLength(1)
@maxLength(4)
@description('Name of the the environment which is used to generate a short unique hash used in all resources.')
param environmentName string

@minLength(1)

@description('Primary location for all resources')
param location string

@description('The resouce name prefix to generate names if prvided resource name is blank')
param namePrefix string

@description('Default Tags for all resources')
param tags object

param servicePrincipalObjectId string

@description('The resource group name')
param resourceGroupName string

@description('The storage account name')
param storageAccountName string

@description('The storage account access tier')
param storageAccessTier string

@description('The storage account sku name')
param storageSkuName string

@description('The storage account container name')
param storageContainerName string

@description('The keyvault name')
param keyVaultName string

@description('The keyvault sku name')
param keyvaultSkuName string

@description('The log analytics workspace name')
param logAnalyticsWorkspaceName string

@description('The log analytics sku name')
param logAnalyticsSkuName string

@description('The appinsgights name')
param appinsightsName string

@description('The search service name')
param searchServiceName string

@description('The search service sku name')
param searchServiceSkuName string = 'standard'

@description('The search index name')
param searchIndexName string

@description('The form recognizer service name')
param formRecognizerServiceName string

@description('The form recognizer service sku name')
param formRecognizerSkuName string = 'S0'

@description('The openai service name')
param openAiServiceName string

@description('The openapi service sku name')
param openAiSkuName string = 'S0'

@description('The openapi GPT deployment name')
param gptDeploymentName string = 'davinci'

@description('The openapi GPT deployment capacity')
param gptDeploymentCapacity int

@description('The openapi GPR model name')
param gptModelName string

@description('The openapi chatGPT deployment name')
param chatGptDeploymentName string = 'chat'

@description('The openapi chatGPT deployment capcity')
param chatGptDeploymentCapacity int

@description('The openapi chatGPT model name')
param chatGptModelName string

@description('The cosmos db account name')
param cosmosAccountName string

@description('The cosmos db name')
param cosmosDbName string = 'employee_db'

@description('The function app name')
param functionAppName string

@description('The app service plan name')
param appServicePlanName string

@description('The app service name')
param appServiceName string

// Variables
var locationShortNameVar = 'eus'
var resourceGroupNameVar = empty(resourceGroupName) ? 'rg-${namePrefix}-${environmentName}-${locationShortNameVar}' : resourceGroupName

// Resources
resource resourceGroup 'Microsoft.Resources/resourceGroups@2021-04-01' = {
  name: resourceGroupNameVar
  location: location
  tags: tags
}

module azname 'core/aznames/aznames.bicep' = {
  name: 'aznames-module'
  scope: resourceGroup
  params: {
    resourceTypes: [ 'storage_account', 'key_vault', 'application_insights', 'log_analytics_workspace', 'search_service', 'form_recognizer_service', 'openai_service', 'cosmosdb_service', 'function_app', 'app_service_plan', 'app_service' ]
    namePrefix: namePrefix
    environment: environmentName
    location: location
  }
}

var storageAccountNameVar = empty(storageAccountName) ? first(filter(azname.outputs.resourceNames, item => item.resourceType == 'storage_account')).resourceName : storageAccountName
var keyVaultNameVar = empty(keyVaultName) ? first(filter(azname.outputs.resourceNames, item => item.resourceType == 'key_vault')).resourceName : keyVaultName
var logAnalyticsWorkspaceNameVar = empty(logAnalyticsWorkspaceName) ? first(filter(azname.outputs.resourceNames, item => item.resourceType == 'log_analytics_workspace')).resourceName : logAnalyticsWorkspaceName
var appinsightsNameVar = empty(appinsightsName) ? first(filter(azname.outputs.resourceNames, item => item.resourceType == 'application_insights')).resourceName : appinsightsName
var searchServiceNameVar = empty(searchServiceName) ? first(filter(azname.outputs.resourceNames, item => item.resourceType == 'search_service')).resourceName : searchServiceName
var formRecognizerServiceNameVar = empty(formRecognizerServiceName) ? first(filter(azname.outputs.resourceNames, item => item.resourceType == 'form_recognizer_service')).resourceName : formRecognizerServiceName
var openAiServiceNameVar = empty(openAiServiceName) ? first(filter(azname.outputs.resourceNames, item => item.resourceType == 'openai_service')).resourceName : openAiServiceName
var cosmosAccountNameVar = empty(cosmosAccountName) ? first(filter(azname.outputs.resourceNames, item => item.resourceType == 'cosmosdb_service')).resourceName : cosmosAccountName
var functionAppNameVar = empty(functionAppName) ? first(filter(azname.outputs.resourceNames, item => item.resourceType == 'function_app')).resourceName : functionAppName
var appServicePlanNameVar = empty(appServicePlanName) ? first(filter(azname.outputs.resourceNames, item => item.resourceType == 'app_service_plan')).resourceName : appServicePlanName
var appServiceNameVar = empty(appServiceName) ? first(filter(azname.outputs.resourceNames, item => item.resourceType == 'app_service')).resourceName : appServiceName

module storage 'core/storage/storage-account.bicep' = {
  name: 'storage-module'
  scope: resourceGroup
  params: {
    name: storageAccountNameVar
    location: location
    tags: tags
    accessTier: storageAccessTier
    skuName: storageSkuName
    deleteRetentionPolicy: {
      enabled: true
      days: 2
    }
    containers: [
      {
        name: storageContainerName
        publicAccess: 'None'
      }
    ]
  }
}

module keyvault 'core/keyvault/keyvault.bicep' = {
  name: 'keyvault-module'
  scope: resourceGroup
  params: {
    name: keyVaultNameVar
    location: location
    tags: tags
    skuName: keyvaultSkuName
    servicePrincipalObjectId: servicePrincipalObjectId
  }
}

module log_analytics 'core/monitor/log-analytics.bicep' = {
  name: 'loganalytics-module'
  scope: resourceGroup
  params: {
    name: logAnalyticsWorkspaceNameVar
    location: location
    tags: tags
    skuName: logAnalyticsSkuName
  }
}

module appInsights 'core/monitor/appinsights.bicep' = {
  name: 'appinsights-module'
  scope: resourceGroup
  params: {
    name: appinsightsNameVar
    location: location
    tags: tags
    logAnalyticsWorkspaceId: log_analytics.outputs.id
  }

  dependsOn: [ log_analytics ]
}

module search 'core/search/search-service.bicep' = {
  name: 'search-module'
  scope: resourceGroup
  params: {
    name: searchServiceNameVar
    location: location
    tags: tags
    skuName: searchServiceSkuName
  }
}

module formRecognizer 'core/ai/cognitive-services.bicep' = {
  name: 'formrecognizer-module'
  scope: resourceGroup
  params: {
    name: formRecognizerServiceNameVar
    kind: 'FormRecognizer'
    location: location
    tags: tags
    skuName: formRecognizerSkuName
  }
}

module openAi 'core/ai/cognitive-services.bicep' = {
  name: 'openai'
  scope: resourceGroup
  params: {
    name: openAiServiceNameVar
    location: location
    tags: tags
    skuName: openAiSkuName
    kind: 'OpenAI'

    deployments: [
      {
        name: gptDeploymentName
        model: {
          format: 'OpenAI'
          name: gptModelName
          version: '2'
        }
        sku: {
          name: 'Standard'
          capacity: gptDeploymentCapacity
        }
      }
      {
        name: chatGptDeploymentName
        model: {
          format: 'OpenAI'
          name: chatGptModelName
          version: '0301'
        }
        sku: {
          name: 'Standard'
          capacity: chatGptDeploymentCapacity
        }
      }
    ]
  }
}

module database 'core/database/cosmos/sql/cosmos-sql-db.bicep' = {
  name: 'database-module'
  scope: resourceGroup
  params: {
    accountName: cosmosAccountNameVar
    databaseName: cosmosDbName
    location: location
    tags: tags
    containers: [ {
        id: 'employee_plans'
        name: 'employee_plans'
        partitionKey: '/plan'
      } ]
    keyVaultName: keyvault.outputs.name
    principalIds: [ servicePrincipalObjectId ]
  }
}

module backend 'core/host/funcApp.bicep' = {
  name: 'funcapp-module'
  scope: resourceGroup
  params: {
    name: functionAppNameVar
    location: location
    tags: tags
    kind: 'linux'
    runtimeName: 'dotnetcore'
    runtimeVersion: '7.0'
    applicationInsightsName: appInsights.outputs.name
    keyVaultName: keyvault.outputs.name
    storageAccountName: storage.outputs.name
    serviceName: 'backend'
    alwaysOn: false
    // allowedOrigins: [ frontend.outputs.uri ]
    appSettings: [
      {
        name: 'AZURE_STORAGE_ACCOUNT'
        value: storage.outputs.name
      }
      {
        name: 'AZURE_STORAGE_CONTAINER'
        value: storageContainerName
      }
      {
        name: 'AZURE_SEARCH_INDEX'
        value: searchIndexName
      }
      {
        name: 'AZURE_SEARCH_SERVICE'
        value: search.outputs.name
      }
      {
        name: 'AZURE_OPENAI_SERVICE'
        value: openAi.outputs.name
      }
      {
        name: 'AZURE_OPENAI_GPT_DEPLOYMENT'
        value: gptDeploymentName
      }
      {
        name: 'AZURE_OPENAI_CHATGPT_DEPLOYMENT'
        value: chatGptDeploymentName
      }
      {
        name: 'AZURE_COSMOS_ENDPOINT'
        value: database.outputs.endpoint
      }
    ]
  }
  dependsOn: [
    storage
    keyvault
    appInsights
    database
    openAi
    search
  ]
}

module appServicePlan 'core/host/appservice-plan.bicep' = {
  name: 'appserviceplan-module'
  scope: resourceGroup
  params: {
    name: appServicePlanNameVar
    location: location
    tags: tags
    skuName: 'B1'
    skuCapacity: 1
    kind: 'linux'
  }
  dependsOn: [
    backend
  ]
}

module frontend 'core/host/appservice.bicep' = {
  name: 'web-module'
  scope: resourceGroup
  params: {
    name: appServiceNameVar
    location: location
    tags: union(tags, { 'azd-service-name': 'frontend' })
    appServicePlanId: appServicePlan.outputs.id
    // keyVaultName: keyvault.outputs.name
    applicationInsightsName: appInsights.outputs.name
    runtimeName: 'NODE'
    runtimeVersion: '18-lts'
    serviceName: 'frontend'
    minimumElasticInstanceCount: 0

    appSettings: [
      {
        name: 'WEBSITE_NODE_DEFAULT_VERSION'
        value: '~18'
      }
    ]
    appCommandLine: 'pm2 serve /home/site/wwwroot --no-daemon --spa'
  }
  dependsOn: [
    appServicePlan
    appInsights
    backend
  ]
}

// Security

// Storage Blob Data Reader
module storageRoleUser 'core/security/role-assignment.bicep' = {
  scope: resourceGroup
  name: 'storage-role-user'
  params: {
    principalId: servicePrincipalObjectId
    roleDefinitionId: '2a2b9908-6ea1-4ae2-8e65-a410df84e7d1'
    principalType: 'User'
  }
}

// Storage Blob Data Contributor
module storageContribRoleUser 'core/security/role-assignment.bicep' = {
  scope: resourceGroup
  name: 'storage-contribrole-user'
  params: {
    principalId: servicePrincipalObjectId
    roleDefinitionId: 'ba92f5b4-2d11-453d-a403-e96b0029c9fe'
    principalType: 'User'
  }
}

// Search Index Data Reader
module searchRoleUser 'core/security/role-assignment.bicep' = {
  scope: resourceGroup
  name: 'search-role-user'
  params: {
    principalId: servicePrincipalObjectId
    roleDefinitionId: '1407120a-92aa-4202-b7e9-c0e197c71c8f'
    principalType: 'User'
  }
}

// Search Index Data Contributor
module searchContribRoleUser 'core/security/role-assignment.bicep' = {
  scope: resourceGroup
  name: 'search-contrib-role-user'
  params: {
    principalId: servicePrincipalObjectId
    roleDefinitionId: '8ebe5a00-799e-43f5-93ac-243d3dce84a7'
    principalType: 'User'
  }
}

// Search Service Contributor
module searchSvcContribRoleUser 'core/security/role-assignment.bicep' = {
  scope: resourceGroup
  name: 'search-svccontrib-role-user'
  params: {
    principalId: servicePrincipalObjectId
    roleDefinitionId: '7ca78c08-252a-4471-8644-bb5ff32d4ba0'
    principalType: 'User'
  }
}

// Cognitive Services OpenAI User
module openAiRoleUser 'core/security/role-assignment.bicep' = {
  scope: resourceGroup
  name: 'openai-role-user'
  params: {
    principalId: servicePrincipalObjectId
    roleDefinitionId: '5e0bd9bd-7b93-4f28-af87-19fc36ad61bd'
    principalType: 'User'
  }
}

// Cognitive Services User
module formRecognizerRoleUser 'core/security/role-assignment.bicep' = {
  scope: resourceGroup
  name: 'formrecognizer-role-user'
  params: {
    principalId: servicePrincipalObjectId
    roleDefinitionId: 'a97b65f3-24c7-4388-baec-2e87135dc908'
    principalType: 'User'
  }
}

//DocumentDB Account Contributor
module cosmosRoleUser 'core/security/role-assignment.bicep' = {
  scope: resourceGroup
  name: 'cosmos-role-user'
  params: {
    principalId: servicePrincipalObjectId
    roleDefinitionId: '5bd9cd88-fe45-4216-938b-f97437e15450'
    principalType: 'User'
  }
}

// Cognitive Services OpenAI User
module openAiRoleBackend 'core/security/role-assignment.bicep' = {
  scope: resourceGroup
  name: 'openai-role-backend'
  params: {
    principalId: backend.outputs.identityPrincipalId
    roleDefinitionId: '5e0bd9bd-7b93-4f28-af87-19fc36ad61bd'
    principalType: 'ServicePrincipal'
  }
  dependsOn: [
    backend
  ]
}

// Storage Blob Data Reader
module storageRoleBackend 'core/security/role-assignment.bicep' = {
  scope: resourceGroup
  name: 'storage-role-backend'
  params: {
    principalId: backend.outputs.identityPrincipalId
    roleDefinitionId: '2a2b9908-6ea1-4ae2-8e65-a410df84e7d1'
    principalType: 'ServicePrincipal'
  }
  dependsOn: [
    backend
  ]
}

// Search Index Data Reader
module searchRoleBackend 'core/security/role-assignment.bicep' = {
  scope: resourceGroup
  name: 'search-role-backend'
  params: {
    principalId: backend.outputs.identityPrincipalId
    roleDefinitionId: '1407120a-92aa-4202-b7e9-c0e197c71c8f'
    principalType: 'ServicePrincipal'
  }
  dependsOn: [
    backend
  ]
}

// Cosmos DB Account Reader Role	
module cosmosRoleBackend 'core/security/role-assignment.bicep' = {
  scope: resourceGroup
  name: 'cosmos-role-backend'
  params: {
    principalId: backend.outputs.identityPrincipalId
    roleDefinitionId: 'fbdf93bf-df7d-467e-a4d2-9458aa1360c8'
    principalType: 'ServicePrincipal'
  }
  dependsOn: [
    backend
  ]
}

// Outputs
output AZURE_LOCATION string = location
output AZURE_TENANT_ID string = subscription().tenantId
output AZURE_SUBSCRIPTION_ID string = subscription().subscriptionId
output AZURE_SERVICEPRINCIPAL_OBJECTID string = servicePrincipalObjectId
output AZURE_RESOURCE_GROUP string = resourceGroup.name
output AZURE_STORAGE_ACCOUNT string = storageAccountNameVar
output AZURE_STORAGE_CONTAINER string = storageContainerName
output AZURE_KEY_VAULT string = keyVaultNameVar
output AZURE_LOG_ANALYTICS_WORKSPACE string = logAnalyticsWorkspaceNameVar
output AZURE_APPINSIGHTS string = appinsightsNameVar
output AZURE_SEARCH_SERVICE string = searchServiceNameVar
output AZURE_FORM_RECOGNIZER_SERVICE string = formRecognizerServiceNameVar
output AZURE_OPENAI_SERVICE string = openAiServiceNameVar
output AZURE_OPENAI_GPT_DEPLOYMENT string = gptDeploymentName
output AZURE_OPENAI_CHATGPT_DEPLOYMENT string = chatGptDeploymentName
output AZURE_COSMOS_ACCOUNT string = cosmosAccountNameVar
output AZURE_FUNCTION_APP string = functionAppNameVar
output AZURE_APP_SERVICE_PLAN string = appServicePlanNameVar
output AZURE_APP_SERVICE string = appServiceNameVar
output AZURE_SEARCH_INDEX string = searchIndexName
output FRONTEND_URI string = frontend.outputs.uri
output BACKEND_URI string = backend.outputs.uri
output BACKEND_KEY string = backend.outputs.functionKey
output COSMOS_ACCOUNT_URI string = database.outputs.endpoint
output COSMOS_DATABASE string = database.outputs.databaseName
output COSMOS_CONTAINERS string = database.outputs.containers
