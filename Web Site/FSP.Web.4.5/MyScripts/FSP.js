$(document).ready(function () {

    $("#websitePath").hide();

    //enable cross-domain calls
    jQuery.support.cors = true;

    var haveAlarms = false;

    function checkForAlerts() {
        var url = $(".websiteUrl").text().trim() + '/Truck/HaveAlarms';
        $.get(url, function (value) {
            
            if (haveAlarms === false && value === true) {
                console.log("Have alarms? %s", value);
            }

            haveAlarms = value;
            
            if (value === true) {

                $("#alertScreenLink").css('color', 'red');
                $("#monitoringTab").css('color', 'red');
                $("#alertMenuItem").css('color', 'red');
            }
            else {
                $("#alertScreenLink").css('color', 'white');
                $("#monitoringTab").css('color', '#999999');
                $("#alertMenuItem").css('color', 'black');
            }

        }, "json");
    }
    checkForAlerts();
    setTimeout(function checkForAlertsFunction() {
        checkForAlerts();
        setTimeout(checkForAlertsFunction, 10000);
    }, 10000);


});