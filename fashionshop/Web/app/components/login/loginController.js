/// <reference path="D:\CSharp\shop\Web\Assets/admin/libs/angular/angular.min.js" />
(function (app) {

    app.controller('loginController', ['$scope', 'loginService', '$injector', 'notificationService',
    function loginController($scope, loginService, $injector, notificationService) {

        $scope.loginData = {
            userName: '',
            passWord: ''
        };

        $scope.loginSubmit = function () {
            loginService.login($scope.loginData.userName, $scope.loginData.passWord).then(function (response) {
                if (response != null && response.data.error != undefined) {
                    notificationService.displayError(response.data.error_description);
                }
                else {
                    var stateService = $injector.get('$state');
                    stateService.go('home');
                }
            });

        }
    }
    ]);



})(angular.module('fashionshop'));