(function () {
    'use strict';
    angular.module("octaApp.map").controller("mapController", ['$scope', '$rootScope', '$window', '$interval', 'trucksService', mapController]);
    function mapController($scope, $rootScope, $window, $interval, trucksService) {
        $scope.header = "Hello World!" + $rootScope.applicationName;

        var DEFAULT_MAP_CENTER_LAT = 33.739660;
        var DEFAULT_MAP_CENTER_LON = -117.832146;
        var ZOOM_9 = 9;

        var mapTopOffset = 95;
        var mapLeftOffset = 295;
        var refreshRate = 2000;

        $scope.map;
        $scope.trucks = [];
        $scope.haveAlarms = false;

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
                console.log(results);
                //for (var i = 0; i < results.length; i++) {
                //    var exists = false;
                //    for (var ii = 0; ii < $scope.trucks.length; ii++) {
                //        if ($scope.trucks[ii].ipAddress === results[i].IPAddress) {
                //            exists = true;
                //            $scope.trucks[ii].update(results[i]);
                //        }
                //    }
                //    if (!exists) $scope.trucks.push(new $rootScope.mtcTruck(results[i]));
                //}
                //drawTruckMarkers();
                //cleanupTruckMarkers();
                //if ($scope.truckToBeFollowed) {
                //    updateMap(new google.maps.LatLng($scope.truckToBeFollowed.lat, $scope.truckToBeFollowed.lon), ZOOM_14);
                //}
            });
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

        angular.element($window).bind('resize', function () {
            sizeMap();
        });

        initMap();
        checkForAlarms();
        getTruckRefreshRate();

    }
}());