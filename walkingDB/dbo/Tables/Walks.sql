CREATE TABLE [dbo].[Walks] (
    [WalkID]              INT             IDENTITY (1, 1) NOT NULL,
    [WalkDate]            DATETIME        NOT NULL,
    [WalkDescription]     TEXT            NULL,
    [WalkTitle]           NVARCHAR (100)  NULL,
    [WalkSummary]         NVARCHAR (1000) NULL,
    [WalkStartPoint]      NVARCHAR (100)  NULL,
    [WalkEndPoint]        NVARCHAR (100)  NULL,
    [WalkType]            NVARCHAR (30)   NOT NULL,
    [WalkAreaName]        NVARCHAR (100)  NOT NULL,
    [CartographicLength]  FLOAT (53)      NULL,
    [MetresOfAscent]      SMALLINT        NULL,
    [WalkCompanions]      NVARCHAR (50)   NULL,
    [WalkTotalTime]       INT             NULL,
    [WalkAverageSpeedKmh] FLOAT (53)      NULL,
    [MovingAverageKmh]    FLOAT (53)      NULL,
    [WalkConditions]      NVARCHAR (200)  NULL,
    CONSTRAINT [PK_Walks] PRIMARY KEY CLUSTERED ([WalkID] ASC),
    CONSTRAINT [FK_Walks_Areas] FOREIGN KEY ([WalkAreaName]) REFERENCES [dbo].[Areas] ([Areaname]),
    CONSTRAINT [FK_Walks_WalkTypes] FOREIGN KEY ([WalkType]) REFERENCES [dbo].[WalkTypes] ([WalkTypeString])
);

