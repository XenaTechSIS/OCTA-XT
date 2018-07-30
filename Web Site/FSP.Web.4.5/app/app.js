﻿var octaApp = angular.module('octaApp',
    [
        'ngRoute',
        'ngSanitize',
        'ui.bootstrap',
        'minicolors',
        'ngMask',
        'octaApp.filters',
        'octaApp.map'        
    ]);

$(function () {
    'use strict';

    toastr.options = {
        closeButton: false,
        debug: false,
        newestOnTop: true,
        progressBar: false,
        positionClass: "toast-bottom-right",
        preventDuplicates: true,
        onclick: null,
        showDuration: 300,
        hideDuration: 1000,
        timeOut: 5000,
        extendedTimeOut: 1000,
        showEasing: "swing",
        hideEasing: "linear",
        showMethod: "fadeIn",
        hideMethod: "fadeOut"
    };
});