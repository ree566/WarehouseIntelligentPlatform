jQuery(function ($) {
    $.timepicker.regional['zh-TW'] = {
        timeOnlyTitle: '選擇時間',
        timeText: '時間',
        hourText: '時',
        minuteText: '分',
        secondText: '秒',
        millisecText: '毫秒',
        timezoneText: '時區',
        currentText: '當前',
        closeText: '完成',
        timeFormat: 'HH:mm',
        amNames: ['AM', 'A'],
        pmNames: ['PM', 'P'],
        isRTL: false
    };
    $.timepicker.setDefaults($.timepicker.regional['zh-TW']);
});