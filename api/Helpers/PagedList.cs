using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace api.Helpers
{
    public class PagedList<T> : List<T>
    {
        public PagedList(IEnumerable<T> items, int totalItemCount, int currentPageNumber, int pageSize)
        {
            CurrentPage = currentPageNumber;
            TotalPages = (int)Math.Ceiling(totalItemCount / (double)pageSize);
            PageSize = pageSize;
            TotalItemCount = totalItemCount;
            AddRange(items);
        }
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public int PageSize { get; set; }
        public int TotalItemCount { get; set; }
        public static async Task<PagedList<T>> CreateAsync(IQueryable<T> source, int currentPageNumber, int pageSize)
        {
            var totalItemCount = await source.CountAsync();
            var items = await source.Skip((currentPageNumber - 1) * pageSize).Take(pageSize).ToListAsync();
            return new PagedList<T>(items, totalItemCount, currentPageNumber, pageSize);
        }
    }
}