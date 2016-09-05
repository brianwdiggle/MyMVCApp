CREATE TABLE [dbo].[Marker] (
    [MarkerID]             INT           IDENTITY (1, 1) NOT NULL,
    [MarkerTitle]          VARCHAR (100) NOT NULL,
    [Hillnumber]           SMALLINT      NULL,
    [GPS_Reference]        NCHAR (14)    NULL,
    [Location_Description] TEXT          NULL,
    [WalkID]               INT           NULL,
    [DateLeft]             DATETIME      NOT NULL,
    [Status]               NCHAR (40)    NOT NULL,
    CONSTRAINT [PK_Marker] PRIMARY KEY CLUSTERED ([MarkerID] ASC),
    CONSTRAINT [FK_Marker_Hills] FOREIGN KEY ([Hillnumber]) REFERENCES [dbo].[Hills] ([Hillnumber]),
    CONSTRAINT [FK_Marker_Marker_Status] FOREIGN KEY ([Status]) REFERENCES [dbo].[Marker_Status] ([Marker_Status]),
    CONSTRAINT [FK_Marker_Walks] FOREIGN KEY ([WalkID]) REFERENCES [dbo].[Walks] ([WalkID])
);

