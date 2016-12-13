Lua 是单线程的，但是Lua却有`thread`类型，显然直觉上Lua的`thread`并非通常意义上的线程，实际上它是一种协程，也就是所谓的`coroutine`。如果是第一次接触这个概念，则建议先从用例上感受coroutine和thread的不同是比较恰当的。

根据前两篇，我们知道Lua的基本类型是：
```
typedef struct lua_TValue{
  Value v;
  int tt_;
}TValue;
```
并且我们知道，Value是一个联合体，包括垃圾回收类型GCObject和其他非垃圾回收类型。Lua的`thread`也是属于GOObject的一种，其在`tag type`中的大类型定义如下：
```
#define LUA_TTHREAD		8
```
根据前一篇，获取和设置`thread`类型的TValue基本信息的接口如下：
```
#define ttisthread(o) checktag((o), ctb(LUA_TTHREAD))
#define thvalue(o)    check_exp(ttisthread(o), gco2th(val_(o).gc))
#define setthvalue(L,obj,x)       \
  {                               \ 
    TValue *io = (obj);           \
    lua_State *x_ = (x);          \
    val_(io).gc = obj2gco(x_);    \ 
    settt_(io, ctb(LUA_TTHREAD)); \
    checkliveness(G(L),io);       \
 }
```
所以，既然`thread`也是一种GCObject，它就一定有GCObject的公共头部：`CommonHeader`，并且根据之前几个GCObject都有用以辅助垃圾回收的`GCObject* gclist`，我们也可推测这个`thread`结构应该有此字段。我们就一睹Lua的`thread`结构的真面目：

# lua_State

```
/*
** 'per thread' state
*/
struct lua_State {
  CommonHeader;
  lu_byte status;
  StkId top;  /* first free slot in the stack */
  global_State *l_G;
  CallInfo *ci;  /* call info for current function */
  const Instruction *oldpc;  /* last pc traced */
  StkId stack_last;  /* last free slot in the stack */
  StkId stack;  /* stack base */
  UpVal *openupval;  /* list of open upvalues in this stack */
  GCObject *gclist;
  struct lua_State *twups;  /* list of threads with open upvalues */
  struct lua_longjmp *errorJmp;  /* current error recover point */
  CallInfo base_ci;  /* CallInfo for first level (C calling Lua) */
  lua_Hook hook;
  ptrdiff_t errfunc;  /* current error handling function (stack index) */
  int stacksize;
  int basehookcount;
  int hookcount;
  unsigned short nny;  /* number of non-yieldable calls in stack */
  unsigned short nCcalls;  /* number of nested C calls */
  lu_byte hookmask;
  lu_byte allowhook;
};
```
好吧，猜中了`CommonHeader`和`GCObject* gclist`，但是数据结构的名字则根本与"thread"无关，而是叫`lua_State`。

抛开名字不说，Lua的结构体的字段布局都是被调整过做内存对齐用的。而我们通常写代码并不是这么来的，通常我们会根据字段在语义上属于一个小模块而放置。那么，我们就先根据字面上的意思，对这些字段重新排版：
```
struct lua_State {
  CommonHeader;

  // 1 
  global_State *l_G;

  // 2
  lu_byte status;

  // 3 
  const Instruction *oldpc;  /* last pc traced */

  // 4 Data Stack: [stak,...,top,...,stack_last], length is stacksize
  int stacksize;
  StkId top;                 /* first free slot in the stack */
  StkId stack;               /* stack base */
  StkId stack_last;          /* last free slot in the stack */
  
  // 5 Call Stack
  CallInfo base_ci;          /* CallInfo for first level (C calling Lua) */  
  CallInfo *ci;              /* call info for current function */
  
  unsigned short nCcalls;    /* number of nested C calls */
  unsigned short nny;        /* number of non-yieldable calls in stack */

  // 6 Up Value
  UpVal *openupval;          /* list of open upvalues in this stack */
  struct lua_State *twups;   /* list of threads with open upvalues */

  // 7 Recover
  struct lua_longjmp *errorJmp;  /* current error recover point */
  ptrdiff_t errfunc;  /* current error handling function (stack index) */

  // 8 Hook for Debug
  lua_Hook hook;
  int basehookcount;
  int hookcount;
  lu_byte hookmask;
  lu_byte allowhook;

  // 9
  GCObject *gclist;
};
```
1. `global_State* l_G;` 这个是Lua的全局对象，所有的lua_State共享一个global_State，global_State里塞进了各种全局字段。此处先不管。
2. `lu_byte status;` 一个`thread`实际上就是一个代码指令**顺序执行**的地方，而这，本质上是一个`状态机`(**state machine**)，状态机执行的过程中会处于各种中间步骤，所以每个步骤都算一种`status`。（ps, state和status常常混淆了可见`lua_State->status`可以作为一个记忆关键字）
3. 一个`thread`的运行过程，就是一个死循环解释执行指令的过程，必不可少的会有一个指针指向最后一次执行的指令的指针：`const Instruction *oldpc;`. 
4. 一个`thread`的运行过程，需要两个基本的Stack：对应于图灵机纸带的DataStack+函数调用过程中动态嵌套的CallStack。Lua里每个对象都是一个TValue，所以DataStack就是一个动态增减的TValue数组，实际上`typedef TValue *StkId; `，重定义为StkId是在语义上做的区分。所以thread数据栈的栈底就是`stack`，顶部是`top`，而`top`到`stack_last`之间则是未使用的部分。本质上，对于一个栈，最重要的信息是：栈底、栈顶、栈空间。
5. 和数据栈直接用TValue数组存储不同，CallStack实际上是由CallInfo所构成的链表，函数执行过程中，动态增减的CallInfo构成了一个链表，但只要这个过程符合**First In，Last Out**，它就是一个名副其实的Stack。同DataStack一样，我们需要记录这个CallStack的栈底：`base_ci`，由于Lua是从宿主语言C开始发起调用的，栈底（最外层的CallInfo)base_ci一定是从C开始发起调用的。而栈顶，就是当前正在执行的**函数**的CallInfo。注意，根据第1篇，我们知道Lua有三种类型的函数：light C function、Lua Closure、C Closure。所以这里的`nCCalls`记录的是CallStack动态增减过程中调用的C函数的个数。而`nny `记录的是`non-yieldable`的调用个数，什么是`yieldable`显然不是此处可以说清楚，暂时不管。
6. 根据第1篇，C Closure和Lua Closure都会有闭包变量。C Closure的闭包直接就是一个TValue数组保存在CClosure里，而Lua Closure的闭包变量，分为`open`和`close`两种状态，如果是`close`状态，则也拷贝到LClosure自己的UpVal数组里，但如果是`open`状态，则直接指向了作用域上的变量地址。可以理解，CallStack展开过程中，从CallStack的栈底到栈顶的所有open的UpVal也构成了一种Stack。Lua把这些open状态的UpVal用链表串在一起，我们可以认为是一个`open upvalue stack`，这个stack的栈底就是`UpVal* openval；`，而一个lua_State代表一个协程，一个协程可能闭包别的协程的变量，所以`struct lua_State *twups;`就是代表了那些闭包了当前lua_State的变量的其他协程。
7.  一个thread在CallStack执行过程中，需要有全局的异常、出错处理。在带有异常的语言里，印象比较深的是投递某个task到特性线程的thread上执行，那个线程上抛出的异常需要保存，在同步会caller线程的时候再返回。所以每个thread都要有自己的全局异常、错误处理。
8. 这是一组全局的Hook点，用以辅助Debug，暂时不管，我们就假设实际是不需要Hook的好了。
9. 垃圾回收专用，先不管。

OK，我们囫囵吞枣了一圈，CallInfo长什么样子还不知道呢：
```
/*
** Information about a call.
** When a thread yields, 'func' is adjusted to pretend that the
** top function has only the yielded values in its stack; in that
** case, the actual 'func' value is saved in field 'extra'. 
** When a function calls another with a continuation, 'extra' keeps
** the function index so that, in case of errors, the continuation
** function can be called with the correct top.
*/
typedef struct CallInfo {
  StkId func;  /* function index in the stack */
  StkId top;  /* top for this function */
  struct CallInfo *previous, *next;  /* dynamic call link */
  union {
    struct {  /* only for Lua functions */
      StkId base;  /* base for this function */
      const Instruction *savedpc;
    } l;
    struct {  /* only for C functions */
      lua_KFunction k;  /* continuation in case of yields */
      ptrdiff_t old_errfunc;
      lua_KContext ctx;  /* context info. in case of yields */
    } c;
  } u;
  ptrdiff_t extra;
  short nresults;  /* expected number of results from this function */
  lu_byte callstatus;
} CallInfo;
```
那一坨注释我们就先要看了，都是说为什么需要有一个extra字段的，在执行过程中临时保持func用的。单说CallInfo内部有一个Union，里面分别是Lua Function需要的字段和C Function需要的字段。

不妨把他们都分别展开一次：

#### Lua CallInfo
```
typedef struct LuaCallInfo  {
  // DataStack  [base,...,top]
  StackId base; 
  StkId top;   

  // Closure
  StkId func;   // Lua Closure
  ptrdiff_t extra;
  
  // Code
  const Instruction* savedpc;

  // Call Result
  lu_byte callstatus;
  short nresults;

  // Call link
  struct CallInfo *previous, *next; 
}
```
`base`和`top`是数据栈，func是一个LClosure，而LClosure里面包含了lua Proto(=指令+参数+局部变量+常量+内嵌函数..)；savedpc就是当前执行的指令。`callstatus`是调用后的结果，`nresults`描述返回结果的个数，便于在执行结束的时候调整top。

显然`struct CallInfo *previous, *next; `是用以串起动态增减的CallStack。

#### C CallInfo
```
typdef struct CCallInfo  {
  // Data
  StkId top;  

  // Closure
  StkId func;   // C Closure
  ptrdiff_t extra;

  // Call Result
  lu_byte callstatus;
  short nresults;

  // Error Recover
  ptrdiff_t old_errfunc;

  // Continuation(or Callback)
  lua_KFunction k;
  lua_KContext ctx;

  // Call link
  struct CallInfo *previous, *next; 
}
```

和Lua CallInfo 稍微有点不同，C CallInfo并不需要DataStack的base，只需要记住数据栈栈顶即可。func的里面就是一个CClosure(lua_CFunction+闭包的TValues数组，代码和数据都简单多了）。执行的过程也别Lua CallInfo简单多了，直接调用CClosure里面的lua_CFunction即可，C函数的执行超出了Lua的控制范围，每一层执行都需要有一个old_errfunc，用以错误处理。

而下面这组：
```
  lua_KFunction k;
  lua_KContext ctx;
```
则是和yield有关的，简单说：lua function都是可以yield的，那么如果lua function里面调用了一个c function，而如果你想在 c functino里也做yield，Lua是做不到再返回到c function里的某个yiled点的，这是因为c 的函数栈并不具有yield的能力。但是如果不解决这个问题，Lua的yiled系统就不完备，Lua 5.2开始的采用这种解决办法：如果要在c function里yield，那么这个yield必须是函数执行的最后一行，调用如下的API：
```
LUA_API void lua_callk (lua_State *L,
   int nargs , 
   int nresults , 
   lua_KContext  ctx, 
   lua_KFunction k) ;
```
由于是最后一行了，所以C function的stack就不需要保留了，在需要yield回来的时候，实际上是yield到lua_KFunction这个函数里去。而lua_KContext ctx显然是一个UserData。这个做法，太熟悉了，我们在C或者C++里要做一个异步API，一般都是这样的：
```
typdef void (*Callback)(void* pUserData,
  int ret);
XXX_API(int) XXX_Async(..., 
  Callback pfnCallback,
  void* pUserData);
```
实际上，这就是一种**yield**，专业术语是**continuation passing style**，那个K就是continuation的缩写。

# 待续
下一次，可以先分析global_State;