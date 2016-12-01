
/**************************************
1. 打开博客园后台
2. 点击`设置`面板
3. 找到`页首Html代码`编辑框
4. 添加下面的JavaScript脚本
 **************************************/
<script>
var tables = document.querySelectorAll("table");
for(var i=0;i<tables.length;i++){
	var table = tables[i];
	var thead = table.querySelector("thead");
	if(thead!=void 0){
		var tlh = thead.querySelector("tr");
		var ths = tlh.querySelectorAll("th");

		var tbody = table.querySelector("tbody");
		var tsh = document.createElement("tr");
		tsh.setAttribute("class","tshead");
		
		tbody.insertBefore(tsh,tbody.firstChild);

		for(var j=0;j<ths.length;j++){
			var td = document.createElement("td");
			td.setAttribute("align", "left");
			td.setAttribute("style", "font-weight:bold");
			td.innerHTML = ths[j].innerHTML;
			tsh.appendChild(td);
		}

		tlh.setAttribute("style", "display:none");
	}
}
</script>
