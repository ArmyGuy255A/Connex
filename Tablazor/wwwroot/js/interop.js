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

export function getPageTitle() {
    return document.title;
}

export function postLoginData(url, userName, password) {
    const data = { userName: userName, password: password };
    return fetch(url, {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json'
        },
        credentials: 'include',  // Important for ASP.NET Core and CORS
        body: JSON.stringify(data)
    }).then(response => response.json());
}
