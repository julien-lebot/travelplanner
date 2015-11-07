/// <reference path="../app.ts" />

module app.services {

    export class TripsService implements IService {
        q;
        http;
        tripData = {
            trips: []
        };

        constructor($http: ng.IHttpService, $q) {
            this.q = $q;
            this.http = $http;
        }

        refresh = () => {
            this.http.get("/api/trips").then(response => {
                this.tripData.trips = [];
                for (var i = 0; i < response.data.length; ++i) {
                    var trip = response.data[i];
                    var start = moment(trip.startDate);
                    this.tripData.trips.push({
                        destination: trip.destination,
                        comment: trip.comment,
                        startDate: start.format("LL"),
                        endDate: moment(trip.endDate).format("LL"),
                        relative: start.fromNow()
                    });
                }
            });
        }

        create = (trip) => {

            var deferred = this.q.defer();

            this.http.post("/api/trips", trip).then(
                response => {
                    this.refresh();
                    deferred.resolve(response);
                },
                error => {
                    deferred.reject(error);
                });

            return deferred.promise;
        }

    };
}

angular.module('app').factory('tripsService', ['$http', '$q', ($http, $q) => new app.services.TripsService($http, $q)]);
//app.registerService('AuthService', ['$http', '$q']);