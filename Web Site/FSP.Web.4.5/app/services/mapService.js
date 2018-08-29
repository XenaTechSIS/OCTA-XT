(function () {
   'use strict';
   angular.module("octaApp").factory("mapService", ["$http", mapService]);

   function mapService($http) {
      return {         
         getBeatPolygons: function () {
            return $http({
               method: 'GET',
               url: $(".websiteUrl").text().trim() + '/Map/GetBeatPolygons'
            }).
               then(function (response) {
                  return response.data;
               });
         },
         getSegmentPolygons: function () {
            return $http({
               method: 'GET',
               url: $(".websiteUrl").text().trim() + '/Map/GetSegmentPolygons'
            }).
               then(function (response) {
                  return response.data;
               });
         },
         getSegments: function () {
            return $http({
               method: 'GET',
               url: $(".websiteUrl").text().trim() + '/Map/GetSegments'
            }).
               then(function (response) {
                  return response.data;
               });
         },
         getYardPolygons: function () {
            return $http({
               method: 'GET',
               url: $(".websiteUrl").text().trim() + '/Map/GetYardPolygons'
            }).
               then(function (response) {
                  return response.data;
               });
         },
         getDropZonePolygons: function () {
            return $http({
               method: 'GET',
               url: $(".websiteUrl").text().trim() + '/Map/GetDropZonePolygons'
            }).
               then(function (response) {
                  return response.data;
               });
         },
         getCallBoxPolygons: function () {
            return $http({
               method: 'GET',
               url: $(".websiteUrl").text().trim() + '/Map/GetCallBoxPolygons'
            }).
               then(function (response) {
                  return response.data;
               });
         },

         saveYard: function (yard) {
            return $http({
               method: 'POST',
               data: yard,
               url: $(".websiteUrl").text().trim() + '/Map/SaveYardPolygon'
            }).
               then(function (response) {
                  return response.data;
               });
         },
         saveSegment: function (segment) {
            console.log("Save Segment %O", segment);
            return $http({
               method: 'POST',
               data: segment,
               url: $(".websiteUrl").text().trim() + '/Map/SaveSegmentPolygon'
            }).
               then(function (response) {
                  return response.data;
               });
         },
         saveBeat: function (beat) {
            return $http({
               method: 'POST',
               data: beat,
               url: $(".websiteUrl").text().trim() + '/Map/SaveBeatPolygon'
            }).
               then(function (response) {
                  return response.data;
               });
         },
         saveDropZone: function (dropZone) {
            return $http({
               method: 'POST',
               data: dropZone,
               url: $(".websiteUrl").text().trim() + '/Map/SaveDropZonePolygon'
            }).
               then(function (response) {
                  return response.data;
               });
         },
         saveCallBox: function (callBox) {
            console.log(callBox);
            return $http({
               method: 'POST',
               data: callBox,
               url: $(".websiteUrl").text().trim() + '/Map/SaveCallBoxPolygon'
            }).
               then(function (response) {
                  return response.data;
               });
         },

         deleteYard: function (yardId) {
            return $http({
               method: 'POST',
               data: {
                  id: yardId
               },
               url: $(".websiteUrl").text().trim() + '/Map/DeleteYard'
            }).
               then(function (response) {
                  return response.data;
               });
         },
         deleteSegment: function (segmentId) {
            return $http({
               method: 'POST',
               data: {
                  id: segmentId
               },
               url: $(".websiteUrl").text().trim() + '/Map/DeleteSegment'
            }).
               then(function (response) {
                  return response.data;
               });
         },
         deleteBeat: function (beatId) {
            return $http({
               method: 'POST',
               data: {
                  id: beatId
               },
               url: $(".websiteUrl").text().trim() + '/Map/DeleteBeat'
            }).
               then(function (response) {
                  return response.data;
               });
         },
         deleteDropZone: function (dropZoneId) {
            return $http({
               method: 'POST',
               data: {
                  id: dropZoneId
               },
               url: $(".websiteUrl").text().trim() + '/Map/DeleteDropZone'
            }).
               then(function (response) {
                  return response.data;
               });
         },
         deleteCallBox: function (callBoxId) {
            return $http({
               method: 'POST',
               data: {
                  id: callBoxId
               },
               url: $(".websiteUrl").text().trim() + '/Map/DeleteCallBox'
            }).
               then(function (response) {
                  return response.data;
               });
         }
      };
   }
}());