angular.module('main')
    .controller('BookDetailsController', ['$scope','$routeParams','bookRepository', function ($scope, $routeParams, bookRepository) {
        function getBook() {
            bookRepository.getBook($routeParams.id, function (results) {
                $scope.book = results;
            })
        };
        getBook();
        $scope.Userrate =0 ;      
        $scope.max = 10;
        $scope.isReadonly = false;
        $scope.rate = function rate() {
            bookRepository.rateBook($scope.Userrate,$scope.book.id, function (results) {
                $scope.book = results;
            })
        };
    }]);