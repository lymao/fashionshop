/// <reference path="/Assets/admin/libs/angular/angular.js" />
(function (app) {

    app.controller('applicationUserListController', applicationUserListController);

    applicationUserListController.$inject = ['$scope', 'apiService', 'notificationService', '$ngBootbox'];

    function applicationUserListController($scope, apiService, notificationService, $ngBootbox) {
        $scope.loading = true;
        $scope.data = [];
        $scope.page = 0;
        $scope.pageCount = 0;
        $scope.search = search;
        $scope.filter;

        function search(page) {
            page = page || 0;
            $scope.loading = true;
            var config = {
                params: {
                    page: page,
                    pageSize: 10
                }
            }

            apiService.get('api/applicationUser/getlistpaging', config, dataLoadCompleted, dataLoadFailed);
        }

        function dataLoadCompleted(result) {
            $scope.data = result.data.Items;
            $scope.page = result.data.Page;
            $scope.pagesCount = result.data.TotalPages;
            $scope.totalCount = result.data.TotalCount;
            $scope.loading = false;
        }
        function dataLoadFailed(response) {
            notificationService.displayError(response.data);
        }
        $scope.search();

        $scope.deleteItem = deleteItem;
        function deleteItem(id) {
            $ngBootbox.confirm('Bạn có chắc chắn muốn xóa không?').then(function () {
                var config = {
                    params: {
                        id: id
                    }
                }
                apiService.del('/api/applicationUser/delete', config, function () {
                    notificationService.displaySuccess('Đã xóa thành công.');
                    search();
                }, function () {
                    notificationService.displayError('Xóa không thành công.');
                });
            })

        }
    }
})(angular.module('fashionshop.application_users'));