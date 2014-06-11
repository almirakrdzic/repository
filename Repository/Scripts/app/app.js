// Main configuration file. Sets up AngularJS module and routes and any other config objects

var appRoot = angular.module('main', ['ngRoute', 'ui.bootstrap', 'ngResource', 'elasticsearch', 'angularStart.services', 'angularStart.directives', 'pascalprecht.translate'], ['$translateProvider', function ($translateProvider) {

    // register translation table
    $translateProvider.translations('en', {
        'NEWBOOKS': 'New resources',
        'LOGIN': 'Login',
        'DR': 'Digital repository',
        'B': 'Resources',
        'UN': 'Username',
        'PASS': 'Password',
        'UP': 'User profile',
        'EP': 'Edit profile',
        'LOGOUT': 'Logout',
        'LOGIN': 'Login',
        'kojiJezik': 'Choose language',
        'EDIT': 'Edit profile',
        'Rate': 'Rate this book:',
        'clear': 'Clear',
        'Dodano': 'This book is added by:',
        'edition': 'Edition:',
        'title': 'Title',
        'Data': 'Data',
        'Description': 'Description',
        'rated': 'Rated:',
        'about': 'About this book:',
        'comment': 'Comment',
        'comm': 'Comment',
        'download': 'Downloaded books',
        'profile': 'Profile',
        'first': 'First Name',
        'last': 'Lastname',
        'uploaded': 'Uploaded books',
        'aboutM': 'About me:',
        'year': 'Year'
    });

    $translateProvider.translations('ba', {
        'NEWBOOKS': 'Novi resursi',
        'LOGIN': 'Prijava',
        'DR': 'Digitalni repozitorij',
        'B': 'Resursi',
        'UN': 'Korisničko ime',
        'PASS': 'Šifra',
        'UP': 'Korisnički profil',
        'EP': 'Edituj profil',
        'LOGOUT': 'Odjavi se',
        'LOGIN': 'Prijavi se',
        'kojiJezik': 'Odaberi jezik',
        'EDIT': 'Izmjena profila',
        'Rate': 'Ocijeni ovu knjigu:',
        'clear': 'Obriši',
        'Dodano': 'Knjigu je dodao:',
        'edition': 'Izdanje:',
        'title': 'Naslov',
        'Data': 'Podaci',
        'Description': 'Opis',
        'rated': 'Ocijenilo:',
        'about': 'O knjizi:',
        'comment': 'Komentar',
        'comm': 'Ostavi komentar',
        'download': 'Preuzete knjige',
        'profile': 'Profil',
        'joined': 'Pridruzio',
        'first': 'Ime',
        'last': 'Prezime',
        'uploaded': 'Dodane knjige',
        'aboutM': 'O meni:',
        'year': 'Godina upisa studija'
    }).preferredLanguage('ba');


}]);     //Define the main module

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
            .when('/comments', {
                templateUrl: '/home/comments',
                controller: 'CommentsController',
                access: { isPublic: false }
            })

            .when('/useruploads/:username', {
                templateUrl: '/home/useruploads',
                controller: 'UserUploadsController',
                access: { isPublic: false }
            })
            .when('/userdownloads/:username', {
                templateUrl: '/home/userdownloads',
                controller: 'UserDownloadsController',
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
            getUploads: function (id, callback) {

                $http.get("api/resource/getuploads/?username=" + id).success(callback);
            },
            getDownloads: function (id, callback) {

                $http.get("api/resource/getdownloads/?username=" + id).success(callback);
            },
            rateBook: function (rate,id,callback) {
                $http.get("api/resource/rate/?rate=" + rate+"&id="+id).success(callback);
            },
            getuser: function (id, callback) {
                $http.get("api/resource/getuser/?id=" + id).success(callback);
            },
            getAuthorsForBook: function (id, callback) {
                $http.get("api/resource/getauthorsforbook/?id=" + id).success(callback);
            },
            getCommentsForBook: function (id, callback) {
            $http.get("api/resource/getcommentsforbook/?id=" + id).success(callback);
        }
        }
    })

    .factory('commentRepository', function ($http) {
        return {
            getComments: function (callback) {

                $http.get("api/resource/getcomments").success(callback);
            },
            getu: function (id, callback) {

                $http.get("api/resource/getu").success(callback);
            },
            kreirajKomentar: function (text,idBook, callback) {
                $http.get("api/resource/addkomentar/?text="+text+"&idBook="+idBook)
                .success(callback)
                 .error(function () {
                 });

            }
        }
    })
     .factory('userRepository', function ($http) {
         return {           
             getUser: function (username, callback) {

                 $http.get("api/user/userdetails/?username=" + username).success(callback);
             },
             editProfile: function (profile, callback) {                
                 $http.post("api/user/editprofile", profile)
                 .success(function () {
                 })
                 .error(function () {
                 });
             },
             postPicture: function (file, profile, callback) {

                 var fd = new FormData();
                 fd.append('file', file);
                 fd.append('profile', angular.toJson(profile));
                 $http.post("api/user/postpicture", fd, {
                     transformRequest: angular.identity,
                     headers: { 'Content-Type': undefined }
                 })
                 .success(function () {
                 })
                 .error(function () {
                 });
             }
         }
     })
    .service('es', function (esFactory) {
        return esFactory({ host: 'localhost:9200' });
    })
     .service('fileReader', function ($q, $log) {
         
         var onLoad = function (reader, deferred, scope) {
             return function () {
                 scope.$apply(function () {
                     deferred.resolve(reader.result);
                 });
             };
         };

         var onError = function (reader, deferred, scope) {
             return function () {
                 scope.$apply(function () {
                     deferred.reject(reader.result);
                 });
             };
         };

         var onProgress = function (reader, scope) {
             return function (event) {
                 scope.$broadcast("fileProgress",
                     {
                         total: event.total,
                         loaded: event.loaded
                     });
             };
         };

         var getReader = function (deferred, scope) {
             var reader = new FileReader();
             reader.onload = onLoad(reader, deferred, scope);
             reader.onerror = onError(reader, deferred, scope);
             reader.onprogress = onProgress(reader, scope);
             return reader;
         };

         var readAsDataURL = function (file, scope) {
             var deferred = $q.defer();

             var reader = getReader(deferred, scope);
             reader.readAsDataURL(file);

             return deferred.promise;
         };

         return {
             readAsDataUrl: readAsDataURL
         };
     })
    .factory('aut', function ($http) {
        var user = {};       

        user.username = '';

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
    .controller('RootController', ['$scope', '$route', '$routeParams', '$location', 'aut', '$modal', '$translate', function ($scope, $route, $routeParams, $location, aut, $modal, $translate) {

        $scope.changeLanguage = function (key) {
            $translate.use(key);
        };

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
            $scope.username = results.replace('"', '');
            aut.username = $scope.username;

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
                aut.username = $scope.username;

                var modalInstance = $modal.open({
                    templateUrl: '/home/edituserprofile',
                    controller: 'EditProfileController',
                    size: 'sm'                   
                });

                modalInstance.result.then(function (selectedItem) {
                    
                }, function () {
                   
                });            

            },
            function (results) {
                $scope.error = "Username or password are incorrect";
                $scope.seeError = true;
            });
            $location.path('/');
        };

        $scope.editProfile = function () {     

                var modalInstance = $modal.open({
                    templateUrl: '/home/edituserprofile',
                    controller: 'EditProfileController',
                    size: 'sm'
                });

                modalInstance.result.then(function (selectedItem) {

                }, function () {

                });         
        };

        $scope.signout = function () {
            aut.logoutUser(function (results) {
                $scope.authenticated = false;
                $scope.username = '';
                aut.username = $scope.username;
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
