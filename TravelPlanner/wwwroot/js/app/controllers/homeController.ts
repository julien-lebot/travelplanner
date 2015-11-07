/// <reference path="../app.ts" />
declare var showDialog: any;
declare var componentHandler: any;

module app.controllers {
    export class HomeController implements IController {
        tripsService : app.services.TripsService;
        scope;

        constructor(
            $scope,
            pageService: app.services.PageService,
            tripsService: app.services.TripsService)
        {
            this.tripsService = tripsService;
            this.scope = $scope;
            pageService.pageData.title = "Home";
            tripsService.refresh();
            $scope.tripsData = tripsService.tripData;
            $scope.addNewTrip = this.addNewTrip;
        }

        hideDialog = (dialog) => {
            $(document).unbind("keyup.dialog");
            dialog.css({ opacity: 0 });
            setTimeout(() => {
                dialog.remove();
            }, 400);
        }

        addNewTrip = () => {
            $('.dialog-container').remove();
            $(document).unbind("keyup.dialog");
            $('<div id="addNewTrip" class="dialog-container"><div class="mdl-card mdl-shadow--16dp">' +
                '<h5>Create new trip</h5>' +
                '<div class="mdl-textfield mdl-js-textfield mdl-textfield--floating-label"><input class="mdl-textfield__input" type="text" id="dest"/><label class="mdl-textfield__label" for="dest">Destination</label></div>' +
                '<span>Start date</span><div class="mdl-textfield mdl-js-textfield"><input class="mdl-textfield__input" type="date" id="start"/></div>' +
                '<span>End date</span><div class="mdl-textfield mdl-js-textfield"><input class="mdl-textfield__input" type="date" id="end"/></div>' +
                '<div class="mdl-textfield mdl-js-textfield mdl-textfield--floating-label"><input class="mdl-textfield__input" type="text" id="comment"/><label class="mdl-textfield__label" for="comment">Comment</label></div>' +
                '<div class="mdl-card__actions dialog-button-bar"></div>' +
                '</div></div>').appendTo("body");
            var dialog = $('#addNewTrip');
            var content = dialog.find('.mdl-card');
            var buttonBar = $('<div class="mdl-card__actions dialog-button-bar"></div>');
            var negButton = $('<button class="mdl-button mdl-js-button mdl-js-ripple-effect">Cancel</button>');
            negButton.click(e => {
                e.preventDefault();
                this.hideDialog(dialog);
            });
            negButton.appendTo(buttonBar);
            var posButton = $('<button class="mdl-button mdl-button--colored mdl-js-button mdl-js-ripple-effect">Create</button>');
            posButton.click(e => {
                e.preventDefault();
                this.tripsService.create({
                    destination: $("#dest").val(),
                    startDate: $("#start").val(),
                    endDate: $("#end").val(),
                    comment: $("#comment").val()
                }).then(
                    response =>
                    {
                        this.tripsService.refresh();
                        this.hideDialog(dialog);
                    },
                    error =>
                    {

                    });
            });
            posButton.appendTo(buttonBar);
            buttonBar.appendTo(content);
            componentHandler.upgradeDom();
            setTimeout(() => {
                dialog.css({ opacity: 1 });
            }, 1);
        };
    };
}

app.registerController('HomeController', ['$scope', 'pageService', 'tripsService']);