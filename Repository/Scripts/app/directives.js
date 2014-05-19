// Define you directives here. Directives can be added to same module as 'main' or a seperate module can be created.

var angularStartDirectives = angular.module('angularStart.directives', []);     //Define the directive module

angularStartDirectives
    .directive('testDirective', function () {             //use as 'test-directive' in HTML
    return {
        restrict: 'A',
        link: function (scope, element, attrs) {
            console.log('Directive linked.');
        }
    };
    })
 .directive('fileInput', function ($parse) {
     return {
         restrict: "EA",
         template: "<input type='file' />",
         replace: true,
         link: function (scope, element, attrs) {

             var modelGet = $parse(attrs.fileInput);
             var modelSet = modelGet.assign;
             var onChange = $parse(attrs.onChange);

             var updateModel = function () {
                 scope.$apply(function () {
                     modelSet(scope, element[0].files[0]);
                     onChange(scope);
                 });
             };

             element.bind('change', updateModel);
         }
     }
 });