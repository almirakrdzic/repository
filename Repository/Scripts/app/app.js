// Main configuration file. Sets up AngularJS module and routes and any other config objects

var user =
       {
           username: '',
           authenticated: false
       };

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
    .service('aut', function ($http) {
       
        return {
            loginUser: function (login) {
                $http.post('api/user/login', login)
               .success(function (data, status, headers, config) {
                   user.username = 'akrdzic1';                  
                   user.authenticated = true;                  
               })
               .error(function (data, status, headers, config) {
                  user.authenticated = false;
               });
            },
            logoutUser: function () {
                $http.post('api/user/signout')
               .success(function (data, status, headers, config) {
                   user.username = '';
                   user.authenticated = false;
               })               
            },
            isAuthenticated: function () {
                return user.authenticated;
            },
            username: function () {
                return user.username;
            }
        }
    })
    .controller('RootController', ['$scope', '$route', '$routeParams', '$location','aut', function ($scope, $route, $routeParams, $location,aut) {
        $scope.authenticated = user.authenticated;
        $scope.username = user.username;
        $scope.login =
        {
           Username: '',
           Password: '',
           RememberMe: false
        }

        $scope.signin = function (login) {
            aut.loginUser(login);
            $scope.authenticated = true;
            $scope.username = login.username;
            $location.path('/books');
        };

        $scope.signout = function () {
            aut.logoutUser();
            $scope.authenticated = false;
            $scope.username = '';
            $location.path('/');
        };
        $scope.$on('$routeChangeSuccess', function (e, current, previous) {
            $scope.activeViewPath = $location.path();
        });
    }]);
