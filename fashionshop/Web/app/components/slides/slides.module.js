/// <reference path="D:\CSharp\shop\Web\Assets/admin/libs/angular/angular.min.js" />
(function () {
    angular.module('fashionshop.slides', ['fashionshop.common']).config(config);

    config.$inject = ['$stateProvider', '$urlRouterProvider'];

    function config($stateProvider, $urlRouterProvider) {
        $stateProvider.state('slides', {
            url: '/slides',
            templateUrl: '/app/components/slides/slideListView.html',
            controller: 'slideListController',
            parent:'base'
        }).state('slide_add', {
            url: '/slide_add',
            templateUrl: '/app/components/slides/slideAddView.html',
            controller: 'slideAddController',
            parent: 'base'
        }).state('slide_import', {
            url: "/slide_import",
            parent: 'base',
            templateUrl: "/app/components/slides/slideImportView.html",
            controller: "slideImportController"
        }).state('slide_edit', {
            url: '/slide_edit/:id',
            templateUrl: '/app/components/slides/slideEditView.html',
            controller: 'slideEditController',
            parent: 'base'
        });
    }
})();