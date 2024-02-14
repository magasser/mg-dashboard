FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

WORKDIR /home/build

COPY *.sln .
COPY MG.Dashboard.Api/*.csproj ./MG.Dashboard.Api/

RUN dotnet restore

COPY MG.Dashboard.Api MG.Dashboard.Api

WORKDIR /home/build/MG.Dashboard.Api

RUN dotnet publish -c Release -o /home/app --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:8.0

WORKDIR /home/app

COPY MG.Dashboard.Api/.env /home/app/.env
COPY aspnetapp.pfx /https/aspnetapp.pfx
COPY --from=build /home/app /home/app

EXPOSE 80
EXPOSE 443

ENTRYPOINT [ "dotnet", "MG.Dashboard.Api.dll" ]