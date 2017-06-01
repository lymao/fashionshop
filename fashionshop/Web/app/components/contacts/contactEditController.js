/// <reference path="D:\CSharp\shop\Web\Assets/admin/libs/angular/angular.js" />

(function (app) {
    app.controller('contactEditController', contactEditController);

    contactEditController.$inject = ['$scope', 'apiService', '$stateParams', 'notificationService', '$state'];

    function contactEditController($scope, apiService, $stateParams, notificationService, $state) {
        $scope.contact = [];
        function loadContactDetail() {
            apiService.get('api/contact/getbyid/' + $stateParams.id, null, function (result) {
                $scope.contact = result.data;
            }, function (error) {
                notificationService.displayError(error.data);
            });
        }
        loadContactDetail();

        // setup editor options
        $scope.editorOptions = {
            language: 'en',
            height:'200'
        };

        $scope.UpdateContact = UpdateContact;
        function UpdateContact() {
            apiService.put('api/contact/update', $scope.contact,
                function (result) {
                    notificationService.displaySuccess(result.data.Name + ' đã được cập nhật.');
                    $state.go('contacts');
                }, function (error) {
                    notificationService.displayError('Cập nhật không thành công.');
                });
        }
    }

})(angular.module('fashionshop.contacts'));