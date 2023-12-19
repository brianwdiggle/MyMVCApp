use walkingDB_DoBIHv18

/* This script will read through the Class table in the latest version of the database of british and irish hills database 
   and add any rows it does not find in the walking database 
   Where the classref already exists in walkingDB, the Sorteq is updated to match DoBIH
   */

DECLARE @classname VARCHAR(30)
DECLARE @sortseq SMALLINT
DECLARE @classref VARCHAR(5)

DECLARE @getclasses CURSOR

DECLARE @classref_walkingDB VARCHAR(5)

SET @getclasses = CURSOR FOR
SELECT SortSeq, Classref, Classname 
FROM [DoBIH_v18].dbo.Class 
ORDER BY Classref

OPEN @getclasses
FETCH NEXT
FROM @getclasses INTO @sortseq, @classref, @classname
WHILE @@FETCH_STATUS = 0
BEGIN
	PRINT 'Row from DoBIH: SortSeq=' + CAST(@sortseq AS varchar(3)) + ' Classref=' + @classref + ' Classname=' + @classname
	SET @classref_walkingDB =  (SELECT Classref FROM walkingDB_DoBIHv18.dbo.Class WHERE Classref=@classref) 

	IF @classref_walkingDB IS NULL 
		BEGIN
			PRINT '   This needs adding to walkingDB....inserting'
			INSERT INTO [walkingDB_DoBIHv18].dbo.Class(SortSeq,Classref, Classname) VALUES(@sortseq, @classref, @classname)
		END
	ELSE
	BEGIN
		PRINT '   Already present..updating SortSeq'
		UPDATE [walkingDB_DoBIHv18].dbo.Class set SortSeq=@sortseq WHERE Classref=@classref
	END
		
    FETCH NEXT
    FROM @getclasses INTO @sortseq, @classref, @classname
	PRINT ''
END

CLOSE @getclasses
DEALLOCATE @getclasses


