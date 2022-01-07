﻿/*
Deployment script for walkingDB

This code was generated by a tool.
Changes to this file may cause incorrect behavior and will be lost if
the code is regenerated.
*/

GO
SET ANSI_NULLS, ANSI_PADDING, ANSI_WARNINGS, ARITHABORT, CONCAT_NULL_YIELDS_NULL, QUOTED_IDENTIFIER ON;

SET NUMERIC_ROUNDABORT OFF;


GO
:setvar DatabaseName "walkingDB"
:setvar DefaultFilePrefix "walkingDB"
:setvar DefaultDataPath "C:\Users\Brian\AppData\Local\Microsoft\VisualStudio\SSDT\MyMVCApp"
:setvar DefaultLogPath "C:\Users\Brian\AppData\Local\Microsoft\VisualStudio\SSDT\MyMVCApp"

GO
:on error exit
GO
/*
Detect SQLCMD mode and disable script execution if SQLCMD mode is not supported.
To re-enable the script after enabling SQLCMD mode, execute the following:
SET NOEXEC OFF; 
*/
:setvar __IsSqlCmdEnabled "True"
GO
IF N'$(__IsSqlCmdEnabled)' NOT LIKE N'True'
    BEGIN
        PRINT N'SQLCMD mode must be enabled to successfully execute this script.';
        SET NOEXEC ON;
    END


GO
USE [$(DatabaseName)];


GO
PRINT N'Creating [IIS APPPOOL\ASP.NET v4.0]...';


GO
CREATE LOGIN [IIS APPPOOL\ASP.NET v4.0]
    FROM WINDOWS WITH DEFAULT_DATABASE = [walkingDB], DEFAULT_LANGUAGE = [us_english];


GO
PRINT N'Creating [IIS APPPOOL\ASP.NET v4.0]...';


GO
CREATE USER [IIS APPPOOL\ASP.NET v4.0] FOR LOGIN [IIS APPPOOL\ASP.NET v4.0];


GO
PRINT N'Creating <unnamed>...';


GO
EXECUTE sp_addrolemember @rolename = N'db_datareader', @membername = N'IIS APPPOOL\ASP.NET v4.0';


GO
PRINT N'Creating <unnamed>...';


GO
EXECUTE sp_addrolemember @rolename = N'db_datawriter', @membername = N'IIS APPPOOL\ASP.NET v4.0';


GO
PRINT N'Creating [dbo].[Arealink]...';


GO
CREATE TABLE [dbo].[Arealink] (
    [Hillnumber] SMALLINT  NOT NULL,
    [Arearef]    CHAR (10) NOT NULL,
    [Alt_Area]   BIT       NOT NULL,
    [AreaLinkID] INT       IDENTITY (1, 1) NOT NULL,
    CONSTRAINT [PK_Arealink] PRIMARY KEY CLUSTERED ([AreaLinkID] ASC)
);


GO
PRINT N'Creating [dbo].[Areas]...';


GO
CREATE TABLE [dbo].[Areas] (
    [Country]   CHAR (2)       NOT NULL,
    [Arearef]   CHAR (10)      NOT NULL,
    [Shortname] NVARCHAR (50)  NOT NULL,
    [Areaname]  NVARCHAR (100) NOT NULL,
    [AreaType]  CHAR (1)       NULL,
    CONSTRAINT [PK_Areas] PRIMARY KEY CLUSTERED ([Areaname] ASC)
);


GO
PRINT N'Creating [dbo].[AreaTypes]...';


GO
CREATE TABLE [dbo].[AreaTypes] (
    [AreaType]     CHAR (1)   NOT NULL,
    [AreaTypeName] NCHAR (50) NOT NULL,
    CONSTRAINT [PK_AreaTypes] PRIMARY KEY CLUSTERED ([AreaType] ASC)
);


GO
PRINT N'Creating [dbo].[Class]...';


GO
CREATE TABLE [dbo].[Class] (
    [SortSeq]   SMALLINT     NOT NULL,
    [Classref]  VARCHAR (5)  NOT NULL,
    [Classname] VARCHAR (30) NOT NULL,
    [ClassType] CHAR (1)     NULL,
    CONSTRAINT [PK_Class] PRIMARY KEY CLUSTERED ([Classref] ASC)
);


GO
PRINT N'Creating [dbo].[Classlink]...';


GO
CREATE TABLE [dbo].[Classlink] (
    [Hillnumber]  SMALLINT    NOT NULL,
    [Classref]    VARCHAR (5) NOT NULL,
    [ClasslinkID] INT         IDENTITY (1, 1) NOT NULL,
    CONSTRAINT [PK_Classlink] PRIMARY KEY CLUSTERED ([ClasslinkID] ASC)
);


GO
PRINT N'Creating [dbo].[HillAscent]...';


GO
CREATE TABLE [dbo].[HillAscent] (
    [AscentID]   INT      IDENTITY (1, 1) NOT NULL,
    [AscentDate] DATETIME NOT NULL,
    [Hillnumber] SMALLINT NOT NULL,
    [WalkID]     INT      NOT NULL,
    CONSTRAINT [PK_HillAscent] PRIMARY KEY CLUSTERED ([AscentID] ASC)
);


GO
PRINT N'Creating [dbo].[Hills]...';


GO
CREATE TABLE [dbo].[Hills] (
    [Hillnumber]       SMALLINT       NOT NULL,
    [Hillname]         NVARCHAR (100) NOT NULL,
    [_Section]         FLOAT (53)     NULL,
    [Classification]   NVARCHAR (50)  NULL,
    [Metres]           FLOAT (53)     NOT NULL,
    [Feet]             FLOAT (53)     NULL,
    [Gridref]          NVARCHAR (8)   NULL,
    [Gridref10]        NVARCHAR (14)  NULL,
    [Colgridref]       NVARCHAR (40)  NULL,
    [Colheight]        FLOAT (53)     NULL,
    [Drop]             FLOAT (53)     NULL,
    [Feature]          NVARCHAR (255) NULL,
    [Observations]     NVARCHAR (255) NULL,
    [Survey]           NVARCHAR (255) NULL,
    [Revision]         DATETIME       NULL,
    [Comments]         NVARCHAR (255) NULL,
    [Map]              NVARCHAR (15)  NULL,
    [Map25]            NVARCHAR (20)  NULL,
    [Xcoord]           INT            NULL,
    [Ycoord]           INT            NULL,
    [Latitude]         FLOAT (53)     NULL,
    [Longitude]        FLOAT (53)     NULL,
    [NumberOfAscents]  SMALLINT       NOT NULL,
    [FirstClimbedDate] DATETIME       NULL,
    CONSTRAINT [PK_Hills] PRIMARY KEY CLUSTERED ([Hillnumber] ASC)
);


GO
PRINT N'Creating [dbo].[Marker]...';


GO
CREATE TABLE [dbo].[Marker] (
    [MarkerID]             INT           IDENTITY (1, 1) NOT NULL,
    [MarkerTitle]          VARCHAR (100) NOT NULL,
    [Hillnumber]           SMALLINT      NULL,
    [GPS_Reference]        NCHAR (14)    NULL,
    [Location_Description] TEXT          NULL,
    [WalkID]               INT           NULL,
    [DateLeft]             DATETIME      NOT NULL,
    [Status]               NCHAR (40)    NOT NULL,
    CONSTRAINT [PK_Marker] PRIMARY KEY CLUSTERED ([MarkerID] ASC)
);


GO
PRINT N'Creating [dbo].[Marker_Observation]...';


GO
CREATE TABLE [dbo].[Marker_Observation] (
    [MarkerObservationID] INT      IDENTITY (1, 1) NOT NULL,
    [MarkerID]            INT      NOT NULL,
    [FoundFlag]           BIT      NOT NULL,
    [WalkID]              INT      NOT NULL,
    [ObservationText]     TEXT     NOT NULL,
    [DateOfObservation]   DATETIME NOT NULL,
    CONSTRAINT [PK_Marker_Observation] PRIMARY KEY CLUSTERED ([MarkerObservationID] ASC)
);


GO
PRINT N'Creating [dbo].[Marker_Status]...';


GO
CREATE TABLE [dbo].[Marker_Status] (
    [Marker_Status] NCHAR (40) NOT NULL,
    CONSTRAINT [PK_Marker_Status] PRIMARY KEY CLUSTERED ([Marker_Status] ASC)
);


GO
PRINT N'Creating [dbo].[MarylynParentChild]...';


GO
CREATE TABLE [dbo].[MarylynParentChild] (
    [MarlylnParentChildID] INT       IDENTITY (1, 1) NOT NULL,
    [ParentRegionAreaRef]  CHAR (10) NOT NULL,
    [ChildRegionAreaRef]   CHAR (10) NOT NULL
);


GO
PRINT N'Creating [dbo].[Walk_AssociatedFile_Types]...';


GO
CREATE TABLE [dbo].[Walk_AssociatedFile_Types] (
    [Walk_AssociatedFile_Type] VARCHAR (40) NOT NULL,
    CONSTRAINT [PK_Walk_AssociatedFile_Types] PRIMARY KEY CLUSTERED ([Walk_AssociatedFile_Type] ASC)
);


GO
PRINT N'Creating [dbo].[Walk_AssociatedFiles]...';


GO
CREATE TABLE [dbo].[Walk_AssociatedFiles] (
    [Walk_AssociatedFileID]        INT           IDENTITY (1, 1) NOT NULL,
    [WalkID]                       INT           NOT NULL,
    [Walk_AssociatedFile_Name]     VARCHAR (200) NOT NULL,
    [Walk_AssociatedFile_Type]     VARCHAR (40)  NOT NULL,
    [Walk_AssociatedFile_Sequence] SMALLINT      NOT NULL,
    [Walk_AssociatedFile_Caption]  TEXT          NULL,
    [Walk_AssociatedFile_MarkerID] INT           NULL,
    CONSTRAINT [PK_Walk_AssociatedFiles] PRIMARY KEY CLUSTERED ([Walk_AssociatedFileID] ASC)
);


GO
PRINT N'Creating [dbo].[Walks]...';


GO
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
    CONSTRAINT [PK_Walks] PRIMARY KEY CLUSTERED ([WalkID] ASC)
);


GO
PRINT N'Creating [dbo].[WalkTypes]...';


GO
CREATE TABLE [dbo].[WalkTypes] (
    [WalkTypeString] NVARCHAR (30) NOT NULL,
    CONSTRAINT [PK_WalkTypes] PRIMARY KEY CLUSTERED ([WalkTypeString] ASC)
);


GO
PRINT N'Creating [dbo].[DF_Hills_NumberOfAscents]...';


GO
ALTER TABLE [dbo].[Hills]
    ADD CONSTRAINT [DF_Hills_NumberOfAscents] DEFAULT ((0)) FOR [NumberOfAscents];


GO
PRINT N'Creating [dbo].[FK_Arealink_Hills]...';


GO
ALTER TABLE [dbo].[Arealink] WITH NOCHECK
    ADD CONSTRAINT [FK_Arealink_Hills] FOREIGN KEY ([Hillnumber]) REFERENCES [dbo].[Hills] ([Hillnumber]);


GO
PRINT N'Creating [dbo].[FK_Areas_AreaTypes]...';


GO
ALTER TABLE [dbo].[Areas] WITH NOCHECK
    ADD CONSTRAINT [FK_Areas_AreaTypes] FOREIGN KEY ([AreaType]) REFERENCES [dbo].[AreaTypes] ([AreaType]);


GO
PRINT N'Creating [dbo].[FK_Classlink_Class]...';


GO
ALTER TABLE [dbo].[Classlink] WITH NOCHECK
    ADD CONSTRAINT [FK_Classlink_Class] FOREIGN KEY ([Classref]) REFERENCES [dbo].[Class] ([Classref]);


GO
PRINT N'Creating [dbo].[FK_Classlink_Hills]...';


GO
ALTER TABLE [dbo].[Classlink] WITH NOCHECK
    ADD CONSTRAINT [FK_Classlink_Hills] FOREIGN KEY ([Hillnumber]) REFERENCES [dbo].[Hills] ([Hillnumber]);


GO
PRINT N'Creating [dbo].[FK_HillAscent_Hills]...';


GO
ALTER TABLE [dbo].[HillAscent] WITH NOCHECK
    ADD CONSTRAINT [FK_HillAscent_Hills] FOREIGN KEY ([Hillnumber]) REFERENCES [dbo].[Hills] ([Hillnumber]);


GO
PRINT N'Creating [dbo].[FK_HillAscent_Walks]...';


GO
ALTER TABLE [dbo].[HillAscent] WITH NOCHECK
    ADD CONSTRAINT [FK_HillAscent_Walks] FOREIGN KEY ([WalkID]) REFERENCES [dbo].[Walks] ([WalkID]);


GO
PRINT N'Creating [dbo].[FK_Marker_Hills]...';


GO
ALTER TABLE [dbo].[Marker] WITH NOCHECK
    ADD CONSTRAINT [FK_Marker_Hills] FOREIGN KEY ([Hillnumber]) REFERENCES [dbo].[Hills] ([Hillnumber]);


GO
PRINT N'Creating [dbo].[FK_Marker_Marker_Status]...';


GO
ALTER TABLE [dbo].[Marker] WITH NOCHECK
    ADD CONSTRAINT [FK_Marker_Marker_Status] FOREIGN KEY ([Status]) REFERENCES [dbo].[Marker_Status] ([Marker_Status]);


GO
PRINT N'Creating [dbo].[FK_Marker_Walks]...';


GO
ALTER TABLE [dbo].[Marker] WITH NOCHECK
    ADD CONSTRAINT [FK_Marker_Walks] FOREIGN KEY ([WalkID]) REFERENCES [dbo].[Walks] ([WalkID]);


GO
PRINT N'Creating [dbo].[FK_Marker_Observation_Marker]...';


GO
ALTER TABLE [dbo].[Marker_Observation] WITH NOCHECK
    ADD CONSTRAINT [FK_Marker_Observation_Marker] FOREIGN KEY ([MarkerID]) REFERENCES [dbo].[Marker] ([MarkerID]);


GO
PRINT N'Creating [dbo].[FK_Marker_Observation_Walks]...';


GO
ALTER TABLE [dbo].[Marker_Observation] WITH NOCHECK
    ADD CONSTRAINT [FK_Marker_Observation_Walks] FOREIGN KEY ([WalkID]) REFERENCES [dbo].[Walks] ([WalkID]);


GO
PRINT N'Creating [dbo].[FK_Walk_AssociatedFiles_Marker]...';


GO
ALTER TABLE [dbo].[Walk_AssociatedFiles] WITH NOCHECK
    ADD CONSTRAINT [FK_Walk_AssociatedFiles_Marker] FOREIGN KEY ([Walk_AssociatedFile_MarkerID]) REFERENCES [dbo].[Marker] ([MarkerID]);


GO
PRINT N'Creating [dbo].[FK_Walk_AssociatedFiles_Walk_AssociatedFile_Types]...';


GO
ALTER TABLE [dbo].[Walk_AssociatedFiles] WITH NOCHECK
    ADD CONSTRAINT [FK_Walk_AssociatedFiles_Walk_AssociatedFile_Types] FOREIGN KEY ([Walk_AssociatedFile_Type]) REFERENCES [dbo].[Walk_AssociatedFile_Types] ([Walk_AssociatedFile_Type]);


GO
PRINT N'Creating [dbo].[FK_Walk_AssociatedFiles_Walks]...';


GO
ALTER TABLE [dbo].[Walk_AssociatedFiles] WITH NOCHECK
    ADD CONSTRAINT [FK_Walk_AssociatedFiles_Walks] FOREIGN KEY ([WalkID]) REFERENCES [dbo].[Walks] ([WalkID]);


GO
PRINT N'Creating [dbo].[FK_Walks_Areas]...';


GO
ALTER TABLE [dbo].[Walks] WITH NOCHECK
    ADD CONSTRAINT [FK_Walks_Areas] FOREIGN KEY ([WalkAreaName]) REFERENCES [dbo].[Areas] ([Areaname]);


GO
PRINT N'Creating [dbo].[FK_Walks_WalkTypes]...';


GO
ALTER TABLE [dbo].[Walks] WITH NOCHECK
    ADD CONSTRAINT [FK_Walks_WalkTypes] FOREIGN KEY ([WalkType]) REFERENCES [dbo].[WalkTypes] ([WalkTypeString]);


GO
PRINT N'Creating [dbo].[sp_DeleteWalk]...';


GO



-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[sp_DeleteWalk]
@WalkID as integer
AS
BEGIN

delete from Walk_AssociatedFiles where WalkID=@WalkID
delete from Marker where WalkID=@WalkID
delete from HillAscent where WalkID=@WalkID
delete from Walks where WalkID=@WalkID


END
GO
PRINT N'Creating [dbo].[sp_GetMyProgress]...';


GO
CREATE procedure dbo.sp_GetMyProgress
as

declare @sortseq int
declare @classref varchar(5)
declare @classname varchar(30)

set @sortseq=1
set @classref = (select Classref from Class where SortSeq = @sortseq)
set @classname = (select Classname from Class where SortSeq = @sortseq)

create table #myprogress (
NumberClimbed int,
TotalHills int,
ClassRef varchar(5),
ClassName varchar(30) )

while @sortseq is not null
begin

	set @sortseq = (select min(SortSeq) from Class where sortseq > @sortseq)
	set @classref = (select Classref from Class where sortseq = @sortseq)
	set @classname = (select Classname from Class where sortseq = @sortseq)
	
	insert into #myprogress(NumberClimbed, TotalHills, ClassRef, ClassName)
	  select 
		(select count(Hillnumber)
		from Hills
		where FirstClimbedDate is not null
		and Hillnumber in ( select Hillnumber from Classlink where Classref=@classref))

		
	, 
		
		(
		select count(Hillnumber)
		from Hills
		where  Hillnumber in ( select Hillnumber from Classlink where Classref=@classref )
		) 
	,

	@classref,
	@classname

	

end

select * from #myprogress order by ClassName asc


drop table #myprogress
GO
PRINT N'Creating [dbo].[sp_GetMyProgressByClassType]...';


GO
CREATE procedure [dbo].[sp_GetMyProgressByClassType]
@ClassType as char
as

declare @sortseq int
declare @classref varchar(5)
declare @classname varchar(30)

set @sortseq=( select min(SortSeq) from Class where ClassType=@ClassType)
set @classref = (select Classref from Class where SortSeq = @sortseq  and classtype=@ClassType)
set @classname = (select Classname from Class where Sortseq = @sortseq and classtype=@ClassType)

print 'sortseq =' + cast(@sortseq as varchar(5)) + ' classref=' + @classref + ' classname=' + @classname


create table #myprogress (
NumberClimbed int,
TotalHills int,
ClassRef varchar(5),
ClassName varchar(30) )

while @sortseq is not null
begin
	
	insert into #myprogress(NumberClimbed, TotalHills, ClassRef, ClassName)
	  select 
		(select count(Hillnumber)
		from Hills
		where FirstClimbedDate is not null
		and Hillnumber in ( select Hillnumber from Classlink where Classref=@classref))

		
	, 
		
		(
		select count(Hillnumber)
		from Hills
		where  Hillnumber in ( select Hillnumber from Classlink where Classref=@classref )
		) 
	,

	@classref,
	@classname

    set @sortseq = (select min(SortSeq) from Class where SortSeq > @sortseq and ClassType=@ClassType)
	set @classref = (select Classref from Class where SortSeq = @sortseq and ClassType=@ClassType)
	set @classname = (select Classname from Class where SortSeq = @sortseq and ClassType=@ClassType)
	
	print 'sortseq =' + cast(@sortseq as varchar(5)) + ' classref=' + @classref + ' classname=' + @classname


end

select * from #myprogress order by ClassName asc


drop table #myprogress
GO
PRINT N'Creating [dbo].[sp_InsertAreaTypes]...';


GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[sp_InsertAreaTypes]
AS
BEGIN
declare @Arearef nvarchar(100)
declare @Areatype char(1)
declare @areaname nvarchar(100)

Declare areascursor cursor for
  select Arearef,Areaname from Areas

open areascursor

fetch next from areascursor into @Arearef,@areaname

while @@FETCH_STATUS = 0 
Begin

    Set @Areatype = substring(@Arearef,1,1)
	print 'Area: ' + @areaname + ' ref: ' + @Arearef + '  type: ' + @Areatype

	update Areas
       set AreaType=@Areatype
    where Areaname=@areaname

    fetch next from areascursor into @Arearef,@areaname

End

close areascursor
deallocate areascursor

END
GO
PRINT N'Creating Permission...';


GO
GRANT CONNECT TO [IIS APPPOOL\ASP.NET v4.0]
    AS [dbo];


GO
PRINT N'Creating Permission...';


GO
GRANT EXECUTE
    ON OBJECT::[dbo].[sp_GetMyProgress] TO [guest]
    AS [dbo];


GO
PRINT N'Creating Permission...';


GO
GRANT EXECUTE
    ON OBJECT::[dbo].[sp_GetMyProgressByClassType] TO [guest]
    AS [dbo];


GO
PRINT N'Creating Permission...';


GO
GRANT EXECUTE
    ON OBJECT::[dbo].[sp_GetMyProgressByClassType] TO [IIS APPPOOL\DefaultAppPool];


GO
PRINT N'Checking existing data against newly created constraints';


GO
USE [$(DatabaseName)];


GO
ALTER TABLE [dbo].[Arealink] WITH CHECK CHECK CONSTRAINT [FK_Arealink_Hills];

ALTER TABLE [dbo].[Areas] WITH CHECK CHECK CONSTRAINT [FK_Areas_AreaTypes];

ALTER TABLE [dbo].[Classlink] WITH CHECK CHECK CONSTRAINT [FK_Classlink_Class];

ALTER TABLE [dbo].[Classlink] WITH CHECK CHECK CONSTRAINT [FK_Classlink_Hills];

ALTER TABLE [dbo].[HillAscent] WITH CHECK CHECK CONSTRAINT [FK_HillAscent_Hills];

ALTER TABLE [dbo].[HillAscent] WITH CHECK CHECK CONSTRAINT [FK_HillAscent_Walks];

ALTER TABLE [dbo].[Marker] WITH CHECK CHECK CONSTRAINT [FK_Marker_Hills];

ALTER TABLE [dbo].[Marker] WITH CHECK CHECK CONSTRAINT [FK_Marker_Marker_Status];

ALTER TABLE [dbo].[Marker] WITH CHECK CHECK CONSTRAINT [FK_Marker_Walks];

ALTER TABLE [dbo].[Marker_Observation] WITH CHECK CHECK CONSTRAINT [FK_Marker_Observation_Marker];

ALTER TABLE [dbo].[Marker_Observation] WITH CHECK CHECK CONSTRAINT [FK_Marker_Observation_Walks];

ALTER TABLE [dbo].[Walk_AssociatedFiles] WITH CHECK CHECK CONSTRAINT [FK_Walk_AssociatedFiles_Marker];

ALTER TABLE [dbo].[Walk_AssociatedFiles] WITH CHECK CHECK CONSTRAINT [FK_Walk_AssociatedFiles_Walk_AssociatedFile_Types];

ALTER TABLE [dbo].[Walk_AssociatedFiles] WITH CHECK CHECK CONSTRAINT [FK_Walk_AssociatedFiles_Walks];

ALTER TABLE [dbo].[Walks] WITH CHECK CHECK CONSTRAINT [FK_Walks_Areas];

ALTER TABLE [dbo].[Walks] WITH CHECK CHECK CONSTRAINT [FK_Walks_WalkTypes];


GO
PRINT N'Update complete.';


GO