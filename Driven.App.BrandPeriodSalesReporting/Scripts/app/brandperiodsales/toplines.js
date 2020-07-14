if (typeof Driven === "undefined") { Driven = {}; }
if (typeof Driven.App === "undefined") { Driven.App = {}; }
if (typeof Driven.App.BrandPeriodSalesReporting === "undefined") { Driven.App.BrandPeriodSalesReporting = {}; }
if (typeof Driven.App.BrandPeriodSalesReporting.BrandPeriodSales === "undefined") { Driven.App.BrandPeriodSalesReporting.BrandPeriodSales = {}; }

Driven.App.BrandPeriodSalesReporting.BrandPeriodSales.Toplines = (function () {

    // private
    var _private = {

        View: {
            ListToplinesTableId: "brandPeriodSalesTable",
            ListToplinesTableSpinner: "brandPeriodSalesTableSpinner",
            ListToplinesTableContainer: "brandPeriodSalesTableContainer",
            ListToplinesUrl: "BrandPeriodSales/GetToplines",
            ListToplinesLength: 10,
            ListToplinesMaxRows: 10000,

            FilterStoreDropdown: "StoreId",
            FilterStatusDropdown: "StatusId",
            FilterPeriodDropdown: "PeriodEndDate",
            FilterIsPastDueCheckbox: "IsPastDue",

            SecIsAdmin: "IsAdmin",
            SecIsApprover: "IsApprover",
            SecTokenName: "__RequestVerificationToken",
            SecTokenValue: "__AjaxAntiForgeryForm",

            SelectAllTopLines: "selectAll",
            SubmitToplinesButton: "SubmitBtn",
            SubmitToplinesUrl: "BrandPeriodSales/SubmitToplines",

            DialogResult: "DialogResult",
            EditToplineUrl: "BrandPeriodSales/EditTopline",
            EditToplineModalId: "EditToplineModal",
            EditToplineTargetId: "EditToplinePartialView"
        },


        InitList: function () {
            var tableId = _private.View.ListToplinesTableId;
            var displayLength = _private.View.ListToplinesLength;

            var isAdmin = ($("#" + _private.View.SecIsAdmin).val().toUpperCase() == "TRUE");
            var isApprover = ($("#" + _private.View.SecIsApprover).val().toUpperCase() == "TRUE");
            var showCheckboxes = isAdmin || isApprover;

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
                "responsive": {
                    "details": {
                        "type": 'column',
                        "target": 11
                    }
                },

                "language": { "emptyTable": "No available reporting periods" },
                "select": { "style": 'multi', "selector": 'td:first-child input' },

                "columnDefs": [
                    { "targets": 0, "sClass": "center", "visible": showCheckboxes, "orderable": false, "responsivePriority": 1 },
                    { "targets": 1, "visible": false, "responsivePriority": 2 },
                    { "targets": 2, "visible": false, "responsivePriority": 2 },
                    { "targets": 3, "sClass": "left", "responsivePriority": 1 },
                    { "targets": 4, "sClass": "center", "responsivePriority": 1 }, 
                    { "targets": 5, "sClass": "center", "responsivePriority": 1 },
                    { "targets": 6, "sClass": "right", "responsivePriority": 2 },
                    { "targets": 7, "sClass": "right", "responsivePriority": 2 },
                    { "targets": 8, "sClass": "right", "responsivePriority": 2 },
                    { "targets": 9, "sClass": "right", "responsivePriority": 2 },
                    { "targets": 10, "orderable": false, "sClass": "centernowrap", "responsivePriority": 1 },
                    { "targets": 11, "orderable": false, "sClass": "control" }
                ],

                "order": [[3, 'asc']],

                "createdRow": function (row, data, index) {
                    if (data[5] == 'For Approval') {
                        $(row).addClass('selectable');
                    }
                },

                "headerCallback": function ( thead, data, start, end, display ) {
                    var dt = this.api();
                    var selectableRows = dt.rows('.selectable').count();

                    $("#" + _private.View.SelectAllTopLines).prop('checked', false);
                    if (selectableRows)
                        $("#" + _private.View.SelectAllTopLines).prop('disabled', false);
                    else
                        $("#" + _private.View.SelectAllTopLines).prop('disabled', true);
                },

                "footerCallback": function (row, data, start, end, display) {
                    var dt = this.api();
                    var selectableRows = 0;
                    var selectableAmt = 0;

                    dt.rows('.selectable').every(function (rowIdx, tableLoop, rowLoop) {
                        selectableRows ++;
                        selectableAmt += parseFloat(this.data()[6].replace(/[\$,]/g, ''));
                    });

                    $(dt.column(0).footer()).html('<h5 class="pull-left"><strong><i>' + selectableRows + ' For Approval totaling $' + selectableAmt.toFixed(2).replace(/(\d)(?=(\d{3})+\.)/g, "$1,") + '</i></strong></h5>');
                }

            });
        },


        onSelectHandler: function (e, dt, type, indexes) {
            var selectedRows = dt.rows({ selected: true }).count();
            var selectableRows = dt.rows('.selectable').count();

            if (selectedRows == selectableRows)
                $("#" + _private.View.SelectAllTopLines).prop('checked', true);
            else
                $("#" + _private.View.SelectAllTopLines).prop('checked', false);

            if (selectedRows)
                $("#" + _private.View.SubmitToplinesButton).removeAttr('disabled');
            else
                $("#" + _private.View.SubmitToplinesButton).attr('disabled', 'disabled');
        },


        ProcessList: function () {
            var storeId = $("#" + _private.View.FilterStoreDropdown).val();
            var periodEndDate = $("#" + _private.View.FilterPeriodDropdown).val();
            var statusId = $("#" + _private.View.FilterStatusDropdown).val();
            var isPastDue = $("#" + _private.View.FilterIsPastDueCheckbox).is(':checked');

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

            if (isPastDue) {
                params["ispastdue"] = isPastDue;
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
                var chk = "<input type=\"checkbox\" value=\"\" disabled>";
                if (o.Status == 'For Approval')
                    chk = "<input type=\"checkbox\" value=\"\">";
                var btn = _private.BuildToplineActions(o.ToplineId, o.CanEdit, o.CanApprove, o.CanSubmit);
                var row = [chk, o.ToplineId, o.CanSubmit, o.StoreId, o.PeriodEndDate, o.Status, o.NetSales, o.TotalTickets, o.FranCalcRoyalty, o.FranCalcAdvertising, btn, '&nbsp;'];

                // little bit of dirty formatting...
                if (o.Status == "Approved") {
                    for (var z = 3, len = row.length; z < len - 1; z++) {
                        row[z] = "<i><span class=\"text-muted\">" + row[z] + "</span></i>";
                    }
                } else {
                    row[4] = (o.IsPastDue) ? "<span class=\"text-danger\"><strong>" + o.PeriodEndDate + "</strong></span>" : o.PeriodEndDate;
                }

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


        BuildToplineActions: function (toplineId, canEdit, canApprove, canSubmit) {

            var d = "data-toplineid=\"" + toplineId + "\"";
            var btns = [];

            if (canEdit == true)
                btns.push(_private.BuildHtmlLink("Edit", d, "Driven.App.BrandPeriodSalesReporting.BrandPeriodSales.OpenEditTopline"));

            if (canSubmit == true)
                btns.push(_private.BuildHtmlLink("Approve", d, "Driven.App.BrandPeriodSalesReporting.BrandPeriodSales.SubmitTopline"));

            return btns.join("&nbsp;|&nbsp;");
        },


        SelectAll: function (table) {
            table.rows('.selectable').every(function (rowIdx, tableLoop, rowLoop) {
                $(this.node()).find('input').prop('checked', true);
                this.select();
            });
        },


        SelectNone: function (table) {
            table.rows('.selected').every(function (rowIdx, tableLoop, rowLoop) {
                $(this.node()).find('input').prop('checked', false);
                this.deselect();
            });
        },


        GetDataTable: function (id) {
            return $("#" + id).DataTable();
        },


        GetSelectedIds: function (table) {
            return $.map(table.rows('.selected').data(), function (item) { return item[1]; });
        },


        UpdateToplines: function (toplineIds, actionButton, url) {
            var url = Driven.App.BrandPeriodSalesReporting.GetRelativeUrl(url);
            var tokenName = _private.View.SecTokenName;
            var tokenValue = $('input[name="' + tokenName + '"]', '#' + _private.View.SecTokenValue).val();

            var params = {};
            params[tokenName] = tokenValue,
            params["toplines"] = toplineIds.join(",");

            Driven.App.BrandPeriodSalesReporting.SetLoading(true);
            $("#" + actionButton).prop('disabled', true);

            $.ajax({ url: url, cache: false, type: 'POST', data: params })
            .done(function (data, textStatus, jqXHR) {
                var result = $.parseJSON(data);
                Driven.App.BrandPeriodSalesReporting.SetMessage(result.Success, result.Message);
                _private.ProcessList();
            })
            .fail(function (jqXHR, textStatus, errorThrown) {
                Driven.App.BrandPeriodSalesReporting.SetMessage(false, Driven.App.BrandPeriodSalesReporting.FormatErrorMessage(jqXHR, textStatus));
            })
            .always(function () {
                $("#" + actionButton).prop('disabled', false);
                Driven.App.BrandPeriodSalesReporting.SetLoading(false);
            });
        },


        SelectTopLines: function () {
            if ($(this).is(':checked')) {
                _private.SelectAll(_private.GetDataTable(_private.View.ListToplinesTableId));
            } else {
                _private.SelectNone(_private.GetDataTable(_private.View.ListToplinesTableId));
            }
        },


        SubmitSelectedToplines: function () {
            bootbox.confirm("Approve Selected Items?", function (result) {
                if (result) {
                    var table = _private.GetDataTable(_private.View.ListToplinesTableId);
                    var validIds = $.map(table.rows('.selected').data(), function (item) {
                        // item[1] = toplineId; item[2] = canSubmit
                        if (item[2]) return item[1];
                    });

                    _private.SelectNone(table);
                    if (validIds && validIds.length > 0) {
                        _private.UpdateToplines(validIds, _private.View.SubmitToplinesButton, _private.View.SubmitToplinesUrl);
                    }
                }
            });
        },


        SubmitTopline: function (e) {
            bootbox.confirm("Approve Item?", function (result) {
                if (result) {
                    var toplineId = $(e).attr('data-toplineid');
                    _private.UpdateToplines([toplineId], _private.View.SubmitToplinesButton, _private.View.SubmitToplinesUrl);
                }
            });
        },


        EditTopline_SubmitBegin: function (xhr) {
            Driven.App.BrandPeriodSalesReporting.SetLoading(true);
            $("#" + _private.View.EditToplineModalId).find(':submit').prop('disabled', true);
        },


        EditTopline_SubmitComplete: function (xhr, status) {
            Driven.App.BrandPeriodSalesReporting.SetLoading(false);
            $("#" + _private.View.EditToplineModalId).find(':submit').prop('disabled', false);
        },


        EditTopline_SubmitFailure: function (jqXHR, textStatus, errorThrown) {
            Driven.App.BrandPeriodSalesReporting.SetMessage(false, Driven.App.BrandPeriodSalesReporting.FormatErrorMessage(jqXHR, textStatus));
        },


        EditTopline_SubmitSuccess: function (data, status, xhr) {
            var $dialogContainer = $("#" + _private.View.EditToplineModalId);
            var $form = $dialogContainer.find("form");
            var result = JSON.parse($form.find("#" + _private.View.DialogResult).val());

            if (result.Success == true) {
                $dialogContainer.modal('hide');
                _private.ProcessList();
                Driven.App.BrandPeriodSalesReporting.SetMessage(true, result.Message);
            }
            else {
                Driven.App.BrandPeriodSalesReporting.BrandPeriodSales.EditTopline.InitForm();
            }
        },


        OpenEditTopline: function (e) {
            Driven.App.BrandPeriodSalesReporting.BrandPeriodSales.EditTopline.Init({
                Url: _private.View.EditToplineUrl,
                ModalId: _private.View.EditToplineModalId,
                TargetId: _private.View.EditToplineTargetId
            });
            
            var toplineId = $(e).attr('data-toplineid');
            Driven.App.BrandPeriodSalesReporting.BrandPeriodSales.EditTopline.OpenModal({ toplineId: toplineId });
        },


        Init: function () {
            $.ajaxSetup({ cache: false });

            _private.InitList();
            _private.ProcessList();

            _private.GetDataTable(_private.View.ListToplinesTableId).on('select', _private.onSelectHandler);
            _private.GetDataTable(_private.View.ListToplinesTableId).on('deselect', _private.onSelectHandler);

            $("#" + _private.View.FilterStoreDropdown).change(_private.ProcessList);
            $("#" + _private.View.FilterStatusDropdown).change(_private.ProcessList);
            $("#" + _private.View.FilterPeriodDropdown).change(_private.ProcessList);
            $("#" + _private.View.FilterIsPastDueCheckbox).change(_private.ProcessList);

            $("#" + _private.View.SelectAllTopLines).click(_private.SelectTopLines);

            $("#" + _private.View.SubmitToplinesButton).click(_private.SubmitSelectedToplines);
            $("#" + _private.View.ApproveToplinesButton).click(_private.ApproveSelectedToplines);

            Driven.App.BrandPeriodSalesReporting.BrandPeriodSales.ApproveTopline = _private.ApproveTopline;
            Driven.App.BrandPeriodSalesReporting.BrandPeriodSales.SubmitTopline = _private.SubmitTopline;
            Driven.App.BrandPeriodSalesReporting.BrandPeriodSales.OpenEditTopline = _private.OpenEditTopline;

            Driven.App.BrandPeriodSalesReporting.InitModals();
            Driven.App.BrandPeriodSalesReporting.BrandPeriodSales.EditTopline.SubmitBegin = _private.EditTopline_SubmitBegin;
            Driven.App.BrandPeriodSalesReporting.BrandPeriodSales.EditTopline.SubmitComplete = _private.EditTopline_SubmitComplete;
            Driven.App.BrandPeriodSalesReporting.BrandPeriodSales.EditTopline.SubmitFailure = _private.EditTopline_SubmitFailure;
            Driven.App.BrandPeriodSalesReporting.BrandPeriodSales.EditTopline.SubmitSuccess = _private.EditTopline_SubmitSuccess;
        }
    };


    // public
    var _public = {
        Init: _private.Init
    };

    return _public;

})();


$(function () {
    Driven.App.BrandPeriodSalesReporting.BrandPeriodSales.Toplines.Init();
});
