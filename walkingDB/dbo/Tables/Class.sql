CREATE TABLE [dbo].[Class] (
    [SortSeq]   SMALLINT     NOT NULL,
    [Classref]  VARCHAR (5)  NOT NULL,
    [Classname] VARCHAR (30) NOT NULL,
    [ClassType] CHAR (1)     NULL,
    CONSTRAINT [PK_Class] PRIMARY KEY CLUSTERED ([Classref] ASC)
);

