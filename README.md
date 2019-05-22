# Cosmos DB Autoscale RU

## Setup

First, create the resource group. select the location that the cosmos db account is or will be located, idealy. Here, I have it as East US.

```bash
location=eastus 
resource_group=cosmosdb-autoscale
az group create -n $resource_group -l $location
```

Create a cosmos db account

```bash
cosmosdb_account_name=<the account name you want>

# create the account in eastus
az cosmosdb create --name $cosmosdb_account_name \
  --resource-group $resource_group \
  --kind GlobalDocumentDB \
  --enable-automatic-failover \
  --locations eastus=0 
```

Create the function app so we can deploy the autoscale function

```bash
suffix=<unique suffix for the function app and storage>

# create storage account in the resource group first
storage_account_name="czmdbscalestore$suffix"

az storage account create \
    --location $location \
    --name $storage_account_name \
    --resource-group $resource_group \
    --sku Standard_LRS \
    --kind StorageV2

suffix=<unique suffix for the function app>
functionapp_name=cosmosdb-autoscale-$suffix

az functionapp create --consumption-plan-location $location \
  --name $functionapp_name \
  --os-type Windows \
  --resource-group $resource_group \
  --runtime dotnet \
  --storage-account MyStorageAccount

```