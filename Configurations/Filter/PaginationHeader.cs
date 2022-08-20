using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AlumniWebsite.API.Configurations.Filter
{
    public class PaginationHeader
    {
        public PaginationHeader(int currentPage, int itemsPerPage, int totalItems, int totalPages)
        {
            CurrentPage = currentPage;
            ItemsPerPage = itemsPerPage;
            TotalItems = totalItems;
            TotalPage = totalPages;

        }

        public int CurrentPage { get; }
        public int ItemsPerPage { get; }
        public int TotalItems { get; }
        public int TotalPage { get; }
    }
}
