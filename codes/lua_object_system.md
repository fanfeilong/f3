版本号:Lua 5.3

# Lua Type

lua 的类型定义在lobject.h这个文件里，主要的类型如下：
- none
- nil
- light user data
- boolean
- number
  - integer
  - float
- function type
  - light C function
  - closure (gc object)
    - lua closure 
    - C closure 
- string (gc object)
- user data (gcobject)
- table (gc object)
- thread ( gc object)

# Lua Value
lua使用一个union来统一表示上述类型：

```
union Value {
  GCObject *gc;    /* collectable objects */
  void *p;         /* light userdata */      
  int b;           /* booleans */            
  lua_CFunction f; /* light C functions */ 
  lua_Integer i;   /* integer numbers */    
  lua_Number n;    /* float numbers */   
};
```
同时，添加一个额外的byte来标记具体的类型：
```
#define TValuefields	Value value_; int tt_  //<值，类型标记>
struct lua_TValue {
  TValuefields; // Value value_; int tt_;
};
typedef struct lua_TValue TValue;
```
如果展开上述代码，则为：
```
typedef struct lua_TValue{
   Value value_;
   int tt_;      
}TValue;
```
其中，`tt_`是一个**8 bits** 的类型标记字段，被分成**3**个部分：
- **0-3**位，表示大类型
- **4-5**位，表示子类型
- 第**6**位，表示是否可以垃圾回收

综合使用上面三点，就可以完整标记所有的lua类型，每种类型标记的值如下(这些定义在lua.h和lobject.h里，此处把它们合在一起，更直观)：
```
#define LUA_TNONE		     (-1)
#define LUA_TNIL		      0
#define LUA_TBOOLEAN		  1
#define LUA_TLIGHTUSERDATA	2
#define LUA_TNUMBER		   3
  #define LUA_TNUMFLT	     (LUA_TNUMBER | (0 << 4))  /* float numbers */
  #define LUA_TNUMINT	     (LUA_TNUMBER | (1 << 4))  /* integer numbers */
#define LUA_TSTRING		   4
  #define LUA_TSHRSTR     	(LUA_TSTRING | (0 << 4))  /* short strings */
  #define LUA_TLNGSTR     	(LUA_TSTRING | (1 << 4))  /* long strings */
#define LUA_TTABLE		    5
#define LUA_TFUNCTION		 6
  #define LUA_TLCL	        (LUA_TFUNCTION | (0 << 4))  /* Lua closure */
  #define LUA_TLCF	        (LUA_TFUNCTION | (1 << 4))  /* light C function */
  #define LUA_TCCL	        (LUA_TFUNCTION | (2 << 4))  /* C closure */
#define LUA_TUSERDATA		 7
#define LUA_TTHREAD		   8
#define LUA_NUMTAGS		   9
#define LUA_TPROTO	        LUA_NUMTAGS
#define LUA_TDEADKEY	      (LUA_NUMTAGS+1)
#define LUA_TOTALTAGS	     (LUA_TPROTO + 2)
#define BIT_ISCOLLECTABLE	 (1 << 6)
```

Value是一个联合体，第一个字段是GCObject，包括：`closure(lua closure+C closure), string, userdata, table, thread`，其他几个则是非垃圾回收类型：`light user data, boolean, light C function, number(integer+float)`.非垃圾回收字段被直接展开在联合体里，GCObject则是可垃圾回收类型的公共类：
```
#define CommonHeader GCObject* next;lua_byte tt; lua_byte marked
typedef struct GCObject{
  CommonHeader; // GCObject* next;lua_byte tt; lua_byte marked;
};
```

# GC Object
展开上述GCObject代码，则为：
```
typedef struct GCObject{
  GCObject* next;
  lua_byte tt; 
  lua_byte marked;
};
```
可见，GCObject是以链表的形式串在一起。其中，tt字段是类型标记字段，既然TValue里已经标记了类型，此处为什么重复标记呢？我的理解是因为在使用的过程中，GCObject未必是作为一个TValue传入，如果只有GCObject指针的时候，重复的tt即可使用上。而marked则是在垃圾回收过程中用以标记对象存活状态的。

所有的GC类型，都有公共的CommonHeader头部，这是在C这种语言里的一种“继承”用法。

## TString
```
typedef struct TString{
  CommonHeader; // GCObject* next;lua_byte tt; lua_byte marked;
  lua_byte extra;
  unsigned int hash;
  size_t len;
  struct TString* hnext;
}TString;
```
由于lua的string有两个子类型：`short string`和`long string`。其中，extra字段用来标记是否是long string，hash字段则是用存储在全局字符串池里的哈希值；len表示长度，lua的字符串并不以\0结尾，所以需要存储长度信息。hnext是用来把全局TString串起来，整个链表就是字符串池。而真正的字符串的内容，直接存储在结构体后面的内存里，为了保证内存的对齐，对上述TString和基本类型合并做一个字节对齐：
```
typedef union { double u; void *s; lua_Integer i; long l; } L_Umaxalign;
typedef union UTString{
  L_Umaxalign dummy;
  TString tsv; 
}UTString;
```
从而，真正的字符串内容的内存地址获取如下：
```
/*
** Get the actual string (array of bytes) from a 'TString'.
** (Access to 'extra' ensures that value is really a 'TString'.)
*/
#define getaddrstr(ts)	(cast(char *, (ts)) + sizeof(UTString))
#define getstr(ts)  \
  check_exp(sizeof((ts)->extra), cast(const char*, getaddrstr(ts)))

/* get the actual string (array of bytes) from a Lua value */
#define svalue(o)       getstr(tsvalue(o))
```

## UData
```
typedef struct Udata{
  CommonHeader;
  lua_byte ttuv_;// user value's tag
  struct Table* metatable;
  size_t len;
  union Value user_; //user value
}Udata;
```
User Data和String的布局基本一样。首先是共同的CommonHeader，然后是一个类型标记字段: `ttuv_`，此处标记的是该UserData里实际存储的值(`user_`字段)的类型；其次最明显的区别是有一个Table类型的`metatable`，所有对User Data的操作都会去这个metatable里查找是否有对应的属性或者方法定义，这也是lua的所有魔法所在。`len`字段则定义了实际的数据长度，同时还有一个附加的用户定义值字段：`user_`。

User Data和String一样把额外的数据块存在结构体后面的内存里，同样地对起始地址做了对齐：
```
typedef union UUdata{
  L_Umaxalign dummy;
  Udata uv;
}UUdata;
```
从而，User Data的额外数据块的地址如下，注意：
```
/*
**  Get the address of memory block inside 'Udata'.
** (Access to 'ttuv_' ensures that value is really a 'Udata'.)
*/
#define getudatamem(u)  \
  check_exp(sizeof((u)->ttuv_), (cast(char*, (u)) + sizeof(UUdata)))
```

另外，对于User Data来说，metatable是每个实例一个，`user_`和`ttuv_`两个字段则是值部分。所以设置和获取UserData的接口如下：
```
#define setuservalue(L,u,o) \
	{ const TValue *io=(o); Udata *iu = (u); \
	  iu->user_ = io->value_; iu->ttuv_ = io->tt_; \
	  checkliveness(G(L),io); }

#define getuservalue(L,u,o) \
	{ TValue *io=(o); const Udata *iu = (u); \
	  io->value_ = iu->user_; io->tt_ = iu->ttuv_; \
	  checkliveness(G(L),io); }
```

总之，UserData=tag+value+metatable=data+metatable；

## Table
```
typedef struct Table {
  CommonHeader;
  lu_byte flags;  /* 1<<p means tagmethod(p) is not present */
  lu_byte lsizenode;  /* log2 of size of 'node' array */
  unsigned int sizearray;  /* size of 'array' array */
  TValue *array;  /* array part */
  Node *node;
  Node *lastfree;  /* any free position is before this position */
  struct Table *metatable;
  GCObject *gclist;
} Table;
```

首先，类似User Data，Table也包括data和metatable，其中metatable的构成如下：
```
  lu_byte flags; // 1<<p means tagmethod(p) is not present
  struct Table* metatable;
```
如果要判断某个预定义下标的元方法是否存在，可以通过1<<p来判断，如果有，则从metatable里获取。

其次，Table包括array部分和hash table部分，array部分如下：
```
// array 
  unsigned int sizearray;
  TValue* array;
```

而hash table部分如下：
```
// hash table
  lu_byte lsizenode;
  Node* node;
  Node* lastfree;
```
Node就是一个key-value，通过key部分的链表串在一起：
```
typedef struct Node {
  TValue i_val;
  TKey i_key;
} Node;

typedef union TKey {
  struct {
    TValuefields;
    int next;  /* for chaining (offset for next node) */
  } nk;
  TValue tvk;
} TKey;
```

最后，gclist是用以垃圾回收的，按下不表。从而，我们可以重新调整下Table的声明顺序，使得更利于阅读：

```
typedef struct Table{
  CommonHeader;
  
  lu_byte flags; // 1<<p means tagmethod(p) is not present
  struct Table* metatable;

  lu_byte lsizenode;
  Node* node;
  Node* lastfree;

  unsigned int sizearray;
  TValue* array;

  GCObject* gclist;
}Table;
```
## Closure
到了最复杂的Closure部分。根据前面的铺垫，我们知道Lua的函数包括Lua Closure, light C function以及 C Closure三种小类型，其中light C function就是纯c函数，在Value的定义里直接用一个lua_CFunction函数指针指向，从而剩下两个Closure类型。

lua的源码里把Lua Closure和 C Closure作为一个联合体，构成了Closure类型：
```
typedef union Closure{
  CClosure c;
  LClosure l;
}Closure;
```
Closure作为一个GC Object，自然需要包含CommonHeader，由于是一个联合体，所以这个CommonHeader就分别拆到了CClosure和LClosure里去了：
```
#define ClosureHeader \
	CommonHeader; lu_byte nupvalues; GCObject *gclist

typedef struct CClosure {
  ClosureHeader;
  lua_CFunction f;
  TValue upvalue[1];  /* list of upvalues */
} CClosure;


typedef struct LClosure {
  ClosureHeader;
  struct Proto *p;
  UpVal *upvals[1];  /* list of upvalues */
} LClosure;
```
注意，这里CommonHeader+nupvalues+gclist共同构成了ClosureHeader，这是因为两种Closure都有公共的部分：`nupvalues`说明闭包变量的个数，gclist则用以垃圾回收。

我们先看比较简单的CClosure，就是直接把lua_CFunction加上被闭包的c变量upvalue[1]数组，此处利用数组在结构的末尾，则只需声明为一个元素的数组即可。

比较复杂的是LClosure，中间的关键结构是Proto* p; 这个字段代表了一个Lua 闭包。我们一步步展开：
```
/*
** Function Prototypes
*/
typedef struct Proto {
  CommonHeader;
  lu_byte numparams;  /* number of fixed parameters */
  lu_byte is_vararg;
  lu_byte maxstacksize;  /* maximum stack used by this function */
  int sizeupvalues;  /* size of 'upvalues' */
  int sizek;  /* size of 'k' */
  int sizecode;
  int sizelineinfo;
  int sizep;  /* size of 'p' */
  int sizelocvars;
  int linedefined;
  int lastlinedefined;
  TValue *k;  /* constants used by the function */
  Instruction *code;
  struct Proto **p;  /* functions defined inside the function */
  int *lineinfo;  /* map from opcodes to source lines (debug information) */
  LocVar *locvars;  /* information about local variables (debug information) */
  Upvaldesc *upvalues;  /* upvalue information */
  struct LClosure *cache;  /* last created closure with this prototype */
  TString  *source;  /* used for debug information */
  GCObject *gclist;
} Proto;
```

调整字节对齐后的结构体并不利于阅读，我们不妨重新排版下：
```
typedef struct Proto{
  CommonHeader;
  
  // 1
  lu_byte numparams;
  lu_byte is_vararg;
  lu_byte maxstacksize;

  // 2
  int sizek;
  TValue* k;
  
  // 3
  int sizelocalvars;
  LocVar* locvars;

  // 4
  int sizeupvalues;
  Upvaldesc* upvalues;

  // 5
  int sizep;
  struct Proto** p;
  struct LClosure* cache;

  // 6
  int sizecode;
  Instruction* code;

  // 7
  int sizelineinfo;
  int* lineinfo;

  // 8
  int linedefined;
  int lastlinedefined;
  TString* source;

  // 9
  GCObject* gclist;
}Proto;
```
1. 函数原型信息
  - `num params`: 函数参数个数
  - `is_vararg`: 是否是有变长参数
  - `maxstacksize`: 最大的函数栈长度
2. 常量
  - `sizek`: 常量数组长度
  - `k`: 常量数组
3. 局部变量
  - `sizelocalvars`:局部变量数组长度
  -  `localvars`: 局部变量数组
4. 闭包变量
  - `sizeupvalues`: 闭包变量数组长度
  - `upvalus`: 闭包变量数组
5. 嵌套的Proto：
  - `sizep`:嵌套的Proto数组长度
  - `p`:嵌套的Proto数组
  - `cache`: 缓存嵌套的Proto的闭包。
6. Proto代表一个可执行函数，前面的信息都是数据部分（参数、常量、局部变量、闭包变量），此处是指令：
  -  `sizecode`：指令数组的长度
  - `code`：三地址指令数组，后面单独讲解。
7. 行信息，用以debug，每行指令都有对应的行信息。
  - `sizelineinfo`:行信息数组长度
  - `lineinfo`:行信息数组
8. 源码
  - `linedefined`和`lastlinedefined`:函数的起始定义行号
  - `source`：源码字符串。
9. gclist，垃圾回收专用，后面讲解。

到此为止，我们把Proto的字段分拆一个闭包函数所需要的每个部分，更易于理解。但还有几个小模块。

#### LocVar
```
/*
** Description of a local variable for function prototypes
** (used for debug information)
*/
typedef struct LocVar {
  TString *varname;
  int startpc;  /* first point where variable is active */
  int endpc;    /* first point where variable is dead */
} LocVar;
```
LocVar的定义，包括变量名+作用域。

#### Upvaldesc
```
/*
** Description of an upvalue for function prototypes
*/
typedef struct Upvaldesc {
  TString *name;  /* upvalue name (for debug information) */
  lu_byte instack;  /* whether it is in stack */
  lu_byte idx;  /* index of upvalue (in stack or in outer function's list) */
} Upvaldesc;
``` 
Upvaldesc只是描述了闭包变量的信息：是否在栈上+在栈上的Index。这里只有描述信息，那么闭包变量的值存储在哪里呢？

我们回头看下LClosure的定义：
```
typedef struct LClosure {
  ClosureHeader;
  struct Proto *p;
  UpVal *upvals[1];  /* list of upvalues */
} LClosure;
```
注意看，这里和CClosure不同的是，CClosure直接用TValue数组存储闭包变量，但LClosure则是用UpVal数组。我们看下UpVal。

#### UpVal
```
/*
** Upvalues for Lua closures
*/
struct UpVal {
  TValue *v;  /* points to stack or to its own value */
  lu_mem refcount;  /* reference counter */
  union {
    struct {  /* (when open) */
      UpVal *next;  /* linked list */
      int touched;  /* mark to avoid cycles with dead threads */
    } open;
    TValue value;  /* the value (when closed) */
  } u;
};
```
UpVal定义在lfunc.h文件里，这里第一个字段`v`就是指向了闭包变量的真正的值的指针。refcount是被闭包的引用计数，按下不谈。单说后面的联合体：
```
  union {
    struct {  /* (when open) */
      UpVal *next;  /* linked list */
      int touched;  /* mark to avoid cycles with dead threads */
    } open;
    TValue value;  /* the value (when closed) */
  } u;
```
注意看，联合体内部有一个open结构和一个value字段。一个Proto在外层函数没有返回之前，处于open状态，闭包的变量，直接通过UpVal ->v这个指针引用。此时open结构用来把当前作用域内的所有闭包变量都串起来做成一个链表，方便查找。此时u->value并没有用到。

但是，如果外层函数返回，则Proto需要把闭包变量的值拷贝出来，保证对象安全。这个拷贝就放在u->value里。此时，UpVal ->v也直接指向内部的u->value。

从而，我们也可以通过判断UpVal ->v和u->value是否相等来判断UpVal处于open还是clsoed状态：
```
#define upisopen(up)	((up)->v != &(up)->u.value)
```

# 待续
对象系统的定义部分就到这里，下次分解下对象系统基本属性读写的util。
......