using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MyMVCAppCS.Models
{
    public class PaginatedList<T> : List<T>
    {

        //----Private variables for use with the public properties---------
        private int _PageIndex;
        private int _PageSize;
        private int _TotalCount;

        private int _TotalPages;
        //-----Page index of the source list---------
        public int PageIndex
        {
            get { return _PageIndex; }
        }

        public int PageSize
        {
            get { return _PageSize; }
        }

        public int TotalCount
        {
            get { return _TotalCount; }
        }

        public int TotalPages
        {
            get { return _TotalPages; }
        }

        //----Constructor-------------------------------


        public PaginatedList()
        {

        }



        public PaginatedList(IQueryable<T> source, int iPageIndex, int iPageSize)
        {
            //-----Set the private variables which as exposed publically as properties-----------
            _PageIndex = iPageIndex;
            _PageSize = iPageSize;
            _TotalCount = source.Count();

            decimal sum = (decimal)_TotalCount / (decimal)iPageSize;
            _TotalPages = (int) Math.Ceiling(sum);

            if (_PageIndex <= 0)
            {
                _PageIndex = 1;
            }

            //----Take the specified page of results from the source list and add it to me------------
            this.AddRange(source.Skip((_PageIndex - 1) * _PageSize).Take(_PageSize));

        }

        public bool HasPreviousPage
        {
            get { return (_PageIndex > 1); }
        }

        public bool HasNextPage
        {
            get { return (_PageIndex < _TotalPages); }
        }

    }


}