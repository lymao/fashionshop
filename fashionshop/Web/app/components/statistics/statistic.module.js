/// <reference path="D:\CSharp\shop\Web\Assets/admin/libs/angular/angular.min.js" />
(function () {
    angular.module('fashionshop.statistics', ['fashionshop.common']).config(config);

    config.$inject = ['$stateProvider', '$urlRouterProvider'];

    function config($stateProvider, $urlRouterProvider) {
        $stateProvider.state('statistic_revenue', {
            url: '/statistic_revenue',
            templateUrl: '/app/components/statistics/revenueStatisticView.html',
            controller: 'revenueStatisticController',
            parent: 'base'
        });
    }
})();