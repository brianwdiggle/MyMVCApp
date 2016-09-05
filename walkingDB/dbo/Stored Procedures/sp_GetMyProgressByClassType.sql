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
GRANT EXECUTE
    ON OBJECT::[dbo].[sp_GetMyProgressByClassType] TO [guest]
    AS [dbo];

