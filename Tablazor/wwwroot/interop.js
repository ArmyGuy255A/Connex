window.initializeTooltips = function () {
    $(function () {
        $('[data-bs-toggle="tooltip"]').tooltip();
    });
};

window.initializeDropdowns = function () {
    $(function () {
        $('.dropdown-toggle').each(function () {
            new bootstrap.Dropdown(this);
        });
    });
};
