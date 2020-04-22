/* CommDataGrid client script V1.0.0     Copyright © 2005 HanRoc Corporation
* 
* Create by Draco Wang
* Date: 2005-10-25 
*/

/*************************
自定义开新窗口函数 -- 查询窗口
**************************/
function CommGridOpenQuery(commGrid_gridID)
{
    var fields = eval(commGrid_gridID + "_ColumnsQueryAndType");
    var txtFilter = document.all(commGrid_gridID + "_txtFilter");
    var resourceName = eval(commGrid_gridID + "_ResourceName");
    var target = systemAbsolutePath + "Common/CommonQuery.aspx?Fields=" + fields ;
    target += "&ResourceName=" + resourceName;
    //打开对话框
    var result = OpenDialog(target,550,340,false);
    if ((result != undefined) && (result != ""))
    {
	    txtFilter.value = result;
    }
}

/*************************
    单击选择所有复选框 
**************************/
function CommGrid_chkSelectAll_onclick(commGrid_gridID) 
{
    var isSingleCheck = eval(commGrid_gridID + "_IsSingleCheck");
	//如果是单选则取消全选功能
	if (isSingleCheck == "True")
	{
		window.event.cancelBubble = true;
		window.event.returnValue = false;	
		return;
	}
	
	var chkAll = event.srcElement;
	var divGrid = document.getElementById(commGrid_gridID + "_grdMain");
	var inputs = divGrid.all.tags("INPUT");
	var arraySel = "window." + commGrid_gridID + "_arraySel";
    eval("if (" + arraySel + "==null) " + arraySel + " = new Array();");
	
	var i;
	for (i = 0; i < inputs.length; i++)
	{
		if (inputs(i).parentElement.getAttribute("level") == "Child")
		{
			inputs(i).checked = chkAll.checked;
			if (chkAll.checked)
			{
				eval(arraySel + "[i] = inputs(i).parentElement.title;");
			}
			else
			{
				eval(arraySel + "[i] = null;");
			}
		}	
	}
	
	CommGrid_changeSelect(commGrid_gridID);		
}

/*************************
    单击单个复选框 
**************************/
function CommGrid_chk_onclick(commGrid_gridID) 
{
	var title = event.srcElement.parentElement.title;
	var chkAll;
	var divGrid = document.getElementById(commGrid_gridID + "_grdMain");
	var inputs = divGrid.all.tags("INPUT");
	var isSingleCheck = eval(commGrid_gridID + "_IsSingleCheck");

	var i;
	//如果是单选则取消功能
	if (isSingleCheck == "False")
	{
		for (i = 0; i < inputs.length; i++)
		{
			if (inputs(i).parentElement.getAttribute("level") == "Parent")
			{
				chkAll = inputs(i);
				break;
			}
		}

	    var hasChecked = false;
        for (i = 0; i < inputs.length; i++)
		{
			if ((inputs(i).checked)
				&&(inputs(i).parentElement.getAttribute("level") == "Child"))
			{
				hasChecked = true;
				break;
			}
		}
		
		chkAll.checked = hasChecked;
	}
	
	var arraySel = "window." + commGrid_gridID + "_arraySel";
    eval("if (" + arraySel + "==null) " + arraySel + " = new Array();");

    for (i = 0; i < inputs.length; i++)
	{
		if (inputs(i) == event.srcElement)
		{
			if (inputs(i).checked)
			{
				eval(arraySel + "[i] = title;");
			}
			else
			{
				eval(arraySel + "[i] = null;");
			}
		}
		else
		{	//如果是单选则取消其余选择
			if (isSingleCheck == "True")
			{
				inputs(i).checked = false;
				eval(arraySel + "[i] = null;");
			}
		}
	}

	CommGrid_changeSelect(commGrid_gridID);	
}

/*************************
    改变单击后改变的属性 
**************************/
function CommGrid_changeSelect(commGrid_gridID)
{
	var num = 0;
	var selectIDs = "";
    var temp;
	var tempArray = new Array();
    var arraySel = "window." + commGrid_gridID + "_arraySel";
	var length = eval(arraySel+".length");
	//将原始数据存入tempArray
	for (i = 0; i < length; i++)
	{
	    temp = eval(arraySel + "[i]")
		if (temp != null)
		{
			tempArray[num] = temp;
			selectIDs += "," + temp.replace(/,/g, '{*}');
			num++;
		}
	}
	//原始数据的string形式
	if (selectIDs != "")
	{
		selectIDs = selectIDs.substr(1,selectIDs.length-1);
	}
    //将选中的ID值保存到隐藏域中
	var txtSelectValue = document.all(commGrid_gridID + "_txtSelectValue");
	if (txtSelectValue != null)
	{
		txtSelectValue.value = selectIDs;
	}
	
    var showDeleteSel = eval(commGrid_gridID + "_ShowDeleteSel");
    var showUpdateSel = eval(commGrid_gridID + "_ShowUpdateSel");
    //如果显示删除选中或者更新选中则更新它们的链接
    if ((showDeleteSel == "True") || (showUpdateSel == "True"))
    {
        var idArray = new Array();
	    var idCount = 0;

        var primaryKey = eval(commGrid_gridID + "_PrimaryKey");
        var keysName = primaryKey.split(",");
	    //以下开始分割原始数据
        if (tempArray.length > 0)
	    {
		    idCount = keysName.length;

		    for (i=0;i < num;i++)
		    {
			    temp = tempArray[i];
			    if (idCount > 1)
			    {
				    temp = temp.substr(1,tempArray[i].length-2);
			    }
			    temp = temp.split(",");
			    //如果主键中的参数个数和实际参数的个数不符则不做动作
			    if (idCount != temp.length) return;

			    for (j=0;j<idCount;j++)
			    {
				    if (idArray[j] == null)
					    idArray[j] =  temp[j];
				    else
					    idArray[j] += "," + temp[j];
			    }
		    }
	    }
        var href;
        var useNumberParams = eval(commGrid_gridID + "_UseNumberParams");
        //以下设置Update超链接
	    var lbtnUpdateSel = document.all(commGrid_gridID + "_lbtnUpdateSel");
	    if (lbtnUpdateSel != null)
	    {
		    if (selectIDs == "")
		    {
			    lbtnUpdateSel.disabled = true;
                lbtnUpdateSel.removeAttribute("href");
                CommGrid_SetGray(lbtnUpdateSel.firstChild);
		    }
		    else
		    {
			    href = eval(commGrid_gridID + "_UrlUpdate");
			    if (useNumberParams == "False")
			    {//名称主键方式
			        for (i=0;i<idCount;i++)
			        {
			            temp = keysName[i].trim();
			            var r = new RegExp("\\{"+temp+"}","ig");
				        href = href.replace(r, idArray[i]);
			        }
			    }
			    else
			    {//数字主键方式
			        for (i=0;i<idCount;i++)
			        {
			            var r = new RegExp("\\{"+i+"}","g");
				        href = href.replace(r,idArray[i]);
			        }
			    }
                lbtnUpdateSel.href = href;
                lbtnUpdateSel.removeAttribute("disabled");
                lbtnUpdateSel.firstChild.style.filter = null;
		    }
	    }
	    //以下设置Delete超链接
	    var lbtnDeleteSel = document.all(commGrid_gridID + "_lbtnDeleteSel");
	    if (lbtnDeleteSel != null)
	    {
	        //lbtnDeleteSel内是一个IMG，所以点击后event.srcElement是IMG，所以要赋值给IMG
            lbtnDeleteSel.firstChild.commGrid_gridID = commGrid_gridID;

		    if (selectIDs == "")
		    {
			    lbtnDeleteSel.disabled = true;
                lbtnDeleteSel.removeAttribute("href");
                lbtnDeleteSel.detachEvent("onclick", CommGrid_OnDeleteSelConfirm);
		        CommGrid_SetGray(lbtnDeleteSel.firstChild);
            }
		    else
		    {
			    href = eval(commGrid_gridID + "_UrlDelete");
			    if (useNumberParams == "False")
			    {//名称主键方式
			        for (i=0;i<idCount;i++)
			        {
			            temp = keysName[i].trim();
			            var r = new RegExp("\\{"+temp+"}","ig");
				        href = href.replace(r, idArray[i]);
			        }
			    }
                else
                {//数字主键方式
			        for (i=0;i<idCount;i++)
			        {
			            var r = new RegExp("\\{"+i+"}","g");
				        href = href.replace(r,idArray[i]);
			        }
                }
			    lbtnDeleteSel.href = href;
                lbtnDeleteSel.removeAttribute("disabled");
                var showConfirmDelSel = eval(commGrid_gridID + "_ShowConfirmDelSel");
                if (showConfirmDelSel == "True")
                {
                    lbtnDeleteSel.attachEvent("onclick", CommGrid_OnDeleteSelConfirm);
                }
                lbtnDeleteSel.firstChild.style.filter = null;
		    }
	    }
    }
}

/*************************
    取得单击单选按钮的项目的编号
**************************/
function CommGrid_GetSingleSelectedID(commGrid_gridID)
{
	var result = "";
	var txtSelectValue = document.all(commGrid_gridID + "_txtSelectValue");
	if (txtSelectValue != null)
	{
		result = txtSelectValue.value;
		result = result.replace(/\{*}/g,",");
		result = result.replace(/\{/g,"");
		result = result.replace(/}/g,"");
	}
	return result;
}

/*************************
    点击删除选中按钮后的事件 
**************************/
function CommGrid_OnDeleteSelConfirm()
{
    var commGrid_gridID = event.srcElement.commGrid_gridID;
    var confirmDelSelString = eval(commGrid_gridID + "_ConfirmDelSelString");
    CommGrid_OnClickConfirm(confirmDelSelString);
}

/*************************
      点击删除按钮后的事件
**************************/
function CommGrid_OnClickConfirm(confirmMsg)
{
	var linkButton = event.srcElement;
	if (linkButton.href != "")
	{
		if (confirmMsg != "")
		{
			if (!window.confirm(confirmMsg))
			{
				window.event.cancelBubble = true;
				window.event.returnValue = false;
			}
		}
	}
}

/*************************
      设定指定标记为灰色滤镜
**************************/
function CommGrid_SetGray(control)
{
    if (control != null)
    {
        control.style.filter = "progid:DXImageTransform.Microsoft.BasicImage(GrayScale=1)";
    }
}

//DataGrid Resize
function commGrid_InnerResize(commGrid_gridID)
{
    var divName = commGrid_gridID + "_divGrid";
    var divGrid = document.all(divName);
    var prevObj;
    var nextObj;
    var parentObj = divGrid.parentNode;
    //初始偏离量
    var offsetH = divGrid.offsetTop;
    //加上divGrid之上的高度
    while (parentObj != null)
    {
        prevObj = parentObj.previousSibling;
        while (prevObj != null)
        {
            if ((prevObj.offsetHeight != undefined) 
                && (prevObj.tagName != "TD")
                )
            {
                offsetH += prevObj.offsetHeight + 1;
            }
            prevObj = prevObj.previousSibling;
        }
        
        parentObj = parentObj.parentNode;
    }
    //加上divGrid之下的高度
    parentObj = divGrid;
    while (parentObj != null)
    {
        nextObj = parentObj.nextSibling;
        while (nextObj != null)
        {
            if((nextObj.offsetHeight != undefined) 
                && (nextObj.tagName != "SCRIPT")
                )
            {
                offsetH += nextObj.offsetHeight + 1;        
            }
            nextObj = nextObj.nextSibling;
        }
        
        parentObj = parentObj.parentNode;
    }
    //新高度
    var heightOffsetValue = eval(commGrid_gridID + "_HeightOffsetValue");
    var newHeight = document.body.clientHeight - offsetH - heightOffsetValue;
    //如果新高度低于最小允许高度则不变更高度
    var heightMin = eval(commGrid_gridID + "_GridMinScrollHeight");
    if (newHeight > heightMin)
    {
	    divGrid.style.height = newHeight;
    }
}

/*************************
  设定行中的选择框为选中状态
**************************/
function CommGrid_SetChecked(checkBoxID)
{
    var tableRow = event.srcElement.parentElement;
    var cellList = tableRow.childNodes;
    var nodeList;
    var i, j;
    
    for (i=0; i<cellList.length; i++)
    {
        nodeList = cellList(i).all.tags("INPUT");
        for (j=0; j<nodeList.length; j++)
        {
            if (nodeList(j).id == checkBoxID)
            {
                nodeList(j).click();
            }
        }
    }
}