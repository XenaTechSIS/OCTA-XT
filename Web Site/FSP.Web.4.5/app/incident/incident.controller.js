(function () {
  'use strict';
  angular.module("octaApp.incident").controller("incidentController", ['$scope', '$filter', '$interval', 'incidentService', incidentController]);
  function incidentController($scope, $filter, $interval, incidentService) {
    var refreshIntervalInMilliseconds = 300000;
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
        name: 'Time',
        show: true
      });
      $scope.columns.push({
        name: 'Location Description',
        show: true
      });
      $scope.columns.push({
        name: 'Direction',
        show: true
      });
      $scope.columns.push({
        name: 'Vehicle Type',
        show: true
      });
      $scope.columns.push({
        name: 'Incident Type',
        show: true
      });
      $scope.columns.push({
        name: 'Assist Type',
        show: true
      });
      $scope.columns.push({
        name: 'Vehicle Make',
        show: true
      });
      $scope.columns.push({
        name: 'Vehicle Color',
        show: true
      });
      $scope.columns.push({
        name: 'Vehicle License Plate',
        show: true
      });
      $scope.columns.push({
        name: 'Comments',
        show: true
      });

      $scope.columns.push({
        name: 'Dispatch #',
        show: false
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
      $scope.columns.push({
        name: 'Tip',
        show: false
      });
      $scope.columns.push({
        name: 'Tip Detail',
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
      console.log('%cGetting incidents', 'color:yellow');
      $scope.isBusyGettingIncidents = true;
      incidentService.getIncidents().then(function (rawIncidents) {
        $scope.isBusyGettingIncidents = false;
        $scope.incidents = rawIncidents;
        $scope.dispatchedIncidents = $filter('filter')($scope.incidents, { Assist: { Acked: false } });
        $scope.activeIncidents = $filter('filter')($scope.incidents, { Assist: { Acked: true, AssistComplete: false }, });
        $scope.todaysCompletedIncidents = $filter('filter')($scope.incidents, { Assist: { AssistComplete: true } });

        console.log('All Incidents %O', $scope.incidents);
        console.log('Dispatched Incidents %O', $scope.dispatchedIncidents);
        console.log('Active Incidents %O', $scope.activeIncidents);
        console.log("Today's Completed Incidents %O", $scope.todaysCompletedIncidents);
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