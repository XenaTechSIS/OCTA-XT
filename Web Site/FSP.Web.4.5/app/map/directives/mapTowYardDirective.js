(function () {
   "use strict";
   angular.module("octaApp.map").directive("mapTowYard", ["utilService", 'mapService', mapTowYard]);

   function mapTowYard(utilService, mapService) {
      return {
         restrict: 'E',
         templateUrl: $(".websiteUrl").text().trim() + '/app/map/directives/mapTowYardTemplate.html',
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

            scope.isEditing = false;
            scope.isAdding = false;
            scope.isBusyGetting = false;
            scope.isBusySaving = false;
            scope.isBusyDeleting = false;

            scope.yards = [];
            scope.polygons = [];
            scope.markers = [];

            scope.selectedYardID = "";
            scope.selectedYard = "";
            scope.selectedPolygon = "";

            function buildDetailsContent(yard) {
               var content = "<table>";
               content += "<tr>";
               content += "<td>Location:</td>";
               content += "<td><strong>" + yard.Location + "</strong></td>";
               content += "</tr>";
               content += "<tr>";
               content += "<td>Description:</td>";
               content += "<td><strong>" + yard.YardDescription + "</strong></td>";
               content += "</tr>";
               content += "<tr>";
               content += "<td>Comments:</td>";
               content += "<td><strong>" + yard.Comments + "</strong></td>";
               content += "</tr>";
               content += "</table>";
               return content;
            }

            function buildPolygons(yard) {

               if (!yard) return;
               if (!yard.PolygonData) return;
               if (!yard.PolygonData.Coordinates) return;

               var cleanLatLng = [];

               yard.PolygonData.Coordinates.forEach(function (coordinate) {
                  cleanLatLng.push({
                     lat: coordinate.lat,
                     lng: coordinate.lng
                  });
               });

               var yardPolygon = new google.maps.Polygon({
                  id: "yardPolygon" + yard.YardID,
                  paths: cleanLatLng,
                  strokeColor: "#000000",
                  strokeOpacity: 0.8,
                  strokeWeight: 2,
                  fillColor: "#000000",
                  fillOpacity: 0.35,
                  editable: false
               });
               scope.polygons.push(yardPolygon);
            }

            function buildMarkers(yard) {

               if (!yard) return;
               if (!yard.PolygonData) return;

               var yardMarker = new MarkerWithLabel({
                  id: "yardMarker" + yard.YardID,
                  animation: google.maps.Animation.DROP,
                  position: new google.maps.LatLng(yard.PolygonData.MiddleLat, yard.PolygonData.MiddleLon),
                  draggable: false,
                  labelContent: yard.Location,
                  labelAnchor: new google.maps.Point(35, 40),
                  labelClass: "googleMapMarkerLabel", // the CSS class for the label
                  labelStyle: { opacity: 0.75 }
               });

               var infowindow = new google.maps.InfoWindow({
                  title: "Yard Details",
                  content: buildDetailsContent(yard)
               });
               yardMarker.addListener('click', function () {
                  infowindow.open(scope.map, yardMarker);
               });
               scope.markers.push(yardMarker);
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
               console.log("New Yard Map Location %s, %s", lat, lon);
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
                        scope.getYardPolygons(true);
                     } else {
                        scope.triggerDisplayMapData();
                     }
                  } else {
                     scope.yards = [];
                     scope.polygons = [];
                     scope.markers = [];

                     scope.selectedYardID = "";
                     scope.selectedYard = "";
                     scope.selectedPolygon = "";
                  }
               }
            });

            scope.getYardPolygons = function (triggerMapUpdate) {
               scope.isBusyGetting = true;
               mapService.getYardPolygons().then(function (rawYards) {
                  scope.isBusyGetting = false;
                  if (!rawYards) {
                     toastr.error('Failed to retrieve yard polygons', 'Error');
                  } else {
                     scope.yards = rawYards;
                     console.log('%i yards found %O', scope.yards.length, scope.yards);
                     scope.polygons = [];
                     scope.markers = [];
                     scope.yards.forEach(function (yard) {
                        buildPolygons(yard);
                        buildMarkers(yard);
                     });

                     if (scope.selectedYardID)
                        scope.selectedYard = utilService.findArrayElement(scope.yards, "YardID", scope.selectedYardID);

                     if (triggerMapUpdate)
                        scope.triggerDisplayMapData();
                  }

               });
            };

            scope.setSelectedYard = function () {
               var yard = utilService.findArrayElement(scope.yards, "YardID", scope.selectedYardID);
               if (!yard) {
                  scope.selectedYard = "";
                  //scope.triggerHideMapData();
                  scope.triggerResetMap();
                  return;
               }
               scope.selectedYard = angular.copy(yard);
               console.log(scope.selectedYard);

               if (!scope.selectedYard.PolygonData) return;
               if (!scope.selectedYard.PolygonData.MiddleLat || !scope.selectedYard.PolygonData.MiddleLon) return;

               scope.triggerSetMapLocation(scope.selectedYard.PolygonData.MiddleLat, scope.selectedYard.PolygonData.MiddleLon, 16);
            };

            scope.setEdit = function () {
               scope.isEditing = true;
               console.log("Edit yard %s", scope.selectedYardID);
               scope.triggerSetEditPolygon("yardPolygon" + scope.selectedYardID);
            };

            scope.cancelEdit = function () {
               scope.isEditing = false;
               var yard = utilService.findArrayElement(scope.yards, "YardID", scope.selectedYardID);
               scope.selectedYard = angular.copy(yard);
               console.log("Cancel edit %O", scope.selectedYard);
               scope.triggerSetCancelEditPolygon("yardPolygon" + scope.selectedYardID, "#000000");
            };

            scope.save = function () {
               console.log("Saving yard...");
               if (scope.selectedPolygon) {
                  var polygonCoords = utilService.getPolygonCoords(scope.selectedPolygon);
                  scope.selectedYard.Position = JSON.stringify(polygonCoords);
               }
               scope.isBusySaving = true;
               mapService.saveYard(scope.selectedYard).then(function (result) {
                  scope.isBusySaving = false;
                  if (result === false || result === "false") {
                     console.error("Save Yard");
                     toastr.error('Failed to save Yard', 'Error');
                  } else {
                     console.log("Save Yard Success");
                     toastr.success('Yard Saved', 'Success');
                     scope.isEditing = false;
                     scope.triggerSetCancelEditPolygon("yardPolygon" + scope.selectedYardID, "#000000");
                     scope.triggerMakeAllPolygonsUneditable();

                     setTimeout(function () {
                        scope.getYardPolygons(false);
                     }, 500);
                  }
               });
            };

            scope.delete = function () {
               if (confirm("Ok to delete this Yard?")) {
                  scope.isBusyDeleting = true;
                  mapService.deleteYard(scope.selectedYard.YardID).then(function (result) {
                     scope.isBusyDeleting = false;
                     scope.isEditing = false;
                     if (result === false || result === "false") {
                        console.error("Delete Yard");
                        toastr.error('Failed to delete Yard', 'Error');
                     } else {
                        console.log("Delete Yard Success");
                        toastr.success('Yard Deleted', 'Success');

                        scope.selectedYardID = "";
                        scope.selectedYard = "";
                        scope.selectedPolygon = "";

                        scope.triggerMakeAllPolygonsUneditable();
                        scope.triggerHideMapData();

                        setTimeout(function () {
                           scope.getYardPolygons(true);
                        }, 500);
                     }
                  });
               }
            };

            scope.prepareNew = function () {
               scope.selectedYard = {
                  YardID: "",
                  YardDescription: "",
                  Position: "",
                  Description: "",
                  Comment: ""
               };
               scope.isAdding = true;
               scope.triggerSetNewPolygon("#000000");
            };

            scope.cancelAdd = function () {
               scope.isAdding = false;

               scope.selectedYardID = "";
               scope.selectedYard = "";
               scope.selectedPolygon = "";

               scope.triggerHideMapData();
               scope.triggerDisplayMapData();
            };

            scope.add = function () {
               console.log("Adding Yard...");
               if (scope.selectedPolygon) {
                  var polygonCoords = utilService.getPolygonCoords(scope.selectedPolygon);
                  scope.selectedYard.Position = JSON.stringify(polygonCoords);
               }
               scope.isBusyAdding = true;
               mapService.saveYard(scope.selectedYard).then(function (result) {
                  scope.isBusyAdding = false;
                  scope.isAdding = false;
                  if (result === false || result === "false") {
                     console.error("Add Yard");
                     toastr.error('Failed to add Yard', 'Error');
                  } else {
                     console.log("Add Yard Success");
                     toastr.success('Yard Added', 'Success');
                     scope.triggerMakeAllPolygonsUneditable();
                     scope.triggerHideMapData();

                     scope.getYardPolygons(true);
                  }
               });
            };

         }
      };
   }
})();
