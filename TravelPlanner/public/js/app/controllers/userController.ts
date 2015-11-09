/// <reference path="../app.ts" />
declare var showDialog: any;
declare var componentHandler: any;

module app.controllers {
    export class UserController implements IController {
        usersService: app.services.UsersService;
        scope;
        location;
        usrId;
        userData = {
            userName: "",
            password: "",
            roles: [
                { name: 'User', selected: false },
                { name: 'UserManager', selected: false },
                { name: 'Admin', selected: false }
            ]
        };
        previousUser = {
            password: "",
            roles: [
                { name: 'User', selected: false },
                { name: 'UserManager', selected: false },
                { name: 'Admin', selected: false }
            ]
        };
        error = {
            genErrors: "",
            userNameErrors: "",
            passwordErrors: "",
            roleErrors: ""
        }

        constructor(
            $scope,
            $location,
            $routeParams,
            pageService: app.services.PageService,
            usersService: app.services.UsersService) {
            this.usrId = $routeParams.userId;
            $scope.deleteEnabled = this.usrId !== undefined;
            this.usersService = usersService;
            this.scope = $scope;
            this.location = $location;
            if (this.usrId === undefined) {
                pageService.pageData.title = "Create user";
                $scope.actionBtnTitle = "Create";
            }
            else {
                pageService.pageData.title = "Edit user";
                $scope.actionBtnTitle = "Save";
                var usr = usersService.userData.users.filter((v, i) => {
                    return (v.id === this.usrId);
                })[0];
                this.userData.userName = usr.userName;
                for (var i = 0; i < this.userData.roles.length; ++i) {
                    var role = usr.roles.find(v => v === this.userData.roles[i].name);
                    this.userData.roles[i].selected = this.previousUser.roles[i].selected = (role !== undefined);
                }
            }

            $scope.save = this.save;
            $scope.cancel = () => $location.path('/users');
            $scope.error = this.error;
            $scope.delete = this.delete;
            $scope.userData = this.userData;
        }

        setErrors = (err) => {
            for (var key in err.modelState) {
                var errors = [];
                for (var i = 0; i < err.modelState[key].length; i++) {
                    errors.push(err.modelState[key][i]);
                }
                var errorStr = errors.join(' ');
                if (errorStr.length > 0) {
                    this.error.genErrors = errorStr;
                }
            }
        };

        delete = () => {
            if (this.usrId !== undefined) {
                showDialog({
                    title: "Confirm",
                    text: "Are you sure you want to delete this user ?",
                    positive: {
                        onClick: () => {
                            this.usersService.delete(this.usrId).then(
                                response => {
                                    this.usersService.refresh();
                                    this.location.path('/users');
                                    return false;
                                },
                                error => {
                                    this.setErrors(error.data);
                                    return true;
                                });
                        }
                    },
                    negative: true
                });
            }
        }

        save = () =>
        {
            var roles = [];
            for (var i = 0; i < this.userData.roles.length; ++i)
            {
                if (this.userData.roles[i].selected)
                {
                    roles.push(this.userData.roles[i].name);
                }
            }

            if (this.usrId === undefined) {
                this.usersService.create({
                    userName: this.userData.userName,
                    password: this.userData.password,
                    roles: roles
                }).then(
                    response => {
                        this.usersService.refresh();
                        this.location.path('/users');
                    },
                    error => {
                        this.setErrors(error.data);
                    });
            }
            else {
                var patch: any = {};
                var prev = this.previousUser;
                var curr = this.userData;
                if (prev.password !== curr.password) {
                    patch.password = curr.password;
                }
                if (prev.roles !== curr.roles) {
                    patch.Roles = roles;
                }

                this.usersService.update(this.usrId, patch).then(
                    response => {
                        this.usersService.refresh();
                        this.location.path('/');
                    },
                    error => {
                        this.setErrors(error.data);
                    });
            }
        };
    };
};

app.registerController('UserController', ['$scope', '$location', '$routeParams', 'pageService', 'usersService']);