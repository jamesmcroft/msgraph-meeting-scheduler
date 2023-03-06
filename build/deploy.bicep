@description('The Azure location for the environment')
param location string = resourceGroup().location
@description('The short name of the project for naming resources')
param projectName string
@description('The short name of the environment for naming resources')
param projectEnvironment string

var appServicePlanName = 'plan-${projectName}-${projectEnvironment}'
var logAnalyticsWorkspaceName = 'log-${projectName}-${projectEnvironment}'
var applicationInsightsName = 'appi-${projectName}-${projectEnvironment}'
var storageAccountName = 'st${projectName}${projectEnvironment}'
var functionAppName = 'func-${projectName}-${projectEnvironment}'

var baseTags = {
    ApplicationName: projectName
    Environment: projectEnvironment
    DataClassification: 'General'
    Criticality: 'High'
    OwnerName: 'James Croft'
    ProjectName: 'MS Graph Meeting Scheduler'
}

module logAnalyticsWorkspace 'log-analytics-workspace.bicep' = {
    name: '${logAnalyticsWorkspaceName}-deploy'
    params: {
        name: logAnalyticsWorkspaceName
        location: location
        tags: union(baseTags, {})
        applicationInsightsName: applicationInsightsName
    }
}

module storageAccount 'storage-account.bicep' = {
    name: '${storageAccountName}-deploy'
    params: {
        name: storageAccountName
        location: location
        tags: union(baseTags, {})
        skuName: 'Standard_LRS'
    }
}

module appServicePlan 'app-service-plan.bicep' = {
    name: '${appServicePlanName}-deploy'
    params: {
        name: appServicePlanName
        location: location
        tags: union(baseTags, {})
    }
}

module functionApp 'function-app.bicep' = {
    name: '${functionAppName}-deploy'
    params: {
        name: functionAppName
        location: location
        tags: union(baseTags, {})
        storageAccountName: storageAccountName
        appServicePlanName: appServicePlanName
        appSettings: [
            {
                name: 'APPINSIGHTS_INSTRUMENTATIONKEY'
                value: logAnalyticsWorkspace.outputs.appInsightsInstrumentationKey
            }
            {
                name: 'APPLICATIONINSIGHTS_CONNECTION_STRING'
                value: logAnalyticsWorkspace.outputs.appInsightsConnectionString
            }
        ]
    }
    dependsOn: [
        logAnalyticsWorkspace
        storageAccount
        appServicePlan
    ]
}
