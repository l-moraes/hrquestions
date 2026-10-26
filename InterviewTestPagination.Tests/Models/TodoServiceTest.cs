using System.Collections.Generic;
using System.Linq;
using InterviewTestPagination.Models;
using InterviewTestPagination.Models.Todo;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace InterviewTestPagination.Tests.Models {
    /// <summary>
    /// Unity testing for <see cref="InterviewTestPagination.Models.Todo.TodoService" />
    /// (uses the in-memory datasource with 55 items, unless stated otherwise)
    /// </summary>
    [TestClass]
    public class TodoServiceTest {

        private TodoService _service;

        [TestInitialize]
        public void Setup() {
            _service = new TodoService();
        }

        [TestMethod]
        public void List_DefaultRequest_ReturnsFirstPageWith20Items() {
            var result = _service.List(new PageRequest());

            Assert.AreEqual(1, result.Page);
            Assert.AreEqual(20, result.PageSize);
            Assert.AreEqual(20, result.Items.Count());
            Assert.AreEqual(55, result.TotalItems);
            Assert.AreEqual(3, result.TotalPages);
        }

        [TestMethod]
        public void List_NullRequest_UsesDefaults() {
            var result = _service.List(null);

            Assert.AreEqual(1, result.Page);
            Assert.AreEqual(20, result.Items.Count());
        }

        [TestMethod]
        public void List_LastPage_ReturnsRemainingItems() {
            var result = _service.List(new PageRequest { Page = 3 });

            Assert.AreEqual(15, result.Items.Count());
        }

        [TestMethod]
        public void List_PageAboveLast_IsClampedToLastPage() {
            var result = _service.List(new PageRequest { Page = 99 });

            Assert.AreEqual(3, result.Page);
        }

        [TestMethod]
        public void List_PageBelowOne_IsClampedToFirstPage() {
            var result = _service.List(new PageRequest { Page = -5 });

            Assert.AreEqual(1, result.Page);
        }

        [TestMethod]
        public void List_PageSizeZero_ReturnsAllItemsInOnePage() {
            var result = _service.List(new PageRequest { PageSize = 0 });

            Assert.AreEqual(55, result.Items.Count());
            Assert.AreEqual(1, result.TotalPages);
            Assert.AreEqual(0, result.PageSize);
        }

        [TestMethod]
        public void List_DefaultSort_IsCreatedDateDescending() {
            var items = _service.List(new PageRequest { PageSize = 0 }).Items.ToList();

            for (var i = 1; i < items.Count; i++) {
                Assert.IsTrue(items[i - 1].CreatedDate >= items[i].CreatedDate);
            }
        }

        [TestMethod]
        public void List_SortByIdAscending_SortsWholeListBeforePaging() {
            var page2 = _service.List(new PageRequest { Page = 2, SortBy = "id" });

            // 2nd page of the whole sorted list, not a sort of the 2nd page only
            Assert.AreEqual(21, page2.Items.First().Id);
            Assert.AreEqual(40, page2.Items.Last().Id);
        }

        [TestMethod]
        public void List_SortByIdDescending_StartsWithHighestId() {
            var result = _service.List(new PageRequest { SortBy = "id", SortDesc = true });

            Assert.AreEqual(55, result.Items.First().Id);
        }

        [TestMethod]
        public void List_SortPropertyIsCaseInsensitive() {
            var result = _service.List(new PageRequest { SortBy = "ID" });

            Assert.AreEqual(1, result.Items.First().Id);
        }

        [TestMethod]
        public void List_UnknownSortProperty_FallsBackToDefaultSort() {
            var expected = _service.List(new PageRequest()).Items.Select(t => t.Id).ToList();
            var actual = _service.List(new PageRequest { SortBy = "xyz" }).Items.Select(t => t.Id).ToList();

            CollectionAssert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void List_EmptyDatasource_ReturnsSingleEmptyPage() {
            _service.Repository = new EmptyRepository();

            var result = _service.List(new PageRequest { Page = 5 });

            Assert.AreEqual(1, result.Page);
            Assert.AreEqual(1, result.TotalPages);
            Assert.AreEqual(0, result.TotalItems);
            Assert.IsFalse(result.Items.Any());
        }

        /// <summary>
        /// Fake datasource to test the service rules without depending on the in-memory data
        /// </summary>
        private class EmptyRepository : IModelRepository<Todo> {
            public IEnumerable<Todo> All() { return Enumerable.Empty<Todo>(); }
            public int Count() { return 0; }
            public IEnumerable<Todo> Query(int skip, int take, string sortBy, bool sortDesc) { return Enumerable.Empty<Todo>(); }
        }
    }
}
