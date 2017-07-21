(function (app) {
    app.controller('orderListController', orderListController);

    orderListController.$inject = ['$scope', 'apiService', 'notificationService'];

    function orderListController($scope, apiService, notificationService) {
        $scope.orders = [];
        $scope.page = 0;
        $scope.pageSize = 2;
        $scope.keyword = '';
        $scope.pamentStatus = '';
        $scope.startDate = '';
        $scope.endDate = '';
        $scope.getOrders = getOrders;

        $scope.search = search;
        function search() {
            getOrders();
        }

        function getOrders(page) {
            $scope.loading = true;
            page = page || 0;
            var config = {
                params: {
                    startDate: $scope.startDate,
                    endDate: $scope.endDate,
                    filter: $scope.keyword,
                    pamentStatus: $scope.pamentStatus,
                    page: page,
                    pageSize: 10
                }
            }
            apiService.get('/api/order/getlistorder', config, function (response) {
                if (response.data.TotalCount == 0) {
                    notificationService.displayWarning('Không có bản ghi nào được tìm thấy.');
                }
                else {
                    notificationService.displaySuccess('Đã tìm thấy ' + response.data.TotalCount + ' bản ghi.');
                }
                $scope.orders = response.data.Items;
                $scope.loading = false;
            }, function (error) {
                notificationService.displayError('Không lấy được danh sách đơn hàng');
            });
        }
        $scope.getOrders();

        $(function () {
            $(".datepicker").datepicker({
                dateFormat: "dd/mm/yy",
                changeMonth: true,
                changeYear: true
            });
        });
    }

})(angular.module('fashionshop.orders'));