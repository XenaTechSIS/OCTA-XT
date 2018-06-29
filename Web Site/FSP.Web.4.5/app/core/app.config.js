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
      $rootScope.applicationName = "OCTA Web 1.0";
      $rootScope.rootUrl = $(".websiteUrl").text().trim();
      $rootScope.pageConfig = {};
      $rootScope.mtcTruck = function (dbTruck) {
         var self = this;
         self.id = "";
         self.truckNumber = "";
         self.ipAddress = "";
         self.beatNumber = "";

         self.update = function (dbTruck) {
            //console.log("Update Truck %O", dbTruck);
            self.truckNumberOrginal = dbTruck.TruckNumber;
            if (dbTruck.TruckNumber !== null) {
               self.id = dbTruck.TruckNumber.replace(' ', '').replace('-', '').replace('_', '');
               self.truckNumber = dbTruck.TruckNumber;
            }
            self.ipAddress = dbTruck.IPAddress;
            self.vehicleStateIconUrl = $(".websiteUrl").text().trim() + '/Images/' + dbTruck.VehicleStateIconUrl;
            self.vehicleState = dbTruck.VehicleState;

            if (dbTruck.BeatNumber !== null) {
               self.beatNumber = dbTruck.BeatNumber.substring(dbTruck.BeatNumber.indexOf("-") + 1);
            }
            self.beatNumberString = self.beatNumber;
            self.beatSegmentNumber = dbTruck.BeatSegmentNumber;

            self.contractorId = dbTruck.ContractorId;
            self.contractorName = dbTruck.ContractorName;

            //location
            self.heading = dbTruck.Heading;
            self.lat = dbTruck.Lat;
            self.lon = dbTruck.Lon;
            self.speed = dbTruck.Speed;

            self.driverName = dbTruck.DriverName;
            self.location = dbTruck.Location;
            self.lastMessage = dbTruck.LastMessage;
            self.speedingTime = dbTruck.SpeedingTime;
            self.speedingValue = dbTruck.SpeedingValue;
            self.outOfBoundsMessage = dbTruck.OutOfBoundsMessage;
            self.outOfBoundsTime = dbTruck.OutOfBoundsTime;
            self.hasAlarm = dbTruck.HasAlarm;
         };

         self.update(dbTruck);
      };
   }
})();