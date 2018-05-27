$(function () {

    function handleYearMonthChange() {
        var month = $('.js-month').val();
        var year = $('.js-year').val();
        window.location = '/Report/WeeklyTagsRun/' + month + '/' + year;
    }

    function init() {
        $('.js-year').on('change', handleYearMonthChange);
        $('.js-month').on('change', handleYearMonthChange);
    }

    init();
})