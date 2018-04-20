using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UserPoolingApi.ViewModels
{
    public class PageResultViewModel
    {
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalNumberOfPages { get; set; }
        public int TotalNumberOfRecords { get; set; }
        public int TotalRcordsFiltered { get; set; }
        public IEnumerable<DisplayUserViewModel> Users { get; set; }
    }
}
