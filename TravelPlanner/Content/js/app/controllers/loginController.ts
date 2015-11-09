/// <reference path="../app.ts" />
'use strict';

module app.controllers {
    export class LoginController implements IController {

        constructor(
            $scope,
            $location: ng.ILocationService,
            authService: app.services.AuthService,
            pageService: app.services.PageService) {
            pageService.pageData.title = "Login";

            $scope.logOut = () => {
                authService.logout();
                $location.path('/');
            };

            $scope.loginData = {
                userName: "",
                password: ""
            };

            $scope.login = () => {
                $scope.usernameErrors = "";
                $scope.passwordErrors = "";
                $scope.errors = "";
                authService.login($scope.loginData).then(
                    response => {
                        var returnUrl = sessionStorage.getItem("tempPath");
                        if (returnUrl) {
                            sessionStorage.removeItem("tempPath");
                            $location.path(returnUrl);
                        }
                        else {
                            $location.path('/');
                        }
                    },
                    response => {
                        $scope.errors = response.error_description;
                    });
            };
        }
    };
}

app.registerController('LoginController', ['$scope', '$location', 'authService', 'pageService']);