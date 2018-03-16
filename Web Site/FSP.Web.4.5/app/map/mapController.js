(function () {
    'use strict';
    angular.module("octaApp.map").controller("mapController", ['$scope', '$rootScope', mapController]);
    function mapController($scope, $rootScope) {
        $scope.header = "Hello World!" + $rootScope.applicationName;
    }
}());