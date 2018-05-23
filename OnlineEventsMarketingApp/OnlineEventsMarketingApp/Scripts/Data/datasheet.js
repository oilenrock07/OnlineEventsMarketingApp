$(function () {

    function handleSubmit(e) {
        var answer = confirm("Are you sure you want to submit this new datasheet? This will delete the existing one and replaced by this one.");
        if (!answer) {
            e.preventDefault();
        }
    }

    function init() {
        $('form').on('submit', handleSubmit);
    }

    init();
})