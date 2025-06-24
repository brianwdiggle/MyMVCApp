namespace MyMVCApp.DAL
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Xml;

    using GeoUK;
    using GeoUK.Coordinates;
    using GeoUK.Ellipsoids;
    using GeoUK.Projections;
    using Convert = GeoUK.Convert;

    public class WalkingStick
    {

        public static string HillClassToLink(string strHillClass, string strLinkText, string strApplicationRoot) 
        {
            if ((strLinkText == "")) 
            {
                strLinkText = strHillClass;
                // Warning!!! Optional parameters not supported at all
            }
            return ("<a href=\"" +  strApplicationRoot + "Walks/HillsInClassification/" 
                        + (strHillClass + ("\">" 
                        + (strLinkText + "</a>"))));
        }

 
        public static string HillClassesToLinks(string strHillClasses, string strApplicationRoot) 
        {
            var oSb = new StringBuilder();
            bool bFirst = true;
            if ((strHillClasses == null)) 
            {
                return "";
            }

            foreach (string strClass in strHillClasses.Split(',')) 
            {
                if ((strClass.Trim().Length > 0)) 
                {
                    if (bFirst) 
                    {
                        bFirst = false;
                    }
                    else 
                    {
                        oSb.Append(", ");
                    }
                    oSb.Append("<a href=\"" + strApplicationRoot + "Walks/HillsInClassification/" 
                                    + (strClass + ("\">" 
                                    + (strClass + "</a>"))));
                }
            }
            return oSb.ToString();
        }

    
        public static string NumberOfAscentsAsColour(int iNumAscents) 
        {
            if ((iNumAscents == 0))
            {
                return "#91949b";
            }
           
            int iMaxR = 139;
            int iMaxG = 140;
            int iMaxB = 144;
            int iMinR = 21;
            int iMinG = 22;
            int iMinB = 62;

            int iRVal = ((iMaxR 
                            - (((iMaxR - iMinR) 
                            / 20) 
                            * iNumAscents)));
            int iGVal = ((iMaxG 
                            - (((iMaxG - iMinG) 
                            / 20) 
                            * iNumAscents)));
            int iBVal = ((iMaxB 
                            - (((iMaxB - iMinB) 
                            / 20) 
                            * iNumAscents)));
            return ("#" 
                        + (iRVal.ToString("X")) 
                        + (iGVal.ToString("X"))
                        + (iBVal.ToString("X")));
        }

        /// <summary>
        /// Given the hillascents associated with a hill, and a date - indicate which ascent number corresponds to this date
        /// </summary>
        /// <param name="oHill"></param>
        /// <returns></returns>
        public static string HillAscentNumber(List<HillAscent> oHillAscents, DateTime dDate)
        {
            string strAscentNumber="";

            List<HillAscent> orderedAscents = oHillAscents.OrderBy(o => o.AscentDate).ToList();

            for (int i = 0; i < orderedAscents.Count; i++)
            {
                if (orderedAscents[i].AscentDate == dDate)
                {
                    switch (i)
                    {
                        case 0:
                            strAscentNumber = "First";
                            break;
                        case 1:
                            strAscentNumber = "Second";
                            break;
                        case 2:
                            strAscentNumber = "Third";
                            break;
                        case 3:
                            strAscentNumber = "Fourth";
                            break;
                        default:
                            strAscentNumber = i.ToString() + "th";
                            break;
                    }

                }
            }

            if (orderedAscents[orderedAscents.Count - 1].AscentDate != dDate)
            {
                strAscentNumber = strAscentNumber + " of " + orderedAscents.Count.ToString() + " ascents.";
            } else
            {
                strAscentNumber = strAscentNumber + " (and most recent) ascent.";
            }

            return strAscentNumber;
        }
    
        public static string FormatHillSummaryAsLine(Hill oHill)
        {
            string strLine = "";
            if ((oHill == null)) 
            {
                return strLine;
            }

            strLine = oHill.Hillname + (", " 
                        + (oHill.Metres + ("m, " 
                        + (oHill.Feet + "ft, "))));

            if (!string.IsNullOrEmpty(oHill.Gridref10)  && (oHill.Gridref10.Length > 0)) 
            {
                if ((!string.IsNullOrEmpty(oHill.Gridref)) && (oHill.Gridref.Length > 0)) 
                {
                    strLine += oHill.Gridref;
                }
            }
            else 
            {
                strLine += oHill.Gridref10;
            }
            if (!(oHill.Classification == null)) 
            {
                strLine += (", " + oHill.Classification.Replace(",", " "));
            }
            return strLine;
        }


        public static string FormatWalkAreaAsLine(Area oArea) {
            string strLine = "";
            if ((oArea == null)) {
                return strLine;
            }
            strLine = (oArea.Areaname + (", Type:" 
                        + (oArea.AreaType + (", Ref:" + oArea.Arearef.TrimEnd()))));
            return strLine;
        }
    
        public static string FormatMarkerAsLine(Marker oMarker) {
            string strLine = "";
            if ((oMarker == null)) {
                return strLine;
            }
            strLine = (oMarker.MarkerTitle.Trim() + (", " 
                        + (oMarker.DateLeft.ToString("dd MMM yyyy").Trim() + (" " + oMarker.GPS_Reference.Trim()))));
            return strLine;
        }
    
        public static string FormatWalkAsLine(Walk oWalk) {
            string strLine = "";
            if ((oWalk == null)) {
                return strLine;
            }
            strLine = (oWalk.WalkTitle.Trim() + (", " + oWalk.WalkDate.ToString("dd MMM yyyy").Trim()));
            if (!(oWalk.CartographicLength == null)) {
                strLine = strLine + ", " 
                            + oWalk.CartographicLength + "Km";
            }

            if (!(oWalk.MetresOfAscent == null)) {
                strLine = strLine + ", " 
                            + (oWalk.MetresOfAscent + "m Ascent");
            }
            return strLine;
        }

  
        public static bool DetermineIfDirectoryExists(string strDirName) 
        {
            try 
            {
                var dDir = new DirectoryInfo(strDirName);
                if (!dDir.Exists) 
                {
                    return false;
                }
                return true;
            }
            catch (Exception) 
            {
                return false;
            }
        }
    
        /// <summary>
        /// Check the directory for files whose name starts with a specified prefix
        /// </summary>
        /// <param name="strRelativePath"></param>
        /// <param name="strFilenamePrefix"></param>
        /// <param name="strRootPath"></param>
        /// <param name="bAtWork"></param>
        /// <returns>Json formatted summary of results</returns>
        public static object CheckFilesInDirectory(string strAppRoot, string strRelativePath, string strFilenamePrefix, ref string strRootPath, bool bAtWork) 
        {
            var oDirInfo = new DirectoryInfo(strRootPath + strRelativePath);
            var oRegex = new Regex(("^" + (strFilenamePrefix + "[0-9]+")));
            int iNumPicturesFound = 0;
           
            var oFiles = oDirInfo.GetFiles((strFilenamePrefix + "*"));

            foreach (var oFI in oFiles) {
                if (oRegex.IsMatch(oFI.Name)) {
                    iNumPicturesFound++;
                }
            }
            var oResults = new{imagesfound = iNumPicturesFound, path = (strAppRoot + "Content/images/" + strRelativePath + "/" + strFilenamePrefix), atwork = bAtWork.ToString(), filenameprefix = strFilenamePrefix};
        
            return oResults;
        }


    /// <summary>
    /// Called from CreateAuto
    /// </summary>
    /// <param name="oWalk"></param>
    /// <param name="oForm"></param>
        public static void FillWalkFromFormVariables(ref Walk oWalk, NameValueCollection oForm) {

            int iLoc = 0;
            int iWalkTotalTime = 0;
            try {
                oWalk.WalkDate = DateTime.Parse(oForm["WalkDate"]);
            }
            catch (Exception) 
            {
                oWalk.WalkDate = DateTime.MinValue;
            }
            oWalk.WalkDescription = oForm["WalkDescription"];
            oWalk.WalkTitle = oForm["WalkTitle"];
            oWalk.WalkSummary = oForm["WalkSummary"];
            oWalk.WalkStartPoint = oForm["WalkStartPoint"];
            oWalk.WalkEndPoint = oForm["WalkEndPoint"];
            oWalk.WalkType = oForm["WalkTypes"];
            iLoc = oForm["WalkAreaName"].IndexOf(", Type:");

            if ((iLoc > 0)) {
                oWalk.WalkAreaName = oForm["WalkAreaName"].Substring(0, iLoc);
            }
            else {
                oWalk.WalkAreaName = oForm["WalkAreaName"].Trim();
            }

            try 
            {
                oWalk.CartographicLength = double.Parse(oForm["CartographicLength"]);
            }
            catch (Exception) 
            {
                oWalk.CartographicLength = null;
            }

            try 
            {
                oWalk.MetresOfAscent = Int16.Parse(oForm["MetresOfAscent"]);
            }
            catch (Exception) 
            {
                oWalk.MetresOfAscent = null;
            }

            try 
            {
                oWalk.WalkAverageSpeedKmh = double.Parse(oForm["WalkAverageSpeedKmh"]);
            }
            catch (Exception) 
            {
                oWalk.WalkAverageSpeedKmh = null;
            }

            try 
            {
                oWalk.MovingAverageKmh = double.Parse(oForm["MovingAverageKmh"]);
            }
            catch (Exception) 
            {
                oWalk.MovingAverageKmh = null;
            }

            oWalk.WalkCompanions = oForm["WalkCompanions"];
            
            try 
            {
                if ((oForm["total_time_hours"] != null && (oForm["total_time_hours"].Length > 0))) 
                {
                    iWalkTotalTime = (int.Parse(oForm["total_time_hours"]) * 60);
                }
            }
            catch (Exception) 
            {
                iWalkTotalTime = 0;
            }

            try {
                if ((oForm["total_time_mins"] != null && (oForm["total_time_mins"].Length > 0))) 
                {
                    iWalkTotalTime += int.Parse(oForm["total_time_mins"]);
                }
            }
            catch (Exception) 
            {
            }

            if ((iWalkTotalTime > 0)) {
                oWalk.WalkTotalTime = iWalkTotalTime;
            }

            //----Generate a summary of the walk based on the start, summits ascended, then the end.
            if (oForm["summary_auto"] != null) 
            {
                var oSB = new StringBuilder();
                oSB.Append(oForm["WalkStartPoint"]);
                int iCounter = 1;
                bool bContinue = true;
                int iFirstLocation;
                int iLastLocation;
                while (bContinue) {
                    if ((oForm[("VisitedSummit" + iCounter)] != null 
                                && (oForm[("VisitedSummit" + iCounter)]).Length > 0)) {
                        // ----Append {hillname}(classifications) -> -----------
                        if ((oSB.ToString().Length > 0)) {
                            oSB.Append(" -> ");
                        }
                        iFirstLocation = oForm[("VisitedSummit" + iCounter)].IndexOf(",");
                        iLastLocation = oForm[("VisitedSummit" + iCounter)].LastIndexOf(",");
                        if (((iFirstLocation < 0) 
                                    || (iLastLocation < 0))) {
                            oSB.Append(oForm[("VisitedSummit" + iCounter)]);
                        }
                        else {
                            oSB.Append((oForm[("VisitedSummit" + iCounter)]).Substring(0, iFirstLocation));
                        }
                        iCounter++;
                    }
                    else {
                        bContinue = false;
                    }
                }
                if ((oForm["WalkEndPoint"] != null 
                            && (oForm["WalkEndPoint"].Length > 0))) {
                    oSB.Append((" -> " + oForm["WalkEndPoint"]));
                }
                oWalk.WalkSummary = oSB.ToString();
            }
            else {
                oWalk.WalkSummary = oForm["WalkSummary"];
            }
            if (oForm["WalkConditions"] != null) {
                oWalk.WalkConditions = oForm["WalkConditions"];
            }
        }

        public static Marker FillMarkerFromFormVariables(Marker oMarker, NameValueCollection oForm)
        {
            var oNewMarker = new Marker
                             {
                                 MarkerTitle = oMarker.MarkerTitle,
                                 DateLeft = oMarker.DateLeft,
                                 GPS_Reference = oMarker.GPS_Reference,
                                 Hillnumber = Int16.Parse(oForm["HillID"]),
                                 Location_Description = oMarker.Location_Description,
                                 Status = oForm["MarkerStatusii"]
       
                             };

            // Allow the walk to be null - it is possible that a marker was placed when not doing a walk as such
            try
            {
                oNewMarker.WalkID = Int32.Parse(oForm["WalkID"]);
            }
            catch (Exception)
            {
                oNewMarker.WalkID = null;
            }

            return oNewMarker;
        }

    
        public static List<HillAscent> FillHillAscentsFromFormVariables(int iWalkID, NameValueCollection oForm) 
        {
        
            var collHillAscents = new List<HillAscent>();
            int iCounter = 1;
            bool bContinue = true;
            DateTime dAscentDate;
            try {
                dAscentDate = DateTime.Parse(oForm["WalkDate"]);
            }
            catch (Exception) 
            {
                dAscentDate = DateTime.MinValue;
            }

            while (bContinue) {
                if ((oForm[("VisitedSummit" + (iCounter + "HillID"))] != null && ((oForm[("VisitedSummit" + (iCounter + "HillID"))].Length > 0) 
                            && (oForm[("VisitedSummit" + iCounter)].Trim().Length > 0)))) {
                    var oHillAscent = new HillAscent
                    {
                        AscentDate = dAscentDate
                    };
                    try {
                        oHillAscent.Hillnumber = Int16.Parse(oForm[("VisitedSummit" + iCounter + "HillID")]);
                        oHillAscent.WalkID = iWalkID;
                        collHillAscents.Add(oHillAscent);
                    }
                    catch (Exception) 
                    {
                    }
                    iCounter++;
                }
                else {
                    bContinue = false;
                }
            }
            return collHillAscents;
        }


    /// <summary>
    /// Given the walk details form, prepare the DAL objects for insertion
    /// </summary>
    /// <param name="iWalkID"></param>
    /// <param name="oForm"></param>
    /// <param name="strRootpath"></param>
    /// <returns></returns>
        public static List<Walk_AssociatedFile> FillHillAssociatedFilesFromFormVariables(int iWalkID, NameValueCollection oForm, string strRootpath) 
        {
    
            var collHillAssociatedFiles = new List<Walk_AssociatedFile>();

            int iCounter = 1;
            int iNumImages;
            bool bContinue = true;
            while (bContinue) 
            {
                if ((oForm[("imagerelpath" + iCounter)] != null 
                            && (oForm[("imagerelpath" + iCounter)]).Length > 0)) {
                    var oHillAssociateFile = new Walk_AssociatedFile
                    {
                        WalkID = iWalkID,
                        Walk_AssociatedFile_Name = CleanUpAssociateFilePath(oForm[("imagerelpath" + iCounter)], "Content/images/"),
                        Walk_AssociatedFile_Type = "Image",
                        Walk_AssociatedFile_Caption = oForm[("imagecaption" + iCounter)],
                        Walk_AssociatedFile_Sequence = (short)(iCounter)
                    };
                    if ((oForm[("imageismarker" + iCounter)] != null 
                                && (oForm[("imageismarker" + iCounter)].Length > 0))) {
                        oHillAssociateFile.Walk_AssociatedFile_MarkerID = Int32.Parse(oForm[("imagemarkerid" + iCounter)]);
                    }
                    collHillAssociatedFiles.Add(oHillAssociateFile);
                    iCounter++;
                }
                else {
                    bContinue = false;
                }
            }
            iNumImages = (iCounter - 1);
            iCounter = 1;
            bContinue = true;
            while (bContinue) {
                if ((oForm[("auxilliary_file" + iCounter)] != null 
                            && (oForm[("auxilliary_file" + iCounter)].Length > 0))) 
                {
                    Walk_AssociatedFile oHillAssociateFile = new Walk_AssociatedFile
                    {
                        WalkID = iWalkID,
                        Walk_AssociatedFile_Name = CleanUpAssociateFilePath(oForm[("auxilliary_file" + iCounter)], "Content/images/"),
                        Walk_AssociatedFile_Type = oForm[("auxilliary_filetype" + iCounter)],
                        Walk_AssociatedFile_Sequence = (short)(iCounter + iNumImages),
                        Walk_AssociatedFile_Caption = oForm[("auxilliary_caption" + iCounter)]
                    };
                    collHillAssociatedFiles.Add(oHillAssociateFile);
                    iCounter++;
                }
                else {
                    bContinue = false;
                }
            }
            return collHillAssociatedFiles;
        }

        /// <summary>
        /// In particular, given a walk file name template, find the directory in which the files exist, and fill in the DAL objects
        /// </summary>
        /// <param name="iWalkID"></param>
        /// <param name="oForm"></param>
        /// <param name="strRootpath"></param>
        /// <returns></returns>
        public static List<Walk_AssociatedFile> FillWalkAssociatedFilesUsingNameTemplate(int iWalkID, NameValueCollection oForm, string strRootPath)
        {
            var collWalkAssociatedFiles = new List<Walk_AssociatedFile>();

            short siFileSequenceCounter = 1;

            string walkfiles_fullpathprefix;
            string walkfiles_nameprefix = oForm["walkfiles_nameprefix"];

            // Return an empty collection if the name prefix is not specified.
            if (walkfiles_nameprefix.Trim().Length == 0)
            {
                return collWalkAssociatedFiles;
            }

            //---Look for the first file which matches the name prefix within the web root
            string[] strAssociatedFiles = Directory.GetFiles(strRootPath,walkfiles_nameprefix + "*.*", SearchOption.AllDirectories);
 
            // Return an empty collection if the name prefix is not specified.
            if (strAssociatedFiles.Length == 0)
            {
                return collWalkAssociatedFiles;
            }

            walkfiles_fullpathprefix = strAssociatedFiles[0].Replace("\\", "/");
            int iLoc;

            //Use the first found file to get the path to the directory.
            try
            {
                iLoc = walkfiles_fullpathprefix.LastIndexOf("/", StringComparison.Ordinal);
            }
            catch (Exception)
            {
                iLoc = 0;
                return collWalkAssociatedFiles;
            }

            string strPathToWalkFiles = walkfiles_fullpathprefix.Substring(0, iLoc);
          //  walkfiles_nameprefix = walkfiles_fullpathprefix.Substring(iLoc + 1, walkfiles_fullpathprefix.Length - iLoc - 1);

            // -----Check that the path specified is valid------------------------
            if (!WalkingStick.DetermineIfDirectoryExists(strPathToWalkFiles))
            {
                throw new Exception("Could not find directory when looking for image files: dir:[" + strPathToWalkFiles + "]" + " strRootPath = [" + strRootPath + "]");
            }


            //---Should be the same from this point----------

            //-----First add all the walk image files-------PROBELM HEre - no files found
            string[] filesindir = Directory.GetFiles(strPathToWalkFiles, walkfiles_nameprefix + "_*");

            // go through looking for each number, assigning an order number to the unsorted list
            for (int imageNumber = 1; imageNumber < filesindir.Length + 1; imageNumber++)
            {
                // Look for image number imageNumber in the unordered list
                for (int iCount = 0; iCount < filesindir.Length; iCount++)
                {
                    // Construct full file name to look for - jpgs and pngs are allowed, either case
                    if (filesindir[iCount].Equals(strPathToWalkFiles + "\\" + walkfiles_nameprefix + "_" + imageNumber.ToString() + ".jpg") ||
                        filesindir[iCount].Equals(strPathToWalkFiles + "\\" + walkfiles_nameprefix + "_" + imageNumber.ToString() + ".JPG") ||
                        filesindir[iCount].Equals(strPathToWalkFiles + "\\" + walkfiles_nameprefix + "_" + imageNumber.ToString() + ".png") ||
                        filesindir[iCount].Equals(strPathToWalkFiles + "\\" + walkfiles_nameprefix + "_" + imageNumber.ToString() + ".PNG"))
                    {
                        // filesindir[iCount] = filesindir[iCount].Replace("\\","/");
                        int iLocFileExtStart = 0;
                        iLocFileExtStart = filesindir[iCount].LastIndexOf(".");
                        string strImageFileExt = filesindir[iCount].Substring(iLocFileExtStart, 4);

                        var oHillAssociateFile = new Walk_AssociatedFile
                        {
                            WalkID = iWalkID,
                            Walk_AssociatedFile_Name = CleanUpAssociateFilePath(strPathToWalkFiles + "/" + walkfiles_nameprefix + "_" + imageNumber.ToString() + strImageFileExt, "Content/images/"),
                            Walk_AssociatedFile_Type = "Image",
                            Walk_AssociatedFile_Sequence = siFileSequenceCounter++
                        };

                        collWalkAssociatedFiles.Add(oHillAssociateFile);
                        break;
                    }
                }
            }

            // Look for any auxilliary files (not walk images) present matching the nameprefix plus a hyphen
            filesindir = Directory.GetFiles(strPathToWalkFiles, walkfiles_nameprefix + "-*");

            // for each aux file, create object and add to result list.
            for (int iCount = 0; iCount < filesindir.Length; iCount++)
            {
                filesindir[iCount] = filesindir[iCount].Replace("\\", "/");

                int iLocNamePrefix = filesindir[iCount].IndexOf(walkfiles_nameprefix);
                filesindir[iCount] = filesindir[iCount].Substring(iLocNamePrefix);

                string strDesc = "";
                string strAuxFileType = "";

                var oHillAssociateFile = new Walk_AssociatedFile
                {
                    WalkID = iWalkID,
                    Walk_AssociatedFile_Name = CleanUpAssociateFilePath(strPathToWalkFiles + "/" + filesindir[iCount], "Content/images/")
                };
                strAuxFileType = DetermineAuxFileType(filesindir[iCount], walkfiles_nameprefix, ref strDesc);

                if (strAuxFileType.Length > 0)
                {
                    oHillAssociateFile.Walk_AssociatedFile_Type = strAuxFileType;
                    oHillAssociateFile.Walk_AssociatedFile_Sequence = siFileSequenceCounter++;
                    oHillAssociateFile.Walk_AssociatedFile_Caption = strDesc;

                    collWalkAssociatedFiles.Add(oHillAssociateFile);
                }
                else
                {
                    Console.WriteLine("Unexpected auxilliary file of unknown type not added to walk: " + oHillAssociateFile.Walk_AssociatedFile_Name);
                }

            }

            return collWalkAssociatedFiles;
        }


        public static string DetermineAuxFileType(string filename, string name_prefix, ref string strDescription)
        {
            string strAuxFileType = "";
     
            // create a string enum from the types in the database, if one does not exist.

            if (filename.Contains(name_prefix + "-Track"))
            {
                strDescription = ExtractDescFromAuxFileName(filename, "-Track");
                return "GPX File";
            }

            if (filename.Contains(name_prefix + "-RouteTrack"))
            {
                strDescription = ExtractDescFromAuxFileName(filename, "-RouteTrack");
                return "GPX File with Route and Track";
            }

            if (filename.Contains(name_prefix + "-Route"))
            {
                strDescription = ExtractDescFromAuxFileName(filename, "-Route");
                return "GPX File with Route";
            }

            if (filename.Contains(name_prefix + "-Marker"))
            {
                strDescription = ExtractDescFromAuxFileName(filename, "-Marker");
                return "GPX File with Marker";
            }

            if (filename.Contains(name_prefix + "-Vid"))
            {
                strDescription = ExtractDescFromAuxFileName(filename, "-Vid");
                return "MOV file";
            }

            if (filename.Contains(name_prefix + "-AltitudeProfile"))
            {
                strDescription = ExtractDescFromAuxFileName(filename, "-AltitudeProfile");
                return "Image - Altitude Profile";
            }

            if (filename.Contains(name_prefix + "-MapRoute"))
            {
                strDescription = ExtractDescFromAuxFileName(filename, "-MapRoute");
                return "Image  - Map with Route"; // note extra space after Image
            }

            if (filename.Contains(name_prefix + "-MapTrack"))
            {
                strDescription = ExtractDescFromAuxFileName(filename, "-MapTrack");
                return "Image - Map with track";
            }

            if (filename.Contains(name_prefix + "-Stats"))
            {
                strDescription = ExtractDescFromAuxFileName(filename, "-Stats");
                return "Image - Stats";
            }
            return strAuxFileType; //-- i.e. return an empty string
        }

        public static string ExtractDescFromAuxFileName(string strAuxFilename, string strAuxFiletype)
        {
            int iLocStart = 0;
            int iLocEnd;
            iLocStart = strAuxFilename.IndexOf(strAuxFiletype)+ strAuxFiletype.Length;
            iLocEnd = strAuxFilename.IndexOf(".", iLocStart);

            return strAuxFilename.Substring(iLocStart, iLocEnd - iLocStart);
        }

        /// <summary>
        /// The associated file path must start with the contents of strPathToImges, which will be the section of the full url after the web app root, to the images directory
        /// </summary>
        /// <param name="strPathToClean"></param>
        /// <param name="strPathToImages"></param>
        /// <returns></returns>
        public static string CleanUpAssociateFilePath(string strPathToClean, string strPathToImages)
        {
            string strCleanedPath = strPathToClean;

            // Ensure that we have only single forward slashes in the path
            strCleanedPath = strCleanedPath.Replace('\\','/');
            strCleanedPath = strCleanedPath.Replace("//", "/");

            // Remove anything which is found before Content/images/  (the historic content of strPathImages)
            int iLoc = strCleanedPath.IndexOf(strPathToImages);
            if (iLoc > 0)
            {
                strCleanedPath = strCleanedPath.Substring(iLoc, strCleanedPath.Length - iLoc);
            }else if (iLoc == -1)
            {  
                // Add in the path to the images directory if not present
                strCleanedPath = strPathToImages + strCleanedPath;
            }

            return strCleanedPath;
        }

    
        public static List<Walk_AssociatedFile> FillExistingAssociatedFilesFromFormVariables(int iWalkID, NameValueCollection oForm, string strRootpath) 
        {
            List<Walk_AssociatedFile> collHillAssociatedFiles = new List<Walk_AssociatedFile>();

            int iCounter = 1;
            int iNumExistingImages = 0;
            int iSequenceCounter = 1;
            bool bContinue = true;
            try 
            {
                iNumExistingImages = int.Parse(oForm["NumExistingImages"]);
            }
            catch (Exception) 
            {
            }

            for (int iExistingImagesCount = 1; (iExistingImagesCount <= iNumExistingImages); iExistingImagesCount++) {
                if ((oForm[("existingimagename" + iExistingImagesCount)] != null 
                            && ((oForm[("existingimagename" + iExistingImagesCount)].Length > 0) 
                            && (oForm[("deletexistingimage" + iExistingImagesCount)] != "on")))) 
                {
                    Walk_AssociatedFile oHillAssociateFile = new Walk_AssociatedFile
                    {
                        WalkID = iWalkID,
                        Walk_AssociatedFile_Name = oForm[("existingimagename" + iExistingImagesCount)],
                        Walk_AssociatedFile_Type = "Image",
                        Walk_AssociatedFile_Caption = oForm[("existingimagecaption" + iExistingImagesCount)],
                        Walk_AssociatedFile_Sequence = (short)iSequenceCounter
                    };
                    if ((oForm[("existingimageismarker" + iExistingImagesCount)] != null 
                                && (oForm[("existingimageismarker" + iExistingImagesCount)].Length > 0))) {
                        oHillAssociateFile.Walk_AssociatedFile_MarkerID = int.Parse(oForm[("existingimagemarkerid" + iExistingImagesCount)]);
                    }
                    iSequenceCounter++;
                    collHillAssociatedFiles.Add(oHillAssociateFile);
                }
            }

            iCounter = 1;
            bContinue = true;
            while (bContinue) {
                if ((oForm[("existingauxfilename" + iCounter)] != null 
                            && ((oForm[("existingauxfilename" + iCounter)].Length > 0) 
                            && (oForm[("delexisting_auxilliary_file" + iCounter)] != "on")))) {
                    var oHillAssociateFile = new Walk_AssociatedFile
                    {
                        WalkID = iWalkID,
                        Walk_AssociatedFile_Name = oForm[("existingauxfilename" + iCounter)],
                        Walk_AssociatedFile_Type = oForm[("existingauxfiletype" + iCounter)],
                        Walk_AssociatedFile_Sequence = (short)(iCounter + iSequenceCounter)
                    };
                    collHillAssociatedFiles.Add(oHillAssociateFile);
                    iCounter++;
                }
                else {
                    bContinue = false;
                }
            }
            return collHillAssociatedFiles;
        }
    
        public static string ConvertPathToRelativeURL(string strPath, string strRootPath) {
            int iLoc = 0;
            iLoc = strPath.IndexOf(strRootPath);
            if ((iLoc == 0)) {
                return strPath.Substring((strRootPath.Length - 1), ((strPath.Length - strRootPath.Length) 
                                + 1)).Replace("\\", "/");
            }
            else {
                return strPath;
            }
        }

        public static string CountryNameFromCountryCode(string strCountryCode) 
        {

            string strCountryname = "";
            if (strCountryCode.Equals("EN")) 
            {
                strCountryname = "England";
            }
            else if (strCountryCode.Equals("IM")) 
            {
                strCountryname = "Isle Of Man";
            }
            else if (strCountryCode.Equals("IR")) 
            {
                strCountryname = "Ireland";
            }
            else if (strCountryCode.Equals("SC")) 
            {
                strCountryname = "Scotland";
            }
            else if (strCountryCode.Equals("WA")) 
            {
                strCountryname = "Wales";
            }
            else if (strCountryCode.Equals("EW")) 
            {
                strCountryname = "England and Wales County Tops";
            }

            return strCountryname;
        }


        public static string ConvertTotalTimeToString(int? iTotalTime, bool shortForm) 
        {


            if (!iTotalTime.HasValue)
            {
                return "";
            }
            else
            {
                
            }

            if ((iTotalTime == 0)) {
                return "";
            }
            string strHour = "hour";
            string strMinute = "min";
            if (shortForm) {
                strHour = "hr";
                strMinute = "m";
            }
            string strRet = "";
            int iNumHours = iTotalTime.Value / 60;
           
            int iNumMins = (iTotalTime.Value % 60);
            if ((iNumHours > 0)) 
            {
                strRet = iNumHours.ToString();
                if ((iNumHours > 1)) {
                    strRet += (" "
                                + (strHour + "s "));
                }
                else {
                    strRet += (" " 
                                + (strHour + " "));
                }
            }
            if ((iNumMins > 0)) {
                strRet += iNumMins.ToString();
                if ((iNumMins > 1)) {
                    strRet += (" " 
                                + (strMinute + "s"));
                }
                else {
                    strRet += (" " + strMinute);
                }
            }
            return strRet;
        }


        public static string HillAscentMarkerPopup(HillAscent oHA)
        {
            string strPopupText;
            string strNumberOfAscents; ;

            switch (oHA.Hill.HillAscents.Count)
            {
                case 0:
                    strNumberOfAscents =  "Not yet ascended";
                    break;
                case 1:
                    strNumberOfAscents = " First ascent.";
                    break;
                case 2:
                    strNumberOfAscents = " Second ascent.";
                    break;
                case 3:
                    strNumberOfAscents = " Third ascent.";
                    break;
                case 4:
                    strNumberOfAscents = " Fourth ascent.";
                    break;
                case 5:
                    strNumberOfAscents = " Fifth ascent.";
                    break;
                case 6:
                    strNumberOfAscents = " Sixth ascent.";
                    break;
                default:
                    strNumberOfAscents = oHA.Hill.HillAscents.Count.ToString() + "th";
                    break;
            }

            strPopupText = "<a href=\"/Walks/HillDetails/" + oHA.Hill.Hillnumber.ToString() + "\">" + oHA.Hill.Hillname + " (" + oHA.Hill.Metres + "m, " + oHA.Hill.Feet + "ft)</a><br/>" + strNumberOfAscents;
   
            return strPopupText;
        }

        public static string MarkerObservationPopup(Marker_Observation oMO, bool bGridRef6)
        {
            string strPopupText = "";

            if (!bGridRef6)
            {
                strPopupText = "<a href=\"/Marker/Details/" + oMO.MarkerID.ToString() + "\">" + oMO.Marker.MarkerTitle + "</a>" + " " + oMO.ObservationText.ToLower();
            }else { 
                strPopupText = "<a href=\"/Marker/Details/" + oMO.MarkerID.ToString() + "\">" + oMO.Marker.MarkerTitle + "</a>" + " " + oMO.ObservationText.ToLower() + ".<br/>Location is 6 digit estimate from hill.";
            }

   

            return strPopupText;
        }

        public static string MarkerPopup(Marker oMarker, string strVirtualRoot)
        {
            string strPopupText = "";
            
            if (oMarker.GPS_Reference == null )
            {
                strPopupText = "<a href=\"/Marker/Details/" + oMarker.MarkerID.ToString() + "\">" + oMarker.MarkerTitle + "</a>" + "<br/>Placed: " + oMarker.DateLeft.ToString("dd MMM yyyy") + "</br>" + oMarker.Location_Description;
            }else if (oMarker.Hill != null && oMarker.Hill.Gridref10 !=null)
            {
                strPopupText = "<a href=\"/Marker/Details/" + oMarker.MarkerID.ToString() + "\">" + oMarker.MarkerTitle + "</a>" + "<br/>Placed: " + oMarker.DateLeft.ToString("dd MMM yyyy") + "</br>" + oMarker.Location_Description + ".<br/>Location taken from Hill";
            }else if (oMarker.Hill != null && oMarker.Hill.Gridref != null)
            {
                strPopupText = "<a href=\"/Marker/Details/" + oMarker.MarkerID.ToString() + "\">" + oMarker.MarkerTitle + "</a>" + "<br/>Placed: " + oMarker.DateLeft.ToString("dd MMM yyyy") + "</br>" + oMarker.Location_Description + ".<br/>Location taken from Hill 6 digit estimate.";
            }

            return strPopupText;
        }

        public static string HillPopup(Hill oHill)
        {
            string strPopupText;
            string strAscents;

            if (oHill.NumberOfAscents==0)
            {
                strAscents = "Unclimbed";
            }else if (oHill.NumberOfAscents == 1)
            {
                DateTime oDT = (DateTime)oHill.FirstClimbedDate;
                strAscents = "1 ascent - " + oDT.ToString("dd MMM yyyy");
            }else
            {
                DateTime oDT = (DateTime)oHill.FirstClimbedDate;
                strAscents = oHill.NumberOfAscents.ToString() + " ascents.</br>First ascent: " + oDT.ToString("dd MMM yyyy");
            }
            strPopupText = "<a href=\"/Walks/HillDetails/" + oHill.Hillnumber.ToString() + "\">" + oHill.Hillname + " (" + oHill.Metres.ToString() + "m ," + oHill.Feet.ToString() + "ft)</a>" + "<br/>" + strAscents;

            return strPopupText;
        }

        public static Boolean WhiteListFormInput(string strFormInput)
        {
            var oRegex = new Regex("^[a-zA-Z'.]{1,40}$");

            return oRegex.IsMatch(strFormInput);
        }

        public static string ExtractFileNameFromPath(string strFullPath)
        {
            int iLoc = strFullPath.LastIndexOf("/");

            return strFullPath.Substring(iLoc + 1, (strFullPath.Length - iLoc) - 1);
        }

        /// <summary>
        /// Given a gridref 10, check whether it is a valid reference
        /// </summary>
        /// <param name="strGridRef10"></param>
        /// <returns></returns>
       public static bool ValidateGridRef10(string strGridRef10)
        {
            if (string.IsNullOrEmpty(strGridRef10)) {
                return false;
            }

            string strTestValue = strGridRef10.Replace(" ","");

            // A valid value MUST be in format AA1234512345
            if (strTestValue.Length!=12)
            {
                return false;
            }

            // Test that the first two characters are alphabetic
            if (!Regex.IsMatch(strTestValue.Substring(0,2), @"^[a-zA-Z]+$"))
            {
                return false;
            }

            if (!Regex.IsMatch(strTestValue.Substring(2,10), @"^[0-9]+$"))
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Given a gridref from the Hill.gridref colum , check whether it is a valid reference
        /// It should be in format AB123123
        /// </summary>
        /// <param name="strGridRef6"></param>
        /// <returns></returns>
        public static bool ValidateGridRef6(string strGridRef6)
        {
            if (string.IsNullOrEmpty(strGridRef6))
            {
                return false;
            }

            string strTestValue = strGridRef6.Trim();

            // A valid value MUST be in format AA123123
            if (strTestValue.Length != 8)
            {
                return false;
            }

            // Test that the first two characters are alphabetic
            if (!Regex.IsMatch(strTestValue.Substring(0,2), @"^[a-zA-Z]+$"))
            {
                return false;
            }

            if (!Regex.IsMatch(strTestValue.Substring(2, 6), @"^[0-9]+$"))
            {
                return false;
            }

            return true;
        }


        /// <summary>
        /// Given a Hill, this returns the OS Grid Reference 10 digit version of the location of the marker 
        /// which is assumed to be 3 metres east of the summit location.
        /// </summary>
        /// <param name="oHill"></param>
        /// <returns></returns>
        public static string FivePacesEastFromSummit(Hill oHill)
        {
            string strGridref10 = "";
            string strLocationOfSummit = "";

            if (oHill.Gridref10 != null && oHill.Gridref10.Trim()!="")
            {
                strLocationOfSummit = oHill.Gridref10;

                string strEasting = strLocationOfSummit.Substring(3, 5);
                int iEastingOfMarker = Int32.Parse(strEasting);

                //---Only add 3M when the alphabetic part of the grid ref doesn't change
                if (iEastingOfMarker < 99997)
                {
                    iEastingOfMarker += 3;
                }

                strGridref10 = oHill.Gridref10.Substring(0, 3) + iEastingOfMarker.ToString().PadLeft(5,'0') + oHill.Gridref10.Substring(8, 6);
            }else if (oHill.Gridref!= null && oHill.Gridref.Trim() != "")
            {
                // Not worth changing this
                strGridref10 = oHill.Gridref.Substring(0, 2) + " " + oHill.Gridref.Substring(2, 3) + "00 " + oHill.Gridref.Substring(5, 3) + "00";
            }
            else
            {
                return "";
            }

            return strGridref10;
        }

        /// <summary>
        /// Convert OS Grid Ref 8 character format to full 10 numeric digit format
        /// </summary>
        /// <param name="strGridRef"></param>
        /// <returns></returns>
        public static string GridrefToGridRef10(string strGridRef)
        {
            string strGridRef10 = "";
            try
            {
                strGridRef10 = strGridRef.Substring(0, 2) + " " + strGridRef.Substring(2, 3) + "00 " + strGridRef.Substring(5, 3) + "00";
            }catch (Exception e)
            {
                Console.WriteLine("problem with gridref [" + strGridRef + "]" + e.Message);
            }
            return strGridRef10;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="strRelPath"></param>
        /// <param name="strRootPath"></param>
        /// <param name="strGPXNode"></param>
        /// <returns></returns>
        public static List<Trackpoint> LoadTrackFromGPXFile(string strRelPath, string strRootPath)
        {
            const string strOSMaps_Route_node = "//x:wpt";
            const string strOSMaps_Track_node = "//x:trkpt";
            const string strGarmin_Track_node = "//x:wpt";
            string strGPXNode = strOSMaps_Track_node;    //---Try OS MAps version first where the track ponts are 


            List<Trackpoint> trackpoints = new List<Trackpoint>();

            string strCleanedUpRelPath;

            if (strRelPath.StartsWith("/Content/images/"))
            {
                strCleanedUpRelPath = strRelPath.Replace("/Content/images/", "");
            }
            else if (strRelPath.StartsWith("Content/images/"))
            {
                strCleanedUpRelPath = strRelPath.Replace("Content/images/", "");
            }
            else
            {
                strCleanedUpRelPath = strRelPath;
            }

            
            string strFullPath = strRootPath + strCleanedUpRelPath;
            strFullPath = strFullPath.Replace("/", @"\");

            //----Load the GPX file as an XML Document
            XmlDocument gpxDoc = new XmlDocument();

            try
            {
                gpxDoc.Load(strFullPath);
            }catch(Exception)
            {
                return trackpoints;
            }
    
            XmlNamespaceManager nsmgr = new XmlNamespaceManager(gpxDoc.NameTable);
            nsmgr.AddNamespace("x", "http://www.topografix.com/GPX/1/1");
            XmlNodeList nl = gpxDoc.SelectNodes(strGPXNode, nsmgr);

            //---Pull out the trackpoints
            foreach (XmlElement xelement in nl)
            {
                Trackpoint oTP = new Trackpoint
                {
                    latitude = float.Parse(xelement.GetAttribute("lat")),
                    longtitude = float.Parse(xelement.GetAttribute("lon"))
                };

                if (xelement.HasChildNodes)
                {
                    for (int i = 0; i < xelement.ChildNodes.Count; i++)
                    {
                        if (xelement.ChildNodes[i].Name == "ele")
                        {
                            oTP.elevation = float.Parse(xelement.ChildNodes[i].InnerText);
                        }else if (xelement.ChildNodes[i].Name == "time")
                        {
                            oTP.time = DateTime.Parse(xelement.ChildNodes[i].InnerText);
                        }
                    }
                }
     
  

                trackpoints.Add(oTP);
            }

            return trackpoints;

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="strRelPath"></param>
        /// <param name="strRootPath"></param>
        /// <param name="strGPXNode"></param>
        /// <returns></returns>
        public static List<Trackpoint> LoadRouteFromGPXFile(string strRelPath, string strRootPath)
        {
            const string strWaypoint_node = "//x:wpt";
    
            const string strGarmin_Track_node = "//x:rtept";
            string strGPXNode = strGarmin_Track_node;    //---Try looking for rtept nodes first


            List<Trackpoint> routepoints = new List<Trackpoint>();

            string strCleanedUpRelPath;

            if (strRelPath.StartsWith("/Content/images/"))
            {
                strCleanedUpRelPath = strRelPath.Replace("/Content/images/", "");
            }
            else if (strRelPath.StartsWith("Content/images/"))
            {
                strCleanedUpRelPath = strRelPath.Replace("Content/images/", "");
            }
            else
            {
                strCleanedUpRelPath = strRelPath;
            }


            string strFullPath = strRootPath + strCleanedUpRelPath;
            strFullPath = strFullPath.Replace("/", @"\");

            //----Load the GPX file as an XML Document
            XmlDocument gpxDoc = new XmlDocument();

            try
            {
                gpxDoc.Load(strFullPath);
            }
            catch (Exception)
            {
                return routepoints;
            }

            XmlNamespaceManager nsmgr = new XmlNamespaceManager(gpxDoc.NameTable);
            nsmgr.AddNamespace("x", "http://www.topografix.com/GPX/1/1");
            XmlNodeList nl = gpxDoc.SelectNodes(strGPXNode, nsmgr);

            //---Pull out the trackpoints
            foreach (XmlElement xelement in nl)
            {
                Trackpoint oTP = new Trackpoint
                {
                    latitude = float.Parse(xelement.GetAttribute("lat")),
                    longtitude = float.Parse(xelement.GetAttribute("lon"))
                };

                if (xelement.HasChildNodes)
                {
                    for (int i = 0; i < xelement.ChildNodes.Count; i++)
                    {
                        if (xelement.ChildNodes[i].Name == "ele")
                        {
                            oTP.elevation = float.Parse(xelement.ChildNodes[i].InnerText);
                        }
                        else if (xelement.ChildNodes[i].Name == "time")
                        {
                            oTP.time = DateTime.Parse(xelement.ChildNodes[i].InnerText);
                        }
                    }
                }

                routepoints.Add(oTP);
            }


            // If no route points were found, see if any waypoint are found in the file
            if (routepoints.Count == 0) {
                nl = gpxDoc.SelectNodes(strWaypoint_node, nsmgr);

                //---Pull out the trackpoints
                foreach (XmlElement xelement in nl)
                {
                    Trackpoint oTP = new Trackpoint
                    {
                        latitude = float.Parse(xelement.GetAttribute("lat")),
                        longtitude = float.Parse(xelement.GetAttribute("lon"))
                    };

                    if (xelement.HasChildNodes)
                    {
                        for (int i = 0; i < xelement.ChildNodes.Count; i++)
                        {
                            if (xelement.ChildNodes[i].Name == "ele")
                            {
                                oTP.elevation = float.Parse(xelement.ChildNodes[i].InnerText);
                            }
                            else if (xelement.ChildNodes[i].Name == "time")
                            {
                                oTP.time = DateTime.Parse(xelement.ChildNodes[i].InnerText);
                            }
                        }
                    }

                    routepoints.Add(oTP);
                }

            }

            return routepoints;

        }


        /// <summary>
        /// Given map bounds defined by the SW and NE lat/long coordinates of a rectangle map area
        /// filter out those markers which are not within this area
        /// </summary>
        /// <param name="markers"></param>
        /// <param name="neLat"></param>
        /// <param name="neLng"></param>
        /// <param name="swLat"></param>
        /// <param name="swLng"></param>
        /// <returns></returns>
        public static List<MapMarker> SelectMarkersInMapBounds(IEnumerable<Marker> markers, float neLat, float neLng, float swLat, float swLng, string strVirtualRoot)
        {
            List<MapMarker> selectedMarkers = new List<MapMarker>();

            EastingNorthing markerEastingNorthing = null;

            // using https://github.com/IeuanWalker/GeoUK convert the map bounds into to 27000 easting/northing coordinates
            LatitudeLongitude swLatLng = new LatitudeLongitude(swLat, swLng);
            LatitudeLongitude neLatLng = new LatitudeLongitude(neLat, neLng);

            Cartesian cartesian = Convert.ToCartesian(new Wgs84(), swLatLng);
            Cartesian bngCartesian = Transform.Etrs89ToOsgb36(cartesian);
            EastingNorthing swBoundsPoint = Convert.ToEastingNorthing(new Airy1830(), new BritishNationalGrid(), bngCartesian);

            cartesian = Convert.ToCartesian(new Wgs84(), neLatLng);
            bngCartesian = Transform.Etrs89ToOsgb36(cartesian);
            EastingNorthing neBoundsPoint = Convert.ToEastingNorthing(new Airy1830(), new BritishNationalGrid(), bngCartesian);

            foreach (Marker marker in markers)
            {
                //---First try the entered grid reference, then the hill's own gridref10, and finally the hills gridref.
                if (marker.GPS_Reference.Trim().Replace(" ","").Length == 12)
                {
                    markerEastingNorthing = ConvertOSGridRefToEastingNorthing(marker.GPS_Reference);

                }else if (marker.GPS_Reference.Trim().Replace(" ","").Length == 8)
                {
                    string strRef10 = GridrefToGridRef10(marker.GPS_Reference.Trim().Replace(" ", ""));
                    markerEastingNorthing = ConvertOSGridRefToEastingNorthing(strRef10);
                
                }else if(marker.Hill.Xcoord!=null && marker.Hill.Ycoord!=null)
                {
                    double easting = (double)marker.Hill.Xcoord;
                    double northing = (double)marker.Hill.Ycoord;

                    //-- add 2 metres the the easting as the marker is usually 5 paces from the centre of the summit cairn.
                    easting += 2;
                    markerEastingNorthing = new EastingNorthing(easting, northing);
                }else
                {
                    // create a dummy location which will never be in map bounds.
                    markerEastingNorthing = new EastingNorthing(-100000, -100000);
                }

                //---Is the marker within the map display bounds?
                if ( markerEastingNorthing !=null &&
                     markerEastingNorthing.Easting > swBoundsPoint.Easting && 
                     markerEastingNorthing.Easting < neBoundsPoint.Easting &&
                     markerEastingNorthing.Northing > swBoundsPoint.Northing &&
                     markerEastingNorthing.Northing < neBoundsPoint.Northing )
                {
                    LatitudeLongitude latlong = Convert27000EastingNorthingToLatLng(markerEastingNorthing);
                    MapMarker mmToAdd = new MapMarker
                    {
                        latitude = latlong.Latitude,
                        longtitude = latlong.Longitude,
                        popupText = MarkerPopup(marker, strVirtualRoot)
                    };
                    selectedMarkers.Add(mmToAdd);
                }

            }

            return selectedMarkers;

        }

        /// <summary>
        /// Given map bounds defined by the SW and NE lat/long coordinates of a rectangle map area
        /// filter out those markers which are not within this area
        /// </summary>
        /// <param name="markers"></param>
        /// <param name="neLat"></param>
        /// <param name="neLng"></param>
        /// <param name="swLat"></param>
        /// <param name="swLng"></param>
        /// <returns></returns>
        public static List<MapMarker> SelectHillsInMapBounds(IEnumerable<Hill> hills, float neLat, float neLng, float swLat, float swLng, string strVirtualRoot)
        {
            List<MapMarker> selectedHills = new List<MapMarker>();

            foreach (Hill hill in hills)
            {
                EastingNorthing hillEastingNorthing = new EastingNorthing((double)hill.Xcoord, (double)hill.Ycoord);

                //---Is the marker within the map display bounds?

                LatitudeLongitude latlong = Convert27000EastingNorthingToLatLng(hillEastingNorthing);
                MapMarker mmToAdd = new MapMarker
                {
                    latitude = latlong.Latitude,
                    longtitude = latlong.Longitude,
                    numberOfAscents = hill.NumberOfAscents,
                    popupText = HillPopup(hill)
                };
                selectedHills.Add(mmToAdd);
            }

            return selectedHills;

        }


        /// <summary>
        /// Convert OS 27000 easting/northing coordinate to WGS84 coordinate required by leaflet.
        /// </summary>
        /// <param name="en"></param>
        /// <returns></returns>
        public static LatitudeLongitude Convert27000EastingNorthingToLatLng(EastingNorthing en)
        {
            // convert to cartesian
            Cartesian cartesian = Convert.ToCartesian(new Airy1830(), new BritishNationalGrid(), en);

            // transform from OSBB36 datum to ETRS89 datum
            Cartesian wgsCartesian = Transform.Osgb36ToEtrs89(cartesian); //ETRS89 is effectively WGS84 which is required by leaftlet.

            // convert back to latitude/longitude
            LatitudeLongitude ll = Convert.ToLatitudeLongitude(new Wgs84(), wgsCartesian);

            return ll;
        }


        /// <summary>
        /// Convert an OS national grid 10 digit reference to an easting and northing.
        /// Following https://digimap.edina.ac.uk/help/our-maps-and-data/bng/
        /// </summary>
        /// <param name="strOSGridRef"></param>
        /// <returns></returns>
        public static EastingNorthing ConvertOSGridRefToEastingNorthing(string strOSGridRef)
        {
            EastingNorthing en = null;
            string strGridLetters="";
            double eastingWithinGridSquare=0;
            double northingWithinGridSquare=0;

            Dictionary<string, EastingNorthing> mappingTable = new Dictionary<string, EastingNorthing>(){
                { "SV", new EastingNorthing(0,0) },
                { "SW", new EastingNorthing(100000,0) },
                { "SX", new EastingNorthing(200000,0) },
                { "SY", new EastingNorthing(300000,0) },
                { "SZ", new EastingNorthing(400000,0) },
                { "TV", new EastingNorthing(500000,0) },

                { "SR", new EastingNorthing(100000,100000) },
                { "SS", new EastingNorthing(200000,100000) },
                { "ST", new EastingNorthing(300000,100000) },
                { "SU", new EastingNorthing(400000,100000) },
                { "TQ", new EastingNorthing(500000,100000) },
                { "TR", new EastingNorthing(600000,100000) },

                { "SM", new EastingNorthing(100000,200000) },
                { "SN", new EastingNorthing(200000,200000) },
                { "SO", new EastingNorthing(300000,200000) },
                { "SP", new EastingNorthing(400000,200000) },
                { "TL", new EastingNorthing(500000,200000) },
                { "TM", new EastingNorthing(600000,200000) },


                { "SH", new EastingNorthing(200000,300000) },
                { "SJ", new EastingNorthing(300000,300000) },
                { "SK", new EastingNorthing(400000,300000) },
                { "TF", new EastingNorthing(500000,300000) },
                { "TG", new EastingNorthing(600000,300000) },

                { "SC", new EastingNorthing(200000,400000) },
                { "SD", new EastingNorthing(300000,400000) },
                { "SE", new EastingNorthing(400000,400000) },
                { "TA", new EastingNorthing(500000,400000) },

                { "NW", new EastingNorthing(100000,500000) },
                { "NX", new EastingNorthing(200000,500000) },
                { "NY", new EastingNorthing(300000,500000) },
                { "NZ", new EastingNorthing(400000,500000) },

                { "NR", new EastingNorthing(100000,600000) },
                { "NS", new EastingNorthing(200000,600000) },
                { "NT", new EastingNorthing(300000,600000) },
                { "NU", new EastingNorthing(400000,600000) },

                { "NL", new EastingNorthing(0,700000) },
                { "NM", new EastingNorthing(100000,700000) },
                { "NN", new EastingNorthing(200000,700000) },
                { "NO", new EastingNorthing(300000,700000) },

                { "NF", new EastingNorthing(0,800000) },
                { "NG", new EastingNorthing(100000,800000) },
                { "NH", new EastingNorthing(200000,800000) },
                { "NJ", new EastingNorthing(300000,800000) },
                { "NK", new EastingNorthing(400000,800000) },

                { "NA", new EastingNorthing(0,900000) },
                { "NB", new EastingNorthing(100000,900000) },
                { "NC", new EastingNorthing(200000,900000) },
                { "ND", new EastingNorthing(300000,900000) },

                { "HW", new EastingNorthing(100000,1000000) },
                { "HX", new EastingNorthing(200000,1000000) },
                { "HY", new EastingNorthing(300000,1000000) },
                { "HZ", new EastingNorthing(400000,1000000) },

                { "HT", new EastingNorthing(300000,1100000) },
                { "HU", new EastingNorthing(400000,1100000) },

                { "HP", new EastingNorthing(400000,1200000) }
            };

            //---strip out any spaces in the grid ref
            string strGridRef = strOSGridRef.Trim().Replace(" ", "");

            if (strGridRef.Trim().Length != 12)
            {
                return en;
            }

            // split out the three sections of the grid ref
            try
            {
                strGridLetters = strGridRef.Substring(0, 2);
                eastingWithinGridSquare = Double.Parse(strGridRef.Substring(2, 5));
                northingWithinGridSquare = Double.Parse(strGridRef.Substring(7, 5));
            }catch (Exception)
            {
                return en;
            }

            // pull out the base easting northing from the mapping table
            EastingNorthing enGridSquareBase = mappingTable[strGridLetters];

            // add the easting and northing within the grid sqaure to the base.
            en = new EastingNorthing(enGridSquareBase.Easting + eastingWithinGridSquare, enGridSquareBase.Northing + northingWithinGridSquare);
  
            return en;
        }

    }
}