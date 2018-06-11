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

                var selectedSegmentId = 0;

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
                    content += "<td>ID:</td>";
                    content += "<td><strong>" + segment.segmentID + "</strong></td>";
                    content += "</tr>";
                    content += "<tr>";
                    content += "<td>Description:</td>";
                    content += "<td><strong>" + segment.segmentDescription + "</strong></td>";
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

                    var latDelta = (segment.maxLat - segment.minLat) / 2;
                    var middleLat = segment.minLat + latDelta;
                    var lonDelta = (segment.maxLon - segment.minLon) / 2;
                    var middleLon = segment.minLon + lonDelta;

                    var segmentMarker = new MarkerWithLabel({
                        id: "segmentMarker" + segment.segmentID,
                        animation: google.maps.Animation.DROP,
                        position: new google.maps.LatLng(middleLat, middleLon),
                        draggable: false,
                        labelContent: segment.segmentID,
                        labelAnchor: new google.maps.Point(25, 40),
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

                scope.setSelectedBeatSegment = function () {
                    scope.selectedBeatSegment = utilService.findArrayElement(scope.segments, "BeatSegmentID", scope.selectedBeatSegmentID);
                    if (!scope.selectedBeatSegment) {
                        scope.triggerHideMapData();
                        scope.triggerResetMap();
                        return;
                    }
                    console.log(scope.selectedBeatSegment);

                    if (!scope.selectedBeatSegment.PolygonData) return;
                    if (!scope.selectedBeatSegment.PolygonData.MiddleLat || !scope.selectedBeatSegment.PolygonData.MiddleLon) return;

                    scope.triggerSetMapLocation(scope.selectedBeatSegment.PolygonData.MiddleLat, scope.selectedBeatSegment.PolygonData.MiddleLon, 16);
                };

                scope.getSegments = function (triggerMapUpdate) {
                    scope.isBusyGettingSegments = true;
                    mapService.getSegmentPolygons().then(function (rawSegments) {
                        scope.isBusyGettingSegments = false;
                        if (!rawSegments) {
                            toastr.error('Failed to retrieve Segments', 'Error');
                        } else {
                            scope.segments = rawSegments;
                            console.log('%i segments found %O', scope.segments.length, scope.segments);
                            scope.polygons = [];
                            scope.markers = [];
                            scope.segments.forEach(function (segment) {
                                buildPolygons(segment);
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
                    console.log("Edit segment %s", scope.selectedBeatSegment.BeatSegmentID);
                    scope.triggerSetEditPolygon("segmentPolygon" + scope.selectedBeatSegment.BeatSegmentID);
                };

                scope.cancelEdit = function () {
                    scope.isEditing = false;
                    console.log("Cancel edit segment %s", scope.selectedBeatSegment.BeatSegmentID);
                    scope.triggerSetCancelEditPolygon("segmentPolygon" + scope.selectedBeatSegment.BeatSegmentID, scope.selectedBeatSegment.Color);
                };

                scope.save = function () {
                    console.log("Saving segment...");
                    if (scope.selectedPolygon) {
                        var polygonCoords = utilService.getPolygonCoords(scope.selectedPolygon);
                        scope.selectedBeatSegment.BeatSegmentExtent = JSON.stringify(polygonCoords);
                    }                        
                    scope.isBusySaving = true;
                    mapService.saveSegment(scope.selectedBeatSegment).then(function (result) {
                        scope.isBusySaving = false;
                        if (result === false || result === "false") {
                            console.error("Save Segment");
                            toastr.error('Failed to save Segment', 'Error');
                        } else {
                            console.log("Save Segment Success");
                            toastr.success('Segment Saved', 'Success');
                            scope.cancelEdit();
                            scope.triggerMakeAllPolygonsUneditable();
                        }
                    });
                };

                scope.delete = function () {
                    if (confirm("Ok to delete this Segment?")) {
                        scope.isBusyDeleting = true;
                        mapService.deleteSegment(scope.selectedBeatSegment.ID).then(function (result) {
                            scope.isBusyDeleting = false;
                            scope.isEditing = false;
                            if (!result) {
                                console.error("Delete Segment");
                                toastr.error('Failed to delete Segment', 'Error');
                            } else {
                                console.log("Delete Segment Success");
                                toastr.success('Segment Deleted', 'Success');
                                selectedSegmentId = 0;
                                scope.selectedBeatSegment = "";
                                scope.triggerMakeAllPolygonsUneditable();
                                scope.triggerHideMapData();
                                scope.getSegments(true);
                            }
                        });
                    }
                };

                scope.prepareNew = function () {
                    scope.selectedBeatSegment = {
                        BeatSegmentID: "",
                        Color: "#000000",
                        geoFence: []
                    };
                    scope.isAdding = true;
                    scope.triggerSetNewPolygon(scope.selectedBeatSegment.color);
                };

                scope.cancelAdd = function () {
                    scope.isAdding = false;
                    scope.selectedBeatSegment = "";
                    scope.triggerHideMapData();
                    scope.triggerDisplayMapData();
                };

                scope.add = function () {
                    console.log("Adding Segment...");
                    if (scope.selectedPolygon)
                        scope.selectedBeatSegment.geoFence = utilService.getPolygonCoords(scope.selectedPolygon);

                    scope.isBusyAdding = true;
                    mapService.addSegment(scope.selectedBeatSegment).then(function (result) {
                        scope.isBusyAdding = false;
                        scope.isAdding = false;
                        if (!result) {
                            console.error("Add Segment");
                            toastr.error('Failed to add Segment', 'Error');
                        } else {
                            console.log("Add Segment Success");
                            toastr.success('Segment Added', 'Success');
                            scope.triggerMakeAllPolygonsUneditable();
                            scope.triggerHideMapData();
                            scope.getSegments(true);
                        }
                    });
                };

                scope.getBeats = function () {
                    generalService.getBeatNumbers().then(function (result) {
                        console.log("Beats %O", result);
                        scope.beats = result;
                    });
                };

            }
        };
    }
})();
