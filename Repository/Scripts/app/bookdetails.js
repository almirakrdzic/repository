angular.module('main')
    .controller('BookDetailsController', ['$scope','$routeParams','bookRepository', function ($scope, $routeParams, bookRepository) {
        function getBook() {
            bookRepository.getBook($routeParams.id, function (results) {
                $scope.book = results;
                $scope.user1 = results.added_by;
     

            })
        };
        getBook();

        $scope.authors = [];
        function getAuthorsForBook() {
            bookRepository.getAuthorsForBook($routeParams.id, function (results) {
                $scope.authors = results;
            })
        };
        getAuthorsForBook();

        $scope.comments = [];
        function getCommentsForBook() {
            bookRepository.getCommentsForBook($routeParams.id, function (results) {
                $scope.comments = results;
            })
        };
        getCommentsForBook();

               
        $scope.Userrate =0 ;      
        $scope.max = 5;
        $scope.isReadonly = false;
        $scope.rate = function rate() {
            bookRepository.rateBook($scope.Userrate,$scope.book.id, function (results) {
                $scope.book = results;
                $scope.Userrate = 0;
            })
        };
    }]);