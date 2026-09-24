@description('Short lowercase prefix used for globally unique resource names.')
@minLength(3)
@maxLength(12)
param prefix string

@description('Azure region for all resources.')
param location string = resourceGroup().location

@description('SQL administrator login.')
param sqlAdministratorLogin string

@secure()
@description('SQL administrator password.')
param sqlAdministratorPassword string

@secure()
@description('JWT signing key, at least 32 characters.')
param jwtSigningKey string

var suffix = uniqueString(subscription().subscriptionId, resourceGroup().id)
var appName = '${prefix}-api-${suffix}'
var sqlServerName = '${prefix}-sql-${suffix}'
var storageName = take(replace('${prefix}docs${suffix}', '-', ''), 24)
var databaseName = 'ClaimsModule'

resource plan 'Microsoft.Web/serverfarms@2023-12-01' = {
  name: '${prefix}-plan'
  location: location
  sku: {
    name: 'B1'
    tier: 'Basic'
  }
  kind: 'linux'
  properties: {
    reserved: true
  }
}

resource storage 'Microsoft.Storage/storageAccounts@2023-05-01' = {
  name: storageName
  location: location
  sku: {
    name: 'Standard_LRS'
  }
  kind: 'StorageV2'
  properties: {
    allowBlobPublicAccess: false
    minimumTlsVersion: 'TLS1_2'
    supportsHttpsTrafficOnly: true
  }
}

resource blobService 'Microsoft.Storage/storageAccounts/blobServices@2023-05-01' = {
  parent: storage
  name: 'default'
}

resource documents 'Microsoft.Storage/storageAccounts/blobServices/containers@2023-05-01' = {
  parent: blobService
  name: 'claim-documents'
  properties: {
    publicAccess: 'None'
  }
}

resource sqlServer 'Microsoft.Sql/servers@2023-08-01-preview' = {
  name: sqlServerName
  location: location
  properties: {
    administratorLogin: sqlAdministratorLogin
    administratorLoginPassword: sqlAdministratorPassword
    minimalTlsVersion: '1.2'
    publicNetworkAccess: 'Enabled'
  }
}

resource allowAzure 'Microsoft.Sql/servers/firewallRules@2023-08-01-preview' = {
  parent: sqlServer
  name: 'AllowAzureServices'
  properties: {
    startIpAddress: '0.0.0.0'
    endIpAddress: '0.0.0.0'
  }
}

resource database 'Microsoft.Sql/servers/databases@2023-08-01-preview' = {
  parent: sqlServer
  name: databaseName
  location: location
  sku: {
    name: 'Basic'
    tier: 'Basic'
  }
}

resource webApp 'Microsoft.Web/sites@2023-12-01' = {
  name: appName
  location: location
  identity: {
    type: 'SystemAssigned'
  }
  properties: {
    serverFarmId: plan.id
    httpsOnly: true
    siteConfig: {
      alwaysOn: true
      ftpsState: 'Disabled'
      minTlsVersion: '1.2'
      linuxFxVersion: 'DOTNETCORE|9.0'
      healthCheckPath: '/health'
      appSettings: [
        {
          name: 'ASPNETCORE_ENVIRONMENT'
          value: 'Production'
        }
        {
          name: 'Jwt__Key'
          value: jwtSigningKey
        }
        {
          name: 'Jwt__Issuer'
          value: 'ClaimsModule'
        }
        {
          name: 'Jwt__Audience'
          value: 'ClaimsModule'
        }
        {
          name: 'Storage__Provider'
          value: 'AzureBlob'
        }
        {
          name: 'Storage__Azure__Container'
          value: documents.name
        }
        {
          name: 'Storage__Azure__ConnectionString'
          value: 'DefaultEndpointsProtocol=https;AccountName=${storage.name};AccountKey=${storage.listKeys().keys[0].value};EndpointSuffix=${environment().suffixes.storage}'
        }
      ]
      connectionStrings: [
        {
          name: 'DefaultConnection'
          type: 'SQLAzure'
          connectionString: 'Server=tcp:${sqlServer.properties.fullyQualifiedDomainName},1433;Initial Catalog=${database.name};Persist Security Info=False;User ID=${sqlAdministratorLogin};Password=${sqlAdministratorPassword};MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;'
        }
      ]
    }
  }
}

output webAppName string = webApp.name
output applicationUrl string = 'https://${webApp.properties.defaultHostName}'
output sqlServerFullyQualifiedDomainName string = sqlServer.properties.fullyQualifiedDomainName
