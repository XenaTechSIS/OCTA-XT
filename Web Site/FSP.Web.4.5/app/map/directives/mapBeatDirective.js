(function () {
    "use strict";
    angular.module("octaApp.map").directive("mapBeat", ["$rootScope", "utilService", "generalService", 'mapService', mapSegment]);

    function mapSegment($rootScope, utilService, generalService, mapService) {
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

                var selectedSegmentId = 0;

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

                function buildDetailsContent(beat) {
                    var content = "<table>";
                    content += "<tr>";
                    content += "<td>ID:</td>";
                    content += "<td><strong>" + beat.BeatID + "</strong></td>";
                    content += "</tr>";
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
                    if (!beat.PolygonData) return;
                    if (!beat.PolygonData.Coordinates) return;

                    var cleanLatLng = [];

                    beat.PolygonData.Coordinates.forEach(function (coordinate) {
                        cleanLatLng.push({
                            lat: coordinate.lat,
                            lng: coordinate.lng
                        });
                    });

                    var beatPolygon = new google.maps.Polygon({
                        id: "beatPolygon" + beat.BeatID,
                        paths: cleanLatLng,
                        strokeColor: beat.BeatColor || "#000000",
                        strokeOpacity: 0.8,
                        strokeWeight: 2,
                        fillColor: beat.BeatColor || "#000000",
                        fillOpacity: 0.35,
                        editable: false
                    });
                    scope.polygons.push(beatPolygon);
                }

                function buildMarkers(beat) {

                    if (!beat) return;
                    if (!beat.PolygonData) return;

                    var beatMarker = new MarkerWithLabel({
                        id: "beatMarker" + beat.BeatID,
                        animation: google.maps.Animation.DROP,
                        position: new google.maps.LatLng(beat.PolygonData.MiddleLat, beat.PolygonData.MiddleLon),
                        draggable: false,
                        labelContent: beat.BeatID,
                        labelAnchor: new google.maps.Point(25, 40),
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
                            scope.selectedPolygon = "";
                        }
                    }
                });

                scope.setSelectedBeat = function () {
                    scope.selectedBeat = utilService.findArrayElement(scope.beats, "BeatID", scope.selectedBeatID);
                    if (!scope.selectedBeat) {
                        scope.triggerHideMapData();
                        scope.triggerResetMap();
                        return;
                    }
                    console.log(scope.selectedBeat);

                    if (!scope.selectedBeat.PolygonData) return;
                    if (!scope.selectedBeat.PolygonData.MiddleLat || !scope.selectedBeat.PolygonData.MiddleLon) return;

                    scope.triggerSetMapLocation(scope.selectedBeat.PolygonData.MiddleLat, scope.selectedBeat.PolygonData.MiddleLon, 16);
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
                                //buildMarkers(segment);
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
                    console.log("Edit beat %s", scope.selectedBeat.BeatID);
                    scope.triggerSetEditPolygon("beatPolygon" + scope.selectedBeat.BeatID);
                };

                scope.cancelEdit = function () {
                    scope.isEditing = false;
                    console.log("Cancel edit beat %s", scope.selectedBeat.BeatID);
                    scope.triggerSetCancelEditPolygon("beatPolygon" + scope.selectedBeat.BeatID, scope.selectedBeat.BeatColor);
                };

                scope.save = function () {
                    console.log("Saving beat...");
                    if (scope.selectedPolygon) {
                        var polygonCoords = utilService.getPolygonCoords(scope.selectedPolygon);
                        scope.selectedBeat.BeatExtent = JSON.stringify(polygonCoords);
                    }
                    scope.isBusySaving = true;
                    mapService.saveBeat(scope.selectedBeat).then(function (result) {
                        scope.isBusySaving = false;
                        if (result === false || result === "false") {
                            console.error("Save Beat");
                            toastr.error('Failed to save Beat', 'Error');
                        } else {
                            console.log("Save Beat Success");
                            toastr.success('Beat Saved', 'Success');
                            scope.cancelEdit();
                            scope.triggerMakeAllPolygonsUneditable();
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
                        BeatColor: "#000000",
                        BeatExtent: "",
                        BeatNumber: "",
                        BeatDescription: ""
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
                    if (scope.selectedPolygon) {
                        var polygonCoords = utilService.getPolygonCoords(scope.selectedPolygon);
                        scope.selectedBeat.BeatExtent = JSON.stringify(polygonCoords);
                    }
                    scope.isBusyAdding = true;
                    mapService.saveBeat(scope.selectedBeat).then(function (result) {
                        scope.isBusyAdding = false;
                        scope.isAdding = false;
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

            }
        };
    }
})();
