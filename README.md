# TestcontaienrsAutoSetup [![CI](https://github.com/DenysMalanichev/TestcontaienrsAutoSetup/actions/workflows/ci.yaml/badge.svg)](https://github.com/DenysMalanichev/TestcontaienrsAutoSetup/actions/workflows/ci.yaml)

## Docker under WSL
In case your Docker is running under WSL2 do not forget to 
expose the docker port:
``` bash
sudo mkdir -p /etc/systemd/system/docker.service.d
sudo vim /etc/systemd/system/docker.service.d/override.conf

# add the below to the override.conf file
[Service]
ExecStart=
ExecStart=/usr/bin/dockerd --host=tcp://0.0.0.0:2375 --host=unix:///var/run/docker.sock
```
> **NOTE:** change the Docker daemon port configured in appsettings.local.json in case it is not 2375 (*DockerPort* setting)

## Entity Framework Core
Simply call the WithEFCoreMigrations method followed by the BuildAndInitializeWithEfContextAsync specifying a ***DataContext*** class as generic parameter.

*Make sure that data for seeding is manually placed in migrations or use a **.HasData()** method*
```C#
var builder = new AutoSetupContainerBuilder(dockerEndpoint!);
var msSqlContainer = await builder.CreateSqlServerContainer()
    .UseDatabaseName("DbName")
    .WithEFCoreMigrations()
    .BuildAndInitializeWithEfContextAsync<CatalogContext>();
```
## Multiple DBs within one container
It is possible to create more then one DB in a container, with distinct names and data
```C#
var msSqlContainer = await builder.CreateSqlServerContainer()
    .AddDatabase(new EFCoreMigration<CatalogContext>())
    .UseDatabaseName("Catalog")
    .AddDatabase(new EFCoreMigration<TenantContext>())
    .UseDatabaseName("Tenant")
    .BuildAndInitializeAsync();
```