if (typeof Driven === "undefined") { Driven = {}; }
if (typeof Driven.App === "undefined") { Driven.App = {}; }
if (typeof Driven.App.BrandPeriodSalesReporting === "undefined") { Driven.App.BrandPeriodSalesReporting = {}; }

Driven.App.BrandPeriodSalesReporting.ApproveCorrections = (function () {

    // private
    var _private = {

        View: {
            ListToplinesTableId: "approveCorrectionsTable",
            ListToplinesTableSpinner: "approveCorrectionsTableSpinner",
            ListToplinesTableContainer: "approveCorrectionsTableContainer",
            ListToplinesUrl: "SalesCorrection/GetToplineCorrections",
            ListToplinesLength: 10,
            ListToplinesMaxRows: 10000,

            FilterStoreDropdown: "StoreId",
            FilterStatusDropdown: "StatusId",
            FilterPeriodDropdown: "PeriodEndDate",

            DataRequestId: "data-requestid",
            DataToplineId: "data-toplineid",

            SecTokenName: "__RequestVerificationToken",
            SecTokenValue: "__AjaxAntiForgeryForm",

            SaveButton: "SaveBtn",
            DialogResult: "DialogResult",

            ApproveToplineCorrectionUrl: "SalesCorrection/ApproveToplineCorrection",
            ApproveToplineCorrectionModalId: "ApproveToplineCorrectionModal",
            ApproveToplineCorrectionTargetId: "ApproveToplineCorrectionPartialView"
        },


        InitApproveList: function () {
            var tableId = _private.View.ListToplinesTableId;
            var displayLength = _private.View.ListToplinesLength;

            $.fn.dataTable.moment('DD/MM/YYYY');

            var table = $("#" + tableId).dataTable({
                "bServerSide": false,
                "iDisplayLength": displayLength,
                "bLengthChange": false,
                "aaSorting": [],

                "paging": true,
                "ordering": true,
                "info": true,
                "searching": false,

                "language": { "emptyTable": "No available sales corrections" },
                "columnDefs": [{ "targets": [1, 2, 7], "sClass": "center" }, { "targets": [3, 4, 5, 6], "sClass": "right" }]
            });
        },


        ProcessApproveList: function () {
            var storeId = $("#" + _private.View.FilterStoreDropdown).val();
            var periodEndDate = $("#" + _private.View.FilterPeriodDropdown).val();
            var statusId = $("#" + _private.View.FilterStatusDropdown).val();

            var tableId = _private.View.ListToplinesTableId;
            var url = Driven.App.BrandPeriodSalesReporting.GetRelativeUrl(_private.View.ListToplinesUrl);

            var params = {};

            if (storeId && storeId.length > 0) {
                params[_private.View.FilterStoreDropdown] = storeId;
            }

            if (periodEndDate && periodEndDate.length > 0) {
                params[_private.View.FilterPeriodDropdown] = periodEndDate;
            }

            if (statusId && statusId.length > 0) {
                params[_private.View.FilterStatusDropdown] = statusId;
            }

            Driven.App.BrandPeriodSalesReporting.SetLoading(true);

            $.getJSON(url, params)
            .done(function (data, textStatus, jqXHR) {
                _private.FillApproveTable(tableId, data);
            })
            .fail(function (jqXHR, textStatus, errorThrown) {
                Driven.App.BrandPeriodSalesReporting.SetMessage(false, Driven.App.BrandPeriodSalesReporting.FormatErrorMessage(jqXHR, textStatus));
            })
            .always(function () {
                Driven.App.BrandPeriodSalesReporting.SetLoading(false);
                $("#" + _private.View.ListToplinesTableContainer).show();
                $("#" + _private.View.ListToplinesTableSpinner).hide();
            });
        },


        FillApproveTable: function (tableId, json) {
            var $table = $("#" + tableId).dataTable();
            $table.fnClearTable();

            // growl if more than max rows
            if (json && json.length >= _private.View.ListToplinesMaxRows) {
                Driven.App.BrandPeriodSalesReporting.SetMessage(true, "Showing the first " + _private.View.ListToplinesMaxRows + " entries");
            }

            var tableData = [];
            for (var i = 0; i < json.length; i++) {
                var o = json[i];
                var btn = _private.BuildApproveActions(o.ToplineId, o.RequestId, o.CanApprove);
                var row = [o.StoreId, o.PeriodEndDate, o.Status, o.ActiveSales, o.Sales, o.ActiveTickets, o.Tickets, btn];

                tableData.push(row);
            }

            if (tableData.length > 0) {
                $table.fnAddData(tableData);
            }
        },


        BuildApproveHtmlLink: function (text, data, clickEvent) {
            var d = (data) ? data : "";
            if (clickEvent) {
                return "<span class=\"link\" " + d + " onclick=\"" + clickEvent + "(this)\">" + text + "</span>";
            } else {
                return "<span class=\"link\" " + d + ">" + text + "</span>";
            }
        },


        BuildApproveActions: function (toplineId, requestId, canApprove) {
            var d = _private.View.DataRequestId + "=\"" + requestId + "\" " + _private.View.DataToplineId + "=\"" + toplineId + "\"";
            var btns = [];

            if (canApprove == true)
                btns.push(_private.BuildApproveHtmlLink("Edit", d, "Driven.App.BrandPeriodSalesReporting.ApproveCorrections.OpenApproveToplineCorrection"));

            return btns.join("&nbsp;|&nbsp;");
        },


        OpenApproveToplineCorrection: function (e) {
            Driven.App.BrandPeriodSalesReporting.ApproveCorrections.ApproveToplineCorrection.Init({
                Url: _private.View.ApproveToplineCorrectionUrl,
                ModalId: _private.View.ApproveToplineCorrectionModalId,
                TargetId: _private.View.ApproveToplineCorrectionTargetId
            });

            var requestId = $(e).attr(_private.View.DataRequestId);
            var toplineId = $(e).attr(_private.View.DataToplineId);

            Driven.App.BrandPeriodSalesReporting.ApproveCorrections.ApproveToplineCorrection.OpenModal({ requestId: requestId, toplineId: toplineId });
        },


        ApproveToplineCorrection_SubmitBegin: function (xhr) {
            Driven.App.BrandPeriodSalesReporting.SetLoading(true);
            $("#" + _private.View.SaveButton).prop('disabled', true);
            //$("#" + _private.View.ApproveToplineModalId).find(':submit').prop('disabled', true);
        },


        ApproveToplineCorrection_SubmitComplete: function (xhr, status) {
            Driven.App.BrandPeriodSalesReporting.SetLoading(false);
            $("#" + _private.View.SaveButton).prop('disabled', false);
            //$("#" + _private.View.ApproveToplineModalId).find(':submit').prop('disabled', false);
        },


        ApproveToplineCorrection_SubmitFailure: function (jqXHR, textStatus, errorThrown) {
            Driven.App.BrandPeriodSalesReporting.SetMessage(false, Driven.App.BrandPeriodSalesReporting.FormatErrorMessage(jqXHR, textStatus));
        },


        ApproveToplineCorrection_SubmitSuccess: function (data, status, xhr) {
            var $dialogContainer = $("#" + _private.View.ApproveToplineCorrectionModalId);
            var $form = $dialogContainer.find("form");
            var result = JSON.parse($form.find("#" + _private.View.DialogResult).val());

            if (result.Success == true) {
                $dialogContainer.modal('hide');
                _private.ProcessApproveList();
                Driven.App.BrandPeriodSalesReporting.SetMessage(true, result.Message);
            }
            else {
                Driven.App.BrandPeriodSalesReporting.ApproveCorrections.ApproveToplineCorrection.InitForm();
            }
        },


        Init: function () {
            $.ajaxSetup({ cache: false });

            _private.InitApproveList();
            _private.ProcessApproveList();

            $("#" + _private.View.FilterStoreDropdown).change(_private.ProcessApproveList);
            $("#" + _private.View.FilterStatusDropdown).change(_private.ProcessApproveList);
            $("#" + _private.View.FilterPeriodDropdown).change(_private.ProcessApproveList);

            Driven.App.BrandPeriodSalesReporting.ApproveCorrections.OpenApproveToplineCorrection = _private.OpenApproveToplineCorrection;

            Driven.App.BrandPeriodSalesReporting.InitModals();

            Driven.App.BrandPeriodSalesReporting.ApproveCorrections.ApproveToplineCorrection.SubmitBegin = _private.ApproveToplineCorrection_SubmitBegin;
            Driven.App.BrandPeriodSalesReporting.ApproveCorrections.ApproveToplineCorrection.SubmitComplete = _private.ApproveToplineCorrection_SubmitComplete;
            Driven.App.BrandPeriodSalesReporting.ApproveCorrections.ApproveToplineCorrection.SubmitFailure = _private.ApproveToplineCorrection_SubmitFailure;
            Driven.App.BrandPeriodSalesReporting.ApproveCorrections.ApproveToplineCorrection.SubmitSuccess = _private.ApproveToplineCorrection_SubmitSuccess;
        }
    };


    // public
    var _public = {
        Init: _private.Init
    };

    return _public;

})();


$(function () {
    Driven.App.BrandPeriodSalesReporting.ApproveCorrections.Init();
});
