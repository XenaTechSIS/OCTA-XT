var ViewModel = function () {
    var self = this;

    self.beats = ko.observableArray([]);
    self.selectedBeat = ko.observable();

    self.drivers = ko.observableArray([]);
    self.selectedDriver = ko.observable();

    self.alarmTypes = ko.observableArray([]);
    self.selectedAlarmType = ko.observable();

    self.excuseTypes = ko.observableArray([{ 'Text': 'Excused', 'IsExcused': 'true' }, { 'Text': 'Not Excused', 'IsExcused': 'false' }]);
    self.selectedExcusedType = ko.observable();

    self.alarms = ko.observableArray([]);
    self.isBusy = ko.observable(false);

    self.selectedDate = ko.observable('');

    self.getAlarms = function () {

        self.isBusy(true);
        console.time('Gettings Alarms');

        var beat = '';
        var driver = '';
        var isExcused = '';

        if (self.selectedBeat() != undefined)
            beat = self.selectedBeat().Text;

        if (self.selectedDriver() != undefined)
            driver = self.selectedDriver().Text;

        if (self.selectedExcusedType() != undefined)
            isExcused = self.selectedExcusedType().IsExcused;

        $.ajax({
            url: $("#websitePath").attr("data-websitePath") + '/AlertMessages/GetAlarmHistory',
            type: "POST",
            dataType: "json",
            data: {
                beat: beat,
                driver: driver,
                date: self.selectedDate(),
                alarmType: self.selectedAlarmType(),
                isExcused: isExcused
            },
            success: function (results) {
                console.timeEnd('Gettings Alarms');
                self.alarms([]);
                for (var i = 0; i < results.length; i++) {
                    self.alarms.push(new alarm(self, results[i]));
                }

                self.isBusy(false);
            },
            error: function (xhr, ajaxOptions, thrownError) {
            }
        });

    };
    self.getAlarmTypes = function () {

        console.time('Gettings Alarm Types');

        $.ajax({
            url: $("#websitePath").attr("data-websitePath") + '/AlertMessages/GetAlarmTypes',
            type: "GET",
            dataType: "json",
            success: function (results) {
                console.timeEnd('Gettings Alarm Types');
                self.alarmTypes([]);
                for (var i = 0; i < results.length; i++) {
                    self.alarmTypes.push(results[i]);
                }

            },
            error: function (xhr, ajaxOptions, thrownError) {
            }
        });

    };
    //filter

    self.getBeats = function () {

        console.time('Gettings Beats');

        $.ajax({
            url: $("#websitePath").attr("data-websitePath") + '/AlertMessages/GetAllBeats',
            type: "GET",
            dataType: "json",
            success: function (results) {
                console.timeEnd('Gettings Beats');
                self.beats([]);
                for (var i = 0; i < results.length; i++) {
                    self.beats.push(results[i]);
                }

            },
            error: function (xhr, ajaxOptions, thrownError) {
            }
        });

    };
    self.getDrivers = function () {

        console.time('Gettings Drivers');

        $.ajax({
            url: $("#websitePath").attr("data-websitePath") + '/AlertMessages/GetAllDrivers',
            type: "GET",
            dataType: "json",
            success: function (results) {
                console.timeEnd('Gettings Drivers');
                self.drivers([]);
                for (var i = 0; i < results.length; i++) {
                    self.drivers.push(results[i]);
                }

            },
            error: function (xhr, ajaxOptions, thrownError) {
            }
        });

    };

    self.filter = function () {
        self.getAlarms();
    };
    self.reset = function () {

        self.selectedBeat(null);
        self.selectedDriver(null);
        self.selectedAlarmType(null);
        self.selectedDate(moment().format('MM/DD/YYYY'));

        self.getAlarms();

    };

    function alarm(root, dbAlarm) {

        var self = this;

        self.BeatNumber = dbAlarm.BeatNumber;
        self.ContractCompanyName = dbAlarm.ContractCompanyName;
        self.VehicleNumber = dbAlarm.VehicleNumber;
        self.DriverName = dbAlarm.DriverName;
        self.AlarmTime = moment(dbAlarm.AlarmTime).format('MM/DD/YY hh:mm A');
        self.AlarmType = dbAlarm.AlarmType;

        self.ExcuseTime = ko.observable(dbAlarm.ExcuseTime);

        var isExcused = false;

        if (dbAlarm.ExcuseTime.length > 0) {
            console.log(dbAlarm.ExcuseTime);
            isExcused = moment(dbAlarm.ExcuseTime).format('MM/DD/YY') != '01/01/01';
        }
        
        self.IsExcused = ko.observable(isExcused);
        self.Comments = ko.observable(dbAlarm.Comments);

        self.updateAlarm = function () {

            console.log('Excuse Time ' + self.ExcuseTime() + ' Comments ' + self.Comments());

            $.ajax({
                url: $("#websitePath").attr("data-websitePath") + '/AlertMessages/UpdateAlarm',
                type: "POST",
                dataType: "json",
                data: {
                    BeatNumber: self.BeatNumber,
                    ContractCompanyName: self.ContractCompanyName,
                    VehicleNumber: self.VehicleNumber,
                    DriverName: self.DriverName,
                    AlarmTime: self.AlarmTime,
                    AlarmType: self.AlarmType,
                    Comments: self.Comments(),
                    IsExcused: self.IsExcused()
                },
                success: function (result) {
                    alert(result);

                },
                error: function (xhr, ajaxOptions, thrownError) {
                }
            });


        }

    }

};

$(function () {

    $(".datepicker").datepicker();

    var vm = new ViewModel();
    vm.selectedDate(moment().format('MM/DD/YYYY'));

    vm.getBeats();
    vm.getDrivers();
    vm.getAlarmTypes();

    vm.getAlarms();
    //setTimeout(function updateAlertHistory() {
    //    console.log('Updating Alert History...');
    //    vm.getAlarms();
    //    setTimeout(updateAlertHistory, 10000);
    //}, 10000);

    ko.applyBindings(vm);
});

