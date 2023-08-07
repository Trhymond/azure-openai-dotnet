// param name string
// param location string = resourceGroup().location
// param tags object = {}

// param customSubDomainName string = name

// param kind string = 'OpenAI'
// param publicNetworkAccess string = 'Enabled'
// param sku object = {
//   name: 'S0'
// }

@minLength(3)
@maxLength(64)
param name string
param location string = resourceGroup().location
param tags object
@allowed(['CognitiveServices','ComputerVision', 'CustomVision.Prediction','CustomVision.Training','Face', 'FormRecognizer','SpeechServices','LUIS','QnAMaker', 'TextAnalytics', 'TextTranslation', 'AnomalyDetector', 'ContentModerator', 'Personalizer', 'OpenAI' ])
param kind string ='CognitiveServices'
param skuName string
param customSubDomainName string = name
param publicNetworkAccess string = 'enabled'
param deployments array = []

resource account 'Microsoft.CognitiveServices/accounts@2023-05-01' = {
  name: name
  location: location
  tags: tags
  kind: kind
  properties: {
    customSubDomainName: customSubDomainName
    publicNetworkAccess: publicNetworkAccess
  }
  sku: {
    name: skuName
  }

  identity: {
    type: 'SystemAssigned'
  }
}

@batchSize(1)
resource deployment 'Microsoft.CognitiveServices/accounts/deployments@2023-05-01' = [for deployment in deployments: {
  parent: account
  name: deployment.name
  properties: {
    model: deployment.model
    raiPolicyName: contains(deployment, 'raiPolicyName') ? deployment.raiPolicyName : null
  }
  sku: contains(deployment, 'sku') ? deployment.sku : {
    name: 'Standard'
    capacity: 20
  }
}]


output id string = account.id
output name string = account.name
output endpoint string = account.properties.endpoint
