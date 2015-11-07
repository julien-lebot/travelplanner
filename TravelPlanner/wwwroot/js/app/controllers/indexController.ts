/// <reference path="../app.ts" />

module app.controllers {
    export class IndexController implements IController {

        constructor(
            $scope,
            $location: ng.ILocationService,
            authService: app.services.AuthService,
            pageService: app.services.PageService)
        {
            $scope.pageData = pageService.pageData;

            $scope.logOut = () =>
            {
                authService.logout();
                $location.path('/login');
            };

            $scope.authentication = authService.authentication;
        }
    };
}

app.registerController('IndexController', ['$scope', '$location', 'authService', 'pageService']);