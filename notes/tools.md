## Notepad
---------------------

#### operations
* 打开：Wine+r: notepad
* 选择一整行：
    * Home+Shilft+End
    * End+Shift+Home
* 查找：Ctrl+f
* 替换：Ctrl+h

## VIM 操作最佳实践
---------------------
#### operations
* 字母、数字、控制键
    * `h`，`j`，`k`，`l`四个方向键：`h`向左，`j`向下，`k`向上，`l`向右，这四个键让手不离盲打区域。
    * `gg`和`G`：`gg`是回到文本文件的第一行，`G`是定位到文本文件的最后一行。
    * gD 跳转到局部变量的定义处
    * %  跳转到配对的括号上
    * [[ 跳转到代码块的开头去
    * '' 跳转到光标上次停靠的地方
    * mx 设置书签
    * `x 跳转到书签处
    * `w`是跳到下一个单词的开头，`W`是跳到以空格分隔的下一个单词的开头，`w`表示word。
    * `e` 是跳到下一个单词的结尾，`E`是跳到以空格分隔的下一个单词的结尾，`e`表示end。
    * `b`是跳到前一个单词的开头，`B`是跳到以空格分隔的前一个单词的开头，`b`表示back。
    * `Ctrl+f`是向前翻页，`Ctrl+b`是向后翻页，`f`表示forward，`b`表示backward。
    * `Ctrl+u`是向前翻半页，`Ctrl+d`是向下翻半页，`u`表示up，`d`表示down。
    * `f+字母`：表示向前定位到第一个找到的字母，比如fa表示向前定位到第一个字母a。
    * `F+字母`：表示向后定位到第一个找到的字母。
    * `n+f+字母`：表示向前定位到第n个找到的字母。
    * `n+F+字母`：表示向后定位到第n个找到的字母。
    * 字符向前和向后定位都可以按分号`;`定位下一个，按逗号`,`定位前一个。
    * `^`跳到行首，`$`跳到行末，`0`跳到行首第一个非空字符。
      如果发生折行，上述与行有关的命令可以加上g前缀用于操作屏幕上的行。
    * `o`,`O`：向前向后新建编辑行
    * `n>`; 增加n个tab缩进
    * `n<`; 减少n个tab缩进
    * :!start{commmand} 异步调用外部命令

* 单词
    * `xp`: cut a character and paste after current 交换字符
    * `caw`: change a word 修改一个单词
    * `d3k`: delete 3 lines upwards 向上删除3行，同理`d3j`向下删除3行
    * `ddp`: delete a line and paste after current line 交换两行
    * `zt`将当前行置顶，`zb`将当前行置底，`zz`将当前行居中，`t`表示top，`b`表示bottom，而前缀`z`表示zoom。
      最好用的翻页组合：按`zz`将当前行居中，然后按`ctrl+u`或者`ctrl+d`半屏翻页。

* 句子
    * `V`选中当前行,进入v模；按`j`或`k`多行选择；`>`或`<`整体缩进；
    
* 正则表达式
	* [Vim Reqular Expressions 101](http://www.vimregex.com/)
	* 单文件替换
	    * %s/pattern/replace/gc
	    * 5,6s/pattern/replace/gc
  * 查找，`/keyword`
      * 下一个:n
      * 上一次:shift+n	
* 多文件
    * :vimgrep /searchpattern/ [g][j] filepattern
    * :[vim多文件替换](http://usevim.com/2012/04/06/search-and-replace-files/)
        * arg
        * arg *.h
        * arglist
        * argdo
        * argdo %s/pattern/replace/ge | update

#### references
  - [Vim Regular Expressions 101](http://vimregex.com/)
  - [VIM IDE STEP BY STEP](http://blog.csdn.net/wooin/article/details/1858917)
  - [coming-home-to-vim](http://stevelosh.com/blog/2010/09/coming-home-to-vim/)
  - [VIM 练级攻略](http://coolshell.cn/articles/5426.html)
  - [VimRepress 用VIM写WordPress博客](http://www.vim.org/scripts/script.php?script_id=3510)
  - [豆瓣HJKL小组](http://www.douban.com/group/hjkl/)
  - [初学者简易.vimrc编写指南](http://edyfox.codecarver.org/html/_vimrc_for_beginners.html)
  - [vim color schemes](http://code.google.com/p/vimcolorschemetest/)
  - [gvim+ctags in windows](http://hi.baidu.com/zengzhaonong/item/e6eb514af9b16cacde2a9f67)
  - [gvim+cscope in windows](http://hi.baidu.com/zengzhaonong/item/69d9dc325170d4c01a969667)
  - [What is your most productive shortcut with Vim?](http://stackoverflow.com/questions/1218390/what-is-your-most-productive-shortcut-with-vim/1220118#1220118)

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
    
## Chrome
-----------

#### operations
* 快捷键
    * `Ctrl`+`Shift`+`n`：新建隐身窗口
    
## Windows文件管理器
----------------------

#### operations
* `Win`+`e`：打开新文件夹
* `Win`+`r`：运行
* `Win`+`l`：锁定屏幕
* `Win`+`i`：Windows8右侧栏
* 安装基于Chromium开发的Tab式文件管理器[Clover](http://cn.ejie.me/)

## SVN
--------

#### operations
* 如果本地进行了一堆复杂操作后，发现提交的时候很多missing之类的冲突，最简单的办法是：
	1. 将当前文件夹的文件和子文件夹都`cut`到某个空目录x
	2. 对当前文件夹做`svn revert`
	3. 将x文件夹下的备份内容`copy`回当前文件夹
	4. 重新执行`svn commit`

## GIT
--------
#### git log
```
git config --global alias.lm  "log --no-merges --color --date=format:'%Y-%m-%d %H:%M:%S' --author='fanfeilong' --pretty=format:'%Cred%h%Creset -%C(yellow)%d%Cblue %s %Cgreen(%cd) %C(bold blue)<%an>%Creset' --abbrev-commit"

git config --global alias.lms  "log --no-merges --color --stat --date=format:'%Y-%m-%d %H:%M:%S' --author='fanfeilong' --pretty=format:'%Cred%h%Creset -%C(yellow)%d%Cblue %s %Cgreen(%cd) %C(bold blue)<%an>%Creset' --abbrev-commit"

git config --global alias.ls "log --no-merges --color --graph --date=format:'%Y-%m-%d %H:%M:%S' --pretty=format:'%Cred%h%Creset -%C(yellow)%d%Cblue %s %Cgreen(%cd) %C(bold blue)<%an>%Creset' --abbrev-commit"

git config --global alias.lss "log --no-merges --color --stat --graph --date=format:'%Y-%m-%d %H:%M:%S' --pretty=format:'%Cred%h%Creset -%C(yellow)%d%Cblue %s %Cgreen(%cd) %C(bold blue)<%an>%Creset' --abbrev-commit"
```

#### references
- [Commit Often, Perfect Later, Publish Once: Git Best Practices](https://sethrobertson.github.io/GitBestPractices/)
- [Understanding the GitHub Flow](https://guides.github.com/introduction/flow/)
- [A successful Git branching model](http://nvie.com/posts/a-successful-git-branching-model/)
- [How to Write a Git Commit Message](http://chris.beams.io/posts/git-commit/)
- [Keeping Commit Histories Clean](https://www.reviewboard.org/docs/codebase/dev/git/clean-commits/)
- [What's the best visual merge tool for Git?](http://stackoverflow.com/questions/137102/whats-the-best-visual-merge-tool-for-git)
- [visual diff](http://meldmerge.org/)
- [the core conception of git](https://lufficc.com/blog/the-core-conception-of-git)

## IM
-------

#### operations
* 对于QQ群，如果不想浪费时间的话，一定要把人多水多的群设置为「只显示消息数目，不弹出消息」，切记切记

## DOT & Graphviz
-------------------

#### operations

#### references
- [wiki:Graphviz](http://en.wikipedia.org/wiki/Graphviz)
- [wiki:DOT language](http://en.wikipedia.org/wiki/DOT_(graph_description_language))
- [Drawing Graphs using Dot and Graphviz](http://www.tonyballantyne.com/graphs.html)

## LaTeX
----------

#### operations

#### references
- [TeX example](http://www.texample.net/)
- [TeX wikibook](http://www.wikibooks.org//wiki/LaTeX)
- [TeX stackxchange](http://tex.stackexchange.com/)
- [CTeX](http://bbs.ctex.org/)
- [LaTeX editor](http://zzg34b.w3.c361.com/)
- [Introduce to TeX by 王垠](http://docs.huihoo.com/homepage/shredderyin/tex_frame.html)
- [LaTeX wiki](http://zh.wikipedia.org/zh-cn/LaTeX)
- [Using Import graphics in LaTeX 2e](http://www.ctex.org/documents/latex/graphics/)
- [PracTeX](http://tug.org/pracjourn/archive.html)
- [LaTeX comminity](http://www.latex-community.org/)
- [LaTeX Templates](http://www.latextemplates.com/)
- [TeX tips](http://www.kronto.org/thesis/tips/index.html)
- [A Beamer Quickstart](http://www.math.umbc.edu/~rouben/beamer/quickstart-Z-H-6.html)
- [Introduction to Beamer](http://www.math-linux.com/spip.php?article77)
- [在 MS Windows 裝 TeX Live 2011](http://exciton.eo.yzu.edu.tw/~lab/latex/install_latex_cjk_ms_windows.html)
- [最简单的 TeXLive CD 安装指南](http://www.math.zju.edu.cn/ligangliu/latexforum/tex_setup.htm)
- [BibTeX Format](http://www.bibtex.org/Format/)
- [TeXMacs (What you see is what you get)](http://www.texmacs.org/tmweb/home/welcome.en.html)
- [Visual Debugging in TEX](http://www.pragma-ade.com/articles/art-visi.pdf)

## MarkDown
----------

#### operations

#### references
- [MarkDown Math in Visual Studio Code](https://github.com/goessner/mdmath)

## UML
----------

#### operations

#### references
- [js sequence diagrams](https://bramp.github.io/js-sequence-diagrams/)
