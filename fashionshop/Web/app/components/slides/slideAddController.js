/// <reference path="D:\CSharp\shop\Web\Assets/admin/libs/angular/angular.js" />

(function (app) {
    app.controller('slideAddController', slideAddController);

    slideAddController.$inject = ['apiService', '$scope', 'notificationService', '$state'];

    function slideAddController(apiService, $scope, notificationService, $state) {

        $scope.slide = {
            Status: true
        }
        $scope.AddSlide = AddSlide;
        function AddSlide() {
            apiService.post('api/slide/create', $scope.slide, function (result) {
                notificationService.displaySuccess(result.data.Name + ' đã được thêm mới.');
                $state.go('slides');
            }, function (error) {
                notificationService.displayError('Thêm mới không thành công.');
            });
        }

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
    }
})(angular.module('fashionshop.slides'));