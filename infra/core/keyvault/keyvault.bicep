@minLength(3)
@maxLength(24)
param name string
param location string = resourceGroup().location
param tags object
@allowed([ 'standard', 'premium' ])
param skuName string = 'standard'
param servicePrincipalObjectId string

resource keyvault 'Microsoft.KeyVault/vaults@2022-07-01' = {
  name: name
  location: location
  tags: tags

  properties: {
    sku: {
      family: 'A'
      name: skuName
    }
    softDeleteRetentionInDays: 90
    enablePurgeProtection: true
    enabledForDiskEncryption: true
    enabledForDeployment: true
    enableSoftDelete: true
    tenantId: subscription().tenantId
    // networkAcls: {
    //   bypass: 'AzureServices'
    //   defaultAction: 'Deny'
    // }
    publicNetworkAccess: 'enabled'
    // enableRbacAuthorization: true
    accessPolicies: [
      {
        objectId: servicePrincipalObjectId
        tenantId: subscription().tenantId
        permissions: {
          secrets: [ 'Get', 'Set', 'List', 'Delete', 'Backup', 'Recover', 'Restore', 'Purge' ]
          // keys: [ 'Get', 'List', 'Update', 'Create', 'Import', 'Delete', 'Recover', 'Backup', 'Restore' ]
          // certificates: [ 'Get', 'List', 'Update', 'Create', 'Import', 'Delete', 'Recover', 'Backup', 'Restore' ]
        }
      }
    ]
  }
}

output id string = keyvault.id
output name string = keyvault.name
output endpoint string = keyvault.properties.vaultUri
