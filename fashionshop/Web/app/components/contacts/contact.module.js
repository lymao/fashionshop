/// <reference path="D:\CSharp\shop\Web\Assets/admin/libs/angular/angular.min.js" />
(function () {
    angular.module('fashionshop.contacts', ['fashionshop.common']).config(config);

    config.$inject = ['$stateProvider', '$urlRouterProvider'];

    function config($stateProvider, $urlRouterProvider) {
        $stateProvider.state('contacts', {
            url: '/contacts',
            templateUrl: '/app/components/contacts/contactListView.html',
            controller: 'contactListController',
            parent:'base'
        }).state('contact_add', {
            url: '/contact_add',
            templateUrl: '/app/components/contacts/contactAddView.html',
            controller: 'contactAddController',
            parent: 'base'
        }).state('contact_edit', {
            url: '/contact_edit/:id',
            templateUrl: '/app/components/contacts/contactEditView.html',
            controller: 'contactEditController',
            parent: 'base'
        });
    }
})();