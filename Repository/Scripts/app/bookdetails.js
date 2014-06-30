angular.module('main')
    .controller('BookDetailsController', ['$scope', '$routeParams', 'bookRepository', 'commentRepository', function ($scope, $routeParams, bookRepository, commentRepository) {
        function getBook() {
            bookRepository.getBook($routeParams.id, function (results) {
                $scope.book = results;
                $scope.user1 = results.added_by;


            })
        };
        getBook();

        $scope.comments = [];
        $scope.imageData = [];
        function getCommentsForBook() {
            bookRepository.getCommentsForBook($routeParams.id, function (results) {
                $scope.comments = results;
                for (i = 0; i < results.length; i++)
                    $scope.imageData[(results[i]).idUser.id] = "data:image/png;base64," + (results[i]).idUser.image;
            })
        };
        getCommentsForBook();

        $scope.tekstKomentara = '';

        $scope.kreirajKomentar = function (id) {

            var komentar =
            {
                Text: '',
                IdBook:''
            }
            komentar.Text = $scope.tekstKomentara;
            komentar.IdBook = $scope.book.id;
            commentRepository.kreirajKomentar(komentar, function (results) {
                getCommentsForBook();
                $scope.tekstKomentara = '';
            })
        };

        $scope.Userrate = 0;
        $scope.max = 5;
        $scope.isReadonly = false;
        $scope.rate = function rate() {
            bookRepository.rateBook($scope.Userrate, $scope.book.id, function (results) {
                $scope.book.rating = results;
                $scope.book.people = $scope.book.people + 1;
                $scope.Userrate = 0;
            })
        };
    }]);