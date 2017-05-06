/// <reference path="D:\CSharp\shop\Web\Assets/admin/libs/angular/angular.min.js" />

(function (app) {
    app.controller('rootController', rootController);

    rootController.$inject = ['$state', 'authData', 'loginService', '$scope', 'authenticationService'];

    function rootController($state, authData, loginService, $scope, authenticationService) {
        $scope.logOut = function () {
            loginService.logOut();
            $state.go('login');
        }
        $scope.authentication = authData.authenticationData;
        //$scope.sideBar = "/app/shared/views/sideBar.html";
        //Lỗi ở hàm validateRequest() này khi refresh lại trang thì bị out ra trang login
        //authenticationService.validateRequest();
    }
})(angular.module('fashionshop'));