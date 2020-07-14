if (typeof Driven === "undefined") { Driven = {}; }
if (typeof Driven.App === "undefined") { Driven.App = {}; }
if (typeof Driven.App.BrandPeriodSalesReporting === "undefined") { Driven.App.BrandPeriodSalesReporting = {}; }
if (typeof Driven.App.BrandPeriodSalesReporting.BrandPeriodSales === "undefined") { Driven.App.BrandPeriodSalesReporting.BrandPeriodSales = {}; }

Driven.App.BrandPeriodSalesReporting.BrandPeriodSales.EditTopline = (function () {

    // private
    var _private = {

        View: {
            SaveButton: "SaveBtn",
            SaveAndApproveButton: "SaveAndApproveBtn",
            IsApproveStatus: "IsApprove",
            Title: "Title",
            ModalTitle: "ToplineModalTitle",
            NetSalesWarning: false,
            CalcRoyaltyFeeWarning: false,
            CalcAdvertisingFeeWarning: false,

            EditToplineSubmitBegin: "Driven.App.BrandPeriodSalesReporting.BrandPeriodSales.EditTopline.SubmitBegin(xhr)",
            EditToplineSubmitSuccess: "Driven.App.BrandPeriodSalesReporting.BrandPeriodSales.EditTopline.SubmitSuccess(data, status, xhr)",
            EditToplineSubmitFailure: "Driven.App.BrandPeriodSalesReporting.BrandPeriodSales.EditTopline.SubmitFailure(xhr, status, error)",
            EditToplineSubmitComplete: "Driven.App.BrandPeriodSalesReporting.BrandPeriodSales.EditTopline.SubmitComplete(xhr, status)"
        },

        Settings: { Url: "", TargetId: "",  ModalId: ""},


        Init: function (settings) {
            $.ajaxSetup({ cache: false });
            _private.Settings = settings;
        },


		InitForm: function () {
			var containerId = _private.Settings.TargetId;
			var url = Driven.App.BrandPeriodSalesReporting.GetRelativeUrl(_private.Settings.Url);
			
			$.validator.addMethod('netSalesValidator', _private.validateNetSales, '');
			$.validator.addMethod('franCalcRoyaltyValidator', _private.validateFranCalcRoyalty, '');
			$.validator.addMethod('franCalcAdvertisingValidator', _private.validateFranCalcAdvertising, '');

			$.validator.unobtrusive.adapters.add('netsales', {}, function (options) {
			    options.rules['netSalesValidator'] = true;
			    options.messages['netSalesValidator'] = options.message;
			});

			$.validator.unobtrusive.adapters.add('francalcroyalty', {}, function (options) {
			    options.rules['franCalcRoyaltyValidator'] = true;
			    options.messages['franCalcRoyaltyValidator'] = options.message;
			});

			$.validator.unobtrusive.adapters.add('francalcadvertising', {}, function (options) {
			    options.rules['franCalcAdvertisingValidator'] = true;
			    options.messages['franCalcAdvertisingValidator'] = options.message;
			});

			Driven.App.BrandPeriodSalesReporting.InitValidators(containerId);

			var $form = $("#" + containerId).find("form");

			$form.attr("action", url);
			$form.attr("method", "POST");
			$form.attr("data-ajax-method", "POST");
			$form.attr("data-ajax-update", "#" + containerId);

			$form.attr("data-ajax-begin", _private.View.EditToplineSubmitBegin);
			$form.attr("data-ajax-success", _private.View.EditToplineSubmitSuccess);
			$form.attr("data-ajax-failure", _private.View.EditToplineSubmitFailure);
			$form.attr("data-ajax-complete", _private.View.EditToplineSubmitComplete);

			$("#" + _private.View.ModalTitle).html($("#" + _private.View.Title).val());
			$("#" + _private.View.SaveAndApproveButton).click(_private.CheckWarnings);
			$("#" + _private.View.SaveButton).click(_private.CheckWarnings);
		},


		CheckWarnings: function () {
		    var containerId = _private.Settings.TargetId;
		    var $form = $("#" + containerId).find("form");
		    var isSaveAndApprove = ($(this)[0].id == _private.View.SaveAndApproveButton);
		    $("#" + _private.View.IsApproveStatus).val(isSaveAndApprove);

		    $form.validate().element("#NetSales");
		    $form.validate().element("#FranCalcRoyalty");
		    $form.validate().element("#FranCalcAdvertising");

		    if ((_private.View.NetSalesWarning) || (_private.View.CalcRoyaltyFeeWarning) || (_private.View.CalcAdvertisingFeeWarning)) {
		        bootbox.confirm('Store Totals do not equal sum of Product Sub-Groups. If this is due to "Other Sales" being included in Store Total, then you may ignore this warning. Do you want to Ignore?', function (result) {
		            if (result) {
		                _private.MarkButtonType(isSaveAndApprove);
		            }
		        });
		    }
		    else {
		        _private.MarkButtonType(isSaveAndApprove);
		    }
		},


		MarkButtonType: function (isSaveAndApprove) {
		    if (isSaveAndApprove) {
		        bootbox.confirm("Submit for Approval?", function (result) {
		            if (result) {
		                $('#form0').submit();
		            }
		        });
		    }
		    else {
		        $('#form0').submit();
		    }
		},


		validateNetSales: function (value, element, params) {
		    var totalSales = _private.getFloat(value);
		    var calcTotalSales = 0;
		    _private.View.NetSalesWarning = false;

		    $(".netsales").each(function(idx, el) {
		        calcTotalSales += _private.getFloat($(el).val());
		    });

		    if ((calcTotalSales != 0) && (totalSales != 0) && (calcTotalSales != totalSales)) {
		        _private.View.NetSalesWarning = true;
		    }

		    return true;
		},


		validateFranCalcRoyalty: function (value, element, params) {
		    var totalFranCalcRoyalty = _private.getFloat(value);
		    var calcFranCalcRoyalty = 0;
		    _private.View.CalcRoyaltyFeeWarning = false;

		    $(".francalcroyalty").each(function (idx, el) {
		        calcFranCalcRoyalty += _private.getFloat($(el).val());
		    });

		    if ((calcFranCalcRoyalty != 0) && (totalFranCalcRoyalty != 0) && (calcFranCalcRoyalty != totalFranCalcRoyalty)) {
		        _private.View.CalcRoyaltyFeeWarning = true;
		    }

		    return true;
		},


		validateFranCalcAdvertising: function (value, element, params) {
		    var totalFranCalcAdvertising = _private.getFloat(value);
		    var calcFranCalcAdvertising = 0;
		    _private.View.CalcAdvertisingFeeWarning = false;

		    $(".francalcadvertising").each(function (idx, el) {
		        calcFranCalcAdvertising += _private.getFloat($(el).val());
		    });

		    if ((calcFranCalcAdvertising != 0) && (totalFranCalcAdvertising != 0) && (calcFranCalcAdvertising != totalFranCalcAdvertising)) {
		        _private.View.CalcAdvertisingFeeWarning = true;
		    }

		    return true;
		},


		getFloat: function (value) {
		    var parsedVal = parseFloat(value.replace(/[^0-9-.]/g, ''));
		    return isNaN(parsedVal) ? 0 : parsedVal;
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


		Submit_Begin: function (xhr) {},
		Submit_Success: function (data, status, xhr) {},
		Submit_Failure: function (jqXHR, textStatus, errorThrown) {},
		Submit_Complete: function (xhr, status) {}
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
