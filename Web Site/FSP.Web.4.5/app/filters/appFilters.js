(function () {
    "use strict";
    var appFilters = angular.module('octaApp.filters', []);

    appFilters.filter("datetimefilter", datetimefilter);
    function datetimefilter() {
        return function (input) {
            if (moment(input).isValid())
                return moment(input).format('MM/DD/YYYY hh:mm A');
            else
                return "";
        };
    }

    appFilters.filter("datetimeHourMinute", datetimeHourMinute);
    function datetimeHourMinute() {
        return function (input) {
            if (moment(input).isValid())
                return moment(input).format('HH:mm');
            else
                return "";
        };
    }

    appFilters.filter("decLatLonToDegreeLatLon", decLatLonToDegreeLatLon);
    function decLatLonToDegreeLatLon() {
        return function (input) {
            if (input) {
                var lat = eval(input.split("~")[0]);
                var lon = eval(input.split("~")[1]);
                var point = new GeoPoint(lon, lat);
                return point.getLatDeg() + ' ' + point.getLonDeg()
            }
            else
                return "";
        };
    }

})();