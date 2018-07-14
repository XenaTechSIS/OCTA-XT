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
            setEditMarker: "&",
            setCancelEditMarker: "&",
            setMarkerPosition: "&",
            setNewMarker: "&",
            makeAllPolygonsUneditable: "&",

            selectedMarker: "=",
            visible: "="
         },
         link: function (scope) {

            var selectedZoomFactor = 15;

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
            scope.selectedPosition = "";

            function buildDetailsContent(callBox) {
               var content = "<table>";
               content += "<tr>";
               content += "<td>Sign Number:</td>";
               content += "<td><strong>" + callBox.SignNumber + "</strong></td>";
               content += "</tr>";
               content += "<tr>";
               content += "<td>Comments:</td>";
               content += "<td><strong>" + callBox.Comments + "</strong></td>";
               content += "</tr>";
               content += "<tr>";
               content += "<td>Location:</td>";
               content += "<td><strong>" + callBox.Location + "</strong></td>";
               content += "</tr>";
               content += "<tr>";
               content += "<td>Lat:</td>";
               content += "<td><strong>" + callBox.PolygonData.Coordinates[0].lat + "</strong></td>";
               content += "</tr>";
               content += "<tr>";
               content += "<td>Lon:</td>";
               content += "<td><strong>" + callBox.PolygonData.Coordinates[0].lng + "</strong></td>";
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
                  signNumber: callBox.SignNumber,
                  animation: google.maps.Animation.DROP,
                  position: new google.maps.LatLng(coor.lat, coor.lng),
                  draggable: false,
                  raisedOnDrag: true,
                  labelContent: callBox.SignNumber,
                  labelAnchor: new google.maps.Point(35, 50),
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

            scope.triggerSetMapLocation = function (lat, lon, zoom) {
               console.log("New CallBox Map Location %s, %s", lat, lon);
               scope.setMapLocation({
                  lat: lat,
                  lon: lon,
                  zoom: zoom
               });
            };

            scope.triggerSetEditMarker = function (id) {
               scope.setEditMarker({
                  id: id
               });
            };

            scope.triggerSetCancelEditMarker = function (id) {
               scope.setCancelEditMarker({
                  id: id
               });
            };

            scope.triggerSetMarkerPosition = function (lat, lon) {
               scope.setMarkerPosition({
                  lat: lat,
                  lon: lon
               });
            };

            scope.triggerSetNewMarker = function () {
               scope.setNewMarker();
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
                     scope.callBoxs = [];
                     scope.polygons = [];
                     scope.markers = [];

                     scope.selectedCallBoxID = "";
                     scope.selectedCallBox = "";
                     scope.selectedPosition = "";
                  }
               }
            });

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

                     if (scope.selectedCallBoxID) {
                        scope.selectedCallBox = utilService.findArrayElement(scope.callBoxs, "CallBoxID", scope.selectedCallBoxID);
                        scope.selectedPosition = scope.selectedCallBox.PolygonData.Coordinates[0];
                        console.log(scope.selectedPosition);
                     }

                     if (triggerMapUpdate)
                        scope.triggerDisplayMapData();
                  }

               });
            };

            scope.setSelectedCallBox = function () {
               var cb = utilService.findArrayElement(scope.callBoxs, "CallBoxID", scope.selectedCallBoxID);
               if (!cb) {
                  scope.selectedCallBox = "";
                  //scope.triggerHideMapData();
                  scope.triggerResetMap();
                  return;
               }

               scope.selectedCallBox = angular.copy(cb);
               console.log(scope.selectedCallBox);

               scope.selectedPosition = scope.selectedCallBox.PolygonData.Coordinates[0];
               console.log(scope.selectedPosition);

               scope.triggerSetMapLocation(scope.selectedPosition.lat, scope.selectedPosition.lng, selectedZoomFactor);
            };

            scope.setEdit = function () {
               scope.isEditing = true;
               console.log("Edit callBox %s", scope.selectedCallBoxID);
               scope.triggerSetEditMarker("callBoxMarker" + scope.selectedCallBoxID);
            };

            scope.cancelEdit = function () {
               scope.isEditing = false;

               var cb = utilService.findArrayElement(scope.callBoxs, "CallBoxID", scope.selectedCallBoxID);
               scope.selectedCallBox = angular.copy(cb);
               console.log("Cancel edit callBox %s", scope.selectedCallBoxID);

               scope.selectedPosition = scope.selectedCallBox.PolygonData.Coordinates[0];
               console.log(scope.selectedPosition);
               scope.triggerSetMapLocation(scope.selectedPosition.lat, scope.selectedPosition.lng, selectedZoomFactor);

               scope.triggerSetCancelEditMarker("callBoxMarker" + scope.selectedCallBoxID);
               scope.triggerSetMarkerPosition(scope.selectedPosition.lat, scope.selectedPosition.lng);

            };

            //we need "selectedPosition" only becauase we also want to allow
            //user to directly update LAT/LON
            scope.$watch("selectedMarker.position", function (newValue) {
               if (newValue) {
                  scope.selectedPosition.lat = newValue.lat();
                  scope.selectedPosition.lng = newValue.lng();
               }
            });

            scope.save = function () {
               console.log("Saving callBox...");
               if (scope.selectedPosition) {
                  var position = [];
                  position.push(scope.selectedPosition);
                  scope.selectedCallBox.Position = JSON.stringify(position);
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
                     scope.isEditing = false;
                     scope.selectedCallBox = "";
                     scope.selectedPosition = "";
                     scope.triggerHideMapData();

                     setTimeout(function () {
                        scope.getCallBoxPolygons(true);
                     }, 250);
                  }
               });
            };

            scope.delete = function () {
               if (confirm("Ok to delete this Call Box?")) {
                  scope.isBusyDeleting = true;
                  mapService.deleteCallBox(scope.selectedCallBox.CallBoxID).then(function (result) {
                     scope.isBusyDeleting = false;
                     scope.isEditing = false;
                     if (result === false || result === "false") {
                        console.error("Delete CallBox");
                        toastr.error('Failed to delete Call Box', 'Error');
                     } else {
                        console.log("Delete CallBox Success");
                        toastr.success('Call Box Deleted', 'Success');

                        scope.selectedCallBoxID = "";
                        scope.selectedCallBox = "";
                        scope.selectedPosition = "";

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
                  Comments: "",
                  Location: "",
                  FreewayID: 0,
                  Position: [],
                  SignNumber: "",
                  SiteType: "L",
                  TelephoneNumber: ""
               };
               scope.selectedPosition = {
                  lat: "",
                  lng: ""
               };
               scope.isAdding = true;
               scope.triggerSetNewMarker();
            };

            scope.cancelAdd = function () {
               scope.isAdding = false;

               scope.selectedCallBoxID = "";
               scope.selectedCallBox = "";
               scope.selectedPosition = "";

               scope.triggerHideMapData();
               scope.triggerDisplayMapData();
            };

            scope.add = function () {
               console.log("Adding CallBox...");
               if (scope.selectedPosition) {
                  var position = [];
                  position.push(scope.selectedPosition);
                  scope.selectedCallBox.Position = JSON.stringify(position);
               }
               scope.isBusyAdding = true;
               mapService.saveCallBox(scope.selectedCallBox).then(function (result) {
                  scope.isBusyAdding = false;
                  if (result === false || result === "false") {
                     console.error("Add CallBox");
                     toastr.error('Failed to add Call Box', 'Error');
                  } else {
                     scope.isAdding = false;
                     scope.selectedCallBox = "";
                     scope.selectedPosition = "";
                     console.log("Add CallBox Success");
                     toastr.success('Call Box Added', 'Success');
                     scope.triggerHideMapData();
                     scope.getCallBoxPolygons(true);
                  }
               });
            };

         }
      };
   }
})();
