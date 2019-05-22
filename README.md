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
functionapp_name="cosmosdb-autoscale-$suffix"
insights_name="$functionapp_name-insights"
storage_account_name="czmdbscalestore$suffix"

# create storage account in the resource group first
az storage account create \
    --location $location \
    --name $storage_account_name \
    --resource-group $resource_group \
    --sku Standard_LRS \
    --kind StorageV2

# Create an app insights to collect telemetry
az resource create \
  --resource-group $resource_group \
  --name $insights_name \
  --resource-type "Microsoft.Insights/components" \
  --properties '{"Application_Type":"web"}'

# function app
az functionapp create --consumption-plan-location $location \
  --name $functionapp_name \
  --os-type Windows \
  --resource-group $resource_group \
  --runtime dotnet \
  --app-insights $insights_name \
  --storage-account $storage_account_name


```

Next, deploy the function to the function app

Unfortunately, just to get the api key for the function URI in order to set our Action Group Action to a webhook, we have to jump through some hoops. Fortunately for you, I've done the work for you.

```bash
# get the Kudu endpoint and credentials
functionapp_kudu_endpoint=https://$(az functionapp show -g $resource_group -n $functionapp_name -o tsv --query 'enabledHostNames[1]')
kudu_credentials=$(az functionapp deployment list-publishing-credentials -g $resource_group -n $functionapp_name -o json)

# with the credentials, get an auth token to get our api key
basic_auth="$(echo $kudu_credentials | jq .publishingUserName -r):$(echo $kudu_credentials | jq .publishingPassword -r)"
auth_token=$(curl --user $basic_auth $functionapp_kudu_endpoint/api/functions/admin/token -H 'Accept:application/json')
auth_token=$(echo $auth_token | jq . -r)

# build the URI to get the api key
functionapp_endpoint=https://$(az functionapp show -g $resource_group -n $functionapp_name -o tsv --query 'enabledHostNames[0]')
functionapp_keys_endpoint=$functionapp_endpoint/admin/functions/evaluateRequestUnits/keys

# get the function app api key
keys_data=$(curl -H "Authorization: Bearer ${auth_token}" -H "Accept: application/json" $functionapp_keys_endpoint)
function_key=$(echo $keys_data | jq -c '.keys[] | select(.name | contains("default")) | .value' -r)

# finally, set the full function URL
function_url=$functionapp_endpoint/api/evaluateRequestUnits?code=$function_key
```


With the function deployed, the alert can now point to the function to use with in the Alert Action Group. Deploy the alert to Azure Monitor to begin triggering


```bash

```

```bash

# get the azure function URL

# next, create the action-group to assign to the alert we will create
az monitor action-group create
az monitor action-group create \
  --action webhook https://alerts.contoso.com apiKey={APIKey} type=HighCPU \
  --name MyActionGroup --resource-group MyResourceGroup
```
