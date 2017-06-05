/// <reference path="/Assets/admin/libs/angular/angular.js" />
(function (app) {
    'use strict';

    app.controller('applicationUserEditController', applicationUserEditController);

    applicationUserEditController.$inject = ['$scope', 'apiService', 'notificationService', '$location', '$stateParams'];

    function applicationUserEditController($scope, apiService, notificationService, $location, $stateParams) {
        $scope.account = {}

        $scope.checkAll = function () {
            $scope.account.Groups = angular.copy($scope.groups);
        };
        $scope.uncheckAll = function () {
            $scope.account.Groups = [];
        };

        function loadDetail() {
            apiService.get('/api/applicationUser/detail/' + $stateParams.id, null,
            function (result) {
                $scope.account = result.data;
            },
            function (result) {
                notificationService.displayError(result.data);
            });
        }
        loadDetail();

        $scope.updateAccount = updateAccount;
        function updateAccount() {
            apiService.put('/api/applicationUser/update', $scope.account, addSuccessed, addFailed);
        }

        function addSuccessed() {
            notificationService.displaySuccess($scope.account.FullName + ' đã được cập nhật thành công.');

            $location.url('application_users');
        }
        function addFailed(response) {
            notificationService.displayError(response.data.Message);
            notificationService.displayErrorValidation(response);
        }
        function loadGroups() {
            apiService.get('/api/applicationGroup/getlistall',
                null,
                function (response) {
                    $scope.groups = response.data;
                }, function (response) {
                    notificationService.displayError('Không tải được danh sách nhóm.');
                });

        }
        loadGroups();

        $(function () {
            $("#datepicker").datepicker({
                changeMonth: true,
                changeYear: true
            });
        });
    }
})(angular.module('fashionshop.application_users'));