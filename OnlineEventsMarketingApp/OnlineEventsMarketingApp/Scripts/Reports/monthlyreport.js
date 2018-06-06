$(function () {

    function handleYearMonthChange() {
        var year = $('.js-year').val();
        window.location = '/Report/MonthlyTagsRun/' + year;
    }

    function init() {
        $('.js-year').on('change', handleYearMonthChange);
    }

    init();
})