/// <reference path="../app.ts" />
'use strict';

module app.services {

    export class PageService implements IService {
        pageData = {
            title: "Home"
        };

        constructor() {

        }
    };
}

angular.module('app').factory('pageService', [() => new app.services.PageService()]);
