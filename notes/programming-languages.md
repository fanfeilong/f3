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


#### Y Combinator 简析
----------------------

#####什么是Combinator
1. 首先必须是个lambda表达式
2. 其次所有的变量都必须是lambda表达式的参数，也就是没有自由变量

**例子**

    (lambda (x) x) //是
    (lambda (x) y) //不是，有自由变量y
    (lambda (x) (lambda (y) x)) //是
    (lambda (x) (lambda (y) (x y))) //是
    (x (lambda (y) y)) //不是，非lambda表达式
    ((lambda (x) x) y) //不是，非lambda表达式

#####如何用lambda做递归
1. 传入自己(待递归的lambda表达式)的拷贝。
2. 延迟（传入自己拷贝后的lambda表达式在调用自己拷贝处的）求值，以免整个lambda还没被使用就死递归在无限求值上。
  
所以这样得到的lambda递归会具有如下形式  

    (define (part-factorial self)
        ((lambda (f)
           (lambda (n)
             (if (= n 0)
                 1
                 (* n (f (- n 1)))))) //此处f是(self,self)，如果不延迟求值，会无限递归
         (self self)))
    
    (define factorial (part-factorial part-factorial))

#####剥离Y Combinator，以便复用

    (define (Y f)
        ((lambda (x) (x x))       //这个是对下面那个lamba的(x x)
         (lambda (x) (f (x x))))) //这个是对目标lambda的(x x)
     
这种剥离只是为了复用，如果你不想复用，每次都像上面的factorial一样写那个结构也可以，理解这点才是重点。这就跟C语言的宏一样，很多用宏做的东西，你展开后看很简单，用宏只是在理解的基础上的复用。学习算法和数据结构也是一样，泛型容器和算法只是在你理解基础上的复用，但你要会手写才真的知道其中关键点。复用只是为了减少代码量，手写结构，并理解才是基础。

#####Y Combinator可以写成更直观：

    (define (Y f)
        ((lambda (y) (y y))       //这个是对下面那个lamba的(y y)
         (lambda (x) (f (x x))))) //这个是对目标lambda的(x x)

全部都是x的那个版本，只是为了装代数符号的逼格而把上面的小y也写成x绕晕你。

#####Y Combinator刚好符合函数不动点性质

    (Y f) = (f (Y f))


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

#### malloc, calloc, realloc and free

##### 函数原型
[维基百科-C Dynamic memmory allocation](http://en.wikipedia.org/wiki/C_dynamic_memory_allocation)
```
void *malloc(size_t size);
void * calloc(size_t number, size_t size);
void * realloc(void *ptr, size_t size);
void free(void *ptr);
```
##### DESCRIPTION
- The malloc() function allocates size bytes of uninitialized memory.  The allocated space is suitably aligned (after possible pointer coercion) for storage of any type of object.
- The calloc() function allocates space for number objects, each size bytes in length.  The result is identical to calling malloc() with an argument of ``number * size'', with the exception that the allocated memory is explicitly initialized to zero bytes.
- The realloc() function changes the size of the previously allocated memory referenced by ptr to size bytes.  The contents of the memory are unchanged up to the lesser of the new and old sizes.  If the new size is larger, the contents of the newly allocated portion of the memory are undefined.  Upon success, the memory referenced by ptr is freed and a pointer to the newly allocated memory is returned.  Note that realloc() and reallocf() may move the memory allocation, resulting in a different return value than ptr.  If ptr is NULL, the realloc() function behaves identically to malloc() for the specified size.
- The free() function causes the allocated memory referenced by ptr to be made available for future allocations.  If ptr is NULL, no action occurs.

##### 内存分配原理
[猛击这个页面](http://blog.163.com/xychenbaihu@yeah/blog/static/132229655201210975312473/)

##### free(NULL)是合法的  
>Yes, it's safe. The language reference for the c language (and, I believe, c++ language by extension) indicate that no action will be taken.
This is OK in the sense that you can safely (re)free a pointer variable without worrying that it's already been deleted.
That's the language taking care of things just so you don't have to do stuff like
if (x) free(x);
You can just free(x) without worry. 

##### malloc(-1)
此时，由于malloc的参数是size_t，所以-1被转成SIZE_MAX..

##### malloc的时候，core dump
>Note that a crash in malloc or free is usually indicative of a bug which has previously corrupted the heap. 

##### malloc超过128k会发生什么
稍微大一点(通常是超过128k)的内存是通过mmap来分配的，小的内存是在heap上通过brk分配的
1、brk是将数据段(.data)的最高地址指针_edata往高地址推；
2、mmap是在进程的虚拟地址空间中（堆和栈中间，称为文件映射区域的地方）找一块空闲的虚拟内存。

#### MALLOC_CHECK_环境变量
>Recent  versions  of  Linux libc (later than 5.4.23) and GNU libc (2.x)
include a malloc implementation which is tunable via environment  vari-
ables.  When MALLOC_CHECK_ is set, a special (less efficient) implemen-
tation is used which is designed to be tolerant against simple  errors,
such as double calls of free() with the same argument, or overruns of a
single byte (off-by-one bugs).  Not all such errors  can  be  protected
against, however, and memory leaks can result.  If MALLOC_CHECK_ is set
to 0, any detected heap corruption is silently ignored; if set to 1,  a
diagnostic is printed on stderr; if set to 2, abort() is called immedi-
ately.  This can be useful because otherwise a crash  may  happen  much
later,  and  the  true cause for the problem is then very hard to track
down.

#### malloc & HeapAlloc
- [malloc-vs-heapalloc](http://stackoverflow.com/questions/8224347/malloc-vs-heapalloc)
- malloc依赖于CRT，是C标准。HeapAlloc是Windows API，Windows其他语言亦可使用。
- malloc在Windows上可能基于HeapAlloc，在Linux下可能基于sbrk和mmap
- Win16时代，有GlobleAlloc,LocalAlloc
- Win32时代，有HeapAlloc，可以指定在哪个Heap上分配内存，每个进程有一个default Heap，而malloc不能指定堆
- malloc是模块可见度，在一个模块内分配的只能在同一个模块内free

#### VisualAlloc
- VisualAlloc可以用来声明对某块虚拟内存的占用，或者提交
- 被提交过的虚拟内存可以再次被提交

#### Comparing Memory Allocation Methods
The following is a brief comparison of the various memory allocation methods:
- CoTaskMemAlloc
- GlobalAlloc
- HeapAlloc
- LocalAlloc
- malloc
- new
- VirtualAlloc

>Although the GlobalAlloc, LocalAlloc, and HeapAlloc functions ultimately allocate memory from the same heap, each provides a slightly different set of functionality. For example, HeapAlloc can be instructed to raise an exception if memory could not be allocated, a capability not available with LocalAlloc. LocalAlloc supports allocation of handles which permit the underlying memory to be moved by a reallocation without changing the handle value, a capability not available with HeapAlloc.
Starting with 32-bit Windows, GlobalAlloc and LocalAlloc are implemented as wrapper functions that call HeapAlloc using a handle to the process's default heap. Therefore, GlobalAlloc and LocalAlloc have greater overhead than HeapAlloc.
Because the different heap allocators provide distinctive functionality by using different mechanisms, you must free memory with the correct function. For example, memory allocated with HeapAlloc must be freed with HeapFree and not LocalFree or GlobalFree. Memory allocated with GlobalAlloc or LocalAlloc must be queried, validated, and released with the corresponding global or local function.
The VirtualAlloc function allows you to specify additional options for memory allocation. However, its allocations use a page granularity, so using VirtualAlloc can result in higher memory usage.
The malloc function has the disadvantage of being run-time dependent. The new operator has the disadvantage of being compiler dependent and language dependent.
The CoTaskMemAlloc function has the advantage of working well in either C, C++, or Visual Basic. It is also the only way to share memory in a COM-based application, since MIDL uses CoTaskMemAlloc and CoTaskMemFree to marshal memory.

#### COM内存管理规则
- The lifetime management of pointers to interfaces is always accomplished through the AddRef and Release methods found on every COM interface. (See "Reference-Counting Rules" below.)
- The following rules apply to parameters to interface member functions, including the return value, that are not passed "by-value":
    - For in parameters, the caller should allocate and free the memory.
    - The out parameters must be allocated by the callee and freed by the caller using the standard COM memory allocator.
    - The in-out parameters are initially allocated by the caller, then freed and re-allocated by the callee if necessary. As with out parameters, the caller is responsible for freeing the final returned value. The standard COM memory allocator must be used.
- If a function returns a failure code, then in general the caller has no way to clean up the out or in-out parameters. This leads to a few additional rules:
    - In error returns, out parameters must always be reliably set to a value that will be cleaned up without any action on the caller's part.
    - Further, it is the case that all out pointer parameters (including pointer members of a caller-allocate callee-fill structure) must explicitly be set to NULL. The most straightforward way to ensure this is (in part) to set these values to NULL on function entry.
    - In error returns, all in-out parameters must either be left alone by the callee (and thus remaining at the value to which it was initialized by the caller; if the caller didn't initialize it, then it's an out parameter, not an in-out parameter) or be explicitly set as in the out parameter error return case.

#### COM引用计数规则
- Rule 1: AddRef must be called for every new copy of an interface pointer, and Release called for every destruction of an interface pointer, except where subsequent rules explicitly permit otherwise.
The following rules call out common nonexceptions to Rule 1.
    - Rule 1a: In-out-parameters to functions. The caller must AddRef the actual parameter, since it will be Released by the callee when the out-value is stored on top of it.
    - Rule 1b: Fetching a global variable. The local copy of the interface pointer fetched from an existing copy of the pointer in a global variable must be independently reference counted, because called functions might destroy the copy in the global while the local copy is still alive.
    - Rule 1c: New pointers synthesized out of "thin air." A function that synthesizes an interface pointer using special internal knowledge, rather than obtaining it from some other source, must do an initial AddRef on the newly synthesized pointer. Important examples of such routines include instance creation routines, implementations of IUnknown::QueryInterface, and so on.
    - Rule 1d: Returning a copy of an internally stored pointer. After the pointer has been returned, the callee has no idea how its lifetime relates to that of the internally stored copy of the pointer. Thus, the callee must call AddRef on the pointer copy before returning it.

- Rule 2: Special knowledge on the part of a piece of code of the relationships of the beginnings and the endings of the lifetimes of two or more copies of an interface pointer can allow AddRef/Release pairs to be omitted.
    - From a COM client's perspective, reference-counting is always a per-interface concept. Clients should never assume that an object uses the same reference count for all interfaces.
    - The return values of AddRef and Release should not be relied upon, and should be used only for debugging purposes.
    - Pointer stability; see details in the OLE Help file under "Reference-Counting Rules," subsection "Stabilizing the this Pointer and Keeping it Valid."

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

#### lua coroutine 的使用场景

- 异步回调转同步：

假设有如下的异步调用的lua代码
```
function(arg)
  async(arg,function(state,ret)
      if state=="case1" then
        process_case1(ret)
      elseif state=="case2" then
        process_case2(ret)
      end
  end)
end

```

可以通过lua的coroutine转成同步写法
```
- lua
coroutine.wrap(function(arg)   
   await_async(arg)

   local error,value = coroutine.yield()
   if value.state=="case1" then
      process_case1(value.ret)
      error,value = coroutine.yield()
   end

   if value.state=="case2" then
      process_case2(value.ret)
   else
      assert(false)
   end
   coroutine.yield()

end)

function await_async(arg)
   local self = coroutine.running()
   async(arg,function(state,ret)   
      if state=="case1" then
        coroutine.resume(self,{state,ret})
      elseif state=="case2" then
        coroutine.resume(self,{state,ret})
      end
   end)
end

```

看似写了更多代码，不过如果在合理的封装下，可以避免callback hell。

实际上，yield/resume的使用，和c里面使用信号量有点类似
```
local value = nil

async(funciton(state,ret)
    value = {state,ret}
    if state=="case1" then
      event.singal()
    elseif state=="case2" then
      event.singal()
    end
end)

event.waitonce()
if value.state=="case1" then
  process_case1()
  event.waitonece()
end

if value.state=="case2" then
  process_case2()
else
  assert(false)
end

```
当然，这里的event.waitonce()会把整个线程卡死，这个代码需要跑在单独的线程上。
这也是event.waitonce()和coroutine.yiled()的区别的地方，如果上fiber的话，则是一样的。

- 动画更新

一个动画引擎内部有一个死循环，不停的刷帧，在关键时间点上调用外部注入的不同时间点的update方法。
如果不用coroutine的情况下，这样写：
```
function update(time)
  switch(time)
    case time1:load() update1() save() break
    case time2:load() update2() sava() break
    case time3:load() update3() save() break
    defulat: break
  end
end
animat.add(update)
```

但是，如果updatei之间有很多依赖状态，这些中间状态如果都用load(),save()等保存的话，会逐渐变的繁杂。
换用coroutine之后可以是：
```
local co = coroutine.wrap(function(time)
  local error = nil
  if time==time1 then
    update1()
    error,time = coroutine.yield()
  end 

  if time==time1 then
    update1()
    error,time = coroutine.yield()
  end 
  
  if time==time2 then
    update1()
    error,time = coroutine.yield()
  end 

  if time==time3 then
    update1()
    error,time = coroutine.yield()
  end 
end)

function update(time)
  coroutine.resume(co,time)
end

animat.add(update)
```

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
