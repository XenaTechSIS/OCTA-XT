(function () {
    'use strict';
    angular.module("octaApp").factory("utilService", utilService);

    function utilService() {
        return {
            findArrayElement: function (arr, propName, propValue) {
                for (var i = 0; i < arr.length; i++)
                    if (arr[i][propName] === propValue) return arr[i];

            },
            findArrayElements: function (arr, propName, propValue) {
                var returnValue = [];
                for (var i = 0; i < arr.length; i++)
                    if (arr[i][propName] === propValue) {
                        returnValue.push(arr[i]);
                    }
                return returnValue;
            },
            getPolygonCoords: function (polygon) {
                var coords = [];
                if (!polygon) return coords;

                var len = polygon.getPath().getLength();
                for (var i = 0; i < len; i++) {
                    var xy = polygon.getPath().getAt(i);
                    var coord = {                      
                        lat: xy.lat(),
                        lng: xy.lng()
                    };
                    coords.push(coord);
                }
                console.log('Polygon coords %O', coords);

                return coords;
            },
            clearPolygonCoords: function (polygon) {
                try {
                    if (!polygon) return;
                    var len = polygon.getPath().getLength();
                    for (var i = 0; i < len; i += 2) {
                        polygon.getPath().removeAt(i);
                    }
                } catch (e) {
                    console.log(e);
                }
            }
        };
    }
}());