if (typeof Driven === "undefined") { Driven = {}; }
if (typeof Driven.App === "undefined") { Driven.App = {}; }
if (typeof Driven.App.BrandPeriodSalesReporting === "undefined") { Driven.App.BrandPeriodSalesReporting = {}; }

Driven.App.BrandPeriodSalesReporting.Notifications = (function () {

    // private
    var _private = { 

        View: {
            NotificationTableId: "NotificationTable",
            NotificationTableSpinner: "NotificationTableSpinner",
            NotificationTableContainer: "NotificationTableContainer",
            NotificationUrl: "Notification/GetNotification",
            NotificationLength: 10,

            DialogResult: "DialogResult",
            CurrentSubBrandId: "Current_SubBrandId",

            FilterCenterStatusDropdown: "CenterStatusId",
            FilterLocalStoreIdTextBox: "LocalStoreIdSearchtxt",
            LocalStoreIdSearchParam: "StoreId",
            CenterStatusIdSearchParam: "StatusId",

            SecCanEdit: "CanEdit",
            SecSubBrandId: "SubBrandId",
            SecTokenName: "__RequestVerificationToken",
            SecTokenValue: "__AjaxAntiForgeryForm",

            chkEmailCheckboxBySubBrand: "chkEmailCheckboxBySubBrand",
            datetimepicker_Email: "datetimepicker_Email",

            chkCallCheckboxBySubBrand: "chkCallCheckboxBySubBrand",
            datetimepicker_Call: "datetimepicker_Call",

            UpdateSubBrand_EmailUrl: "Notification/UpdateSubBrand_Email",
            UpdateSubBrand_CallUrl: "Notification/UpdateSubBrand_Call",

            SubmitEmailNotificationsUrl: "Notification/UpdateEmailNotification",
            SubmitCallNotificationsUrl: "Notification/UpdateCallNotification",
            SubmitTimeZoneNotificationsUrl: "Notification/UpdateTimeZoneNotification",

            HidenEmailSendTime: "EmailSendTime",
            HidenCallSendTime: "CallSendTime",

            EditLevel1NotificationButton: "EditLevel1NotificationBtn",
            EditLevel2NotificationButton: "EditLevel2NotificationBtn",
            EditLevel3NotificationButton: "EditLevel3NotificationBtn",
            EditLevelNotificationUrl: "Notification/EditLevelNotification",
            EditLevelNotificationKey: "SubBrandId",
            EditLevelNotificationLevel: "Level",
            EditLevelNotificationModalId: "EditNotificationModal",
            EditLevelNotificationTargetId: "EditNotificationPartialView",

            EditBrandNotificationButton: "EditBrandNotificationBtn",
            EditBrandNotificationUrl: "Notification/EditBrandNotification",
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


        InitKeys: function () {
            $("form input").keypress(function (e) {
                if (e.which === 13) {
                    _private.ProcessList();
                }
            });
        },


        InitList: function () {
            var tableId = _private.View.NotificationTableId;
            var displayLength = _private.View.NotificationLength;
            var canEdit = ($("#" + _private.View.SecCanEdit).val().toUpperCase() == "TRUE");

            var table = $("#" + tableId).dataTable({
                "bServerSide": false,
                "iDisplayLength": displayLength,
                "bLengthChange": false,
                "aaSorting": [],
                "paging": true,
                "ordering": true,
                "info": true,
                "searching": false,
                "responsive": {
                    "details": {
                        "type": 'column',
                        "target": 5
                    }
                },

                "language": { "emptyTable": "No available Notification" },

                "columnDefs": [
                    { "targets": 0, "visible": false, "responsivePriority": 2 },
                    { "targets": 1, "sClass": "left", "responsivePriority": 1 },
                    { "targets": 2, "sClass": "left", "responsivePriority": 2 },
                    { "targets": 3, "sClass": "left", "responsivePriority": 2 },
                    { "targets": 4, "sClass": "center", "orderable": false, "responsivePriority": 2 },
                    { "targets": 5, "sClass": "center", "orderable": false, "responsivePriority": 2 },
                    { "targets": 6, "sClass": "center", "orderable": false, "responsivePriority": 2 }
                ],
                "order": [[1, 'asc']]
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


        ProcessList: function (e) {
            if ((e) && (e.data.resetSearchText)) {
                $("#" + _private.View.FilterLocalStoreIdTextBox).val('');
                $("#" + _private.View.FilterCenterStatusDropdown + ">option:eq(1)").prop('selected', true);
            }

            var tableId = _private.View.NotificationTableId;
            var url = Driven.App.BrandPeriodSalesReporting.GetRelativeUrl(_private.View.NotificationUrl);

            var centerStatusIdFilter = $("#" + _private.View.FilterCenterStatusDropdown).val();
            var localStoreIdSearch = $("#" + _private.View.FilterLocalStoreIdTextBox).val();

            var params = {};
            params[_private.View.LocalStoreIdSearchParam] = (localStoreIdSearch && localStoreIdSearch.length > 0) ? localStoreIdSearch : "";
            params[_private.View.CenterStatusIdSearchParam] = (centerStatusIdFilter && centerStatusIdFilter.length > 0) ? centerStatusIdFilter : "";

            Driven.App.BrandPeriodSalesReporting.SetLoading(true);

            $.getJSON(url, params)
            .done(function (data, textStatus, jqXHR) {
                _private.FillNotificationTable(tableId, data);
            })
            .fail(function (jqXHR, textStatus, errorThrown) {
                Driven.App.BrandPeriodSalesReporting.SetMessage(false, Driven.App.BrandPeriodSalesReporting.FormatErrorMessage(jqXHR, textStatus));
            })
            .always(function () {
                Driven.App.BrandPeriodSalesReporting.SetLoading(false);
                $("#" + _private.View.NotificationTableContainer).show();
                $("#" + _private.View.NotificationTableSpinner).hide();
                _private.GetDataTable(tableId).responsive.recalc();
            });
        },


        FillNotificationTable: function (tableId, json) {
            var $table = $("#" + tableId).dataTable();
            $table.fnClearTable();
            var tableData = [];
            for (var i = 0; i < json.length; i++) {
                var o = json[i];
                var chks = _private.BuildActions(o.CenterId, o.CanEdit, o.ExcludeEmail, o.ExcludeCall).split("|");
                var zzz = _private.BuildTimeZoneDrop(o.CenterId, o.TimeZone)

                var row = [o.CenterId, o.ShopId, o.LocationStoreId, o.CenterStatusDescription, chks[0], chks[1], zzz];
                tableData.push(row);
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


        BuildHtmlCheckbox: function (data, flg, clickEvent) {
            var d = (data) ? data : "";

            if (flg == true)
                return "<input " + d + "  type='checkbox' onclick=\"" + clickEvent + "(this)\" checked>";
            else
                return "<input " + d + "  type='checkbox' onclick=\"" + clickEvent + "(this)\">";
        },


        BuildTimeZoneDrop: function (centerId, timeZone) {
            var a = ' selected ', b = '';
            var r = '<select class="timezonedrop" data-centerid="' + centerId + '">';

            r += '<option value="0"' + ((timeZone == 0) ? a : b) + '>Select</option>';
            r += '<option value="-3"' + ((timeZone == -3) ? a : b) + '>Newfoundland</option>';
            r += '<option value="-4"' + ((timeZone == -4) ? a : b) + '>Atlantic</option>';
            r += '<option value="-5"' + ((timeZone == -5) ? a : b) + '>Eastern</option>';
            r += '<option value="-6"' + ((timeZone == -6) ? a : b) + '>Central</option>';
            r += '<option value="-7"' + ((timeZone == -7) ? a : b) + '>Mountain</option>';
            r += '<option value="-8"' + ((timeZone == -8) ? a : b) + '>Pacific</option>';

            r += "</select>";

            return r;
        },


        BuildActions: function (centerId, canEdit, excludeEmail, excludeCall) {
            var d = "data-centerid=\"" + centerId + "\"";
            var chks = [];

            if (canEdit == true) {
                chks.push(_private.BuildHtmlCheckbox(d, excludeEmail, "Driven.App.BrandPeriodSalesReporting.Notifications.UpdateExcludeEmail"));
                chks.push(_private.BuildHtmlCheckbox(d, excludeCall, "Driven.App.BrandPeriodSalesReporting.Notifications.UpdateExcludeCall"));
            }
            return chks.join("&nbsp;|&nbsp;");
        },


        GetDataTable: function (id) {
            return $("#" + id).DataTable();
        },


        UpdateExcludeEmail: function (e) {
            var centerId = $(e).attr('data-centerid');
            IsEmailExclude = e.checked;
            _private.UpdateEmailNotifications([centerId], IsEmailExclude, _private.View.SubmitEmailNotificationsUrl);
        },


        UpdateExcludeCall: function (e) {
            var centerId = $(e).attr('data-centerid');
            IsCallExclude = e.checked;
            _private.UpdateCallNotifications([centerId], IsCallExclude, _private.View.SubmitCallNotificationsUrl);
        },


        UpdateEmailNotifications: function (centerId, IsEmailExclude, url) {
            var url = Driven.App.BrandPeriodSalesReporting.GetRelativeUrl(url);
            var tokenName = _private.View.SecTokenName;
            var tokenValue = $('input[name="' + tokenName + '"]', '#' + _private.View.SecTokenValue).val();

            var params = {};
            params[tokenName] = tokenValue,
            params["centerId"] = centerId;
            params["IsEmailExclude"] = IsEmailExclude;

            Driven.App.BrandPeriodSalesReporting.SetLoading(true);

            $.ajax({ url: url, cache: false, type: 'POST', data: params })
           .done(function (data, textStatus, jqXHR) {
               var result = $.parseJSON(data);
               Driven.App.BrandPeriodSalesReporting.SetMessage(result.Success, result.Message);
               //_private.ProcessList();
           })
           .fail(function (jqXHR, textStatus, errorThrown) {
               Driven.App.BrandPeriodSalesReporting.SetMessage(false, Driven.App.BrandPeriodSalesReporting.FormatErrorMessage(jqXHR, textStatus));
           })
           .always(function () {
               Driven.App.BrandPeriodSalesReporting.SetLoading(false);
           });
        },


        UpdateCallNotifications: function (centerId, IsCallExclude, url) {
            var url = Driven.App.BrandPeriodSalesReporting.GetRelativeUrl(url);
            var tokenName = _private.View.SecTokenName;
            var tokenValue = $('input[name="' + tokenName + '"]', '#' + _private.View.SecTokenValue).val();

            var params = {};
            params[tokenName] = tokenValue,
            params["centerId"] = centerId;
            params["IsCallExclude"] = IsCallExclude;

            Driven.App.BrandPeriodSalesReporting.SetLoading(true);

            $.ajax({ url: url, cache: false, type: 'POST', data: params })
           .done(function (data, textStatus, jqXHR) {
               var result = $.parseJSON(data);
               Driven.App.BrandPeriodSalesReporting.SetMessage(result.Success, result.Message);
               //_private.ProcessList();
           })
           .fail(function (jqXHR, textStatus, errorThrown) {
               Driven.App.BrandPeriodSalesReporting.SetMessage(false, Driven.App.BrandPeriodSalesReporting.FormatErrorMessage(jqXHR, textStatus));
           })
           .always(function () {
               Driven.App.BrandPeriodSalesReporting.SetLoading(false);
           });
        },


        UpdateTimeZone: function (e) {
            var centerId = $(this).attr('data-centerid');
            var timeZone = $(this).val();

            var url = Driven.App.BrandPeriodSalesReporting.GetRelativeUrl(_private.View.SubmitTimeZoneNotificationsUrl);
            var tokenName = _private.View.SecTokenName;
            var tokenValue = $('input[name="' + tokenName + '"]', '#' + _private.View.SecTokenValue).val();

            var params = {};
            params[tokenName] = tokenValue,
            params["centerId"] = centerId;
            params["timeZone"] = timeZone;

            Driven.App.BrandPeriodSalesReporting.SetLoading(true);

            $.ajax({ url: url, cache: false, type: 'POST', data: params })
           .done(function (data, textStatus, jqXHR) {
               var result = $.parseJSON(data);
               Driven.App.BrandPeriodSalesReporting.SetMessage(result.Success, result.Message);
               //_private.ProcessList();
           })
           .fail(function (jqXHR, textStatus, errorThrown) {
               Driven.App.BrandPeriodSalesReporting.SetMessage(false, Driven.App.BrandPeriodSalesReporting.FormatErrorMessage(jqXHR, textStatus));
           })
           .always(function () {
               Driven.App.BrandPeriodSalesReporting.SetLoading(false);
           });
        },


        UpdateEmailFlagBySubBrand: function (event) {
            var url = Driven.App.BrandPeriodSalesReporting.GetRelativeUrl(_private.View.UpdateSubBrand_EmailUrl);
            var tokenName = _private.View.SecTokenName;
            var tokenValue = $('input[name="' + tokenName + '"]', '#' + _private.View.SecTokenValue).val();

            var params = {};
            params[tokenName] = tokenValue,
            params["isEmailEnabled"] = $("#" + _private.View.chkEmailCheckboxBySubBrand).is(':checked');

            if ($("#" + _private.View.chkEmailCheckboxBySubBrand).is(':checked')) {
                $("#" + _private.View.datetimepicker_Email).find("input").attr('readonly', false);
                if (!$("#" + _private.View.datetimepicker_Email).find("input").val()) {
                    $("#" + _private.View.datetimepicker_Email).find("input").val(Driven.App.BrandPeriodSalesReporting.FormatTime(new Date));
                }
                params["emailSendTime"] = $("#" + _private.View.datetimepicker_Email).find("input").val();
            }
            else {
                params["emailSendTime"] = "00:00:00.0000000";
                $("#" + _private.View.datetimepicker_Email).find("input").val("");
                $("#" + _private.View.datetimepicker_Email).find("input").attr('readonly', true);
            }

            Driven.App.BrandPeriodSalesReporting.SetLoading(true);

            $.ajax({ url: url, cache: false, type: 'POST', data: params })
            .done(function (data, textStatus, jqXHR) {
                var result = $.parseJSON(data);
                Driven.App.BrandPeriodSalesReporting.SetMessage(result.Success, result.Message);

                $("#" + _private.View.HidenEmailSendTime).val(params["emailSendTime"]);
                _private.ProcessList();
            })
            .fail(function (jqXHR, textStatus, errorThrown) {
                Driven.App.BrandPeriodSalesReporting.SetMessage(false, Driven.App.BrandPeriodSalesReporting.FormatErrorMessage(jqXHR, textStatus));
            })
            .always(function () {
                Driven.App.BrandPeriodSalesReporting.SetLoading(false);
                $("#" + _private.View.NotificationTableContainer).show();
                $("#" + _private.View.NotificationTableSpinner).hide();
            });
        },


        UpdateCallFlagBySubBrand: function (event) {
            var url = Driven.App.BrandPeriodSalesReporting.GetRelativeUrl(_private.View.UpdateSubBrand_CallUrl);
            var tokenName = _private.View.SecTokenName;
            var tokenValue = $('input[name="' + tokenName + '"]', '#' + _private.View.SecTokenValue).val();

            var params = {};
            params[tokenName] = tokenValue,
            params["isCallEnabled"] = $("#" + _private.View.chkCallCheckboxBySubBrand).is(':checked');

            if ($("#" + _private.View.chkCallCheckboxBySubBrand).is(':checked')) {
                $("#" + _private.View.datetimepicker_Call).find("input").attr('readonly', false);

                if (!$("#" + _private.View.datetimepicker_Call).find("input").val()) {
                    $("#" + _private.View.datetimepicker_Call).find("input").val(Driven.App.BrandPeriodSalesReporting.FormatTime(new Date));
                }
                params["callSendTime"] = $("#" + _private.View.datetimepicker_Call).find("input").val();
            }
            else {
                params["callSendTime"] = "00:00:00.0000000";
                $("#" + _private.View.datetimepicker_Call).find("input").val("");
                $("#" + _private.View.datetimepicker_Call).find("input").attr('readonly', true);
            }
            Driven.App.BrandPeriodSalesReporting.SetLoading(true);

            $.ajax({ url: url, cache: false, type: 'POST', data: params })
            .done(function (data, textStatus, jqXHR) {
                var result = $.parseJSON(data);
                Driven.App.BrandPeriodSalesReporting.SetMessage(result.Success, result.Message);
                $("#" + _private.View.HidenCallSendTime).val(params["callSendTime"]);
                _private.ProcessList();
            })
            .fail(function (jqXHR, textStatus, errorThrown) {
                Driven.App.BrandPeriodSalesReporting.SetMessage(false, Driven.App.BrandPeriodSalesReporting.FormatErrorMessage(jqXHR, textStatus));
            })
            .always(function () {
                Driven.App.BrandPeriodSalesReporting.SetLoading(false);
                $("#" + _private.View.NotificationTableContainer).show();
                $("#" + _private.View.NotificationTableSpinner).hide();
            });
        },


        Init: function () {
            $.ajaxSetup({ cache: false });

            $('#datetimepicker_Email').datetimepicker({
                format: 'LT',
                allowInputToggle: true,
                stepping: 15
            }).on('dp.hide', function (e) {

                oldValue = $("#" + _private.View.HidenEmailSendTime).val();
                newValue = $("#" + _private.View.datetimepicker_Email).find("input").val();
                if (oldValue.toLocaleLowerCase() != newValue.toLocaleLowerCase()) {
                    Driven.App.BrandPeriodSalesReporting.Notifications.UpdateEmailFlagBySubBrand(this);
                }
            });

            $('#datetimepicker_Call').datetimepicker({
                format: 'LT',
                allowInputToggle: true,
                stepping: 15
            }).on('dp.hide', function (e) {
                oldValue = $("#" + _private.View.HidenCallSendTime).val();
                newValue = $("#" + _private.View.datetimepicker_Call).find("input").val();
                if (oldValue.toLocaleLowerCase() != newValue.toLocaleLowerCase()) {
                    Driven.App.BrandPeriodSalesReporting.Notifications.UpdateCallFlagBySubBrand(this);
                }
            });

            _private.InitKeys();
            _private.InitList();
            _private.InitEditors();
            _private.ProcessList();

            $("#" + _private.View.FilterLocalStoreIdTextBox).change({ resetSearchText: false }, _private.ProcessList);
            $("#" + _private.View.FilterCenterStatusDropdown).change({ resetSearchText: false }, _private.ProcessList);

            $("#" + _private.View.chkEmailCheckboxBySubBrand).click(_private.UpdateEmailFlagBySubBrand);
            $("#" + _private.View.chkCallCheckboxBySubBrand).click(_private.UpdateCallFlagBySubBrand);
            $("#" + _private.View.NotificationTableId).delegate(".timezonedrop", "change", _private.UpdateTimeZone);

            Driven.App.BrandPeriodSalesReporting.Notifications.UpdateExcludeEmail = _private.UpdateExcludeEmail;
            Driven.App.BrandPeriodSalesReporting.Notifications.UpdateExcludeCall = _private.UpdateExcludeCall;

            Driven.App.BrandPeriodSalesReporting.Notifications.UpdateEmailFlagBySubBrand = _private.UpdateEmailFlagBySubBrand;
            Driven.App.BrandPeriodSalesReporting.Notifications.UpdateCallFlagBySubBrand = _private.UpdateCallFlagBySubBrand;

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
        Init: _private.Init,
    };

    return _public;

})();


$(function () {
    Driven.App.BrandPeriodSalesReporting.Notifications.Init();
});