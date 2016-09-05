CREATE TABLE [dbo].[Classlink] (
    [Hillnumber]  SMALLINT    NOT NULL,
    [Classref]    VARCHAR (5) NOT NULL,
    [ClasslinkID] INT         IDENTITY (1, 1) NOT NULL,
    CONSTRAINT [PK_Classlink] PRIMARY KEY CLUSTERED ([ClasslinkID] ASC),
    CONSTRAINT [FK_Classlink_Class] FOREIGN KEY ([Classref]) REFERENCES [dbo].[Class] ([Classref]),
    CONSTRAINT [FK_Classlink_Hills] FOREIGN KEY ([Hillnumber]) REFERENCES [dbo].[Hills] ([Hillnumber])
);

