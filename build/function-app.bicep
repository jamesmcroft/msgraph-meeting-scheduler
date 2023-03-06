@description('The name for the associated App Service Plan.')
param appServicePlanName string
@description('The name for the associated Storage Account.')
param storageAccountName string
@description('The name for the Function app.')
param name string
@description('The location for the Function app. Defaults to the location of the resource group.')
param location string = resourceGroup().location
@description('The application settings for the Function app.')
param appSettings array = []
@description('The tags for the Function app.')
param tags object = {}

resource appServicePlan 'Microsoft.Web/serverfarms@2022-03-01' existing = {
    name: appServicePlanName
}

resource storageAccount 'Microsoft.Storage/storageAccounts@2022-05-01' existing = {
    name: storageAccountName
}

resource functionApp 'Microsoft.Web/sites@2022-03-01' = {
    name: name
    location: location
    tags: tags
    kind: 'functionapp,linux'
    properties: {
        reserved: true
        serverFarmId: appServicePlan.id
        httpsOnly: true
        siteConfig: {
            ftpsState: 'FtpsOnly'
            linuxFxVersion: 'DOTNET-ISOLATED|7'
            netFrameworkVersion: 'v7.0'
            appSettings: concat(appSettings, [
                    {
                        name: 'AzureWebJobsStorage'
                        value: 'DefaultEndpointsProtocol=https;AccountName=${storageAccount.name};EndpointSuffix=${environment().suffixes.storage};AccountKey=${listKeys(storageAccount.id, storageAccount.apiVersion).keys[0].value}'
                    }
                    {
                        name: 'FUNCTIONS_EXTENSION_VERSION'
                        value: '~4'
                    }
                    {
                        name: 'FUNCTIONS_WORKER_RUNTIME'
                        value: 'dotnet-isolated'
                    }
                    {
                        name: 'SCM_DO_BUILD_DURING_DEPLOYMENT'
                        value: '0'
                    }
                    {
                        name: 'WEBSITE_CONTENTAZUREFILECONNECTIONSTRING'
                        value: 'DefaultEndpointsProtocol=https;AccountName=${storageAccount.name};EndpointSuffix=${environment().suffixes.storage};AccountKey=${listKeys(storageAccount.id, storageAccount.apiVersion).keys[0].value}'
                    }
                    {
                        name: 'WEBSITE_CONTENTSHARE'
                        value: toLower(name)
                    }
                ])
        }
    }
}

@description('The resource ID of the Function app.')
output functionAppId string = functionApp.id
