@description('The name for the App Service Plan.')
param name string
@description('The location for the App Service Plan. Defaults to the location of the resource group.')
param location string = resourceGroup().location
@description('The tags for the App Service Plan resource.')
param tags object = {}

resource appServicePlan 'Microsoft.Web/serverfarms@2022-03-01' = {
    name: name
    location: location
    tags: tags
    properties: {
        reserved: true
    }
    sku: {
        name: 'Y1'
    }
}

@description('The resource ID of the App Service Plan resource.')
output appServicePlanId string = appServicePlan.id
