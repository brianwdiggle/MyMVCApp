﻿namespace MyMVCApp.DAL
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.IO;
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


        public static string HillAscentMarkerPopup(HillAscent oHA)
        {
            string strPopupText = "";
            string strNumberOfAscents = "";

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

            strPopupText = oHA.Hill.Hillname + " (" + oHA.Hill.Metres + "m)" + strNumberOfAscents;

            return strPopupText;
        }

        public static string MarkerObservationPopup(Marker_Observation oMO)
        {
            string strPopupText = "";

            strPopupText = "<a href=\"/Marker/Details/"  + oMO.MarkerID.ToString() + "\">" + oMO.Marker.MarkerTitle + "</a>" + " " + oMO.ObservationText.ToLower();

            return strPopupText;
        }

        public static string MarkerPopup(Marker oMarker, string strVirtualRoot)
        {
            string strPopupText = "";
            
            strPopupText = "<a href=\"/Marker/Details/" + oMarker.MarkerID.ToString() + "\">" + oMarker.MarkerTitle + "</a>" + "<br/>" + oMarker.Location_Description;

            return strPopupText;
        }

        public static string HillPopup(Hill oHill, string strVirtualRoot)
        {
            string strPopupText = "";

            string strAscents = "";
            if (oHill.NumberOfAscents==0)
            {
                strAscents = "Unclimbed";
            }else if (oHill.NumberOfAscents == 1)
            {
                strAscents = "1 ascent";
            }else
            {
                strAscents = oHill.NumberOfAscents.ToString() + " ascents";
            }
            strPopupText = "<a href=\"/Walks/HillDetails/" + oHill.Hillnumber.ToString() + "\">" + oHill.Hillname + "</a>" + "<br/>" + strAscents;

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

                strGridref10 = oHill.Gridref10.Substring(0, 3) + iEastingOfMarker.ToString() + oHill.Gridref10.Substring(8, 6);
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
            return strGridRef.Substring(0, 2) + " " + strGridRef.Substring(2, 3) + "00 " + strGridRef.Substring(5, 3) + "00";
        }

        public static List<Trackpoint> LoadDataFromGPXFile(string strRelPath, string strRootPath, string strGPXNode)
        {
            List<Trackpoint> trackpoints = new List<Trackpoint>();

            string strCleanedUpRelPath;

            if (strRelPath.StartsWith("/Content/images/"))
            {
                strCleanedUpRelPath = strRelPath.Replace("/Content/images/", "");
               // strCleanedUpRelPath = res2.Replace("/", @"\");
            }
            else if (strRelPath.StartsWith("Content/images/"))
            {
                strCleanedUpRelPath = strRelPath.Replace("Content/images/", "");
               // strCleanedUpRelPath = res2.Replace("/", @"\");
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
            }catch(Exception e)
            {
                return trackpoints;
            }
    
            XmlNamespaceManager nsmgr = new XmlNamespaceManager(gpxDoc.NameTable);
            nsmgr.AddNamespace("x", "http://www.topografix.com/GPX/1/1");
            XmlNodeList nl = gpxDoc.SelectNodes(strGPXNode, nsmgr);

            //---Pull out the trackpoints
            foreach (XmlElement xelement in nl)
            {
                Trackpoint oTP = new Trackpoint();
                
                oTP.latitude = float.Parse(xelement.GetAttribute("lat"));
                oTP.longtitude = float.Parse(xelement.GetAttribute("lon"));

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


        public static List<Marker> SelectMarkersInMapBounds(IEnumerable<Marker> markers, float neLat, float neLng, float swLat, float swLng)
        {
            List<Marker> selectedMarkers = new List<Marker>();

            EastingNorthing markerEastingNorthing = null;

            // using https://github.com/IeuanWalker/GeoUK
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
                    selectedMarkers.Add(marker);
                }

            }

            return selectedMarkers;

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
                eastingWithinGridSquare = Double.Parse(strGridRef.Substring(3, 5));
                northingWithinGridSquare = Double.Parse(strGridRef.Substring(6, 5));
            }catch (Exception e)
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