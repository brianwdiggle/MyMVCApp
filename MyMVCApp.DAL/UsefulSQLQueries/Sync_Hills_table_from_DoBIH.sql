use [walkingDB_DoBIHv18-v2]


/* 
   Update walkingDB with the latest data from the Hills table in DoBIH

   Use a cursor to read through DoBIH.Hills one row at a time.
   For Hillnumber < 5620 (existing hills in WalkingDB) do an update
   For Hillnumber >5619 to 31999 do an insert
   (Hillnumber >31999 are manually added overseas hills)
   */

DECLARE @Hillnumber SMALLINT
DECLARE @Hillname NVARCHAR(255)
DECLARE @Section FLOAT
DECLARE @Classification VARCHAR(2555)
DECLARE @Metres FLOAT 
DECLARE @Feet FLOAT
DECLARE @Gridref NVARCHAR(8)
DECLARE @Gridref10 NVARCHAR(14)
DECLARE @Colgridref NVARCHAR(40)
DECLARE @Colheight FLOAT
DECLARE @Drop FLOAT
DECLARE @Feature NVARCHAR(255)
DECLARE @Observations NVARCHAR(255)
DECLARE @Survey NVARCHAR(255)
DECLARE @Revision NVARCHAR(255)
DECLARE @Comments NVARCHAR(255)
DECLARE @Map NVARCHAR(15)
DECLARE @Map25 NVARCHAR(20)
DECLARE @Xcoord INT
DECLARE @Ycoord INT
DECLARE @Latitude FLOAT 
DECLARE @Longitude FLOAT
DECLARE @NumberOfAscents SMALLINT  /* Set this to 0 */

DECLARE @gethills CURSOR

SET @gethills = CURSOR FOR
SELECT	Hillnumber, 
		Hillname, 
		_Section, 
		[Classification], 
		Metres, 
		Feet,
		Gridref,
      Gridref10,
      Colgridref,
      Colheight,
      [Drop],
      Feature,
      Observations,
      Survey,
      Revision,
      Comments,
      Map,
      Map25,
      Xcoord,
      Ycoord,
      Latitude,
      Longitude
FROM [DoBIH_v18].dbo.Hills
ORDER BY Hillnumber

OPEN @gethills

FETCH NEXT
FROM @gethills INTO @Hillnumber, 
		@Hillname, 
		@Section, 
		@Classification, 
		@Metres, 
		@Feet,
		@Gridref,
        @Gridref10,
        @Colgridref,
        @Colheight,
        @Drop,
        @Feature,
        @Observations,
        @Survey,
        @Revision,
        @Comments,
        @Map,
        @Map25,
        @Xcoord,
        @Ycoord,
        @Latitude,
        @Longitude

WHILE @@FETCH_STATUS = 0
BEGIN
	PRINT 'Hillnumber ' + CAST(@Hillnumber AS varchar(5)) + ' Hillname=' + @Hillname


	IF @Hillnumber < 5620 
		BEGIN
			PRINT '   This Hill needs updating....'
			Update [walkingDB_DoBIHv18-v2].dbo.Hills
			Set 		
				Hillname=@Hillname,
				_Section = @Section, 
				[Classification]= @Classification,
				Metres=@Metres,
				Feet=@Feet,
				Gridref=@Gridref,
				Gridref10=Gridref10,
				Colgridref=@Colgridref,
				Colheight=@Colheight,
				[Drop]=@Drop,
				Feature=@Feature,
				Observations=@Observations,
				Survey=@Survey,
				Revision=@Revision,
				Comments=@Comments,
				Map=@Map,
				Map25=@Map25,
				Xcoord=@Xcoord,
				Ycoord=@Ycoord,
				Latitude=@Latitude,
				Longitude=@Longitude
			WHERE Hillnumber=@Hillnumber
		END
	ELSE
		BEGIN
			PRINT '   This Hill needs inserting....'
			INSERT INTO [walkingDB_DoBIHv18-v2].dbo.Hills 
				(	Hillnumber,
					Hillname, 
					_Section, 
					[Classification], 
					Metres, 
					Feet,
					Gridref,
					Gridref10,
					Colgridref,
					Colheight,
					[Drop],
					Feature,
					Observations,
					Survey,
					Revision,
					Comments,
					Map,
					Map25,
					Xcoord,
					Ycoord,
					Latitude,
					Longitude
				) 
					VALUES 
				(	@Hillnumber,
					@Hillname, 
					@Section, 
					@Classification, 
					@Metres, 
					@Feet,
					@Gridref,
					@Gridref10,
					@Colgridref,
					@Colheight,
					@Drop,
					@Feature,
					@Observations,
					@Survey,
					@Revision,
					@Comments,
					@Map,
					@Map25,
					@Xcoord,
					@Ycoord,
					@Latitude,
					@Longitude
				)
		END
		
	PRINT ''

	FETCH NEXT
	FROM @gethills INTO @Hillnumber, 
		@Hillname, 
		@Section, 
		@Classification, 
		@Metres, 
		@Feet,
		@Gridref,
        @Gridref10,
        @Colgridref,
        @Colheight,
        @Drop,
        @Feature,
        @Observations,
        @Survey,
        @Revision,
        @Comments,
        @Map,
        @Map25,
        @Xcoord,
        @Ycoord,
        @Latitude,
        @Longitude
END

CLOSE @gethills
DEALLOCATE @gethills

