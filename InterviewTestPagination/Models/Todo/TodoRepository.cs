using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace InterviewTestPagination.Models.Todo {

    /// <summary>
    /// No need to use an actual persistent datasource.
    /// All operations can be mocked in-memory as long as they are consistent with the chosen datasource implementation
    /// (e.g. dont create new model instances when executing a search 'query', etc).
    /// TL;DR: from this point on Database-like operations can be mocked.
    /// </summary>
    public class TodoRepository : IModelRepository<Todo> {

        /// <summary>
        /// Example in-memory model datasource 'indexed' by id.
        /// </summary>
        private static readonly IDictionary<long, Todo> DataSource = new ConcurrentDictionary<long, Todo>();

        /// <summary>
        /// Properties the list can be sorted by (whitelist: anything else falls back to the default sort).
        /// </summary>
        private static readonly IDictionary<string, Func<Todo, object>> SortKeys =
            new Dictionary<string, Func<Todo, object>>(StringComparer.OrdinalIgnoreCase) {
                ["id"] = t => t.Id,
                ["task"] = t => t.Task,
                ["createdDate"] = t => t.CreatedDate
            };

        static TodoRepository() {
            // initializing datasource
            var startDate = DateTime.Today;
            for (var i = 1; i <= 55; i++) {
                var createdDate = startDate.AddDays(i);
                DataSource[i] = new Todo(id: i, task: "Dont forget to do " + i, createdDate: createdDate);
            }
        }

        public IEnumerable<Todo> All() {
            return DataSource.Values.OrderByDescending(t => t.CreatedDate);
        }

        public int Count() {
            return DataSource.Count;
        }

        public IEnumerable<Todo> Query(int skip, int take, string sortBy, bool sortDesc) {
            Func<Todo, object> key;
            if (sortBy == null || !SortKeys.TryGetValue(sortBy, out key)) {
                // default sort: newest first
                key = t => t.CreatedDate;
                sortDesc = true;
            }

            var sorted = sortDesc ? DataSource.Values.OrderByDescending(key) : DataSource.Values.OrderBy(key);
            var query = sorted.Skip(skip);
            return (take > 0 ? query.Take(take) : query).ToList();
        }
    }
}
