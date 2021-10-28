# See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

# Define the "runtime" image which will run the DICOM Server
FROM mcr.microsoft.com/dotnet/aspnet:5.0.11-alpine3.13@sha256:2b67665d62471d30de99340361d516d4555a9276cb4ff78fb09db784384b1574 AS runtime
RUN set -x && \
    # See https://www.abhith.net/blog/docker-sql-error-on-aspnet-core-alpine/
    apk add --no-cache icu-libs && \
    addgroup nonroot && \
    adduser -S -D -H -s /sbin/nologin -G nonroot -g nonroot nonroot
ENV DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=false \
    ASPNETCORE_URLS=http://+:8080
EXPOSE 8080
USER nonroot

# Copy the DICOM Server project and build it
FROM mcr.microsoft.com/dotnet/sdk:5.0.402-alpine3.13@sha256:f08db3184fc8fbb12dd4a9d12eb2e3c0a36ca60a4dd13759edde22605fbd00bd AS build
ARG BUILD_CONFIGURATION=Release
ARG CONTINUOUS_INTEGRATION_BUILD=false
WORKDIR /dicom-server
COPY . .
WORKDIR /dicom-server/src/Microsoft.Health.Dicom.Web
RUN dotnet build "Microsoft.Health.Dicom.Web.csproj" -c $BUILD_CONFIGURATION -p:ContinuousIntegrationBuild=$CONTINUOUS_INTEGRATION_BUILD -warnaserror

# Publish the DICOM Server from the build
FROM build as publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "Microsoft.Health.Dicom.Web.csproj" -c $BUILD_CONFIGURATION --no-build -o /app/publish

# Copy the published application
FROM runtime AS dicom-server
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Microsoft.Health.Dicom.Web.dll"]

ENV ApplicationInsights__InstrumentationKey="f91831d6-a007-4c77-b613-c9c90f7e4a74"
ENV BlobStore__ConnectionString="DefaultEndpointsProtocol=https;AccountName=penchedicom5dw3uulea5cww;AccountKey=6jnJ8bVEHNdDE4ftNMix2HbIb//ELXivRRYaoO0a2Nqn7Gw4pxNAGz/6f0AGHBIlULeGq7qCU49PRjmOXU86eA==;"
ENV DicomFunctions__BaseAddress="https://penchedicomk8s2-functions.azurewebsites.net/api/"
ENV DicomFunctions__FunctionAccessKey="pBDk42yeJBXWC1oPW2yOTqUogRAvJ5ySknuf2kkqwC5tFbPZq5dTpQ=="
ENV DicomServer__Features__EnableExtendedQueryTags="True"
ENV SqlServer__ConnectionString="Server=tcp:penchedicomk8s2.database.windows.net,1433;Initial Catalog=Dicom;Persist Security Info=False;User ID=dicomAdmin;Password=jUI&*p)_;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;"
