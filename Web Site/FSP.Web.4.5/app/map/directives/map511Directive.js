(function () {
  "use strict";
  angular.module("octaApp.map").directive("map511", ["utilService", 'mapService', map511]);

  function map511(utilService, mapService) {
    return {
      restrict: 'E',
      templateUrl: $(".websiteUrl").text().trim() + '/app/map/directives/map511Template.html',
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
        visible: "=",
        canEdit: "="
      },
      link: function (scope) {

        var selectedZoomFactor = 15;

        scope.isEditing = false;
        scope.isAdding = false;
        scope.isBusyGetting = false;
        scope.isBusySaving = false;
        scope.isBusyDeleting = false;

        scope.five11s = [];
        scope.polygons = [];
        scope.markers = [];

        scope.selectedFiveElevenSignID = "";
        scope.selectedFive11 = "";
        scope.selectedPosition = "";

        function buildDetailsContent(five11) {
          var content = "<table>";
          content += "<tr>";
          content += "<td>Sign Number:</td>";
          content += "<td><strong>" + five11.SignNumber + "</strong></td>";
          content += "</tr>";
          content += "<tr>";
          content += "<td>Phone #:</td>";
          content += "<td><strong>" + five11.TelephoneNumber + "</strong></td>";
          content += "</tr>";
          content += "<tr>";
          content += "<td>Location:</td>";
          content += "<td><strong>" + five11.Location + "</strong></td>";
          content += "</tr>";
          content += "<tr>";
          content += "<td>Lat:</td>";
          content += "<td><strong>" + five11.PolygonData.Coordinates[0].lat + "</strong></td>";
          content += "</tr>";
          content += "<tr>";
          content += "<td>Lon:</td>";
          content += "<td><strong>" + five11.PolygonData.Coordinates[0].lng + "</strong></td>";
          content += "</tr>";
          content += "</table>";
          return content;
        }

        function buildMarkers(five11) {

          if (!five11) return;
          if (!five11.PolygonData) return;
          if (!five11.PolygonData.Coordinates) return;
          var coor = five11.PolygonData.Coordinates[0];

          var five11Marker = new MarkerWithLabel({
            id: "five11Marker" + five11.Five11SignID,
            signNumber: five11.SignNumber,
            animation: google.maps.Animation.DROP,
            position: new google.maps.LatLng(coor.lat, coor.lng),
            draggable: false,
            raisedOnDrag: true,
            labelContent: five11.SignNumber,
            labelAnchor: new google.maps.Point(35, 50),
            labelClass: "googleMapMarkerLabel", // the CSS class for the label
            labelStyle: { opacity: 0.75 }
          });

          var infowindow = new google.maps.InfoWindow({
            title: "Five 11 Details",
            content: buildDetailsContent(five11)
          });
          five11Marker.addListener('click', function () {
            infowindow.open(scope.map, five11Marker);
          });
          scope.markers.push(five11Marker);
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
          console.log("New Five11 Map Location %s, %s", lat, lon);
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
                scope.getFive11Polygons(true);
              } else {
                scope.triggerDisplayMapData();
              }
            } else {
              scope.five11s = [];
              scope.polygons = [];
              scope.markers = [];

              scope.selectedFiveElevenSignID = "";
              scope.selectedFive11 = "";
              scope.selectedPosition = "";
            }
          }
        });

        scope.getFive11Polygons = function (triggerMapUpdate) {
          scope.isBusyGetting = true;
          mapService.getFive11Polygons().then(function (rawFive11s) {
            scope.isBusyGetting = false;
            if (!rawFive11s) {
              toastr.error('Failed to retrieve five11 polygons', 'Error');
            } else {
              scope.five11s = rawFive11s;
              console.log('%i five11s found %O', scope.five11s.length, scope.five11s);
              scope.polygons = [];
              scope.markers = [];
              scope.five11s.forEach(function (five11) {
                buildMarkers(five11);
              });

              if (scope.selectedFiveElevenSignID) {
                scope.selectedFive11 = utilService.findArrayElement(scope.five11s, "Five11SignID", scope.selectedFiveElevenSignID);
                scope.selectedPosition = scope.selectedFive11.PolygonData.Coordinates[0];
                console.log(scope.selectedPosition);
              }

              if (triggerMapUpdate)
                scope.triggerDisplayMapData();
            }

          });
        };

        scope.setSelectedFiveEleven = function () {
          var cb = utilService.findArrayElement(scope.five11s, "Five11SignID", scope.selectedFiveElevenSignID);
          if (!cb) {
            scope.selectedFive11 = "";
            scope.triggerHideMapData();
            scope.triggerResetMap();
            return;
          }

          scope.selectedFive11 = angular.copy(cb);
          console.log(scope.selectedFive11);

          scope.selectedPosition = scope.selectedFive11.PolygonData.Coordinates[0];
          console.log(scope.selectedPosition);

          scope.triggerSetMapLocation(scope.selectedPosition.lat, scope.selectedPosition.lng, selectedZoomFactor);
        };

        scope.setEdit = function () {
          scope.isEditing = true;
          console.log("Edit five11 %s", scope.selectedFiveElevenSignID);
          scope.triggerSetEditMarker("five11Marker" + scope.selectedFiveElevenSignID);
        };

        scope.cancelEdit = function () {
          scope.isEditing = false;

          var cb = utilService.findArrayElement(scope.five11s, "Five11SignID", scope.selectedFiveElevenSignID);
          scope.selectedFive11 = angular.copy(cb);
          console.log("Cancel edit five11 %s", scope.selectedFiveElevenSignID);

          scope.selectedPosition = scope.selectedFive11.PolygonData.Coordinates[0];
          console.log(scope.selectedPosition);
          scope.triggerSetMapLocation(scope.selectedPosition.lat, scope.selectedPosition.lng, selectedZoomFactor);

          scope.triggerSetCancelEditMarker("five11Marker" + scope.selectedFiveElevenSignID);
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
          console.log("Saving five11...");
          if (scope.selectedPosition) {
            var position = [];
            position.push(scope.selectedPosition);
            scope.selectedFive11.Position = JSON.stringify(position);
          }
          scope.isBusySaving = true;
          mapService.saveFive11(scope.selectedFive11).then(function (result) {
            scope.isBusySaving = false;
            if (result === false || result === "false") {
              console.error("Save Five 11");
              toastr.error('Failed to save 511 Sign', 'Error');
            } else {
              console.log("Save Five 11 Success");
              toastr.success('511 Sign Saved', 'Success');
              scope.isEditing = false;
              scope.selectedFive11 = "";
              scope.selectedPosition = "";
              scope.triggerHideMapData();

              setTimeout(function () {
                scope.getFive11Polygons(true);
              }, 250);
            }
          });
        };

        scope.delete = function () {
          if (confirm("Ok to delete this 511 Sign?")) {
            scope.isBusyDeleting = true;
            mapService.deleteFive11(scope.selectedFive11.Five11SignID).then(function (result) {
              scope.isBusyDeleting = false;
              scope.isEditing = false;
              if (result === false || result === "false") {
                console.error("Delete Five11");
                toastr.error('Failed to delete 511 Sign', 'Error');
              } else {
                console.log("Delete Five11 Success");
                toastr.success('511 Sign Deleted', 'Success');

                scope.selectedFiveElevenSignID = "";
                scope.selectedFive11 = "";
                scope.selectedPosition = "";

                scope.triggerHideMapData();

                setTimeout(function () {
                  scope.getFive11Polygons(true);
                }, 500);
              }
            });
          }
        };

        scope.prepareNew = function () {
          scope.selectedFive11 = {
            Five11SignID: "",
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

          scope.selectedFiveElevenSignID = "";
          scope.selectedFive11 = "";
          scope.selectedPosition = "";

          scope.triggerHideMapData();
          scope.triggerDisplayMapData();
        };

        scope.add = function () {
          console.log("Adding Five11...");
          if (scope.selectedPosition) {
            var position = [];
            position.push(scope.selectedPosition);
            scope.selectedFive11.Position = JSON.stringify(position);
          }
          scope.isBusyAdding = true;
          mapService.saveFive11(scope.selectedFive11).then(function (result) {
            scope.isBusyAdding = false;
            if (result === false || result === "false") {
              console.error("Add Five11");
              toastr.error('Failed to add 511 Sign', 'Error');
            } else {
              scope.isAdding = false;
              scope.selectedFive11 = "";
              scope.selectedPosition = "";
              console.log("Add Five11 Success");
              toastr.success('511 Sign Added', 'Success');
              scope.triggerHideMapData();
              scope.getFive11Polygons(true);
            }
          });
        };

      }
    };
  }
})();
