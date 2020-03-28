// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your Javascript code.
(function ($) {
    'use strict';

    $('form#changeCulture a.submit').click(function (e) {
        $(this).parents('form:first').submit();
        return false;
    });
}(jQuery));