(function () {
    'use strict';
    angular.module("octaApp.map").controller("mapController", ['$scope', '$rootScope', '$window', mapController]);
    function mapController($scope, $rootScope, $window) {
        $scope.header = "Hello World!" + $rootScope.applicationName;

        var DEFAULT_MAP_CENTER_LAT = 33.739660;
        var DEFAULT_MAP_CENTER_LON = -117.832146;
        var ZOOM_9 = 9;

        $scope.map;

        function sizeMap() {
            var menuHeight = 100;
            var sideMenuWidth = 300;
            var wHeight = ($window.innerHeight - menuHeight) + 'px';
            var wWidth = ($window.innerWidth - sideMenuWidth) + 'px';
            $("#googleMap").height(wHeight);
            $("#googleMap").width(wWidth);

            $("#googleMapSideNavigation").height(wHeight);
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
            controlUI.addEventListener('click', function() {
                updateMap(new google.maps.LatLng(DEFAULT_MAP_CENTER_LAT, DEFAULT_MAP_CENTER_LON), ZOOM_9);
            });

            controlDiv.index = 1;
            $scope.map.controls[google.maps.ControlPosition.TOP_CENTER].push(controlDiv);

        }

        function updateMap(latlng, zoom) {
            $scope.map.panTo(latlng);
            $scope.map.setZoom(zoom);
        }

        $scope.initMap = function () {
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

        angular.element($window).bind('resize', function() {
            sizeMap();
        });

        $scope.initMap();

    }
}());