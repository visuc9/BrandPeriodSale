CKEDITOR.editorConfig = function (config) {

    config.extraPlugins = 'placeholder,richcombo,floatpanel,panel,listblock,button,placeholder_select,doNothing,elementspath_custom',

    config.toolbar_Basic = [['placeholder_select']];

    config.toolbar = 'Basic';
    config.placeholder_select = { placeholders: ['AdvertisingFee', 'Brand', 'LocalStoreID', 'LocalStoreID-ReportingPeriodList', 'Logo', 'ReportingPeriod', 'ReportingPeriodList', 'RequestNumber', 'RoyaltyFee', 'ServiceFee'] };

    config.removePlugins = 'elementspath';
    config.resize_enabled = false;

    config.height = '34px';
    config.keystrokes = [[13 /*Enter*/, 'doNothing'], [CKEDITOR.SHIFT + 13 /* shift + Enter*/, 'doNothing']];
};
