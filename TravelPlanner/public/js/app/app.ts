/// <reference path="../../typings/angularjs/angular.d.ts" />
/// <reference path="../../typings/angularjs/angular-route.d.ts" />

// based on https://github.com/santialbo/AngularJS-TypeScript-Project-Template

'use strict';

// Create and register modules
var modules = ['app.controllers', 'app.directives', 'app.filters', 'app.services'];
modules.forEach((module) => angular.module(module, []));

// *** Push ngRoute or $routeProvider won't work ***
modules.push("ngRoute");

angular.module('app', modules);

// Url routing
angular.module('app').config([
    '$routeProvider',
    ($routeProvider: ng.route.IRouteProvider) => {
        $routeProvider
            .when('/', {
                templateUrl: '/Partials/Home',
                controller: 'app.controllers.HomeController'
            })
            .when('/register', {
                templateUrl: '/Partials/Register',
                controller: 'app.controllers.RegisterController'
            })
            .when('/login', {
                templateUrl: '/Partials/Login',
                controller: 'app.controllers.LoginController'
            })
            .when('/trip', {
                templateUrl: '/Partials/Trip',
                controller: 'app.controllers.TripController'
            })
            .when('/trip/:tripId', {
                templateUrl: '/Partials/Trip',
                controller: 'app.controllers.TripController'
            })
            .when('/users', {
                templateUrl: '/Partials/Users',
                controller: 'app.controllers.UsersController'
            })
            .when('/users/:userId', {
                templateUrl: '/Partials/User',
                controller: 'app.controllers.UserController'
            })
            .when('/newUser', {
                templateUrl: '/Partials/User',
                controller: 'app.controllers.UserController'
            })
            .otherwise({
                redirectTo: '/'
            });
    }
]).
    run(($rootScope, $location, $timeout, authService: app.services.AuthService) => {
        $rootScope.$on("$routeChangeStart", (event, next, current) => {
            if (!authService.authentication.isAuth) {
                // no logged user, redirect to /login
                if (next.templateUrl !== "/Partials/Login" &&
                    next.templateUrl !== "/Partials/Register") {
                    $location.path("/login");
                }
            }
            $rootScope.currentPath = $location.path();
        });
        $rootScope.$on('$viewContentLoaded', () => {
            $timeout(() => {
                componentHandler.upgradeAllRegistered();
            });
        });
    });

module app {

    // *** Modules need to be populated to be correctly defined, otherwise they will give warnings. null fixes this ***/
    export module controllers { null; }
    export module directives { null; }
    export module filters { null; }
    export module services { null; }

    export interface IController { }
    export interface IDirective {
        restrict: string;
        link($scope: ng.IScope, element: JQuery, attrs: ng.IAttributes): any;
    }
    export interface IFilter {
        filter(input: any, ...args: any[]): any;
    }
    export interface IService { }

    /**
     * Register new controller.
     *
     * @param className
     * @param services
     */
    export function registerController(className: string, services = []) {
        var controller = 'app.controllers.' + className;
        services.push(app.controllers[className]);
        angular.module('app.controllers').controller(controller, services);
    }

    /**
     * Register new filter.
     *
     * @param className
     * @param services
     */
    export function registerFilter(className: string, services = []) {
        var filter = className.toLowerCase();
        services.push(() => (new app.filters[className]()).filter);
        angular.module('app.filters').filter(filter, services);
    }

    /**
     * Register new directive.
     *
     * @param className
     * @param services
     */
    export function registerDirective(className: string, services = []) {
        var directive = className[0].toLowerCase() + className.slice(1);
        services.push(() => new app.directives[className]());
        angular.module('app.directives').directive(directive, services);
    }

    /**
     * Register new service.
     *
     * @param className
     * @param services
     */
    export function registerService(className: string, services = []) {
        var service = className[0].toLowerCase() + className.slice(1);
        services.push(app.services[className]);
        angular.module('app.services').factory(service, services);
    }
}
