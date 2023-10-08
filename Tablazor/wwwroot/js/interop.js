window.initializeTooltips = function () {
    console.log("Initializing tooltips in interop.js");
    
    $(function () {
        $('[data-bs-toggle="tooltip"]').tooltip();
    });
};

window.initializeDropdowns = function () {
    console.log("Initializing dropdowns in interop.js");
    // $('.dropdown-toggle').dropdown();
};

export function initializeTooltips() {
    window.initializeTooltips();
}

export function initializeDropdowns() {
    window.initializeDropdowns();
}

export function print() {
    console.log('Hello from interop.js');
}