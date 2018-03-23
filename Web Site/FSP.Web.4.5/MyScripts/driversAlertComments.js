var ViewModel = function () {
    var self = this;

    self.beats = ko.observableArray([]);
    self.selectedBeat = ko.observable();

    self.drivers = ko.observableArray([]);
    self.selectedDriver = ko.observable();
    
    self.alarmTypes = ko.observableArray([]);
    self.selectedAlarmType = ko.observable();

    self.driversAlertComments = ko.observableArray([]);
    self.isBusy = ko.observable(false);

    self.selectedDate = ko.observable('');

    self.getDriversAlertComments = function () {

        self.isBusy(true);
        console.time('Gettings Drivers Alert Comments');

        var beat = '';
        var driver = '';
       
        if (self.selectedBeat() != undefined)
            beat = self.selectedBeat().Text;

        if (self.selectedDriver() != undefined)
            driver = self.selectedDriver().Text;     

        $.ajax({
            url: $(".websiteUrl").text().trim() + '/AlertMessages/GetDriversAlertComments',
            type: "POST",
            dataType: "json",
            data: {
                beat: beat,
                driver: driver,
                date: self.selectedDate(),
                alarmType: self.selectedAlarmType()              
            },
            success: function (results) {
                console.timeEnd('Gettings Drivers Alert Comments');
                self.driversAlertComments([]);
                for (var i = 0; i < results.length; i++) {
                    self.driversAlertComments.push(new driversAlertComment(self, results[i]));
                }

                self.isBusy(false);
            },
            error: function (error) {
                console.log(error);
            }
        });

    };

    self.getAlarmTypes = function () {

        console.time('Gettings Alarm Types');

        $.ajax({
            url: $(".websiteUrl").text().trim() + '/AlertMessages/GetAlarmTypes',
            type: "GET",
            dataType: "json",
            success: function (results) {
                console.timeEnd('Gettings Alarm Types');
                self.alarmTypes([]);
                for (var i = 0; i < results.length; i++) {
                    self.alarmTypes.push(results[i]);
                }

            },
            error: function () {
            }
        });

    };
   
    self.getBeats = function () {

        console.time('Gettings Beats');

        $.ajax({
            url: $(".websiteUrl").text().trim() + '/AlertMessages/GetAllBeats',
            type: "GET",
            dataType: "json",
            success: function (results) {
                console.timeEnd('Gettings Beats');
                self.beats([]);
                for (var i = 0; i < results.length; i++) {
                    self.beats.push(results[i]);
                }

            },
            error: function () {
            }
        });

    };

    self.getDrivers = function () {

        console.time('Gettings Drivers');

        $.ajax({
            url: $(".websiteUrl").text().trim() + '/AlertMessages/GetAllDrivers',
            type: "GET",
            dataType: "json",
            success: function (results) {
                console.timeEnd('Gettings Drivers');
                self.drivers([]);
                for (var i = 0; i < results.length; i++) {
                    self.drivers.push(results[i]);
                }

            },
            error: function () {
            }
        });

    };

    self.filter = function () {
        self.getDriversAlertComments();
    };

    self.reset = function () {

        self.selectedBeat(null);
        self.selectedDriver(null);
        self.selectedAlarmType(null);
        self.selectedDate(moment().format('MM/DD/YYYY'));

        self.getDriversAlertComments();

    };

    function driversAlertComment(root, dbAlarm) {

        var self = this;

        self.BeatNumber = dbAlarm.BeatNumber;
        self.VehicleNumber = dbAlarm.VehicleNumber;
        self.DriverFirstName = dbAlarm.DriverFirstName;
        self.DriverLastName = dbAlarm.DriverLastName;
        self.Datestamp = moment(dbAlarm.Datestamp).format('MM/DD/YY hh:mm A');
        self.Explanation = dbAlarm.Explanation;
        self.AlarmType = dbAlarm.AlarmType;            
        self.CHPLogNumber = dbAlarm.CHPLogNumber;
        self.ExceptionType = dbAlarm.ExceptionType;
     
    }

};

$(function () {

    $(".datepicker").datepicker();

    var vm = new ViewModel();
    vm.selectedDate(moment().format('MM/DD/YYYY'));

    vm.getBeats();
    vm.getDrivers();
    vm.getAlarmTypes();

    vm.getDriversAlertComments();
    //setTimeout(function updateAlertHistory() {
    //    console.log('Updating Alert History...');
    //    vm.getAlarms();
    //    setTimeout(updateAlertHistory, 10000);
    //}, 10000);

    ko.applyBindings(vm);
});

