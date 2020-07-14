if (typeof Driven === "undefined") { Driven = {}; }
if (typeof Driven.App === "undefined") { Driven.App = {}; }
if (typeof Driven.App.BrandPeriodSalesReporting === "undefined") { Driven.App.BrandPeriodSalesReporting = {}; }
if (typeof Driven.App.BrandPeriodSalesReporting.SubmitCorrections === "undefined") { Driven.App.BrandPeriodSalesReporting.SubmitCorrections = {}; }

Driven.App.BrandPeriodSalesReporting.SubmitCorrections.EditToplineCorrection = (function () {

    // private
    var _private = {

        View: {
            SaveButton: "SaveBtn",
            Title: "Title",
            ModalTitle: "ToplineModalTitle",
            NetSalesWarning: false,
            CalcRoyaltyFeeWarning: false,
            CalcAdvertisingFeeWarning: false,

            EditToplineCorrectionSubmitBegin: "Driven.App.BrandPeriodSalesReporting.SubmitCorrections.EditToplineCorrection.SubmitBegin(xhr)",
            EditToplineCorrectionSubmitSuccess: "Driven.App.BrandPeriodSalesReporting.SubmitCorrections.EditToplineCorrection.SubmitSuccess(data, status, xhr)",
            EditToplineCorrectionSubmitFailure: "Driven.App.BrandPeriodSalesReporting.SubmitCorrections.EditToplineCorrection.SubmitFailure(xhr, status, error)",
            EditToplineCorrectionSubmitComplete: "Driven.App.BrandPeriodSalesReporting.SubmitCorrections.EditToplineCorrection.SubmitComplete(xhr, status)"
        },

        Settings: { Url: "", TargetId: "", ModalId: "" },


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

            $form.attr("data-ajax-begin", _private.View.EditToplineCorrectionSubmitBegin);
            $form.attr("data-ajax-success", _private.View.EditToplineCorrectionSubmitSuccess);
            $form.attr("data-ajax-failure", _private.View.EditToplineCorrectionSubmitFailure);
            $form.attr("data-ajax-complete", _private.View.EditToplineCorrectionSubmitComplete);

            $("#" + _private.View.ModalTitle).html($("#" + _private.View.Title).val());
            $("#" + _private.View.SaveButton).click(_private.CheckWarnings);
        },


        CheckWarnings: function () {
            var containerId = _private.Settings.TargetId;
            var $form = $("#" + containerId).find("form");
            _private.MarkButtonType(false);
        },


        MarkButtonType: function (isSaveAndApprove) {
            if (isSaveAndApprove) {
                bootbox.confirm("Submit for Approval?", function (result) {
                    if (result) {
                        $('#form0').submit();
                    }
                });
            }
            else {
                $('#form0').submit();
            }
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
