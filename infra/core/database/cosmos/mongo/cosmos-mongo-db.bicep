param accountName string
param databaseName string
param location string = resourceGroup().location
param tags object = {}

param collections array = []

module cosmos 'cosmos-mongo-account.bicep' = {
  name: 'cosmos-mongo-account'
  params: {
    name: accountName
    location: location
    tags: tags
  }
}

resource database 'Microsoft.DocumentDB/databaseAccounts/mongodbDatabases@2022-08-15' = {
  name: '${accountName}/${databaseName}'
  tags: tags
  properties: {
    resource: { id: databaseName }
  }

  resource list 'collections' = [for collection in collections: {
    name: collection.name
    properties: {
      resource: {
        id: collection.id
        shardKey: { _id: collection.shardKey }
        indexes: [ { key: { keys: [ collection.indexKey ] } } ]
      }
    }
  }]

  dependsOn: [
    cosmos
  ]
}

output databaseName string = databaseName
output endpoint string = cosmos.outputs.endpoint
