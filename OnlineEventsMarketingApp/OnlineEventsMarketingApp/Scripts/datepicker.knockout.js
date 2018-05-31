ko.bindingHandlers.datePicker = {
	init: function (element, valueAccessor, allBindingsAccessor, viewModel) {
		var dataSource = valueAccessor();

		var options = {
			keyboardNavigation: true,
			autoclose: true,
			format: 'mm/dd/yyyy'
		};

		$(element).datepicker(options);
		$(element).datepicker('update', dataSource());
		$(element).on("changeDate", function (ev) {
			var observable = valueAccessor();
			if ($(element).is(':focus')) {
				// Don't update while the user is in the field...
				// Instead, handle focus loss
				$(element).one('blur', function (ev) {
					var dateVal = $(element).datepicker("getDate");
					observable(dateVal);
				});
			}
			else {
				observable(ev.date);
			}
		});
		//handle removing an element from the dom
		ko.utils.domNodeDisposal.addDisposeCallback(element, function () {
			$(element).datepicker('remove');
		});
	},
	update: function (element, valueAccessor) {
		var value = ko.utils.unwrapObservable(valueAccessor());

		value = (typeof (value) !== 'object') ? new Date(parseFloat(value.replace(/[^0-9]/g, ''))) : value;
		$(element).datepicker('update', value);
	}
};