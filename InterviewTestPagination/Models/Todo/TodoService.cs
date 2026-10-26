using System;

namespace InterviewTestPagination.Models.Todo {
    /// <summary>
    /// Service layer for the <see cref="Todo"/> model: applies the pagination rules
    /// and translates page requests into datasource queries.
    /// </summary>
    public class TodoService : IModelService<Todo> {

        private IModelRepository<Todo> _repository = new TodoRepository();

        public IModelRepository<Todo> Repository {
            get { return _repository; }
            set { _repository = value; }
        }

        /// <summary>
        /// Lists one page of <see cref="Todo"/> entries.
        /// Out of range pages are clamped to the first/last page and PageSize 0 lists all entries.
        /// </summary>
        /// <returns></returns>
        public PagedResult<Todo> List(PageRequest request) {
            request = request ?? new PageRequest();

            var total = Repository.Count();
            var showAll = request.PageSize <= 0;
            var pageSize = showAll ? total : request.PageSize;
            var totalPages = pageSize > 0 ? Math.Max(1, (int)Math.Ceiling(total / (double)pageSize)) : 1;
            var page = Math.Min(Math.Max(1, request.Page), totalPages);

            // invoke Datasource layer
            return new PagedResult<Todo> {
                Items = Repository.Query((page - 1) * pageSize, pageSize, request.SortBy, request.SortDesc),
                Page = page,
                PageSize = showAll ? 0 : pageSize,
                TotalItems = total,
                TotalPages = totalPages
            };
        }
    }
}
