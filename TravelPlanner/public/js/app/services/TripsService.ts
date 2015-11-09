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
                        id: trip.id,
                        destination: trip.destination,
                        comment: trip.comment,
                        startDate: start,
                        endDate: moment(trip.endDate),
                        relative: start.fromNow()
                    });
                }
            });
        }

        create = (trip) =>
        {
            return this.http.post("/api/trips", trip);
        }

        update = (id, trip) =>
        {
            return this.http.patch("/api/trips/" + id, trip);
        }

        delete = (id) =>
        {
            return this.http.delete("/api/trips/" + id);
        }
    };
}

angular.module('app').factory('tripsService', ['$http', '$q', ($http, $q) => new app.services.TripsService($http, $q)]);
//app.registerService('AuthService', ['$http', '$q']);