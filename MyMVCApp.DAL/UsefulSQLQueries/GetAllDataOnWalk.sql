declare @walkid int

set @walkid=319

select max(WalkID) from dbo.Walks

select * from Walk_AssociatedFiles where WalkID=@walkid

select * from HillAscent where WalkID=@walkid

select * from Walks where WalkID=@walkid

select * from Marker where WalkID=@walkid

---------------
select * from Walks

select * from Marker where MarkerID=185


