var x = [];
var e = !1;
var y = [];
var t = function (e, t) {
    var n = {};
    e.hasAttribute("data-bs-delay-hide") && (n.hide = e.getAttribute("data-bs-delay-hide")),
        e.hasAttribute("data-bs-delay-show") && (n.show = e.getAttribute("data-bs-delay-show")),
        n && (t.delay = n),
        e.hasAttribute("data-bs-dismiss") && "click" == e.getAttribute("data-bs-dismiss") && (t.dismiss = "click");
    var i = new bootstrap.Tooltip(e, t);
    return (
        t.dismiss &&
        "click" === t.dismiss &&
        e.addEventListener("click", function (e) {
            i.hide();
        }),
        i
    );
}
x.slice.call(document.querySelectorAll('[data-bs-toggle="tooltip"]')).map(function (e) { t(e, {}) });

"undefined" != typeof jQuery &&
    void 0 !== $.fn.select2 &&
    (y.slice.call(document.querySelectorAll('[data-control="select2"], [data-kt-select2="true"]')).map(function (e) {
        var t = { dir: document.body.getAttribute("direction") };
        "true" == e.getAttribute("data-hide-search") && (t.minimumResultsForSearch = 1 / 0), $(e).select2(t);
    }),
        !1 === e &&
        ((e = !0),
            $(document).on("select2:open", function (e) {
                var t = document.querySelectorAll(".select2-container--open .select2-search__field");
                t.length > 0 && t[t.length - 1].focus();
            })
        )
    );

