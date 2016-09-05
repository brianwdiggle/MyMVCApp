namespace MyMVCApp.DAL
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.Data;
    using System.Linq;

    public class SqlWalkingRepository : IWalkingRepository
    {
        readonly WalkingDataContext myWalkingDB;

        //----Declare default constructor where no connection string is available
        public SqlWalkingRepository()
        {
	        this.myWalkingDB = new WalkingDataContext();
        }

        public SqlWalkingRepository(string connectionString)
        {
            this.myWalkingDB = new WalkingDataContext(connectionString);
        }

        public int AddWalk(Walk walk)
        {
	        this.myWalkingDB.Log = Console.Out;
	        this.myWalkingDB.Walks.InsertOnSubmit(walk);
	        this.myWalkingDB.SubmitChanges();
	        return walk.WalkID;

        }


  
        // --------------------------------------------------------------------------------------------------
        //  Function: SearchForWalks
        //            Search for walks based upon keywords
        // 
        //  NOTE: This only works for one keyword at present, and there is no whitelisting.
        // --------------------------------------------------------------
        public IQueryable<Walk> SearchForWalks(string searchTerms) {
            
            if (!WalkingStick.WhiteListFormInput(searchTerms)) 
            {
                return null;
            }
            IQueryable<Walk> walks = from mywalk in this.myWalkingDB.Walks
                                  where mywalk.WalkDescription.Contains(searchTerms)
                                  select mywalk;
            return walks;
        }




        //----------------------------------------------------------------------------------------------
        // Function AddWalkSummitsVisited
        // Add visited summits to a walk
        //----------------------------------------------------
        public int AddWalkSummitsVisited(List<HillAscent> summits)
        {

            int iResult = 0;

            this.myWalkingDB.HillAscents.InsertAllOnSubmit(summits);

            foreach (HillAscent oHillAscent in summits)
            {
                int iHillNumber = oHillAscent.Hillnumber;
                Hill oHill = this.myWalkingDB.Hills.SingleOrDefault(h => h.Hillnumber == iHillNumber);

                if (oHill == null)
                {
                    throw new NullReferenceException("Could not find Hill ID " + iHillNumber + " in the Hills table in the database");
                }

                if (oHill.NumberOfAscents == 0)
                {
                    oHill.FirstClimbedDate = oHillAscent.AscentDate;
                }
                else
                {
                    //---Its not the first time an ascent has been logged for this hill, but the ascent may still be the first---
                    if (oHill.FirstClimbedDate > oHillAscent.AscentDate)
                    {
                        oHill.FirstClimbedDate = oHillAscent.AscentDate;
                    }
                }

                int iNumAscents = this.GetNumberOfHillAscentsByHillID(iHillNumber);
                //TODO Change this so that the number of ascents is calculated from the number of ascents in hillascents-----
                oHill.NumberOfAscents = (Int16)(iNumAscents + 1);

            }

            this.myWalkingDB.SubmitChanges();

            return iResult;

        }

        public IQueryable<HillAscent> GetHillAscents(int iHillId)
        {

	        IQueryable<HillAscent> oAscents = from ascent in this.myWalkingDB.HillAscents 
                              where ascent.Hillnumber == iHillId
                              select ascent;

	        return oAscents;

        }

        public int GetNumberOfHillAscentsByHillID(int iHillId)
        {

            IQueryable<HillAscent> oAscents = from ascent in this.myWalkingDB.HillAscents 
                              where ascent.Hillnumber == iHillId
                              select ascent;

	        return oAscents.Count();


        }

        public List<Walk_AssociatedFile> GetWalkAssociatedFiles(int walkid)
        {

            IQueryable<Walk_AssociatedFile> oWalkAssociatedFiles = from associatedfile in this.myWalkingDB.Walk_AssociatedFiles
                                          where associatedfile.WalkID == walkid
                                          select associatedfile;

	        return oWalkAssociatedFiles.ToList();

        }


    
        public IQueryable<Walk_AssociatedFile> GetWalkAuxilliaryFiles(int walkid)
        {

            IQueryable<Walk_AssociatedFile> walkAssociatedFiles =
                from associatedfile in this.myWalkingDB.Walk_AssociatedFiles
                where associatedfile.WalkID == walkid && associatedfile.Walk_AssociatedFile_Type != "Image"
                select associatedfile;

            return walkAssociatedFiles;
        }

    
        // ----------------------------------------------------------------------------------------------
        //  Function AddWalkAssociatedFiles
        //  Add assoicated files to a walk
        // ----------------------------------------------------
        public int AddWalkAssociatedFiles(List<Walk_AssociatedFile> collAssociatedFiles) {
        
            this.myWalkingDB.Walk_AssociatedFiles.InsertAllOnSubmit(collAssociatedFiles);
            this.myWalkingDB.SubmitChanges();
            return 0;
        }

        public void DeleteHill(Walk walk)
        {
        }


    
        public int DeleteHillAscentsForWalk(int iWalkId)
        {

            IEnumerable<HillAscent> existingHillAscents = from myHillAscent in this.myWalkingDB.HillAscents
                                                          where myHillAscent.WalkID == iWalkId
                                                          select myHillAscent;
            this.myWalkingDB.HillAscents.DeleteAllOnSubmit(existingHillAscents);
            this.myWalkingDB.SubmitChanges();
            return 0;
        }

        public int DeleteAssociateFilesForWalk(int iWalkId)
        {

            IEnumerable<Walk_AssociatedFile> existingAssocFiles = from myAssocFile in this.myWalkingDB.Walk_AssociatedFiles
                                                                  where myAssocFile.WalkID == iWalkId
                                                                  select myAssocFile;

            this.myWalkingDB.Walk_AssociatedFiles.DeleteAllOnSubmit(existingAssocFiles);
            this.myWalkingDB.SubmitChanges();
            return 0;
        }




        public void DeleteMarker(Marker marker)
        {
        }


        public void DeleteWalk(Walk walk)
        {
        }

        public IQueryable<Hill> FindHillsByArea(string strAreaReference)
        {
	        //---A little awkward due to the arealink table--------

	        //----Does this get all hills above 2500 feet? Note h. includes associated information link arealink - how to use?
	        //----Example of using a linq language integrated query rather than a lambda expression to get the records

            IQueryable<Hill> hills = from hillarealink in this.myWalkingDB.Arealinks
                           where hillarealink.Arearef == strAreaReference
                           join hill in this.myWalkingDB.Hills on hillarealink.Hillnumber equals hill.Hillnumber
                           orderby hill.Hillname
                           select hill;
	        
	        return hills;
        }

        public IQueryable<Hill> FindHillsAboveFeet(int iFeet)
        {
	        //---A little awkward due to the arealink table--------

	        //----Does this get all hills above 2500 feet? Note h. includes associated information link arealink - how to use?
	        //----Example of using a linq language integrated query rather than a lambda expression to get the records

            IQueryable<Hill> q = from h in this.myWalkingDB.Hills
                       where h.Feet > iFeet
                       select h;

	        return q;

        }

        //----------------------------------------------
        //
        // Function: FindWalksByArea
        //
        // Returns a list of walks selected by walking area. IQueryable enables further downstream processing
        //
        //-------------------------------------------------------------

        public IQueryable<Walk> FindWalksByArea(string strAreaName)
        {

            return this.myWalkingDB.Walks.Where(w => w.WalkAreaName == strAreaName);

        }

        public IQueryable<Hill> FindAllHills()
        {
            return this.myWalkingDB.Hills;
        }

        //------------------------------------------------------------------------------------------------------------
        // Function: GetAllWalkingAreas
        // Returns a list of walks selected by walking area. IQueryable enables further downstream processing
        //-------------------------------------------------------------

        public IQueryable<Area> GetAllWalkingAreas()
        {
            return this.myWalkingDB.Areas;
        }

   
    // ------------------------------------------------------------------------------------------------------------
    //  Function: GetAllWalkingArea(strCoutry)
    //  Overloaded method for getting all walking areas with specified country code. Returns a listing of all walking areas
    // -----------------------------------------------------------
        public IQueryable<Area> GetAllWalkingAreas(string strCountryCode)
        {
      
            return this.myWalkingDB.Areas.Where(area => area.Country==strCountryCode).OrderBy(area => area.Shortname);
        }

        public string GetWalkAreaTypeNameFromType(string strAreaType)
        {
            AreaType oAreaType = this.myWalkingDB.AreaTypes.SingleOrDefault(at => at.AreaType1.Equals(strAreaType));
            return oAreaType.AreaTypeName;
        }

        //------------------------------------------------------------------------------------------------------------
        // Function: GetAllWalkingArea(strCoutry)
        // Overloaded method for getting all walking areas with specified country code and area type. Returns a listing of all walking areas
        //-----------------------------------------------------------

        public IQueryable<Area> GetAllWalkingAreas(string strCountryCode, string strAreaType)
        {

            return this.myWalkingDB.Areas.Where(area => area.Country == strCountryCode && area.AreaType.Equals(strAreaType)).OrderBy( area => area.Shortname);

        }

        public IQueryable<Area> FindWalkAreasByNameLike(string strNamePortion)
        {

            IQueryable<Area> q = from h in this.myWalkingDB.Areas where h.Areaname.Contains(strNamePortion) select h;
	        return q;

        }

        //---------------------------------------------------------------------------------------------
        // Function: FindWalksByTitle
        // Used by edit marker for AJAX walk name suggestions
        //--------------------------------------------------------
        public IQueryable<Walk> FindWalksByTitleLike(string strTitlePortion)
        {

            IQueryable<Walk> q = from w in this.myWalkingDB.Walks where w.WalkTitle.Contains(strTitlePortion) select w;
	        return q;

        }


        public string GetWalkAreaNameFromAreaRef(string strAreaRef)
        {

            if (strAreaRef == null)
            {
                return "";
            }
            Area oArea;

            try
            {
                oArea = this.myWalkingDB.Areas.FirstOrDefault( a => a.Arearef == strAreaRef);
            }
            catch (Exception)
            {
                oArea = new Area();
            }

            return oArea.Areaname.Trim() + " (" + oArea.AreaType1.AreaTypeName + " region)";

        }


        public IQueryable<Marker> FindAllMarkers()
        {
            return this.myWalkingDB.Markers;

        }

        //------------------------------------------------------------------------------------------------------------
        //
        // Function: FindAllWalks
        //
        // Returns an IQueryable list of all the walks (further processing can be accomplished down the line)
        //
        //-------------------------------------------------------------

        public IQueryable<Walk> FindAllWalks()
        {
            return this.myWalkingDB.Walks;
        }


        public List<Marker> GetMarkersCreatedOnWalk(int iWalkId)
        {

	        List<Marker> oMarkers = new List<Marker>();

            IQueryable<Marker> markerquery = from marker in this.myWalkingDB.Markers where marker.WalkID == iWalkId select marker;

	        foreach (Marker oMarker in markerquery.AsEnumerable()) {
		        oMarkers.Add(oMarker);
	        }

	        return oMarkers;

        }

        //------------------------------------------------------------------------------------------------------------
        //
        // Function: GetHillDetails
        //
        // Returns single Hill 
        //
        //-------------------------------------------------------------

        public Hill GetHillDetails(int siHillnumber)
        {

            return this.myWalkingDB.Hills.SingleOrDefault(h => h.Hillnumber == siHillnumber);

        }

 
        // ------------------------------------------------------------------------------------------------------------
        // 
        //  Function: GetHillsByClassification
        // 
        //  Returns an iQueryable list of all the hills which are of a certain classification
        // 
        // -------------------------------------------------------------
        public IQueryable<Hill> GetHillsByClassification(string strHillClassification) 
        {
   
            // ---Not as simple as that - the text in hill table given comma separated list of classifications-----
            if (strHillClassification != null)
            {
                return from hillclass in this.myWalkingDB.Classlinks
                       where hillclass.Classref == strHillClassification
                       join myHill in this.myWalkingDB.Hills on hillclass.Hillnumber equals myHill.Hillnumber
                       orderby myHill.Hillname
                       select myHill;
        
            }

            return from myHill in this.myWalkingDB.Hills orderby myHill.Hillname select myHill;
           
        }


        //------------------------------------------------------------------------------------------------------------
        // Function:GetAllHillClassifications
        // Returns an iQueryable list of all the HillClassifications
        //-------------------------------------------------------------

        public IQueryable<Class> GetAllHillClassifications()
        {
            return this.myWalkingDB.Classes;
        }

        public IQueryable<Hill> FindHillsByNameLike(string hillNamePortion)
        {
            IQueryable<Hill> hills = from h in this.myWalkingDB.Hills where h.Hillname.Contains(hillNamePortion) select h;

            return hills;
        }

        /// <summary>
        /// Returns an iQueryable list of all the Hills which contain specified substring
        /// </summary>
        /// <param name="strHillNamePortion"></param>
        /// <param name="strAreaRef"></param>
        /// <returns></returns>
        public IQueryable<Hill> FindHillsInAreaByNameLike(string strHillNamePortion, string strAreaRef)
        {
            IQueryable<Hill> hills = from hillarealink in this.myWalkingDB.Arealinks
                                     where hillarealink.Arearef.Contains(strAreaRef)
                                     join myhill in this.myWalkingDB.Hills on hillarealink.Hillnumber equals
                                         myhill.Hillnumber
                                     orderby myhill.Hillname
                                     select myhill;

            IQueryable<Hill> returnhills = from h in hills where h.Hillname.Contains(strHillNamePortion) select h;

            return returnhills;
        }


        public int CreateMarker(Marker marker)
        {
            this.myWalkingDB.Markers.InsertOnSubmit(marker);
            this.myWalkingDB.SubmitChanges();
            return marker.MarkerID;
        }

        public int AssociateMarkersWithWalk(NameValueCollection oForm, int iWalkID)
        {
            int iRetval = 0;
            var arrMarkerIDs = oForm["markers_added"].Split(':');

            // ----First associate any newly created markers (created with ajax) with the newly created walk----------------
            for (int iCounter = 0; iCounter <= arrMarkerIDs.Count() - 1; iCounter++)
            {
                if ((arrMarkerIDs[iCounter] != null
                            && (arrMarkerIDs[iCounter].Length > 0)))
                {
                    try
                    {
                        int iMarkerId = int.Parse(arrMarkerIDs[iCounter]);
                        // ----Update marker with walk ID------
                        Marker oMarker = this.myWalkingDB.Markers.SingleOrDefault(m => m.MarkerID == iMarkerId);
                        oMarker.WalkID = iWalkID;
                        // ----Write a "created" marker observation--------
                        Marker_Observation oMarkerObs = new Marker_Observation();
                        oMarkerObs.MarkerID = iMarkerId;
                        oMarkerObs.FoundFlag = false;
                        oMarkerObs.WalkID = iWalkID;
                        oMarkerObs.ObservationText = "Set in place";
                        try
                        {
                            oMarkerObs.DateOfObservation = DateTime.Parse(oForm["WalkDate"]);
                        }
                        catch (Exception)
                        {
                            oMarkerObs.DateOfObservation = DateTime.MinValue;
                        }
                        this.myWalkingDB.Marker_Observations.InsertOnSubmit(oMarkerObs);
                    }
                    catch (Exception)
                    {
                        iRetval = 1;
                    }
                }
            }
            int iImageCounter = 1;
            // ----Now write any marker observations necessary for newly added images either in create or edit-----------------------------
            while ((oForm[("imagerelpath" + iImageCounter)] != null
                        && (oForm[("imagerelpath" + iImageCounter)].Length > 0)))
            {
                if ( oForm[("imageismarker" + iImageCounter)] == "on"
                            && !arrMarkerIDs.Contains(oForm["imagemarkerid" + iImageCounter]))
                {
                    try
                    {
                        int iMarkerId = int.Parse(oForm[("imagemarkerid" + iImageCounter)]);
                        Marker oMarker = this.myWalkingDB.Markers.SingleOrDefault(m => m.MarkerID == iMarkerId);
                        // ----Write any found/not found observation. Only do so if the walk is not the walk on which the marker was created--------
                        if ((oMarker.WalkID != iWalkID))
                        {
                            Marker_Observation oMarkerObs = new Marker_Observation();
                            // ---If the marker is not yet associated with a walk then associate it with this walk
                            if ((oMarker.WalkID == null))
                            {
                                oMarker.WalkID = iWalkID;
                            }
                            oMarkerObs.MarkerID = iMarkerId;
                            oMarkerObs.WalkID = iWalkID;
                            if ((oForm[("imagemarkernotfound" + iImageCounter)] == "on"))
                            {
                                oMarker.Status = "Marker Left - Revisited, not found      ";
                                oMarkerObs.ObservationText = "Revisited but not found";
                                oMarkerObs.FoundFlag = false;
                            }
                            else
                            {
                                oMarkerObs.ObservationText = "Revisited and found";
                                oMarker.Status = "Marker Left - Found again               ";
                                oMarkerObs.FoundFlag = true;
                            }
                            try
                            {
                                oMarkerObs.DateOfObservation = DateTime.Parse(oForm["WalkDate"]);
                            }
                            catch (Exception)
                            {
                                oMarkerObs.DateOfObservation = DateTime.MinValue;
                            }
                            // ----Need to test that this observation has not already been inserted------
                            bool bAlreadyAdded = this.FindMarkerObservationLike(oMarkerObs);
                            if (!bAlreadyAdded)
                            {
                                this.myWalkingDB.Marker_Observations.InsertOnSubmit(oMarkerObs);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        throw new Exception("An error occurred when preparing the marker observation for new images:" + ex.Message, ex);
                    }
                }
                iImageCounter = (iImageCounter + 1);
            }

            int iNumExistingImages = 0;
            try
            {
                iNumExistingImages = int.Parse(oForm["numexistingimages"]);
            }
            catch (Exception)
            {
            }
            // ----Now write any marker observations necessary for existing images in edit walk-----------------------------
            for (int iExistingImageCount = 1; (iExistingImageCount <= iNumExistingImages); iExistingImageCount++)
            {
                if ((oForm[("existingimageismarker" + iExistingImageCount)] == "on")
                            && !arrMarkerIDs.Contains(oForm[("existingimagemarkerid" + iExistingImageCount)]))
                {
                    try
                    {
                        int iMarkerId = int.Parse(oForm[("existingimagemarkerid" + iExistingImageCount)]);
                        Marker oMarker = this.myWalkingDB.Markers.SingleOrDefault(m => m.MarkerID == iMarkerId);
                        // ----Write any found/not found observation. Only do so if the walk is not the walk on which the marker was created--------
                        if (((oMarker.WalkID != iWalkID)|| (oMarker.WalkID == null)))
                        {
                            // ---If the marker is not yet associated with a walk then associate it with this walk
                            if ((oMarker.WalkID == null))
                            {
                                oMarker.WalkID = iWalkID;
                            }
                            Marker_Observation oMarkerObs = new Marker_Observation();
                            oMarkerObs.MarkerID = iMarkerId;
                            oMarkerObs.WalkID = iWalkID;
                            if ((oForm[("existingimagemarkernotfound" + iExistingImageCount)] == "on"))
                            {
                                oMarker.Status = "Marker Left - Revisited, not found      ";
                                oMarkerObs.ObservationText = "Revisited but not found";
                                oMarkerObs.FoundFlag = false;
                            }
                            else
                            {
                                oMarkerObs.ObservationText = "Revisited and found";
                                oMarker.Status = "Marker Left - Found again               ";
                                oMarkerObs.FoundFlag = true;
                            }
                            try
                            {
                                oMarkerObs.DateOfObservation = DateTime.Parse(oForm["WalkDate"]);
                            }
                            catch (Exception)
                            {
                                oMarkerObs.DateOfObservation = DateTime.MinValue;
                            }
                            // ----Need to test that this observation has not already been inserted------
                            bool bAlreadyAdded = this.FindMarkerObservationLike(oMarkerObs);
                            if (!bAlreadyAdded)
                            {
                                this.myWalkingDB.Marker_Observations.InsertOnSubmit(oMarkerObs);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        throw new Exception("An error occurred when preparing the marker observation for existing images:" + ex.Message, ex);
                    }
                }
            }
            this.myWalkingDB.SubmitChanges();
            return iRetval;
        }


    
   

    
        // ------------------------------------------------------------------------------------------------------------
        //  Function: GetMarkerDetails
        //  Returns a single marker object
        // -------------------------------------------------------------
        public Marker GetMarkerDetails(int iMarkerId) {

            return this.myWalkingDB.Markers.SingleOrDefault(m => m.MarkerID == iMarkerId);
        }
    
        public IQueryable<Marker_Status> GetAllMarkerStatusOptions()
        {

            IQueryable<Marker_Status> q = from Marker_Status in this.myWalkingDB.Marker_Status select Marker_Status;

            return q;
        }
    
        public IQueryable<Marker> FindMarkersByNameLike(string strNamePortion)
        {

            IQueryable<Marker> q = from m in this.myWalkingDB.Markers where m.MarkerTitle.Contains(strNamePortion) select m;

            return q;
        }
    

        public bool FindMarkerObservationLike(Marker_Observation oMarkerObs) 
        {
            bool bFoundSameObs = false;
            IQueryable<Marker_Observation> q = from mo in this.myWalkingDB.Marker_Observations
                                               where
                                                   mo.MarkerID == oMarkerObs.MarkerID 
                                                   && mo.WalkID == oMarkerObs.WalkID
                                                   && mo.DateOfObservation == oMarkerObs.DateOfObservation
                                               select mo;
        
            if (q.Any()) {
                bFoundSameObs = true;
            }

            return bFoundSameObs;
        }
    
        // ------------------------------------------------------------------------------------------------------------
        //  Function: GetWalkDetails
        //  Returns single walk
        // -------------------------------------------------------------
        public Walk GetWalkDetails(int iWalkId) {
            return this.myWalkingDB.Walks.SingleOrDefault(w => w.WalkID==iWalkId);
        }
    
        public void UpdateHillDetails(Walk walk) {
        }
    
        public IQueryable<HillAscent> GetAllHillAscents() 
        {
            return this.myWalkingDB.HillAscents;
        }
    
        public int UpdateMarkerDetails(Marker marker, NameValueCollection oForm) 
        {
            marker.MarkerTitle = oForm["MarkerTitle"];
            marker.DateLeft = DateTime.Parse(oForm["DateLeft"]);
            marker.GPS_Reference = oForm["GPS_Reference"].Trim();
            try 
            {
                marker.Hillnumber = Int16.Parse(oForm["Hillnumber"]);
            }
            catch (Exception) 
            {
                marker.Hillnumber = marker.Hillnumber;
            }
            marker.Location_Description = oForm["Location_Description"];
            marker.Status = oForm["Status"];
            try 
            {
                marker.WalkID = int.Parse(oForm["WalkID"]);
            }
            catch (Exception) {
                marker.WalkID = marker.WalkID;
            }
            this.myWalkingDB.SubmitChanges();

            return 0;
        }


        public void UpdateWalkDetails(Walk walk, NameValueCollection oForm, string strRootPath)
        {
            //---Want to avoid a situation where form has been left overnight, and connection lost
            if (this.myWalkingDB.Connection.State != ConnectionState.Open)
            {
                this.myWalkingDB.Connection.Close();
                this.myWalkingDB.Connection.Open();
            }

            // ----Update the walk object. Unit of work pattern ensures the changes made are committed below-----
            WalkingStick.FillWalkFromFormVariables(ref walk, oForm);

            // -----Delete existing hill ascents. Do this better in future-----------------
            this.DeleteHillAscentsForWalk(walk.WalkID);

            // ---Add hill ascents as per updated form-----------------
            List<HillAscent> arHillAscents = WalkingStick.FillHillAscentsFromFormVariables(walk.WalkID, oForm);
            this.AddWalkSummitsVisited(arHillAscents);

            // ---Delete the existing associated files---------
            this.DeleteAssociateFilesForWalk(walk.WalkID);

            // ----Add updated existing associated files--------
            List<Walk_AssociatedFile> arAssociatedFiles = WalkingStick.FillExistingAssociatedFilesFromFormVariables(walk.WalkID, oForm, strRootPath);
            this.AddWalkAssociatedFiles(arAssociatedFiles);

            // ---Add the any new associated files-----
            arAssociatedFiles = WalkingStick.FillHillAssociatedFilesFromFormVariables(walk.WalkID, oForm, strRootPath);
            this.AddWalkAssociatedFiles(arAssociatedFiles);

            // ---update any markers created by ajax call with walk id, and add any marker observations----------------
            this.AssociateMarkersWithWalk(oForm, walk.WalkID);
            this.myWalkingDB.SubmitChanges();
        }

        // ------------------------------------------------------------------------------------------------------------
        //  Function: GetHillClassificationName
        //  Given a single classification code, return the classification name
        // -------------------------------------------------------------
        public string GetHillClassificationName(string strClassificationCode)
        {
     
            
            Class hillclass = this.myWalkingDB.Classes.SingleOrDefault(hc => hc.Classref == strClassificationCode);
           
            if (hillclass != null)
            {
                return hillclass.Classname;
            }

            return "Hill class not recognised";

        }

        public IQueryable<WalkType> GetWalkTypes()
        {

            IQueryable<WalkType> wts = from myWalkType in this.myWalkingDB.WalkTypes select myWalkType;

            return wts;
        }

        public IQueryable<Walk_AssociatedFile> GetAllImages()
        {
            IQueryable <Walk_AssociatedFile> walkImages= from walkImage in this.myWalkingDB.Walk_AssociatedFiles
                                                 where walkImage.Walk_AssociatedFile_Type == "Image"
                                                 select walkImage;

            return walkImages;
        }

        public IQueryable<Walk_AssociatedFile_Type> GetAssociatedFileTypes()
        {

            IQueryable<Walk_AssociatedFile_Type> q = from walktype in this.myWalkingDB.Walk_AssociatedFile_Types
                                                     select walktype;
            return q;
        }

        public List<MyProgress> GetMyProgress()
        {
            
            var mp = this.myWalkingDB.sp_GetMyProgress();
            return mp.ToList();
        }

        public List<MyProgress> GetMyProgressByClassType(char cClassType)
        {
            
            var mp = this.myWalkingDB.sp_GetMyProgressByClassType(cClassType);
            return mp.ToList();
        }

    }
}