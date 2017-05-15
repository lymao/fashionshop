/// <reference path="D:\CSharp\shop\Web\Assets/admin/libs/angular/angular.js" />

(function (app) {
    app.controller('productAddController', productAddController);

    productAddController.$inject = ['apiService', '$scope', 'notificationService', '$state', 'commonService'];

    function productAddController(apiService, $scope, notificationService, $state, commonService) {

        $scope.product = {
            CreatedDate: new Date(),
            Status: true
        }
        $scope.AddProduct = AddProduct;
        function AddProduct() {
            $scope.product.MoreImages = JSON.stringify($scope.moreImages);
            apiService.post('api/product/create', $scope.product, function (result) {
                notificationService.displaySuccess(result.data.Name + ' đã được thêm mới.');
                $state.go('products');
            }, function (error) {
                notificationService.displayError('Thêm mới không thành công.');
            });
        }

        $scope.GetSeoTitle = GetSeoTitle;
        function GetSeoTitle() {
            $scope.product.Alias = commonService.getSeoTitle($scope.product.Name);
        }

        // setup editor options
        $scope.editorOptions = {
            language: 'en'
        };

        $scope.ChooseImage = function () {
            var finder = new CKFinder();
            finder.selectActionFunction = function (fileUrl) {
                $scope.$apply(function () {
                    $scope.product.Image = fileUrl;
                });
            }
            finder.popup();
        }

        $scope.ChooseImage1 = function () {
            var finder = new CKFinder();
            finder.selectActionFunction = function (fileUrl) {
                $scope.$apply(function () {
                    $scope.product.Image1 = fileUrl;
                });
            }
            finder.popup();
        }

        $scope.moreImages = [];
        $scope.ChooseMoreImage = function () {
            var finder = new CKFinder();
            finder.selectActionFunction = function (fileUrl) {
                var moreImg = $scope.moreImages.length;
                var i;
                for (i = 0; i < moreImg; i++) {
                    var file = $scope.moreImages[i];
                    var res = '';
                    if (file == fileUrl) {
                        res = 1; break;
                    }
                }
                if (res !== 1) {
                    $scope.$apply(function () {
                        $scope.moreImages.push(fileUrl);
                    });
                }
            }
            finder.popup();
        }

        $scope.remove = remove;
        function remove(index) {
            $scope.moreImages.splice(index, 1);
        }

        $scope.flatFolders = [];
        $scope.loadParentCategory = loadParentCategory;
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
        $scope.loadParentCategory();

        function times(n, str) {
            var result = '';
            for (var i = 0; i < n; i++) {
                result += str;
            }
            return result;
        };
        function recur(item, level, arr) {
            arr.push({
                Name: times(level, '\xa0 - \xa0') + ' ' + item.Name,// '\xa0' is '&nbsp;' in javascript
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
    }
})(angular.module('fashionshop.products'));