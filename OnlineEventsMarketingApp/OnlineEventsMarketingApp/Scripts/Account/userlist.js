$(function () {

    function handleUserDeleteClick(e) {
        if (!confirm('Are you sure you want to delete this user?')) {
            e.preventDefault();
            return;
        }

    }

    function init() {
        $('.js-userDelete').on('click', handleUserDeleteClick);
    }

    init();
})