if (typeof Driven === "undefined") { Driven = {}; }
if (typeof Driven.App === "undefined") { Driven.App = {}; }
if (typeof Driven.App.BrandPeriodSalesReporting === "undefined") { Driven.App.BrandPeriodSalesReporting = {}; }
if (typeof Driven.App.BrandPeriodSalesReporting.CorrectionType === "undefined") { Driven.App.BrandPeriodSalesReporting.CorrectionType = {}; }

Driven.App.BrandPeriodSalesReporting.CorrectionType.CorrectionTypes = (function () {

    // private
    var _private = {

        View: {
            CorrectionTypesTableId: "CorrectionTypesTable",
            CorrectionTypesTableSpinner: "CorrectionTypesTableSpinner",
            CorrectionTypesTableContainer: "CorrectionTypesTableContainer",
            CorrectionTypesUrl: "CorrectionType/GetCorrectionTypes",
            CorrectionTypesLength: 10,

            DialogResult: "DialogResult",

            SecTokenName: "__RequestVerificationToken",
            SecTokenValue: "__AjaxAntiForgeryForm",

            CreateCorrectionTypeButton: "CreateCorrectionTypeBtn",
            CreateCorrectionTypeUrl: "CorrectionType/CreateCorrectionType",
            CreateCorrectionTypeModalId: "CorrectionTypeModal",
            CreateCorrectionTypeTargetId: "CorrectionTypePartialView",

            EditCorrectionTypeUrl: "CorrectionType/EditCorrectionType",
            EditCorrectionTypeModalId: "CorrectionTypeModal",
            EditCorrectionTypeTargetId: "CorrectionTypePartialView"
        },


        InitList: function () {
            var tableId = _private.View.CorrectionTypesTableId;
            var displayLength = _private.View.CorrectionTypesLength;

            var table = $("#" + tableId).dataTable({
                "bServerSide": false,
                "iDisplayLength": displayLength,
                "bLengthChange": false,
                "aaSorting": [],

                "paging": true,
                "ordering": true,
                "info": true,
                "searching": false,

                "language": { "emptyTable": "No available Correction Types" },

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
            var tableId = _private.View.CorrectionTypesTableId;
            var url = Driven.App.BrandPeriodSalesReporting.GetRelativeUrl(_private.View.CorrectionTypesUrl);

            var params = {};
            Driven.App.BrandPeriodSalesReporting.SetLoading(true);

            $.getJSON(url, params)
            .done(function (data, textStatus, jqXHR) {
                _private.FillCorrectionTypeTable(tableId, data);
            })
            .fail(function (jqXHR, textStatus, errorThrown) {
                Driven.App.BrandPeriodSalesReporting.SetMessage(false, Driven.App.BrandPeriodSalesReporting.FormatErrorMessage(jqXHR, textStatus));
            })
            .always(function () {
                Driven.App.BrandPeriodSalesReporting.SetLoading(false);
                $("#" + _private.View.CorrectionTypesTableContainer).show();
                $("#" + _private.View.CorrectionTypesTableSpinner).hide();
                _private.GetDataTable(tableId).responsive.recalc();
            });
        },


        FillCorrectionTypeTable: function (tableId, json) {

            var $table = $("#" + tableId).dataTable();
            $table.fnClearTable();

            var tableData = [];
            for (var i = 0; i < json.length; i++) {
                var o = json[i];
                var btn = _private.BuildCorrectionTypeActions(o.CorrectionTypeId);
                var row = [o.CorrectionTypeId, o.CorrectionTypeCodeId, o.CorrectionTypeDescription, btn];

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


        BuildCorrectionTypeActions: function (correctionTypeId) {

            var d = "data-correctiontypeid=\"" + correctionTypeId + "\"";
            var btns = [];

            btns.push(_private.BuildHtmlLink("Edit", d, "Driven.App.BrandPeriodSalesReporting.CorrectionType.OpenEditCorrectionType"));

            return btns.join("&nbsp;|&nbsp;");
        },


        GetDataTable: function (id) {
            return $("#" + id).DataTable();
        },


        OpenCreateCorrectionType: function () {
            Driven.App.BrandPeriodSalesReporting.CorrectionType.CreateCorrectionType.Init({
                Url: _private.View.CreateCorrectionTypeUrl,
                ModalId: _private.View.CreateCorrectionTypeModalId,
                TargetId: _private.View.CreateCorrectionTypeTargetId
            });

            var id = $("#" + _private.View.CurrentBillCodeId).val();
            Driven.App.BrandPeriodSalesReporting.CorrectionType.CreateCorrectionType.OpenModal({ billCodeId: id });
        },


        CreateCorrectionType_SubmitBegin: function (xhr) {
            Driven.App.BrandPeriodSalesReporting.SetLoading(true);
            $("#" + _private.View.CreateCorrectionTypeModalId).find(':submit').prop('disabled', true);
        },


        CreateCorrectionType_SubmitComplete: function (xhr, status) {
            Driven.App.BrandPeriodSalesReporting.SetLoading(false);
            $("#" + _private.View.CreateCorrectionTypeModalId).find(':submit').prop('disabled', false);
        },


        CreateCorrectionType_SubmitFailure: function (jqXHR, textStatus, errorThrown) {
            Driven.App.BrandPeriodSalesReporting.SetMessage(false, Driven.App.BrandPeriodSalesReporting.FormatErrorMessage(jqXHR, textStatus));
        },


        CreateCorrectionType_SubmitSuccess: function (data, status, xhr) {
            var $dialogContainer = $("#" + _private.View.CreateCorrectionTypeModalId);
            var $form = $dialogContainer.find("form");
            var result = JSON.parse($form.find("#" + _private.View.DialogResult).val());

            if (result.Success == true) {
                $dialogContainer.modal('hide');
                _private.ProcessList();
                Driven.App.BrandPeriodSalesReporting.SetMessage(true, result.Message);
            }
            else {
                Driven.App.BrandPeriodSalesReporting.CorrectionType.CreateCorrectionType.InitForm();
            }
        },


        OpenEditCorrectionType: function (e) {
            Driven.App.BrandPeriodSalesReporting.CorrectionType.EditCorrectionType.Init({
                Url: _private.View.EditCorrectionTypeUrl,
                ModalId: _private.View.EditCorrectionTypeModalId,
                TargetId: _private.View.EditCorrectionTypeTargetId
            });

            var id = $(e).attr('data-correctiontypeid');
            Driven.App.BrandPeriodSalesReporting.CorrectionType.EditCorrectionType.OpenModal({ correctionTypeId: id });
        },


        EditCorrectionType_SubmitBegin: function (xhr) {
            Driven.App.BrandPeriodSalesReporting.SetLoading(true);
            $("#" + _private.View.EditCorrectionTypeModalId).find(':submit').prop('disabled', true);
        },


        EditCorrectionType_SubmitComplete: function (xhr, status) {
            Driven.App.BrandPeriodSalesReporting.SetLoading(false);
            $("#" + _private.View.EditCorrectionTypeModalId).find(':submit').prop('disabled', false);
        },


        EditCorrectionType_SubmitFailure: function (jqXHR, textStatus, errorThrown) {
            Driven.App.BrandPeriodSalesReporting.SetMessage(false, Driven.App.BrandPeriodSalesReporting.FormatErrorMessage(jqXHR, textStatus));
        },


        EditCorrectionType_SubmitSuccess: function (data, status, xhr) {
            var $dialogContainer = $("#" + _private.View.EditCorrectionTypeModalId);
            var $form = $dialogContainer.find("form");
            var result = JSON.parse($form.find("#" + _private.View.DialogResult).val());

            if (result.Success == true) {
                $dialogContainer.modal('hide');
                _private.ProcessList();
                Driven.App.BrandPeriodSalesReporting.SetMessage(true, result.Message);
            }
            else {
                Driven.App.BrandPeriodSalesReporting.CorrectionType.EditCorrectionType.InitForm();
            }
        },


        Init: function () {
            $.ajaxSetup({ cache: false });

            _private.InitList();
            _private.ProcessList();

            $("#" + _private.View.CreateCorrectionTypeButton).click(_private.OpenCreateCorrectionType);

            Driven.App.BrandPeriodSalesReporting.InitModals();

            Driven.App.BrandPeriodSalesReporting.CorrectionType.OpenCreateCorrectionType = _private.OpenCreateCorrectionType;
            Driven.App.BrandPeriodSalesReporting.CorrectionType.CreateCorrectionType.SubmitBegin = _private.CreateCorrectionType_SubmitBegin;
            Driven.App.BrandPeriodSalesReporting.CorrectionType.CreateCorrectionType.SubmitComplete = _private.CreateCorrectionType_SubmitComplete;
            Driven.App.BrandPeriodSalesReporting.CorrectionType.CreateCorrectionType.SubmitFailure = _private.CreateCorrectionType_SubmitFailure;
            Driven.App.BrandPeriodSalesReporting.CorrectionType.CreateCorrectionType.SubmitSuccess = _private.CreateCorrectionType_SubmitSuccess;

            Driven.App.BrandPeriodSalesReporting.CorrectionType.OpenEditCorrectionType = _private.OpenEditCorrectionType;
            Driven.App.BrandPeriodSalesReporting.CorrectionType.EditCorrectionType.SubmitBegin = _private.EditCorrectionType_SubmitBegin;
            Driven.App.BrandPeriodSalesReporting.CorrectionType.EditCorrectionType.SubmitComplete = _private.EditCorrectionType_SubmitComplete;
            Driven.App.BrandPeriodSalesReporting.CorrectionType.EditCorrectionType.SubmitFailure = _private.EditCorrectionType_SubmitFailure;
            Driven.App.BrandPeriodSalesReporting.CorrectionType.EditCorrectionType.SubmitSuccess = _private.EditCorrectionType_SubmitSuccess;
        }
    };


    // public
    var _public = {
        Init: _private.Init
    };

    return _public;

})();


$(function () {
    Driven.App.BrandPeriodSalesReporting.CorrectionType.CorrectionTypes.Init();
});
