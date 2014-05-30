angular.module('main')
    .controller('UserProfileController', ['$scope', 'userRepository', '$routeParams', function ($scope, userRepository, $routeParams) {

        function getUser() {
            userRepository.getUser($routeParams.username, function (results) {
                $scope.userDetails = results;
                $scope.imageSrc = "http://localhost:4416/Account/GetProfilePic/?username=" + results.username;
            })
        };
        getUser();

    }]);