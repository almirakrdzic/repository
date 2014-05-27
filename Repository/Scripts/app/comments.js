angular.module('main')
    .controller('CommentsController', ['$scope', '$routeParams', 'commentRepository', function ($scope, $routeParams, commentRepository) {

        $scope.comments = [];
        $scope.booksSize = 0;
        function getComments() {
            commentRepository.getComments(function (results) {
                data = results;
                data1 = data;
                $scope.comments = data1.slice(0, $scope.itemsPerPage);
                $scope.booksSize = results.length;
            })
        };
        getComments();

        $scope.user = function getUser() {
            commentRepository.getUser($scope.comment.userId, function (results) {
                $scope.user = results;
            })
        };
    }])