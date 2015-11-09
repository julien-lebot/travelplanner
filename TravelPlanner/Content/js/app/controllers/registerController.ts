/// <reference path="../app.ts" />
'use strict';

module app.controllers {

    export class RegisterController implements IController {

        constructor(
            $scope,
            $location: ng.ILocationService,
            authService: app.services.AuthService,
            pageService: app.services.PageService,
            $timeout) {
            pageService.pageData.title = "Register";

            $scope.savedSuccessfully = false;

            $scope.registration = {
                userName: "",
                password: ""
            };

            var startTimer = () => {
                var timer = $timeout(() => {
                    $timeout.cancel(timer);
                    $location.path('/login');
                }, 2000);
            }

            $scope.signUp = () => {
                $scope.errors = "";
                $scope.usernameErrors = "";
                $scope.passwordErrors = "";
                authService.register($scope.registration).then(
                    response => {
                        $scope.savedSuccessfully = true;
                        startTimer();
                    },
                    response => {
                        for (var key in response.data.modelState) {
                            var errors = [];
                            for (var i = 0; i < response.data.modelState[key].length; i++) {
                                errors.push(response.data.modelState[key][i]);
                            }
                            var errorStr = errors.join(' ');
                            if (key.indexOf('UserName') > -1) {
                                $scope.usernameErrors = errorStr;
                            }
                            else if (key.indexOf('Password') > -1) {
                                $scope.passwordErrors = errorStr;
                            }
                            else {
                                $scope.errors = errorStr;
                            }
                        }
                    });
            };
        }
    };
}

app.registerController('RegisterController', ['$scope', '$location', 'authService', 'pageService', '$timeout']);