using System.Collections.Generic;

namespace InterviewTestPagination.Models {
    /// <summary>
    /// A single page of items plus the information needed to navigate the other pages
    /// (server-to-client contract).
    /// </summary>
    public class PagedResult<T> {
        public IEnumerable<T> Items { get; set; }

        /// <summary>Page actually returned (may differ from the requested one if it was out of range)</summary>
        public int Page { get; set; }

        /// <summary>Items per page; 0 means 'all items'</summary>
        public int PageSize { get; set; }

        public int TotalItems { get; set; }
        public int TotalPages { get; set; }
    }
}
