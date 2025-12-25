$(document).on("click", "#showmodalbtn", function () {
    var ProductID = $(this).data("productid");
    var colorid = $(this).data("colorid");
    var cashsalesRate = $(this).data("salesrate");
    var creditsalesrate3 = $(this).data("creditsalesrate3");
    var creditsalesrate6 = $(this).data("creditsalesrate6");
    var creditsalesrate12 = $(this).data("creditsalesrate12");
    var productname = $(this).data("productname");
    $("#CashSalesRate").val(cashsalesRate);
    $("#CreditSalesRate3").val(creditsalesrate3);
    $("#CreditSalesRate6").val(creditsalesrate6);
    $("#CreditSalesRate12").val(creditsalesrate12);
    $("#ProductID").val(ProductID);
    $("#ColorID").val(colorid);
    $("#ProductName").val(productname);
    $("#myModal").modal("show");
    //alert(ProductID);
});


$(document).on("click", "#Submitbtn", function () {

    var cashsalesRate = $("#CashSalesRate").val();
    var creditsalesrate3 = $("#CreditSalesRate3").val();
    var creditsalesrate6 = $("#CreditSalesRate6").val();
    var creditsalesrate12 = $("#CreditSalesRate12").val();

    var ProductID = getDefaultIntIfEmpty($("#ProductID").val());
    var ColorID = getDefaultIntIfEmpty($("#ColorID").val());
    var ConcernID = $(this).data("concernid");

    if (ConcernID == 1 || ConcernID == 5 || ConcernID == 6) {
        if ((cashsalesRate == "" || cashsalesRate == 0) || (creditsalesrate3 == "" || creditsalesrate3 == 0) || (creditsalesrate6 == "" || creditsalesrate6 == 0) || (creditsalesrate12 == "" || creditsalesrate12 == 0)) {
            toastr.error("Please Enter Cash or Credit Sales Rate.");
            return;
        }

    }
    else {
        if ((cashsalesRate == "" || cashsalesRate == 0)) {
            toastr.error("Please Enter Cash Sales Rate.");
            return;
        }
        if (creditsalesrate3 == "" || creditsalesrate3 == undefined) {
            creditsalesrate3 = cashsalesRate;
        }
        if (creditsalesrate6 == "" || creditsalesrate6 == undefined) {
            creditsalesrate6 = cashsalesRate;
        }
        if (creditsalesrate12 == "" || creditsalesrate12 == undefined) {
            creditsalesrate12 = cashsalesRate;
        }
    }

    var SalesRateModel = {
        'ProductID': ProductID,
        'ColorID': ColorID,
        'SalesRate': cashsalesRate,
        'CreditSalesRate3': creditsalesrate3,
        'CreditSalesRate6': creditsalesrate6,
        'CreditSalesRate12': creditsalesrate12
    };

    $.ajax({
        url: "/Stock/UpdateSalesRate/",
        type: "POST",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        data: JSON.stringify(SalesRateModel),
        success: function (data) {
            //console.log(data);
            if (data == true) {
                $("#myModal").modal("hide");
                ClearTextBox();
            }
            location.reload();
        },
        error: function (data) {
            console.log(data);
            toastr.error("Update Failed");
            location.reload();
        }

    });

});





function ClearTextBox() {
    $("#ProductID").val("");
    $("#ColorID").val("");
    $("#ProductName").val("");
    $("#CashSalesRate").val("");
    $("#CreditSalesRate3").val("");
    $("#CreditSalesRate6").val("");
    $("#CreditSalesRate12").val("");
}