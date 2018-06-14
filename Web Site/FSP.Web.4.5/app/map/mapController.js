(function () {
    'use strict';
    angular.module("octaApp.map").controller("mapController", ['$scope', '$rootScope', '$window', '$interval', '$compile', 'trucksService', 'utilService', mapController]);
    function mapController($scope, $rootScope, $window, $interval, $compile, trucksService, utilService) {
        $scope.header = "Hello World!" + $rootScope.rootUrl;

        console.log($scope.header);

        var DEFAULT_MAP_CENTER_LAT = 33.739660;
        var DEFAULT_MAP_CENTER_LON = -117.832146;
        var ZOOM_9 = 9;
        var ZOOM_11 = 11;
        var ZOOM_12 = 12;
        var ZOOM_13 = 13;
        var ZOOM_14 = 14;
        var ZOOM_15 = 15;
        var ZOOM_16 = 16;
        var ZOOM_17 = 17;
        var ZOOM_18 = 18;
        var ZOOM_20 = 20;
        var IS_SELECTED_COLOR = "#FF9900";
        var IS_EDITING_COLOR = "#FF0000";

        var mapTopOffset = 95;
        var mapLeftOffset = 295;
        var refreshRate = 2000;

        $scope.map;
        $scope.infowindow = new google.maps.InfoWindow({
            content: ''
        });
        $scope.mapPolygon = new google.maps.Polygon({
            strokeColor: '#FF0000',
            strokeOpacity: 0.8,
            strokeWeight: 2,
            fillColor: '#FF0000',
            fillOpacity: 0.35
        });
        $scope.generalMarker = new google.maps.Marker({});

        $scope.isBusyGettingTrucks = false;
        $scope.trucks = [];
        $scope.truckMarkers = [];
        $scope.selectedTruck = "";
        $scope.selectedTruckMarker = "";
        $scope.truckToBeFollowed = "";
        $scope.polygons = [];
        $scope.selectedPolygon = {};

        $scope.haveAlarms = false;

        //filter
        $scope.filterApplied = false;
        $scope.allFiltersChecked = true;
        $scope.onPatrolChecked = true;
        $scope.driverLoggedOnChecked = true;
        $scope.onAssistChecked = true;
        $scope.onBreakLunchChecked = true;
        $scope.onRollOutInChecked = true;
        $scope.notLoggedOnChecked = true;
        $scope.contractorNameFilter = "";

        $scope.toggleAllFilters = function () {
            console.log("Toggle all filters %s", $scope.allFiltersChecked);
            $scope.onPatrolChecked = $scope.allFiltersChecked;
            $scope.driverLoggedOnChecked = $scope.allFiltersChecked;
            $scope.onAssistChecked = $scope.allFiltersChecked;
            $scope.onBreakLunchChecked = $scope.allFiltersChecked;
            $scope.onRollOutInChecked = $scope.allFiltersChecked;
            $scope.notLoggedOnChecked = $scope.allFiltersChecked;
        };
        $scope.clearContractorName = function () {
            $scope.contractorNameFilter = "";
        }

        $scope.segmentsVisible = false;
        $scope.polygons = [];
        $scope.selectedPolygon = {};
        $scope.markers = [];

        function sizeMap() {

            var wHeight = ($window.innerHeight - mapTopOffset) + 'px';
            var wWidth = ($window.innerWidth - mapLeftOffset) + 'px';
            $("#googleMap").height(wHeight);
            $("#googleMap").width(wWidth);

            $(".mapSideNavigation").height(wHeight);
        }

        function setMapControls() {

            var controlDiv = document.createElement('div');

            // Set CSS for the control border.
            var controlUI = document.createElement('div');
            controlUI.style.backgroundColor = '#fff';
            controlUI.style.border = '2px solid #fff';
            controlUI.style.borderRadius = '3px';
            controlUI.style.boxShadow = '0 2px 6px rgba(0,0,0,.3)';
            controlUI.style.cursor = 'pointer';
            controlUI.style.marginBottom = '22px';
            controlUI.style.textAlign = 'center';
            controlUI.title = 'Click to recenter the map';
            controlDiv.appendChild(controlUI);

            // Set CSS for the control interior.
            var controlText = document.createElement('div');
            controlText.style.color = 'rgb(25,25,25)';
            controlText.style.fontFamily = 'Roboto,Arial,sans-serif';
            controlText.style.fontSize = '12px';
            controlText.style.lineHeight = '38px';
            controlText.style.paddingLeft = '5px';
            controlText.style.paddingRight = '5px';
            controlText.innerHTML = 'Center Map';
            controlUI.appendChild(controlText);

            // Setup the click event listeners: simply set the map to Chicago.
            controlUI.addEventListener('click', function () {
                updateMap(new google.maps.LatLng(DEFAULT_MAP_CENTER_LAT, DEFAULT_MAP_CENTER_LON), ZOOM_11);
            });

            controlDiv.index = 1;
            $scope.map.controls[google.maps.ControlPosition.TOP_CENTER].push(controlDiv);

        }

        function updateMap(latlng, zoom) {
            $scope.map.panTo(latlng);
            $scope.map.setZoom(zoom);
        }

        function checkForAlarms() {
            console.log("Checking for alarms...");
            trucksService.getTrucksRefreshRate().then(function (result) {

                if ($scope.haveAlarms === false && result === true) {
                    console.log("Have alarms? %s", $scope.haveAlarms);
                }
                $scope.haveAlarms = result;

                if ($scope.haveAlarms) {
                    $("#monitoringTab").css("color", "red");
                    $("#alertMenuItem").css("color", "red");
                }
                else {
                    $("#monitoringTab").css("color", "#999999");
                    $("#alertMenuItem").css("color", "black");
                }

            });
        }

        function getTruckRefreshRate() {
            console.log("Getting refresh rate...");
            trucksService.getTrucksRefreshRate().then(function (result) {
                refreshRate = result;
                console.log(refreshRate);

                getTrucks();
                $interval(function () {
                    getTrucks();
                }, eval(refreshRate));
            });
        }

        function filterTrucks() {

            if (!$scope.filterApplied) return;
            if ($scope.allFiltersChecked && $scope.contractorNameFilter === "") return;

            var filteredTrucks = [];

            $scope.trucks.forEach(function (truck) {
                var truckIsGood = false;
                if (truck.vehicleState === "On Patrol" && $scope.onPatrolChecked) truckIsGood = true;
                if (truck.vehicleState === "On Incident" && $scope.onAssistChecked) truckIsGood = true;
                if (truck.vehicleState === "On Break" && $scope.onBreakLunchChecked) truckIsGood = true;

                //if ($scope.contractorNameFilter) {
                //    if ($scope.contractorNameFilter === truck.contractorName) truckIsGood = false;
                //}
                //else {
                //    truckIsGood = true;
                //}

                if (truckIsGood) filteredTrucks.push(truck);
            });

            $scope.trucks = filteredTrucks;

            console.log("%c Filtered Trucks %O", "color:red", $scope.trucks);
        }

        function getTrucks() {
            if ($scope.isBusyGettingTrucks) return;
            $scope.isBusyGettingTrucks = true;
            trucksService.getTrucks().then(function (rawTrucks) {
                $scope.isBusyGettingTrucks = false;
                //console.group("Truck Request");

                try {
                    //console.log("%c Raw Trucks %O (%s)", "color:green", rawTrucks, new Date());

                    rawTrucks.forEach(function (rawTruck) {
                        var existingTruck = utilService.findArrayElement($scope.trucks, "truckNumber", rawTruck.TruckNumber);
                        if (!existingTruck) $scope.trucks.push(new $rootScope.mtcTruck(rawTruck));
                        else existingTruck.update(rawTruck);
                    });

                    //console.log("%c Trucks %O", "color:blue", $scope.trucks);

                } catch (e) {

                }

                //console.groupEnd();

                //console.group("Map Trucks");

                try {

                    filterTrucks();
                    drawTruckMarkers();
                    cleanupTruckMarkers();

                    if ($scope.truckToBeFollowed && $scope.truckToBeFollowed.lat && $scope.truckToBeFollowed.lon) {
                        updateMap(new google.maps.LatLng($scope.truckToBeFollowed.lat, $scope.truckToBeFollowed.lon), ZOOM_15);
                    }
                } catch (e) {

                }

                //console.groupEnd();
            });
        }

        function drawTruckMarkers() {
            $scope.trucks.forEach(function (truck) {
                setTruckMarker(truck);
            });
        }

        function cleanupTruckMarkers() {
            //console.group("Remove Trucks");
            $scope.truckMarkers.forEach(function (truckMarker) {
                var truck = utilService.findArrayElement($scope.trucks, "id", truckMarker.id);
                if (!truck) {
                    //console.log("Removing truck marker for %s", truckMarker.id);
                    truckMarker.setMap(null);
                }
            });
            //console.groupEnd();
        }

        function buildDetailsContent(truck) {
            var content = "<table>";
            content += "<tr>";
            content += "<td>Last Msg </td>";
            content += "<td><strong>" + truck.lastMessage + "</strong></td>";
            content += "</tr>";
            content += "<tr>";
            content += "<td>Beat </td>";
            content += "<td><strong>" + truck.beatNumber + "</strong></td>";
            content += "</tr>";
            content += "<td>Driver </td>";
            content += "<td><strong>" + truck.driverName + "</strong></td>";
            content += "</tr>";
            content += "<tr>";
            content += "<td>Truck Number </td>";
            content += "<td><strong>" + truck.truckNumber + "</strong></td>";
            content += "</tr>";
            content += "<tr>";
            content += "<td>Contractor </td>";
            content += "<td><strong>" + truck.contractorName + "</strong></td>";
            content += "</tr>";
            content += "<tr>";
            content += "<td>Status </td>";
            content += "<td><strong>" + truck.vehicleState + "</strong></td>";
            content += "</tr>";
            content += "<tr>";
            content += "<td>Speed </td>";
            content += "<td><strong>" + truck.speed + " mph</td>";
            content += "</tr>";

            content += "<tr>";
            content += "<td colspan='2' style='text-align:center; padding:5px;'>";
            content += "<div class='btn-group' role='group'>";
            content += "<button type='button' class='btn btn-info btn-sm' ng-click='follow(\"" + truck.id + "\")'>Follow</button>";
            content += "<button type='button' class='btn btn-primary btn-sm' ng-click='zoomTo(\"" + truck.id + "\")'>Zoom In</button>";
            content += "</div>";
            content += "</td>";
            content += "</tr>";
            content += "</table>";
            return content;
        }

        function setTruckMarker(truck) {

            var detailsContent = $compile(buildDetailsContent(truck))($scope);
            var truckMarker = utilService.findArrayElement($scope.truckMarkers, "id", truck.id);

            if (!truckMarker) {
                console.log("New marker for truck-id: %s - %O", truck.id, truck);
                truckMarker = new RichMarker({
                    id: truck.id,
                    map: $scope.map,
                    animation: google.maps.Animation.DROP,
                    draggable: false,
                    flat: true,
                    width: 35,
                    height: 35,
                    anchor: RichMarkerPosition.TOP,
                    detailsContent: detailsContent,
                    content: "<div style='cursor: pointer !important'><img id='truckIcon" + truck.id + "' src='" + truck.vehicleStateIconUrl + "' class='truckIcon' /></div>"
                });
                google.maps.event.addListener(truckMarker, 'click', (function (truckMarker, scope) {
                    return function () {
                        scope.selectedTruckMarker = truckMarker;
                        scope.infowindow.setContent(truckMarker.detailsContent[0]);
                        scope.infowindow.open(scope.map, truckMarker);
                    };
                })(truckMarker, $scope));
                $scope.truckMarkers.push(truckMarker);
            }
            truckMarker.detailsContent = detailsContent;
            if (truckMarker.map === null) truckMarker.setMap($scope.map);

            var latlng = new google.maps.LatLng(truck.lat, truck.lon);
            truckMarker.setPosition(latlng);

            var truckMarkerText = truck.beatNumberString;
            if (truckMarkerText === 'Not Assigned' || truckMarkerText === 'Not set')
                truckMarkerText = truck.id;

            var truckMarkerRichContent = "<img id='truckIcon" + truck.id + "' src='" + truck.vehicleStateIconUrl + "' class='truckIcon' />" +
                "<span class='truckIconText'>" + truckMarkerText + "</span>";

            truckMarker.setContent(truckMarkerRichContent);

            $("#truckIcon" + truck.id).rotate({
                angle: truck.heading - 90
            });

            //update info content with latest TRUCK values
            if ($scope.selectedTruckMarker && $scope.selectedTruckMarker.id === truckMarker.id && truckMarker.detailsContent) {
                console.log("Infowindow requsted for: %s", truckMarker.id);
                $scope.infowindow.setContent(truckMarker.detailsContent[0]);
            }
        }

        function removeAllMapEvents() {
            google.maps.event.clearListeners($scope.map, 'dblclick');
            google.maps.event.clearListeners($scope.map, 'click');
            $scope.polygons.forEach(function (polygon) {
                polygon.setEditable(false);
                google.maps.event.clearListeners(polygon, 'dblclick');
            });
        }

        function initMap() {
            console.log("Initializing Map...");

            var defaultMapLocation = new google.maps.LatLng(DEFAULT_MAP_CENTER_LAT, DEFAULT_MAP_CENTER_LON);

            var mapOptions = {
                center: defaultMapLocation,
                zoom: ZOOM_11,
                mapTypeId: google.maps.MapTypeId.ROADMAP //ROADMAP //SATELLITE //HYBRID //TERRAIN
            };

            var mapElement = document.getElementById('googleMap');

            $scope.map = new google.maps.Map(mapElement, mapOptions);
            google.maps.event.trigger($scope.map, "resize");

            sizeMap();
            setMapControls();
        };

        //user actions
        $scope.follow = function (truckId) {
            console.log("FOLLOW truck: %s", truckId);
            if (!truckId) return;
            $scope.truckToBeFollowed = utilService.findArrayElement($scope.trucks, "id", truckId);
            if (!$scope.truckToBeFollowed) return;

            updateMap(new google.maps.LatLng($scope.truckToBeFollowed.lat, $scope.truckToBeFollowed.lon), ZOOM_15);
        };

        $scope.stopFollow = function () {
            $scope.truckToBeFollowed = "";
            updateMap(new google.maps.LatLng(DEFAULT_MAP_CENTER_LAT, DEFAULT_MAP_CENTER_LON), ZOOM_11);
        };

        $scope.zoomTo = function (truckId) {
            console.log("ZOOM truck: %s", truckId);
            if (!truckId) return;
            var truckToBeZoomedTo = utilService.findArrayElement($scope.trucks, "id", truckId);
            if (!truckToBeZoomedTo) return;

            updateMap(new google.maps.LatLng(truckToBeZoomedTo.lat, truckToBeZoomedTo.lon), ZOOM_14);
        };

        $scope.filter = function () {
            console.log("Filter request");
            $scope.filterApplied = true;
            filterTrucks();
            drawTruckMarkers();
            cleanupTruckMarkers();
        };

        $scope.clearFilter = function () {
            console.log("Clear Filter");

            $scope.filterApplied = false;
            $scope.allFiltersChecked = true;
            $scope.onPatrolChecked = true;
            $scope.driverLoggedOnChecked = true;
            $scope.onAssistChecked = true;
            $scope.onBreakLunchChecked = true;
            $scope.onRollOutInChecked = true;
            $scope.notLoggedOnChecked = true;
            $scope.contractorNameFilter = "";

            getTrucks();
        };

        $('#segments').on('show.bs.collapse', function () {
            console.log("segments visible");
            $scope.hideMapData();
            $scope.resetMap();
            setTimeout(function () {
                $scope.segmentsVisible = true;
                $scope.$apply();
            }, 250);
        });

        $('#segments').on('hidden.bs.collapse', function () {
            console.log("segments invisible");
            $scope.segmentsVisible = false;
            $scope.$apply();
            $scope.hideMapData();
            $scope.resetMap();
        });

        $('#beats').on('show.bs.collapse', function () {
            console.log("beats visible");
            $scope.hideMapData();
            $scope.resetMap();
            setTimeout(function () {
                $scope.beatsVisible = true;
                $scope.$apply();
            }, 250);
        });

        $('#beats').on('hidden.bs.collapse', function () {
            console.log("beats invisible");
            $scope.beatsVisible = false;
            $scope.$apply();
            $scope.hideMapData();
            $scope.resetMap();
        });

        $('#towTruckYards').on('show.bs.collapse', function () {
            console.log("towTruckYards visible");
            $scope.hideMapData();
            $scope.resetMap();
            setTimeout(function () {
                $scope.towTruckYardsVisible = true;
                $scope.$apply();
            }, 250);
        });

        $('#towTruckYards').on('hidden.bs.collapse', function () {
            console.log("towTruckYards invisible");
            $scope.towTruckYardsVisible = false;
            $scope.$apply();
            $scope.hideMapData();
            $scope.resetMap();
        });

        $scope.resetMap = function () {
            console.log("resetMap");
            removeAllMapEvents();
            updateMap(new google.maps.LatLng(DEFAULT_MAP_CENTER_LAT, DEFAULT_MAP_CENTER_LON), ZOOM_11);
        };

        $scope.displayMapData = function (polygons, markers) {
            console.log("displayMapData, number of polygons: %i, number of markers: %i", polygons.length, markers.length);

            if (polygons) {
                $scope.polygons = polygons;
                $scope.polygons.forEach(function (polygon) {
                    polygon.setMap($scope.map);
                });
            }
            if (markers) {
                $scope.markers = markers;
                $scope.markers.forEach(function (marker) {
                    marker.setMap($scope.map);
                });
                //markerClusterer = new MarkerClusterer($scope.map, $scope.markers, markerClusterOptions);
            }
        };

        $scope.hideMapData = function () {
            console.log("hideMapData");
            removeAllMapEvents();
            $scope.polygons.forEach(function (polygon) {
                polygon.setMap(null);
            });
            $scope.markers.forEach(function (marker) {
                marker.setMap(null);
            });
            // if (markerClusterer !== null && markerClusterer !== undefined)
            //     markerClusterer.clearMarkers();

            $scope.polygons = [];
            $scope.markers = [];
            $scope.selectedPolygon = {};
        };

        $scope.setMapLocation = function (lat, lon, zoom) {
            console.log("setMapLocation");
            updateMap(new google.maps.LatLng(lat, lon), zoom);
        };

        $scope.setEditPolygon = function (id) {
            console.log("setEditPolygon");
            $scope.selectedPolygon = utilService.findArrayElement($scope.polygons, "id", id);
            if (!$scope.selectedPolygon) return;

            google.maps.event.addListener($scope.selectedPolygon, 'dblclick', function (e) {
                if (e.vertex === undefined) {
                    return;
                }
                $scope.selectedPolygon.getPath().removeAt(e.vertex);
            });
            google.maps.event.addListener($scope.map, 'click', function (e) {
                var path = $scope.selectedPolygon.getPath();
                if (path === undefined)
                    path = [];
                path.push(e.latLng);
            });

            $scope.selectedPolygon.setEditable(true);
            $scope.selectedPolygon.setOptions({
                strokeColor: IS_EDITING_COLOR,
                fillColor: IS_EDITING_COLOR
            });
        };

        $scope.setCancelEditPolygon = function (id, color) {
            console.log("setCancelEditPolygon");
            $scope.selectedPolygon = utilService.findArrayElement($scope.polygons, "id", id);
            if (!$scope.selectedPolygon) return;
            $scope.selectedPolygon.setEditable(false);
            $scope.selectedPolygon.setOptions({
                strokeColor: color,
                fillColor: color
            });
            var polygon = $scope.selectedPolygon;
            removeAllMapEvents();
        };

        $scope.setNewPolygon = function (color) {
            console.log("setNewPolygon");
            $scope.hideMapData();
            $scope.selectedPolygon = {};

            $scope.selectedPolygon = new google.maps.Polygon({
                strokeColor: color,
                strokeOpacity: 0.8,
                strokeWeight: 2,
                fillColor: color,
                fillOpacity: 0.35,
                editable: true
            });
            $scope.selectedPolygon.setMap($scope.map);

            google.maps.event.addListener($scope.selectedPolygon, 'dblclick', function (e) {
                if (e.vertex === undefined) {
                    return;
                }
                $scope.selectedPolygon.getPath().removeAt(e.vertex);
            });
            google.maps.event.addListener($scope.map, 'click', function (e) {
                var path = $scope.selectedPolygon.getPath();
                path.push(e.latLng);
            });

            $scope.polygons.push($scope.selectedPolygon);
        };

        $scope.makeAllPolygonsUneditable = function () {
            console.log("makeAllPolygonsUneditable");
            $scope.polygons.forEach(function (polygon) {
                polygon.setEditable(false);
            });
        };

        angular.element($window).bind('resize', function () {
            sizeMap();
        });

        initMap();
        checkForAlarms();
        getTruckRefreshRate();

    }
}());