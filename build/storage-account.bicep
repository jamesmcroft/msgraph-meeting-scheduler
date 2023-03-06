@description('The name for the Storage Account.')
param name string
@description('The location for the Storage Account. Defaults to the location of the resource group.')
param location string = resourceGroup().location
@description('The SKU name for the Storage Account. Defaults to Standard_LRS.')
@allowed([
    'Premium_LRS'
    'Premium_ZRS'
    'Standard_GRS'
    'Standard_GZRS'
    'Standard_LRS'
    'Standard_RAGRS'
    'Standard_RAGZRS'
    'Standard_ZRS'
])
param skuName string = 'Standard_LRS'
@description('The blob storage access tier. If the skuName is a premium SKU name, this will be ignored. Defaults to Hot.')
@allowed([
    'Hot'
    'Cool'
])
param blobAccessTier string = 'Hot'
@description('The tags for the Storage Account.')
param tags object = {}

resource storageAccount 'Microsoft.Storage/storageAccounts@2022-05-01' = {
    name: name
    location: location
    tags: tags
    kind: 'StorageV2'
    sku: {
        name: skuName
    }
    properties: {
        accessTier: startsWith(skuName, 'Premium') ? 'Premium' : blobAccessTier
        networkAcls: {
            bypass: 'AzureServices'
            defaultAction: 'Allow'
        }
        supportsHttpsTrafficOnly: true
        encryption: {
            services: {
                blob: {
                    enabled: true
                }
                file: {
                    enabled: true
                }
            }
            keySource: 'Microsoft.Storage'
        }
    }
}

@description('The resource ID of the storage account resource.')
output storageAccountId string = storageAccount.id
@description('The connection string of the storage account resource.')
output storageAccountConnectionString string = 'DefaultEndpointsProtocol=https;AccountName=${storageAccount.name};EndpointSuffix=${environment().suffixes.storage};AccountKey=${listKeys(storageAccount.id, storageAccount.apiVersion).keys[0].value}'
