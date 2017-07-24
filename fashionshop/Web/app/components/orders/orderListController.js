(function (app) {
    app.controller('orderListController', orderListController);

    orderListController.$inject = ['$scope', 'apiService', 'notificationService', '$ngBootbox','$filter'];

    function orderListController($scope, apiService, notificationService, $ngBootbox, $filter) {
        $scope.orders = [];
        $scope.page = 0;
        $scope.pagesCount = 0;
        $scope.pageSize = 2;
        $scope.keyword = '';
        $scope.pamentStatus = '';
        $scope.startDate = '';
        $scope.endDate = '';


        $scope.search = search;
        function search() {
            getOrders();
        }

        $scope.getOrders = getOrders;
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
            };
            apiService.get('/api/order/getlistorder', config, function (response) {
                if (response.data.TotalCount === 0) {
                    notificationService.displayWarning('Không có bản ghi nào được tìm thấy.');
                }
                if ($scope.keyword || $scope.startDate || $scope.endDate) {
                    notificationService.displayInfo(response.data.TotalCount + ' bản ghi được tìm thấy');
                }
                $scope.orders = response.data.Items;
                $scope.page = response.data.Page;
                $scope.pagesCount = response.data.TotalPages;
                $scope.totalCount = response.data.TotalCount;
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

        $scope.deleteOrder = deleteOrder;
        function deleteOrder(id) {
            $ngBootbox.confirm('Bạn có chắc chắn muống xóa không?').then(function () {
                var config = {
                    params: { id: id }
                };
                apiService.del('/api/order/delete', config, function () {
                    notificationService.displaySuccess('Xóa thành công');
                    $scope.getOrders();
                }, function (err) {
                    notificationService.displayError('Lỗi: ' + err.data.Message);
                });
            });
        }

        //Xóa nhiều bản ghi
        $scope.deleteMultiple = deleteMultiple;
        function deleteMultiple() {
            $ngBootbox.confirm('Bạn có chắc muốn xóa những bản ghi đã chọn?').then(function () {
                var listId = [];
                $.each($scope.selected, function (i, item) {
                    listId.push(item.ID);
                });
                var config = {
                    params: {
                        checkedOrders: JSON.stringify(listId)
                    }
                };
                apiService.del('/api/order/deletemulti', config, function (result) {
                    notificationService.displaySuccess('Đã xóa ' + result.data + ' bản ghi.');
                    search();
                }, function (error) {
                    notificationService.displayError('Lỗi: ' + error.data.Message);
                });
            });
        }

        $scope.selectAll = selectAll;
        $scope.isAll = true;
        function selectAll() {
            if ($scope.isAll === true) {
                angular.forEach($scope.orders, function (item) {
                    item.checked = true;
                });
                $scope.isAll = false;
            } else {
                angular.forEach($scope.orders, function (item) {
                    item.checked = false;
                });
                $scope.isAll = true;
            }
        }

        //phương thức lắng nghe khi ta check vào checkbox
        $scope.$watch("orders", function (n, o) {
            var checked = $filter("filter")(n, { checked: true });
            if (checked.length) {
                $scope.selected = checked;
                $('#btnDelete').removeAttr('disabled');
            } else {
                $('#btnDelete').attr('disabled', 'disabled');
            }
        }, true);

    }

})(angular.module('fashionshop.orders'));