#### 源码阅读顺序
-----------------
[Recommended reading order](http://www.reddit.com/comments/63hth/ask_reddit_which_oss_codebases_out_there_are_so/c02pxbp)
- lmathlib.c, lstrlib.c: get familiar with the external C API. Don't bother with the pattern matcher though. Just the easy functions.
- lapi.c: Check how the API is implemented internally. Only skim this to get a feeling for the code. Cross-reference to lua.h and luaconf.h as needed.
- lobject.h: tagged values and object representation. skim through this first. you'll want to keep a window with this file open all the time.
- lstate.h: state objects. ditto.
- lopcodes.h: bytecode instruction format and opcode definitions. easy.
- lvm.c: scroll down to luaV_execute, the main interpreter loop. see how all of the instructions are implemented. skip the details for now. reread later.
- ldo.c: calls, stacks, exceptions, coroutines. tough read.
- lstring.c: string interning. cute, huh?
- ltable.c: hash tables and arrays. tricky code.
- ltm.c: metamethod handling, reread all of lvm.c now.
- You may want to reread lapi.c now.
- ldebug.c: surprise waiting for you. abstract interpretation is used to find object names for tracebacks. does bytecode verification, too.
- lparser.c, lcode.c: recursive descent parser, targetting a register-based VM. start from chunk() and work your way through. read the expression parser and the code generator parts last.
- lgc.c: incremental garbage collector. take your time.
- Read all the other files as you see references to them. Don't let your stack get too deep though.

If you're done before X-Mas and understood all of it, you're good. The information density of the code is rather high.

#### Lua的类型标记
------------------

Lua的类型标记是这样的：
- 0-3标志位，表示大类型
- 4-5标志位，表示子类型，比如Number下可以细分整型和浮点型
- 6标志位表示十分可以垃圾回收。
```
#define LUA_TNONE		    (-1)
#define LUA_TNIL		    0
#define LUA_TBOOLEAN		1
#define LUA_TLIGHTUSERDATA	2
#define LUA_TNUMBER		 3
  #define LUA_TNUMFLT	(LUA_TNUMBER | (0 << 4))  /* float numbers */
  #define LUA_TNUMINT	(LUA_TNUMBER | (1 << 4))  /* integer numbers */
#define LUA_TSTRING		  4
  #define LUA_TSHRSTR	(LUA_TSTRING | (0 << 4))  /* short strings */
  #define LUA_TLNGSTR	(LUA_TSTRING | (1 << 4))  /* long strings */
#define LUA_TTABLE		  5
#define LUA_TFUNCTION		6
  #define LUA_TLCL	(LUA_TFUNCTION | (0 << 4))  /* Lua closure */
  #define LUA_TLCF	(LUA_TFUNCTION | (1 << 4))  /* light C function */
  #define LUA_TCCL	(LUA_TFUNCTION | (2 << 4))  /* C closure */
#define LUA_TUSERDATA		7
#define LUA_TTHREAD		  8
#define LUA_NUMTAGS		  9
#define LUA_TPROTO	    LUA_NUMTAGS
#define LUA_TDEADKEY	  (LUA_NUMTAGS+1)
#define LUA_TOTALTAGS	  (LUA_TPROTO + 2)
#define BIT_ISCOLLECTABLE	(1 << 6)
```

Lua的类型包括基本类型和可垃圾回收类型两大类，两大类又分别有很多子类，他们都尽量以union的形式组合，string和userdata的结构体为了内存连续直接把数据放在结构体末尾。函数包括lua_CFunction和Proto两种原子，他们又分别可以和Upvalue构成LClosure和CClosure，两种Closure又以union组合在一起。Lua的整个类型的组织非常简洁。可GC类型以隐式链表的形式组织在一起。

因为用了union，所以string和userdata 的数据部分要么只能用二级指针指向别的内存，要么只能像Lua的做法那样挂接到结构体的尾巴。显然Lua选择后者。

```
union Value {
  GCObject *gc;    /* collectable objects */
  void *p;         /* light userdata */      //注意和UserData区分开，light userdata只是一个void*
  int b;           /* booleans */            //布尔类型
  lua_CFunction f; /* light C functions */   //注意后Clousure区分开，这是不带闭包的lua_CFunction
  lua_Integer i;   /* integer numbers */     //整型
  lua_Number n;    /* float numbers */       //浮点型
};

#define TValuefields	Value value_; int tt_  //类型包括<值，类型标记>，类型标记参考前一节笔记

struct lua_TValue {
  TValuefields;
};
typedef struct lua_TValue TValue;

/*
** Common Header for all collectable objects (in macro form, to be
** included in other objects)
*/
#define CommonHeader	GCObject *next; lu_byte tt; lu_byte marked  //可垃圾回收类型构成一个隐式链表，关键字段包括<类型，标记>

/*
** Common header in struct form
*/
typedef struct GCheader {
  CommonHeader;
} GCheader;

/* type to ensure maximum alignment */
#if !defined(LUAI_USER_ALIGNMENT_T)
#define LUAI_USER_ALIGNMENT_T	union { double u; void *s; long l; }
#endif

typedef LUAI_USER_ALIGNMENT_T L_Umaxalign;

/*
** Header for string value; string bytes follow the end of this structure
*/
typedef union TString {

  //通过在uinon里插入double,void*,long使得string结构体的大小在对齐后不会小于这些基本类型
  L_Umaxalign dummy;  /* ensures maximum alignment for strings */   
  struct {
    CommonHeader;                                                                    //垃圾回收头部信息
    lu_byte extra;  /* reserved words for short strings; "has hash" for longs */     //short string的保留字段，long string时表示是否有hash
    size_t len;  /* number of characters in string */                                //长度
    union TString *hnext;  /* linked list for hash table */                          //string也够成一个隐式链表
    unsigned int hash;                                                               //hash值
  } tsv; //字符串类型头部信息，数据部分紧接在该结构体后一位，以`\0`结尾
} TString;

/* get the actual string (array of bytes) from a TString */
#define getstr(ts)	cast(const char *, (ts) + 1) //获取tsv结构体末尾的字符串内容

/* get the actual string (array of bytes) from a Lua value */
#define svalue(o)       getstr(rawtsvalue(o))    //获取tsv并获取紧接着的字符串内容


/*
** Header for userdata; memory area follows the end of this structure
*/
typedef union Udata {
  L_Umaxalign dummy;  /* ensures maximum alignment for `local' udata */  //参考string的注释
  struct {
    CommonHeader;                               //参考string的注释
    lu_byte ttuv_;  /* user value's tag */      //用以可以通过该字段自定义tag，第一节的那些tag是内置的，用户数据定义的tag不能与内置tag重复
    struct Table *metatable;                    //自定义数据的元表，提供操作符重载机会
    size_t len;  /* number of bytes */          //用户数据长度
    union Value user_;  /* user value */        //用户数据（递归的用Value定义）
  } uv;
} Udata;

#define setuservalue(L,u,o) \
	{ const TValue *io=(o); Udata *iu = (u); \
	  iu->uv.user_ = io->value_; iu->uv.ttuv_ = io->tt_; \
	  checkliveness(G(L),io); }//赋值UserData的两个重要字段，user_和ttuv_，参考TValuefield（value,tt)

#define getuservalue(L,u,o) \
	{ TValue *io=(o); const Udata *iu = (u); \
	  io->value_ = iu->uv.user_; io->tt_ = iu->uv.ttuv_; \
	  checkliveness(G(L),io); }//获取UserData的值，=setuservalue(L,o,u),除了最后一步checkliveness不同

/*
** Description of an upvalue for function prototypes
*/
typedef struct Upvaldesc {
  TString *name;  /* upvalue name (for debug information) */                    //上游变量的名字，用以DEBUG，也就是无论是否Debug都有该字段
  lu_byte instack;  /* whether it is in stack */                                //是否在当前LuaState的Stack里
  lu_byte idx;  /* index of upvalue (in stack or in outer function's list) */   //upValue的位置，当前函数或者外层函数的Stack，取决于上一个变量
} Upvaldesc;

/*
** Description of a local variable for function prototypes
** (used for debug information)
*/
typedef struct LocVar {
  TString *varname;                                            //局部变量的名字，所以对于Lua的Proto的栈来说，只有两种语义的变量UpValueh和LocalValue
  int startpc;  /* first point where variable is active */     //作用域开始位置
  int endpc;    /* first point where variable is dead */       //作用域结束位置
} LocVar;

//Upvaldesc和LocalVar都只是变量的索引信息，所以并不属于垃圾回收链表的一部分，不需要带CommonHeader

/*
** Function Prototypes
*/
typedef struct Proto {
  CommonHeader;                                                                     //函数原型
  TValue *k;  /* constants used by the function */                                  //常量数组，常量为什么发音为k，参考`具体编程语言`那个帖子
  Instruction *code;                                                                //代码（指令）数组。解释执行的时候用到
  struct Proto **p;  /* functions defined inside the function */                    //内部定义的原型数组
  int *lineinfo;  /* map from opcodes to source lines (debug information) */        //行信息数组，用以Debug
  LocVar *locvars;  /* information about local variables (debug information) */     //局部变量索引数组，变量都在LuaState的栈上
  Upvaldesc *upvalues;  /* upvalue information */                                   //上游变量索引数组，变量在当前函数和上一层函数的Stack上
  union Closure *cache;  /* last created closure with this prototype */             //在当前原型上的最后一次闭包
  TString  *source;  /* used for debug information */                               //原型的源码文本，用以调试
  int sizeupvalues;  /* size of 'upvalues' */                                       //上游变量索引数组的长度
  int sizek;  /* size of `k' */                                                     //常量数组的长度
  int sizecode;                                                                     //指令数组的长度
  int sizelineinfo;                                                                 //行数组的长度
  int sizep;  /* size of `p' */                                                     //内部定义的原型数组的长度
  int sizelocvars;                                                                  //局部变量数组的长度
  int linedefined;                                                                  //原型被定义的开始行位置
  int lastlinedefined;                                                              //原型被定义的结束行位置
  GCObject *gclist;                                                                 //可回收对象列表
  lu_byte numparams;  /* number of fixed parameters */                              //定长参数个数
  lu_byte is_vararg;                                                                //是否有不定参数
  lu_byte maxstacksize;  /* maximum stack used by this function */                  //该原型的栈最大值
} Proto;

/*
** Upvalues for Lua closures
*/
struct UpVal {
  TValue *v;  /* points to stack or to its own value */                 //闭包变量在栈上的位置或者值
  lu_mem refcount;  /* reference counter */                             //引用计数
  union { 
    struct {  /* (when open) */
      UpVal *next;  /* linked list */                                   //所有打开的闭包变量够成一个隐式链表
      int touched;  /* mark to avoid cycles with dead threads */        //避免循环，TODO：
    } open;                                                             //闭包变量打开时的值
    TValue value;  /* the value (when closed) */                        //闭包变量关闭时的值
  } u;
};
typedef struct UpVal UpVal;

/*
** Closures
*/

#define ClosureHeader \
	CommonHeader; lu_byte nupvalues; GCObject *gclist   //闭包头部，包括GC用的CommonHeader和上游变量个数以及垃圾回收列表

typedef struct CClosure {
  ClosureHeader;                               //闭包头部
  lua_CFunction f;                             //C的lua函数
  TValue upvalue[1];  /* list of upvalues */   //被闭包的上游变量数组，对于C函数来说，闭包的变量直接使用TValue封装
} CClosure;


typedef struct LClosure {
  ClosureHeader;                               //闭包头部
  struct Proto *p;                             //Lua原型
  UpVal *upvals[1];  /* list of upvalues */    //被闭包的上游变量，对于Lua原型来说，被闭包的变量有打开和关闭两种状态
} LClosure;

//使用union组合，Lua里对数据类型全部用这种方式做到空间节省和抽象复用的折中
typedef union Closure {
  CClosure c;
  LClosure l;
} Closure;

/*
** Tables
*/

typedef union TKey {
  struct {
    TValuefields;
    int next;  /* for chaining (offset for next node) */
  } nk;//TODO，nk有啥用？下一个有效位置？
  TValue tvk;
} TKey;

//键值对
typedef struct Node {
  TValue i_val;
  TKey i_key;
} Node;


typedef struct Table {
  CommonHeader;                                                        //GC公共头部
  lu_byte flags;  /* 1<<p means tagmethod(p) is not present */         //表的flags，TODO：
  lu_byte lsizenode;  /* log2 of size of `node' array */               //表长度的log2值，原因是动态增长的时候是几何级数增加？
  struct Table *metatable;                                             //元表，用以操作符重载，发现string类型没有元表，貌似基本类型的元表另外定义
  TValue *array;  /* array part */                                     //Lua的表被分成数组段和Hash字典段
  Node *node;                                                          //Hash链表
  Node *lastfree;  /* any free position is before this position */     //最后一个自由位置，所有的自由位置都是在这个位置之前
  GCObject *gclist;                                                    //TODO，为啥Proto、Closure、Table都需要一个gclist？
  int sizearray;  /* size of `array' array */                          //数组部分的长度
} Table;

/*
** Union of all collectable objects
*/
union GCObject {
  GCheader gch;  /* common header */           //这个地方不能用CommonHeader，因为外层是个union，所以必须用struct吧CommonHeader包在里面
  union TString ts;                            //字符串类型
  union Udata u;                               //用户数据类型
  union Closure cl;                            //闭包类型
  struct Table h;                              //表类型
  struct Proto p;                              //原型类型，原型即可以够成Closure的一部分，也可以独立使用，所以Closure和Proto都有gc头部和gclist
  struct lua_State th;  /* thread */           //线程类型
};
```

LuaState表示一个Lua Thead状态，global_State是全局共享的State，global_State里拥有一个mainThread,Lua_State里拥有当前调用函数信息，所有的Lua函数调用都在同一个线程（LuaState）上运行按栈的方式，类似汇编的函数帧。
```

typedef struct stringtable {
  TString **hash;                                //字符串Hash数组
  int nuse;  /* number of elements */            //已用长度
  int size;                                      //总长度
} stringtable;

/*
** information about a call
*/
typedef struct CallInfo {
  StkId func;  /* function index in the stack */                                  //函数在栈里的起始位置
  StkId	top;  /* top for this function */                                         //函数的顶部位置
  struct CallInfo *previous, *next;  /* dynamic call link */                      //上一个和下一个函数调用信息，构成双向链表
  short nresults;  /* expected number of results from this function */            //返回值个数
  lu_byte callstatus;                                                             //调用状态，TODO：
  ptrdiff_t extra;                                                                //额外字段，TODO：
  union {                 
    struct {  /* only for Lua functions */
      StkId base;  /* base for this function */                                   //Lua函数只要指明Proto或者LClosure在栈里的位置
      const Instruction *savedpc;                                                 //以及保存的指令指针即可
    } l;                                                                          //Lua函数
    struct {  /* only for C functions */
      int ctx;  /* context info. in case of yields */                             //C函数，需要上下文信息，以支持yields 
      lua_CFunction k;  /* continuation in case of yields */                      //如果发生yield，TODO
      ptrdiff_t old_errfunc;                                                      //错误处理函数
      lu_byte old_allowhook;                                                      //是否允许hook
      lu_byte status;                                                             //C函数状态，TODO：
    } c;
  } u;
} CallInfo;

/*
** `per thread' state
*/
struct lua_State {
  CommonHeader;                                                                  //GC头部
  lu_byte status;                                                                //状态
  StkId top;  /* first free slot in the stack */                                 //栈顶（栈的第一个可用位置）
  global_State *l_G;                                                             //全局共享的State
  CallInfo *ci;  /* call info for current function */                            //当前正在运行的函数的信息
  const Instruction *oldpc;  /* last pc traced */                                //上一次指令指针
  StkId stack_last;  /* last free slot in the stack */                           //栈的最后一个可用位置
  StkId stack;  /* stack base */                                                 //栈底
  int stacksize;                                                                 //栈的大小
  unsigned short nny;  /* number of non-yieldable calls in stack */              //栈上不可以yiled的函数的个数，TODO：
  unsigned short nCcalls;  /* number of nested C calls */                        //嵌套的c函数调用个数
  lu_byte hookmask;                                                              //hook蒙板，TODO：
  lu_byte allowhook;                                                             //允许hook
  int basehookcount;                                                             //基本hook个数
  int hookcount;                                                                 //hook总数
  lua_Hook hook;                                                                 //hook函数
  UpVal *openupval;  /* list of open upvalues in this stack */                   //打开的UpValue链表
  GCObject *gclist;                                                              //gclist
  struct lua_State *twups;  /* list of threads with open upvalues */             //拥有open upvalue的子lua_State链表数组
  struct lua_longjmp *errorJmp;  /* current error recover point */               //错误处理跳转点
  ptrdiff_t errfunc;  /* current error handling function (stack index) */        //错误处理函数
  CallInfo base_ci;  /* CallInfo for first level (C calling Lua) */              //调用栈的根信息
};

/*
** `global state', shared by all threads of this state
*/
typedef struct global_State {
  lua_Alloc frealloc;  /* function to reallocate memory */                        //内存分配器
  void *ud;         /* auxiliary data to `frealloc' */                            //内存分配器的参数（上下文）
  lu_mem totalbytes;  /* number of bytes currently allocated - GCdebt */          //已经分配的总字节数
  l_mem GCdebt;  /* bytes allocated not yet compensated by the collector */       //已经分配，但未偿还的字节数
  lu_mem GCmemtrav;  /* memory traversed by the GC */                             //被GC遍历到的总字节数
  lu_mem GCestimate;  /* an estimate of the non-garbage memory in use */          //未垃圾回收字节数估计
  stringtable strt;  /* hash table for strings */                                 //字符串Hash表
  TValue l_registry;                                                              //寄存器
  unsigned int seed;  /* randomized seed for hashes */                            //随机种子
  lu_byte currentwhite;                                                           //TODO：
  lu_byte gcstate;  /* state of garbage collector */                              //垃圾收集器的状态
  lu_byte gckind;  /* kind of GC running */                                       //垃圾收集器的种类
  lu_byte gcrunning;  /* true if GC is running */                                 //GC是否正在跑
  GCObject *allgc;  /* list of all collectable objects */                         //所有可被垃圾回收的对象数组
  GCObject **sweepgc;  /* current position of sweep in list */                    //正在扫描的位置
  GCObject *finobj;  /* list of collectable objects with finalizers */            //拥有finalizer的可回收对象数组
  GCObject *gray;  /* list of gray objects */                                     //标记为灰色的对象数组
  GCObject *grayagain;  /* list of objects to be traversed atomically */          //TODO：
  GCObject *weak;  /* list of tables with weak values */                          //带有弱引用值的表的数组
  GCObject *ephemeron;  /* list of ephemeron tables (weak keys) */                //带有弱引用键的表的数组
  GCObject *allweak;  /* list of all-weak tables */                               //所有的弱引用表数组
  GCObject *tobefnz;  /* list of userdata to be GC */                             //可以被GC的UserData数组
  GCObject *fixedgc;  /* list of objects not to be collected */                   //不可被垃圾回收的对象数组
  struct lua_State *twups;  /* list of threads with open upvalues */              //带有Open UpValues的lua_State对象数组
  Mbuffer buff;  /* temporary buffer for string concatenation */                  //字符串拼接的临时缓存
  unsigned int gcfinnum;  /* number of finalizers to call in each GC step */      //每次gc的时候需要调用的finalizer个数
  int gcpause;  /* size of pause between successive GCs */                        //两次gc之间的时间间隔
  int gcstepmul;  /* GC `granularity' */                                          //gc的粒度
  lua_CFunction panic;  /* to be called in unprotected errors */                  //未处理错误的处理回调
  struct lua_State *mainthread;                                                   //主线程
  const lua_Number *version;  /* pointer to version number */                     //版本号
  TString *memerrmsg;  /* memory-error message */                                 //内存错误信息
  TString *tmname[TM_N];  /* array with tag-method names */                       //tag-methods的名字数组
  struct Table *mt[LUA_NUMTAGS];  /* metatables for basic types */                //基本类型的元表
} global_State;

```

#### 大写转小写
---------------

lctype.h是lua对ctype.h的优化

```
/*
** this 'ltolower' only works for alphabetic characters
*/
#define ltolower(c)	((c) | ('A' ^ 'a'))
```

`A^a`得到的是大写字符和小写字符之间的差值，刚好是32位，二进制表示是`100000`，而小写字符是在`65`到`97`之间，`65`的二进制表示是`1000001`,`96`的表示是`1100000`，也就是说`65`到`97`之间的二进制表示，在二进制的第6位刚好都是0，而第6位如果改为1则刚好是这些数字加32的结果。所以上面的`ltolower`只要将字符和32做或操作就可以将其值加32，而用`A^a`表示32也是可以理解的，`A`和`a`的二进制只在第6位不同，这个位的二进制值刚好是32。真是黑科技啊。

#### 判断ASCII字符类型
----------------------

lua提供了几个宏用来判断ASCII字符的子类型：
```
#define ALPHABIT	0
#define DIGITBIT	1
#define PRINTBIT	2
#define SPACEBIT	3
#define XDIGITBIT	4


#define MASK(B)		(1 << (B))


/*
** add 1 to char to allow index -1 (EOZ)
*/
#define testprop(c,p)	(luai_ctype_[(c)+1] & (p))

/*
** 'lalpha' (Lua alphabetic) and 'lalnum' (Lua alphanumeric) both include '_'
*/
#define lislalpha(c)	testprop(c, MASK(ALPHABIT))
#define lislalnum(c)	testprop(c, (MASK(ALPHABIT) | MASK(DIGITBIT)))
#define lisdigit(c)	testprop(c, MASK(DIGITBIT))
#define lisspace(c)	testprop(c, MASK(SPACEBIT))
#define lisprint(c)	testprop(c, MASK(PRINTBIT))
#define lisxdigit(c)	testprop(c, MASK(XDIGITBIT))
```
这几个宏的基本作用就是，通过luai_ctype_数组获取字符所对应的类型，然后和字符的类型枚举做与（&）操作来判断。这里利用了一个特点，即ASCII字符总是从0开始排到255，所以可以直接把c用来做数组的下标。当然，为了支持值为-1（溢出）的查找，数组第一个元素为0，所以真正查找的时候的下标是(c+1),因为c的范围本来是0-256,加上-1，最终的范围是(0,257).我们来看下luai_ctype_数组：
```
LUAI_DDEF const lu_byte luai_ctype_[UCHAR_MAX + 2] = {
  0x00,  /* EOZ */
  0x00,  0x00,  0x00,  0x00,  0x00,  0x00,  0x00,  0x00,	/* 0. */
  0x00,  0x08,  0x08,  0x08,  0x08,  0x08,  0x00,  0x00,
  0x00,  0x00,  0x00,  0x00,  0x00,  0x00,  0x00,  0x00,	/* 1. */
  0x00,  0x00,  0x00,  0x00,  0x00,  0x00,  0x00,  0x00,
  0x0c,  0x04,  0x04,  0x04,  0x04,  0x04,  0x04,  0x04,	/* 2. */
  0x04,  0x04,  0x04,  0x04,  0x04,  0x04,  0x04,  0x04,
  0x16,  0x16,  0x16,  0x16,  0x16,  0x16,  0x16,  0x16,	/* 3. */
  0x16,  0x16,  0x04,  0x04,  0x04,  0x04,  0x04,  0x04,
  0x04,  0x15,  0x15,  0x15,  0x15,  0x15,  0x15,  0x05,	/* 4. */
  0x05,  0x05,  0x05,  0x05,  0x05,  0x05,  0x05,  0x05,
  0x05,  0x05,  0x05,  0x05,  0x05,  0x05,  0x05,  0x05,	/* 5. */
  0x05,  0x05,  0x05,  0x04,  0x04,  0x04,  0x04,  0x05,
  0x04,  0x15,  0x15,  0x15,  0x15,  0x15,  0x15,  0x05,	/* 6. */
  0x05,  0x05,  0x05,  0x05,  0x05,  0x05,  0x05,  0x05,
  0x05,  0x05,  0x05,  0x05,  0x05,  0x05,  0x05,  0x05,	/* 7. */
  0x05,  0x05,  0x05,  0x04,  0x04,  0x04,  0x04,  0x00,
  0x00,  0x00,  0x00,  0x00,  0x00,  0x00,  0x00,  0x00,	/* 8. */
  0x00,  0x00,  0x00,  0x00,  0x00,  0x00,  0x00,  0x00,
  0x00,  0x00,  0x00,  0x00,  0x00,  0x00,  0x00,  0x00,	/* 9. */
  0x00,  0x00,  0x00,  0x00,  0x00,  0x00,  0x00,  0x00,
  0x00,  0x00,  0x00,  0x00,  0x00,  0x00,  0x00,  0x00,	/* a. */
  0x00,  0x00,  0x00,  0x00,  0x00,  0x00,  0x00,  0x00,
  0x00,  0x00,  0x00,  0x00,  0x00,  0x00,  0x00,  0x00,	/* b. */
  0x00,  0x00,  0x00,  0x00,  0x00,  0x00,  0x00,  0x00,
  0x00,  0x00,  0x00,  0x00,  0x00,  0x00,  0x00,  0x00,	/* c. */
  0x00,  0x00,  0x00,  0x00,  0x00,  0x00,  0x00,  0x00,
  0x00,  0x00,  0x00,  0x00,  0x00,  0x00,  0x00,  0x00,	/* d. */
  0x00,  0x00,  0x00,  0x00,  0x00,  0x00,  0x00,  0x00,
  0x00,  0x00,  0x00,  0x00,  0x00,  0x00,  0x00,  0x00,	/* e. */
  0x00,  0x00,  0x00,  0x00,  0x00,  0x00,  0x00,  0x00,
  0x00,  0x00,  0x00,  0x00,  0x00,  0x00,  0x00,  0x00,	/* f. */
  0x00,  0x00,  0x00,  0x00,  0x00,  0x00,  0x00,  0x00,
};
```
可以看到，直接在对应字符的下标+1所在位置设置了其类型的值，一共有7种：
```
 - 0x04       对应的二进制为   00000100  
 - 0x16       对应的二进制为   00010110
 - 0x15       对应的二进制为   00010101
 - 0x05       对应的二进制为   00000101
 - 0x08       对应的二进制为   00001000
 - 0x0c       对应的二进制为   00001100
 - 0x00       对应的二进制为   00000000
```
其实这里的每个值都是等于对应位置的ASCII字符的子类型的枚举(ALPHABIT DIGITBIT PRINTBIT SPACEBIT XDIGITBIT)的或操作的结果。看起来复杂，实际上就是查表求Flag再做判断。


#### var_arg
------------
```
#define va_start _crt_va_start
#define va_arg _crt_va_arg
#define va_end _crt_va_end
```

##### X86的一个实现如下：

```
#elif   defined(_M_IX86)
#define _INTSIZEOF(n)   ( (sizeof(n) + sizeof(int) - 1) & ~(sizeof(int) - 1) )
#define _crt_va_start(ap,v)  ( ap = (va_list)_ADDRESSOF(v) + _INTSIZEOF(v) )
#define _crt_va_arg(ap,t)    ( *(t *)((ap += _INTSIZEOF(t)) - _INTSIZEOF(t)) )
#define _crt_va_end(ap)      ( ap = (va_list)0 )
```

- _crt_va_start是获取fmt后面的变长参数内存的buffer，
- _crt_va_arg就只是把fmt后面的buffer进行挨个decode，得到fmt里的%号后面的字符所指示的类型的字节，让好ap自增
- _crg_va_end则是把ap指针设置为NUL
- 在其他平台下（非x86），可能因为存在对齐而使得ap自增的时候需要增加对齐部分的长度
- 使用_INTSIZEOF而不是sizeof，这是为了对齐：

[_INTSIZEOF解释](http://www.cnblogs.com/diyunpeng/archive/2010/01/09/1643160.html)

1. 我们知道对于IX86，sizeof(int)一定是4的整数倍，所以~(sizeof(int) - 1) )的值一定是
右面［sizeof(n)-1］/2位为0，整个这个宏也就是保证了右面［sizeof(n)-1］/2位为0，其余位置
为1，所以_INTSIZEOF(n)的值只有可能是4，8，16,......等等，实际上是实现了字节对齐。

2. `#define _INTSIZEOF(n)  ((sizeof(n)+sizeof(int)-1)&~(sizeof(int) - 1) )`
的目的在于把sizeof(n)的结果变成至少是sizeof(int)的整倍数，这个一般用来在结构中实现按int的倍数对齐。
如果sizeof(int)是4，那么，当sizeof(n)的结果在1~4之间是，_INTSIZEOF(n)的结果会是4；当sizeof(n)的结果在5~8时，
_INTSIZEOF(n)的结果会是8；当sizeof(n)的结果在9~12时，_INTSIZEOF(n)的结果会是12；……总之，会是sizeof(int)的倍数。

##### 理论基础：

对于两个正整数 x, n 总存在整数 q, r 使得
x = nq + r, 其中  0<= r <n                  //最小非负剩余
q, r 是唯一确定的。q = [x/n], r = x - n[x/n]. 这个是带余除法的一个简单形式。在 c 语言中， q, r 容易计算出来： q = x/n, r = x % n.
    
所谓把 x 按 n 对齐指的是：若 r=0, 取 qn, 若 r>0, 取 (q+1)n. 这也相当于把 x 表示为：
x = nq + r', 其中 -n < r' <=0                //最大非正剩余   
nq 是我们所求。关键是如何用 c 语言计算它。由于我们能处理标准的带余除法，所以可以把这个式子转换成一个标准的带余除法，然后加以处理：
x+n = qn + (n+r')，其中 0<n+r'<=n            //最大正剩余
x+n-1 = qn + (n+r'-1), 其中 0<= n+r'-1 <n    //最小非负剩余
所以 qn = [(x+n-1)/n]n. 用 c 语言计算就是：
((x+n-1)/n)*n
若 n 是 2 的方幂, 比如 2^m，则除为右移 m 位，乘为左移 m 位。所以把 x+n-1 的最低 m 个二进制位清 0就可以了。得到：
(x+n-1) & (~(n-1))



