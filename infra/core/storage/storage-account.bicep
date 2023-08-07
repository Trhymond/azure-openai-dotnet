@minLength(3)
@maxLength(24)
param name string
param location string = resourceGroup().location
param tags object
@allowed([ 'Hot', 'Cool', 'Premium' ])
param accessTier string = 'Hot'
@allowed([ 'Premium_LRS', 'Premium_ZRS', 'Standard_GRS', 'Standard_GZRS', 'Standard_LRS', 'Standard_RAGRS', 'Standard_RAGZRS', 'Standard_ZRS' ])
param skuName string = 'Standard_LRS'

param containers array = []
param deleteRetentionPolicy object = {}

resource storage 'Microsoft.Storage/storageAccounts@2022-09-01' = {
  name:   name
  location: location
  tags: tags
  kind: 'StorageV2'
  sku: {
    name: skuName
  }
  properties: {
    accessTier: accessTier
    allowBlobPublicAccess: true
    allowCrossTenantReplication: true
    allowSharedKeyAccess: true
    supportsHttpsTrafficOnly: true
    defaultToOAuthAuthentication: false
    dnsEndpointType: 'Standard'
    minimumTlsVersion: 'TLS1_2'
    networkAcls: {
      bypass: 'AzureServices'
      defaultAction: 'Allow'
    }
    publicNetworkAccess: 'Enabled'

    routingPreference: {
      publishInternetEndpoints: true
      publishMicrosoftEndpoints: false
    }
  }

  identity: {
    type: 'SystemAssigned'
  }

  resource blobServices 'blobServices' = if (!empty(containers)) {
    name: 'default'
    properties: {
      deleteRetentionPolicy: deleteRetentionPolicy
      lastAccessTimeTrackingPolicy: {
        enable: true
      }
    }
    resource container 'containers' = [for container in containers: {
      name: container.name
      properties: {
        publicAccess: contains(container, 'publicAccess') ? container.publicAccess : 'None'
      }
    }]
  }
}

output id string = storage.id
output name string = storage.name
output primaryEndpoints object = storage.properties.primaryEndpoints
