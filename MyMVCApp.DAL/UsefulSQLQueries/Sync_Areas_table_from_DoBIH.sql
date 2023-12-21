use [walkingDB_DoBIHv18-v2]

begin transaction

/* Add Tumps by Topo section to the Areas table
   */

DECLARE @country NVARCHAR(255)
DECLARE @arearef NVARCHAR(255)
DECLARE @shortname VARCHAR(255)
DECLARE @areaname VARCHAR(255)

DECLARE @areascursor CURSOR
DECLARE @countrycode CHAR(2)
DECLARE @match_arearef CHAR(10)

SET @areascursor = CURSOR FOR
SELECT Country, Arearef, Shortname, Areaname 
FROM [DoBIH_v18].dbo.Areas 
WHERE Left(Arearef,1)='T'

OPEN @areascursor

FETCH NEXT
FROM @areascursor INTO @country, @arearef, @shortname, @areaname

WHILE @@FETCH_STATUS = 0
BEGIN
	PRINT 'Row from DoBIH: arearef=' + @arearef 

	IF @country='E' OR @country='SE' or @country='SWEM' or @country='SWE'
		Set @countrycode = 'EN'

	IF @country='S'
		Set @countrycode = 'SC'

	IF @country='W' OR @country = 'WE'
		Set @countrycode = 'WA'

	IF @country='I'
		Set @countrycode = 'IR'

	IF @country='M' or @country='WEM'
		Set @countrycode = 'IM'

	IF @country='(all)'
		Set @countrycode = 'EN'

	IF @country='C'
		Set @countrycode = 'CH'

	Print '		Countrycode=' + @countrycode


	INSERT INTO Areas(Country,Arearef,Shortname, Areaname, AreaType)
		Values(@countrycode,@arearef, CAST(@areaname as NVARCHAR(50)), @areaname, 'T')

	FETCH NEXT
	FROM @areascursor INTO @country, @arearef, @shortname, @areaname
	
	PRINT ''
END

CLOSE @areascursor
DEALLOCATE @areascursor

select * from dbo.Areas order by AreaRef


commit transaction
