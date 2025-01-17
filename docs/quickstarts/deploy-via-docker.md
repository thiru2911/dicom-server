# Deploy the Medical Imaging Server for DICOM locally using Docker

This quickstart guide details how to build and run the Medical Imaging Server for DICOM in Docker. By using Docker Compose, all of the necessary dependencies are started automatically in containers without requiring any installations on your development machine. In particular, the Medical Imaging Server for DICOM in Docker starts a container for [SQL Server](https://docs.microsoft.com/sql/linux/quickstart-install-connect-docker?view=sql-server-ver15&pivots=cs1-bash) and the Azure Storage emulator called [Azurite](https://github.com/Azure/Azurite).

> **IMPORTANT**
>
> This sample has been created to enable Development/Test scenarios and is not suitable for production scenarios. Passwords are contained in deployment files, the SQL server connection is not encrypted, authentication on the Medical Imaging Server for DICOM has been disabled, and data is not persisted between container restarts.

## Visual Studio (DICOM Server Only)

You can easily run and debug the Medical Imaging Server for DICOM right from Visual Studio. Simply open up the solution file *Microsoft.Health.Dicom.sln* in Visual Studio 2019 (or later) and run the "docker-compose" project. This should build each of the images and run the containers locally without any additional action.

Once it's ready, a web page should open automatically for the URL `https://localhost:63839` where you can communicate with the Medical Imaging Server for DICOM.

## Command Line

Run the following command from the root of the `microsoft/dicom-server` repository, replacing `<SA_PASSWORD>` with your chosen password (be sure to follow the [SQL Server password complexity requirements](https://docs.microsoft.com/sql/relational-databases/security/password-policy?view=sql-server-ver15#password-complexity)):

```bash
env SAPASSWORD='<SA_PASSWORD>' docker-compose -f samples/docker/docker-compose.yaml -p dicom-server up -d
```

Given the DICOM API is likely to start before the SQL server is ready, you may need to restart the API container once the SQL server is healthy. This can be done using `docker restart <container-name>`, i.e. docker restart `docker restart docker_dicom-api_1`.

Once deployed the Medical Imaging Server for DICOM should be available at `http://localhost:8080/`.

Additionally if uncommented in the docker-compose file the
* SQL Server is able to be browsed using a TCP connection to `localhost:1433`
* the storage containers can be examined via [Azure Storage Explorer](https://azure.microsoft.com/features/storage-explorer/) using the [default storage emulator connection string](https://docs.microsoft.com/en-us/azure/storage/common/storage-use-emulator#connect-to-the-emulator-account-using-the-well-known-account-name-and-key)
* [FHIR](https://github.com/microsoft/fhir-server) can be accessible via `http://localhost:8081`

You can also connect to them via their IP rather rather than expose them on localhost. The following command will help you understand the ports & ips the services are exposed on

```bash
docker inspect -f 'Name: {{.Name}} - IPs: {{range .NetworkSettings.Networks}}{{.IPAddress}}{{end}} - Ports: {{.Config.ExposedPorts}}' $(docker ps -aq)
```

### Run in Docker with a custom configuration

To build the `dicom-api` image run the following command from the root of the `microsoft/dicom-server`repository:

```bash
docker build -f src/microsoft.health.dicom.web/Dockerfile -t microsoft.health.dicom.web .
```

When running the container, additional configuration details can also be specified such as:

```bash
docker run -d \
    -e DicomServer__Security__Enabled="false"
    -e SqlServer__ConnectionString="Server=tcp:<sql-server-fqdn>,1433;Initial Catalog=Dicom;Persist Security Info=False;User ID=sa;Password=<sql-sa-password>;MultipleActiveResultSets=False;Connection Timeout=30;" \
    -e SqlServer__AllowDatabaseCreation="true" \
    -e SqlServer__Initialize="true" \
    -e DataStore="SqlServer" \
    -e BlobStore__ConnectionString="<blob-connection-string" \
    -p 8080:8080
    microsoft.health.dicom.web
    microsoft.health.dicom.web
```

## Next steps

Once deployment is complete you can access your Medical Imaging Server at `https://localhost:63839` or `https://localhost:8080` depending on the above mechanism. Make sure to specify the version as part of the url when making requests. More information can be found in the [Api Versioning Documentation](../api-versioning.md)

* [Use Medical Imaging Server for DICOM APIs](../tutorials/use-the-medical-imaging-server-apis.md)
* [Upload DICOM files via the Electron Tool](../../tools/dicom-web-electron)
* [Enable Azure AD Authentication](../how-to-guides/enable-authentication-with-tokens.md)
* [Enable Identity Server Authentication](../development/identity-server-authentication.md)
