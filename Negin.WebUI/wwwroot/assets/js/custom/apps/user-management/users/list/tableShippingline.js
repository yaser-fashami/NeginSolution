"use strict"; var KTShippingLineList = function () { var t, e = document.getElementById("kt_datatable_responsive"), n = () => { e.querySelectorAll('[data-kt-shippingline-table-filter="delete_row"]').forEach(t => { t.addEventListener("click", function (t) { t.preventDefault(); let e = t.target.closest("tr"), n = e.querySelectorAll("td")[0].innerText, o = $(e.querySelector('.id[type="hidden"]')).val(); Swal.fire({ text: "Are you sure you want to delete " + n + "?", icon: "warning", showCancelButton: !0, buttonsStyling: !1, confirmButtonText: "Yes, delete!", cancelButtonText: "No, cancel", customClass: { confirmButton: "btn fw-bold btn-danger", cancelButton: "btn fw-bold btn-active-light-primary" } }).then(function (t) { t.isConfirmed ? $.ajax({ url: "/BasicInfo/DeleteShippingLine", type: "GET", data: { id: o }, success: function () { Swal.fire({ text: "You have deleted " + n + "!.", icon: "success", buttonsStyling: !1, confirmButtonText: "Ok, got it!", customClass: { confirmButton: "btn fw-bold btn-primary" } }).then(function () { location.reload() }).then(function () { l() }) } }) : Swal.fire({ text: customerName + " was not deactive.", icon: "error", buttonsStyling: !1, confirmButtonText: "Ok, got it!", customClass: { confirmButton: "btn fw-bold btn-primary" } }) }) }) }) }; return { init: function () { e && (t = $(e).DataTable({ info: !1, order: [], pageLength: 10, lengthChange: !1, columnDefs: [{ orderable: !1, targets: 0 }, { orderable: !1, targets: 6 },] })).on("draw", function () { a(), n(), l() }), n() } } }(); KTUtil.onDOMContentLoaded(function () { KTShippingLineList.init() });