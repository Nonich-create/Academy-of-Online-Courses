using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Students.MVC.Models
{
    public class PaginationModel<T>
    {
        public IEnumerable<T> Data { get; set; }
        public int PageNumber { get; private set; }
        public int TotalPages { get; private set; }
        public const double pageSize = 10;
        public string searchString { get; set; }
        public int serachParameter { get; set; }
        public PaginationModel(int count, int pageNumber)
        {
            PageNumber = pageNumber;
            TotalPages = (int)Math.Ceiling(count / pageSize);
        }
    }
}
