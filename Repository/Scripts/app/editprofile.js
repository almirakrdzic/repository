angular.module('main')
    .controller('EditProfileController', ['$scope', 'userRepository', '$modalInstance', 'userRepository', 'aut', 'fileReader', function ($scope, userRepository, $modalInstance, userRepository, aut, fileReader) {

       function getUser() {
            userRepository.getUser(aut.username, function (results) {
                $scope.profile = results;
                $scope.imageSrc = "data:image/png;base64," + results.image;
            })
        };
        getUser();      

        $scope.ok = function (file) {
            userRepository.editProfile($scope.profile, function (results) {

            });
            $modalInstance.close($scope.userDetails);
        };

        $scope.cancel = function () {
            $modalInstance.dismiss('cancel');
        };

        $scope.readFile = function (file) {    
           
            fileReader.readAsDataUrl(file, $scope)
                     .then(function (result) {
                         $scope.imageSrc = result;
                         $scope.profile.image = result;
                     });
        };


    }]);