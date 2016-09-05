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
