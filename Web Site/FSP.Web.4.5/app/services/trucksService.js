(function () {
    'use strict';
    angular.module("octaApp").factory("trucksService", ["$http", trucksService]);

    function trucksService($http) {
        return {
            getTrucksRefreshRate: function () {
                return $http({
                    method: 'GET',
                    url: $(".websiteUrl").text().trim() + '/Truck/GetTruckRefreshRate'
                }).
                    then(function (response) {
                        return response.data;
                    });
            },
            getTrucks: function () {
                return $http({
                    method: 'GET',
                    url: $(".websiteUrl").text().trim() + "/Truck/UpdateAllTrucks"
                }).
                    then(function (response) {
                        return response.data;
                    });
            },
            setSelectedTruck: function (truckId) {
                return $http({
                    method: 'GET',
                    url: $(".websiteUrl").text().trim() + "/Trucks/SetSelectedTruck?truckId=" + truckId
                }).
                    then(function (response) {
                        return response.data;
                    });
            },
            checkForAlarms: function () {
                return $http({
                        method: 'GET',
                        url: $(".websiteUrl").text().trim() + "/Truck/HaveAlarms"
                    }).
                    then(function (response) {
                        return response.data;
                    });
            }
        };
    }
}());