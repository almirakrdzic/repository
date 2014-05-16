
appRoot.controller('BooksController', function ($scope, $location, $resource,es) {

    $scope.books = []
    $scope.booksSize = 0;
    $scope.maxSize = 5;
    $scope.currentPage = 1;
    $scope.itemsPerPage = 4;
    $scope.rate = 7;
    $scope.field = "Title";
    function getAllBooks() {
        es.search({
            index: 'books',
            size: 50,
            body: {
                "query": {
                    "query_string": {
                        "query": "*",
                        "fields": ["title"]
                    }
                }
            }

        }).then(function (response) {
            data = response.hits.hits;
            data1 = data;
            $scope.books = data1.slice(0, $scope.itemsPerPage);
            $scope.booksSize = data.length;
        });
    };
    getAllBooks();

    $scope.search = function () {
        var field = $scope.field;
        es.search({
            index: 'books',
            size: 50,
            body: {
                "query": {
                    "query_string": {
                        "query": $scope.query,
                        "fields": [field.toLowerCase()]
                    }
                }
            }

        }).then(function (response) {
            data = response.hits.hits;
            data1 = data;
            $scope.books = data1.slice(0, $scope.itemsPerPage);
            $scope.booksSize = data.length;
        });
    };

    $scope.$watch("currentPage", function (newValue, oldValue) {
        data1 = data;
        offset = (newValue - 1) * $scope.itemsPerPage;
        limit = $scope.itemsPerPage;
        $scope.books = data1.slice(offset, offset + limit);
    });

});