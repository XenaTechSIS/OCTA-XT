(function () {
  'use strict';
  angular.module("octaApp").factory("incidentService", ["$http", incidentService]);

  function incidentService($http) {
    return {
      getIncidents: function () {
        return $http({
          method: 'GET',
          url: $(".websiteUrl").text().trim() + '/Incident/GetIncidents'
        }).
          then(function (response) {
            return response.data;
          });
      }     
    };
  }
}());