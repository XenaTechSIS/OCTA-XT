﻿(function () {
   "use strict";
   angular.module("octaApp.map").directive("mapBeat", ["utilService", 'mapService', '$filter', mapBeat]);

   function mapBeat(utilService, mapService, $filter) {
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
            visible: "=",
            canEdit: "="
         },
         link: function (scope) {

            var selectedZoomFactor = 13;

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

            scope.segmentPickerButtonTitle = "Edit Segment List";

            scope.beatCenterCoords = [];

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
                        strokeColor: beat.BeatColor || "#000000",
                        strokeOpacity: 0.8,
                        strokeWeight: 2,
                        fillColor: beat.BeatColor || "#000000",
                        fillOpacity: 0.35,
                        editable: false
                     });
                     scope.polygons.push(beatSegmentPolygon);

                  }
               });
            }

            function centerCoordExists(coords) {
               if (scope.beatCenterCoords.length === 0) return false;
               var item = $filter('filter')(scope.beatCenterCoords, { lat: coords.lat, lng: coords.lng }, true)[0];
               return item !== undefined;
            }

            function getBeatCenterCoordinate(beat) {

               if (!beat) return;
               if (!beat.BeatSegments) return;

               var numberOfSegments = beat.BeatSegments.length;

               if (numberOfSegments === 0) return;

               var beatCenter = {
                  lat: "",
                  lng: ""
               };

               var segmentIndex = 0;
               var coordIndex = 0;
               if (numberOfSegments > 2)
                  segmentIndex = Math.ceil(numberOfSegments / 2);

               do {
                  var segment = beat.BeatSegments[segmentIndex];
                  if (!segment.PolygonData) return beatCenter;
                  var segmentPolygon = segment.PolygonData;
                  if (!segmentPolygon.Coordinates) return beatCenter;
                  if (segmentPolygon.Coordinates.length === 0) return beatCenter;

                  beatCenter = segmentPolygon.Coordinates[coordIndex];
                  segmentIndex++;
                  coordIndex++;
                  if (segmentIndex > numberOfSegments - 1)
                     segmentIndex = 0;
                  if (coordIndex > segmentPolygon.Coordinates.length - 1)
                     coordIndex = 0;
               }
               while (centerCoordExists(beatCenter));

               scope.beatCenterCoords.push(beatCenter);
               return beatCenter;
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

            function setBeatMapLocation(beat) {
               var centerCoord = getBeatCenterCoordinate(beat);
               if (!centerCoord) return;
               if (!centerCoord.lat && !centerCoord.lng) return;

               scope.triggerSetMapLocation(centerCoord.lat, centerCoord.lng, selectedZoomFactor);
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
               if (isVisible === undefined) return;
               console.log('isVisible: %s', isVisible);
               if (isVisible === true) {
                  if (scope.polygons.length === 0) {
                     scope.getBeatPolygons(true);
                  } else {
                     scope.triggerDisplayMapData();
                  }
               } else {
                  scope.isAdding = false;
                  scope.isEditing = false;
                  scope.selectedBeatID = "";
                  scope.selectedBeat = "";
                  scope.selectedPolygon = "";
                  scope.polygons = [];
               }

            });

            scope.prepareToAddSegments = function () {
               scope.selectedBeat.selectedSegmentIds = [];
               scope.selectedBeat.BeatSegments.forEach(function (segment) {
                  scope.selectedBeat.selectedSegmentIds.push(segment.BeatSegmentID);
               });
               $("#segmentPickerModal").modal("show");
            };

            scope.saveSelectedSegments = function () {
               scope.selectedBeat.BeatSegments = [];
               scope.selectedBeat.selectedSegmentIds.forEach(function (segmentId) {
                  var segment = utilService.findArrayElement(scope.allSegments, "BeatSegmentID", segmentId);
                  if (segment) {
                     scope.selectedBeat.BeatSegments.push(segment);
                  }
               });
               $("#segmentPickerModal").modal("hide");
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
                     scope.beatCenterCoords = [];

                     if (scope.selectedBeatID) {
                        scope.selectedBeat = utilService.findArrayElement(scope.beats, "BeatID", scope.selectedBeatID);
                        buildPolygons(scope.selectedBeat);

                        buildMarkers(scope.selectedBeat);
                        setBeatMapLocation(scope.selectedBeat);
                     }
                     else {
                        scope.beats.forEach(function (beat) {
                           buildPolygons(beat);
                           buildMarkers(beat);
                        });
                     }

                     if (triggerMapUpdate)
                        scope.triggerDisplayMapData();
                  }

               });
            };

            scope.setSelectedBeat = function () {
               scope.beatCenterCoords = [];
               var beat = utilService.findArrayElement(scope.beats, "BeatID", scope.selectedBeatID);
               scope.triggerHideMapData();
               scope.polygons = [];
               scope.markers = [];
               if (!beat) {
                  scope.selectedBeat = "";
                  scope.beats.forEach(function (beat) {
                     buildPolygons(beat);
                     buildMarkers(beat);
                  });
                  var self = scope;
                  setTimeout(function () {
                     self.triggerDisplayMapData();
                     self.triggerResetMap();
                  }, 200);
                  return;
               }
               scope.selectedBeat = angular.copy(beat);
               scope.selectedBeat.selectedSegmentIds = [];
               scope.selectedBeat.BeatSegments.forEach(function (segment) {
                  scope.selectedBeat.selectedSegmentIds.push(segment.BeatSegmentID);
               });
               //08-09-2018
               //When seleting a beat, only show that beat and its segments                         
               buildPolygons(scope.selectedBeat);
               buildMarkers(scope.selectedBeat);
               scope.triggerDisplayMapData();
               setBeatMapLocation(scope.selectedBeat);
            };

            scope.setEdit = function () {
               scope.isEditing = true;
               scope.segmentPickerButtonTitle = "Edit Segment List";
               console.log("Edit beat %s", scope.selectedBeatID);
            };

            scope.cancelEdit = function () {
               scope.beatCenterCoords = [];
               scope.isEditing = false;
               var beat = utilService.findArrayElement(scope.beats, "BeatID", scope.selectedBeatID);
               scope.selectedBeat = angular.copy(beat);
               scope.selectedBeat.selectedSegmentIds = [];
               setBeatMapLocation(scope.selectedBeat);
            };

            scope.save = function () {
               console.log("Saving beat...");
               scope.isBusySaving = true;
               mapService.saveBeat(scope.selectedBeat).then(function (response) {
                  scope.isBusySaving = false;
                  scope.isEditing = false;                                                          
                  console.log('Save Beat result: %O', response);

                  if (response === false || response === "false") {
                     console.error("Save Beat");
                     toastr.error('Failed to save Beat', 'Error');
                  }
                  else if (response.result === false || response.result === "false") {
                     console.error("Save Beat");
                     toastr.error('Failed to save Beat', 'Error');
                  } else {
                     console.log("Save Beat Success");
                     toastr.success('Beat Saved', 'Success');                     
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
                  BeatExtent: "",
                  selectedSegmentIds: []
               };
               scope.isAdding = true;
               scope.segmentPickerButtonTitle = "Add Segments";               
            };

            scope.add = function () {
               console.log("Adding Beat...");
               scope.isBusyAdding = true;
               mapService.saveBeat(scope.selectedBeat).then(function (response) {
                  scope.isBusyAdding = false;
                  scope.isAdding = false;
                  console.log('Add Beat result: %O', response);                 
                  
                  if (response === false || response === "false") {
                     console.error("Add Beat");
                     toastr.error('Failed to add Beat', 'Error');
                  }
                  else if (response.result === false || response.result === "false") {
                     console.error("Add Beat");
                     toastr.error('Failed to add Beat', 'Error');
                  } else {
                     console.log("Add Beat Success");
                     scope.selectedBeatID = response.record.BeatID;
                     toastr.success('Beat Added', 'Success');
                     scope.selectedBeat.selectedSegmentIds = [];
                     scope.selectedBeatID = "";
                     scope.getBeatPolygons(true);
                  }
               });
            };

            scope.cancelAdd = function () {
               scope.isAdding = false;

               scope.selectedBeatID = "";
               scope.selectedBeat = "";
               scope.selectedPolygon = "";

               scope.triggerHideMapData();
               scope.triggerDisplayMapData();
            };

            scope.getSegments = function () {
               mapService.getSegments().then(function (segments) {
                  scope.allSegments = segments;
                  console.log('All segments %O', scope.allSegments);
               });
            };

            scope.getSegments();

         }
      };
   }
})();
