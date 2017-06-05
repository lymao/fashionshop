/// <reference path="/Assets/admin/libs/angular/angular.js" />
(function (app) {
    'use strict';

    app.controller('applicationGroupAddController', applicationGroupAddController);

    applicationGroupAddController.$inject = ['$scope', 'apiService', 'notificationService', '$location', 'commonService'];

    function applicationGroupAddController($scope, apiService, notificationService, $location, commonService) {
        $scope.group = {
            ID: 0,
            Roles: []
        }

        function loadRoles() {
            apiService.get('/api/applicationRole/getlistall', null, function (response) {
                $scope.roles = response.data;
            }, function (response) {
                notificationService.displayError('Không tải được danh sách quyền.');
            });
        }
        loadRoles();

        $scope.checkAll = function () {
            $scope.group.Roles = angular.copy($scope.roles);
        };
        $scope.uncheckAll = function () {
            $scope.group.Roles = [];
        };


        $scope.addAppGroup = addApplicationGroup;

        function addApplicationGroup() {
            apiService.post('/api/applicationGroup/add', $scope.group, addSuccessed, addFailed);
        }

        function addSuccessed() {
            notificationService.displaySuccess($scope.group.Name + ' đã được thêm mới.');

            $location.url('application_groups');
        }

        function addFailed(response) {
            notificationService.displayError(response.data.Message);
            notificationService.displayErrorValidation(response);
        }
    }
})(angular.module('fashionshop.application_groups'));