param keyvaultName string
param permissions object = {
  secrets: [ 'get', 'list' ]
}
param principalId string

resource keyVault 'Microsoft.KeyVault/vaults@2022-07-01' existing = {
  name: keyvaultName
}

resource keyVaultAccessPolicies 'Microsoft.KeyVault/vaults/accessPolicies@2022-07-01' = {
  parent: keyVault
  name: 'add'
  properties: {
    accessPolicies: [
      {
        objectId: principalId
        tenantId: subscription().tenantId
        permissions: permissions
      }
    ]
  }
}
