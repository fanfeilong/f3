### 编程语言笔记

#### debug
* [Why debugging is all about understanding](http://futurice.com/blog/why-debugging-is-all-about-understanding)

#### scheme笔记
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

#### c 语言
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

#### c++ 语言
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

#### csharp 笔记
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

#### Program Practice
- Values
  - Communication(Readable)
  - Simplicity
  - Flexibility 
- Principles
  - Local Consquences
  - Minimize Repetition
  - Logic and Data Together
  - Symmetry
  - Declarative Expression
  - Rate of Change
- Cost of software
  - Cost of Develop
  - Cose of Maintain
    - Cost of Understand
    - Cost of Change
    - Cost of Test
    - Cost of Deploy

#### Monad
----------

##### 描述
----------------
>In pure functional programming languages you aren't allowed to cause side effects. The only way a function can 'interact' with the outside world is to return something. So if you have a function that needs to return a number, but has a certain side effect, the only way to do this is to return the number and return some data 'containing' the side effect. But that's a pain in the ass - you have to write all of your code to return multiple values, and you have to write plumbing code to pass these side-effect containing values around. What monads can do is give a uniform API to side-effect containing data so you can just concentrate on the number that you want to return, and have the side-effect carried along automatically in the background. What's neat about monads is the same API can be used for many different problems, not just handling side-effects.

##### 探讨
---------
基于集合论的结构：群、环、域、模、拓扑空间、算子、单子等。唯一目的就是寻找这样的集合，其元素在给定几条公理化操作下，保持某些不变的性质。这样的集合结构上可以建立公理、推导引理、推导定理，使得只要是这种结构的集合，就可以使用这些公理、引理、定理证明其拥有怎样的性质和结论。

单子，正是这样一种结构，只是其刚好被这帮搞lisp的人用在了以函数为元素的集合上，并且这样的一种结构刚好能用解决函数级别的抽象封装问题。想清楚这点，就不至于陷入细节而不知其所以然。

#### 关于特性的思考
* 语言特性并不是越多越好
* 并不是掌握语言特性多，或者知道细节多就牛逼，和牛逼没关系
* 只是如果要经常用，不得不做这些索引，以便快速查询，免得浪费人生，仅此而已

#### C++98的问题
C++11虽然出来了，但国内很多公司都不允许使用，从实际情况来看，似乎能掌握好C++11标准的新手程序员也是参差不齐。只用C++98的问题在于，需要自己关心的细节太多。最显著的是静态函数和成员函数之间的不能自如切换，其次是闭包的缺失，智能指针就更不说了。很多人说用boost库，boost库那一坨代码看着就想吐，根本没有让人用到欲望，还不如手工做一些简洁的封装。

>关于私有继承的第一个规则正如你现在所看到的：和公有继承相反，如果两个类之间的继承关系为私有，编译器一般不会将派生类对象（如Student）转换成基类对象（如Person）。这就是上面的代码中为对象s调用dance会失败的原因。第二个规则是，从私有基类继承而来的成员都成为了派生类的私有成员，即使它们在>基类中是保护或公有成员。行为特征就这些。

>这为我们引出了私有继承的含义：私有继承意味着 "用...来实现"。如果使类D私有继承于类B，这样做是因为你想利用类B中已经存在的某些代码，而不是因为类型B的对象和类型D的对象之间有什么概念上的关系。因而，私有继承纯粹是一种实现技术。

**评价**
私有继承简直是鼓励错误的代码设计，废品。

#### 常量以k开头命名的原因
>"c" was the tag for type "char", so it couldn't also be used for "const"; so "k" was chosen, since that's the first letter of "konstant" in German, and is widely used for constants in mathematics.

>http://stackoverflow.com/questions/5016622/where-does-the-k-prefix-for-constants-come-from

>It's a historical oddity, still common practice among teams who like to blindly apply coding standards that they don't understand.

>Long ago, most commercial programming languages were weakly typed; automatic type checking, which we take for granted now, was still mostly an academic topic. This meant that is was easy to write code with category errors; it would compile and run, but go wrong in ways that were hard to diagnose. To reduce these errors, a chap called Simonyi suggested that you begin each variable name with a tag to indicate its (conceptual) type, making it easier to spot when they were misused. Since he was Hungarian, the practise became known as "Hungarian notation".

>Some time later, as typed languages (particularly C) became more popular, some idiots heard that this was a good idea, but didn't understand its purpose. They proposed adding redundant tags to each variable, to indicate its declared type. The only use for them is to make it easier to check the type of a variable; unless someone has changed the type and forgotten to update the type, in which case they are actively harmful.

>The second (useless) form was easier to describe and enforce, so it was blindly adopted by many, many teams; decades later, you still see it used, and even advocated, from time to time.

>"c" was the tag for type "char", so it couldn't also be used for "const"; so "k" was chosen, since that's the first letter of "konstant" in German, and is widely used for constants in mathematics.

#### 匈牙利命名法
- [匈牙利命名法wik](http://zh.wikipedia.org/zh/%E5%8C%88%E7%89%99%E5%88%A9%E5%91%BD%E5%90%8D%E6%B3%95)
- [应用型匈牙利命名法 vs 系统型匈牙利命名法](http://www.cnblogs.com/xuxn/archive/2012/05/16/real-hungarian-notation.html)

**评价:**
- 匈牙利命名法这种编码方式并不见得就带来更好的阅读信息。以Google的Chromium为例，其命名风格就完全抛弃了匈牙利命名法。至今还在用匈牙利命名法风格的，都是品味和惯性问题。实际上很多工程上的老古董都是不思上进的结果，那些被这些老古董虐待过的，并以此为荣，觉得懂这些老古董才是高级货的，都是没见过世面的，以为非此路不可。可惜了，思维的局限限制了进步。

#### C的数组下标是指针偏移的缩写
[With C arrays, why is it the case that a[5] == 5[a] ?](http://stackoverflow.com/questions/381542/with-c-arrays-why-is-it-the-case-that-a5-5a)
The C standard defines the [] operator as follows:
`a[b] == *(a + b)`
Therefore a[5] will evaluate to:
`*(a + 5)`
and 5[a] will evaluate to
`*(5 + a)`
and from elementary school math we know those are equal.

This is the direct artifact of arrays behaving as pointers, "a" is a memory address. "a[5]" is the value that's 5 elements further from "a". The address of this element is "a + 5". This is equal to offset "a" from "5" elements at the beginning of the address space (5 + a).

**评价**：
指针计算是最基本的计算，其他的各种操作都是建立在指针计算的基础上重新定义的计算，这样就有了可推导的性质。

#### 位操作
[How do you set, clear and toggle a single bit in C/C++?](http://stackoverflow.com/questions/47981/how-do-you-set-clear-and-toggle-a-single-bit-in-c-c)
```
& 
0 0 0
1 0 0
0 1 0
1 1 1
```

```
|
0 0 0
0 1 1
1 0 1
1 1 1
```

```
^
0 0 0
0 1 1
1 0 1
1 1 0
```

=>

Setting a bit:
`number|=(1<<n)`

Clearing a bit:
`number&=~(1<<n)`

Toggling a bit:
`number^=1<<n`

Checking a bit:
`bit=number&(1<<n)`

**评价:**
高级编程语言完全可以把对位的操作封装成普通函数操作，比如
```
bit.set(number,i)
bit.clear(number,i)
bit.toggle(number,i)
bit.get(number,i)
```
然后通过编译器或者解释器的优化来达到同样的性能。这样就可以去掉这样的语法噪音
发明`&`,`|`,`^`完全是增加语法噪音，估计是被数学的`符号发明症`所影响，问题是数学的符号一般
都只是用来做逻辑上的推理和解释，并不需要被天天写和运行。

- [Low Level Bit Hacks You Absolutely Must Know](http://www.catonmat.net/blog/low-level-bit-hacks-you-absolutely-must-know/)
- [Bit Twiddling Hacks](http://graphics.stanford.edu/~seander/bithacks.html)

#### 多个提前返回时的资源释放
```
xxx x = new xxx();
bool quit=false;
do{
  if(...){
    quit=true;
    break;  
  }

  if(...){
    quit=true;
    break;  
  }
}while(false);
if(quit){
  delete x;
  return;
}
//other code
```

#### 转轴
很多时候，一个代码有好多个分支，写代码的时候需要使用`if else if esle if else`之类的繁琐的分支语句，我们可以通过`Enum+Translate+SwitchCase`的方式让代码更扁平化点，我称之为`转轴`。
```
typedef enum tagXXXState{
  XXXState_1,
  XXXState_2
}XXXState;
XXXState TranslateState(condition){
  if(condition){
    return XXXState_1;
  }else{
    return XXXState_2;
  }
}
void Func(condition){
  switch(TranslateState(condition)){
    case XXXState_1:break;
    case XXXState_2:break;
    default:break;
  }
}
```
此处实例的分支只有2个，也没有嵌套，自然是看上去多余的，不过如果分支很多，又有很多个嵌套，则使用这种`转轴`代码会把嵌套的if else转换成扁平的switch-case代码。

#### 明确加锁范围
加锁的范围要明确，利用C++的RAII，使用自动锁，同时最小化局部范围
```
{ 
  AutoThreadLock lock(m_lock); 
  //Data
}
```


#### Lock & Reentry
- Lock
  - Lock, check threadid, enable or disable reentry
  - Unlock, check threadid, enable or disable Reentry

#### StateMachine
- define states
- function `dowork` by switch states
- in branch methods or events
    - check state
    - do work
    - change state
    - reentry `dowork` function



#### 调度状态机

假设有如下队列
```
function Queue:Push(arg) end
function Queue:Pop() end
function Queue:Size() end
```

假设有一个异步消费队列的调用，我们可以做异步链式调用：
```
local queue = ...
function shedule()
  local item = queue:Pop()
  if item then
    dosomethinig_async(item,function()
      --// 在回调里异步调度下一个
      async(function()
        shedule() 
      end)
    end)
  end
end
```

但我们也可以用状态机调度
```
local queue = ...
function shedule()
  --// 定时刷帧，如果窗口不为0，就批量调度一部分item
  start_timer(function()
    local cc = congestion_controller
    if cc:current_window()>0 then
      while cc.dec_winidow() do
        local item = queue:Pop()
        --// 回调里只要更新cc状态即可
        dosomethinig_async(item,function() 
          cc.update() 
        end)
      end
    end
  end,1000)
end
```

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

#### 参考资料
-------------
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