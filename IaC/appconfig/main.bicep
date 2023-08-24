@description('Specifies the name of the App Configuration store.')
param configStoreName string

@description('Specifies the Azure location where the app configuration store should be created.')
param location string

@description('Specifies the names of the key-value resources. The name is a combination of key and label with $ as delimiter. The label is optional.')
param keyValueNames array = [
  'Pod1:Settings:BackgroundColor'
  'Pod1:Settings:FontColor'
  'Pod1:Settings:FontSize'
  'Pod1:Settings:Message'
  'Pod1:Settings:Sentinel'
]

@description('Specifies the values of the key-value resources. It\'s optional')
param keyValueValues array = [
  'black'
  'white'
  '24'
  'OA was here, demo about Azure App Configuration V1.0'
  '2'
]

resource configStore 'Microsoft.AppConfiguration/configurationStores@2021-10-01-preview' = {
  name: configStoreName
  location: location
  sku: {
    name: 'standard'
  }
}

resource configStoreKeyValue 'Microsoft.AppConfiguration/configurationStores/keyValues@2021-10-01-preview' = [for (item, i) in keyValueNames: {
  parent: configStore
  name: item
  properties: {
    value: keyValueValues[i]
  }
}]
