param name string
param location string = resourceGroup().location
param tags object

@allowed([ 'GlobalDocumentDB', 'MongoDB', 'Parse' ])
param kind string

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

output endpoint string = cosmos.properties.documentEndpoint
output id string = cosmos.id
output name string = cosmos.name
