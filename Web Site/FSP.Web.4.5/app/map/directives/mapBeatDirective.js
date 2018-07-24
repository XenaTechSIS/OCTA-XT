(function () {
   "use strict";
   angular.module("octaApp.map").directive("mapBeat", ["utilService", 'mapService', mapBeat]);

   function mapBeat(utilService, mapService) {
      return {
         restrict: 'E',
         templateUrl: $(".websiteUrl").text().trim() + '/app/map/directives/mapBeatTemplate.html',
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

            var selectedZoomFactor = 15;

            scope.isEditing = false;
            scope.isAdding = false;
            scope.isBusyGetting = false;
            scope.isBusySaving = false;
            scope.isBusyDeleting = false;

            scope.beats = [];
            scope.polygons = [];
            scope.markers = [];

            scope.selectedBeatID = "";
            scope.selectedBeat = "";
            scope.selectedPolygon = "";
            scope.selectedSegment = "";

            function buildDetailsContent(beat) {
               var content = "<table>";
               content += "<tr>";
               content += "<td>Beat Number:</td>";
               content += "<td><strong>" + beat.BeatNumber + "</strong></td>";
               content += "</tr>";
               content += "<tr>";
               content += "<td>Description:</td>";
               content += "<td><strong>" + beat.BeatDescription + "</strong></td>";
               content += "</tr>";
               content += "</table>";
               return content;
            }

            function buildPolygons(beat) {

               if (!beat) return;
               if (!beat.BeatSegments) return;


               beat.BeatSegments.forEach(function (beatSegment) {
                  if (beatSegment.PolygonData && beatSegment.PolygonData.Coordinates) {

                     var segmentLatLng = [];

                     beatSegment.PolygonData.Coordinates.forEach(function (coordinate) {
                        segmentLatLng.push({
                           lat: coordinate.lat,
                           lng: coordinate.lng
                        });
                     });

                     var beatSegmentPolygon = new google.maps.Polygon({
                        id: "beatSegmentPolygon_" + beat.BeatID,
                        paths: segmentLatLng,
                        strokeColor: beatSegment.Color || "#000000",
                        strokeOpacity: 0.8,
                        strokeWeight: 2,
                        fillColor: beatSegment.Color || "#000000",
                        fillOpacity: 0.35,
                        editable: false
                     });
                     scope.polygons.push(beatSegmentPolygon);

                  }
               });


            }

            function getBeatCenterCoordinate(beat) {

               if (!beat) return;
               if (!beat.BeatSegments) return;

               var numberOfSegments = beat.BeatSegments.length;

               if (numberOfSegments === 0) return;

               var middleSegmentIndex = 0;
               if (numberOfSegments > 2)
                  middleSegmentIndex = Math.ceil(numberOfSegments / 2);

               var segment = beat.BeatSegments[middleSegmentIndex];
               if (!segment.PolygonData) return;
               var segmentPolygon = segment.PolygonData;
               if (!segmentPolygon.Coordinates) return;
               if (segmentPolygon.Coordinates.length === 0) return;

               return segmentPolygon.Coordinates[0];
            }

            function buildMarkers(beat) {

               var centerCoord = getBeatCenterCoordinate(beat);
               if (!centerCoord) return;
               if (!centerCoord.lat && !centerCoord.lng) return;

               var beatMarker = new MarkerWithLabel({
                  id: "beatMarker" + beat.BeatID,
                  animation: google.maps.Animation.DROP,
                  position: new google.maps.LatLng(centerCoord.lat, centerCoord.lng),
                  draggable: false,
                  labelContent: beat.BeatNumber,
                  labelAnchor: new google.maps.Point(35, 40),
                  labelClass: "googleMapMarkerLabel", // the CSS class for the label
                  labelStyle: { opacity: 0.75 }
               });

               var infowindow = new google.maps.InfoWindow({
                  title: "Beat Details",
                  content: buildDetailsContent(beat)
               });
               beatMarker.addListener('click', function () {
                  infowindow.open(scope.map, beatMarker);
               });
               scope.markers.push(beatMarker);
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
               console.log("New Beat Map Location %s, %s", lat, lon);
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
                     if (scope.polygons.length === 0) {
                        scope.getBeatPolygons(true);
                     } else {
                        scope.triggerDisplayMapData();
                     }
                  } else {
                     scope.beats = [];
                     scope.polygons = [];
                     scope.markers = [];
                     scope.selectedBeatID = "";
                     scope.selectedBeat = "";
                     scope.selectedPolygon = "";
                  }
               }
            });

            scope.getSegments = function () {
               mapService.getSegments().then(function (segments) {
                  scope.allSegments = segments;
               });
            };

            scope.getBeatPolygons = function (triggerMapUpdate) {
               scope.isBusyGetting = true;
               mapService.getBeatPolygons().then(function (rawBeats) {
                  scope.isBusyGetting = false;
                  if (!rawBeats) {
                     toastr.error('Failed to retrieve beat polygons', 'Error');
                  } else {
                     scope.beats = rawBeats;
                     console.log('%i beats found %O', scope.beats.length, scope.beats);
                     scope.polygons = [];
                     scope.markers = [];
                     scope.beats.forEach(function (beat) {
                        buildPolygons(beat);
                        buildMarkers(beat);
                     });

                     if (scope.selectedBeatID)
                        scope.selectedBeat = utilService.findArrayElement(scope.beats, "BeatID", scope.selectedBeatID);

                     if (triggerMapUpdate)
                        scope.triggerDisplayMapData();
                  }

               });
            };

            scope.setSelectedBeat = function () {
               var beat = utilService.findArrayElement(scope.beats, "BeatID", scope.selectedBeatID);
               if (!beat) {
                  scope.selectedBeat = "";
                  scope.triggerHideMapData();
                  scope.triggerResetMap();
                  return;
               }
               scope.selectedBeat = angular.copy(beat);
               console.log(scope.selectedBeat);

               var centerCoord = getBeatCenterCoordinate(scope.selectedBeat);
               if (!centerCoord) return;
               if (!centerCoord.lat && !centerCoord.lng) return;

               scope.triggerSetMapLocation(centerCoord.lat, centerCoord.lng, selectedZoomFactor);
            };

            scope.setEdit = function () {
               scope.isEditing = true;
               console.log("Edit beat %s", scope.selectedBeatID);
               scope.triggerSetEditPolygon("beatSegmentPolygon_" + scope.selectedBeatID);
            };

            scope.addSegment = function () {
               if (!scope.selectedBeat) return;
               if (!scope.selectedSegment) return;

               var segment = utilService.findArrayElement(scope.selectedBeat.BeatSegments, "BeatSegmentID", scope.selectedSegment.BeatSegmentID);
               if (segment) return;

               scope.selectedSegment.BeatID = scope.selectedBeat.BeatID;
               scope.selectedBeat.BeatSegments.push(angular.copy(scope.selectedSegment));
               scope.selectedSegment = "";
            };

            scope.cancelEdit = function () {
               scope.isEditing = false;
               console.log("Cancel edit beat %s", scope.selectedBeatID);

               var beat = utilService.findArrayElement(scope.beats, "BeatID", scope.selectedBeatID);
               scope.selectedBeat = angular.copy(beat);

               scope.triggerSetCancelEditPolygon("beatSegmentPolygon_" + scope.selectedBeat.BeatID, scope.selectedBeat.BeatColor);

               var centerCoord = getBeatCenterCoordinate(scope.selectedBeat);
               if (!centerCoord) return;
               if (!centerCoord.lat && !centerCoord.lng) return;
               scope.triggerSetMapLocation(centerCoord.lat, centerCoord.lng, selectedZoomFactor);

            };

            scope.save = function () {
               console.log("Saving beat...");
               scope.isBusySaving = true;
               mapService.saveBeat(scope.selectedBeat).then(function (result) {
                  scope.isBusySaving = false;
                  if (result === false || result === "false") {
                     console.error("Save Beat");
                     toastr.error('Failed to save Beat', 'Error');
                  } else {
                     console.log("Save Beat Success");
                     toastr.success('Beat Saved', 'Success');
                     scope.isEditing = false;
                     scope.triggerSetCancelEditPolygon("beatPolygon" + scope.selectedBeat.BeatID, scope.selectedBeat.BeatColor);
                     scope.triggerMakeAllPolygonsUneditable();

                     // setTimeout(function () {
                     //    scope.getBeatPolygons(false);
                     // }, 500);

                     scope.triggerHideMapData();
                     setTimeout(function () {
                        scope.getBeatPolygons(true);
                     }, 250);
                  }
               });
            };

            scope.delete = function () {
               if (confirm("Ok to delete this Beat?")) {
                  scope.isBusyDeleting = true;
                  mapService.deleteBeat(scope.selectedBeat.BeatID).then(function (result) {
                     scope.isBusyDeleting = false;
                     scope.isEditing = false;
                     if (result === false || result === "false") {
                        console.error("Delete Beat");
                        toastr.error('Failed to delete Beat', 'Error');
                     } else {
                        console.log("Delete Beat Success");
                        toastr.success('Beat Deleted', 'Success');

                        scope.selectedBeatID = "";
                        scope.selectedBeat = "";
                        scope.selectedPolygon = "";

                        scope.triggerMakeAllPolygonsUneditable();
                        scope.triggerHideMapData();

                        setTimeout(function () {
                           scope.getBeatPolygons(true);
                        }, 500);

                     }
                  });
               }
            };

            scope.prepareNew = function () {
               scope.selectedBeat = {
                  BeatID: "",
                  BeatNumber: "",
                  BeatDescription: "",
                  BeatSegments: [],
                  BeatColor: "#000000",
                  BeatExtent: ""
               };
               scope.isAdding = true;
               scope.triggerSetNewPolygon(scope.selectedBeat.BeatColor);
            };

            scope.cancelAdd = function () {
               scope.isAdding = false;

               scope.selectedBeatID = "";
               scope.selectedBeat = "";
               scope.selectedPolygon = "";

               scope.triggerHideMapData();
               scope.triggerDisplayMapData();
            };

            scope.add = function () {
               console.log("Adding Beat...");
               scope.isBusyAdding = true;
               mapService.saveBeat(scope.selectedBeat).then(function (result) {
                  scope.isBusyAdding = false;
                  scope.isAdding = false;
                  scope.selectedBeatID = "";
                  scope.selectedBeat = "";
                  scope.selectedPolygon = "";
                  if (result === false || result === "false") {
                     console.error("Add Beat");
                     toastr.error('Failed to add Beat', 'Error');
                  } else {
                     console.log("Add Beat Success");
                     toastr.success('Beat Added', 'Success');
                     scope.triggerMakeAllPolygonsUneditable();
                     scope.triggerHideMapData();
                     scope.getBeatPolygons(true);
                  }
               });
            };

            scope.getSegments();
         }
      };
   }
})();
