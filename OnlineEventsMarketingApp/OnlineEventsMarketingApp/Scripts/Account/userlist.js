$(function () {

    function handleUserDeleteClick(e) {
        var text = $(this).text();
        if (!confirm('Are you sure you want to '+ text + ' this user?')) {
            e.preventDefault();
            return;
        }

    }

    function init() {
        $('.js-userDelete').on('click', handleUserDeleteClick);
    }

    init();
})