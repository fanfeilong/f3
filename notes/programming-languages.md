## 基本概念

#### 作用域
- dynamic scoping
```
y=3                -- y in current scope is 3
function test(x)
  return function()
    return x+y
  end
end

local f = test(10) -- function(y) 10+y end
y=5                -- y in current scope is 5 
f()                -- result 5+10=15

y=6                -- y in current scope is 5 
f()                -- result 6+10=16
```

- lexical scoping
```
y=3                -- y in current scope is 3
function test(x)
  return function()
    return x+y     -- capture the y=3 value
  end
end

local f = test(10) -- function(y) 10+y end
y=5                -- y in current scope is 5 
f()                -- result 3+10=13, since `y` in test is 3, which is captured value

y=6                -- y in current scope is 5 
f()                -- result 3+10=13, since `y` in test is 3, which is captured value
```

dynamic scoping只是根据名字来判断当前符号在作用域中的值，在某些特殊场合下有用，但是对于结构化编程来说是不好的。
如果从绑定的角度来说，可以看成是dynamic scope只bind了变量的名字，而lexical scoping则bind了变量名字和当前的值。

#### 求值顺序
假设有函数：
```
function func(exp){
   return exp+exp;
}
```

func被调用的过程中，对参数的求值有三种可能：
- call by name: 
```
func(1+1)
=> return (1+1)+(1+1)  // 1 
=> return 2+(1+1)      // 2 
=> return 2+2          // 3
=> return 4            // 4
=> 4                   // 5
```

- call by value:
```
func(1+1)
=> func(2)             // 1    
=> return 2+2          // 2
=> return 4            // 3
=> 4                   // 4
```

- call by need:
```
func(1+1)
=> func(v)             // where v=1+1
=> return (v)+(v)      // 1, evaluate v=>1+1=2
=> return 2+2          // 2
=> return 4            // 3
=> 4                   // 4
```

#### 文法
- [python grammar](https://docs.python.org/3/reference/grammar.html)
- [lua grammar](https://www.lua.org/manual/5.3/manual.html#9)
  - lua的exp并不是stat，这和其他语言并不相同，例如python里有exp_stat

#### 跨语言调用、协程、闭包
跨语言交互，本质上就是A语言内部提供了一个让B语言注册函数给A的内部表的约定，B语言用这个规则把自己的函数注册给A，A在执行过程中遇到这个函数调用，发现不是自己语言写的，就去那个其他语言注册的函数表里找，找到后就调用。所以怎么注册，完全就是A说了算，A规定了要怎么传参数给B，B怎么返回参数给A。

协程就是一个线程里有一个调度器和两个函数，这个调度器一会儿去执行函数A，然后一会儿执行函数B，一会儿再去执行函数A。由于Lua的函数怎么执行是Lua的解释器说了算，Lua解释器里把函数执行过程中重要的数据、指令，都做成数组，执行到哪里只是偏移这些数据指令数组的位置而已，所以可以随时切换所谓的“协程”去执行。

理解闭包你要理解在Lua看来一个函数就是一个结构体，这个结构体可以把不属于自己作用域范围的外层变量拷贝一个引用，当外层作用域退出的时候，如果函数还是要用那个变量，就把那个变量拷贝一份就是了。

#### 关于特性的思考
* 语言特性并不是越多越好
* 并不是掌握语言特性多，或者知道细节多就牛逼，和牛逼没关系
* 只是如果要经常用，不得不做这些索引，以便快速查询，免得浪费人生，仅此而已


## 编程语言

#### scheme
---------------

##### 语言基础
* atom: null?, zero?, number?, quote?
* list: car,cdr,cons
* control: cond else, let, define
* s expression = atom or list
* lambda expression

##### 重要概念
* recursion
* tail recursion
* continuation
* call with current continuation
* continuation passing style (CPS)
* CPS transform
* lazy calculation
* collection
* closure
* coroutine
* lambda recursion
* Y combinator
* close variable, free variable

#### c
-----------

* struct 
  * aligin
  * tail zero array
  * function pointer members
* function
  * declare
  * function pointer
* number
* string
* macro
* typedef
* trait
* [sizeof](http://en.wikipedia.org/wiki/Sizeof)
* [typeof](http://en.wikipedia.org/wiki/Typeof)
* [offsetof,containof](http://en.wikipedia.org/wiki/Offsetof)
* [size and malloc](http://publications.gbdirect.co.uk/c_book/chapter5/sizeof_and_malloc.html)
* [right-left-rule](http://ieng9.ucsd.edu/~cs30x/rt_lt.rule.html)
* [Everything you need to know about pointers in C](http://boredzo.org/pointers/)

#### c++
-------------

* basic
  * const 
  * type cast
  * io
  * [Pointers](http://www.cplusplus.com/doc/tutorial/pointers/)
* class
  * default functions
  * function override
  * const
* template
  * function template
  * class template
  * specification
  * partial specification
  * concept lite
  * template template
  * variant template arguments
  * trait
* stl
  * functor
  * allocator
  * container
  * algorithm
  * iterator
  * adaptor
* c++ 98/03/11/14
* [More C++ Idioms](http://en.wikibooks.org/wiki/More_C%2B%2B_Idioms)

#### csharp
----------------

##### 核心概念
* lambda expressions, delegate, event, `Action<T>` and `Func<T>`
* anonymous types
* closure
* extension methods, extension attributes
* implicity typed local variable
* object and collection initializer
* query expressions
* expression tree
* linq to object,linq to xml, linq to sql, linq to anything
* [LINQ](http://code.msdn.microsoft.com/101-LINQ-Samples-3fb9811b)
* `Task<T>`, async/await
* `IEnumrable<T>`, `IComparable<T>`, `IQueyable<T>`, `IObserverable<T>`
* dynamic

#### Java
---------

##### Basic
* [Java SE document](https://docs.oracle.com/javase/8/docs/api/allclasses-noframe.html)
* [What is the Java equivalent of unsigned?](http://javamex.com/java_equivalents/unsigned.shtml)

#### Haskell
* [Haskell-Guid-zhcn](http://learnyouahaskell-zh-tw.csie.org/zh-cn/chapters.html)
* [Prelude.hs](http://www.cse.unsw.edu.au/~en1000/haskell/inbuilt.html#init)
* [Haskell operators](http://www.imada.sdu.dk/~rolf/Edu/DM22/F06/haskell-operatorer.pdf)

#### ErLang
- 轻量多进程是一种分布式开发的可行构架，强隔离性。
- 基于消息传递的分布式消息传递机制，一定程度上解决了对低粒度可见锁的依赖，但逻辑上的“锁”不属于他要解决的问题
- 多进程间的并发执行，单进程内的顺序执行。多进程设定在分布式机器上，带来伸缩性；单进程的顺序性带来开发的便利性。
- 基于监督树的速错机制（And和Or节点），多层次定义一个系统的错误粒度，如果干复杂的事出错，就干点简单的事
- 基于Behavior的模块化插件开发方式。将软件开发的不同抽象层分离出来，核心人员写Behavior，应用开发写具体的实现。
- 提出“乖函数”的概念。
- 利用尾递归技术，实现一定程度的热更新。

#### Go
读了rob pike的这篇[Less is exponentially more](https://commandcenter.blogspot.jp/2012/06/less-is-exponentially-more.html)，作者谈了Go的设计初衷和理念。还分析了为什么一开始使用Go的从Python、Ruby说社区来的多，而从C++来的少。两种不同的设计理念。
- regular syntax (don't need a symbol table to parse)
- garbage collection (only)
- no header files
- explicit dependencies
- no circular dependencies
- constants are just numbers
- int and int32 are distinct types
- letter case sets visibility
- methods for any type (no classes)
- no subtype inheritance (no subclasses)
- package-level initialization and well-defined order of initialization
- files compiled together in a package
- package-level globals presented in any order
- no arithmetic conversions (constants help)
- interfaces are implicit (no "implements" declaration)
- embedding (no promotion to superclass)
- methods are declared as functions (no special location)
- methods are just functions
- interfaces are just methods (no data)
- methods match by name only (not by type)
- no constructors or destructors
- postincrement and postdecrement are statements, not expressions
- no preincrement or predecrement
- assignment is not an expression
- evaluation order defined in assignment, function call (no "sequence point")
- no pointer arithmetic
- memory is always zeroed
- legal to take address of local variable
- no "this" in methods
- segmented stacks
- no const or other type annotations
- no templates
- no exceptions
- builtin string, slice, map
- array bounds checking

## 参考资料
-------------
- [A list of programming languages that are actively developed on GitHub](https://github.com/showcases/programming-languages)
- [Monads in C ](http://blog.sigfpe.com/2007/03/monads-in-c-pt-iii.html)
- [Monads in C++](http://bartoszmilewski.com/2011/07/11/monads-in-c/)
- [Introduction to Category Theory](http://www.cs.nott.ac.uk/~gmh/cat.html)
- [Monoids, Functors, Applicatives, and Monads: 10 Main Ideas](http://monadmadness.wordpress.com/2015/01/02/monoids-functors-applicatives-and-monads-10-main-ideas/)
- [a page about call/cc](http://www.madore.org/~david/computers/callcc.html)
- [stackoverflow:what is call/cc](http://stackoverflow.com/questions/612761/what-is-call-cc)
- [douban:call/cc](http://www.douban.com/note/66859771/)
- [wesdyer:cps.net](http://blogs.msdn.com/b/wesdyer/archive/2007/12/22/continuation-passing-style.aspx)
- [schemewiki:call/cc](http://community.schemewiki.org/?call-with-current-continuation)
- [Introduction to Y Combinator](http://mvanier.livejournal.com/2897.html)
- [A Description of the C++ typename keyword](http://pages.cs.wisc.edu/~driscoll/typename.html)
- [Curiously recurring template pattern(CRTP)](http://en.wikipedia.org/wiki/Curiously_recurring_template_pattern)
- [Resource Acquisition Is Initialization (RAII)](http://en.wikibooks.org/wiki/C%2B%2B_Programming/RAII)
- [Attach by Initialization](http://en.wikibooks.org/wiki/More_C%2B%2B_Idioms/Attach_by_Initialization)
- [Attorney-Client](http://en.wikibooks.org/wiki/More_C%2B%2B_Idioms/Friendship_and_the_Attorney-Client)
- [Base-from-Member](http://en.wikibooks.org/wiki/More_C%2B%2B_Idioms/Base-from-Member)
- [Substitution Failure Is Not An Error(SFINAE](http://en.wikibooks.org/wiki/More_C%2B%2B_Idioms/SFINAE)
- [Non Virtual Interface(NVI)](http://en.wikibooks.org/wiki/More_C%2B%2B_Idioms/Non-Virtual_Interface)
- [How to write an interpreter(yin wang)](http://www.yinwang.org/blog-cn/2012/08/01/interpreter/)
- [Bad Engineering Properties of Object-Oriented Languages](http://doc.cat-v.org/programming/bad_properties_of_OO)

## 趣味编程
- [编程语言伪简史](http://www.ithome.com/html/it/46432.htm) [A brief incomplete and mostly wrong history of programming languages](http://james-iry.blogspot.co.at/2009/05/brief-incomplete-and-mostly-wrong.html)