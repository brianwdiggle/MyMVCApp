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
GRANT EXECUTE
    ON OBJECT::[dbo].[sp_GetMyProgress] TO [guest]
    AS [dbo];

