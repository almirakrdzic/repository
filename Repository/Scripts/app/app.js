// Main configuration file. Sets up AngularJS module and routes and any other config objects

var appRoot = angular.module('main', ['ngRoute','ui.bootstrap', 'ngResource','elasticsearch', 'angularStart.services', 'angularStart.directives']);     //Define the main module

appRoot
    .config(['$routeProvider', function ($routeProvider) {
        //Setup routes to load partial templates from server. TemplateUrl is the location for the server view (Razor .cshtml view)
        $routeProvider
            .when('/home', {
                templateUrl: '/home/main',
                controller: 'MainController',
                access: { isPublic: true }
            })
            .when('/userprofile/:username', {
                templateUrl: '/home/userprofile',
                controller: 'UserProfileController',
                access: { isPublic: false }
            })
            .when('/books', {
                templateUrl: '/home/books',
                controller: 'BooksController',
                access: { isPublic: false }
            })
            .when('/book/details/:id', {
                templateUrl: '/home/bookdetalis',
                controller: 'BookDetailsController',
                access: { isPublic: false }
            })
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

        user.isLogged = function (callbackSuccess, callbackError) {
            $http.get('api/user/islogged')
           .success(callbackSuccess)
           .error(callbackError);
        };

        return user;
    })
    .controller('RootController', ['$scope', '$route', '$routeParams', '$location', 'aut', function ($scope, $route, $routeParams, $location, aut) {

        $scope.authenticated = false;
        $scope.username = '';
        $scope.error = '';
        $scope.seeError = false;
        $scope.login =
        {
           Username: '',
           Password: '',
           RememberMe: false
        }

        aut.isLogged(function (results) {
            $scope.authenticated = true;
            results = results.replace('"', '');
            $scope.username = results.replace('"','');

        },
             function (results) {
                 $scope.authenticated = false;
                 $scope.username = '';          
                
             });

        $scope.signin = function (isValid) {
            aut.loginUser($scope.login, function (results) {               
                $scope.authenticated = true;
                $scope.username = $scope.login.Username;
                $scope.login.Username = '';
                $scope.login.Password = '';
                $scope.seeError = false;
            },
            function (results) {
                $scope.error = "Username or password are incorrect";
                $scope.seeError = true;
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

        $scope.$on('$routeChangeStart', function (e, current, previous) {
            if (current.access.isPublic == true) { return; }
            aut.isLogged(function (results) {
               
            },
             function (results) {
                 $scope.authenticated = false;
                 $scope.username = '';
                 $scope.seeError = true;
                 $location.path('/home');
             });           
        });

        $scope.$on('$routeChangeSuccess', function (e, current, previous) {
            $scope.activeViewPath = $location.path();
        });
    }]);
