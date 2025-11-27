Database migrations for AlbumUploader

Files in this folder are simple SQL migration scripts that can be applied
against a development or production Azure SQL database.

Local application (SQL Server / Azure SQL Edge):

```powershell
sqlcmd -S <server> -d <database> -U <user> -P <password> -i backend/migrations/001_create_albums_and_mediaitems.sql
```

Use Azure Data Studio or SSMS for interactive runs. For CI/CD, integrate the scripts
into your deployment pipeline (e.g., az cli or Azure SQL DACPAC deployments).
