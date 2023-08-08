@minLength(3)
@maxLength(60)
param name string
param location string = resourceGroup().location
param tags object
@allowed([ 'basic', 'free', 'standard', 'standard2', 'standard3', 'storage_optimized_l1', 'storage_optimized_l2' ])
param skuName string = 'standard'

@allowed([
  'default'
  'highDensity'
])
param hostingMode string = 'default'

resource search 'Microsoft.Search/searchServices@2022-09-01' = {
  name: name
  location: location
  tags: tags
  sku: {
    name: skuName
  }
  properties: {
    authOptions: {
      aadOrApiKey: {
        aadAuthFailureMode: 'http401WithBearerChallenge'
      }
    }
    disableLocalAuth: false
    hostingMode: hostingMode
    partitionCount: 1
    publicNetworkAccess: 'enabled'
    replicaCount: 1
  }

  identity: {
    type: 'SystemAssigned'
  }
}

output id string = search.id
output endpoint string = 'https://${name}.search.windows.net/'
output name string = search.name
