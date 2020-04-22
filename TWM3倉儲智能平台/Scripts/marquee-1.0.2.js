$.fn.extend({
    marquee: function (sDirection, speed, bOneByOne, showTime, bSingleLine) {

        /*--- 初始化参数 ---*/
        if (sDirection == undefined)    // 默认向上滚动
            sDirection = "up";

        if (bOneByOne == undefined)     // bOneByOne = true时，段间间隔显示区域一半的距离
            bOneByOne = true;           // bOneByOne = false时，段间是end to end即收尾相接的

        if (speed == undefined)
            speed = 2;
        else if (speed < 0)
            speed = 0;
        else if (speed > 10)
            speed = 10;

        var nStep;          // 内层div的步进量
        var nInterval;      // 定时驱动移动程序

        switch (speed) {
            case 0:
                nStep = 1;
                nInterval = 128;
                break;
            case 1:
                nStep = 1;
                nInterval = 64;
                break;
            case 2:
                nStep = 1;
                nInterval = 32;
                break;
            case 3:
                nStep = 1;
                nInterval = 16;
                break;
            case 4:
                nStep = 1;
                nInterval = 8;
                break;
            case 5:
                nStep = 1;
                nInterval = 4;
                break;
            case 6:
                nStep = 1;
                nInterval = 2;
                break;
            case 7:
                nStep = 1;
                nInterval = 1;
                break;
            case 8:
                nStep = 2;
                nInterval = 1;
                break;
            case 9:
                nStep = 4;
                nInterval = 1;
                break;
            case 10:
                nStep = 8;
                nInterval = 1;
                break;
        }

        if (showTime == undefined)
            showTime = 0;

        var isShown = false;    // 是否停下来展示过了
        var isStop = false;   // 停下来，用于展示，展示时间=showTime

        if (bSingleLine == undefined)
            bSingleLine = true;

        var THIS = this;

        /*--- 初始化每个段的位置 ---*/

        var contentMeasure = 0;  // 所有滚动段落的宽度或者高度之和

        THIS.children().each(function (index, value) {
            var interDiv = $(value);

            interDiv.css("position", "absolute");

            if(bSingleLine)
               interDiv.css( "white-space", "nowrap");

            // marquee的位置初始化
            switch (sDirection.toLowerCase()) {
                case "up":
                    value.displacement = THIS.outerHeight() + contentMeasure;

                    if (bOneByOne)
                        contentMeasure += interDiv.outerHeight() + THIS.outerHeight() / 2;
                    else
                        contentMeasure += interDiv.outerHeight();
                    break;
                case "down":
                    value.displacement = -interDiv.outerHeight() - contentMeasure;

                    if (bOneByOne)
                        contentMeasure += interDiv.outerHeight() + THIS.outerHeight() / 2;
                    else
                        contentMeasure += interDiv.outerHeight();
                    break;
                case "left":
                    value.displacement = THIS.outerWidth() + contentMeasure;

                    if (bOneByOne)
                        contentMeasure += interDiv.outerWidth() + THIS.outerWidth() / 2;
                    else
                        contentMeasure += interDiv.outerWidth();
                    break;
                case "right":
                    value.displacement = -interDiv.outerWidth() - contentMeasure;

                    interDiv.addClass("right");         // 使元素从右到左排列（包括文字和图片）

                    if (bOneByOne)
                        contentMeasure += interDiv.outerWidth() + THIS.outerWidth() / 2;
                    else
                        contentMeasure += interDiv.outerWidth();
                    break;
            }
        });

        var timer;
        var marquee_start = function () {

            switch (sDirection.toLowerCase()) {
                case "up":
                    if (isStop) break;

                    THIS.children().each(function (index, value) {
                        var interDiv = $(value);

                        if (value.displacement < nStep &&
                                value.displacement >= 0 &&
                                showTime > 0 &&
                                !isShown) {
                            isShown = true;
                            isStop = true;
                        }

                        isShown = false;

                        interDiv.css("top", value.displacement + "px");

                        if ((value.displacement + interDiv.outerHeight()) <= 0) {

                            if (THIS.children().length == 1 ||
                                        contentMeasure - interDiv.outerHeight() < THIS.outerHeight())
                                value.displacement = THIS.outerHeight();
                            else
                                value.displacement = contentMeasure + value.displacement;

                        }
                        else
                            value.displacement -= nStep;
                    });

                    break;
                case "left":
                    if (isStop) break;

                    THIS.children().each(function (index, value) {
                        var interDiv = $(value);

                        if (value.displacement < nStep &&
                                value.displacement >= 0 &&
                                showTime > 0 &&
                                !isShown) {
                            isShown = true;
                            isStop = true;
                        }

                        isShown = false;

                        interDiv.css("left", value.displacement + "px");

                        if ((value.displacement + interDiv.outerWidth()) <= 0) {
                            if (THIS.children().length == 1 ||
                                    contentMeasure - interDiv.outerWidth() < THIS.outerWidth())
                                value.displacement = THIS.outerWidth();
                            else
                                value.displacement = contentMeasure + value.displacement;
                        }
                        else
                            value.displacement -= nStep;

                    });
                    break;
                case "right":
                    if (isStop) break;

                    THIS.children().each(function (index, value) {
                        var interDiv = $(value);

                        if (value.displacement < nStep &&
                                value.displacement >= 0 &&
                                showTime > 0 &&
                                !isShown) {
                            isShown = true;
                            isStop = true;
                        }

                        isShown = false;

                        interDiv.css("left", value.displacement + "px");

                        if (value.displacement >= THIS.outerWidth()) {
                            if (THIS.children().length == 1 ||
                                contentMeasure - interDiv.outerWidth() < THIS.outerWidth())
                                value.displacement = -interDiv.outerWidth();
                            else
                                value.displacement = -contentMeasure + value.displacement;
                        }
                        else
                            value.displacement += nStep;
                    });
                    break;
                case "down":
                    if (isStop) break;

                    THIS.children().each(function (index, value) {
                        var interDiv = $(value);

                        if (value.displacement < nStep &&
                                value.displacement >= 0 &&
                                showTime > 0 &&
                                !isShown) {
                            isShown = true;
                            isStop = true;
                        }

                        isShown = false;

                        interDiv.css("top", value.displacement + "px");

                        if (value.displacement >= THIS.outerHeight()) {
                            if (THIS.children().length == 1 ||
                                contentMeasure - interDiv.outerHeight() < THIS.outerHeight())
                                value.displacement = -interDiv.outerHeight();
                            else
                                value.displacement = -contentMeasure + value.displacement;
                        }
                        else
                            value.displacement += nStep;
                    });
                    break;
            }

            if (isStop) {
                isStop = false;
                timer = setTimeout(marquee_start, showTime);
            }
            else
                timer = setTimeout(marquee_start, nInterval);
        };

        var marquee_pause = function () {
            clearTimeout(timer);
        };

        // 启动marquee
        marquee_start();

        // 当展示模式时，取消鼠标操作
        if (showTime > 0) return;

        THIS.children().each(function (index, value) {
            var interDiv = $(value);

            // 当鼠标移动到内DIV上时暂停
            interDiv.mouseover(function () {
                marquee_pause();
            });

            // 当鼠标离开内DIV上时继续
            interDiv.mouseout(function () {
                marquee_start();
            });
        });

    }
});