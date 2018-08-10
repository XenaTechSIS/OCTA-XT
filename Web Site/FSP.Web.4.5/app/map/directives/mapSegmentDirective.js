(function () {
   "use strict";
   angular.module("octaApp.map").directive("mapSegment", ["$rootScope", "utilService", "generalService", 'mapService', mapSegment]);

   function mapSegment($rootScope, utilService, generalService, mapService) {
      return {
         restrict: 'E',
         templateUrl: $(".websiteUrl").text().trim() + '/app/map/directives/mapSegmentTemplate.html',
         scope: {
            resetMap: "&",
            setMapLocation: "&",
            displayMapData: "&",
            hideMapData: "&",
            setEditPolygon: "&",
            setCancelEditPolygon: "&",
            setNewPolygon: "&",
            makeAllPolygonsUneditable: "&",

            selectedPolygon: "=",
            visible: "="
         },
         link: function (scope) {

            var selectedZoomFactor = 16;

            scope.isEditing = false;
            scope.isAdding = false;
            scope.isBusyGetting = false;
            scope.isBusySaving = false;
            scope.isBusyDeleting = false;

            scope.segments = [];
            scope.polygons = [];
            scope.markers = [];
            scope.selectedBeatSegmentID = "";
            scope.selectedBeatSegment = "";

            function buildDetailsContent(segment) {
               var content = "<table>";
               content += "<tr>";
               content += "<td>Number:</td>";
               content += "<td><strong>" + segment.BeatSegmentNumber + "</strong></td>";
               content += "</tr>";
               content += "<tr>";
               content += "<td>Description:</td>";
               content += "<td><strong>" + segment.BeatSegmentDescription + "</strong></td>";
               content += "</tr>";
               content += "<tr>";
               content += "<td>CHP Description:</td>";
               content += "<td><strong>" + segment.CHPDescription + "</strong></td>";
               content += "</tr>";
               content += "<tr>";
               content += "<td>CHP Description 2:</td>";
               content += "<td><strong>" + segment.CHPDescription2 + "</strong></td>";
               content += "</tr>";
               content += "</table>";
               return content;
            }

            function buildPolygons(segment) {

               if (!segment) return;
               if (!segment.PolygonData) return;
               if (!segment.PolygonData.Coordinates) return;

               var cleanLatLng = [];

               segment.PolygonData.Coordinates.forEach(function (coordinate) {
                  cleanLatLng.push({
                     lat: coordinate.lat,
                     lng: coordinate.lng
                  });
               });

               var segmentPolygon = new google.maps.Polygon({
                  id: "segmentPolygon" + segment.BeatSegmentID,
                  paths: cleanLatLng,
                  strokeColor: segment.Color || "#000000",
                  strokeOpacity: 0.8,
                  strokeWeight: 2,
                  fillColor: segment.Color || "#000000",
                  fillOpacity: 0.35,
                  editable: false
               });
               scope.polygons.push(segmentPolygon);
            }

            function buildMarkers(segment) {

               if (!segment) return;
               if (!segment.PolygonData) return;

               var segmentMarker = new MarkerWithLabel({
                  id: "segmentMarker" + segment.BeatSegmentID,
                  animation: google.maps.Animation.DROP,
                  position: new google.maps.LatLng(segment.PolygonData.MiddleLat, segment.PolygonData.MiddleLon),
                  draggable: false,
                  labelContent: segment.BeatSegmentNumber,
                  labelAnchor: new google.maps.Point(35, 40),
                  labelClass: "googleMapMarkerLabel", // the CSS class for the label
                  labelStyle: { opacity: 0.75 }
               });

               var infowindow = new google.maps.InfoWindow({
                  title: "Segment Details",
                  content: buildDetailsContent(segment)
               });
               segmentMarker.addListener('click', function () {
                  infowindow.open(scope.map, segmentMarker);
               });
               scope.markers.push(segmentMarker);
            }

            function setSegmentMapLocation(segment) {
               if (!segment.PolygonData) return;
               if (!segment.PolygonData.MiddleLat || !segment.PolygonData.MiddleLon) return;
               scope.triggerSetMapLocation(segment.PolygonData.MiddleLat, segment.PolygonData.MiddleLon, selectedZoomFactor);
            }

            scope.triggerDisplayMapData = function () {
               scope.displayMapData({
                  polygons: scope.polygons,
                  markers: scope.markers
               });
            };

            scope.triggerHideMapData = function () {
               scope.hideMapData();
            };

            scope.triggerResetMap = function () {
               scope.resetMap();
            };

            scope.triggerMakeAllPolygonsUneditable = function () {
               scope.makeAllPolygonsUneditable();
            };

            scope.triggerSetMapLocation = function (lat, lon, zoom) {
               console.log("New Segment Map Location %s, %s", lat, lon);
               scope.setMapLocation({
                  lat: lat,
                  lon: lon,
                  zoom: zoom
               });
            };

            scope.triggerSetEditPolygon = function (id) {
               scope.setEditPolygon({
                  id: id
               });
            };

            scope.triggerSetCancelEditPolygon = function (id, color) {
               scope.setCancelEditPolygon({
                  id: id,
                  color: color
               });
            };

            scope.triggerSetNewPolygon = function (color) {
               scope.setNewPolygon({
                  color: color
               });
            };

            scope.$watch("visible", function (isVisible) {
               if (isVisible !== undefined) {
                  if (isVisible) {
                     if (scope.segments.length === 0) {
                        scope.getSegments(true);
                        scope.getBeats();
                     } else {
                        scope.triggerDisplayMapData();
                     }
                  } else {
                     scope.selectedBeatSegment = "";
                  }
               }
            });

            scope.getBeats = function () {
               generalService.getBeatNumbers().then(function (result) {
                  console.log("Beats %O", result);
                  scope.beats = result;
               });
            };

            scope.getSegments = function (triggerMapUpdate) {
               scope.isBusyGetting = true;
               mapService.getSegmentPolygons().then(function (rawSegments) {
                  scope.isBusyGetting = false;
                  if (!rawSegments) {
                     toastr.error('Failed to retrieve segments', 'Error');
                  } else {
                     scope.segments = rawSegments;
                     console.log('%i segment polygons found %O', scope.segments.length, scope.segments);

                     scope.polygons = [];
                     scope.markers = [];

                     if (scope.selectedBeatSegmentID) {
                        scope.selectedBeatSegment = utilService.findArrayElement(scope.segments, "BeatSegmentID", scope.selectedBeatSegmentID);
                        buildPolygons(scope.selectedBeatSegment);
                        buildMarkers(scope.selectedBeatSegment);
                        setSegmentMapLocation(scope.selectedBeatSegment);
                     }
                     else {
                        scope.segments.forEach(function (segment) {
                           buildPolygons(segment);
                           buildMarkers(segment);
                        });
                     }
                     if (triggerMapUpdate)
                        scope.triggerDisplayMapData();
                  }

               });
            };

            scope.setSelectedBeatSegment = function () {
               var seg = utilService.findArrayElement(scope.segments, "BeatSegmentID", scope.selectedBeatSegmentID);
               scope.triggerHideMapData();
               scope.polygons = [];
               scope.markers = [];

               if (!seg) {
                  scope.selectedBeatSegment = "";
                  scope.segments.forEach(function (segment) {
                     buildPolygons(segment);
                     buildMarkers(segment);
                  });
                  var self = scope;
                  setTimeout(function () {
                     self.triggerDisplayMapData();
                     self.triggerResetMap();
                  }, 200);
                  return;
               }

               scope.selectedBeatSegment = angular.copy(seg);
               buildPolygons(scope.selectedBeatSegment);
               buildMarkers(scope.selectedBeatSegment);
               scope.triggerDisplayMapData();
               setSegmentMapLocation(scope.selectedBeatSegment);
            };

            scope.setEdit = function () {
               scope.isEditing = true;
               console.log("Edit segment %s", scope.selectedBeatSegmentID);
               scope.triggerSetEditPolygon("segmentPolygon" + scope.selectedBeatSegmentID);
            };

            scope.cancelEdit = function () {
               scope.isEditing = false;
               console.log("Cancel edit segment %s", scope.selectedBeatSegmentID);
               var seg = utilService.findArrayElement(scope.segments, "BeatSegmentID", scope.selectedBeatSegmentID);
               scope.selectedBeatSegment = angular.copy(seg);
               console.log("Cancel edit %O", scope.selectedBeatSegment);
               scope.triggerSetCancelEditPolygon("segmentPolygon" + scope.selectedBeatSegment.BeatSegmentID, scope.selectedBeatSegment.Color);
               scope.triggerSetMapLocation(scope.selectedBeatSegment.PolygonData.MiddleLat, scope.selectedBeatSegment.PolygonData.MiddleLon, selectedZoomFactor);
            };

            scope.save = function () {
               console.log("Saving segment...");
               if (scope.selectedPolygon) {
                  var polygonCoords = utilService.getPolygonCoords(scope.selectedPolygon);
                  scope.selectedBeatSegment.BeatSegmentExtent = JSON.stringify(polygonCoords);
               }
               scope.isBusySaving = true;
               mapService.saveSegment(scope.selectedBeatSegment).then(function (response) {
                  scope.isBusySaving = false;
                  if (response.result === false || response.result === "false") {
                     console.error("Save Segment");
                     toastr.error('Failed to save Segment', 'Error');
                  } else {
                     console.log("Save Segment Success");
                     toastr.success('Segment Saved', 'Success');
                     scope.isEditing = false;

                     scope.triggerSetCancelEditPolygon("segmentPolygon" + scope.selectedBeatSegment.BeatSegmentID, scope.selectedBeatSegment.Color);
                     scope.triggerMakeAllPolygonsUneditable();
                     // setTimeout(function () {
                     //    scope.getSegments(false);
                     // }, 250);

                     scope.triggerHideMapData();
                     setTimeout(function () {
                        scope.getSegments(true);
                     }, 250);

                  }
               });
            };

            scope.delete = function () {
               if (confirm("Ok to delete this Segment?")) {
                  scope.isBusyDeleting = true;
                  mapService.deleteSegment(scope.selectedBeatSegment.BeatSegmentID).then(function (result) {
                     scope.isBusyDeleting = false;
                     scope.isEditing = false;
                     if (result === false || result === "false") {
                        console.error("Delete Segment");
                        toastr.error('Failed to delete Segment', 'Error');
                     } else {
                        console.log("Delete Segment Success");
                        toastr.success('Segment Deleted', 'Success');

                        scope.selectedBeatSegmentID = "";
                        scope.selectedBeatSegment = "";
                        scope.selectedPolygon = "";

                        scope.triggerMakeAllPolygonsUneditable();
                        scope.triggerHideMapData();
                        scope.triggerResetMap();

                        setTimeout(function () {
                           scope.getSegments(true);
                           scope.getBeats();
                        }, 500);

                     }
                  });
               }
            };

            scope.prepareNew = function () {
               scope.selectedBeatSegment = {
                  BeatSegmentID: "",
                  Color: "#000000",
                  BeatSegmentExtent: "",
                  CHPDescription: "",
                  CHPDescription2: ""
               };
               scope.isAdding = true;
               scope.triggerSetNewPolygon(scope.selectedBeatSegment.Color);
            };

            scope.cancelAdd = function () {
               scope.isAdding = false;

               scope.selectedBeatSegmentID = "";
               scope.selectedBeatSegment = "";
               scope.selectedPolygon = "";

               scope.triggerHideMapData();
               scope.triggerDisplayMapData();
            };

            scope.add = function () {
               console.log("Adding Segment...");
               if (scope.selectedPolygon) {
                  var polygonCoords = utilService.getPolygonCoords(scope.selectedPolygon);
                  scope.selectedBeatSegment.BeatSegmentExtent = JSON.stringify(polygonCoords);
               }
               scope.isBusyAdding = true;
               mapService.saveSegment(scope.selectedBeatSegment).then(function (response) {
                  scope.isBusyAdding = false;
                  scope.isAdding = false;
                  scope.selectedBeatSegmentID = response.record.BeatSegmentID;
                  if (response.result === false || response.result === "false") {
                     console.error("Add Segment");
                     toastr.error('Failed to add Segment', 'Error');
                  } else {
                     console.log("Add Segment Success");
                     toastr.success('Segment Added', 'Success');
                     scope.triggerMakeAllPolygonsUneditable();
                     scope.triggerHideMapData();
                     scope.selectedBeatSegment = "";
                     scope.getSegments(true);
                  }
               });
            };
         }
      };
   }
})();
