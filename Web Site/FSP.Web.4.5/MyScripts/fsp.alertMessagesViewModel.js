﻿/// <reference path="../Scripts/knockout-2.1.0.js" />
/// <reference path="fsp.constructor.js" />
/// <reference path="fsp.truckCollection.js" />
/// <reference path="../Scripts/knockout-2.2.1.js" />

lata.FspWeb.prototype.dispatchViewModel = function () {

    var self = this;

    self.message = ko.observable('');
    self.resultingBeats = ko.observableArray([]);
    self.currentSort = ko.observable("beatNumber");
    self.currentSortDirection = ko.observable("Asc");
    self.canSendTrucks = ko.observable(false);
    self.isSorting = false;

    self.columns = ko.observableArray([
        new column(self, "Beat #", "beatNumber", true),
        new column(self, "Truck #", "truckNumber", true),
        new column(self, "Driver", "driverName", true),
        new column(self, "Contractor", "contractorName", true),
        new column(self, "Location", "location", true),
        new column(self, "Status", "status", true),
        new column(self, "Last Status Change", "lastStatusChanged", true),

        new column(self, "Current Speed", "speed", false),
        new column(self, "Max Speed", "speedingValue", false),
        new column(self, "Speeding Time", "speedingTime", false),
        new column(self, "Heading", "headingText", false),
        new column(self, "Out of Bounds Message", "outOfBoundsMessage", false),
        new column(self, "Out of Bounds Time", "outOfBoundsTime", false),
        new column(self, "Last Update", "lastMessage", false)
    ]);

    var initDispatch = function () {

        ////truck collection file
        fspWeb.startTruckService();

        //handle truck collection change
        fspWeb.trucksChanged.subscribe(function () {
            //alert('Truck collection changed');    

            if (self.isSorting === false)
                self.doSort();
        });

    }

    self.clearBeats = function () {

        self.fspDirection('');
        self.fspFreeway('');
        self.fspLocation('');
        self.fspCrossStreet('');
        self.comments('');
        self.resultingBeats([]);

        self.canSendTrucks(false);
    }

    self.toggleTruckSelection1 = function (item) {
        var truckIsSelected = ko.utils.arrayFirst(fspWeb.selectedTrucks(), function (i) { return i.truckNumber === item.truckNumber; });
        if (!truckIsSelected) {
            $("#avail" + item.id).addClass('highlight');
            fspWeb.selectedTrucks.push(item);
        }
        else {
            $("#avail" + item.id).removeClass('highlight');
            fspWeb.selectedTrucks.remove(function (j) { return j.truckNumber === item.truckNumber; });
        }
    }
    self.toggleTruckSelection2 = function (item) {
        var truckIsSelected = ko.utils.arrayFirst(fspWeb.selectedTrucks(), function (i) { return i.truckNumber === item.truckNumber; });
        if (!truckIsSelected) {
            $("#loggedOn" + item.id).addClass('highlight');
            fspWeb.selectedTrucks.push(item);
        }
        else {
            $("#loggedOn" + item.id).removeClass('highlight');
            fspWeb.selectedTrucks.remove(function (j) { return j.truckNumber === item.truckNumber; });
        }
    }
    self.toggleTruckSelection3 = function (item) {
        var truckIsSelected = ko.utils.arrayFirst(fspWeb.selectedTrucks(), function (i) { return i.truckNumber === item.truckNumber; });
        if (!truckIsSelected) {
            $("#notLoggedOn" + item.id).addClass('highlight');
            fspWeb.selectedTrucks.push(item);
        }
        else {
            $("#notLoggedOn" + item.id).removeClass('highlight');
            fspWeb.selectedTrucks.remove(function (j) { return j.truckNumber === item.truckNumber; });
        }
    }

    self.unSelectTruck = function (item) {

        var truckIsSelected = ko.utils.arrayFirst(fspWeb.selectedTrucks(), function (i) { return i.truckNumber === item.truckNumber; });
        if (truckIsSelected) {
            $("#avail" + item.id).removeClass('highlight');
            $("#loggedOn" + item.id).removeClass('highlight');
            $("#notLoggedOn" + item.id).removeClass('highlight');
            fspWeb.selectedTrucks.remove(function (j) { return j.truckNumber === item.truckNumber; });
        }
    }

    self.sendAlertMessage = function () {

        var url = fspWeb.SERVICE_BASE_URL + "/AlertMessages/SendAlertMessage";

        $.ajax({
            url: url,
            type: "POST",
            dataType: "json",
            data: {
                alertMessage: self.message(),
                selectedTrucks: JSON.stringify(fspWeb.selectedTrucks())
            },
            error: function (xhr, ajaxOptions, thrownError) {
                alert(xhr.status);
                alert(thrownError);
            },
            success: function (value) {

                if (value === true)
                    alert('Alert message successfully sent.');
                else
                    alert('Error sending alert message.');

                for (var i = 0; i < fspWeb.selectedTrucks().length; i++) {
                    var selectedTruck = fspWeb.selectedTrucks()[i];

                    $("#avail" + selectedTruck.id).removeClass('highlight');
                    $("#loggedOn" + selectedTruck.id).removeClass('highlight');
                    $("#notLoggedOn" + selectedTruck.id).removeClass('highlight');

                }
                fspWeb.selectedTrucks([]);
                self.message('');
            }
        });

    }

    //show Config
    self.showConfig = function (item) {
        try {

            $("#configModal").modal('show');

        } catch (e) {

        }
    }

    self.allSelected = ko.observable(false);
    self.selectAll = function () {

        for (var i = 0; i < fspWeb.trucks().length; i++) {

            try {
                var item = fspWeb.trucks()[i];

                var truckIsSelected = ko.utils.arrayFirst(fspWeb.selectedTrucks(), function (i) { return i.truckNumber === item.truckNumber; });
                if (!truckIsSelected && item.vehicleState() != 'Waiting for Driver Login') {
                    $("#avail" + item.id).addClass('highlight');
                    $("#loggedOn" + item.id).addClass('highlight');

                    fspWeb.selectedTrucks.push(item);
                }
            } catch (e) {

            }

        }

        self.allSelected(true);

    };

    self.removeAll = function () {
        fspWeb.selectedTrucks([]);
        self.allSelected(false);

        for (var i = 0; i < fspWeb.trucks().length; i++) {
            var item = fspWeb.trucks()[i];
            $("#avail" + item.id).removeClass('highlight');
            $("#loggedOn" + item.id).removeClass('highlight');
        }


    };

    showAssistsList = function () {
        try {
            var url = fspWeb.SERVICE_BASE_URL + "/Assist/Index";;
            var windowName = "Assists";
            var windowSize = 'width=1000,height=800';
            window.open(url, '', windowSize);
            event.preventDefault();
        } catch (e) {

        }

    }

    //column
    function column(root, name, value, isVisible) {
        var self = this;

        self.name = ko.observable(name);
        self.value = ko.observable(value);
        self.isVisible = ko.observable(isVisible);

        self.sort = function (item) {

            root.currentSort(item.value());

            if (root.currentSortDirection() === 'Asc')
                root.currentSortDirection('Desc');
            else
                root.currentSortDirection('Asc');

            doSort();

        };
    }

    doSort = function () {

        self.isSorting = true;


        if (currentSort() === 'truckNumber') {
            if (currentSortDirection() === 'Asc')
                fspWeb.trucks.sort(function (left, right) { return left.truckNumber == right.truckNumber ? 0 : (left.truckNumber < right.truckNumber ? -1 : 1) })
            else
                fspWeb.trucks.sort(function (left, right) { return left.truckNumber == right.truckNumber ? 0 : (left.truckNumber > right.truckNumber ? -1 : 1) })
        }
        else if (currentSort() === 'beatNumber') {
            if (currentSortDirection() === 'Asc')
                fspWeb.trucks.sort(function (left, right) { return left.beatNumber() == right.beatNumber() ? 0 : (left.beatNumber() < right.beatNumber() ? -1 : 1) })
            else
                fspWeb.trucks.sort(function (left, right) { return left.beatNumber() == right.beatNumber() ? 0 : (left.beatNumber() > right.beatNumber() ? -1 : 1) })
        }
        else if (currentSort() === 'driverName') {
            if (currentSortDirection() === 'Asc')
                fspWeb.trucks.sort(function (left, right) { return left.driverName() == right.driverName() ? 0 : (left.driverName() < right.driverName() ? -1 : 1) })
            else
                fspWeb.trucks.sort(function (left, right) { return left.driverName() == right.driverName() ? 0 : (left.driverName() > right.driverName() ? -1 : 1) })
        }
        else if (currentSort() === 'contractorName') {
            if (currentSortDirection() === 'Asc')
                fspWeb.trucks.sort(function (left, right) { return left.contractorName() == right.contractorName() ? 0 : (left.contractorName() < right.contractorName() ? -1 : 1) })
            else
                fspWeb.trucks.sort(function (left, right) { return left.contractorName() == right.contractorName() ? 0 : (left.contractorName() > right.contractorName() ? -1 : 1) })
        }
        else if (currentSort() === 'location') {
            if (currentSortDirection() === 'Asc')
                fspWeb.trucks.sort(function (left, right) { return left.location() == right.location() ? 0 : (left.location() < right.location() ? -1 : 1) })
            else
                fspWeb.trucks.sort(function (left, right) { return left.location() == right.location() ? 0 : (left.location() > right.location() ? -1 : 1) })
        }
        else if (currentSort() === 'status') {
            if (currentSortDirection() === 'Asc')
                fspWeb.trucks.sort(function (left, right) { return left.vehicleState() == right.vehicleState() ? 0 : (left.vehicleState() < right.vehicleState() ? -1 : 1) })
            else
                fspWeb.trucks.sort(function (left, right) { return left.vehicleState() == right.vehicleState() ? 0 : (left.vehicleState() > right.vehicleState() ? -1 : 1) })
        }
        else if (currentSort() === 'speed') {
            if (currentSortDirection() === 'Asc')
                fspWeb.trucks.sort(function (left, right) { return left.speed() == right.speed() ? 0 : (left.speed() < right.speed() ? -1 : 1) })
            else
                fspWeb.trucks.sort(function (left, right) { return left.speed() == right.speed() ? 0 : (left.speed() > right.speed() ? -1 : 1) })
        }
        else if (currentSort() === 'lastUpdate') {
            if (currentSortDirection() === 'Asc')
                fspWeb.trucks.sort(function (left, right) { return left.lastUpdate() == right.lastUpdate() ? 0 : (left.lastUpdate() < right.lastUpdate() ? -1 : 1) })
            else
                fspWeb.trucks.sort(function (left, right) { return left.lastUpdate() == right.lastUpdate() ? 0 : (left.lastUpdate() > right.lastUpdate() ? -1 : 1) })
        }

        else if (currentSort() === 'speedingTime') {
            if (currentSortDirection() === 'Asc')
                fspWeb.trucks.sort(function (left, right) { return left.speedingTime() == right.speedingTime() ? 0 : (left.speedingTime() < right.speedingTime() ? -1 : 1) })
            else
                fspWeb.trucks.sort(function (left, right) { return left.speedingTime() == right.speedingTime() ? 0 : (left.speedingTime() > right.speedingTime() ? -1 : 1) })
        }
        else if (currentSort() === 'speedingValue') {
            if (currentSortDirection() === 'Asc')
                fspWeb.trucks.sort(function (left, right) { return left.speedingValue() == right.speedingValue() ? 0 : (left.speedingValue() < right.speedingValue() ? -1 : 1) })
            else
                fspWeb.trucks.sort(function (left, right) { return left.speedingValue() == right.speedingValue() ? 0 : (left.speedingValue() > right.speedingValue() ? -1 : 1) })
        }
        else if (currentSort() === 'outOfBoundsMessage') {
            if (currentSortDirection() === 'Asc')
                fspWeb.trucks.sort(function (left, right) { return left.outOfBoundsMessage() == right.outOfBoundsMessage() ? 0 : (left.outOfBoundsMessage() < right.outOfBoundsMessage() ? -1 : 1) })
            else
                fspWeb.trucks.sort(function (left, right) { return left.outOfBoundsMessage() == right.outOfBoundsMessage() ? 0 : (left.outOfBoundsMessage() > right.outOfBoundsMessage() ? -1 : 1) })
        }
        else if (currentSort() === 'outOfBoundsTime') {
            if (currentSortDirection() === 'Asc')
                fspWeb.trucks.sort(function (left, right) { return left.outOfBoundsTime() == right.outOfBoundsTime() ? 0 : (left.outOfBoundsTime() < right.outOfBoundsTime() ? -1 : 1) })
            else
                fspWeb.trucks.sort(function (left, right) { return left.outOfBoundsTime() == right.outOfBoundsTime() ? 0 : (left.outOfBoundsTime() > right.outOfBoundsTime() ? -1 : 1) })
        }

        self.isSorting = false;

    }

    return {
        initDispatch: initDispatch
    };

}();

$(function () {

    fspWeb = new lata.FspWeb();

    //apply binding to ko
    ko.applyBindings(fspWeb);

    //init
    fspWeb.dispatchViewModel.initDispatch();

});


