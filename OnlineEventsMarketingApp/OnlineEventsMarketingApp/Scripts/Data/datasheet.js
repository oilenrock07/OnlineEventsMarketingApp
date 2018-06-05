$(function () {

    function handleSubmit(e) {
        if ($('#File').get(0).files.length === 0) {
            alert('No file selected');
            e.preventDefault();
            return;
        }

        var answer = confirm("Are you sure you want to submit this new datasheet? This will delete the existing one and replaced by this one.");
        if (!answer) {
            e.preventDefault();
            return;
        }
        showLoading();
    }

    function handleExportClick(e) {
        
    }

    function handleMonthYearChange() {
        var month = $('#Month').val();
        var year = $('#Year').val();
        var action = $('#DateSheetForm').attr('location');
        window.location = action + month + '/' + year; //'/Data/' + action + '/' + month + '/' + year;
    }

    function init() {
        $('form').on('submit', handleSubmit);
        $('.js-export').on('click', handleExportClick);
        $('#Month').on('change', handleMonthYearChange);
        $('#Year').on('change', handleMonthYearChange);
    }

    init();
})