param
(
    [Parameter(Mandatory = $true)]
    $projectName,
    [Parameter(Mandatory = $true)]
    $projectEnvironment,
    [Parameter(Mandatory = $true)]
    $location,
    [Parameter()]
    $bicepFile = ".\deploy.bicep"
)

$resourceGroupName = "rg-$projectName-$projectEnvironment"

az --version

Write-Host "Ensuring resource group $resourceGroupName exists in $location..."
az group create --name $resourceGroupName --location $location

Write-Host "Deploying Azure infrastructure to $resourceGroupName in $location..."
az deployment group create --resource-group $resourceGroupName `
    --template-file $bicepFile `
    --parameters location=$location `
    --parameters projectName=$projectName `
    --parameters projectEnvironment=$projectEnvironment
