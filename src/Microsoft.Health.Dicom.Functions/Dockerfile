FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS installer-env

WORKDIR /dicom-server
COPY . .
WORKDIR /dicom-server/src/Microsoft.Health.Dicom.Functions
RUN cd /dicom-server/src/Microsoft.Health.Dicom.Functions && \
    mkdir -p /home/site/wwwroot && \
    dotnet publish Microsoft.Health.Dicom.Functions.csproj --output /home/site/wwwroot

# To enable ssh & remote debugging on app service change the base image to the one below
# FROM mcr.microsoft.com/azure-functions/dotnet:3.0-appservice
FROM mcr.microsoft.com/azure-functions/dotnet:3.0
ENV AzureWebJobsScriptRoot=/home/site/wwwroot \
    AzureFunctionsJobHost__Logging__Console__IsEnabled=true

ENV WEBSITE_HOSTNAME=host.docker.internal
COPY --from=installer-env ["/home/site/wwwroot", "/home/site/wwwroot"]
