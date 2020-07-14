if (typeof Driven === "undefined") { Driven = {}; }
if (typeof Driven.App === "undefined") { Driven.App = {}; }
if (typeof Driven.App.BrandPeriodSalesReporting === "undefined") { Driven.App.BrandPeriodSalesReporting = {}; }

Driven.App.BrandPeriodSalesReporting.SubmitCorrections = (function () {

    // private
    var _private = {

        View: {
            ListToplinesTableId: "brandPeriodSalesTable",
            ListToplinesTableSpinner: "brandPeriodSalesTableSpinner",
            ListToplinesTableContainer: "brandPeriodSalesTableContainer",
            ListToplinesUrl: "SalesCorrection/GetToplineCorrections",
            ListToplinesLength: 10,
            ListToplinesMaxRows: 10000,

            FilterStoreDropdown: "StoreId",
            FilterStatusDropdown: "StatusId",
            FilterPeriodDropdown: "PeriodEndDate",

            SecIsAdmin: "IsAdmin",
            SecIsApprover: "IsApprover",
            SecTokenName: "__RequestVerificationToken",
            SecTokenValue: "__AjaxAntiForgeryForm",

            SubmitToplinesButton: "SubmitBtn",
            SubmitToplinesUrl: "SalesCorrection/SubmitToplineCorrections",

            SaveButton: "SaveBtn",
            DialogResult: "DialogResult",

            CreateToplineCorrectionButton: "CreateToplineCorrectionBtn",
            CreateToplineCorrectionUrl: "SalesCorrection/CreateToplineCorrection",
            CreateToplineCorrectionCheckUrl: "SalesCorrection/CanCreateToplineCorrection",

            CreateToplineCorrectionModalId: "EditToplineCorrectionModal",
            CreateToplineCorrectionTargetId: "EditToplineCorrectionPartialView",

            EditToplineCorrectionUrl: "SalesCorrection/EditToplineCorrection",
            EditToplineCorrectionModalId: "EditToplineCorrectionModal",
            EditToplineCorrectionTargetId: "EditToplineCorrectionPartialView"
        },


        InitList: function () {
            var tableId = _private.View.ListToplinesTableId;
            var displayLength = _private.View.ListToplinesLength;

            var isAdmin = ($("#" + _private.View.SecIsAdmin).val().toUpperCase() == "TRUE");
            var isApprover = ($("#" + _private.View.SecIsApprover).val().toUpperCase() == "TRUE");

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
                "columnDefs": [{ "targets": [1, 2, 7], "sClass": "center" }, { "targets": [3, 4, 5, 6], "sClass": "right" } ]
            });
        },


        ProcessList: function () {
            var storeId = $("#" + _private.View.FilterStoreDropdown).val();
            var periodEndDate = $("#" + _private.View.FilterPeriodDropdown).val();
            var statusId = $("#" + _private.View.FilterStatusDropdown).val();

            var tableId = _private.View.ListToplinesTableId;
            var url = Driven.App.BrandPeriodSalesReporting.GetRelativeUrl(_private.View.ListToplinesUrl);

            var params = {};

            if (storeId && storeId.length > 0) {
                params["storeid"] = storeId;
            }

            if (periodEndDate && periodEndDate.length > 0) {
                params["periodenddate"] = periodEndDate;
            }

            if (statusId && statusId.length > 0) {
                params["statusid"] = statusId;
            }

            Driven.App.BrandPeriodSalesReporting.SetLoading(true);

            $.getJSON(url, params)
            .done(function (data, textStatus, jqXHR) {
                _private.FillToplineTable(tableId, data);
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


        FillToplineTable: function (tableId, json) {
            var $table = $("#" + tableId).dataTable();
            $table.fnClearTable();

            // growl if more than max rows
            if (json && json.length >= _private.View.ListToplinesMaxRows) {
                Driven.App.BrandPeriodSalesReporting.SetMessage(true, "Showing the first " + _private.View.ListToplinesMaxRows + " entries");
            }

            var tableData = [];
            for (var i = 0; i < json.length; i++) {
                var o = json[i];

                var btn = _private.BuildToplineActions(o.ToplineId, o.RequestId, o.CanEdit);
                var row = [o.StoreId, o.PeriodEndDate, o.Status, o.ActiveSales, o.Sales, o.ActiveTickets, o.Tickets, btn];

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


        BuildToplineActions: function (toplineId, requestId, canEdit) {
            var d = "data-requestid=\"" + requestId + "\" data-toplineid=\"" + toplineId + "\"";
            var btns = [];

            if (canEdit == true)
                btns.push(_private.BuildHtmlLink("Edit", d, "Driven.App.BrandPeriodSalesReporting.SubmitCorrections.OpenEditToplineCorrection"));

            return btns.join("&nbsp;|&nbsp;");
        },


        OpenCreateToplineCorrection: function () {
            Driven.App.BrandPeriodSalesReporting.SubmitCorrections.CreateToplineCorrection.Init({
                Url: _private.View.CreateToplineCorrectionUrl,
                ModalId: _private.View.CreateToplineCorrectionModalId,
                TargetId: _private.View.CreateToplineCorrectionTargetId
            });

            var localStoreId = $("#StoreId").val();
            var periodEndDate = $("#PeriodEndDate").val();

            if (periodEndDate && localStoreId) {
                var url = Driven.App.BrandPeriodSalesReporting.GetRelativeUrl(_private.View.CreateToplineCorrectionCheckUrl);

                var params = {};
                params["localstoreid"] = localStoreId;
                params["periodenddate"] = periodEndDate;

                Driven.App.BrandPeriodSalesReporting.SetLoading(true);

                $.getJSON(url, params)
                .done(function (data, textStatus, jqXHR) {
                    if (data.Success == true) {
                        Driven.App.BrandPeriodSalesReporting.SubmitCorrections.CreateToplineCorrection.OpenModal({ localStoreId: localStoreId, periodEndDate: periodEndDate });
                    } else {
                        Driven.App.BrandPeriodSalesReporting.SetMessage(true, data.Message, "warning");
                    }
                })
                .fail(function (jqXHR, textStatus, errorThrown) {
                    Driven.App.BrandPeriodSalesReporting.SetMessage(false, Driven.App.BrandPeriodSalesReporting.FormatErrorMessage(jqXHR, textStatus));
                })
                .always(function () {
                    Driven.App.BrandPeriodSalesReporting.SetLoading(false);
                });

            }
            else {
                if (localStoreId)
                    Driven.App.BrandPeriodSalesReporting.SetMessage(true, "Select a <b>Reporting Period</b>", "warning");
                else if (periodEndDate)
                    Driven.App.BrandPeriodSalesReporting.SetMessage(true, "Select a <b>Store</b>", "warning");
                else
                    Driven.App.BrandPeriodSalesReporting.SetMessage(true, "Select a <b>Store</b> and <b>Reporting Period</b>", "warning");
            }
        },


        CreateToplineCorrection_SubmitBegin: function (xhr) {
            Driven.App.BrandPeriodSalesReporting.SetLoading(true);
            $("#" + _private.View.SaveButton).prop('disabled', true);
            //$("#" + _private.View.CreateToplineModalId).find(':submit').prop('disabled', true);
        },


        CreateToplineCorrection_SubmitComplete: function (xhr, status) {
            Driven.App.BrandPeriodSalesReporting.SetLoading(false);
            $("#" + _private.View.SaveButton).prop('disabled', false);
            //$("#" + _private.View.CreateToplineModalId).find(':submit').prop('disabled', false);
        },


        CreateToplineCorrection_SubmitFailure: function (jqXHR, textStatus, errorThrown) {
            Driven.App.BrandPeriodSalesReporting.SetMessage(false, Driven.App.BrandPeriodSalesReporting.FormatErrorMessage(jqXHR, textStatus));
        },


        CreateToplineCorrection_SubmitSuccess: function (data, status, xhr) {
            var $dialogContainer = $("#" + _private.View.CreateToplineCorrectionModalId);
            var $form = $dialogContainer.find("form");
            var result = JSON.parse($form.find("#" + _private.View.DialogResult).val());

            if (result.Success == true) {
                $dialogContainer.modal('hide');
                _private.ProcessList();
                Driven.App.BrandPeriodSalesReporting.SetMessage(true, result.Message);
            }
            else {
                Driven.App.BrandPeriodSalesReporting.SubmitCorrections.CreateToplineCorrection.InitForm();
            }
        },


        OpenEditToplineCorrection: function (e) {
            Driven.App.BrandPeriodSalesReporting.SubmitCorrections.EditToplineCorrection.Init({
                Url: _private.View.EditToplineCorrectionUrl,
                ModalId: _private.View.EditToplineCorrectionModalId,
                TargetId: _private.View.EditToplineCorrectionTargetId
            });

            var requestId = $(e).attr('data-requestid');
            var toplineId = $(e).attr('data-toplineid');

            Driven.App.BrandPeriodSalesReporting.SubmitCorrections.EditToplineCorrection.OpenModal({ requestId: requestId, toplineId: toplineId });
        },


        EditToplineCorrection_SubmitBegin: function (xhr) {
            Driven.App.BrandPeriodSalesReporting.SetLoading(true);
            $("#" + _private.View.SaveButton).prop('disabled', true);
            //$("#" + _private.View.EditToplineModalId).find(':submit').prop('disabled', true);
        },


        EditToplineCorrection_SubmitComplete: function (xhr, status) {
            Driven.App.BrandPeriodSalesReporting.SetLoading(false);
            $("#" + _private.View.SaveButton).prop('disabled', false);
            //$("#" + _private.View.EditToplineModalId).find(':submit').prop('disabled', false);
        },


        EditToplineCorrection_SubmitFailure: function (jqXHR, textStatus, errorThrown) {
            Driven.App.BrandPeriodSalesReporting.SetMessage(false, Driven.App.BrandPeriodSalesReporting.FormatErrorMessage(jqXHR, textStatus));
        },


        EditToplineCorrection_SubmitSuccess: function (data, status, xhr) {
            var $dialogContainer = $("#" + _private.View.EditToplineCorrectionModalId);
            var $form = $dialogContainer.find("form");
            var result = JSON.parse($form.find("#" + _private.View.DialogResult).val());

            if (result.Success == true) {
                $dialogContainer.modal('hide');
                _private.ProcessList();
                Driven.App.BrandPeriodSalesReporting.SetMessage(true, result.Message);
            }
            else {
                Driven.App.BrandPeriodSalesReporting.SubmitCorrections.EditToplineCorrection.InitForm();
            }
        },


        Init: function () {
            $.ajaxSetup({ cache: false });

            _private.InitList();
            _private.ProcessList();

            $("#" + _private.View.FilterStoreDropdown).change(_private.ProcessList);
            $("#" + _private.View.FilterStatusDropdown).change(_private.ProcessList);
            $("#" + _private.View.FilterPeriodDropdown).change(_private.ProcessList);

            $("#" + _private.View.CreateToplineCorrectionButton).click(_private.OpenCreateToplineCorrection);
            Driven.App.BrandPeriodSalesReporting.SubmitCorrections.OpenEditToplineCorrection = _private.OpenEditToplineCorrection;

            Driven.App.BrandPeriodSalesReporting.InitModals();

            Driven.App.BrandPeriodSalesReporting.SubmitCorrections.CreateToplineCorrection.SubmitBegin = _private.CreateToplineCorrection_SubmitBegin;
            Driven.App.BrandPeriodSalesReporting.SubmitCorrections.CreateToplineCorrection.SubmitComplete = _private.CreateToplineCorrection_SubmitComplete;
            Driven.App.BrandPeriodSalesReporting.SubmitCorrections.CreateToplineCorrection.SubmitFailure = _private.CreateToplineCorrection_SubmitFailure;
            Driven.App.BrandPeriodSalesReporting.SubmitCorrections.CreateToplineCorrection.SubmitSuccess = _private.CreateToplineCorrection_SubmitSuccess;

            Driven.App.BrandPeriodSalesReporting.SubmitCorrections.EditToplineCorrection.SubmitBegin = _private.EditToplineCorrection_SubmitBegin;
            Driven.App.BrandPeriodSalesReporting.SubmitCorrections.EditToplineCorrection.SubmitComplete = _private.EditToplineCorrection_SubmitComplete;
            Driven.App.BrandPeriodSalesReporting.SubmitCorrections.EditToplineCorrection.SubmitFailure = _private.EditToplineCorrection_SubmitFailure;
            Driven.App.BrandPeriodSalesReporting.SubmitCorrections.EditToplineCorrection.SubmitSuccess = _private.EditToplineCorrection_SubmitSuccess;
        }
    };


    // public
    var _public = {
        Init: _private.Init
    };

    return _public;

})();


$(function () {
    Driven.App.BrandPeriodSalesReporting.SubmitCorrections.Init();
});
