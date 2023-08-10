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

AZURE_STORAGE_BLOB_ENDPOINT="https://${AZURE_STORAGE_ACCOUNT}.blob.core.windows.net"
AZURE_SEARCH_SERVICE_ENDPOINT="https://${AZURE_SEARCH_SERVICE}.search.windows.net/"
AZURE_FORMRECOGNIZER_SERVICE_ENDPOINT="https://${AZURE_FORM_RECOGNIZER_SERVICE}.cognitiveservices.azure.com/"

if [ -z "$AZD_PREPDOCS_RAN" ] || [ "$AZD_PREPDOCS_RAN" = "false" ]; then
    echo 'Running "PrepareDocs.dll"'

    pwd
    
    dotnet build "app/prepdocs/prepdocs.csproj" 

    dotnet run --project "app/prepdocs/prepdocs.csproj" -- \
      './data/*.pdf' \
      --storageendpoint "$AZURE_STORAGE_BLOB_ENDPOINT" \
      --container "$AZURE_STORAGE_CONTAINER" \
      --searchendpoint "$AZURE_SEARCH_SERVICE_ENDPOINT" \
      --searchindex "$AZURE_SEARCH_INDEX" \
      --formrecognizerendpoint "$AZURE_FORMRECOGNIZER_SERVICE_ENDPOINT" \
      --tenantid "$AZURE_TENANT_ID" \
      -v

    azd env set AZD_PREPDOCS_RAN "true"
else
    echo "AZD_PREPDOCS_RAN is set to true. Skipping the run."
fi
