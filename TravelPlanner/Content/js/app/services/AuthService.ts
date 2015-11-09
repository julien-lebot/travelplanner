/// <reference path="../app.ts" />
'use strict';

module app.services {

    export class AuthService implements IService {
        q;
        http;
        authentication = {
            isAuth: false,
            isAdmin: false,
            isUserManager: false,
            id: "",
            userName: "",
            roles: [],
            token: ""
        };

        constructor($http: ng.IHttpService, $q) {
            this.q = $q;
            this.http = $http;
            this.restore();
        }

        register = registration => {
            this.logout();

            return this.http.post('/api/users', registration).then(response => response);
        };

        login = loginData => {
            var data = "grant_type=password&username=" + loginData.userName + "&password=" + loginData.password;

            var deferred = this.q.defer();

            this.http.post('/token', data, { headers: { 'Content-Type': 'application/x-www-form-urlencoded' } }).success(response => {
                this.authentication.isAuth = true;
                this.authentication.userName = loginData.userName;
                this.authentication.token = response.access_token;

                this.http.get('api/users/current')
                    .success(
                        usrRes => {
                            this.authentication.id = usrRes.id;
                            this.authentication.roles = usrRes.roles;
                            this.authentication.isAdmin = usrRes.roles.find((e, i, a) => e.indexOf("Admin") > -1);
                            this.authentication.isUserManager = usrRes.roles.find((e, i, a) => e.indexOf("UserManager") > -1);

                            sessionStorage.setItem('authorizationData', JSON.stringify({
                                token: response.access_token,
                                userName: loginData.userName,
                                id: usrRes.id,
                                roles: usrRes.roles
                        }));

                            deferred.resolve(response);
                        })
                    .error((e, s) => {
                        this.logout();
                        deferred.reject(e);
                    });

            }).error((err, status) => {
                this.logout();
                deferred.reject(err);
            });

            return deferred.promise;
        };

        logout = () => {
            sessionStorage.removeItem('authorizationData');

            this.authentication.isAuth = false;
            this.authentication.userName = "";
        };

        restore = () => {
            var authData = sessionStorage.getItem('authorizationData');
            if (authData) {
                authData = JSON.parse(authData);
                this.authentication.isAuth = true;
                this.authentication.userName = authData.userName;
                this.authentication.token = authData.token;
                this.authentication.id = authData.id;
                this.authentication.roles = authData.roles;
                this.authentication.isAdmin = authData.roles.find((e, i, a) => e.indexOf("Admin") > -1);
                this.authentication.isUserManager = authData.roles.find((e, i, a) => e.indexOf("UserManager") > -1);
            }
        };
    };
}

angular.module('app').factory('authService', ['$http', '$q', ($http, $q) => new app.services.AuthService($http, $q)]);
//app.registerService('AuthService', ['$http', '$q']);