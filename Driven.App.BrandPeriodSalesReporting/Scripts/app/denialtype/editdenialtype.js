if (typeof Driven === "undefined") { Driven = {}; }
if (typeof Driven.App === "undefined") { Driven.App = {}; }
if (typeof Driven.App.BrandPeriodSalesReporting === "undefined") { Driven.App.BrandPeriodSalesReporting = {}; }
if (typeof Driven.App.BrandPeriodSalesReporting.DenialType === "undefined") { Driven.App.BrandPeriodSalesReporting.DenialType = {}; }

Driven.App.BrandPeriodSalesReporting.DenialType.EditDenialType = (function () {

    // private
    var _private = {

        View: {
            EditDenialTypeSubmitBegin: "Driven.App.BrandPeriodSalesReporting.DenialType.EditDenialType.SubmitBegin(xhr)",
            EditDenialTypeSubmitSuccess: "Driven.App.BrandPeriodSalesReporting.DenialType.EditDenialType.SubmitSuccess(data, status, xhr)",
            EditDenialTypeSubmitFailure: "Driven.App.BrandPeriodSalesReporting.DenialType.EditDenialType.SubmitFailure(xhr, status, error)",
            EditDenialTypeSubmitComplete: "Driven.App.BrandPeriodSalesReporting.DenialType.EditDenialType.SubmitComplete(xhr, status)"
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

            $form.attr("data-ajax-begin", _private.View.EditDenialTypeSubmitBegin);
            $form.attr("data-ajax-success", _private.View.EditDenialTypeSubmitSuccess);
            $form.attr("data-ajax-failure", _private.View.EditDenialTypeSubmitFailure);
            $form.attr("data-ajax-complete", _private.View.EditDenialTypeSubmitComplete);
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
