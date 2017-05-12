///<reference path="D:\CSharp\shop\Web\Assets/admin/libs/angular/angular.js" />
(function (app) {
    app.controller('productCategoryEditController',productCategoryEditController);
    productCategoryEditController.$inject = ['apiService', '$scope', 'notificationService', '$stateParams', '$state', 'commonService'];

    function productCategoryEditController(apiService, $scope, notificationService, $stateParams, $state, commonService) {
        $scope.productCategory = {
            CreatedDate: new Date(),
            Status: true
        }
        $scope.UpdateProductCategory = UpdateProductCategory;
        function UpdateProductCategory() {
            apiService.put('api/productcategory/update', $scope.productCategory,
                function (result) {
                    notificationService.displaySuccess(result.data.Name + ' đã được cập nhật.');
                    $state.go('product_categories');
                }, function (error) {
                    notificationService.displayError('Cập nhật không thành công.');
                });
        }

        function loadProductCategoryDetail() {
            apiService.get('api/productcategory/getbyid/' + $stateParams.id, null, function (result) {
                $scope.productCategory = result.data;
            }, function (error) {
                notificationService.displayError(error);
            });
        }
        loadProductCategoryDetail();

        $scope.flatFolders = [];
        function loadParentCategory() {
            apiService.get('api/productcategory/getallparents', null, function (result) {
                $scope.parentCategories = commonService.getTree(result.data, "ID", "ParentID");
                $scope.parentCategories.forEach(function (item) {
                    recur(item, 0, $scope.flatFolders);
                });
            }, function () {
                console.log('Cannot get list parent');
            });
        }
        loadParentCategory();

        function times(n, str) {
            var result = '';
            for (var i = 0; i < n; i++) {
                result += str;
            }
            return result;
        };
        function recur(item, level, arr) {
            arr.push({
                Name: times(level, '-') + ' ' + item.Name,
                ID: item.ID,
                Level: level,
                Indent: times(level, '-')
            });
            if (item.children) {
                item.children.forEach(function (item) {
                    recur(item, level + 1, arr);
                });
            }
        };

        $scope.GetSeoTitle = GetSeoTitle;
        function GetSeoTitle() {
            $scope.productCategory.Alias = commonService.getSeoTitle($scope.productCategory.Name);
        }

    }
})(angular.module('fashionshop.product_categories'));