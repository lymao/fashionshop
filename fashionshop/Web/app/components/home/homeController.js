/// <reference path="D:\CSharp\shop\Web\Assets/admin/libs/angular/angular.js" />
(function (app) {
    app.controller('homeController', homeController);

    homeController.inject = ['authData', '$injector'];

    function homeController(authData, $injector) {
        if (authData.authenticationData.IsAuthenticated == false) {
            var stateService = $injector.get('$state');
            stateService.go('login');
        }

    }

})(angular.module('fashionshop'));