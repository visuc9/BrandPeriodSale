if (typeof Driven === "undefined") { Driven = {}; }
if (typeof Driven.App === "undefined") { Driven.App = {}; }
if (typeof Driven.App.BrandPeriodSalesReporting === "undefined") { Driven.App.BrandPeriodSalesReporting = {}; }
if (typeof Driven.App.BrandPeriodSalesReporting.Notifications === "undefined") { Driven.App.BrandPeriodSalesReporting.Notifications = {}; }

Driven.App.BrandPeriodSalesReporting.Notifications.EditLevelNotification = (function () {

    // private
    var _private = {

        View: {
            Level: "Level",
            LevelTitle: "LevelTitle",
            LevelTitleBase: "Non-Receipt of Sales - Level",
            CallTextTextBox: "CallText",
            EmailSubjectTextBox: "EmailSubject",
            EmailBodyTextBox: "EmailBody",

            EditLevelNotificationSubmitBegin: "Driven.App.BrandPeriodSalesReporting.Notifications.EditLevelNotification.SubmitBegin(xhr)",
            EditLevelNotificationSubmitSuccess: "Driven.App.BrandPeriodSalesReporting.Notifications.EditLevelNotification.SubmitSuccess(data, status, xhr)",
            EditLevelNotificationSubmitFailure: "Driven.App.BrandPeriodSalesReporting.Notifications.EditLevelNotification.SubmitFailure(xhr, status, error)",
            EditLevelNotificationSubmitComplete: "Driven.App.BrandPeriodSalesReporting.Notifications.EditLevelNotification.SubmitComplete(xhr, status)",

            EditorSkin: "bootstrapck",
            EditorSkinPath: "scripts/ckeditor-4.5.11/local_skins/bootstrapck/",
            EditorConfigMultiLinePath: "scripts/ckeditor-4.5.11/local_settings/config_multiline.js",
            EditorConfigMultiLineNoHtmlPath: "scripts/ckeditor-4.5.11/local_settings/config_multilinenohtml.js",
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

            $form.attr("data-ajax-begin", _private.View.EditLevelNotificationSubmitBegin);
            $form.attr("data-ajax-success", _private.View.EditLevelNotificationSubmitSuccess);
            $form.attr("data-ajax-failure", _private.View.EditLevelNotificationSubmitFailure);
            $form.attr("data-ajax-complete", _private.View.EditLevelNotificationSubmitComplete);

            var editorSkin = _private.View.EditorSkin + "," + Driven.App.BrandPeriodSalesReporting.GetRelativeUrl(_private.View.EditorSkinPath);

            CKEDITOR.replace(_private.View.EmailSubjectTextBox, {
                skin: editorSkin,
                customConfig: Driven.App.BrandPeriodSalesReporting.GetRelativeUrl(_private.View.EditorConfigSingleLinePath)
            });

            CKEDITOR.replace(_private.View.EmailBodyTextBox, {
                skin: editorSkin,
                customConfig: Driven.App.BrandPeriodSalesReporting.GetRelativeUrl(_private.View.EditorConfigMultiLinePath)
            });

            CKEDITOR.replace(_private.View.CallTextTextBox, {
                skin: editorSkin,
                customConfig: Driven.App.BrandPeriodSalesReporting.GetRelativeUrl(_private.View.EditorConfigMultiLineNoHtmlPath)
            });

            CKEDITOR.instances[_private.View.EmailSubjectTextBox].setData(decodeURI($("#" + _private.View.EmailSubjectTextBox).val()));
            CKEDITOR.instances[_private.View.EmailBodyTextBox].setData(decodeURI($("#" + _private.View.EmailBodyTextBox).val()));
            CKEDITOR.instances[_private.View.CallTextTextBox].setData(decodeURI($("#" + _private.View.CallTextTextBox).val()));

            $("#" + _private.View.LevelTitle).html(_private.View.LevelTitleBase + " " + $("#" + _private.View.Level).val());

            $form.find(':submit').click(_private.ValidateEditors);
        },


        ValidateEditors: function () {
            var $form = $("#" + _private.Settings.TargetId).find("form");
            $form.validate().settings.ignore = [];

            $("#" + _private.View.EmailSubjectTextBox).val(encodeURI(CKEDITOR.instances[_private.View.EmailSubjectTextBox].getData()));
            $("#" + _private.View.EmailBodyTextBox).val(encodeURI(CKEDITOR.instances[_private.View.EmailBodyTextBox].getData()));
            $("#" + _private.View.CallTextTextBox).val(encodeURI(CKEDITOR.instances[_private.View.CallTextTextBox].getData()));
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


