/// <reference path="/Assets/admin/libs/angular/angular.js" />
(function (app) {
    'use strict';

    app.controller('applicationUserAddController', applicationUserAddController);

    applicationUserAddController.$inject = ['$scope', 'apiService', 'notificationService', '$location', 'commonService'];

    function applicationUserAddController($scope, apiService, notificationService, $location, commonService) {
        $scope.account = {
            Groups: []
        }

        $scope.checkAll = function () {
            $scope.account.Groups = angular.copy($scope.groups);
        };
        $scope.uncheckAll = function () {
            $scope.account.Groups = [];
        };

        $scope.addAccount = addAccount;

        function addAccount() {
            apiService.post('/api/applicationUser/add', $scope.account, addSuccessed, addFailed);
        }

        function addSuccessed() {
            notificationService.displaySuccess($scope.account.UserName + ' đã được thêm mới.');

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