$(document).on("click", "#showmodalbtn", function () {
    var ProductID = $(this).data("productid");
    var colorid = $(this).data("colorid");
    var RP = $(this).data("salesrate");//RP
    var mrp = $(this).data("mrp");//MRP
    var creditsalesrate3 = $(this).data("creditsalesrate3");
    var creditsalesrate6 = $(this).data("creditsalesrate6");
    var creditsalesrate12 = $(this).data("creditsalesrate12");
    var productname = $(this).data("productname");
    $("#CashSalesRate").val(RP);
    $("#MRP").val(mrp);
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

    var RP = $("#CashSalesRate").val(); //RP
    var MRP = $("#MRP").val();
    var creditsalesrate3 = $("#CreditSalesRate3").val();
    var creditsalesrate6 = $("#CreditSalesRate6").val();
    var creditsalesrate12 = $("#CreditSalesRate12").val();

    var ProductID = getDefaultIntIfEmpty($("#ProductID").val());
    var ColorID = getDefaultIntIfEmpty($("#ColorID").val());
    var ConcernID = $(this).data("concernid");

    //if (ConcernID == 1 || ConcernID == 5 || ConcernID == 6) {
    //    if ((RP == "" || RP == 0) || (creditsalesrate3 == "" || creditsalesrate3 == 0) || (creditsalesrate6 == "" || creditsalesrate6 == 0) || (creditsalesrate12 == "" || creditsalesrate12 == 0)) {
    //        toastr.error("Please Enter Cash or Credit Sales Rate.");
    //        return;
    //    }

    //}
    //else {
    if ((RP == "" || RP == 0)) {
        toastr.error("Please Enter RP.");
        return;
    }
    if ((MRP == "" || MRP == 0)) {
        toastr.error("Please Enter MRP.");
        return;
    }

    if (creditsalesrate3 == "" || creditsalesrate3 == undefined) {
        creditsalesrate3 = 0;
    }
    if (creditsalesrate6 == "" || creditsalesrate6 == undefined) {
        creditsalesrate6 = 0;
    }
    if (creditsalesrate12 == "" || creditsalesrate12 == undefined) {
        creditsalesrate12 = 0;
    }
    //}

    var SalesRateModel = {
        'ProductID': ProductID,
        'ColorID': ColorID,
        'SalesRate': MRP,
        'CreditSalesRate3': creditsalesrate3,
        'CreditSalesRate6': creditsalesrate6,
        'CreditSalesRate12': creditsalesrate12,
        'MRP': RP
    };

    $.ajax({
        url: "/Stock/UpdateEmobileSalesRate/",
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
    $("#RP").val("");
    $("#CreditSalesRate3").val("");
    $("#CreditSalesRate6").val("");
    $("#CreditSalesRate12").val("");
}