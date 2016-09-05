CREATE TABLE [dbo].[Marker_Observation] (
    [MarkerObservationID] INT      IDENTITY (1, 1) NOT NULL,
    [MarkerID]            INT      NOT NULL,
    [FoundFlag]           BIT      NOT NULL,
    [WalkID]              INT      NOT NULL,
    [ObservationText]     TEXT     NOT NULL,
    [DateOfObservation]   DATETIME NOT NULL,
    CONSTRAINT [PK_Marker_Observation] PRIMARY KEY CLUSTERED ([MarkerObservationID] ASC),
    CONSTRAINT [FK_Marker_Observation_Marker] FOREIGN KEY ([MarkerID]) REFERENCES [dbo].[Marker] ([MarkerID]),
    CONSTRAINT [FK_Marker_Observation_Walks] FOREIGN KEY ([WalkID]) REFERENCES [dbo].[Walks] ([WalkID])
);

