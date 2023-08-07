@minLength(3)
@maxLength(60)
param name string
param location string = resourceGroup().location
param tags object
param kind string = 'linux'
param skuName string = 'Free'
param skuTier string = 'Free'
param repositoryUrl string
param branch string
@secure()
param repositoryToken string
param appLocation string
param apiLocation string
param appArtifactLocation string
param appSettings object

resource frontend 'Microsoft.Web/staticSites@2022-03-01' = {
  name: name
  location: location
  tags: tags
  sku: {
    name: skuName
    tier: skuTier
  }
  kind: kind

  properties: {
    provider: 'github'
    repositoryUrl: repositoryUrl
    repositoryToken: repositoryToken
    branch:  branch
    buildProperties: {
      appLocation: appLocation
      apiLocation: apiLocation
      appArtifactLocation: appArtifactLocation
      skipGithubActionWorkflowGeneration: false
    }

    allowConfigFileUpdates: true
    publicNetworkAccess: 'Enabled'
  }

  identity: {
    type: 'SystemAssigned'
  }
}

resource name_appsettings 'Microsoft.Web/staticSites/config@2022-03-01' = {
  parent: frontend
  name: 'appsettings'
  properties: appSettings
}

output name string = frontend.name
output uri string = 'https://${web.properties.defaultHostname}'
output deploymentToken string = frontend.listSecrets().properties.apiKey

