angular.module('main')
    .controller('MainController', ['$scope', 'aut', '$location', 'bookRepository','userRepository', 'vcRecaptchaService', function ($scope, aut, $location, bookRepository,userRepository, vcRecaptchaService) {
       
        $scope.model = {
            key: '6LdFUvESAAAAAKEbBbC_mYn6I2fJ6x4AzhjacKDa'
        };

        $scope.profile =
        {
            username: '',
            password: '',
            firstName: '',
            lastName: '',
            email:''
        }

        $scope.books = [];
        $scope.booksSize = 0;
        function getBooks() {
            bookRepository.getBooks(function (results) {
                data = results;
                data1 = data;
                $scope.books = data1.slice(0, $scope.itemsPerPage);
                $scope.booksSize = results.length;
            })
        };
        getBooks();
        $scope.maxSize = 5;
        $scope.currentPage = 1;
        $scope.itemsPerPage = 3;
        $scope.$watch("currentPage", function (newValue, oldValue) {
            data1 = data;
            offset = (newValue - 1) * $scope.itemsPerPage;
            limit = $scope.itemsPerPage;
            $scope.books = data1.slice(offset, offset + limit);
        });
       
        $scope.register = function ()
        {
            userRepository.register($scope.profile, vcRecaptchaService.data(),
                function (result)
                {
                },
                function (result) {
                });
            console.log('sending the captcha response to the server', vcRecaptchaService.data());
        }
    }]);