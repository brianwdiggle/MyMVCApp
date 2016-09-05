CREATE TABLE [dbo].[Walk_AssociatedFiles] (
    [Walk_AssociatedFileID]        INT           IDENTITY (1, 1) NOT NULL,
    [WalkID]                       INT           NOT NULL,
    [Walk_AssociatedFile_Name]     VARCHAR (200) NOT NULL,
    [Walk_AssociatedFile_Type]     VARCHAR (40)  NOT NULL,
    [Walk_AssociatedFile_Sequence] SMALLINT      NOT NULL,
    [Walk_AssociatedFile_Caption]  TEXT          NULL,
    [Walk_AssociatedFile_MarkerID] INT           NULL,
    CONSTRAINT [PK_Walk_AssociatedFiles] PRIMARY KEY CLUSTERED ([Walk_AssociatedFileID] ASC),
    CONSTRAINT [FK_Walk_AssociatedFiles_Marker] FOREIGN KEY ([Walk_AssociatedFile_MarkerID]) REFERENCES [dbo].[Marker] ([MarkerID]),
    CONSTRAINT [FK_Walk_AssociatedFiles_Walk_AssociatedFile_Types] FOREIGN KEY ([Walk_AssociatedFile_Type]) REFERENCES [dbo].[Walk_AssociatedFile_Types] ([Walk_AssociatedFile_Type]),
    CONSTRAINT [FK_Walk_AssociatedFiles_Walks] FOREIGN KEY ([WalkID]) REFERENCES [dbo].[Walks] ([WalkID])
);

