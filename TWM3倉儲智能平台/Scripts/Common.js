function createFrame(url) {
    var s = '<iframe name="mainFrame" scrolling="auto" frameborder="0"  src="' + url + '" style="width:100%;height:100%;"></iframe>';
    return s;
}

/***************************************
*打开窗口相关
****************************************/
function OpenDialog(targetUrl, width, height, useCurrentDir) {
    //是否在路径前加入当前路径
    if ((typeof useCurrentDir != "boolean") || (useCurrentDir == true)) {//加
        targetUrl = systemCurrentDirectory + targetUrl;
    }
    var currentTime = new Date();
    var path = systemAbsolutePath + "Dialog.aspx?system_renew=" + currentTime.getTime() + "&targetUrl=" + encodeURIComponent(targetUrl);

    return window.showModalDialog(path, window, "unadorned:1;status:1;help:0;edge:raised;scroll:0;dialogHeight:" + height + "px;" + "dialogWidth:" + width + "px;")
}

function OpenWindow(targetUrl, width, height, useCurrentDir, top, left) {
    //使用Frame作为窗口
    if (systemWindowStyle == 'Frame') {
        //是否在路径前加入当前路径
        if ((typeof useCurrentDir != "boolean") || (useCurrentDir == true)) {//加
            targetUrl = systemCurrentDirectory + targetUrl;
        }

        /*
        var path= systemAbsolutePath + "Window.aspx?targetUrl=" + encodeURIComponent(targetUrl);
        var oTarget = document.getElementById("WindowFrame");
        if(oTarget != null)
        {
        document.body.removeChild(oTarget);		
        }
        var bodyWidth = document.body.clientWidth;
        var bodyHeight = document.body.clientHeight;		
        var scrLeft = document.body.scrollLeft;
        var scrTop = document.body.scrollTop;
        var posTop;		
        if (typeof top == "number")
        {
        posTop = top;
        }
        else
        {
        posTop = (bodyHeight - height)/2 + scrTop;	
        }
        var posLeft;
        if (typeof left == "number")
        {
        posLeft = left;
        }
        else
        {
        posLeft = (bodyWidth -width)/2 + scrLeft;
        }
        var html = "<IFRAME ID='WindowFrame' src='" + path + "' SCROLLING='no' frameborder='0' ";
        html += "progid:DXImageTransform.Microsoft.[glass](duration=1) ";
        html +="width='" + width + "' height='" + height +  "'";
        html +=" style='position:absolute;left:" + posLeft + "px;top:" + posTop + "px;'></IFRAME>";
        var oFrame = document.createElement(html);
        document.body.insertAdjacentElement("afterBegin",oFrame);
        */
        var actionType = GetQueryString(targetUrl, "type");
        var titleText = '';
        switch (actionType) {
            case "A":
                titleText = "新增";
                break;
            case "U":
                titleText = "修改";
                break;
            case "D":
                titleText = "删除";
                break;
            case "V":
                titleText = "查看";
                break;
            default:
                titleText = "";
                break;
        }

        var tempTag = $("#WindowFrame");
        if (tempTag != null) {
            tempTag.remove();
        }
        var myWindow = $("<div id='WindowFrame'></div>");
        $('body').append(myWindow);
        $("#WindowFrame").dialog({
            title: titleText,
            href: targetUrl,
            width: width,
            height: height,
            modal: true
        });
    }
    //使用Window作为窗口
    else if (systemWindowStyle == "Window") {
        var leftPos = (screen.width - width) / 2;
        var topPos = (screen.height - height) / 2;
        var status = "top=40,left=" + leftPos + ",top=" + topPos + ",height=" + height + ",width=" + width + ",status=no,toolbar=no,menubar=no,scrollbars=yes,resizable=no";
        window.open(targetUrl, null, status);
    }
}

function GetQueryString(sURL, sKey) {
    var vValue;
    vValue = sURL.match("(\\?|&)" + sKey + "=([^&$]*)");
    vValue = (vValue == null || vValue.length == 1) ? "" : vValue[2];
    return vValue;
}

/*************************
自定义开新窗口函数 -- 选择窗口
**************************/
function OpenSelect(pageName, textBoxID, width, height, disableServerClick) {
    //打开对话框
    var url = systemAbsolutePath + "Common/" + pageName;
    var textBoxParam = document.all(textBoxID);
    if (textBoxParam != null) {
        url += "?text=" + textBoxParam.value;
    }
    var result = OpenDialog(url, width, height, false);

    if ((result != undefined)) {
        //设定文本框
        //result = result.split(",");
        var textBox = document.all(textBoxID);
        if (textBox != null) {
            textBox.value = result;
            textBox.focus();
            textBox.select();
        }
    }
    //是否禁止服务端点击,默认为false
    if ((typeof disableServerClick != "boolean") || (disableServerClick == true)) {
        window.event.cancelBubble = true;
        window.event.returnValue = false;
        return false;
    }
}

/*****************************
当单击radioButton时Focus inputControl
*****************************/
function FocusInputOnClickRadioButton(radioButton, inputControl) {
    if (radioButton.checked) {
        inputControl.focus();
        if (inputControl.isTextEdit) {
            inputControl.select();
        }
    }
}

/*****************************
当Focus inputControl时选中radioButton
*****************************/
function CheckRadioButtonOnInputFocus(radioButton) {
    radioButton.checked = true;
}

/*****************************
当radioButton选中时检查与它关联的输入控件值是否为空
*****************************/
function ValidOnRadioButtonChecked(radioButtonID, inputControlID) {
    var radioButton = document.getElementById(radioButtonID);
    if (radioButton.checked) {
        var inputControl = document.getElementById(inputControlID);
        if (inputControl.value.trim() != "") {
            return true;
        }
        else {
            return false;
        }
    }
    return true;
}

/*****************************
取得查询字串
*****************************/
function GetQueryString(url, name) {
    var result;
    result = url.match("(\\?|&)" + name + "=([^&$]*)");
    result = (result == null || result.length == 1) ? "" : result[2];
    return result;
}

/*****************************
给字符串添加去除空格函数
*****************************/
String.prototype.trim = function () {
    // 用正则表达式将前后空格  
    // 用空字符串替代。  
    return this.replace(/(^\s*)|(\s*$)/g, "");
}

/*****************************
DashBoard
*****************************/
// 对Date的扩展，将 Date 转化为指定格式的String 
// 月(M)、日(d)、小时(h)、分(m)、秒(s)、季度(q) 可以用 1-2 个占位符， 
// 年(y)可以用 1-4 个占位符，毫秒(S)只能用 1 个占位符(是 1-3 位的数字) 
// 例子： 
// (new Date()).Format("yyyy-MM-dd hh:mm:ss.S") ==> 2006-07-02 08:09:04.423 
// (new Date()).Format("yyyy-M-d h:m:s.S")      ==> 2006-7-2 8:9:4.18 
Date.prototype.Format = function (fmt) { //author: meizz 
    var o = {
        "M+": this.getMonth() + 1,                 //月份 
        "d+": this.getDate(),                    //日 
        "h+": this.getHours(),                   //小时 
        "m+": this.getMinutes(),                 //分 
        "s+": this.getSeconds(),                 //秒 
        "q+": Math.floor((this.getMonth() + 3) / 3), //季度 
        "S": this.getMilliseconds()             //毫秒 
    };
    if (/(y+)/.test(fmt))
        fmt = fmt.replace(RegExp.$1, (this.getFullYear() + "").substr(4 - RegExp.$1.length));
    for (var k in o)
        if (new RegExp("(" + k + ")").test(fmt))
            fmt = fmt.replace(RegExp.$1, (RegExp.$1.length == 1) ? (o[k]) : (("00" + o[k]).substr(("" + o[k]).length)));
    return fmt;
}

function isBetweenStartEndTime(startTime, endTime) {
    var result;
    var nowDate = new Date();
    var hour = nowDate.getHours();
    var minutes = nowDate.getHours();
    var compareTime = nowDate.Format("hh:mm");

    var count = 0;
    for (var i = 0; i < startTime.length; i++) {
        if (compareTime >= startTime[i] && compareTime <= endTime[i]) {
            count++;
        }
    }

    if (count > 0) {
        result = true;
    }
    else {
        result = false;
    }

    return result;
}

//轮转Tab
function turningTabIndex() {
    var t = $("#tabs");
    var currentTab = t.tabs("getSelected");
    var currentIndex = t.tabs("getTabIndex", currentTab);
    currentIndex++;
    if (currentIndex >= t.tabs("tabs").length) {
        currentIndex = 0;
    }
    t.tabs("select", currentIndex);
    refreshTab({ tabTitle: currentIndex, url: "" });
}

//刷新tab
//@data 
//example: {tabTitle:'tabTitle',url:'refreshUrl'}
//如果tabTitle为空，则默认刷新当前选中的tab
//如果url为空，则默认以原来的url进行reload
function refreshTab(data) {
    var refreshTab = data.tabTitle ? $('#tabs').tabs('getTab', data.tabTitle) : $('#tabs').tabs('getSelected');
    refreshTab.panel("refresh");

    if ($.browser.msie) {
        CollectGarbage();
    }
}