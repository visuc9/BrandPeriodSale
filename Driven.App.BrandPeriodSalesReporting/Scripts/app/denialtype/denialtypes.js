if (typeof Driven === "undefined") { Driven = {}; }
if (typeof Driven.App === "undefined") { Driven.App = {}; }
if (typeof Driven.App.BrandPeriodSalesReporting === "undefined") { Driven.App.BrandPeriodSalesReporting = {}; }
if (typeof Driven.App.BrandPeriodSalesReporting.DenialType === "undefined") { Driven.App.BrandPeriodSalesReporting.DenialType = {}; }

Driven.App.BrandPeriodSalesReporting.DenialType.DenialTypes = (function () {

    // private
    var _private = {

        View: {
            DenialTypesTableId: "DenialTypesTable",
            DenialTypesTableSpinner: "DenialTypesTableSpinner",
            DenialTypesTableContainer: "DenialTypesTableContainer",
            DenialTypesUrl: "DenialType/GetDenialTypes",
            DenialTypesLength: 10,

            DialogResult: "DialogResult",

            SecTokenName: "__RequestVerificationToken",
            SecTokenValue: "__AjaxAntiForgeryForm",

            CreateDenialTypeButton: "CreateDenialTypeBtn",
            CreateDenialTypeUrl: "DenialType/CreateDenialType",
            CreateDenialTypeModalId: "DenialTypeModal",
            CreateDenialTypeTargetId: "DenialTypePartialView",

            EditDenialTypeUrl: "DenialType/EditDenialType",
            EditDenialTypeModalId: "DenialTypeModal",
            EditDenialTypeTargetId: "DenialTypePartialView"
        },


        InitList: function () {
            var tableId = _private.View.DenialTypesTableId;
            var displayLength = _private.View.DenialTypesLength;

            var table = $("#" + tableId).dataTable({
                "bServerSide": false,
                "iDisplayLength": displayLength,
                "bLengthChange": false,
                "aaSorting": [],

                "paging": true,
                "ordering": true,
                "info": true,
                "searching": false,

                "language": { "emptyTable": "No available Denial Types" },

                "columnDefs": [
                    { "targets": 0, "sClass": "left" },
                    { "targets": 1, "sClass": "left" },
                    { "targets": 2, "sClass": "left" },
                    { "targets": 3, "orderable": false, "sClass": "center" }
                ],

                "order": [[0, 'asc']]

            });
        },


        ProcessList: function () {
            var tableId = _private.View.DenialTypesTableId;
            var url = Driven.App.BrandPeriodSalesReporting.GetRelativeUrl(_private.View.DenialTypesUrl);

            var params = {};
            Driven.App.BrandPeriodSalesReporting.SetLoading(true);

            $.getJSON(url, params)
            .done(function (data, textStatus, jqXHR) {
                _private.FillDenialTypeTable(tableId, data);
            })
            .fail(function (jqXHR, textStatus, errorThrown) {
                Driven.App.BrandPeriodSalesReporting.SetMessage(false, Driven.App.BrandPeriodSalesReporting.FormatErrorMessage(jqXHR, textStatus));
            })
            .always(function () {
                Driven.App.BrandPeriodSalesReporting.SetLoading(false);
                $("#" + _private.View.DenialTypesTableContainer).show();
                $("#" + _private.View.DenialTypesTableSpinner).hide();
                _private.GetDataTable(tableId).responsive.recalc();
            });
        },


        FillDenialTypeTable: function (tableId, json) {

            var $table = $("#" + tableId).dataTable();
            $table.fnClearTable();

            var tableData = [];
            for (var i = 0; i < json.length; i++) {
                var o = json[i];
                var btn = _private.BuildDenialTypeActions(o.DenialTypeId);
                var row = [o.DenialTypeId, o.DenialTypeCodeId, o.DenialTypeDescription, btn];

                tableData.push(row);
            }

            if (tableData.length > 0) {
                $table.fnAddData(tableData);
            }

        },


        BuildHtmlLink: function (text, data, clickEvent) {
            var d = (data) ? data : "";
            if (clickEvent) {
                return "<span class=\"link\" " + d + " onclick=\"" + clickEvent + "(this)\">" + text + "</span>";
            } else {
                return "<span class=\"link\" " + d + ">" + text + "</span>";
            }
        },


        BuildDenialTypeActions: function (denialTypeId) {

            var d = "data-denialtypeid=\"" + denialTypeId + "\"";
            var btns = [];

            btns.push(_private.BuildHtmlLink("Edit", d, "Driven.App.BrandPeriodSalesReporting.DenialType.OpenEditDenialType"));

            return btns.join("&nbsp;|&nbsp;");
        },


        GetDataTable: function (id) {
            return $("#" + id).DataTable();
        },


        OpenCreateDenialType: function () {
            Driven.App.BrandPeriodSalesReporting.DenialType.CreateDenialType.Init({
                Url: _private.View.CreateDenialTypeUrl,
                ModalId: _private.View.CreateDenialTypeModalId,
                TargetId: _private.View.CreateDenialTypeTargetId
            });

            var id = $("#" + _private.View.CurrentBillCodeId).val();
            Driven.App.BrandPeriodSalesReporting.DenialType.CreateDenialType.OpenModal({ billCodeId: id });
        },


        CreateDenialType_SubmitBegin: function (xhr) {
            Driven.App.BrandPeriodSalesReporting.SetLoading(true);
            $("#" + _private.View.CreateDenialTypeModalId).find(':submit').prop('disabled', true);
        },


        CreateDenialType_SubmitComplete: function (xhr, status) {
            Driven.App.BrandPeriodSalesReporting.SetLoading(false);
            $("#" + _private.View.CreateDenialTypeModalId).find(':submit').prop('disabled', false);
        },


        CreateDenialType_SubmitFailure: function (jqXHR, textStatus, errorThrown) {
            Driven.App.BrandPeriodSalesReporting.SetMessage(false, Driven.App.BrandPeriodSalesReporting.FormatErrorMessage(jqXHR, textStatus));
        },


        CreateDenialType_SubmitSuccess: function (data, status, xhr) {
            var $dialogContainer = $("#" + _private.View.CreateDenialTypeModalId);
            var $form = $dialogContainer.find("form");
            var result = JSON.parse($form.find("#" + _private.View.DialogResult).val());

            if (result.Success == true) {
                $dialogContainer.modal('hide');
                _private.ProcessList();
                Driven.App.BrandPeriodSalesReporting.SetMessage(true, result.Message);
            }
            else {
                Driven.App.BrandPeriodSalesReporting.DenialType.CreateDenialType.InitForm();
            }
        },


        OpenEditDenialType: function (e) {
            Driven.App.BrandPeriodSalesReporting.DenialType.EditDenialType.Init({
                Url: _private.View.EditDenialTypeUrl,
                ModalId: _private.View.EditDenialTypeModalId,
                TargetId: _private.View.EditDenialTypeTargetId
            });

            var id = $(e).attr('data-denialtypeid');
            Driven.App.BrandPeriodSalesReporting.DenialType.EditDenialType.OpenModal({ denialTypeId: id });
        },


        EditDenialType_SubmitBegin: function (xhr) {
            Driven.App.BrandPeriodSalesReporting.SetLoading(true);
            $("#" + _private.View.EditDenialTypeModalId).find(':submit').prop('disabled', true);
        },


        EditDenialType_SubmitComplete: function (xhr, status) {
            Driven.App.BrandPeriodSalesReporting.SetLoading(false);
            $("#" + _private.View.EditDenialTypeModalId).find(':submit').prop('disabled', false);
        },


        EditDenialType_SubmitFailure: function (jqXHR, textStatus, errorThrown) {
            Driven.App.BrandPeriodSalesReporting.SetMessage(false, Driven.App.BrandPeriodSalesReporting.FormatErrorMessage(jqXHR, textStatus));
        },


        EditDenialType_SubmitSuccess: function (data, status, xhr) {
            var $dialogContainer = $("#" + _private.View.EditDenialTypeModalId);
            var $form = $dialogContainer.find("form");
            var result = JSON.parse($form.find("#" + _private.View.DialogResult).val());

            if (result.Success == true) {
                $dialogContainer.modal('hide');
                _private.ProcessList();
                Driven.App.BrandPeriodSalesReporting.SetMessage(true, result.Message);
            }
            else {
                Driven.App.BrandPeriodSalesReporting.DenialType.EditDenialType.InitForm();
            }
        },


        Init: function () {
            $.ajaxSetup({ cache: false });

            _private.InitList();
            _private.ProcessList();

            $("#" + _private.View.CreateDenialTypeButton).click(_private.OpenCreateDenialType);

            Driven.App.BrandPeriodSalesReporting.InitModals();

            Driven.App.BrandPeriodSalesReporting.DenialType.OpenCreateDenialType = _private.OpenCreateDenialType;
            Driven.App.BrandPeriodSalesReporting.DenialType.CreateDenialType.SubmitBegin = _private.CreateDenialType_SubmitBegin;
            Driven.App.BrandPeriodSalesReporting.DenialType.CreateDenialType.SubmitComplete = _private.CreateDenialType_SubmitComplete;
            Driven.App.BrandPeriodSalesReporting.DenialType.CreateDenialType.SubmitFailure = _private.CreateDenialType_SubmitFailure;
            Driven.App.BrandPeriodSalesReporting.DenialType.CreateDenialType.SubmitSuccess = _private.CreateDenialType_SubmitSuccess;

            Driven.App.BrandPeriodSalesReporting.DenialType.OpenEditDenialType = _private.OpenEditDenialType;
            Driven.App.BrandPeriodSalesReporting.DenialType.EditDenialType.SubmitBegin = _private.EditDenialType_SubmitBegin;
            Driven.App.BrandPeriodSalesReporting.DenialType.EditDenialType.SubmitComplete = _private.EditDenialType_SubmitComplete;
            Driven.App.BrandPeriodSalesReporting.DenialType.EditDenialType.SubmitFailure = _private.EditDenialType_SubmitFailure;
            Driven.App.BrandPeriodSalesReporting.DenialType.EditDenialType.SubmitSuccess = _private.EditDenialType_SubmitSuccess;
        }
    };


    // public
    var _public = {
        Init: _private.Init
    };

    return _public;

})();


$(function () {
    Driven.App.BrandPeriodSalesReporting.DenialType.DenialTypes.Init();
});
