CREATE TABLE [dbo].[AreaTypes] (
    [AreaType]     CHAR (1)   NOT NULL,
    [AreaTypeName] NCHAR (50) NOT NULL,
    CONSTRAINT [PK_AreaTypes] PRIMARY KEY CLUSTERED ([AreaType] ASC)
);

