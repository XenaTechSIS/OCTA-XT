(function () {
   "use strict";
   angular.module("octaApp.map").directive("segmentPicker", segmentPicker);

   function segmentPicker() {
      return {
         restrict: 'E',
         templateUrl: $(".websiteUrl").text().trim() + '/app/map/directives/beatSegmentPickerTemplate.html',
         scope: {
            allSegments: "=",
            selectedBeat: "="
         },
         link: function (scope) {

            //scope.condition = true;
            // scope.localAllSegments = [];

            // scope.$watchCollection("selectedSegmentIds", function (newValues, oldValues) {
            //    if (newValues) {
            //       console.log('Selected segments %O', newValues);
            //       scope.localAllSegments = scope.allSegments;
            //    }
            // });

            // scope.$watchCollection("allSegments", function (newValues, oldValues) {
            //    if (newValues) {
            //       scope.localAllSegments = scope.allSegments;
            //    }
            // });

         }
      };
   }
})();
