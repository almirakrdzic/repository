angular.module('main')
    .controller('MainController', ['$scope','aut','$location','bookRepository', function ($scope, aut,$location,bookRepository) {
       
        $scope.books = [];
        $scope.booksSize = 0;
        function getBooks() {
            bookRepository.getBooks(function (results) {
                data = results;
                data1 = data;
                $scope.books = data1.slice(0, $scope.itemsPerPage);
                $scope.booksSize = results.length;
            })
        };
        getBooks();
        $scope.maxSize = 5;
        $scope.currentPage = 1;
        $scope.itemsPerPage = 2;
        $scope.$watch("currentPage", function (newValue, oldValue) {
            data1 = data;
            offset = (newValue - 1) * $scope.itemsPerPage;
            limit = $scope.itemsPerPage;
            $scope.books = data1.slice(offset, offset + limit);
        });
       
        
    }]);