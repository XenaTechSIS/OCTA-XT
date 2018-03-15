lata.FspWeb.prototype.alertsViewModel = function () {

    var self = this;

    self.alerts = ko.observableArray([]);
    self.isBusy = ko.observable(false);
    self.excuseComments = ko.observable('');

    var alarmToBeExcused;
    var alarmToBeExcusedType;

    var init = function () {

        self.getAlerts();
        setTimeout(function updateAlerts() {
            console.log('Updating Alerts...');
            self.getAlerts();
            setTimeout(updateAlerts, 10000);
        }, 10000);

    };

    self.getAlerts = function () {

        self.isBusy(true);
        console.time('Gettings Alerts');

        $.ajax({
            url: $("#websitePath").attr("data-websitePath") + '/AlertMessages/GetAlerts',
            type: "GET",
            dataType: "json",
            success: function (dbAlerts) {
                console.timeEnd('Gettings Alerts');
                self.alerts([]);
                console.log(dbAlerts.length + ' alerts received');
                for (var i = 0; i < dbAlerts.length; i++) {

                    dbAlerts[i].LogOnAlarmTimeFormatted = moment(dbAlerts[i].LogOnAlarmTime).format('HH:mm');
                    dbAlerts[i].LogOnAlarmTime = moment(dbAlerts[i].LogOnAlarmTime).format('MM/DD/YY');
                    console.log('LogOn Alarm Excused: ' + moment(dbAlerts[i].LogOnAlarmExcused).format('MM/DD/YY'));
                    dbAlerts[i].IsLogOnAlarmExcused = moment(dbAlerts[i].LogOnAlarmExcused).format('MM/DD/YY') != '01/01/01';

                    dbAlerts[i].RollOutAlarmTimeFormatted = moment(dbAlerts[i].RollOutAlarmTime).format('HH:mm');
                    dbAlerts[i].RollOutAlarmTime = moment(dbAlerts[i].RollOutAlarmTime).format('MM/DD/YY');
                    console.log('Roll Out Excused: ' + moment(dbAlerts[i].RollOutAlarmExcused).format('MM/DD/YY'));
                    dbAlerts[i].IsRollOutAlarmExcused = moment(dbAlerts[i].RollOutAlarmExcused).format('MM/DD/YY') != '01/01/01';

                    dbAlerts[i].OnPatrolAlarmTimeFormatted = moment(dbAlerts[i].OnPatrolAlarmTime).format('HH:mm');
                    dbAlerts[i].OnPatrolAlarmTime = moment(dbAlerts[i].OnPatrolAlarmTime).format('MM/DD/YY');
                    console.log('On Patrol Excused: ' + moment(dbAlerts[i].OnPatrolAlarmExcused).format('MM/DD/YY'));
                    dbAlerts[i].IsOnPatrolAlarmExcused = moment(dbAlerts[i].OnPatrolAlarmExcused).format('MM/DD/YY') != '01/01/01';

                    dbAlerts[i].RollInAlarmTimeFormatted = moment(dbAlerts[i].RollInAlarmTime).format('HH:mm');
                    dbAlerts[i].RollInAlarmTime = moment(dbAlerts[i].RollInAlarmTime).format('MM/DD/YY');
                    console.log('Roll In Excused: ' + moment(dbAlerts[i].RollInAlarmExcused).format('MM/DD/YY'));
                    dbAlerts[i].IsRollInAlarmExcused = moment(dbAlerts[i].RollInAlarmExcused).format('MM/DD/YY') != '01/01/01';

                    dbAlerts[i].LogOffAlarmTimeFormatted = moment(dbAlerts[i].LogOffAlarmTime).format('HH:mm');
                    dbAlerts[i].LogOffAlarmTime = moment(dbAlerts[i].LogOffAlarmTime).format('MM/DD/YY');
                    console.log('Log Off Excused: ' + moment(dbAlerts[i].LogOffAlarmExcused).format('MM/DD/YY'));
                    dbAlerts[i].IsLogOffAlarmExcused = moment(dbAlerts[i].LogOffAlarmExcused).format('MM/DD/YY') != '01/01/01';

                    dbAlerts[i].SpeedingAlarmTimeFormatted = moment(dbAlerts[i].SpeedingTime).format('HH:mm');
                    dbAlerts[i].SpeedingAlarmTime = moment(dbAlerts[i].SpeedingTime).format('MM/DD/YY');                    
                    dbAlerts[i].IsSpeedingAlarmExcused = false;

                    dbAlerts[i].OutOfBoundsAlarmTimeFormatted = moment(dbAlerts[i].OutOfBoundsStartTime).format('HH:mm');
                    dbAlerts[i].OutOfBoundsAlarmTime = moment(dbAlerts[i].OutOfBoundsStartTime).format('MM/DD/YY');
                    console.log('Out of Bounds Excused: ' + moment(dbAlerts[i].OutOfBoundsExcused).format('MM/DD/YY'));
                    dbAlerts[i].IsOutOfBoundsAlarmExcused = moment(dbAlerts[i].OutOfBoundsExcused).format('MM/DD/YY') != '01/01/01';

                    dbAlerts[i].IncidentAlarmTimeFormatted = moment(dbAlerts[i].IncidentAlarmTime).format('HH:mm');
                    dbAlerts[i].IncidentAlarmTime = moment(dbAlerts[i].IncidentAlarmTime).format('MM/DD/YY');
                    console.log('Incident Excused: ' + moment(dbAlerts[i].IncidentAlarmExcused).format('MM/DD/YY'));
                    dbAlerts[i].IsIncidentAlarmExcused = moment(dbAlerts[i].IncidentAlarmExcused).format('MM/DD/YY') != '01/01/01';

                    dbAlerts[i].StationaryAlarmStartFormatted = moment(dbAlerts[i].StationaryAlarmStart).format('HH:mm');
                    dbAlerts[i].StationaryAlarmStart = moment(dbAlerts[i].StationaryAlarmStart).format('MM/DD/YY');
                    console.log('Stationary Excused: ' + moment(dbAlerts[i].StationaryAlarmExcused).format('MM/DD/YY'));
                    dbAlerts[i].IsStationaryAlarmExcused = moment(dbAlerts[i].StationaryAlarmExcused).format('MM/DD/YY') != '01/01/01';

                    dbAlerts[i].GPSIssueAlarmStartFormatted = moment(dbAlerts[i].GPSIssueAlarmStart).format('HH:mm');
                    dbAlerts[i].GPSIssueAlarmStart = moment(dbAlerts[i].GPSIssueAlarmStart).format('MM/DD/YY');
                    console.log('GPS Issue Excused: ' + moment(dbAlerts[i].GPSIssueAlarmExcused).format('MM/DD/YY'));
                    dbAlerts[i].IsGPSIssueAlarmExcused = moment(dbAlerts[i].GPSIssueAlarmExcused).format('MM/DD/YY') != '01/01/01';

                    console.log(dbAlerts[i]);

                    self.alerts.push(dbAlerts[i]);
                }

                self.isBusy(false);
            },
            error: function (xhr, ajaxOptions, thrownError) {
            }
        });

    };

    self.clearAlarm = function (alarmObject, alarmType) {
        var url = $("#websitePath").attr("data-websitePath") + '/AlertMessages/ClearAlarm?id=' + alarmObject.VehicleNumber + '&alarmType=' + alarmType;
        console.log('Clearing alarm ' + url);

        $.ajax({
            url: url,
            type: "GET",
            dataType: "json",
            success: function (result) {
                self.getAlerts();
                alert(result);
            },
            error: function (xhr, ajaxOptions, thrownError) {
            }
        });
    };

    self.excuseAlarm = function (alarmObject, alarmType) {

        alarmToBeExcused = alarmObject;
        alarmToBeExcusedType = alarmType;
        $("#excuseCommentsModal").modal('show');

    };

    self.cancelExcuse = function () {

        $("#excuseCommentsModal").modal('hide');
        alarmToBeExcused = null;
        alarmToBeExcusedType = null;
        self.excuseComments('');
    };

    self.excuseAlarmSubmit = function () {

        $("#excuseCommentsModal").modal('hide');
        var url = $("#websitePath").attr("data-websitePath") + '/AlertMessages/ExcuseAlarm?vehicleNumber=' + alarmToBeExcused.VehicleNumber + '&beatNumber=' + alarmToBeExcused.BeatNumber + '&alarmType=' + alarmToBeExcusedType + '&driverName=' + alarmToBeExcused.DriverName + '&comments=' + self.excuseComments();
        console.log('Excusing alarm ' + url);

        $.ajax({
            url: url,
            type: "GET",
            dataType: "json",
            success: function (result) {

                alarmToBeExcused = null;
                alarmToBeExcusedType = null;
                self.excuseComments('');

                self.getAlerts();
                alert(result);
            },
            error: function (xhr, ajaxOptions, thrownError) {
            }
        });

    };

    return {
        init: init
    }

}();

$(function () {

    fspWeb = new lata.FspWeb();

    //apply binding to ko
    ko.applyBindings(fspWeb);

    fspWeb.alertsViewModel.init();

});

