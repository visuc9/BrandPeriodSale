CKEDITOR.editorConfig = function (config) {

    config.extraPlugins = 'placeholder,richcombo,floatpanel,panel,listblock,button,placeholder_select,doNothing,elementspath_custom',
    config.toolbar_None = [[]];

    config.toolbar = 'None';
    config.removePlugins = 'elementspath';
    config.resize_enabled = false;

    config.height = '34px';
    config.keystrokes = [[13 /*Enter*/, 'doNothing'], [CKEDITOR.SHIFT + 13 /* shift + Enter*/, 'doNothing']];
};
