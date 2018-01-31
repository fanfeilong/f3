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