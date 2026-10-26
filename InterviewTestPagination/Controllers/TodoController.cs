using System.Web.Http;
using InterviewTestPagination.Models;
using InterviewTestPagination.Models.Todo;

namespace InterviewTestPagination.Controllers {
    /// <summary>
    /// 'Rest' controller for the <see cref="Todo"/>
    /// model.
    /// </summary>
    [RoutePrefix("api/Todo")]
    public class TodoController : ApiController {

        // TODO: [low priority] setup DI
        private readonly IModelService<Todo> _todoService = new TodoService();

        /// <summary>
        /// Lists one page of todos.
        /// e.g.: GET api/Todo/Todos?page=2&amp;pageSize=20&amp;sortBy=id&amp;sortDesc=false
        /// </summary>
        [HttpGet, Route("Todos")]
        public PagedResult<Todo> Todos([FromUri] PageRequest request) {
            return _todoService.List(request);
        }

    }
}
