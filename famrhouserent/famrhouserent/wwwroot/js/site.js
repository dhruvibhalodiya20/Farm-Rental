// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

// Add MVC specific functions
function handleAjaxError(xhr, status, error) {
    if (xhr.status === 401) {
        window.location.href = '/Auth/Login';
    } else {
        alert('An error occurred. Please try again.');
    }
}
