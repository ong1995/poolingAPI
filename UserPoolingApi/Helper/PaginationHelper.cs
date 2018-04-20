using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

public static class PaginationHelper
{
    public static PagedResults<T> ToPagedList<T>(this IQueryable<T> source, 
        PagedParamModel param, Expression<Func<T, bool>> filter = null, 
        Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null) where T : class
    {
        IQueryable<T> query = source;

        int TotalItems = query.Count();
        if (filter != null)
        {
            query = query.Where(filter);
        }

        int recordsFiltered = query.Count();

        if (orderBy != null)
        {
            query = orderBy(query);
        }

        query = query.Skip(param.PageSize*(param.PageNumber-1)).Take(param.PageSize);

        var result = new PagedResults<T>()
        {
            PageNumber = param.PageNumber,
            PageSize = param.PageSize,
            TotalNumberOfPages = (int)Math.Ceiling(recordsFiltered / (double)param.PageSize),
            TotalNumberOfRecords = TotalItems,
            TotalRcordsFiltered = recordsFiltered,
            Users = query.ToList()
        };


        return result;
    }
}

#region PaginationModel
public class PagedParamModel
{
    // properties are not capital due to json mapping
    //public int draw { get; set; }
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    //public List<Column> columns { get; set; }
    public string searchValue { get; set; }
    //public List<Order> order { get; set; }
}
#endregion

public class PagedResults<T>
{
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public int TotalNumberOfPages { get; set; }
    public int TotalNumberOfRecords { get; set; }
    public int TotalRcordsFiltered { get; set; }
    public IEnumerable<T> Users { get; set; }
}