angular.module('main')
    .controller('CommentsController', ['$scope', 'commentRepository', 'aut', 'fileReader', function ($scope, commentRepository, aut, fileReader) {

        $scope.comments = [];

        function getComments() {
            commentRepository.getComments(function (results) {
                $scope.comments = results;        
            })
        };
        getComments();

        $scope.komentar =
        {
            Text: '',
            IdUser: '7',
            IdBook: '5'

        }

        $scope.ok = function () {
            commentRepository.kreirajKomentar($scope.komentar, function (results) {

            });
           
        };


        //$scope.kreirajKomentar = function () {
        //    var comment = {
        //        "Text": $scope.text,
        //        "IdUser": 1, 
        //        "IdBook": 1 
        //    };
        //    commentRepository.kreirajKomentar(comment)

        //    .success(function () {
        //        $scope.comments.push(comment);
        //    });

        //    $scope.text = '';
        //};

       
    }])