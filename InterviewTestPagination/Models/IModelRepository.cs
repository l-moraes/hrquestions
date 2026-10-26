using System.Collections.Generic;

namespace InterviewTestPagination.Models {

    /// <summary>
    /// Datasource/Driver layer's main entry-point.
    /// Only knows datasource-like operations (count, sort, skip/take), not the concept of 'page'.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IModelRepository<T> {
        /// <summary>
        /// Lists all entries of type T
        /// </summary>
        /// <returns></returns>
        IEnumerable<T> All();

        /// <summary>
        /// Total number of entries of type T
        /// </summary>
        /// <returns></returns>
        int Count();

        /// <summary>
        /// Lists a sorted slice of the entries of type T.
        /// The whole datasource is sorted before the slice is taken.
        /// </summary>
        /// <param name="skip">number of entries to skip</param>
        /// <param name="take">number of entries to take; 0 or less means no limit</param>
        /// <param name="sortBy">property to sort by; unknown or null falls back to the default sort</param>
        /// <param name="sortDesc">true for descending order</param>
        /// <returns></returns>
        IEnumerable<T> Query(int skip, int take, string sortBy, bool sortDesc);
    }
}
