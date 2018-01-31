## Visual Studio
------------------

#### operations
* 快捷键
	* `Ctrl`+`Enter` 在上面打开新一行
    * `Ctrl`+`Shift`+`Enter` 在下面打开新一行
    * `Ctrl`+`-`,`Ctrl`+`+`，切换前后两个编辑位置
    * `Ctrl+K, Ctrl+C` `Ctrl+K, Ctrl+U` 切换注释
    * `Ctrl+u, Ctrl+U` 切换选中文本大小写
    * `Ctrl+w` 选中单词
    * 当你在光标停留行使用快捷键`Ctrl+C`，`Ctrl+X`，`Ctrl+L`时，可以复制，剪切，删除整行内容。
* Visual Studio 2008禁用智能提示
	1. 打开目录C:\Program Files (x86)\Microsoft Visual Studio 9.0\VC\vcpackages 
	2. 重命名feacp.dll为feacp_disable.dll，或者删除之
	3. Visual Studio自带的智能提示总是会卡，而且会导致IDE经常死掉，还是禁用掉，用VA的
* Visual Studio 2008设置代码导航线
	1. 打开注册表，找到HKEY_CURRENT_USER\Software\Microsoft\VisualStudio\9.0\Text Editor
	2. 新建项Guides
	3. 写入值RGB(128,128,128) 4,75,80
	4. 其中，RGB表示颜色，4,75,80表示导航线的列位置，用逗号隔开，可以加入任意多条线
* Visual Studio 2008美观设置
	1. 字体Lucida Console或者Source Code Pro
	2. LineNumber，背景色淡绿色，字体深灰色
	3. VA，当前行高亮采用DottedBox
	4. 在Addin目录下安装插件[WordLight](http://code.google.com/p/wordlight/)，高亮文件内选中文本的匹配项
	5. VA或者其他插件设置右侧地图式滚动面板