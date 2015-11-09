/// <reference path="../app.ts" />

module app.services {

    export class UsersService implements IService {
        q;
        http;
        userData = {
            users: []
        };

        constructor($http: ng.IHttpService, $q) {
            this.q = $q;
            this.http = $http;
        }

        refresh = () => {
            this.http.get("/api/users").then(response => {
                this.userData.users = [];
                for (var i = 0; i < response.data.length; ++i) {
                    var usr = response.data[i];
                    this.userData.users.push({
                        id: usr.id,
                        userName: usr.userName,
                        roles: usr.roles
                    });
                }
            });
        }

        create = (usr) =>
        {
            return this.http.post("/api/users", usr);
        }

        update = (id, usr) =>
        {
            return this.http.patch("/api/users/" + id, usr);
        }

        delete = (id) =>
        {
            return this.http.delete("/api/users/" + id);
        }
    };
}

angular.module('app').factory('usersService', ['$http', '$q', ($http, $q) => new app.services.UsersService($http, $q)]);
//app.registerService('AuthService', ['$http', '$q']);