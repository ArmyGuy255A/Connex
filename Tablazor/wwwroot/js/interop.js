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

// window.initializeDropdowns = function () {
//     $(function () {
//         $('.dropdown-toggle').each(function () {
//             new bootstrap.Dropdown(this);
//         });
//     });
// };


export function initializeTooltips() {
    window.initializeTooltips();
}

export function initializeDropdowns() {
    window.initializeDropdowns();

    // document.querySelector('.dropdown-toggle').addEventListener('click', function() {
    //     console.log('Dropdown clicked');
    // });

    // $('.dropdown-toggle').click(function() {
    //     console.log('Dropdown clicked---');
    //     $(this).next('.dropdown-menu').toggle();
    //
    // });
    $('.dropdown-toggle').click(function(event) {
        event.stopPropagation();
        console.log('Dropdown clicked');
        // Rest of your code
    });
    
    $('.dropdown-toggle').on('show.bs.dropdown', function () {
        console.log('Dropdown is about to be shown');
    });

    $('.dropdown-toggle').on('shown.bs.dropdown', function (event) {
        console.log('Dropdown is shown');
    });

    $('.dropdown-toggle').on('hide.bs.dropdown', function (event) {
        console.log('Dropdown is about to be hidden');
        
    });

    $('.dropdown-toggle').on('hidden.bs.dropdown', function (event) {
        console.log('Dropdown is hidden');
        
    });


}
export function print() {
    console.log('Hello from interop.js');
}