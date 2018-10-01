(function () {
   "use strict";
   angular.module("octaApp.incident").directive("incidentList", incidentList);

   function incidentList() {
      return {
         restrict: 'E',
         templateUrl: $(".websiteUrl").text().trim() + '/app/incident/directives/incident.list.template.html',
         scope: {
            incidents: "=",
            title: "=",
            columns: "="
         },
         link: function (scope) {

            scope.sortColumn = "BeatNumber";
            scope.predicate = false;

            scope.updateSortColumn = function (sortColumn) {
               scope.sortColumn = sortColumn;
               scope.predicate = !scope.predicate;
            };
         }
      };
   }
})();
