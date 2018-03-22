$(document).ready(function () {

    $("#websitePath").hide();

    //enable cross-domain calls
    jQuery.support.cors = true;
   
    function checkForAlerts() {
        var url = $(".websiteUrl").text().trim() + '/Truck/HaveAlarms';
        $.get(url, function (value) {

            console.log("Have alarms? %s", value);

            if (value === true) {
                //$("#alertScreenLink").attr('class', 'btn btn-danger');
                $("#alertScreenLink").css('color', 'red');
                $("#monitoringTab").css('color', 'red');
                $("#alertMenuItem").css('color', 'red');
            }
            else {
                //$("#alertScreenLink").attr('class', 'btn btn-info');
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