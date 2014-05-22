angular.module('main')
    .controller('UserDownloadsController', ['$scope', 'aut', '$location', 'bookRepository', '$routeParams', function ($scope, aut, $location, bookRepository, $routeParams) {

        $scope.books = [];
        $scope.booksSize = 0;
        function getDownloads() {
            bookRepository.getDownloads($routeParams.username, function (results) {
                data = results;
                data1 = data;
                $scope.books = data1.slice(0, $scope.itemsPerPage);
                $scope.booksSize = results.length;
            })
        };

        getDownloads();
        $scope.maxSize = 5;
        $scope.currentPage = 1;
        $scope.itemsPerPage = 4;
        $scope.$watch("currentPage", function (newValue, oldValue) {
            data1 = data;
            offset = (newValue - 1) * $scope.itemsPerPage;
            limit = $scope.itemsPerPage;
            $scope.books = data1.slice(offset, offset + limit);
        });


    }]);