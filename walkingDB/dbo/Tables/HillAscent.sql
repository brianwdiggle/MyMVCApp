CREATE TABLE [dbo].[HillAscent] (
    [AscentID]   INT      IDENTITY (1, 1) NOT NULL,
    [AscentDate] DATETIME NOT NULL,
    [Hillnumber] SMALLINT NOT NULL,
    [WalkID]     INT      NOT NULL,
    CONSTRAINT [PK_HillAscent] PRIMARY KEY CLUSTERED ([AscentID] ASC),
    CONSTRAINT [FK_HillAscent_Hills] FOREIGN KEY ([Hillnumber]) REFERENCES [dbo].[Hills] ([Hillnumber]),
    CONSTRAINT [FK_HillAscent_Walks] FOREIGN KEY ([WalkID]) REFERENCES [dbo].[Walks] ([WalkID])
);

