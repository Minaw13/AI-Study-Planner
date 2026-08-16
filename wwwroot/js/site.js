// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

document.addEventListener("DOMContentLoaded", function () {

    const animatedElements = document.querySelectorAll(
        ".animated-heading, .animated-feature"
    );

    const observer = new IntersectionObserver(function (entries, observer) {

        entries.forEach(function (entry) {

            if (entry.isIntersecting) {

                entry.target.classList.add("animate-in");

                observer.unobserve(entry.target);
            }

        });

    }, {
        threshold: 0.2
    });

    animatedElements.forEach(function (element) {
        observer.observe(element);
    });

});