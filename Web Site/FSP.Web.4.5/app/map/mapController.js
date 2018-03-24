(function () {
    'use strict';
    angular.module("octaApp.map").controller("mapController", ['$scope', '$rootScope', '$window', '$interval', '$compile', 'trucksService', 'utilService', mapController]);
    function mapController($scope, $rootScope, $window, $interval, $compile, trucksService, utilService) {
        $scope.header = "Hello World!" + $rootScope.rootUrl;

        console.log($scope.header);
            
        var DEFAULT_MAP_CENTER_LAT = 33.739660;
        var DEFAULT_MAP_CENTER_LON = -117.832146;
        var ZOOM_9 = 9;
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

        $scope.trucks = [];
        $scope.truckMarkers = [];
        $scope.selectedTruck = "";
        $scope.selectedTruckMarker = "";
        $scope.truckToBeFollowed = "";
        $scope.polygons = [];
        $scope.selectedPolygon = {};

        $scope.haveAlarms = false;

        //filter
        $scope.allFiltersChecked = true;
        $scope.onPatrolChecked = true;
        $scope.driverLoggedOnChecked = true;
        $scope.onAssistChecked = true;
        $scope.onBreakLunchCheched = true;
        $scope.onRollOutInChecked = true;
        $scope.notLoggedOnChecked = true;
        $scope.contractorNameFilter = "";
        $scope.toggleAllFilters = function () {
            console.log("Toggle all filters %s", $scope.allFiltersChecked);
            $scope.onPatrolChecked = $scope.allFiltersChecked;
            $scope.driverLoggedOnChecked = $scope.allFiltersChecked;
            $scope.onAssistChecked = $scope.allFiltersChecked;
            $scope.onBreakLunchCheched = $scope.allFiltersChecked;
            $scope.onRollOutInChecked = $scope.allFiltersChecked;
            $scope.notLoggedOnChecked = $scope.allFiltersChecked;
        };
        $scope.clearContractorName = function () {
            $scope.contractorNameFilter = "";
        }


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
                updateMap(new google.maps.LatLng(DEFAULT_MAP_CENTER_LAT, DEFAULT_MAP_CENTER_LON), ZOOM_9);
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
                $scope.haveAlarms = result;
                console.log("Have alarms? %s", $scope.haveAlarms);

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

        function getTrucks() {
            console.log("Getting trucks...");
            trucksService.getTrucks().then(function (results) {
                console.log("Trucks found: %O", results);
                for (var i = 0; i < results.length; i++) {
                    var exists = false;
                    for (var ii = 0; ii < $scope.trucks.length; ii++) {
                        if ($scope.trucks[ii].id === results[i].Id) {
                            exists = true;
                            $scope.trucks[ii].update(results[i]);
                        }
                    }
                    if (!exists) $scope.trucks.push(new $rootScope.mtcTruck(results[i]));
                }
                drawTruckMarkers();
                cleanupTruckMarkers();
                if ($scope.truckToBeFollowed) {
                    updateMap(new google.maps.LatLng($scope.truckToBeFollowed.lat, $scope.truckToBeFollowed.lon), ZOOM_14);
                }
            });
        }

        function drawTruckMarkers() {
            $scope.trucks.forEach(function (truck) {
                setTruckMarker(truck);
            });
        }

        function cleanupTruckMarkers() {
            for (var i = 0; i < $scope.truckMarkers.length; i++) {
                var truckMarker = $scope.truckMarkers[i];
                var truck = utilService.findArrayElement($scope.trucks, "id", truckMarker.id);
                if (!truck) {
                    truckMarker.setMap(null);
                }
            }
        }

        function buildDetailsContent(truck) {
            var content = "<table>";
            content += "<tr>";
            content += "<td>Last Msg:</td>";
            content += "<td><strong>" + truck.lastMessage + "</strong></td>";
            content += "</tr>";
            content += "<tr>";
            content += "<td>Beat:</td>";
            content += "<td><strong>" + truck.beatNumber + "</strong></td>";
            content += "</tr>";
            content += "<td>Driver:</td>";
            content += "<td><strong>" + truck.driverName + "</strong></td>";
            content += "</tr>";
            content += "<tr>";
            content += "<td>Truck Number:</td>";
            content += "<td><strong>" + truck.truckNumber + "</strong></td>";
            content += "</tr>";
            content += "<tr>";
            content += "<td>Contractor:</td>";
            content += "<td><strong>" + truck.contractorName + "</strong></td>";
            content += "</tr>";
            content += "<tr>";
            content += "<td>Status:</td>";
            content += "<td><strong>" + truck.vehicleState + "</strong></td>";
            content += "</tr>";
            content += "<tr>";
            content += "<td>Speed:</td>";
            content += "<td><strong>" + truck.speed + "mph</td>";
            content += "</tr>";
            content += "<tr>";
            content += "<td><button ng-click='follow(" + truck.truckNumber + ")'>Follow</button></td>";
            content += "<td><button ng-click='zoomTo(" + truck.truckNumber + ")'>Zoom In</button></td>";
            content += "</tr>";
            content += "</table>";
            return content;
        }

        function setTruckMarker(truck) {

            var detailsContent = $compile(buildDetailsContent(truck))($scope);

            var truckMarker = utilService.findArrayElement($scope.truckMarkers, "id", truck.id);
            if (!truckMarker) {
                console.log("New truck marker %O", truck);
                truckMarker = new RichMarker({
                    id: truck.id,
                    map: $scope.map,
                    animation: google.maps.Animation.DROP,
                    draggable: false,
                    flat: true,
                    anchor: RichMarkerPosition.MIDDLE,
                    detailsContent: detailsContent,
                    content: "<div style='cursor: pointer'><img id='truckIcon" + truck.id + "' src='" + truck.vehicleStateIconUrl + "' class='truckIcon' /></div>"
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

            var latlng = new google.maps.LatLng(truck.lat, truck.lon);
            truckMarker.setPosition(latlng);

            var truckMarkerText = truck.beatNumberString;
            if (truckMarkerText === 'Not set')
                truckMarkerText = truck.id;

            var truckMarkerRichContent = "<img id='truckIcon" + truck.id + "' src='" + truck.vehicleStateIconUrl + "' class='truckIcon' />" +
                "<span class='truckIconText'>" + truckMarkerText + "</span>";

            truckMarker.setContent(truckMarkerRichContent);

            $("#truckIcon" + truck.id).rotate({
                angle: truck.heading - 90
            });

            if ($scope.selectedTruckMarker && $scope.selectedTruckMarker.id === truckMarker.id) {
                $scope.infowindow.setContent(truckMarker.detailsContent[0]);
            }

        }

        function initMap() {
            console.log("Initializing Map...");

            var defaultMapLocation = new google.maps.LatLng(DEFAULT_MAP_CENTER_LAT, DEFAULT_MAP_CENTER_LON);

            var mapOptions = {
                center: defaultMapLocation,
                zoom: ZOOM_9,
                mapTypeId: google.maps.MapTypeId.ROADMAP //ROADMAP //SATELLITE //HYBRID //TERRAIN
            };

            var mapElement = document.getElementById('googleMap');

            $scope.map = new google.maps.Map(mapElement, mapOptions);
            google.maps.event.trigger($scope.map, "resize");

            sizeMap();
            setMapControls();
        };

        //user actions
        $scope.follow = function (truckNumber) {
            if (!truckNumber) return;
            console.log("Follow %s", truckNumber);
            $scope.truckToBeFollowed = utilService.findArrayElement($scope.trucks, "truckNumber", truckNumber);
            if ($scope.truckToBeFollowed === undefined) return;

            updateMap(new google.maps.LatLng($scope.truckToBeFollowed.lat, $scope.truckToBeFollowed.lon), ZOOM_14);
        };

        $scope.zoomTo = function (truckNumber) {
            if (!truckNumber) return;
            console.log("Zoom To %s", truckNumber);
            var truckToBeZoomedTo = utilService.findArrayElement($scope.trucks, "truckNumber", truckNumber);
            if (truckToBeZoomedTo === undefined) return;

            updateMap(new google.maps.LatLng(truckToBeZoomedTo.lat, truckToBeZoomedTo.lon), ZOOM_14);
        };

        angular.element($window).bind('resize', function () {
            sizeMap();
        });

        initMap();
        checkForAlarms();
        getTruckRefreshRate();

    }
}());