apiStr=$(docker ps | findstr 'api.mg-dashboard')
mqttStr=$(docker ps | findstr 'mqtt.mg-dashboard')
websiteStr=$(docker ps | findstr 'website.mg-dashboard')

IFS=' '

read -ra apiId <<< "$apiStr"
read -ra mqttId <<< "$apiStr"
read -ra mqttId <<< "$apiStr"

echo "Container ID for api.mg-dashboard: $apiId"
echo "Container ID for mqtt.mg-dashboard: $mqttId"
echo "Container ID for website.mg-dashboard: $mqttId"

mkdir -p images

echo "Start packaging..."
docker commit $apiId mg-dashboard-api
docker commit $mqttId mg-dashboard-mqtt
docker commit $mqttId mg-dashboard-website

docker save mg-dashboard-api -o images/mg-dashboard-api.tar
echo "Packaged api.mg-dashboard at images/mg-dashboard-api.tar"

docker save mg-dashboard-mqtt -o images/mg-dashboard-mqtt.tar
echo "Packaged mqtt.mg-dashboard at images/mg-dashboard-api.tar"

docker save mg-dashboard-website -o images/mg-dashboard-website.tar
echo "Packaged website.mg-dashboard at images/mg-dashboard-website.tar"