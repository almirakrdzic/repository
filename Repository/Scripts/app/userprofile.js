angular.module('main')
    .controller('UserProfileController', ['$scope', 'userRepository', '$routeParams', 'bookRepository', function ($scope, userRepository, $routeParams, bookRepository) {

        function getUser() {
            userRepository.getUser($routeParams.username, function (results) {
                $scope.userDetails = results;
                $scope.imageSrc = "data:image/png;base64," + results.image;
            })
        };
        getUser();

        $scope.books = [];
        $scope.booksSize = 0;
        $scope.getUploads = function () {
            bookRepository.getUploads($routeParams.username, function (results) {
                data = results;
                data1 = data;
                $scope.books = data1.slice(0, $scope.itemsPerPage);
                $scope.booksSize = results.length;
            })
        };
        $scope.getDownloads = function () {
            bookRepository.getDownloads($routeParams.username, function (results) {
                data = results;
                data1 = data;
                $scope.books = data1.slice(0, $scope.itemsPerPage);
                $scope.booksSize = results.length;
            })
        };

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