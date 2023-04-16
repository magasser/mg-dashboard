FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build

WORKDIR /home/build

COPY *.sln .
COPY MG.Dashboard.Mqtt.Server/*.csproj ./MG.Dashboard.Mqtt.Server/

RUN dotnet restore

COPY MG.Dashboard.Mqtt.Server MG.Dashboard.Mqtt.Server

WORKDIR /home/build/MG.Dashboard.Mqtt.Server

RUN dotnet publish -c Release -o /home/app --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:6.0

WORKDIR /home/app

COPY MG.Dashboard.Mqtt.Server/.env /home/app/.env
COPY --from=build /home/app /home/app

EXPOSE 1883
EXPOSE 8083

ENTRYPOINT [ "dotnet", "MG.Dashboard.Mqtt.Server.dll" ]