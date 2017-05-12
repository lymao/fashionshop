/// <reference path="D:\CSharp\shop\Web\Assets/admin/libs/angular/angular.js" />

(function (app) {
    app.controller('productListController', productListController);

    productListController.$inject = ['$scope', 'apiService', 'notificationService', '$ngBootbox', '$filter'];

    function productListController($scope, apiService, notificationService, $ngBootbox, $filter) {
        $scope.loading = true;
        $scope.products = [];
        $scope.page = 0;
        $scope.pagesCount = 0;
        $scope.keyword = '';
        $scope.getProducts = getProducts;

        $scope.search = search;
        function search() {
            getProducts();
        }

        $scope.exportExcel = exportExcel;
        function exportExcel() {
            var config = {
                params: {
                    filter: $scope.keyword
                }
            }
            apiService.get('/api/product/ExportXls', config, function (response) {
                if (response.status = 200) {
                    window.location.href = response.data.Message;
                }
            }, function (error) {
                notificationService.displayError(error);

            });
        }

        $scope.exportPdf = exportPdf;
        function exportPdf(productId) {
            var config = {
                params: {
                    id: productId
                }
            }
            apiService.get('/api/product/ExportPdf', config, function (response) {
                if (response.status = 200) {
                    window.location.href = response.data.Message;
                }
            }, function (error) {
                notificationService.displayError(error);

            });
        }

        function getProducts(page) {
            $scope.loading = true;
            page = page || 0;
            var config = {
                params: {
                    keyword: $scope.keyword,
                    page: page,
                    pageSize: 20
                }
            }
            apiService.get('/api/product/getall', config, function (result) {
                if (result.data.TotalCount == 0) {
                    notificationService.displayWarning('Không có bản ghi nào được tìm thấy.');
                }
                else {
                    notificationService.displaySuccess('Đã tìm thấy ' + result.data.TotalCount + ' bản ghi.');
                }
                $scope.products = result.data.Items;
                $scope.page = result.data.Page;
                $scope.pagesCount = result.data.TotalPages;
                $scope.totalCount = result.data.TotalCount;
                $scope.loading = false;
            }, function () {
                console.log('Load product failed.');
            });
        }
        $scope.getProducts();

        $scope.deleteProduct = deleteProduct;
        function deleteProduct(id) {
            $ngBootbox.confirm('Bạn có chắc muốn xóa?').then(function () {
                var config = {
                    params: {
                        id: id
                    }
                }
                apiService.del('api/product/delete', config, function () {
                    notificationService.displaySuccess('Xóa thành công');
                    search();
                }, function () {
                    notificationService.displayError('Xóa không thành công');
                })
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
                        checkedProduct: JSON.stringify(listId)
                    }
                }
                apiService.del('api/product/deletemulti', config, function (result) {
                    notificationService.displaySuccess('Đã xóa ' + result.data + ' bản ghi.');
                    search();
                }, function (error) {
                    notificationService.displayError('Xóa không thành công');
                });
            });
        }

        // hàm logic: đầu tiên isAll= true, khi click vào hàm selectAll ở View kiểm tra đk nếu isAll=true thì lăp qua products và gán cho ng-model=item.checked
        // ở ngoài View là true, ngược lại isAll=false
        $scope.selectAll = selectAll;
        $scope.isAll = true;
        function selectAll() {
            if ($scope.isAll === true) {
                angular.forEach($scope.products, function (item) {
                    item.checked = true;
                });
                $scope.isAll = false;
            } else {
                angular.forEach($scope.products, function (item) {
                    item.checked = false;
                });
                $scope.isAll = true;
            }
        }

        //phương thức lắng nghe khi ta check vào checkbox
        $scope.$watch("products", function (n, o) {
            var checked = $filter("filter")(n, { checked: true });
            if (checked.length) {
                $scope.selected = checked;
                $('#btnDelete').removeAttr('disabled');
            } else {
                $('#btnDelete').attr('disabled', 'disabled');
            }
        }, true);

    }
})(angular.module('fashionshop.products'));