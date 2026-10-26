(function (angular) {
    "use strict";

    angular
        .module("todoApp")
        .directive("todoPaginatedList", [todoPaginatedList])
        .directive("pagination", [pagination]);

    /**
     * Directive definition function of 'todoPaginatedList'.
     *
     * Paginated and sortable list of Todo items.
     * Owns the list state (page, page size, sorting, loading) and fetches each page from the server.
     *
     * @returns {} directive definition object
     */
    function todoPaginatedList() {
        var directive = {
            restrict: "E",
            templateUrl: "app/templates/todo.list.paginated.html",
            scope: {}, // isolated scope: the list owns its own state
            controller: ["$scope", "$http", controller]
        };

        function controller($scope, $http) {
            $scope.todos = [];
            $scope.loading = false;
            $scope.paging = { page: 1, pageSize: 20, totalItems: 0, totalPages: 1 };
            $scope.sort = { by: null, desc: false }; // null = server default (createdDate desc), no indicator

            var lastRequestId = 0; // used to discard responses of outdated requests

            $scope.load = function (page, pageSize) {
                var requestId = ++lastRequestId;
                $scope.loading = true;
                $http.get("api/Todo/Todos", {
                    params: { page: page, pageSize: pageSize, sortBy: $scope.sort.by, sortDesc: $scope.sort.desc }
                }).then(function (response) {
                    if (requestId !== lastRequestId) { return; } // a newer request is on its way
                    var result = response.data;
                    $scope.todos = result.items;
                    $scope.paging.page = result.page;
                    $scope.paging.pageSize = result.pageSize;
                    $scope.paging.totalItems = result.totalItems;
                    $scope.paging.totalPages = result.totalPages;
                }).finally(function () {
                    if (requestId === lastRequestId) { $scope.loading = false; }
                });
            };

            $scope.sortBy = function (property) {
                // same column toggles the order, a new column starts ascending
                $scope.sort.desc = $scope.sort.by === property ? !$scope.sort.desc : false;
                $scope.sort.by = property;
                $scope.load(1, $scope.paging.pageSize); // back to first page, keep page size
            };

            $scope.load(1, 20);
        }

        return directive;
    }

    /**
     * Directive definition function of 'pagination' directive.
     *
     * Reusable pagination component: model agnostic, it only displays the paging state
     * and notifies the parent through 'onChange({ page, pageSize })' when the user navigates.
     * The parent is responsible for fetching the page and updating the bound values.
     *
     * @returns {} directive definition object
     */
    function pagination() {
        var directive = {
            restrict: "E",
            templateUrl: "app/templates/pagination.html",
            scope: {
                page: "=",
                pageSize: "=", // 0 means 'all'
                totalItems: "=",
                totalPages: "=",
                onChange: "&" // invoked with { page: n, pageSize: n }
            },
            controller: ["$scope", controller]
        };

        function controller($scope) {
            $scope.pageSizes = [
                { value: 10, label: "10" },
                { value: 20, label: "20" },
                { value: 30, label: "30" },
                { value: 0, label: "all" }
            ];

            // local copies so the inputs don't write directly into the parent's state
            $scope.$watch("page", function (page) { $scope.inputPage = page; });
            $scope.$watch("pageSize", function (size) { $scope.selectedSize = size; });

            $scope.goTo = function (page) {
                if (!page) { return; } // empty or invalid number input
                page = Math.max(1, Math.min(page, $scope.totalPages || 1));
                $scope.inputPage = page;
                $scope.onChange({ page: page, pageSize: $scope.pageSize });
            };

            $scope.changePageSize = function (size) {
                $scope.onChange({ page: 1, pageSize: size });
            };
        }

        return directive;
    }

})(angular);
