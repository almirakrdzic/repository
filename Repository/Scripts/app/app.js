// Main configuration file. Sets up AngularJS module and routes and any other config objects

var appRoot = angular.module('main', ['vcRecaptcha','ngRoute', 'ngSanitize', 'ui.bootstrap', 'ngResource', 'elasticsearch', 'angularStart.services', 'angularStart.directives', 'pascalprecht.translate'], ['$translateProvider', function ($translateProvider) {


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
        'edition': 'Year of publication:',
        'title': 'Title',
        'data': 'Data',
        'author': 'Author',
        'rated': 'Rated:',
        'about': 'About this book:',
        'comment': 'Comment',
        'comm': 'Comment',
        'download': 'Downloaded books',
        'profile': 'Profile',
        'first': 'First Name',
        'last': 'Lastname',
        'uploaded': 'Uploaded books',
        'year': 'Year',
        'depart': 'Department',
        'score': 'Score',
        'searchby': 'Search by:',
        'aboutme': 'About me',
        'upload': 'Upload resource',
        'register': 'Register',
        'firstname': 'FirstName',
        'lastname':'LastName'
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
        'edition': 'Godina izdavanja:',
        'title': 'Naslov',
        'data': 'Podaci',
        'author': 'Autor',
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
        'year': 'Godina upisa studija',
        'depart': 'Odsjek',
        'score': 'Nivo bitnosti',
        'searchby': 'Traži po:',
        'aboutme': 'O meni',
        'upload': 'Dodaj resurs',
        'register': 'Registruj se',
        'firstname': 'Ime',
        'lastname': 'Prezime'
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
            .when('/addBook', {
                templateUrl: '/home/addbook',
                controller: 'AddBookController',
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
            addBook: function (book,file, callback) {
                var fd = new FormData();
                fd.append('Content', file);
                fd.append('Title', book.title);
                fd.append('ISBN', book.isbn);
                fd.append('Description', book.description);
                fd.append('Edition', book.edition);

                $http.post("api/resource/addbook", fd, {
                    transformRequest: angular.identity,
                    headers: { 'Content-Type': undefined }
                })
                .success(callback)
                 .error(function () {
                 });

            },
            getUploads: function (id, callback) {

                $http.get("api/resource/getuploads/?username=" + id).success(callback);
            },
            getDownloads: function (id, callback) {

                $http.get("api/resource/getdownloads/?username=" + id).success(callback);
            },
            rateBook: function (rate, id, callback) {
                $http.get("api/resource/rate/?rate=" + rate + "&id=" + id).success(callback);
            },
            getuser: function (id, callback) {
                $http.get("api/resource/getuser/?id=" + id).success(callback);
            },
            getAuthorsForBook: function (id, callback) {
                $http.get("api/resource/getauthorsforbook/?id=" + id).success(callback);
            },
            getCommentsForBook: function (id, callback) {
                $http.get("api/resource/getcommentsforbook/?id=" + id).success(callback);
            },
            searchBooks: function (query,field, callback) {
                $http.get("api/resource/searchbooks/?query="+query+"&field="+field).success(callback);
            },
            translateQuery: function (query, callback) {
                $http.get("api/resource/translatetext/?text=" + query).success(callback);
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
            kreirajKomentar: function (comment, callback) {
                $http.post("api/resource/addkomentar",comment)
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
             editProfile: function (profile, successCallback,errorCallback) {
                 $http.post("api/user/editprofile", profile)
                 .success(successCallback)
                 .error(errorCallback);
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
             },
             register:function(profile,captcha,successCallback,errorCallback)
             {
                 $http.post("api/user/register", profile, captcha).success(successCallback).error(errorCallback);
             },
             getYear: function (callback) {
                 $http.get("api/user/getyear").success(callback);
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

         var readAsBinaryString = function (file, scope) {
             var deferred = $q.defer();

             var reader = getReader(deferred, scope);
             reader.readAsBinaryString(file);

             return deferred.promise;
         };

         return {
             readAsDataUrl: readAsDataURL,
             readAsBinaryString: readAsBinaryString
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
    .directive('ncgRequestVerificationToken', function ($http) {
        return function (scope, element, attrs) {
            $http.defaults.headers.common['RequestVerificationToken'] = attrs.ncgRequestVerificationToken || "no request verification token";
    };
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
