@minLength(10)
@maxLength(260)
param name string
param location string = resourceGroup().location
param tags object
param logAnalyticsWorkspaceId string

resource appinsights 'Microsoft.Insights/components@2020-02-02' = {
  name: name
  location: location
  tags: tags
  kind: 'web'
  properties: {
    Application_Type: 'web'
    WorkspaceResourceId: logAnalyticsWorkspaceId
    Flow_Type: 'Bluefield'
    IngestionMode: 'LogAnalytics'
    RetentionInDays: 90
    publicNetworkAccessForIngestion: 'Enabled'
    publicNetworkAccessForQuery: 'Enabled'
  }
}

output id string = appinsights.id
output name string = appinsights.name
output connectionString string = appinsights.properties.ConnectionString
