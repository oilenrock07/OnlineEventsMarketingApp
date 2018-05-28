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
        }
    }

    function handleExportClick(e) {
        
    }

    function init() {
        $('form').on('submit', handleSubmit);
        $('.js-export').on('click', handleExportClick);
    }

    init();
})