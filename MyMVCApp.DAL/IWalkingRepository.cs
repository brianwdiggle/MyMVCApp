// -----------------------------------------------------------------------
// <copyright file="IWalkingRepository.cs" company="Callcredit Information Group">
//   
// </copyright>
// -----------------------------------------------------------------------

namespace MyMVCApp.DAL
{

    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Interface defining Walking Repository
    /// </summary>
 
    public interface IWalkingRepository
    {

        //-----walk related methods-----------

        int AddWalk(Walk walk);
        int AddWalkSummitsVisited(List<HillAscent> summits);
        int AddWalkAssociatedFiles(List<Walk_AssociatedFile> assocfiles);
        List<Walk_AssociatedFile> GetWalkAssociatedFiles(int walkid);
        IQueryable<Walk_AssociatedFile> GetWalkAuxilliaryFiles(int walkid);

        IQueryable<Walk> FindAllWalks();
        IQueryable<Walk> FindWalksByArea(string strAreaName);

        Walk GetWalkDetails(int id);

        void UpdateWalkDetails(Walk walk, System.Collections.Specialized.NameValueCollection oForm, string strRootPath);

        void DeleteWalk(Walk walk);
        IQueryable<Walk> SearchForWalks(string searchTerms);

        //-----Hill related methods-------------

        IQueryable<Hill> FindAllHills();
        IQueryable<Hill> FindHillsByArea(string strAreaRef);
        Hill GetHillDetails(int id);
        IQueryable<Hill> FindHillsAboveFeet(int iFeet);
        IQueryable<Hill> GetHillsByClassification(string strHillClass);
        IQueryable<Hill> FindHillsByNameLike(string strHillNamePortion);
        IQueryable<Hill> FindHillsInAreaByNameLike(string strHillNamePortion, string strAreaRef);

        void UpdateHillDetails(Walk walk);

        void DeleteHill(Walk walk);
        IQueryable<HillAscent> GetHillAscents(int iHillId);
        int GetNumberOfHillAscentsByHillID(int iHillID);
        int DeleteHillAscentsForWalk(int iWalkID);
        int DeleteAssociateFilesForWalk(int iWalkID);
        IQueryable<HillAscent> GetAllHillAscents();

        //------Hill Classifications------------------------
        IQueryable<Class> GetAllHillClassifications();
        string GetHillClassificationName(string strClassificationCode);

        //-----Marker related methods------------------

        IQueryable<Marker> FindAllMarkers();
        Marker GetMarkerDetails(int id);
        IQueryable<Marker> FindMarkersByNameLike(string strNamePortion);

        int UpdateMarkerDetails(Marker marker, System.Collections.Specialized.NameValueCollection oForm);
        void DeleteMarker(Marker marker);
        int CreateMarker(Marker marker);
        int AssociateMarkersWithWalk(System.Collections.Specialized.NameValueCollection oForm, int iWalkID);
        List<Marker> GetMarkersCreatedOnWalk(int iWalkID);
        bool FindMarkerObservationLike(Marker_Observation oMarkerObs);
        IQueryable<Marker_Status> GetAllMarkerStatusOptions();

        //------Walking Areas-----------------------------------
        IQueryable<Area> GetAllWalkingAreas();
        IQueryable<Area> GetAllWalkingAreas(string strCountryCode);
        IQueryable<Area> GetAllWalkingAreas(string strCountryCode, string strAreaType);
        IQueryable<Area> FindWalkAreasByNameLike(string strNamePortion);
        IQueryable<Walk> FindWalksByTitleLike(string strTitlePortion);
        string GetWalkAreaNameFromAreaRef(string strAreaRef);
        string GetWalkAreaTypeNameFromType(string strAreaType);

        //------Walk Types------------------------------------
        IQueryable<WalkType> GetWalkTypes();

        //------Associated files------------------------------
        IQueryable<Walk_AssociatedFile> GetAllImages();
            
        //------Associated File types--------------------------
        IQueryable<Walk_AssociatedFile_Type> GetAssociatedFileTypes();

        //----Progress---------------------------------
        List<MyProgress> GetMyProgress();
        List<MyProgress> GetMyProgressByClassType(char cClassType);


    }


}

