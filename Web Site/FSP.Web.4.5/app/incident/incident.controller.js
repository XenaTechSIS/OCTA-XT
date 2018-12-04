(function () {
   'use strict';
   angular.module("octaApp.incident").controller("incidentController", ['$scope', '$rootScope', '$filter', '$interval', 'incidentService', incidentController]);
   function incidentController($scope, $rootScope, $filter, $interval, incidentService) {
      var refreshIntervalInMilliseconds = 30000;
      $scope.header = "Incidents";
      $scope.showDispatched = false;
      $scope.showActive = false;
      $scope.showTodays = true;

      $scope.incidents = [];
      $scope.dispatchedIncidentsTitle = "Dispatched & Acked";
      $scope.activeIncidentsTitle = "Active";
      $scope.todaysCompletedTitle = "Today's Completed";
      $scope.dispatchedIncidents = [];
      $scope.activeIncidents = [];
      $scope.todaysCompletedIncidents = [];

      $scope.isBusyGettingIncidents = false;

      $scope.allColumnsChecked = false;
      $scope.columns = [];

      $scope.initColumns = function () {
         $scope.columns = [];
         $scope.columns.push({
            name: 'Beat #',
            show: true
         });
         $scope.columns.push({
            name: 'Truck #',
            show: true
         });
         $scope.columns.push({
            name: 'Driver',
            show: true
         });
         $scope.columns.push({
            name: 'Dispatch Summary Message',
            show: true
         });
         $scope.columns.push({
            name: 'Time',
            show: true
         });
         $scope.columns.push({
            name: 'Dispatch #',
            show: true
         });
         $scope.columns.push({
            name: 'State',
            show: false
         });
         $scope.columns.push({
            name: 'Contractor',
            show: false
         });
         $scope.columns.push({
            name: 'Assist #',
            show: true
         });

         $scope.columns.push({
            name: 'Vehicle Color',
            show: false
         });
         $scope.columns.push({
            name: 'Vehicle License Plate',
            show: false
         });
         $scope.columns.push({
            name: 'Vehicle Make',
            show: false
         });
         $scope.columns.push({
            name: 'Drop Zone',
            show: false
         });
         $scope.columns.push({
            name: 'Dispatch Time',
            show: false
         });
         $scope.columns.push({
            name: 'State',
            show: false
         });
         $scope.columns.push({
            name: 'Customer Last Name',
            show: false
         });
         $scope.columns.push({
            name: 'Customer Wait Time',
            show: false
         });
      };

      $scope.showColumnConfig = function () {
         $("#columnConfigModal").modal("show");
      };

      $scope.toggleAllColumns = function () {
         $scope.allColumnsChecked = !$scope.allColumnsChecked;
         $scope.columns.forEach(function (col) {
            col.show = $scope.allColumnsChecked;
         });
      };

      $scope.submitColumnConfig = function () {
         $("#columnConfigModal").modal("hide");
      };

      $scope.getIncidents = function () {
         console.log('%cGetting incidents', 'color:red');
         $scope.isBusyGettingIncidents = true;
         incidentService.getIncidents().then(function (rawIncidents) {
            $scope.isBusyGettingIncidents = false;
            console.log(rawIncidents);
            $scope.incidents = rawIncidents;
            $scope.dispatchedIncidents = $filter('filter')($scope.incidents, { IsAcked: false });
            $scope.activeIncidents = $filter('filter')($scope.incidents, { IsAcked: true, IsIncidentComplete: false });
            $scope.todaysCompletedIncidents = $filter('filter')($scope.incidents, { IsIncidentComplete: true });
         });
      };

      $scope.getIncidents();
      $scope.initColumns();

      var self = $scope;
      var refreshInterval = $interval(function () {
         self.getIncidents();
      }, refreshIntervalInMilliseconds);

      $scope.$on('$destroy', function () {
         $interval.cancel(refreshInterval);
         refreshInterval = undefined;
      });

   }
}());