# 源码阅读顺序
Lua的源码短小精悍，一般的阅读顺序是建议从外围到内层。例如，下面这个顺序是reddit上的一篇帖子的推荐：

[Recommended reading order](http://www.reddit.com/comments/63hth/ask_reddit_which_oss_codebases_out_there_are_so/c02pxbp)
> If you're done before X-Mas and understood all of it, you're good. The information density of the code is rather high.

- ` lmathlib.c`,` lstrlib.c`: get familiar with the external C API. Don't bother with the pattern matcher though. Just the easy functions.
-  `lapi.c`: Check how the API is implemented internally. Only skim this to get a feeling for the code. Cross-reference to lua.h and luaconf.h as needed.
-  `lobject.h`: tagged values and object representation. skim through this first. you'll want to keep a window with this file open all the time.
-  `lstate.h`: state objects. ditto.
-  `lopcodes.h`: bytecode instruction format and opcode definitions. easy.
-  `lvm.c`: scroll down to luaV_execute, the main interpreter loop. see how all of the instructions are implemented. skip the details for now. reread later.
-  `ldo.c`: calls, stacks, exceptions, coroutines. tough read.
-  `lstring.c`: string interning. cute, huh?
-  `ltable.c`: hash tables and arrays. tricky code.
-  `ltm.c`: metamethod handling, reread all of lvm.c now.
- You may want to reread `lapi.c` now.
- `ldebug.c`: surprise waiting for you. abstract interpretation is used to find object names for tracebacks. does bytecode verification, too.
- `lparser.c`,` lcode.c`: recursive descent parser, targetting a register-based VM. start from chunk() and work your way through. read the expression parser and the code generator parts last.
- `lgc.c`: incremental garbage collector. take your time.
- Read all the other files as you see references to them. Don't let your stack get too deep though.

我并没有按这个顺序读。外围的API在使用Lua以及给Lua写扩展库的过程中，已经比较熟悉了，C 的 API 在我看来都是按需查文档的事情，所以`lmath`和`lapi.h`这两部分并需要特地去每个看。所以也是直接到`lobject.h`和`lstate.h`来的。

其实，从`lobject`和`lstate`出发，我觉的是很有道理的。因为：**程序=算法+数据结构**。所以我们要先从梳理数据结构开始，这也是为什么在前几篇里，我总是有意略掉某个模块的具体细节。等把数据结构的轮廓整体大致理清之后，再把每个部分的方法部分展开，就变成体力活。当然一个语言在发展演化过程中，肯定不是一开始就是这样的，我们这样的做法，好像这些结构的设计是“天然的”，“一开始就想的非常周全”似的，所以有时候会拼命为某个字段为什么需要，为什么在那个地方给找理由，但实际的开发过程可能是经过N个版本和补丁后才变成那样的。

到这篇，我们开始看global_State对象，不过我依然会只做到梳理下脉络，而不展开细节。

# global_State
上一篇提到，在lua_State对象里有一个global_State，这个才是Lua 状态机的全局状态的存储的地方，所有的lua_State共享这个全局状态，由于Lua被设计为单线程的，所以global_State上的状态控制变的简单很多，完全不考虑多线程问题。

```
/*
** 'global state', shared by all threads of this state
*/
typedef struct global_State {
  lua_Alloc frealloc;  /* function to reallocate memory */
  void *ud;         /* auxiliary data to 'frealloc' */
  lu_mem totalbytes;  /* number of bytes currently allocated - GCdebt */
  l_mem GCdebt;  /* bytes allocated not yet compensated by the collector */
  lu_mem GCmemtrav;  /* memory traversed by the GC */
  lu_mem GCestimate;  /* an estimate of the non-garbage memory in use */
  stringtable strt;  /* hash table for strings */
  TValue l_registry;
  unsigned int seed;  /* randomized seed for hashes */
  lu_byte currentwhite;
  lu_byte gcstate;  /* state of garbage collector */
  lu_byte gckind;  /* kind of GC running */
  lu_byte gcrunning;  /* true if GC is running */
  GCObject *allgc;  /* list of all collectable objects */
  GCObject **sweepgc;  /* current position of sweep in list */
  GCObject *finobj;  /* list of collectable objects with finalizers */
  GCObject *gray;  /* list of gray objects */
  GCObject *grayagain;  /* list of objects to be traversed atomically */
  GCObject *weak;  /* list of tables with weak values */
  GCObject *ephemeron;  /* list of ephemeron tables (weak keys) */
  GCObject *allweak;  /* list of all-weak tables */
  GCObject *tobefnz;  /* list of userdata to be GC */
  GCObject *fixedgc;  /* list of objects not to be collected */
  struct lua_State *twups;  /* list of threads with open upvalues */
  Mbuffer buff;  /* temporary buffer for string concatenation */
  unsigned int gcfinnum;  /* number of finalizers to call in each GC step */
  int gcpause;  /* size of pause between successive GCs */
  int gcstepmul;  /* GC 'granularity' */
  lua_CFunction panic;  /* to be called in unprotected errors */
  struct lua_State *mainthread;
  const lua_Number *version;  /* pointer to version number */
  TString *memerrmsg;  /* memory-error message */
  TString *tmname[TM_N];  /* array with tag-method names */
  struct Table *mt[LUA_NUMTAGS];  /* metatables for basic types */
} global_State;
```

这么多字段，即使加了一堆注释，看上去依然是不直观的。怎样算直观的呢？我觉的能比较清晰看出层级关系的代码比较直观，层级关系一般就是代表了一个对象是由哪些子模块构成的，这是符合人类的线性直觉的吧？姑且怎么做：

```
/*
** 'global state', shared by all threads of this state
*/
typedef struct global_State {
  // Version
  const lua_Number *version;      /* pointer to version number */

  // Hash
  unsigned int seed;              /* randomized seed for hashes */

  // Registry
  TValue l_registry;

  // String table
  stringtable strt;               /* hash table for strings */
  Mbuffer buff;                   /* temporary buffer for string concatenation */

  // Meta table
  TString *tmname[TM_N];          /* array with tag-method names */
  struct Table *mt[LUA_NUMTAGS];  /* metatables for basic types */

  // Thread list
  struct lua_State *mainthread;
  struct lua_State *twups;        /* list of threads with open upvalues */
  
  // Error Recover
  lua_CFunction panic;            /* to be called in unprotected errors */

  // Memory Allocator
  lua_Alloc frealloc;             /* function to reallocate memory */
  void *ud;                       /* auxiliary data to 'frealloc' */
  
  // GC
  lu_mem totalbytes;              /* number of bytes currently allocated - GCdebt */
  TString *memerrmsg;             /* memory-error message */
  
  l_mem  GCdebt;                  /* bytes allocated not yet compensated by the collector */
  lu_mem GCmemtrav;               /* memory traversed by the GC */
  lu_mem GCestimate;              /* an estimate of the non-garbage memory in use */

  lu_byte currentwhite;
  lu_byte gcstate;                /* state of garbage collector */
  lu_byte gckind;                 /* kind of GC running */
  lu_byte gcrunning;              /* true if GC is running */

  int gcpause;                    /* size of pause between successive GCs */
  int gcstepmul;                  /* GC 'granularity' */
  unsigned int gcfinnum;          /* number of finalizers to call in each GC step */
  
  GCObject *allgc;                /* list of all collectable objects */
  GCObject *finobj;               /* list of collectable objects with finalizers */
  GCObject *gray;                 /* list of gray objects */
  GCObject *grayagain;            /* list of objects to be traversed atomically */
  GCObject *weak;                 /* list of tables with weak values */
  GCObject *ephemeron;            /* list of ephemeron tables (weak keys) */
  GCObject *allweak;              /* list of all-weak tables */
  GCObject *tobefnz;              /* list of userdata to be GC */
  GCObject *fixedgc;              /* list of objects not to be collected */

  GCObject **sweepgc;             /* current position of sweep in list */

} global_State;

```
分开后，看一下，后面一大票都是垃圾回收相关的。要是一个语言不用处理垃圾回收的话，代码应该会清爽非常多，从这点来看，也许可以整理一个不管垃圾回收版本的Lua源码来看:)。

现在，我们可以进一步把垃圾回收的部分拆开：
```
typedef struct lua_GCInfo{
  lu_mem totalbytes;              /* number of bytes currently allocated - GCdebt */
  TString *memerrmsg;             /* memory-error message */
  
  l_mem  GCdebt;                  /* bytes allocated not yet compensated by the collector */
  lu_mem GCmemtrav;               /* memory traversed by the GC */
  lu_mem GCestimate;              /* an estimate of the non-garbage memory in use */

  lu_byte currentwhite;
  lu_byte gcstate;                /* state of garbage collector */
  lu_byte gckind;                 /* kind of GC running */
  lu_byte gcrunning;              /* true if GC is running */

  int gcpause;                    /* size of pause between successive GCs */
  int gcstepmul;                  /* GC 'granularity' */
  unsigned int gcfinnum;          /* number of finalizers to call in each GC step */
  
  GCObject *allgc;                /* list of all collectable objects */
  GCObject *finobj;               /* list of collectable objects with finalizers */
  GCObject *gray;                 /* list of gray objects */
  GCObject *grayagain;            /* list of objects to be traversed atomically */
  GCObject *weak;                 /* list of tables with weak values */
  GCObject *ephemeron;            /* list of ephemeron tables (weak keys) */
  GCObject *allweak;              /* list of all-weak tables */
  GCObject *tobefnz;              /* list of userdata to be GC */
  GCObject *fixedgc;              /* list of objects not to be collected */

  GCObject **sweepgc;             /* current position of sweep in list */
}GCInfo;
```

从而，global_State就会变的清晰很多：
```

typedef struct global_State {
  // Version
  const lua_Number *version;      /* pointer to version number */

  // Hash
  unsigned int seed;              /* randomized seed for hashes */

  // Global Registry
  TValue l_registry;

  // Global String table
  stringtable strt;               /* hash table for strings */
  Mbuffer buff;                   /* temporary buffer for string concatenation */

  // Global Meta table
  TString *tmname[TM_N];          /* array with tag-method names */
  struct Table *mt[LUA_NUMTAGS];  /* metatables for basic types */

  // Global Thread list
  struct lua_State *mainthread;
  struct lua_State *twups;        /* list of threads with open upvalues */
  
  // Memory Allocator
  lua_Alloc frealloc;             /* function to reallocate memory */
  void *ud;                       /* auxiliary data to 'frealloc' */

  // GC Info
  GCInfo *gcinfo;

  // Error Recover
  lua_CFunction panic;            /* to be called in unprotected errors */
}
```
- registry: 
  - 注册表管理全局数据
- string
  - stringtable: 全局字符串表，几乎每个语言都会对字符串做池化，作成immutable的，Lua的字符串分短字符串和长字符串
  - buff: 在Lua解析(parse)源代码的过程中，以及字符串处理过程中需要的临时缓存
- meta table，其实无论什么语言，所有的魔法都可以归结为“查表”，例如面向对象里的虚函数表，所有的OOP机制都依赖于虚函数表。
  - tmname: metatable的预定义方法名字数组，tm是tag method的缩写
  - mt：每个基本类型一个metatable，注意table、userdata等则是每个实例一个metatable。metatable+tag method可以说是整个Lua最重要的Hook机制。
- thread，当然global_State需要持有所有的线程（协程）。
  - mainthread: 主线程
  - twups: 闭包了当前线程变量的其他线程列表
- memory
  - frealloc: Lua的全局内存分配器，用户可以替换成自己的
  - ud: 分配器的userdata
- gc
  - 垃圾回收所需要的信息特别多，先不管，整个垃圾回收系统应该单独来分析。
- error handle
  - panic: 全局错误处理响应点

其中，stringtable如下：
```
typedef struct stringtable {
  TString **hash;
  int nuse;  /* number of elements */
  int size;
} stringtable;
```

## 待续
  这样我们初步把gloabl_State的轮廓搞清楚了。后面，按global_State的子模块，逐个分析。