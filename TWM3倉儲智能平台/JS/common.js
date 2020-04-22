/***************************************
*打开窗口相关
****************************************/
function OpenWindow(targetUrl, width, height) {
    //使用Window作为窗口
    var leftPos = (screen.width - width) / 2;
    var topPos = (screen.height - height) / 2;
    var status = "top=40,left=" + leftPos + ",top=" + topPos + ",height=" + height + ",width=" + width + ",status=no,toolbar=no,menubar=no,scrollbars=yes,resizable=no";
    window.open(targetUrl, null, status);
}