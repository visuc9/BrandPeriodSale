if (typeof Driven === "undefined") { Driven = {}; }
if (typeof Driven.App === "undefined") { Driven.App = {}; }

Driven.App.BrandPeriodSalesReporting = {

    FillDropdown: function (id, url) {

        Driven.App.BrandPeriodSalesReporting.SetLoading(true);

        $.ajax(url, { cache: false })
            .done(function (data, textStatus, jqXHR) {
                var $ddl = $("#" + id);
                $ddl.html('');
                $.each(data, function (id, option) {
                    $ddl.append($('<option></option>').val(option.id).html(option.name));
                });
            })
            .fail(function (jqXHR, textStatus, errorThrown) {
                Driven.App.BrandPeriodSalesReporting.SetMessage(false, Driven.App.BrandPeriodSalesReporting.FormatErrorMessage(jqXHR, textStatus));
            })
            .always(function (data, textStatus, jqXHR) {
                Driven.App.BrandPeriodSalesReporting.SetLoading(false);
            });

    },


	GetRelativeUrl: function(path) {
		var appPath = $("#RelativeUrl").val();
		return appPath + path;
	},


	InitValidators: function (containerId) {
		var $container = $("#" + containerId);
		var $jQval = $.validator;
		$jQval.unobtrusive.parse($container);

		var $form = $container.find("form");
		$.validator.unobtrusive.parse($form);
	},


	InitModals: function () {
		$(document).on('show.bs.modal', '.modal', function (event) {
			var zIndex = 1040 + (10 * $('.modal:visible').length);
            $(this).css('z-index', zIndex);
            $(this).css('opacity', 1);
            $(this).css('padding-top', 110);
			setTimeout(function () {
				$('.modal-backdrop').not('.modal-stack').css('z-index', zIndex - 1).addClass('modal-stack');
			}, 0);
		});

		$('.modal').on('hidden.bs.modal', function (e) {
		    if ($('.modal').hasClass('in')) {
		        $('body').addClass('modal-open');
		    }
		});
	},


	SetMessage: function (success, msg, type) {
	    $.bootstrapGrowl(msg, {
	        type: ((type) ? type : ((success) ? 'success' : 'danger')),
			align: 'center',
			width: 'auto',
			delay: ((success) ? 4000 : 0),
			allow_dismiss: true
		});
	},


	SetLoading: function (s) {
		if (s == true) {
			$("#loadingAlert").show();
		} else {
			$("#loadingAlert").hide();
		}
	},


	FormatErrorMessage: function (jqXHR, exception) {
		if (jqXHR.status === 0) {
		    return ('<strong>Not connected.</strong> Please verify your network connection.');

		} else if (jqXHR.status == 404) {
			return ('The requested page not found. [404]');

		} else if (jqXHR.status == 412) {
		    return ('Session Timeout.');

		} else if (jqXHR.status == 405) {
		    return (JSON.parse(jqXHR.responseText));

		} else if (jqXHR.status == 500) {
		    return ('Internal Server Error [500].');

		} else if (exception === 'parsererror') {
		    return ('Requested JSON parse failed.');

		} else if (exception === 'timeout') {
		    return ('Time out error.');

		} else if (exception === 'abort') {
		    return ('Ajax request aborted.');

		} else {
			return ('<strong>Uncaught Error.</strong> ' + jqXHR.responseText);
		}
	},


    FormatTime: function (date) {
        var hours = date.getHours();
        var minutes = date.getMinutes();
        var ampm = hours >= 12 ? 'PM' : 'AM';
        hours = hours % 12;
        hours = hours ? hours : 12; // the hour '0' should be '12'
        minutes = minutes < 10 ? '0' + minutes : minutes;
        var strTime = hours + ':' + minutes + ' ' + ampm;
        return strTime;
    }


}
