$(function () {     
    function handleYearMonthChange() {
        var year = $('.js-year').val();

        if (oldOptions.length == 0)
            window.location = '/Report/MonthlyTagsRun/' + year;
        else
            window.location = '/Report/MonthlyTagsRun/' + year + '?months=' + oldOptions;
    }

    function handleDropdownButtonClose() {
        var options = [];
        $('input:checked').each(function () {
            options.push($(this).data('value'));
        });

        if (!arrayMatch(options, oldOptions)) {
            oldOptions = options.slice();
            handleYearMonthChange();
        }
    }

    function handleDropdownButtonClick(event) {
        var $target = $(event.currentTarget),
            $input = $target.find('input');

        if ($input.prop('checked')) {
            $input.prop('checked', false);
        } else {
            $input.prop('checked', true);
        }

        return false;
    }

    function init() {
        $('.js-year').on('change', handleYearMonthChange);
        $('.dropdown-button a').on('click', handleDropdownButtonClick);
        $('.dropdown-button').on('hidden.bs.dropdown', handleDropdownButtonClose);
    }

    init();
})