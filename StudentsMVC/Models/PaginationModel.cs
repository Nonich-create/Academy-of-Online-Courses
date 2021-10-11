using System;
using System.Collections.Generic;

namespace Students.MVC.Models
{
    #region PaginationModel
    public class PaginationModel<T>
    {
        public IEnumerable<T> Data { get; set; }
        public int PageNumber { get; private set; }
        public int TotalPages { get; private set; }
        public const double pageSize = 10;
        public string searchString { get; set; }
        public int searchParameter { get; set; }
        public PaginationModel(int count, int pageNumber)
        {
            PageNumber = pageNumber;
            TotalPages = (int)Math.Ceiling(count / pageSize);
        }
    }
    #endregion
}
