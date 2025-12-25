$.fn.select2.defaults.set("theme", "bootstrap");
// Add slideDown animation to dropdown
$('.dropdown').on('show.bs.dropdown', function (e) {
    $(this).find('.dropdown-menu').first().stop(true, true).slideDown(300);
});
// Add slideUp animation to dropdown
$('.dropdown').on('hide.bs.dropdown', function (e) {
    $(this).find('.dropdown-menu').first().stop(true, true).slideUp(300);
});
function rowStyle(row, index) {
    var classes = ['info', 'info'];

    if (index % 2 === 0 && index / 2 < classes.length) {
        return {
            classes: classes[index / 2]
        };
    }
    return {};
}
function sorter(a, b) {
    if (a < b) return 1;
    if (a > b) return -1;
    return 0;
}
function queryParams() {
    return {
        type: 'owner',
        sort: 'updated',
        direction: 'desc',
        per_page: 1,
        page: 1
    };
}
function detailFormatter(index, row) {
    var html = [];
    $.each(row, function (key, value) {
        html.push('<p><b>' + key + ':</b> ' + value + '</p>');
    });
    return html.join('');
}
$(function () {
    if (window.location.pathname.toLowerCase().indexOf("/account/login") != -1) {
        $("body").addClass("login-bg");
        $("#footer").addClass("login-footer-bg");
    }
    else {
        $("#navbar").addClass("navbr-bg");
    }
    if (window.location.pathname.toLowerCase().indexOf("/creditsalesorder/create") != -1 ||
        window.location.pathname.toLowerCase().indexOf("/creditsalesorder/edit") != -1 ||
        window.location.pathname.toLowerCase().indexOf("/salesorder/create") != -1 ||
        window.location.pathname.toLowerCase().indexOf("/salesorder/edit") != -1 ||
        window.location.pathname.toLowerCase().indexOf("/purchaseorder/create") != -1 ||
        window.location.pathname.toLowerCase().indexOf("/poreturn/create") != -1 ||
        window.location.pathname.toLowerCase().indexOf("/transfer/create") != -1 ||
        window.location.pathname.toLowerCase().indexOf("/purchaseorder/edit") != -1) {
        $(".col-md-6").css("padding-left", "5px");
        $(".col-md-6").css("padding-right", "5px");
    }
})

function getDefaultFloatIfEmpty(val) {
    if (val == undefined || val == '' || isNaN(val))
        return 0.0;
    else
        return parseFloat(val);
}

function getDefaultIntIfEmpty(val) {
    if (val == undefined || val == '' || isNaN(val))
        return 0;
    else
        return parseInt(val);
}
$(function () {
    $('.bootstrap-table table tbody tr td:last-child a:first-child').addClass('btn btn-xs btn-primary');
    $('.bootstrap-table table tbody tr td:last-child a:nth-child(2)').addClass('btn btn-xs btn-info');
    $('.bootstrap-table table tbody tr td:last-child a:nth-child(3)').addClass('btn btn-xs btn-danger');
    $('.bootstrap-table table tbody tr td:last-child a:nth-child(4)').addClass('btn btn-xs btn-default');
    $('.bootstrap-table table tbody tr td:last-child a:nth-child(5)').addClass('btn btn-xs btn-info');
    $('.bootstrap-table table tbody tr td:last-child a:nth-child(6)').addClass('btn btn-xs btn-danger');
    $('.bootstrap-table table tbody tr td:last-child a:nth-child(7)').addClass('btn btn-xs btn-default');
})


$(document).on('click', 'input[type="number"]', function () {
    $(this).select();
})


$(".Transfer").addClass("label label-info");
$(".Return").addClass("label label-danger");


$(".table").addClass("text-nowrap");
$("h4").addClass("inline-header");

$(".Success").addClass("label label-info");
$(".Fail").addClass("label label-danger");

$(".Sales").addClass("label label-info");
$(".Return").addClass("label label-danger");
$(".ProductReturn").addClass("label label-warning");
$(".Pending").addClass("label label-primary");
toastr.options.progressBar = true;
