/* One off script to move manually created overseas hills out of bounds of the DoBIH */


begin transaction
/* Create the overseas hills with the "out of range" hill numbers */

PRINT 'Creating new overseas hills out of range'

INSERT [dbo].[Hills] ([Hillnumber], [Hillname], [_Section], [Classification], [Metres], [Feet], [Gridref], [Gridref10], [Colgridref], [Colheight], [Drop], [Feature], [Observations], [Survey], [Revision], [Comments], [Map], [Map25], [Xcoord], [Ycoord], [Latitude], [Longitude], [NumberOfAscents], [FirstClimbedDate]) VALUES (32001, N'Gokyo Kala Pattar', NULL, NULL, 5430, 18000, NULL, NULL, NULL, NULL, 1000, N'Prayer Flag', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 1, CAST(N'1989-10-25T00:00:00.000' AS DateTime))
INSERT [dbo].[Hills] ([Hillnumber], [Hillname], [_Section], [Classification], [Metres], [Feet], [Gridref], [Gridref10], [Colgridref], [Colheight], [Drop], [Feature], [Observations], [Survey], [Revision], [Comments], [Map], [Map25], [Xcoord], [Ycoord], [Latitude], [Longitude], [NumberOfAscents], [FirstClimbedDate]) VALUES (32002, N'Carrantoohil', NULL, NULL, 1038, 3406, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 1, CAST(N'2000-09-15T00:00:00.000' AS DateTime))
INSERT [dbo].[Hills] ([Hillnumber], [Hillname], [_Section], [Classification], [Metres], [Feet], [Gridref], [Gridref10], [Colgridref], [Colheight], [Drop], [Feature], [Observations], [Survey], [Revision], [Comments], [Map], [Map25], [Xcoord], [Ycoord], [Latitude], [Longitude], [NumberOfAscents], [FirstClimbedDate]) VALUES (32003, N'Hacha Grande', NULL, NULL, 566, 1844, NULL, NULL, NULL, NULL, NULL, N'trig point', N'Small platform with pillar on summit', NULL, CAST(N'2012-02-22T00:00:00.000' AS DateTime), NULL, NULL, NULL, NULL, NULL, NULL, NULL, 1, CAST(N'2012-02-15T00:00:00.000' AS DateTime))
INSERT [dbo].[Hills] ([Hillnumber], [Hillname], [_Section], [Classification], [Metres], [Feet], [Gridref], [Gridref10], [Colgridref], [Colheight], [Drop], [Feature], [Observations], [Survey], [Revision], [Comments], [Map], [Map25], [Xcoord], [Ycoord], [Latitude], [Longitude], [NumberOfAscents], [FirstClimbedDate]) VALUES (32004, N'Montanha Caldareta', NULL, NULL, 300, 1000, NULL, NULL, NULL, NULL, NULL, N'Cairn', NULL, NULL, CAST(N'2014-04-25T00:00:00.000' AS DateTime), NULL, NULL, NULL, NULL, NULL, NULL, NULL, 1, CAST(N'2014-04-16T00:00:00.000' AS DateTime))
INSERT [dbo].[Hills] ([Hillnumber], [Hillname], [_Section], [Classification], [Metres], [Feet], [Gridref], [Gridref10], [Colgridref], [Colheight], [Drop], [Feature], [Observations], [Survey], [Revision], [Comments], [Map], [Map25], [Xcoord], [Ycoord], [Latitude], [Longitude], [NumberOfAscents], [FirstClimbedDate]) VALUES (32005, N'Gioupari', NULL, NULL, 1125, 3690, NULL, NULL, NULL, NULL, NULL, N'Broken cigarette trig', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 1, CAST(N'2015-08-24T00:00:00.000' AS DateTime))
INSERT [dbo].[Hills] ([Hillnumber], [Hillname], [_Section], [Classification], [Metres], [Feet], [Gridref], [Gridref10], [Colgridref], [Colheight], [Drop], [Feature], [Observations], [Survey], [Revision], [Comments], [Map], [Map25], [Xcoord], [Ycoord], [Latitude], [Longitude], [NumberOfAscents], [FirstClimbedDate]) VALUES (32006, N'Mount Ainos', NULL, NULL, 1627, 5337, NULL, NULL, NULL, NULL, NULL, N'Cairn and Trig point with visitors book', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 1, CAST(N'2015-08-28T00:00:00.000' AS DateTime))


PRINT 'Updating HillAscent to point at the new hills'
update HillAscent set Hillnumber=32001 where Hillnumber=5620
update HillAscent set Hillnumber=32002 where Hillnumber=5621
update HillAscent set Hillnumber=32003 where Hillnumber=5622
update HillAscent set Hillnumber=32004 where Hillnumber=5623
update HillAscent set Hillnumber=32005 where Hillnumber=5624
update HillAscent set Hillnumber=32006 where Hillnumber=5625


Print 'Updating AreaLink to point at these new hills'
update Arealink set Hillnumber=32001 where Hillnumber=5620
update Arealink set Hillnumber=32002 where Hillnumber=5621
update Arealink set Hillnumber=32003 where Hillnumber=5622
update Arealink set Hillnumber=32004 where Hillnumber=5623
update Arealink set Hillnumber=32005 where Hillnumber=5624
update Arealink set Hillnumber=32006 where Hillnumber=5625

PRINT 'Deleting the existing overseas hills where were going to cause clashes'
Delete from Hills Where Hillnumber >5619 AND Hillnumber< 6000

rollback transaction