/// <reference path="D:\CSharp\shop\Web\Assets/admin/libs/angular/angular.js" />

(function (app) {
    app.controller('contactAddController', contactAddController);

    contactAddController.$inject = ['apiService', '$scope', 'notificationService', '$state'];

    function contactAddController(apiService, $scope, notificationService, $state) {

        $scope.contact = {
            Status: true
        }
        $scope.AddContact = AddContact;
        function AddContact() {
            apiService.post('api/contact/create', $scope.contact, function (result) {
                notificationService.displaySuccess(result.data.Name + ' đã được thêm mới.');
                $state.go('contacts');
            }, function (error) {
                notificationService.displayError('Thêm mới không thành công.');
            });
        }

        // setup editor options
        $scope.editorOptions = {
            language: 'en',
            height:'200'
        };
    }
})(angular.module('fashionshop.contacts'));