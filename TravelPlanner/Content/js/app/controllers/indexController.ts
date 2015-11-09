/// <reference path="../app.ts" />
'use strict';

module app.controllers {
    export class IndexController implements IController {

        constructor(
            $scope,
            $location: ng.ILocationService,
            authService: app.services.AuthService,
            pageService: app.services.PageService,
            tripsService: app.services.TripsService)
        {
            $scope.pageData = pageService.pageData;
            
            $scope.logOut = () =>
            {
                authService.logout();
                $location.path('/login');
            };

            $scope.authentication = authService.authentication;
            $scope.showAboutPanel = () => showDialog({ title: "About", text: "Travel Planner by Nick", positive: "Ok" });
            $scope.printCurrentPage = () =>
            {
                var my_window = window.open("", "mywindow", "status=1,height=1080,width=1920");
                my_window.document.write('<html>' +
                    '<head>' +
                    '   <link href="https://fonts.googleapis.com/css?family=Roboto:regular,bold,italic,thin,light,bolditalic,black,medium&amp;lang=en" rel="stylesheet">' +
                    '   <style>html, body { font-family: "Roboto", "Helvetica", sans-serif; font-size: 14px; font- weight: 400; line - height: 20px; color: rgba(0,0,0, 0.87); font- size: 1em; line - height: 1.4; }</style>' +
                    '   <title>Print trips</title>' +
                    '</head>');
                my_window.document.write('<body onafterprint="self.close()"><button onclick="window.print()">Print</button>'); 
                var trips = [];
                if ($("#all-trips").hasClass("is-active"))
                {
                    trips = tripsService.tripData.trips;
                    my_window.document.write('<h1>All my trips</h1><ul>');
                }
                else
                {
                    var ref = moment().add(1, 'months');
                    trips = tripsService.tripData.trips.filter((v, i, a) => v.startDate.month() === ref.month() && v.startDate.year() === ref.year());
                    my_window.document.write('<h1>My trips for ' + ref.format('MMMM YYYY') + ' </h1><ul>');
                }
                for (var i = 0; i < trips.length; ++i)
                {
                    my_window.document.write('<li><h3>Trip to ' + trips[i].destination + '</h3>');
                    my_window.document.write('<p><span>Start date: </span>' + trips[i].startDate.format('LL') + '</p>');
                    my_window.document.write('<p><span>End date: </span>' + trips[i].endDate.format('LL') + '</p>');
                    my_window.document.write('<p><span>Comment: </span>' + trips[i].comment + '</li></p>');
                }
                my_window.document.write('</ul></body></html>');
                // Sometimes the text disappear if we try to print automatically
                /*var script = my_window.document.createElement("script");
                script.type = "text/javascript";
                script.src = "https://ajax.googleapis.com/ajax/libs/jquery/1.7.2/jquery.min.js";
                $(my_window).ready(() =>
                {
                    setTimeout(() => my_window.print(), 10);
                });*/
            }
        }
    };
}

app.registerController('IndexController', ['$scope', '$location', 'authService', 'pageService', 'tripsService']);