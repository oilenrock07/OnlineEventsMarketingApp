$(function () {
    function handleMonthYearChange() {
        var month = $('.js-month').val();
        var year = $('.js-year').val();

        var startDate = new Date(year, month - 1, 1);
        var endDate = new Date(year, month, 0);
        
        $(".datepicker").datepicker('remove');
        $(".datepicker").datepicker({
            startDate: startDate,
            endDate: endDate
        });
    }

    $('.js-month').change(function () {
        $('.datepicker').val('');
        handleMonthYearChange();
    });

    function init() {
        $('.js-month .js-year').on('change', handleMonthYearChange);
        handleMonthYearChange();
    }

    init();
})
