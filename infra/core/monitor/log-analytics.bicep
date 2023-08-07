

@minLength(4)
@maxLength(63)
param name string
param location string = resourceGroup().location
param tags object
param skuName string

resource log_analytics 'Microsoft.OperationalInsights/workspaces@2022-10-01' = {
  name:   name
  location: location
  tags: tags
   
  properties: {
    sku: {
      name: skuName  
    }
    retentionInDays: 90
    publicNetworkAccessForIngestion: 'Enabled'
    publicNetworkAccessForQuery: 'Enabled'
  }

  identity: {
    type: 'SystemAssigned'
  }
}

output id string = log_analytics.id
output name string = log_analytics.name

