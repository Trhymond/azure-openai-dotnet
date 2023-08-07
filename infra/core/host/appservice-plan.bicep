@minLength(3)
@maxLength(40)
param name string
param location string = resourceGroup().location
param tags object
param kind string
param reserved bool = true
param skuName string
param skuCapacity int
param zoneRedundant bool = false

resource appServicePlan 'Microsoft.Web/serverfarms@2022-03-01' = {
  name: name
  location: location
  tags: tags
  sku: {
    name: skuName
    capacity: skuCapacity
  }
  kind: kind
  properties: {
    reserved: reserved
    perSiteScaling: true
    zoneRedundant: zoneRedundant
  }
}

output id string = appServicePlan.id
output name string = appServicePlan.name
