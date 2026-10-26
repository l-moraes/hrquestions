namespace InterviewTestPagination.Models {
    /// <summary>
    /// Pagination and sorting parameters sent by the client (client-to-server contract).
    /// </summary>
    public class PageRequest {
        /// <summary>1-based page number</summary>
        public int Page { get; set; } = 1;

        /// <summary>Items per page; 0 means 'all items'</summary>
        public int PageSize { get; set; } = 20;

        /// <summary>Property to sort by; null means the default sort</summary>
        public string SortBy { get; set; }

        public bool SortDesc { get; set; }
    }
}
