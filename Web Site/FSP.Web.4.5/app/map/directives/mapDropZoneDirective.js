(function () {
   "use strict";
   angular.module("octaApp.map").directive("mapDropZone", ["$rootScope", "utilService", 'mapService', mapDropZone]);

   function mapDropZone($rootScope, utilService, mapService) {
      return {
         restrict: 'E',
         templateUrl: $(".websiteUrl").text().trim() + '/app/map/directives/mapDropZoneTemplate.html',
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

            scope.dropZones = [];
            scope.polygons = [];
            scope.markers = [];

            scope.selectedDropZoneID = "";
            scope.selectedDropZone = "";
            scope.selectedPolygon = "";

            function buildDetailsContent(dropZone) {
               var content = "<table>";
               content += "<tr>";
               content += "<td>ID:</td>";
               content += "<td><strong>" + dropZone.DropZoneID + "</strong></td>";
               content += "</tr>";
               content += "<tr>";
               content += "<td>Number:</td>";
               content += "<td><strong>" + dropZone.DropZoneNumber + "</strong></td>";
               content += "</tr>";
               content += "<tr>";
               content += "<td>Description:</td>";
               content += "<td><strong>" + dropZone.DropZoneDescription + "</strong></td>";
               content += "</tr>";
               content += "</table>";
               return content;
            }

            function buildPolygons(dropZone) {

               if (!dropZone) return;
               if (!dropZone.PolygonData) return;
               if (!dropZone.PolygonData.Coordinates) return;

               var cleanLatLng = [];

               dropZone.PolygonData.Coordinates.forEach(function (coordinate) {
                  cleanLatLng.push({
                     lat: coordinate.lat,
                     lng: coordinate.lng
                  });
               });

               var dropZonePolygon = new google.maps.Polygon({
                  id: "dropZonePolygon" + dropZone.DropZoneID,
                  paths: cleanLatLng,
                  strokeColor: dropZone.Color || "#000000",
                  strokeOpacity: 0.8,
                  strokeWeight: 2,
                  fillColor: dropZone.Color || "#000000",
                  fillOpacity: 0.35,
                  editable: false
               });
               scope.polygons.push(dropZonePolygon);
            }

            function buildMarkers(dropZone) {

               if (!dropZone) return;
               if (!dropZone.PolygonData) return;

               var dropZoneMarker = new MarkerWithLabel({
                  id: "dropZoneMarker" + dropZone.DropZoneID,
                  animation: google.maps.Animation.DROP,
                  position: new google.maps.LatLng(dropZone.PolygonData.MiddleLat, dropZone.PolygonData.MiddleLon),
                  draggable: false,
                  labelContent: dropZone.DropZoneNumber,
                  labelAnchor: new google.maps.Point(35, 40),
                  labelClass: "googleMapMarkerLabel", // the CSS class for the label
                  labelStyle: { opacity: 0.75 }
               });

               var infowindow = new google.maps.InfoWindow({
                  title: "Drop Zone Details",
                  content: buildDetailsContent(dropZone)
               });
               dropZoneMarker.addListener('click', function () {
                  infowindow.open(scope.map, dropZoneMarker);
               });
               scope.markers.push(dropZoneMarker);
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
               console.log("New DropZone Map Location %s, %s", lat, lon);
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
                        scope.getDropZonePolygons(true);
                     } else {
                        scope.triggerDisplayMapData();
                     }
                  } else {
                     scope.dropZones = [];
                     scope.polygons = [];
                     scope.markers = [];
                     scope.selectedDropZoneID = "";
                     scope.selectedDropZone = "";
                     scope.selectedPolygon = "";
                  }
               }
            });

            scope.getDropZonePolygons = function (triggerMapUpdate) {
               scope.isBusyGetting = true;
               mapService.getDropZonePolygons().then(function (rawDropZones) {
                  scope.isBusyGetting = false;
                  if (!rawDropZones) {
                     toastr.error('Failed to retrieve Drop Zone polygons', 'Error');
                  } else {
                     scope.dropZones = rawDropZones;
                     console.log('%i dropzones found %O', scope.dropZones.length, scope.dropZones);
                     scope.polygons = [];
                     scope.markers = [];
                     scope.dropZones.forEach(function (dropZone) {
                        buildPolygons(dropZone);
                        buildMarkers(dropZone);
                     });

                     if (scope.selectedDropZoneID)
                        scope.selectedDropZone = utilService.findArrayElement(scope.dropZones, "DropZoneID", scope.selectedDropZoneID);

                     if (triggerMapUpdate)
                        scope.triggerDisplayMapData();
                  }

               });
            };

            scope.setSelectedDropZone = function () {
               console.log('Drop Zone ID %s', scope.selectedDropZoneID);
               var dz = utilService.findArrayElement(scope.dropZones, "DropZoneID", scope.selectedDropZoneID);
               if (!dz) {
                  scope.selectedDropZone = "";
                  //scope.triggerHideMapData();
                  scope.triggerResetMap();
                  return;
               }
               scope.selectedDropZone = angular.copy(dz);
               console.log(scope.selectedDropZone);
               if (!scope.selectedDropZone.PolygonData) return;
               if (!scope.selectedDropZone.PolygonData.MiddleLat || !scope.selectedDropZone.PolygonData.MiddleLon) return;
               scope.triggerSetMapLocation(scope.selectedDropZone.PolygonData.MiddleLat, scope.selectedDropZone.PolygonData.MiddleLon, selectedZoomFactor);
            };

            scope.setEdit = function () {
               scope.isEditing = true;
               console.log("Edit %O", scope.selectedDropZone);
               console.log("Drop Zones %O", scope.dropZones);
               scope.triggerSetEditPolygon("dropZonePolygon" + scope.selectedDropZone.DropZoneID);
            };

            scope.cancelEdit = function () {
               scope.isEditing = false;
               console.log("Drop Zones %O", scope.dropZones);
               var dz = utilService.findArrayElement(scope.dropZones, "DropZoneID", scope.selectedDropZoneID);
               scope.selectedDropZone = angular.copy(dz);
               console.log("Cancel edit %O", scope.selectedDropZone);
               scope.triggerSetCancelEditPolygon("dropZonePolygon" + scope.selectedDropZone.DropZoneID, scope.selectedDropZone.Color);
               scope.triggerSetMapLocation(scope.selectedDropZone.PolygonData.MiddleLat, scope.selectedDropZone.PolygonData.MiddleLon, selectedZoomFactor);
            };

            scope.save = function () {
               console.log("Saving dropZone...");
               if (scope.selectedPolygon) {
                  var polygonCoords = utilService.getPolygonCoords(scope.selectedPolygon);
                  scope.selectedDropZone.Position = JSON.stringify(polygonCoords);
               }
               scope.isBusySaving = true;
               mapService.saveDropZone(scope.selectedDropZone).then(function (result) {
                  scope.isBusySaving = false;
                  if (result === false || result === "false") {
                     console.error("Save Drop Zone");
                     toastr.error('Failed to save Drop Zone', 'Error');
                  } else {
                     console.log("Save Drop Zone Success");
                     toastr.success('Drop Zone Saved', 'Success');
                     scope.isEditing = false;
                     scope.selectedDropZone = "";
                     scope.selectedPolygon = "";

                     // scope.triggerSetCancelEditPolygon("dropZonePolygon" + scope.selectedDropZone.DropZoneID, scope.selectedDropZone.Color);
                     // scope.triggerMakeAllPolygonsUneditable();

                     scope.triggerHideMapData();

                     setTimeout(function () {
                        scope.getDropZonePolygons(true);
                     }, 250);
                  }
               });
            };

            scope.delete = function () {
               if (confirm("Ok to delete this Drop Zone?")) {
                  scope.isBusyDeleting = true;
                  mapService.deleteDropZone(scope.selectedDropZone.DropZoneID).then(function (result) {
                     scope.isBusyDeleting = false;
                     if (result === false || result === "false") {
                        console.error("Delete DropZone");
                        toastr.error('Failed to delete Drop Zone', 'Error');
                     } else {
                        scope.isEditing = false;
                        console.log("Delete DropZone Success");
                        toastr.success('Drop Zone Deleted', 'Success');

                        scope.selectedDropZoneID = "";
                        scope.selectedDropZone = "";
                        scope.selectedPolygon = "";

                        scope.triggerMakeAllPolygonsUneditable();
                        scope.triggerHideMapData();
                        scope.triggerResetMap();

                        setTimeout(function () {
                           scope.getDropZonePolygons(true);
                        }, 500);
                     }
                  });
               }
            };

            scope.prepareNew = function () {
               scope.selectedDropZone = {
                  DropZoneID: "",
                  DropZoneDescription: "",
                  DropZoneNumber: "",
                  Comments: "",
                  Location: "",
                  Restrictions: "",
                  Capacity: 0,
                  City: "",
                  PDPhoneNumber: "",
                  Position: ""
               };
               scope.isAdding = true;
               scope.triggerSetNewPolygon(scope.selectedDropZone.Color);
            };

            scope.cancelAdd = function () {
               scope.isAdding = false;

               scope.selectedDropZoneID = "";
               scope.selectedDropZone = "";
               scope.selectedPolygon = "";

               scope.triggerHideMapData();
               scope.triggerDisplayMapData();
            };

            scope.add = function () {
               console.log("Adding DropZone...");
               if (scope.selectedPolygon) {
                  var polygonCoords = utilService.getPolygonCoords(scope.selectedPolygon);
                  scope.selectedDropZone.Position = JSON.stringify(polygonCoords);
               }
               scope.isBusyAdding = true;
               mapService.saveDropZone(scope.selectedDropZone).then(function (result) {
                  scope.isBusyAdding = false;
                  scope.isAdding = false;
                  if (result === false || result === "false") {
                     console.error("Add DropZone");
                     toastr.error('Failed to add Drop Zone', 'Error');
                     scope.prepareNew();
                  } else {
                     console.log("Add Drop Zone Success");
                     toastr.success('Drop Zone Added', 'Success');
                     scope.triggerMakeAllPolygonsUneditable();
                     scope.triggerHideMapData();
                     scope.selectedDropZone = "";
                     scope.getDropZonePolygons(true);
                  }
               });
            };

         }
      };
   }
})();
