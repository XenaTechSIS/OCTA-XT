(function () {
   "use strict";
   angular.module("octaApp.map").directive("mapCallBox", ["utilService", 'mapService', mapCallBox]);

   function mapCallBox(utilService, mapService) {
      return {
         restrict: 'E',
         templateUrl: $(".websiteUrl").text().trim() + '/app/map/directives/mapCallBoxTemplate.html',
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

            var selectedYardId = 0;

            scope.isEditing = false;
            scope.isAdding = false;
            scope.isBusyGetting = false;
            scope.isBusySaving = false;
            scope.isBusyDeleting = false;

            scope.callBoxs = [];
            scope.polygons = [];
            scope.markers = [];

            scope.selectedCallBoxID = "";
            scope.selectedCallBox = "";
            scope.selectedPolygon = "";

            function buildDetailsContent(callBox) {
               var content = "<table>";
               content += "<tr>";
               content += "<td>Number:</td>";
               content += "<td><strong>" + callBox.SignNumber + "</strong></td>";
               content += "</tr>";
               content += "<tr>";
               content += "<td>Comments:</td>";
               content += "<td><strong>" + callBox.Comments + "</strong></td>";
               content += "</tr>";              
               content += "</table>";
               return content;
            }
           
            function buildMarkers(callBox) {

               if (!callBox) return;
               if (!callBox.PolygonData) return;
               if (!callBox.PolygonData.Coordinates) return;
               var coor = callBox.PolygonData.Coordinates[0];

               var callBoxMarker = new MarkerWithLabel({
                  id: "callBoxMarker" + callBox.CallBoxID,
                  animation: google.maps.Animation.DROP,
                  position: new google.maps.LatLng(coor.lat, coor.lng),
                  draggable: false,
                  labelContent: callBox.SignNumber,
                  labelAnchor: new google.maps.Point(35, 40),
                  labelClass: "googleMapMarkerLabel", // the CSS class for the label
                  labelStyle: { opacity: 0.75 }
               });

               var infowindow = new google.maps.InfoWindow({
                  title: "Call Box Details",
                  content: buildDetailsContent(callBox)
               });
               callBoxMarker.addListener('click', function () {
                  infowindow.open(scope.map, callBoxMarker);
               });
               scope.markers.push(callBoxMarker);
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
               console.log("New CallBox Map Location %s, %s", lat, lon);
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
                        scope.getCallBoxPolygons(true);
                     } else {
                        scope.triggerDisplayMapData();
                     }
                  } else {
                     scope.selectedPolygon = "";
                  }
               }
            });

            scope.setSelectedCallBox = function () {
               scope.selectedCallBox = utilService.findArrayElement(scope.callBoxs, "CallBoxID", scope.selectedCallBoxID);
               if (!scope.selectedCallBox) {
                  scope.triggerHideMapData();
                  scope.triggerResetMap();
                  return;
               }
               console.log(scope.selectedCallBox);

               if (!scope.selectedCallBox.PolygonData) return;
               if (!scope.selectedCallBox.PolygonData.MiddleLat || !scope.selectedCallBox.PolygonData.MiddleLon) return;

               scope.triggerSetMapLocation(scope.selectedCallBox.PolygonData.MiddleLat, scope.selectedCallBox.PolygonData.MiddleLon, 16);
            };

            scope.getCallBoxPolygons = function (triggerMapUpdate) {
               scope.isBusyGetting = true;
               mapService.getCallBoxPolygons().then(function (rawCallBoxs) {
                  scope.isBusyGetting = false;
                  if (!rawCallBoxs) {
                     toastr.error('Failed to retrieve callbox polygons', 'Error');
                  } else {
                     scope.callBoxs = rawCallBoxs;
                     console.log('%i callboxes found %O', scope.callBoxs.length, scope.callBoxs);
                     scope.polygons = [];
                     scope.markers = [];
                     scope.callBoxs.forEach(function (callBox) {                       
                        buildMarkers(callBox);
                     });

                     //if (selectedSegmentId)
                     //    scope.selectedBeatSegment = utilService.findArrayElement(scope.segments, "ID", selectedSegmentId);

                     if (triggerMapUpdate)
                        scope.triggerDisplayMapData();
                  }

               });
            };

            scope.setEdit = function () {
               scope.isEditing = true;
               console.log("Edit callBox %s", scope.selectedCallBox.CallBoxID);
               scope.triggerSetEditPolygon("callBoxPolygon" + scope.selectedCallBox.CallBoxID);
            };

            scope.cancelEdit = function () {
               scope.isEditing = false;
               console.log("Cancel edit callBox %s", scope.selectedCallBox.CallBoxID);
               scope.triggerSetCancelEditPolygon("callBoxPolygon" + scope.selectedCallBox.CallBoxID, scope.selectedCallBox.Color);
            };

            scope.save = function () {
               console.log("Saving callBox...");
               if (scope.selectedPolygon) {
                  var polygonCoords = utilService.getPolygonCoords(scope.selectedPolygon);
                  scope.selectedCallBox.Position = JSON.stringify(polygonCoords);
               }
               scope.isBusySaving = true;
               mapService.saveCallBox(scope.selectedCallBox).then(function (result) {
                  scope.isBusySaving = false;
                  if (result === false || result === "false") {
                     console.error("Save Call Box");
                     toastr.error('Failed to save Call Box', 'Error');
                  } else {
                     console.log("Save Call Box Success");
                     toastr.success('Call Box Saved', 'Success');
                     scope.cancelEdit();
                     scope.triggerMakeAllPolygonsUneditable();
                  }
               });
            };

            scope.delete = function () {
               if (confirm("Ok to delete this Call Box?")) {
                  scope.isBusyDeleting = true;
                  mapService.deleteYard(scope.selectedCallBox.CallBoxID).then(function (result) {
                     scope.isBusyDeleting = false;
                     scope.isEditing = false;
                     if (result === false || result === "false") {
                        console.error("Delete CallBox");
                        toastr.error('Failed to delete CallBox', 'Error');
                     } else {
                        console.log("Delete CallBox Success");
                        toastr.success('CallBox Deleted', 'Success');

                        scope.selectedYardID = "";
                        scope.selectedCallBox = "";
                        scope.selectedPolygon = "";

                        scope.triggerMakeAllPolygonsUneditable();
                        scope.triggerHideMapData();

                        setTimeout(function () {
                           scope.getCallBoxPolygons(true);
                        }, 500);
                     }
                  });
               }
            };

            scope.prepareNew = function () {
               scope.selectedCallBox = {
                  CallBoxID: "",
                  CallBoxDescription: "",
                  CallBoxNumber: "",
                  Location: "",
                  Restrictions: "",
                  Capacity: 0,
                  City: "",
                  PDPhoneNumber: "",
                  Position: ""               
               };
               scope.isAdding = true;
               scope.triggerSetNewPolygon(scope.selectedCallBox.Color);
            };

            scope.cancelAdd = function () {
               scope.isAdding = false;

               scope.selectedYardID = "";
               scope.selectedCallBox = "";
               scope.selectedPolygon = "";

               scope.triggerHideMapData();
               scope.triggerDisplayMapData();
            };

            scope.add = function () {
               console.log("Adding CallBox...");
               if (scope.selectedPolygon) {
                  var polygonCoords = utilService.getPolygonCoords(scope.selectedPolygon);
                  scope.selectedCallBox.Position = JSON.stringify(polygonCoords);
               }
               scope.isBusyAdding = true;
               mapService.saveYard(scope.selectedCallBox).then(function (result) {
                  scope.isBusyAdding = false;
                  scope.isAdding = false;
                  if (result === false || result === "false") {
                     console.error("Add CallBox");
                     toastr.error('Failed to add CallBox', 'Error');
                  } else {
                     console.log("Add CallBox Success");
                     toastr.success('CallBox Added', 'Success');
                     scope.triggerMakeAllPolygonsUneditable();
                     scope.triggerHideMapData();

                     scope.getCallBoxPolygons(true);
                  }
               });
            };

         }
      };
   }
})();
