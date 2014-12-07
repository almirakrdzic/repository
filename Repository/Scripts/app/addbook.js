appRoot.controller('AddBookController', function ($scope, $location, $resource, es, bookRepository, $translate, fileReader) {

   $scope.book = {
       Title: "",
       ISBN: "",
       Edition: "",
       Description: "",
       Content :null
   }

    $scope.fileTitle = "Select";

    $scope.addBook = function() {
        bookRepository.addBook($scope.book,$scope.file, function() {

        });
    };

   $scope.switchTitle = function(file) {
       $scope.fileTitle = "Change";
   };
    
});
