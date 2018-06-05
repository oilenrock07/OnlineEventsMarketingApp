$(function () {

    function handleYearMonthChange() {
        var month = $('.js-month').val();
        var year = $('.js-year').val();
        window.location = subdomain + '/Report/WeeklyTagsRun/' + year + '/' + month;
    }

    function init() {
        $('.js-year').on('change', handleYearMonthChange);
        $('.js-month').on('change', handleYearMonthChange);
    }

    init();
})