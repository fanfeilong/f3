[译注]为什么大多数编程语言里，= 号被用来做赋值操作符？

equals-as-assignment: https://www.hillelwayne.com/post/equals-as-assignment/ 
在这篇文章里，作者对这个问题做了一下考古。这些语言里面，初始化、赋值、相等三种操作符的变迁。

这个内容很有意思，文章中提到的编程语言以及编程语言的发明者线索众多，我很感兴趣。以下做一个概要翻译笔记，同时把涉及到的这些编程语言和编程语言发明者的维基百科中英文链接索引出来。作为一个有趣的索引，方便日后查询。

<hr/>

60年代最早的四大语言里，这三种动作的符号分别如下：

FORTRAN II: `=` `=` `.EQ.`
COBOL: `INITIALIZE` `MOVE` `EQUAL` 
ALGOL-60: `N/A` `:=` `=` 
LISP: `let` `set` `equal`

ALGOL-60由于语言里并不原生支持IO，导致其虽然在编程语言的历史上影响很大，但可能也是最不实用的语言，并且，它并没有特别的符号表示初始化动作，而是通过类型+变量名（interger x）的方式定义变量，然后再赋值。

但是由于ALGOL-60语言很强大（注：从今天的角度来看可能很难体会到底强大在哪里，光理解古老的语法都看上去很麻烦），于是1963年Christopher Strachey（University of Cambridge，早期语言不是从实验室就是学校出来的感觉）在ALGOL上加了一堆特性后，捣鼓出了CPL语言，把初始化变量的方法改成了`integer x = 5`这种方式。

CPL里有三种初始化：`=` 表示初始化为一个值，`≃` 表示初始化为一个引用，如果`x≃y`,那么y的改动，x也跟着被改动。但是`x≃y+1`会导致程序崩溃（囧） ，另外还有一个「总是替换」的初始化`≡`，每次解释的时候，都会符号替换，不过不知道如果`x≡x`,会发生什么奇怪的事情（囧）。CPL里面初始化和相等都是用`=`号，但是在初始化之前有类型修饰，所以并不会产生二义性。

一年之后，Kenneth E. Iverson 发明了APL语言，这个语言使用`←`来表示赋值。显然，键盘上并没有对应的按钮(囧)。 APL的后继编程语言J，J语言使用了`=:`来表示赋值。APL语言深度影响了S语言（一个统计方面的语言）,S语言又深度影响了现代统计语言R，R语言里使用了`<-`来表示赋值操作符。

回头说CPL语言，CPL有一个主要的问题是没人能实现它，一些人实现了这个语言的部分特性，但是整个语言的设计对于编译器开发者来说太不友好。于是，Martin Richards 裁剪掉了很多不必要的复杂特性，创建了BCPL语言，这是在1967年。而，第一个CPL语言的编译器在1970年才出来。（注：我想起了如今的C++语言特性，已经膨胀到很复杂的地步了，在计算机世界，总是存在CSIC/RISC两种流派，在我从事计算机工作后，许多高明的程序员都跟我说过同样的一句话：“做加法容易，做减法难，能做减法才是更高明的”。）

对于初始化，Richards认为CPL的三种不同的初始化语义中的`≡`可以被普通函数替换，并且和赋值是一样的，因此他统一用`=`号表示初始化。而全局内存地址的命名则是一个例外，使用`:`号。对于相等比较，继续使用`=`号。对于重新赋值，则和CPL、AGOL一样，继续使用`:=`。

此后，很多后继的编程语言使用`:=`做初始化，使用`:=`表示重新赋值，使用`=`比较相等。这个风格一直到Niklaus Wirth发明了Pascal，这是为什么我们把这种风格叫做“Pascal style”。

作者认为BCPL也可以被看作第一个弱类型语言，唯一的数据类型是`word`，者是的编译器具有更好的可移植性，当然同时伴随着更多的逻辑错误。而Richards希望通过代码的改进、有意义的描述和命名可以减少这类错误。BCPL同时引入了`{`和`}`作为块（`block`）的定义。

Ken Thompson想要在PDP-7上使用BCPL。但是有一个问题，BCPL编译器虽然已经算“小”，仍然比PDP-7的最小工作内存(16kb-4kb)大4倍左右。因此，Thompson需要自己创建一个新的，更小的编程语言（注：挠自己的痒处，重新发明轮子无论在人类的演化而言，还是对于计算机世界短短的发展历史而言，都是一个内在的推动力，甚至有许多人认为学习技术和工具，最好的方式就是重新发明轮子。实际上，重新发明的轮子往往和原轮子之间有变动，类似于基因突变，而我们知道突变，从种群的角度来说，是有益的，同时也意味着大部分的突变可能都没什么用，甚至有害，但突变丰富了多样性，增加了种群在优胜劣汰过程中的生存能力）。Thompson 也希望减少源代码的体积，于是引入了`++`， `--`这样的自增自减操作符，最后他发明了B语言。

回到正题，BCPL总是使用`=`号做初始化，使用`:=`做重新赋值，Thompson决定把他们合并，Thompson选择使用`=`号统一表示初始化和赋值，你猜为什么？当然还是因为`=`少写了一个`:`，更小，更有利于语言源代码体积的减少（注：这算一个特定场合的特定需求）。不过，这就会导致`x=y`是比较还是赋值动作产生了二义性。于是Thompson决定用`==`来表示相等判断。Thompson写到：

>Since assignment is about twice as frequent as equality testing in typical programs, it’s appropriate that the operator be half as long.

从编码的角度来说，因为赋值的频率是相等的频率的2倍左右，则让赋值操作符的长度是相等操作符的一半，这符合编码的原理。

在Dennis Ritchie加入后，Thompson于1969年发布了B语言的第一个版本。在这个过程中Ole Dahl 和 Kristen Nygaard则发明了第一个OOP编程语言Simula 67。Simula继续采用ALGOL的策略，严格区分初始化和重新两个步骤。与此同时，Alan Kay开始折腾smalltalk编程语言，smalltalk也添加了块的概念，并且采用了相同的语法（注：是指和Simula相同的语法）。于是，直到1971年，大部分编程语言使用`:=`来表示赋值。

直到，从B语言演化出来的C语言里，继续保持了B语言的设计：初始化和赋值使用`=`操作符，相等使用`==`操作符。（注：此后，C系列的语言基本上一路保留这种风格，直到今天JavaScript语言由于undefined的问题，引入了一个`===`的全等操作...）

最后，一年之后(1973)，Robin Milner发明了ML语言。作者认为ML是都一个强调纯函数(pure function)和不可变(no mutation, inmutable)的函数式语言。但ML语言还是有一个后门，可以使用`:=`重新赋值。从1980年开始，一些面向正确(注：correctness-oriented，不知道怎么翻译)的命令式语言都是用`:=`表示赋值，特别是Eiffel（注：强调契约式设计，Design by Contract）和Ada语言（注：我们都知道第一个程序员是诗人拜伦的女儿Ada，她写出的程序在纸上，没跑过）。

最后的最后，`=`号从来就不是赋值操作符的`天然/自然的选择(the natual choice)`。ALGOL系列的编程语言里大部分采用的是`:=`做为赋值操作符，这大概是因为`=`号在数学上更`自然`地和相等绑定在一起。而今天，大多数语言选择使用`=`号做赋值操作符，则是从C语言继承而来的。于是我们可以从C语言追溯到CPL等一系列`古老`的编程语言。

作者不知道这篇文章应该如何归类，作者就是喜欢软件的历史。

<hr/>

[1] https://en.wikipedia.org/wiki/Fortran
[2] https://zh.wikipedia.org/wiki/Fortran
[3] https://en.wikipedia.org/wiki/COBOL
[4] https://zh.wikipedia.org/wiki/COBOL
[5] https://en.wikipedia.org/wiki/ALGOL
[6] https://zh.wikipedia.org/wiki/ALGOL
[7] https://en.wikipedia.org/wiki/Lisp_(programming_language)
[8] https://zh.wikipedia.org/wiki/LISP
[9] https://en.wikipedia.org/wiki/Christopher_Strachey
[10] https://zh.wikipedia.org/wiki/克里斯托弗·斯特雷奇
[11] https://en.wikipedia.org/wiki/Kenneth_E._Iverson
[12] https://zh.wikipedia.org/wiki/肯尼斯·艾佛森
[13] https://en.wikipedia.org/wiki/J_(programming_language)
[14] https://zh.wikipedia.org/wiki/J语言
[15] https://en.wikipedia.org/wiki/S_%28programming_language%29
[16] https://zh.wikipedia.org/wiki/S語言
[17] https://en.wikipedia.org/wiki/R_(programming_language)
[18] https://zh.wikipedia.org/wiki/R语言
[19] https://en.wikipedia.org/wiki/Martin_Richards_(computer_scientist)
[20] https://zh.wikipedia.org/wiki/馬丁·理察德
[21] https://en.wikipedia.org/wiki/BCPL
[22] https://zh.wikipedia.org/wiki/BCPL
[23] https://en.wikipedia.org/wiki/Pascal_(programming_language)
[24] https://zh.wikipedia.org/wiki/Pascal_(程式語言)
[25] https://en.wikipedia.org/wiki/Niklaus_Wirth
[26] https://zh.wikipedia.org/wiki/尼克劳斯·维尔特
[27] https://en.wikipedia.org/wiki/Ken_Thompson
[28] https://zh.wikipedia.org/wiki/肯·汤普逊
[29] https://en.wikipedia.org/wiki/Dennis_Ritchie
[30] https://zh.wikipedia.org/wiki/丹尼斯·里奇
[31] https://en.wikipedia.org/wiki/Ole-Johan_Dahl
[32] https://zh.wikipedia.org/wiki/奧利-約翰·達爾
[33] https://en.wikipedia.org/wiki/Kristen_Nygaard
[34] https://zh.wikipedia.org/wiki/克利斯登·奈加特
[35] https://en.wikipedia.org/wiki/Simula
[36] https://zh.wikipedia.org/wiki/Simula
[37] https://en.wikipedia.org/wiki/Alan_Kay
[38] https://zh.wikipedia.org/wiki/艾伦·凯
[39] https://en.wikipedia.org/wiki/Smalltalk
[40] https://zh.wikipedia.org/wiki/Smalltalk
[41] https://en.wikipedia.org/wiki/C_(programming_language)
[42] https://zh.wikipedia.org/wiki/C语言
[43] https://en.wikipedia.org/wiki/JavaScript
[44] https://zh.wikipedia.org/wiki/JavaScript
[45] https://en.wikipedia.org/wiki/ML_(programming_language)
[46] https://zh.wikipedia.org/wiki/ML语言
[47] https://en.wikipedia.org/wiki/Robin_Milner
[48] https://zh.wikipedia.org/wiki/罗宾·米尔纳
[49] https://en.wikipedia.org/wiki/Eiffel_(programming_language)
[50] https://zh.wikipedia.org/wiki/Eiffel
[51] https://en.wikipedia.org/wiki/Bertrand_Meyer
[52] https://zh.wikipedia.org/wiki/伯特蘭·邁耶
[53] https://en.wikipedia.org/wiki/Ada_(programming_language)
[54] https://zh.wikipedia.org/wiki/Ada
