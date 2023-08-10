#!/bin/sh

set -e 

echo ""
echo "Loading azd .env file from current environment"
echo ""
while IFS='=' read -r key value; do
    value=$(echo "$value" | sed 's/^"//' | sed 's/"$//')
    export "$key=$value"
done <<EOF
$(azd env get-values)
EOF

echo "Environment variables set."

AUTH_TOKEN=$(az account get-access-token --resource=https://management.azure.com --query accessToken --output tsv)
URL="https://management.azure.com/subscriptions/$AZURE_SUBSCRIPTION_ID/resourcegroups/$AZURE_RESOURCE_GROUP/providers/Microsoft.Search/searchServices/$AZURE_SEARCH_SERVICE?api-version=2021-04-01-Preview"

curl -X PATCH --url $URL \
--header 'Content-Type: application/json' \
--header 'Accept: application/json' \
--header 'Authorization: Bearer '$AUTH_TOKEN \
--data '{
    "properties": {
        "semanticSearch": "free"
    }
}'


