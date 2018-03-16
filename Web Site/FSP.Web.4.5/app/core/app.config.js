(function () {
    "use strict";
    var app = angular.module('octaApp');
    app.config(['$httpProvider', appConfig]);
    app.run(['$rootScope', appRun]);

    function appConfig($httpProvider) {
        //this is for CORS operations        
        $httpProvider.defaults.useXDomain = true;
        delete $httpProvider.defaults.headers.common['X-Requested-With'];

        //disable IE ajax request caching
        $httpProvider.defaults.headers.common['If-Modified-Since'] = 'Mon, 26 Jul 1997 05:00:00 GMT';
        $httpProvider.defaults.headers.common['Cache-Control'] = 'no-cache';
        $httpProvider.defaults.headers.common['Pragma'] = 'no-cache';        
    }

    function appRun($rootScope) {
        $rootScope.applicationName = 'OCTA Web 1.0';
    }
})();