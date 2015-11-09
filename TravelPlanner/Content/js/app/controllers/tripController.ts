/// <reference path="../app.ts" />
'use strict';

declare var showDialog: any;

module app.controllers {
    export class TripController implements IController {
        tripsService: app.services.TripsService;
        scope;
        location;
        tripId;
        tripData = {
            destination: "",
            startDate: null,
            endDate: null,
            comment: ""
        };
        previousTrip = {
            destination: "",
            startDate: null,
            endDate: null,
            comment: "" 
        };
        error = {
            genErrors: "",
            destErrors: "",
            startErrors: "",
            endErrors: "",
            commentErrors: ""
        }

        constructor(
            $scope,
            $location,
            $routeParams,
            pageService: app.services.PageService,
            tripsService: app.services.TripsService) {
            this.tripId = $routeParams.tripId;
            $scope.deleteEnabled = this.tripId !== undefined;
            this.tripsService = tripsService;
            this.scope = $scope;
            this.location = $location;
            if (this.tripId === undefined) {
                pageService.pageData.title = "Create trip";
                $scope.actionBtnTitle = "Create";
            }
            else {
                pageService.pageData.title = "Edit trip";
                $scope.actionBtnTitle = "Save";
                var trip = tripsService.tripData.trips.filter((v, i) => {
                    return (v.id === this.tripId);
                })[0];
                this.tripData.destination = this.previousTrip.destination = trip.destination;
                this.tripData.startDate = this.previousTrip.startDate = new Date(trip.startDate);
                this.tripData.endDate = this.previousTrip.endDate = new Date(trip.endDate);
                this.tripData.comment = this.previousTrip.comment = trip.comment;
            }
            $scope.save = this.save;
            $scope.cancel = () => $location.path('/');
            $scope.error = this.error;
            $scope.delete = this.delete;
            $scope.tripData = this.tripData;
        }

        setErrors = (err) => {
            for (var key in err.modelState) {
                var errors = [];
                for (var i = 0; i < err.modelState[key].length; i++) {
                    errors.push(err.modelState[key][i]);
                }
                var errorStr = errors.join(' ');
                if (errorStr.length > 0) {
                    if (key.indexOf('startDate') > -1) {
                        this.error.startErrors = errorStr;
                    }
                    else if (key.indexOf('endDate') > -1) {
                        this.error.endErrors = errorStr;
                    }
                    else {
                        this.error.genErrors = errorStr;
                    }
                }
            }
        };

        delete = () =>
        {
            if (this.tripId !== undefined)
            {
                showDialog({
                    title: "Confirm",
                    text: "Are you sure you want to delete this trip ?",
                    positive: {
                        onClick: () =>
                        {
                            this.tripsService.delete(this.tripId).then(
                                response =>
                                {
                                    this.tripsService.refresh();
                                    this.location.path('/');
                                    return false;
                                },
                                error =>
                                {
                                    this.setErrors(error.data);
                                    return true;
                                });
                        }
                    },
                    negative: true
                });
            }
        }

        save = () => {
            if (this.tripId === undefined) {
                this.tripsService.create(this.tripData).then(
                    response => {
                        this.tripsService.refresh();
                        this.location.path('/');
                    },
                    error => {
                        this.setErrors(error.data);
                    });
            }
            else {
                var patch: any = {};
                var prev = this.previousTrip;
                var curr = this.tripData;
                if (prev.destination !== curr.destination) {
                    patch.Destination = curr.destination;
                }
                if (prev.comment !== curr.comment) {
                    patch.Comment = curr.comment;
                }
                if (prev.startDate !== curr.startDate) {
                    patch.StartDate = moment(curr.startDate).format("YYYY/MM/DD");
                }
                if (prev.endDate !== curr.endDate) {
                    patch.EndDate = moment(curr.endDate).format("YYYY/MM/DD");
                }
                this.tripsService.update(this.tripId, patch).then(
                    response => {
                        this.tripsService.refresh();
                        this.location.path('/');
                    },
                    error => {
                        this.setErrors(error.data);
                    });
            }
        };
    };
};

app.registerController('TripController', ['$scope', '$location', '$routeParams', 'pageService', 'tripsService']);