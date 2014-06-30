angular.module('main')
    .controller('EditProfileController', ['$scope', 'userRepository', '$modalInstance', 'userRepository', 'aut', 'fileReader', function ($scope, userRepository, $modalInstance, userRepository, aut, fileReader) {

        $scope.seeFirstNameError = false;
        $scope.seeLastNameError = false;
        $scope.seeEmailError = false;
        $scope.seeAboutMeError = false;

        userRepository.getYear(function(result){
            var year = result;
            var years = [];
            for (i = 1999; i <= year; i++)
                years[i - 1999] = i;
            $scope.years = years;
        });
       
       

       function getUser() {
            userRepository.getUser(aut.username, function (results) {
                $scope.profile = results;
                $scope.imageSrc = "data:image/png;base64," + results.image;
            })
        };
        getUser();      

        $scope.ok = function (file) {
            userRepository.editProfile($scope.profile, function (results) {
                $modalInstance.close($scope.userDetails);
            }, function (result) {
                if (result.modelState["profile.FirstName"] != null)
                    $scope.seeFirstNameError = true;
                else
                    $scope.seeFirstNameError = false;

                if (result.modelState["profile.LastName"] != null)
                    $scope.seeLastNameError = true;
                else
                    $scope.seeLastNameError = false;

                if (result.modelState["profile.Email"] != null)
                    $scope.seeEmailError = true;
                else
                    $scope.seeEmailError = false;

                if (result.modelState["profile.AboutMe"] != null)
                    $scope.seeAboutMeError = true;
                else
                    $scope.seeAboutMeError = false;                
            }
            );
           
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