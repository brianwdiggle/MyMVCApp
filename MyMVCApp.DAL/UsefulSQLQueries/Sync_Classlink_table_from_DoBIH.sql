use [walkingDB_DoBIHv18-v2]

/* This script will read through the Classlink table in the latest version of the database of british and irish hills database 
  and add rows to the Classlink table in walkingDB
   */

DECLARE @Hillnumber INT
DECLARE @Classref VARCHAR(5)

DECLARE @classlinkcursor CURSOR

SET @classlinkcursor = CURSOR FOR
SELECT Hillnumber, Classref 
FROM [DoBIH_v18].dbo.Classlink 
ORDER BY Hillnumber, Classref

/* delete all the existing classlinks from walkingDB */

TRUNCATE TABLE Classlink

DBCC CHECKIDENT('Classlink',RESEED,1)

OPEN @classlinkcursor
FETCH NEXT
FROM @classlinkcursor INTO @Hillnumber, @Classref
WHILE @@FETCH_STATUS = 0
BEGIN
	PRINT 'Row from DoBIH: Hillnumber=' + CAST(@Hillnumber AS varchar(6)) + ' Classref=' + @classref
		BEGIN
			PRINT '   ....inserting'
			INSERT INTO [walkingDB_DoBIHv18-v2].dbo.Classlink(Hillnumber, Classref) VALUES(@Hillnumber, @classref)
		END
		
    FETCH NEXT
	FROM @classlinkcursor INTO @Hillnumber, @Classref
	PRINT ''
END

CLOSE @classlinkcursor
DEALLOCATE @classlinkcursor


