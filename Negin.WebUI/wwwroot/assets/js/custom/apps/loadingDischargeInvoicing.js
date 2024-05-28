
$("#invoicing_btn").click(function () {
    Swal.fire({
        icon: "warning",
        title:"Are you sure you want to issue this invoice?",
        showCancelButton: !0,
        buttonsStyling: !1,
        confirmButtonText: "Yes, do it!",
        cancelButtonText: "No, cancel",
        customClass: { confirmButton: "btn fw-bold btn-success", cancelButton: "btn fw-bold btn-active-light-primary" },
    }).then(function (t) {
        t.isConfirmed
            ? $.ajax({
                url: "/Billing/LoadingDischargeInvoicing",
                type: "GET",
                success: function (res) {
                    if (res.state) {
                        Swal.fire({ text: "You have issued " + res.message + "!.", icon: "success", buttonsStyling: !1, confirmButtonText: "Ok, got it!", customClass: { confirmButton: "btn fw-bold btn-primary" } })
                            .then(function () {
                                window.location = "/Billing/LoadingDischargeInvoiceList"
                            })
                    } else {
                        Swal.fire({ text: res.message + "!.", icon: "warning", buttonsStyling: !1, confirmButtonText: "Ok, got it!", customClass: { confirmButton: "btn fw-bold btn-primary" } })
                            .then(function () {
                                window.location = "/Billing/LoadingDischargeInvoiceList"
                            })
                    }
                },
            }) : () => { }
    })
})
