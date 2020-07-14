if (typeof Driven === "undefined") { Driven = {}; }
if (typeof Driven.App === "undefined") { Driven.App = {}; }
if (typeof Driven.App.BrandPeriodSalesReporting === "undefined") { Driven.App.BrandPeriodSalesReporting = {}; }
if (typeof Driven.App.BrandPeriodSalesReporting.Notifications === "undefined") { Driven.App.BrandPeriodSalesReporting.Notifications = {}; }

Driven.App.BrandPeriodSalesReporting.Notifications.EditBrandNotification = (function () {

    // private
    var _private = {

        View: {
            RecipientsTextBox: "Recipients",
            EmailSubjectTextBox: "EmailSubject",
            EmailBodyTextBox: "EmailBody",

            EditBrandNotificationSubmitBegin: "Driven.App.BrandPeriodSalesReporting.Notifications.EditBrandNotification.SubmitBegin(xhr)",
            EditBrandNotificationSubmitSuccess: "Driven.App.BrandPeriodSalesReporting.Notifications.EditBrandNotification.SubmitSuccess(data, status, xhr)",
            EditBrandNotificationSubmitFailure: "Driven.App.BrandPeriodSalesReporting.Notifications.EditBrandNotification.SubmitFailure(xhr, status, error)",
            EditBrandNotificationSubmitComplete: "Driven.App.BrandPeriodSalesReporting.Notifications.EditBrandNotification.SubmitComplete(xhr, status)",

            EditorSkin: "bootstrapck",
            EditorSkinPath: "scripts/ckeditor-4.5.11/local_skins/bootstrapck/",

            EditorConfigMultiLinePath: "scripts/ckeditor-4.5.11/local_settings/config_multiline.js",
            EditorConfigSingleLinePath: "scripts/ckeditor-4.5.11/local_settings/config_singleline.js",
            EditorConfigSingleLineNonePath: "scripts/ckeditor-4.5.11/local_settings/config_singlelinenone.js"
        },


        Settings: { Url: "", TargetId: "", ModalId: "" },


        Init: function (settings) {
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

            $form.attr("data-ajax-begin", _private.View.EditBrandNotificationSubmitBegin);
            $form.attr("data-ajax-success", _private.View.EditBrandNotificationSubmitSuccess);
            $form.attr("data-ajax-failure", _private.View.EditBrandNotificationSubmitFailure);
            $form.attr("data-ajax-complete", _private.View.EditBrandNotificationSubmitComplete);

            var editorSkin = _private.View.EditorSkin + "," + Driven.App.BrandPeriodSalesReporting.GetRelativeUrl(_private.View.EditorSkinPath);

            CKEDITOR.replace(_private.View.EmailSubjectTextBox, {
                skin: editorSkin,
                customConfig: Driven.App.BrandPeriodSalesReporting.GetRelativeUrl(_private.View.EditorConfigSingleLinePath)
            });

            CKEDITOR.replace(_private.View.EmailBodyTextBox, {
                skin: editorSkin,
                customConfig: Driven.App.BrandPeriodSalesReporting.GetRelativeUrl(_private.View.EditorConfigMultiLinePath)
            });

            CKEDITOR.replace(_private.View.RecipientsTextBox, {
                skin: editorSkin,
                customConfig: Driven.App.BrandPeriodSalesReporting.GetRelativeUrl(_private.View.EditorConfigSingleLineNonePath)
            });

            CKEDITOR.instances[_private.View.RecipientsTextBox].setData(decodeURI($("#" + _private.View.RecipientsTextBox).val()));
            CKEDITOR.instances[_private.View.EmailSubjectTextBox].setData(decodeURI($("#" + _private.View.EmailSubjectTextBox).val()));
            CKEDITOR.instances[_private.View.EmailBodyTextBox].setData(decodeURI($("#" + _private.View.EmailBodyTextBox).val()));

            $form.find(':submit').click(_private.ValidateEditors);
        },


        ValidateEditors: function () {
            var $form = $("#" + _private.Settings.TargetId).find("form");
            $form.validate().settings.ignore = [];

            $("#" + _private.View.RecipientsTextBox).val(encodeURI(CKEDITOR.instances[_private.View.RecipientsTextBox].getData()));
            $("#" + _private.View.EmailSubjectTextBox).val(encodeURI(CKEDITOR.instances[_private.View.EmailSubjectTextBox].getData()));
            $("#" + _private.View.EmailBodyTextBox).val(encodeURI(CKEDITOR.instances[_private.View.EmailBodyTextBox].getData()));
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
