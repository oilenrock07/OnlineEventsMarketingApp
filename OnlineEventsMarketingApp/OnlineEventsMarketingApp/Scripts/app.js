﻿$(function () {

    function handleChangeDate() {
        $(this).datepicker('hide');
    }

    function init() {
        $('.datepicker').each(function (index, value) {

            var startDate = $(value).attr('start');
            var endDate = $(value).attr('end');

            $(value).datepicker({ format: 'mm/dd/yyyy', startDate: startDate, endDate: endDate });
        });

        $('.timepicker').datetimepicker({ format: 'LT' });
        $('.datepicker').on('changeDate', handleChangeDate);
    }

    init();
});

function showLoading() {
    $("body").addClass('loading');
}

function hideLoading() {
    $("body").removeClass('loading');
}

function arrayMatch(a1, a2) {
    return a1.length == a2.length && a1.every(function (v, i) { return v === a2[i] });
}