/// <reference path="../app.ts" />
'use strict';

module app.filters {
    import Moment = moment.Moment;

    export class RangeTo implements IFilter {
        filter(start: number, end: number) {
            var out = [];
            for (var i = start; i < end; ++i) {
                out.push(i);
            }
            return out;
        }
    }

    export class Splice implements IFilter {
        filter(input: any, start: number, howMany: number) {
            return input.splice(start, howMany);
        }
    }

    export class DateRange implements IFilter {
        filter(input: Array<Date>, start: Date, end: Date) {
            var result = [];
            for (var i = 0; i < input.length; ++i) {
                if (input[i] > start && input[i] < end) {
                    result.push(input[i]);
                }
            }
            return result;
        }
    }

    export class InMonth implements IFilter {
        filter(input: Array<any>, reference: Moment) {
            var result = [];
            for (var i = 0; i < input.length; ++i) {
                if (input[i].startDate.month() === reference.month() &&
                    input[i].startDate.year() === reference.year()) {
                    result.push(input[i]);
                }
            }
            return result;
        }
    }
}

app.registerFilter('RangeTo', []);
app.registerFilter('Splice', []);
app.registerFilter('DateRange', []);
app.registerFilter('InMonth', []);