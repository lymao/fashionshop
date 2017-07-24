(function (app) {

    app.controller('orderDetailController', orderDetailController);

    orderDetailController.$inject = ['$scope', 'apiService', '$stateParams','notificationService'];

    function orderDetailController($scope, apiService, $stateParams, notificationService) {
        $scope.products = [];
        $scope.orderDetails = [];
        $scope.order = {};

        $scope.getOrder = getOrder;
        function getOrder() {
            apiService.get('api/order/getorder/' + $stateParams.id, null, function (result) {
                $scope.order = result.data;
                $scope.orderDetails = result.data.OrderDetails;
                $scope.loadData();
            }, function (err) {
                notificationService.displayError('Lỗi: ' + err.data.Message);
            });
        }
        $scope.getOrder();

        $scope.loadData = loadData;
        function loadData() {
            if ($scope.orderDetails.length !== 0) {
                $scope.showOrderDetailTable = false;
            } else {
                $scope.showOrderDetailTable = true;
            }
        }
        $scope.loadData();

        $scope.exportExcel = exportExcel;
        function exportExcel() {
            var config = {
                params: {
                    id: $stateParams.id
                }
            }
            apiService.get('/api/order/exportExcel/'+ $stateParams.id, null, function (response) {
                if (response.status = 200) {
                    window.location.href = response.data;
                }
            }, function (error) {
                notificationService.displayError(error.data.Message);

            });
        }
    }

})(angular.module('fashionshop.orders'));