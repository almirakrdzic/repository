angular.module('main')
    .controller('EditProfileController', ['$scope', 'userRepository', '$modalInstance', 'userRepository', 'aut', 'fileReader', function ($scope, userRepository, $modalInstance, userRepository, aut, fileReader) {
       
        $scope.profile =
        {
            Username: aut.username,
            FirstName: '',
            LastName: '',
            Email: '',
            Year: '',
            Department: '',
            AbouteMe:''

        }

        function getUser() {
            userRepository.getUser(aut.username, function (results) {
                $scope.profile = results;
               
            })
        };
        getUser();
        $scope.imageSrc = "http://localhost:4416/Account/GetProfilePic/?username=" + $scope.profile.Username;
   

        $scope.ok = function (file) {
            userRepository.editProfile($scope.profile, function (results) {

            });
            $modalInstance.close($scope.userDetails);
        };

        $scope.cancel = function () {
            $modalInstance.dismiss('cancel');
        };

        $scope.readFile = function (file) {
            userRepository.postPicture(file, $scope.profile, function (results) {

            });
           fileReader.readAsDataUrl(file, $scope)
                    .then(function (result) {
                        $scope.imageSrc = result;
                    });
        };   


    }]);