namespace MyMVCApp.DAL
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.IO;
    using System.Text;
    using System.Text.RegularExpressions;

    public class WalkingStick
    {

    
        public static string HillClassToLink(string strHillClass, string strLinkText, string strApplicationRoot) 
        {
            if ((strLinkText == "")) 
            {
                strLinkText = strHillClass;
                // Warning!!! Optional parameters not supported
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
                return "#8b8c90";
            }
            //  From CCCCFF to 666699 i.e. 
            int iMaxR = 240;
            int iMaxG = 240;
            int iMaxB = 255;
            int iMinR = 122;
            int iMinG = 122;
            int iMinB = 173;
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
                    strLine = (strLine + oHill.Gridref);
                }
            }
            else 
            {
                strLine = (strLine + oHill.Gridref10);
            }
            if (!(oHill.Classification == null)) 
            {
                strLine = (strLine + (", " + oHill.Classification.Replace(",", " ")));
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
                    iWalkTotalTime = (iWalkTotalTime + int.Parse(oForm["total_time_mins"]));
                }
            }
            catch (Exception) 
            {
            }

            if ((iWalkTotalTime > 0)) {
                oWalk.WalkTotalTime = iWalkTotalTime;
            }

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
                        iCounter = (iCounter + 1);
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
                    var oHillAscent = new HillAscent();
                    oHillAscent.AscentDate = dAscentDate;
                    try {
                        oHillAscent.Hillnumber = Int16.Parse(oForm[("VisitedSummit" + iCounter + "HillID")]);
                        oHillAscent.WalkID = iWalkID;
                        collHillAscents.Add(oHillAscent);
                    }
                    catch (Exception) 
                    {
                    }
                    iCounter = (iCounter + 1);
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
                    var oHillAssociateFile = new Walk_AssociatedFile();
                    oHillAssociateFile.WalkID = iWalkID;
                    oHillAssociateFile.Walk_AssociatedFile_Name = CleanUpAssociateFilePath(oForm[("imagerelpath" + iCounter)], "Content/images/");
                    oHillAssociateFile.Walk_AssociatedFile_Type = "Image";
                    oHillAssociateFile.Walk_AssociatedFile_Caption = oForm[("imagecaption" + iCounter)];
                    oHillAssociateFile.Walk_AssociatedFile_Sequence = (short)(iCounter);
                    if ((oForm[("imageismarker" + iCounter)] != null 
                                && (oForm[("imageismarker" + iCounter)].Length > 0))) {
                        oHillAssociateFile.Walk_AssociatedFile_MarkerID = Int32.Parse(oForm[("imagemarkerid" + iCounter)]);
                    }
                    collHillAssociatedFiles.Add(oHillAssociateFile);
                    iCounter = (iCounter + 1);
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
                    Walk_AssociatedFile oHillAssociateFile = new Walk_AssociatedFile();
                    oHillAssociateFile.WalkID = iWalkID;
                    oHillAssociateFile.Walk_AssociatedFile_Name = CleanUpAssociateFilePath(oForm[("auxilliary_file" + iCounter)], "Content/images/");
                    oHillAssociateFile.Walk_AssociatedFile_Type = oForm[("auxilliary_filetype" + iCounter)];
                    oHillAssociateFile.Walk_AssociatedFile_Sequence = (short)(iCounter + iNumImages);
                    oHillAssociateFile.Walk_AssociatedFile_Caption = oForm[("auxilliary_caption" + iCounter)];
                    collHillAssociatedFiles.Add(oHillAssociateFile);
                    iCounter = (iCounter + 1);
                }
                else {
                    bContinue = false;
                }
            }
            return collHillAssociatedFiles;
        }

        /// <summary>
        /// Given the walk details form, prepare the DAL objects for insertion
        /// TODO: In particular, given a directory in which the walk associated files have been prepared, examine the directory and add the files 
        /// found to the walk
        /// </summary>
        /// <param name="iWalkID"></param>
        /// <param name="oForm"></param>
        /// <param name="strRootpath"></param>
        /// <returns></returns>
        public static List<Walk_AssociatedFile> FillWalkAssociatedFilesByExaminingDirectory(int iWalkID, NameValueCollection oForm, string strRootPath)
        {
            var collWalkAssociatedFiles = new List<Walk_AssociatedFile>();
 
            short siFileSequenceCounter = 1;

            string walkfiles_fullpathprefix = oForm["walkfiles_reldir"];
            string walkfiles_nameprefix;

            if (walkfiles_fullpathprefix.Trim().Length==0)
            {
                return collWalkAssociatedFiles;
            }

            walkfiles_fullpathprefix = walkfiles_fullpathprefix.Replace("\\", "/");
            int iLoc;

            try
            {
                iLoc = walkfiles_fullpathprefix.LastIndexOf("/", StringComparison.Ordinal);
            }
            catch (Exception)
            {
                iLoc = 0;
            }

            string strRelPath = walkfiles_fullpathprefix.Substring(0, iLoc);
            walkfiles_nameprefix = walkfiles_fullpathprefix.Substring(iLoc + 1, walkfiles_fullpathprefix.Length - iLoc - 1);

            // -----Check that the path specified is valid------------------------
            if (!WalkingStick.DetermineIfDirectoryExists(strRootPath + strRelPath))
            {
                throw new Exception("Could not find directory when looking for image files: dir:[" + strRootPath + strRelPath + "]" + " strRootPath = [" + strRootPath + "]");
            }

            //-----First add all the walk image files----
            string[] filesindir= Directory.GetFiles(strRootPath + strRelPath, walkfiles_nameprefix + "_*");
            
            // go through looking for each number, assigning an order number to the unsorted list
            for (int imageNumber =1; imageNumber< filesindir.Length+1; imageNumber++)
            {
                // Look for image number imageNumber in the unordered list
                for (int iCount = 0; iCount < filesindir.Length; iCount++)
                {
                    // Construct full file name to look for - jpgs and pngs are allowed, either case
                    if (filesindir[iCount].Equals(strRootPath + strRelPath + "\\" + walkfiles_nameprefix + "_" + imageNumber.ToString() + ".jpg") ||
                        filesindir[iCount].Equals(strRootPath + strRelPath + "\\" + walkfiles_nameprefix + "_" + imageNumber.ToString() + ".JPG") ||
                        filesindir[iCount].Equals(strRootPath + strRelPath + "\\" + walkfiles_nameprefix + "_" + imageNumber.ToString() + ".png") ||
                        filesindir[iCount].Equals(strRootPath + strRelPath + "\\" + walkfiles_nameprefix + "_" + imageNumber.ToString() + ".PNG"))
                    {
                        // filesindir[iCount] = filesindir[iCount].Replace("\\","/");
                        int iLocFileExtStart = 0;
                        iLocFileExtStart = filesindir[iCount].LastIndexOf(".");
                        string strImageFileExt = filesindir[iCount].Substring(iLocFileExtStart, 4);

                        var oHillAssociateFile = new Walk_AssociatedFile();
                        oHillAssociateFile.WalkID = iWalkID;
                        oHillAssociateFile.Walk_AssociatedFile_Name = CleanUpAssociateFilePath(strRelPath + "/" + walkfiles_nameprefix + "_" + imageNumber.ToString() + strImageFileExt, "Content/images/");
                        oHillAssociateFile.Walk_AssociatedFile_Type = "Image";
                        oHillAssociateFile.Walk_AssociatedFile_Sequence = siFileSequenceCounter++;
      
                        collWalkAssociatedFiles.Add(oHillAssociateFile);
                        break;
                    }
                }
            }

            // Look for any auxilliary files (not walk images) present matching the nameprefix.

            filesindir = Directory.GetFiles(strRootPath + strRelPath, walkfiles_nameprefix + "-*");

   
            // for each aux file, create object and add to result list.
            for (int iCount = 0; iCount < filesindir.Length; iCount++)
            {
  
                filesindir[iCount] = filesindir[iCount].Replace("\\", "/");

                int iLocNamePrefix = filesindir[iCount].IndexOf(walkfiles_nameprefix);
                filesindir[iCount] = filesindir[iCount].Substring(iLocNamePrefix);

                string strDesc = "";

                var oHillAssociateFile = new Walk_AssociatedFile();
                oHillAssociateFile.WalkID = iWalkID;
                oHillAssociateFile.Walk_AssociatedFile_Name = CleanUpAssociateFilePath(strRelPath + "/" + filesindir[iCount], "Content/images/");
                oHillAssociateFile.Walk_AssociatedFile_Type = DetermineAuxFileType(filesindir[iCount], walkfiles_nameprefix, ref strDesc);
                oHillAssociateFile.Walk_AssociatedFile_Sequence = siFileSequenceCounter++;
                oHillAssociateFile.Walk_AssociatedFile_Caption = strDesc;

                collWalkAssociatedFiles.Add(oHillAssociateFile);
            }
            
            return collWalkAssociatedFiles;
        }

        public static string DetermineAuxFileType(string filename, string name_prefix, ref string strDescription)
        {
            string strAuxFileType = "";
            int iLoc = 0;
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
            return strAuxFileType;
        }

        public static string ExtractDescFromAuxFileName(string strAuxFilename, string strAuxFiletype)
        {
            int iLocStart = 0;
            int iLocEnd = 0;
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

            // Remove anything which is found before Content/images/  (the historic content of strPathImages
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
                    Walk_AssociatedFile oHillAssociateFile = new Walk_AssociatedFile();
                    oHillAssociateFile.WalkID = iWalkID;
                    oHillAssociateFile.Walk_AssociatedFile_Name = oForm[("existingimagename" + iExistingImagesCount)];
                    oHillAssociateFile.Walk_AssociatedFile_Type = "Image";
                    oHillAssociateFile.Walk_AssociatedFile_Caption = oForm[("existingimagecaption" + iExistingImagesCount)];
                    oHillAssociateFile.Walk_AssociatedFile_Sequence = (short)iSequenceCounter;
                    if ((oForm[("existingimageismarker" + iExistingImagesCount)] != null 
                                && (oForm[("existingimageismarker" + iExistingImagesCount)].Length > 0))) {
                        oHillAssociateFile.Walk_AssociatedFile_MarkerID = int.Parse(oForm[("existingimagemarkerid" + iExistingImagesCount)]);
                    }
                    iSequenceCounter = (iSequenceCounter + 1);
                    collHillAssociatedFiles.Add(oHillAssociateFile);
                }
            }

            iCounter = 1;
            bContinue = true;
            while (bContinue) {
                if ((oForm[("existingauxfilename" + iCounter)] != null 
                            && ((oForm[("existingauxfilename" + iCounter)].Length > 0) 
                            && (oForm[("delexisting_auxilliary_file" + iCounter)] != "on")))) {
                    var oHillAssociateFile = new Walk_AssociatedFile();
                    oHillAssociateFile.WalkID = iWalkID;
                    oHillAssociateFile.Walk_AssociatedFile_Name = oForm[("existingauxfilename" + iCounter)];
                    oHillAssociateFile.Walk_AssociatedFile_Type = oForm[("existingauxfiletype" + iCounter)];
                    oHillAssociateFile.Walk_AssociatedFile_Sequence = (short)(iCounter + iSequenceCounter);
                    collHillAssociatedFiles.Add(oHillAssociateFile);
                    iCounter = (iCounter + 1);
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
                    strRet = (strRet + (" " 
                                + (strHour + "s ")));
                }
                else {
                    strRet = (strRet + (" " 
                                + (strHour + " ")));
                }
            }
            if ((iNumMins > 0)) {
                strRet = (strRet + iNumMins.ToString());
                if ((iNumMins > 1)) {
                    strRet = (strRet + (" " 
                                + (strMinute + "s")));
                }
                else {
                    strRet = (strRet + (" " + strMinute));
                }
            }
            return strRet;
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
    }
}