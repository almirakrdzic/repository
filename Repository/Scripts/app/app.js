// Main configuration file. Sets up AngularJS module and routes and any other config objects

var appRoot = angular.module('main', ['ngRoute','ui.bootstrap', 'ngResource','elasticsearch', 'angularStart.services', 'angularStart.directives']);     //Define the main module

appRoot
    .config(['$routeProvider', function ($routeProvider) {
        //Setup routes to load partial templates from server. TemplateUrl is the location for the server view (Razor .cshtml view)
        $routeProvider
            .when('/home', { templateUrl: '/home/main', controller: 'MainController' })           
            .when('/userprofile/:username', { templateUrl: '/home/userprofile', controller: 'UserProfileController' })
            .when('/books', { templateUrl: '/home/books', controller: 'BooksController' })
            .when('/book/details/:id', { templateUrl: '/home/bookdetalis', controller: 'BookDetailsController' })           
            .otherwise({ redirectTo: '/home' });
    }])
    .factory('bookRepository', function ($http) {       
        return {           
            getBooks: function (callback) {

                $http.get("api/resource/get").success(callback);
            },
            getBook: function (id, callback) {

                $http.get("api/resource/get/?id=" + id).success(callback);
            },
            rateBook: function (rate,id,callback) {
                $http.get("api/resource/rate/?rate=" + rate+"&id="+id).success(callback);
            }
        }
    })
     .factory('userRepository', function ($http) {
         return {           
             getUser: function (username, callback) {

                 $http.get("api/user/userdetails/?username=" + username).success(callback);
             }
         }
     })
    .service('es', function (esFactory) {
        return esFactory({ host: 'localhost:9200' });
    })
    .factory('aut', function ($http) {
        var user = {};       

        user.loginUser = function (login, callbackSuccess, callbackError) {
            $http.post('api/user/login', login)
           .success(callbackSuccess)
           .error(callbackError);
        };

        user.logoutUser = function (callbackSuccess, callbackError) {
            $http.post('api/user/signout')
           .success(callbackSuccess)
           .error(callbackError);
        };

        return user;
    })
    .controller('RootController', ['$scope', '$route', '$routeParams', '$location','aut', function ($scope, $route, $routeParams, $location,aut) {
        $scope.authenticated = false;
        $scope.username = '';
        $scope.login =
        {
           Username: '',
           Password: '',
           RememberMe: false
        }

        $scope.signin = function (login) {
            aut.loginUser(login, function (results) {
                $scope.authenticated = true;
                $scope.username = login.Username;
                $scope.login.Username = '';
                $scope.login.Password = '';
            },
            function (results) {              
            });
            $location.path('/');
        };

        $scope.signout = function () {
            aut.logoutUser(function (results) {
                $scope.authenticated = false;
                $scope.username = '';
            },
             function (results) {               
             });
            $location.path('/');
        };
        $scope.$on('$routeChangeSuccess', function (e, current, previous) {
            $scope.activeViewPath = $location.path();
        });
    }]);
