


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



