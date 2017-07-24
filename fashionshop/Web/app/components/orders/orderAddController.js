(function (app) {

    app.controller('orderAddController', orderAddController);

    orderAddController.$inject = ['$scope', 'apiService', 'notificationService', '$state'];

    function orderAddController($scope, apiService, notificationService, $state) {
        $scope.products = [];
        $scope.entityOrderDetail = {};
        $scope.orderDetails = [];
        $scope.order = { Status: true };

        $scope.loadData = loadData;
        function loadData() {
            if ($scope.orderDetails.length !== 0) {
                $scope.showOrderDetailTable = false;
            } else {
                $scope.showOrderDetailTable = true;
            }
        }
        $scope.loadData();

        $scope.getAllProduct = getAllProduct;
        function getAllProduct() {
            apiService.get('api/order/getallproduct', null, function (result) {
                $scope.products = result.data;
            }, function (err) {
                notificationService.displayError(err.data.Message);
            });
        }
        $scope.getAllProduct();

        $scope.saveOrderDetail = saveOrderDetail;
        function saveOrderDetail() {
            $scope.entityOrderDetail.Product = $scope.products.find(x => x.ID === $scope.entityOrderDetail.ProductId);
            angular.forEach($scope.products, function (value) {
                angular.forEach(value.ProductSizes, function (item) {
                    if (item.Size.ID === $scope.entityOrderDetail.SizeId) {
                        $scope.entityOrderDetail.Size = item.Size;
                    }
                });
            });
            $scope.orderDetails.push($scope.entityOrderDetail);
            $scope.entityOrderDetail = {};
            $scope.loadData();
        }

        $scope.ChangeProduct = ChangeProduct;
        function ChangeProduct() {
            var config = {
                params: {
                    productId: $scope.entityOrderDetail.ProductId
                }
            }
            apiService.get('api/order/geproductbyid', config, function (result) {
                $scope.entityOrderDetail.Price = result.data.Price;
                $scope.sizes = [];
                angular.forEach(result.data.ProductSizes, function (item) {
                    $scope.sizes.push(item.Size);
                });
            }, function (err) {
                notificationService.displayError(err.data.Message);
            });
        }

        $scope.deleteDetail = deleteDetail;
        function deleteDetail(index) {
            $scope.orderDetails.splice(index, 1);
            $scope.loadData();
        }

        $scope.AddOrder = AddOrder;      
        function AddOrder() {
            $scope.order.OrderDetails = $scope.orderDetails;
            apiService.post('api/order/createorder', JSON.stringify($scope.order), function (result) {
                $state.go('orders');
                notificationService.displaySuccess("Thêm đơn hàng thành công.");
            }, function (err) {
                notificationService.displayError(err.data.Message);
            });
        }
    }

})(angular.module('fashionshop.orders'));