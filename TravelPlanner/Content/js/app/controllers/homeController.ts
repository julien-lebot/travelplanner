/// <reference path="../app.ts" />
'use strict';

declare var showDialog: any;
declare var componentHandler: any;

module app.controllers {
    export class HomeController implements IController {
        tripsService : app.services.TripsService;
        scope;

        constructor(
            $scope,
            $location,
            pageService: app.services.PageService,
            tripsService: app.services.TripsService)
        {
            this.tripsService = tripsService;
            this.scope = $scope;
            pageService.pageData.title = "Home";
            tripsService.refresh();
            $scope.tripsData = tripsService.tripData;
            $scope.addNewTrip = () => $location.path('/trip');
            $scope.filterUpcoming = true;
            $scope.referenceDate = moment().add(1, 'months');
        }
    };
}

app.registerController('HomeController', ['$scope', '$location', 'pageService', 'tripsService']);