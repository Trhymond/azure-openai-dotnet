param name string
param location string = resourceGroup().location
param tags object

param connectionStringKey string = 'AZURE-COSMOS-CONNECTION-STRING'
param keyVaultName string

@allowed([ 'GlobalDocumentDB', 'MongoDB', 'Parse' ])
param kind string

param baseTime string = utcNow('u')

resource cosmos 'Microsoft.DocumentDB/databaseAccounts@2022-08-15' = {
  // checkov:skip=CKV_AZURE_132: LOW: Ensure cosmosdb does not allow privileged escalation by restricting management plane changes
  name: name
  kind: kind
  location: location
  tags: tags
  properties: {
    consistencyPolicy: { defaultConsistencyLevel: 'Session' }
    locations: [
      {
        locationName: location
        failoverPriority: 0
        isZoneRedundant: false
      }
    ]
    databaseAccountOfferType: 'Standard'
    enableAutomaticFailover: false
    enableMultipleWriteLocations: false
    apiProperties: (kind == 'MongoDB') ? { serverVersion: '4.0' } : {}
    capabilities: [ { name: 'EnableServerless' } ]
  }
}

resource cosmosConnectionString 'Microsoft.KeyVault/vaults/secrets@2022-07-01' = {
  parent: keyVault
  name: connectionStringKey
  properties: {
    value: cosmos.listConnectionStrings().connectionStrings[0].connectionString
    contentType: 'cosmos connection string'
    attributes: {
      exp: dateTimeToEpoch(dateTimeAdd(baseTime, 'P180D'))
    }
  }
}

resource keyVault 'Microsoft.KeyVault/vaults@2022-07-01' existing = {
  name: keyVaultName
}

output connectionStringKey string = connectionStringKey
output endpoint string = cosmos.properties.documentEndpoint
output id string = cosmos.id
output name string = cosmos.name
