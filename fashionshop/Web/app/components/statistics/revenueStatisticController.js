/// <reference path="D:\CSharp\fashionshop\fashionshop\Web\Assets/admin/libs/angular/angular.js" />

(function (app) {

    app.controller('revenueStatisticController', revenueStatisticController);

    revenueStatisticController.$inject = ['$scope', 'apiService', 'notificationService', '$filter'];

    function revenueStatisticController($scope, apiService, notificationService, $filter) {

        $scope.tableData = [];
        $scope.chartData = [];
        $scope.labels = [];
        $scope.series = ['Lợi nhuận', 'Doanh thu'];
        function getStatistic() {
            var config = {
                params: {
                    //mm/dd/yyyy
                    fromDate: '01/01/2015',
                    toDate: '01/01/2018'
                }
            }
            apiService.get('/api/statistic/getrevenue', config, function (res) {
                $scope.tableData = res.data;
                var lables = [];
                var chartData = [];
                var revenues = [];
                var benefits = [];
                $.each(res.data, function (i, item) {
                    lables.push($filter('date')(item.Date, 'dd/MM/yyyy'));
                    benefits.push(item.Benefit);
                    revenues.push(item.Revenues);
                })
                chartData.push(benefits);
                chartData.push(revenues);

                $scope.labels = lables;
                $scope.chartData = chartData;

            }, function () {
                notificationService.displayError('Không thể tải được dữ liệu.');
            });    
        }
        getStatistic();
    }

})(angular.module('fashionshop.statistics'));