/// <reference path="D:\CSharp\shop\Web\Assets/admin/libs/angular/angular.min.js" />
(function () {
    angular.module('fashionshop.product_categories', ['fashionshop.common']).config(config);

    config.$inject = ['$stateProvider', '$urlRouterProvider'];

    function config($stateProvider, $urlRouterProvider) {
        $stateProvider.state('product_categories', {
            url: '/product_categories',
            templateUrl: '/app/components/product_categories/productCategoryListView.html',
            controller: 'productCategoryListController',
            parent: 'base'
        }).state('add_product_category', {
            url: '/add_product_category',
            templateUrl: '/app/components/product_categories/productCategoryAddView.html',
            controller: 'productCategoryAddController',
            parent: 'base'
        }).state('edit_product_category', {
            url: '/edit_product_category/:id',
            templateUrl: '/app/components/product_categories/productCategoryEditView.html',
            controller: 'productCategoryEditController',
            parent: 'base'
        });
    }
})();