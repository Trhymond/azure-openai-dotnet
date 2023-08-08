
#!/bin/sh

echo ""
echo "Loading allowed origins to function app"
echo ""

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


echo "$AZURE_RESOURCE_GROUP"
echo "$AZURE_FUNCTION_APP"
echo "$FRONTEND_URI"

az functionapp cors add -g "$AZURE_RESOURCE_GROUP" -n "$AZURE_FUNCTION_APP" --allowed-origins "$FRONTEND_URI"