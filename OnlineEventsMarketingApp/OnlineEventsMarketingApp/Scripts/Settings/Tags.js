$(function () {

    function handleCreate(e) {
        if (!confirm('Are you sure you want to delete this user?')) {
            e.preventDefault();
            return;
        }

    }

    function init() {
        $('.js-createTag').on('click', handleCreate);
    }

    init();
})