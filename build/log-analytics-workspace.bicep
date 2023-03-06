@description('The name for the Log Analytics Workspace.')
param name string
@description('The location for the Log Analytics Workspace. Defaults to the location of the resource group.')
param location string = resourceGroup().location
@description('The name for Application Insights.')
param applicationInsightsName string
@description('The retention period in days for the Log Analytics Workspace. Defaults to 30 days.')
param retentionInDays int = 30
@description('The SKU name for the Log Analytics Workspace. Defaults to PerGB2018.')
@allowed([
    'CapacityReservation'
    'Free'
    'LACluster'
    'PerGB2018'
    'PerNode'
    'Premium'
    'Standalone'
    'Standard'
])
param skuName string = 'PerGB2018'
@description('The tags for the Log Analytics Workspace and Application Insights.')
param tags object = {}

resource logAnalyticsWorkspace 'Microsoft.OperationalInsights/workspaces@2022-10-01' = {
    name: name
    location: location
    tags: tags
    properties: {
        retentionInDays: retentionInDays
        features: {
            searchVersion: 1
        }
        sku: {
            name: skuName
        }
    }
}

resource appInsights 'Microsoft.Insights/components@2020-02-02' = {
    name: applicationInsightsName
    location: location
    tags: tags
    kind: 'web'
    properties: {
        Application_Type: 'web'
        WorkspaceResourceId: logAnalyticsWorkspace.id
    }
}

@description('The resource ID of the Log Analytics Workspace.')
output logAnalyticsWorkspaceId string = logAnalyticsWorkspace.id
@description('The customer ID associated with the Log Analytics Workspace.')
output logAnalyticsWorkspaceCustomerId string = logAnalyticsWorkspace.properties.customerId
@description('The resource ID of Application Insights.')
output appInsightsId string = appInsights.id
@description('The instrumentation key for Application Insights.')
output appInsightsInstrumentationKey string = appInsights.properties.InstrumentationKey
@description('The connection string for Application Insights.')
output appInsightsConnectionString string = appInsights.properties.ConnectionString
