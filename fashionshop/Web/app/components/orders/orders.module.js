(function () {
    angular.module('fashionshop.orders', ['fashionshop.common']).config(config);

    config.$inject = ['$stateProvider', '$urlRouterProvider'];

    function config($stateProvider, $urlRouterProvider) {
        $stateProvider.state('orders', {
            url: '/orders',
            templateUrl: '/app/components/orders/orderListView.html',
            controller: 'orderListController',
            parent: 'base'
        }).state('order_add', {
            url: '/order_add',
            templateUrl: '/app/components/orders/orderAddView.html',
            controller: 'orderAddController',
            parent: 'base'
        }).state('order_edid', {
            url: '/order_edid',
            templateUrl: '/app/components/orders/orderEditView.html',
            controller: 'orderEditController',
            parent: 'base'
        });
    }
})();