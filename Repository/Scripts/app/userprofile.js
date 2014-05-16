angular.module('main')
    .controller('UserProfileController', ['$scope', 'userRepository', '$routeParams', function ($scope, userRepository, $routeParams) {

        function getUser() {
            userRepository.getUser($routeParams.username, function (results) {                            
                $scope.userDetails = results;
            })
        };
        getUser();
    }]);