## Package
```
    Microsoft.EntityFrameworkCore.SqlServer
    Microsoft.EntityFrameworkCore.Tools
```

## Config SQL DB
1. install sql server 2022
2. install sql server management studio
3. opensql server management studio and connect to DB, choose Server name
   
## Add New Table to DataBAse

1. open class in Models.
2. in class add all pror with get,set methods
3. in DBUsers context add public DbSet<_name_> _name_ { get; set; }

## Migrate and update dB
In file appsettings.json, edit the name of ConnectionStrings
```
    "ConnectionStrings": { "conn": "data source=*****Your name*****;initial catalog=FloweSHopDB;integrated security=true;TrustServerCertificate=True" }
```
1. run in pm console
```bash
  add-migration _explain_
```
2. run update
```bash
  update-database
```
