
lata.FspWeb.prototype.startTruckService = function () {

    //hub methods
    fspWeb.towTruckHub = $.connection.towTruckHub;

    fspWeb.getTrucks = function () {
        //new
        var truckCollectionUrl = fspWeb.SERVICE_BASE_URL + '/Truck/UpdateAllTrucks';

        console.group("New Truck Request");
        console.log('%c Timestamp: %s', 'color: green', new Date());
        console.time('Web Request');

        $.get(truckCollectionUrl,
            function (trucks) {

                console.timeEnd('Web Request');
                console.time('Rendering');
                $.each(trucks, function (i, truck) {
                    fspWeb.addOrUpdateTruck(truck);
                });

                for (var i = 0; i < fspWeb.trucks().length; i++) {
                    var uiTruck = fspWeb.trucks()[i];
                    var truckIsGood = false;

                    for (var ii = 0; ii < trucks.length; ii++) {
                        var serverTruck = trucks[ii];

                        if (serverTruck.TruckNumber === uiTruck.truckNumber) {
                            truckIsGood = true;
                        }
                    }

                    if (truckIsGood === false) {
                        fspWeb.trucks.remove(function (item) { return item.truckNumber === uiTruck.truckNumber; });
                    }
                }
                console.timeEnd('Rendering');
                console.groupEnd("New Truck Request");

            }, "json");

    }

    //add or update lmt in corridor lmt list
    fspWeb.addOrUpdateTruck = function (dbTruck) {

        try {

            //console.log("Add or update truck %s", dbTruck.TruckNumber);

            var currentTruck = ko.utils.arrayFirst(fspWeb.trucks(), function (i) { return i.truckNumber === dbTruck.TruckNumber; });

            if (currentTruck) {
                currentTruck.update(dbTruck);

                if (fspWeb.trucksChanged() === true)
                    fspWeb.trucksChanged(false);
                else
                    fspWeb.trucksChanged(true);

            }
            else {

                //var addTruck = true;

                ////check for user contractor association
                //if (dbTruck.UserContractorName) {
                //    if (dbTruck.UserContractorName === dbTruck.ContractorName)
                //        addTruck = true;
                //    else
                //        addTruck = false;
                //}

                //if (addTruck)

                var tr = new fspWeb.truck(dbTruck);
                fspWeb.trucks.push(tr);
            }

        } catch (e) {

        }

    };

    fspWeb.towTruckHub.client.setSelectedTruck = function (truckNumber, userId) {

        var currentTruck = ko.utils.arrayFirst(fspWeb.trucks(), function (i) { return i.truckNumber === truckNumber; });
        if (currentTruck) {
            try {

                //alert('Current UserId ' + fspWeb.userId + '\nRequesting UserId ' + userId);

                if (userId === fspWeb.userId) {
                    //alert('Setting truck to: ' + truckNumber);
                    fspWeb.selectedId(currentTruck.id);
                    //alert('Setting truck to: ' + truckNumber);
                }
            } catch (e) {

            }
        }
    };

    fspWeb.towTruckHub.client.stopFollowingTruck = function (userId) {

        try {
            //alert('Current UserId ' + fspWeb.userId + ' requesting UserId ' + userId);

            if (userId === fspWeb.userId) {
                //alert('Stop following truck');
                fspWeb.selectedId(null);
                //alert('Setting truck to: ' + truckNumber);
            }
        } catch (e) {

        }

    };

    fspWeb.towTruckHub.client.updateUserId = function (userId) {
        //alert('New UserId ' + userId);
        fspWeb.userId = userId;
    };

    $.connection.hub.start(function () {

        fspWeb.getTrucks();

        //var clientRefreshRate = 1000;

        //TK 12.10.2016
        var clientRefreshRate = 5000;

        setTimeout(function updateTrucks() {
            fspWeb.getTrucks();
            setTimeout(updateTrucks, clientRefreshRate);
        }, clientRefreshRate);

        fspWeb.towTruckHub.server.initialize();
    });

};


