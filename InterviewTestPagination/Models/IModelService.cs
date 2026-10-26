namespace InterviewTestPagination.Models {
    /// <summary>
    /// Model Service layer's main entry-point.
    /// Should translate high-level commands and data structures into something that can be used by the data source layer.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IModelService<T> {

        /// <summary>
        /// Reference to Repository layer
        /// </summary>
        IModelRepository<T> Repository { get; set; }

        /// <summary>
        /// Lists one page of entries of model T according to the request
        /// (out of range pages are clamped, PageSize 0 lists all entries)
        /// </summary>
        /// <returns></returns>
        PagedResult<T> List(PageRequest request);
    }
}
