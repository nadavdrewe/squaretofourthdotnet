SET XACT_ABORT ON;
BEGIN TRANSACTION;
IF OBJECT_ID(N'dbo.SapDiscoveryDocuments', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.SapDiscoveryDocuments (
        Id varchar(32) NOT NULL CONSTRAINT PK_SapDiscoveryDocuments PRIMARY KEY,
        AccessHash varchar(64) NOT NULL,
        Revision int NOT NULL,
        DocumentJson nvarchar(max) NOT NULL,
        UpdatedUtc datetime2 NOT NULL,
        CONSTRAINT CK_SapDiscoveryDocuments_Json CHECK (ISJSON(DocumentJson)=1)
    );
END;
IF OBJECT_ID(N'dbo.SapDiscoveryRevisions', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.SapDiscoveryRevisions (
        DocumentId varchar(32) NOT NULL,
        Revision int NOT NULL,
        Actor nvarchar(450) NOT NULL,
        Status varchar(32) NOT NULL,
        ChangedUtc datetime2 NOT NULL,
        DocumentJson nvarchar(max) NOT NULL,
        CONSTRAINT PK_SapDiscoveryRevisions PRIMARY KEY (DocumentId, Revision),
        CONSTRAINT FK_SapDiscoveryRevisions_Document FOREIGN KEY (DocumentId) REFERENCES dbo.SapDiscoveryDocuments(Id),
        CONSTRAINT CK_SapDiscoveryRevisions_Json CHECK (ISJSON(DocumentJson)=1)
    );
END;
COMMIT;
