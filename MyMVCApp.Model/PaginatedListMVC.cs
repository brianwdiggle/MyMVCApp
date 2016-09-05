using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MyMVCAppCS.Models
{
    public class PaginatedListMVC<T> : PaginatedList<T>
    {

        //----Will hold the html for the page navigation links------

        private string _PageNavigationLinks;

        private string _RecordsShowing;
        //----Holds the maximum number of pagination links to display----

        private int _MaxPaginationLinks;
        //----Public Property which will hold the html for the page navigation links-----
        public string PageNavigationLinks
        {
            get { return _PageNavigationLinks; }
        }

        public int MaxPaginationLinks
        {
            get { return _MaxPaginationLinks; }
            set { _MaxPaginationLinks = value; }
        }

        public string RecordsShowing
        {
            get { return _RecordsShowing; }
            set { _RecordsShowing = value; }
        }


        //------------------------------------------------------------------------
        //
        // New - Constructor
        //
        //--------------------------


        //---Call the base class constructor to initialise a few values supplied as parameters-----------
        public PaginatedListMVC(IQueryable<T> source, int iPageIndex, int iPageSize, string urlbase, int iMaxPageLinks, string strOrderBy)
            : base(source, iPageIndex, iPageSize)
        {

            _MaxPaginationLinks = iMaxPageLinks;
            _PageNavigationLinks = GeneratePaginationLinks(urlbase, strOrderBy);
            _RecordsShowing = GenerateRecordsShowing(iPageIndex, iPageSize);

        }


        //--------------------------------------------------------------------------------------------------------
        // Function:     GenerateRecordsShowing
        // Decription:   Returns string with "showing x-y of z"
        //---------------------------------------------------------------

        private string GenerateRecordsShowing(int iPageIndex, int iPageSize)
        {

            string strRecsShowing = "";

            int iFrom = 0;
            int iTo = 0;

            iFrom = ((iPageIndex - 1) * PageSize) + 1;
            iTo = iFrom + PageSize - 1;

            if (iTo > TotalCount)
            {
                iTo = TotalCount;
            }

            try
            {
                strRecsShowing = strRecsShowing + Convert.ToString(iFrom) + " - " + Convert.ToString(iTo) + " of " + Convert.ToString(TotalCount);
            }
            catch (Exception)
            {
                strRecsShowing = strRecsShowing + " (Error converting record range)";
            }

            return strRecsShowing;

        }


        //------------------------------------------------------------------------------------------
        // Function:     GeneratePaginationLinks
        // Description:  Generate html for pagination links
        //---------------------------------------------

        private string GeneratePaginationLinks(string myUrlBase, string strOrderBy)
        {

            string strNavlinks = "";
            int iStartLinkPage = 0;
            int iEndLinkPage = 0;

            string strPageLink = null;

            if (myUrlBase.Contains("?"))
            {
                strPageLink = "&page=";
            }
            else
            {
                strPageLink = "/";
            }


            if (PageIndex >= MaxPaginationLinks)
            {
                iStartLinkPage = (PageIndex - MaxPaginationLinks) + 1;
                if (iStartLinkPage < 1)
                {
                    iStartLinkPage = 1;
                }
            }
            else
            {
                iStartLinkPage = 1;
            }

            if ((iStartLinkPage + MaxPaginationLinks) > (TotalPages))
            {
                iEndLinkPage = TotalPages;
            }
            else
            {
                iEndLinkPage = iStartLinkPage + (MaxPaginationLinks - 1);
            }

            //----If the calculated start page for the numbered links is greater than 1, display a "first" link----------------

            if (iStartLinkPage > 1)
            {
                strNavlinks = strNavlinks + "<a href=\"" + myUrlBase + strPageLink + "1" + strOrderBy + "\">First</a> ";
            }

            if (HasPreviousPage)
            {
                strNavlinks = strNavlinks + "<a href=\"" + myUrlBase + strPageLink + (PageIndex - 1).ToString() + strOrderBy + "\">Previous</a> ";
            }

            for (int iPageCount = iStartLinkPage; iPageCount <= iEndLinkPage; iPageCount++)
            {
                if (iPageCount == PageIndex)
                {
                    strNavlinks = strNavlinks + "<b>" + iPageCount.ToString() + "</b> ";
                }
                else
                {
                    strNavlinks = strNavlinks + "<a href=\"" + myUrlBase + strPageLink + iPageCount.ToString() + strOrderBy + "\">" + iPageCount.ToString() + "</a> ";
                }
            }

            if (HasNextPage)
            {
                strNavlinks = strNavlinks + "<a href=\"" + myUrlBase + strPageLink + (PageIndex + 1).ToString() + strOrderBy + "\">Next</a> ";
            }

            //----If the calculated end page for the numbered links is less than the total number of pages, then display a "Last" link----
            if (iEndLinkPage < TotalPages)
            {
                strNavlinks = strNavlinks + " <a href=\"" + myUrlBase + strPageLink + TotalPages.ToString() + strOrderBy + "\">Last</a>";
            }

            return strNavlinks;

        }

    }


}