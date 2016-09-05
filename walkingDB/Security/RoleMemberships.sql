EXECUTE sp_addrolemember @rolename = N'db_datareader', @membername = N'IIS APPPOOL\ASP.NET v4.0';


GO
EXECUTE sp_addrolemember @rolename = N'db_datawriter', @membername = N'IIS APPPOOL\ASP.NET v4.0';

