if (typeof Driven === "undefined") { Driven = {}; }
if (typeof Driven.App === "undefined") { Driven.App = {}; }
if (typeof Driven.App.BrandPeriodSalesReporting === "undefined") { Driven.App.BrandPeriodSalesReporting = {}; }
if (typeof Driven.App.BrandPeriodSalesReporting.SubmitCorrections === "undefined") { Driven.App.BrandPeriodSalesReporting.SubmitCorrections = {}; }

Driven.App.BrandPeriodSalesReporting.SubmitCorrections.CreateToplineCorrection = (function () {

    // private
    var _private = {

        View: {
            Title: "Title",
            ModalTitle: "ToplineModalTitle",

            CreateToplineCorrectionSubmitBegin: "Driven.App.BrandPeriodSalesReporting.SubmitCorrections.CreateToplineCorrection.SubmitBegin(xhr)",
            CreateToplineCorrectionSubmitSuccess: "Driven.App.BrandPeriodSalesReporting.SubmitCorrections.CreateToplineCorrection.SubmitSuccess(data, status, xhr)",
            CreateToplineCorrectionSubmitFailure: "Driven.App.BrandPeriodSalesReporting.SubmitCorrections.CreateToplineCorrection.SubmitFailure(xhr, status, error)",
            CreateToplineCorrectionSubmitComplete: "Driven.App.BrandPeriodSalesReporting.SubmitCorrections.CreateToplineCorrection.SubmitComplete(xhr, status)"
        },

        Settings: { Url: "", TargetId: "", ModalId: "" },


        ChangeStore: function (localStoreId) {
            var periodEndDate = $("#PeriodEndDate").val();
            var url = Driven.App.BrandPeriodSalesReporting.GetRelativeUrl("SalesCorrection/GetProductGroups");

            // clear sales
            var listItems = $("#productGroupsTable").find('[data-salesprodsubgrpid]');
            listItems.each(function (idx, txt) {
                var sales = $(txt);
                var subGrpId = sales.data("salesprodsubgrpid");
                sales.val("");
            });

            // clear tickets
            var listItems = $("#productGroupsTable").find('[data-ticketsprodsubgrpid]');
            listItems.each(function (idx, txt) {
                var sales = $(txt);
                var subGrpId = sales.data("ticketsprodsubgrpid");
                sales.val("");
            });

            if (localStoreId) {
                Driven.App.BrandPeriodSalesReporting.SetLoading(true);

                var params = {};
                params["localstoreid"] = localStoreId;
                params["periodenddate"] = periodEndDate;

                Driven.App.BrandPeriodSalesReporting.SetLoading(true);

                $.getJSON(url, params)
                .done(function (data, textStatus, jqXHR) {

                    for (var i = 0; i < data.ProductGroups.length; i++) {
                        var o = data.ProductGroups[i];
                        $($("#productGroupsTable").find('[data-ticketsprodsubgrpid="' + o.ProdSubGrpID + '"]')[0]).val(o.TotalTickets);
                        $($("#productGroupsTable").find('[data-salesprodsubgrpid="' + o.ProdSubGrpID + '"]')[0]).val(o.NetSales);
                    }

                    $("#NetSales").val(data.NetSales);
                    $("#TotalTickets").val(data.TotalTickets);
                })
                .fail(function (jqXHR, textStatus, errorThrown) {
                    Driven.App.BrandPeriodSalesReporting.SetMessage(false, Driven.App.BrandPeriodSalesReporting.FormatErrorMessage(jqXHR, textStatus));
                })
                .always(function () {
                    Driven.App.BrandPeriodSalesReporting.SetLoading(false);
                });
            }

        },


        Init: function (settings) {
            $.ajaxSetup({ cache: false });
            _private.Settings = settings;
        },


        InitForm: function () {
            var containerId = _private.Settings.TargetId;
            var url = Driven.App.BrandPeriodSalesReporting.GetRelativeUrl(_private.Settings.Url);

            Driven.App.BrandPeriodSalesReporting.InitValidators(containerId);

            var $form = $("#" + containerId).find("form");

            $form.attr("action", url);
            $form.attr("method", "POST");
            $form.attr("data-ajax-method", "POST");
            $form.attr("data-ajax-update", "#" + containerId);

            $form.attr("data-ajax-begin", _private.View.CreateToplineCorrectionSubmitBegin);
            $form.attr("data-ajax-success", _private.View.CreateToplineCorrectionSubmitSuccess);
            $form.attr("data-ajax-failure", _private.View.CreateToplineCorrectionSubmitFailure);
            $form.attr("data-ajax-complete", _private.View.CreateToplineCorrectionSubmitComplete);

            $("#" + _private.View.ModalTitle).html($("#" + _private.View.Title).val());
            //$("#LocalStoreId").change(function () { _private.ChangeStore($(this).val()) });
        },


        OpenModal: function (params) {
            var url = Driven.App.BrandPeriodSalesReporting.GetRelativeUrl(_private.Settings.Url);
            Driven.App.BrandPeriodSalesReporting.SetLoading(true);

            $.ajax(url, { cache: false, data: params })
                .done(_private.OpenModal_Done)
                .fail(_private.OpenModal_Fail)
                .always(_private.OpenModal_Always);
        },


        OpenModal_Done: function (data, textStatus, jqXHR) {
            $("#" + _private.Settings.TargetId).html(data);
            $("#" + _private.Settings.ModalId).modal('show');
            _private.InitForm();
        },


        OpenModal_Fail: function (jqXHR, textStatus, errorThrown) {
            Driven.App.BrandPeriodSalesReporting.SetMessage(false, Driven.App.BrandPeriodSalesReporting.FormatErrorMessage(jqXHR, textStatus));
        },


        OpenModal_Always: function (data, textStatus, jqXHR) {
            Driven.App.BrandPeriodSalesReporting.SetLoading(false);
        },


        Submit_Begin: function (xhr) { },
        Submit_Success: function (data, status, xhr) { },
        Submit_Failure: function (jqXHR, textStatus, errorThrown) { },
        Submit_Complete: function (xhr, status) { }
    };


    // public
    var _public = {
        Init: _private.Init,
        InitForm: _private.InitForm,

        OpenModal: _private.OpenModal,

        SubmitBegin: _private.SubmitBegin,
        SubmitSuccess: _private.SubmitSuccess,
        SubmitFailure: _private.SubmitFailure,
        SubmitComplete: _private.SubmitComplete
    };

    return _public;

})();
