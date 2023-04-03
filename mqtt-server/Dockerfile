FROM mcr.microsoft.com/dotnet/sdk:6.0 as build-env

WORKDIR /src
COPY MG.Dashboard.Mqtt.Server/*.csproj .
RUN dotnet restore
COPY MG.Dashboard.Mqtt.Server .
RUN dotnet publish MG.Dashboard.Mqtt.Server.csproj -c Release -o /publish

FROM mcr.microsoft.com/dotnet/aspnet:6.0 as runtime

WORKDIR /publish
COPY --from=build-env /publish .
EXPOSE 1883

ENTRYPOINT ["dotnet", "MG.Dashboard.Mqtt.Server.dll"]