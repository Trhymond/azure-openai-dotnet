#!/bin/bash

#file_name=$(find ./app/frontend/dist  -name 'index*.js')
file_name="../app/frontend/.env.production"
backend_url="$1"
api_key="$2"

echo $backend_url
echo $file_name

# sed -i -e "s|__VITE_API_BASE_URL__|$backend_url|g" $file_name

#echo "VITE_API_BASE_URL=$backend_url/api" >> $file_name

cat > $file_name <<EOF
VITE_API_BASE_URL=$backend_url
VITE_API_KEY=$api_key
EOF


npm install; npm run build