/// <reference path="D:\CSharp\shop\Web\Assets/admin/libs/angular/angular.js" />

(function (app) {
    app.controller('slideEditController', slideEditController);

    slideEditController.$inject = ['$scope', 'apiService', '$stateParams', 'notificationService', '$state'];

    function slideEditController($scope, apiService, $stateParams, notificationService, $state) {
        $scope.slide = [];
        function loadSlideDetail() {
            apiService.get('api/slide/getbyid/' + $stateParams.id, null, function (result) {
                $scope.slide = result.data;
            }, function (error) {
                notificationService.displayError(error.data);
            });
        }
        loadSlideDetail();

        // setup editor options
        $scope.editorOptions = {
            language: 'en',
            height:'200'
        };

        $scope.ChooseImage = function () {
            var finder = new CKFinder();
            finder.selectActionFunction = function (fileUrl) {
                $scope.$apply(function () {
                    $scope.slide.Image = fileUrl;
                });
            }
            finder.popup();
        }

        $scope.UpdateSlide = UpdateSlide;
        function UpdateSlide() {
            apiService.put('api/slide/update', $scope.slide,
                function (result) {
                    notificationService.displaySuccess(result.data.Name + ' đã được cập nhật.');
                    $state.go('slides');
                }, function (error) {
                    notificationService.displayError('Cập nhật không thành công.');
                });
        }
    }

})(angular.module('fashionshop.slides'));