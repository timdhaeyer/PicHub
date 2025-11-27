-- SQL Server compatible migration: creates Albums and MediaItems if missing

IF DB_ID(N'PicHubDev') IS NULL
BEGIN
    PRINT 'Creating database PicHubDev';
    CREATE DATABASE [PicHubDev];
END
GO

USE [PicHubDev];
GO

IF NOT EXISTS (SELECT 1 FROM sys.tables t JOIN sys.schemas s ON t.schema_id = s.schema_id WHERE t.name = 'Albums' AND s.name = 'dbo')
BEGIN
    PRINT 'Creating table Albums';
    CREATE TABLE dbo.Albums (
      Id UNIQUEIDENTIFIER NOT NULL PRIMARY KEY,
      Title NVARCHAR(256) NOT NULL,
      Description NVARCHAR(1024) NULL,
      CreatedBy UNIQUEIDENTIFIER NULL,
      CreatedAt DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
      PublicToken NVARCHAR(128) NOT NULL,
      AllowUploads BIT NOT NULL DEFAULT 1,
      MaxFileSizeMB INT NOT NULL DEFAULT 50,
      AlbumSizeTshirt NVARCHAR(8) NOT NULL DEFAULT 'M',
      TotalBytesUsed BIGINT NOT NULL DEFAULT 0,
      RetentionDays INT NOT NULL DEFAULT 30,
      ExpiresAt DATETIME2 NULL
    );
END
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes i JOIN sys.tables t ON i.object_id = t.object_id WHERE t.name = 'Albums' AND i.name = 'IDX_Albums_PublicToken')
BEGIN
    PRINT 'Creating index IDX_Albums_PublicToken';
    CREATE INDEX IDX_Albums_PublicToken ON dbo.Albums (PublicToken);
END
GO

IF NOT EXISTS (SELECT 1 FROM sys.tables t JOIN sys.schemas s ON t.schema_id = s.schema_id WHERE t.name = 'MediaItems' AND s.name = 'dbo')
BEGIN
    PRINT 'Creating table MediaItems';
    CREATE TABLE dbo.MediaItems (
      Id UNIQUEIDENTIFIER NOT NULL PRIMARY KEY,
      AlbumId UNIQUEIDENTIFIER NOT NULL,
      Filename NVARCHAR(512) NOT NULL,
      ContentType NVARCHAR(128) NOT NULL,
      SizeBytes BIGINT NOT NULL,
      StoragePath NVARCHAR(1024) NOT NULL,
      ThumbnailPath NVARCHAR(1024) NULL,
      UploadedAt DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
      UploadedBy UNIQUEIDENTIFIER NULL,
      IsProcessed BIT NOT NULL DEFAULT 0,
      CONSTRAINT FK_MediaItems_Albums FOREIGN KEY (AlbumId) REFERENCES dbo.Albums(Id) ON DELETE CASCADE
    );
END
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes i JOIN sys.tables t ON i.object_id = t.object_id WHERE t.name = 'MediaItems' AND i.name = 'IDX_MediaItems_AlbumId')
BEGIN
    PRINT 'Creating index IDX_MediaItems_AlbumId';
    CREATE INDEX IDX_MediaItems_AlbumId ON dbo.MediaItems (AlbumId);
END
GO
