CKEDITOR.editorConfig = function (config) {

    config.extraPlugins = 'placeholder,colorbutton,justify,font,richcombo,floatpanel,panel,listblock,button,placeholder_select',

    config.toolbar_NoHtml = [['placeholder_select']];

    config.removePlugins = 'elementspath';
    config.height = '100px';
    config.toolbar = 'NoHtml';
    config.placeholder_select = { placeholders: ['AdvertisingFee', 'Brand', 'LocalStoreID', 'LocalStoreID-ReportingPeriodList', 'Logo', 'ReportingPeriod', 'ReportingPeriodList', 'RequestNumber', 'RoyaltyFee', 'ServiceFee'] };
};
