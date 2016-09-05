CREATE TABLE [dbo].[Areas] (
    [Country]   CHAR (2)       NOT NULL,
    [Arearef]   CHAR (10)      NOT NULL,
    [Shortname] NVARCHAR (50)  NOT NULL,
    [Areaname]  NVARCHAR (100) NOT NULL,
    [AreaType]  CHAR (1)       NULL,
    CONSTRAINT [PK_Areas] PRIMARY KEY CLUSTERED ([Areaname] ASC),
    CONSTRAINT [FK_Areas_AreaTypes] FOREIGN KEY ([AreaType]) REFERENCES [dbo].[AreaTypes] ([AreaType])
);

