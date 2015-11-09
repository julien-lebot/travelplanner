/// <reference path="../app.ts" />
declare var showDialog: any;
declare var componentHandler: any;

module app.controllers {
    export class UsersController implements IController {
        usersService : app.services.UsersService;
        scope;

        constructor(
            $scope,
            $location,
            pageService: app.services.PageService,
            usersService: app.services.UsersService)
        {
            this.usersService = usersService;
            this.scope = $scope;
            pageService.pageData.title = "Users";
            usersService.refresh();
            $scope.userData = usersService.userData;
            $scope.addNewUser = () => $location.path('/newUser');
            $scope.goToUser = (id) => $location.path('/users/' + id);
        }
    };
}

app.registerController('UsersController', ['$scope', '$location', 'pageService', 'usersService']);