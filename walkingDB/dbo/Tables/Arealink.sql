CREATE TABLE [dbo].[Arealink] (
    [Hillnumber] SMALLINT  NOT NULL,
    [Arearef]    CHAR (10) NOT NULL,
    [Alt_Area]   BIT       NOT NULL,
    [AreaLinkID] INT       IDENTITY (1, 1) NOT NULL,
    CONSTRAINT [PK_Arealink] PRIMARY KEY CLUSTERED ([AreaLinkID] ASC),
    CONSTRAINT [FK_Arealink_Hills] FOREIGN KEY ([Hillnumber]) REFERENCES [dbo].[Hills] ([Hillnumber])
);

