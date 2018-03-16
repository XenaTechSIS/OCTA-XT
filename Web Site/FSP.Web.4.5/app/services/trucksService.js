﻿(function () {
    'use strict';
    angular.module("octaApp").factory("trucksService", ["$http", trucksService]);

    function trucksService($http) {
        return {
            getTrucksRefreshRate: function () {
                return $http({
                    method: 'GET',
                    url: $(".websiteUrl").text().trim() + '/Trucks/GetTruckRefreshRate'
                }).
                    then(function (response) {
                        return response.data;
                    });
            },
            getTrucks: function () {
                return $http({
                    method: 'GET',
                    url: $(".websiteUrl").text().trim() + "/Trucks/GetAll"
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
            }
        };
    }
}());