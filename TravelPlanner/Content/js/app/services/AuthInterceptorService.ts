/// <reference path="../app.ts" />
'use strict';

module app.services {

    export class AuthInterceptorService implements IService {
        injector;
        location;
        $q;

        constructor($q, $injector, $location: angular.ILocationProvider)
        {
            this.injector = $injector;
            this.location = $location;
            this.$q = $q;
        }

        request = config =>
        {
            config.headers = config.headers || {};
            var authService = this.injector.get('authService');
            var authData = authService.authentication;
            if (authData.isAuth) {
                config.headers.Authorization = 'Bearer ' + authData.token;
            }

            return config;
        };

        responseError = rejection =>
        {
            if (rejection.status === 401) {
                var authService = this.injector.get('authService');
                authService.logout();
                // Save current location
                sessionStorage.setItem("tempPath", this.location.path());
                this.location.path('/login');
            }
            return this.$q.reject(rejection);
        };
    };
}

angular.module('app').factory('authInterceptorService', ['$q', '$injector', '$location', ($q, $injector, $location) => new app.services.AuthInterceptorService($q, $injector, $location)]);
angular.module('app').config(['$httpProvider', ($httpProvider: ng.IHttpProvider) =>
{
    $httpProvider.interceptors.push('authInterceptorService');
}]);
