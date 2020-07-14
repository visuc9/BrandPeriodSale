if (typeof Driven === "undefined") { Driven = {}; }
if (typeof Driven.App === "undefined") { Driven.App = {}; }
if (typeof Driven.App.BrandPeriodSalesReporting === "undefined") { Driven.App.BrandPeriodSalesReporting = {}; }

Driven.App.BrandPeriodSalesReporting.Notifications = (function () {

    // private
    var _private = {

        View: {
            ListNotificationsTableId: "NotificationsTable",
            ListNotificationsTableSpinner: "NotificationsTableSpinner",
            ListNotificationsTableContainer: "NotificationsTableContainer",
            ListNotificationsUrl: "Notification2/GetNotifications",
            ListNotificationsLength: 10,

            DialogResult: "DialogResult",
            SecSubBrandId: "SubBrandId",
            SecTokenName: "__RequestVerificationToken",
            SecTokenValue: "__AjaxAntiForgeryForm",

            EditLevel1NotificationButton: "EditLevel1NotificationBtn",
            EditLevel2NotificationButton: "EditLevel2NotificationBtn",
            EditLevel3NotificationButton: "EditLevel3NotificationBtn",
            EditLevelNotificationUrl: "Notification2/EditLevelNotification",
            EditLevelNotificationKey: "SubBrandId",
            EditLevelNotificationLevel: "Level",
            EditLevelNotificationModalId: "EditNotificationModal",
            EditLevelNotificationTargetId: "EditNotificationPartialView",

            EditBrandNotificationButton: "EditBrandNotificationBtn",
            EditBrandNotificationUrl: "Notification2/EditBrandNotification",
            EditBrandNotificationKey: "SubBrandId",
            EditBrandNotificationModalId: "EditNotificationModal",
            EditBrandNotificationTargetId: "EditNotificationPartialView",

            EditPluginScript: "plugin.js",
            EditPluginPlaceholderName: "placeholder",
            EditPluginPlaceholderPath: "scripts/ckeditor-4.5.11/local_plugins/placeholder/",
            EditPluginPlaceholderSelectName: "placeholder_select",
            EditPluginPlaceholderSelectPath: "scripts/ckeditor-4.5.11/local_plugins/placeholder_select/",
            EditPluginDoNothingName: "doNothing",
            EditPluginDoNothingPath: "scripts/ckeditor-4.5.11/local_plugins/donothing/",
            EditPluginElementsPathCustomName: "elementspath_custom",
            EditPluginElementsPathCustomPath: "scripts/ckeditor-4.5.11/local_plugins/elementspath_custom/"
            //scrollbar bug in chrome browser - https://dev.ckeditor.com/ticket/14716
        },


        InitList: function () {
            var tableId = _private.View.ListNotificationsTableId;

            var table = $("#" + tableId).dataTable({
                "bServerSide": false,
                "iDisplayLength": _private.View.ListNotificationsLength,
                "bLengthChange": false,
                "aaSorting": [],
                "paging": true,
                "ordering": true,
                "info": true,
                "searching": false,
                "language": { "emptyTable": "No available Locations" },
                "columnDefs": [
                    { "targets": 3, "orderable": false, "sClass": "center" },
                    { "targets": 4, "orderable": false, "sClass": "center" }
                ]
            });
        },


        InitEditors: function () {
            CKEDITOR.plugins.addExternal(_private.View.EditPluginPlaceholderName, Driven.App.BrandPeriodSalesReporting.GetRelativeUrl(_private.View.EditPluginPlaceholderPath), _private.View.EditPluginScript);
            CKEDITOR.plugins.addExternal(_private.View.EditPluginPlaceholderSelectName, Driven.App.BrandPeriodSalesReporting.GetRelativeUrl(_private.View.EditPluginPlaceholderSelectPath), _private.View.EditPluginScript);
            CKEDITOR.plugins.addExternal(_private.View.EditPluginDoNothingName, Driven.App.BrandPeriodSalesReporting.GetRelativeUrl(_private.View.EditPluginDoNothingPath), _private.View.EditPluginScript);
            CKEDITOR.plugins.addExternal(_private.View.EditPluginElementsPathCustomName, Driven.App.BrandPeriodSalesReporting.GetRelativeUrl(_private.View.EditPluginElementsPathCustomPath), _private.View.EditPluginScript);

            CKEDITOR.addCss('.cke_editable p{margin:0; vertical-align:middle; padding-top:6px; padding-right:12px; padding-bottom:6px; padding-left:12px} .cke_editable {  /* white-space: nowrap; overflow-y: auto; */ color:#555555;  font-size: 14px; margin: 0px; padding: 0px }');

            CKEDITOR.on('dialogDefinition', function (ev) {
                ev.data.definition.resizable = CKEDITOR.DIALOG_RESIZE_NONE;
            });

            $.fn.modal.Constructor.prototype.enforceFocus = function () {
                modal_this = this
                $(document).on('focusin.modal', function (e) {
                    if (modal_this.$element[0] !== e.target && !modal_this.$element.has(e.target).length
                    && !$(e.target.parentNode).hasClass('cke_dialog_ui_input_select')
                    && $(e.target.parentNode).hasClass('cke_contents cke_reset')
                    && !$(e.target.parentNode).hasClass('cke_dialog_ui_input_text')) {
                        modal_this.$element.focus()
                    }
                })
            };
        },


        GetDataTable: function (id) {
            return $("#" + id).DataTable();
        },


        ProcessList: function () {
            var tableId = _private.View.ListNotificationsTableId;
            var url = Driven.App.BrandPeriodSalesReporting.GetRelativeUrl(_private.View.ListNotificationsUrl);
            var params = {};

            Driven.App.BrandPeriodSalesReporting.SetLoading(true);

            $.getJSON(url, params)
            .done(function (data, textStatus, jqXHR) {
                _private.FillNotificationsTable(tableId, data);
            })
            .fail(function (jqXHR, textStatus, errorThrown) {
                Driven.App.BrandPeriodSalesReporting.SetMessage(false, Driven.App.BrandPeriodSalesReporting.FormatErrorMessage(jqXHR, textStatus));
            })
            .always(function () {
                Driven.App.BrandPeriodSalesReporting.SetLoading(false);
                $("#" + _private.View.ListNotificationsTableContainer).show();
                $("#" + _private.View.ListNotificationsTableSpinner).hide();
                _private.GetDataTable(tableId).responsive.recalc();
            });
        },


        FillNotificationsTable: function (tableId, json) {
            var $table = $("#" + tableId).dataTable();
            $table.fnClearTable();

            var tableData = [];
            for (var i = 0; i < json.length; i++) {
                var o = json[i];

                var shopId = "";
                var locationStoreId = "";
                var storeStatus = "";
                var excludeEmail = "";
                var excludeCall = "";

                tableData.push([shopId, locationStoreId, storeStatus, excludeEmail, excludeCall]);
            }

            if (tableData.length > 0) {
                $table.fnAddData(tableData);
            }
        },


        OpenEditBrandNotification: function (e) {
            Driven.App.BrandPeriodSalesReporting.Notifications.EditBrandNotification.Init({
                Url: _private.View.EditBrandNotificationUrl,
                ModalId: _private.View.EditBrandNotificationModalId,
                TargetId: _private.View.EditBrandNotificationTargetId
            });

            var params = {};
            params[_private.View.EditBrandNotificationKey] = $("#" + _private.View.SecSubBrandId).val();
            Driven.App.BrandPeriodSalesReporting.Notifications.EditBrandNotification.OpenModal(params);
        },


        EditBrandNotification_SubmitBegin: function (xhr) {
            Driven.App.BrandPeriodSalesReporting.SetLoading(true);
            $("#" + _private.View.EditBrandNotificationModalId).find(':submit').prop('disabled', true);
        },


        EditBrandNotification_SubmitComplete: function (xhr, status) {
            Driven.App.BrandPeriodSalesReporting.SetLoading(false);
        },


        EditBrandNotification_SubmitFailure: function (jqXHR, textStatus, errorThrown) {
            Driven.App.BrandPeriodSalesReporting.SetMessage(false, Driven.App.BrandPeriodSalesReporting.FormatErrorMessage(jqXHR, textStatus));
        },


        EditBrandNotification_SubmitSuccess: function (data, status, xhr) {
            var $dialogContainer = $("#" + _private.View.EditBrandNotificationModalId);
            var $form = $dialogContainer.find("form");
            var result = JSON.parse($form.find("#" + _private.View.DialogResult).val());

            if (result.Success == true) {
                $dialogContainer.modal('hide');
                Driven.App.BrandPeriodSalesReporting.SetMessage(result.Success, result.Message);
                _private.ProcessList();
            }
            else {
                Driven.App.BrandPeriodSalesReporting.Notifications.EditBrandNotification.InitForm();
            }
        },


        OpenEditLevelNotification: function (e) {
            Driven.App.BrandPeriodSalesReporting.Notifications.EditLevelNotification.Init({
                Url: _private.View.EditLevelNotificationUrl,
                ModalId: _private.View.EditLevelNotificationModalId,
                TargetId: _private.View.EditLevelNotificationTargetId
            });

            var params = {};
            params[_private.View.EditLevelNotificationKey] = $("#" + _private.View.SecSubBrandId).val();
            params[_private.View.EditLevelNotificationLevel] = $(this).attr('data-button');
            Driven.App.BrandPeriodSalesReporting.Notifications.EditLevelNotification.OpenModal(params);
        },


        EditLevelNotification_SubmitBegin: function (xhr) {
            Driven.App.BrandPeriodSalesReporting.SetLoading(true);
            $("#" + _private.View.EditLevelNotificationModalId).find(':submit').prop('disabled', true);
        },


        EditLevelNotification_SubmitComplete: function (xhr, status) {
            Driven.App.BrandPeriodSalesReporting.SetLoading(false);
        },


        EditLevelNotification_SubmitFailure: function (jqXHR, textStatus, errorThrown) {
            Driven.App.BrandPeriodSalesReporting.SetMessage(false, Driven.App.BrandPeriodSalesReporting.FormatErrorMessage(jqXHR, textStatus));
        },


        EditLevelNotification_SubmitSuccess: function (data, status, xhr) {
            var $dialogContainer = $("#" + _private.View.EditLevelNotificationModalId);
            var $form = $dialogContainer.find("form");
            var result = JSON.parse($form.find("#" + _private.View.DialogResult).val());

            if (result.Success == true) {
                $dialogContainer.modal('hide');
                Driven.App.BrandPeriodSalesReporting.SetMessage(result.Success, result.Message);
                _private.ProcessList();
            }
            else {
                Driven.App.BrandPeriodSalesReporting.Notifications.EditLevelNotification.InitForm();
            }
        },


        Init: function () {
            $.ajaxSetup({ cache: false });

            _private.InitList();
            _private.InitEditors();
            _private.ProcessList();

            $("#" + _private.View.EditBrandNotificationButton).click(_private.OpenEditBrandNotification);
            $("#" + _private.View.EditLevel1NotificationButton).click(_private.OpenEditLevelNotification);
            $("#" + _private.View.EditLevel2NotificationButton).click(_private.OpenEditLevelNotification);
            $("#" + _private.View.EditLevel3NotificationButton).click(_private.OpenEditLevelNotification);

            Driven.App.BrandPeriodSalesReporting.InitModals();

            Driven.App.BrandPeriodSalesReporting.Notifications.EditBrandNotification.SubmitBegin = _private.EditBrandNotification_SubmitBegin;
            Driven.App.BrandPeriodSalesReporting.Notifications.EditBrandNotification.SubmitComplete = _private.EditBrandNotification_SubmitComplete;
            Driven.App.BrandPeriodSalesReporting.Notifications.EditBrandNotification.SubmitFailure = _private.EditBrandNotification_SubmitFailure;
            Driven.App.BrandPeriodSalesReporting.Notifications.EditBrandNotification.SubmitSuccess = _private.EditBrandNotification_SubmitSuccess;

            Driven.App.BrandPeriodSalesReporting.Notifications.EditLevelNotification.SubmitBegin = _private.EditLevelNotification_SubmitBegin;
            Driven.App.BrandPeriodSalesReporting.Notifications.EditLevelNotification.SubmitComplete = _private.EditLevelNotification_SubmitComplete;
            Driven.App.BrandPeriodSalesReporting.Notifications.EditLevelNotification.SubmitFailure = _private.EditLevelNotification_SubmitFailure;
            Driven.App.BrandPeriodSalesReporting.Notifications.EditLevelNotification.SubmitSuccess = _private.EditLevelNotification_SubmitSuccess;
        }
    };


    // public
    var _public = {
        Init: _private.Init
    };

    return _public;

})();


$(function () {
    Driven.App.BrandPeriodSalesReporting.Notifications.Init();
});
