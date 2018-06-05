$(function () {

    function handleDelete(e) {
        if (!confirm('Are you sure you want to delete this tag?')) {
            e.preventDefault();
            return;
        }
    }

    function handleYearMonthChange() {
        var month = $('.js-month').val();
        var year = $('.js-year').val();
        window.location = subdomain + '/Settings/Tags/' + month + '/' + year;
    }

    function init() {
        $('.js-delete').on('click', handleDelete);
        $('.js-year').on('change', handleYearMonthChange);
        $('.js-month').on('change', handleYearMonthChange);
    }

    init();
})