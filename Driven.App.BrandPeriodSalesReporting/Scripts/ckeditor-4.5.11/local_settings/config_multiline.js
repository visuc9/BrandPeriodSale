CKEDITOR.editorConfig = function (config) {

    config.extraPlugins = 'placeholder,colorbutton,justify,font,richcombo,floatpanel,panel,listblock,button,placeholder_select',

    config.toolbar_Advanced = [
            ['Bold', 'Italic', 'Underline', 'Strike', 'Subscript', 'Superscript', '-', 'RemoveFormat'],
            ['NumberedList', 'BulletedList', '-', 'Outdent', 'Indent', '-', 'Blockquote', 'JustifyLeft', 'JustifyCenter', 'JustifyRight', 'JustifyBlock'],
            ['TextColor', 'BGColor'],
            ['Font', 'FontSize', 'Format', 'placeholder_select']
    ];

    config.height = '100px';
    config.toolbar = 'Advanced';
    config.placeholder_select = { placeholders: ['AdvertisingFee', 'Brand', 'LocalStoreID', 'LocalStoreID-ReportingPeriodList', 'Logo', 'ReportingPeriod', 'ReportingPeriodList', 'RequestNumber', 'RoyaltyFee', 'ServiceFee'] };
};
