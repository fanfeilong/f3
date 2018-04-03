在上一篇里，我们知道一个Lua的对象包含对象的值和对象的类型信息。回顾一下：
```
typedef struct lua_TValue {
  Value value_;  
  int tt_;
}TValue;
```
其中，`tt_`是tag type的简写，一共包含三个部分，分别是：
- bit位0-3表示大类型
- bit位4-5表示小类型
- bit位6表示是否可以垃圾回收

显然，既然Lua的所有类型统一用TValue表示，TValue就应该具备一些基本的操作，用来存取类型示例的基本属性。从Lua的源码来看，可以把TValue的基本操作归结如下：
1. 判断对象的具体类型
2. 获取对象的值
3. 设置对象的值

如果用ADT来表示，可以写成：
```
TValue:
   get_type()
   get_value()
   set_value()
```
我们逐个来看。
# get type
Lua里，用了一组宏来判断TValue的具体类型，首先是四个萃取`tt_`信息的宏:
```
/* raw type tag of a TValue */
#define rttype(o)	((o)->tt_)

/* tag with no variants (bits 0-3) */
#define novariant(x)	((x) & 0x0F)

/* type tag of a TValue (bits 0-3 for tags + variant bits 4-5) */
#define ttype(o)	(rttype(o) & 0x3F)

/* type tag of a TValue with no variants (bits 0-3) */
#define ttnov(o)	(novariant(rttype(o)))
```
其中norariant是直接操作`tt_`字段的，其他三个参数都是TValue对象，我们可以去掉novariant：
```
#define rttype(o)	((o)->tt_)
#define ttype(o)	 (rttype(o) & 0x3F)
#define ttnov(o)	 (rttype(o) & 0x0F)
```
进一步，还可以把rttype展开：
```
#define rttype(o)	((o)->tt_)
#define ttype(o)	 ((o)->tt_ & 0x3F) 
#define ttnov(o)	 ((o)->tt_ & 0x0F)
```
Lua并没有针对tt_三个部分的萃取都单独提供宏，萃取4-5bit以及萃取第6个bit由于后面各只被用到一次，所以不必单独提供一个宏。当然，如果单独提供，则会让api看上去更完备和对称。

进一步，Lua提供了两个基本的check：
```
#define checktag(o,t)	(rttype(o) == (t))
#define checktype(o,t)   (ttnov(o) == (t))
```

现在，就可以对每种TValue类型做判断：
```
/* Macros to test type */
#define ttisnumber(o)		  checktype((o), LUA_TNUMBER)
#define ttisfloat(o)		   checktag((o), LUA_TNUMFLT)
#define ttisinteger(o)		 checktag((o), LUA_TNUMINT)
#define ttisnil(o)		     checktag((o), LUA_TNIL)
#define ttisboolean(o)		 checktag((o), LUA_TBOOLEAN)
#define ttislightuserdata(o)   checktag((o), LUA_TLIGHTUSERDATA)
#define ttisstring(o)		  checktype((o), LUA_TSTRING)
#define ttisshrstring(o)	   checktag((o), ctb(LUA_TSHRSTR))
#define ttislngstring(o)	   checktag((o), ctb(LUA_TLNGSTR))
#define ttistable(o)		   checktag((o), ctb(LUA_TTABLE))
#define ttisfunction(o)	    checktype(o, LUA_TFUNCTION)
#define ttisclosure(o)		 ((rttype(o) & 0x1F) == LUA_TFUNCTION)
#define ttisCclosure(o)		checktag((o), ctb(LUA_TCCL))
#define ttisLclosure(o)		checktag((o), ctb(LUA_TLCL))
#define ttislcf(o)		     checktag((o), LUA_TLCF)
#define ttisfulluserdata(o)	checktag((o), ctb(LUA_TUSERDATA))
#define ttisthread(o)		  checktag((o), ctb(LUA_TTHREAD))
#define ttisdeadkey(o)		 checktag((o), LUA_TDEADKEY)
```
实际上，我们可以针对上一篇的类型列表做一个缩进：
```
#define ttisnil(o)               checktag((o), LUA_TNIL)
#define ttislightuserdata(o)     checktag((o), LUA_TLIGHTUSERDATA)
#define ttisboolean(o)           checktag((o), LUA_TBOOLEAN)
#define ttisnumber(o)            checktype((o), LUA_TNUMBER) // 1          
    #define ttisfloat(o)         checktag((o), LUA_TNUMFLT)
    #define ttisinteger(o)       checktag((o), LUA_TNUMINT)
#define ttisfunction(o)          checktype(o, LUA_TFUNCTION) // 2
    #define ttislcf(o)           checktag((o), LUA_TLCF)
    #define ttisclosure(o)       ((rttype(o) & 0x1F) == LUA_TFUNCTION) // **4**
        #define ttisCclosure(o)  checktag((o), ctb(LUA_TCCL))
        #define ttisLclosure(o)  checktag((o), ctb(LUA_TLCL))
#define ttisstring(o)            checktype((o), LUA_TSTRING) // 3
    #define ttisshrstring(o)     checktag((o), ctb(LUA_TSHRSTR))
    #define ttislngstring(o)     checktag((o), ctb(LUA_TLNGSTR))
#define ttisfulluserdata(o)      checktag((o), ctb(LUA_TUSERDATA))
#define ttistable(o)             checktag((o), ctb(LUA_TTABLE))
#define ttisthread(o)            checktag((o), ctb(LUA_TTHREAD))
#define ttisdeadkey(o)           checktag((o), LUA_TDEADKEY)
```
可以看到，1,2,3三个地方判断大类型，用的是checktype，也就是只需判断0-3bit即可；而**4**这个地方判断则使用的是4-5bit判断小类型。其余的地方使用的是0-6bit；不过还差一点，那就是`cbt`宏。展开看一下：
```
/* Bit mark for collectable types */
#define BIT_ISCOLLECTABLE    (1 << 6)

/* mark a tag as collectable */
#define ctb(t)            ((t) | BIT_ISCOLLECTABLE)
```
可见，`ctb`宏的作用是设置6个bit，表示这是一个可垃圾回收类型。此外，针对GCObject，Lua还提供了一个判断TValue里的tag type和GCObject里的tag  type是否一致的判断：
```
/* Macros for internal tests */
#define righttt(obj)		(ttype(obj) == gcvalue(obj)->tt)
```

# get value
获取无类型信息的Value：
```
#define val_(o)		((o)->value_)
```

进一步，根据上一节的类型判断，就可以实现带类型校验的值获取方法，依然是一组宏：
```
/* Macros to access values */
#define ivalue(o)    check_exp(ttisinteger(o), val_(o).i)
#define fltvalue(o)  check_exp(ttisfloat(o), val_(o).n)
#define nvalue(o)    check_exp(ttisnumber(o), \
   (ttisinteger(o) ? cast_num(ivalue(o)) : fltvalue(o)))
#define gcvalue(o)   check_exp(iscollectable(o), val_(o).gc)
#define pvalue(o)    check_exp(ttislightuserdata(o), val_(o).p)
#define tsvalue(o)   check_exp(ttisstring(o), gco2ts(val_(o).gc))
#define uvalue(o)    check_exp(ttisfulluserdata(o), gco2u(val_(o).gc))
#define clvalue(o)   check_exp(ttisclosure(o), gco2cl(val_(o).gc))
#define clLvalue(o)  check_exp(ttisLclosure(o), gco2lcl(val_(o).gc))
#define clCvalue(o)  check_exp(ttisCclosure(o), gco2ccl(val_(o).gc))
#define fvalue(o)    check_exp(ttislcf(o), val_(o).f)
#define hvalue(o)    check_exp(ttistable(o), gco2t(val_(o).gc))
#define bvalue(o)    check_exp(ttisboolean(o), val_(o).b)
#define thvalue(o)   check_exp(ttisthread(o), gco2th(val_(o).gc))

/* a dead value may get the 'gc' field, but cannot access its contents */
#define deadvalue(o)   check_exp(ttisdeadkey(o), cast(void *, val_(o).gc))

#define l_isfalse(o)   (ttisnil(o) || (ttisboolean(o) && bvalue(o) == 0))

#define iscollectable(o)   (rttype(o) & BIT_ISCOLLECTABLE)
```
注意到，里面用到了几个`gco2x`的宏，这组宏的作用是把GCObject的子类型转为更具体的类型，再次展开：
```
#define cast_u(o)	cast(union GCUnion *, (o))

/* macros to convert a GCObject into a specific value */
#define gco2ts(o)  \
	check_exp(novariant((o)->tt) == LUA_TSTRING, &((cast_u(o))->ts))
#define gco2u(o)  check_exp((o)->tt == LUA_TUSERDATA, &((cast_u(o))->u))
#define gco2lcl(o)  check_exp((o)->tt == LUA_TLCL, &((cast_u(o))->cl.l))
#define gco2ccl(o)  check_exp((o)->tt == LUA_TCCL, &((cast_u(o))->cl.c))
#define gco2cl(o)  \
	check_exp(novariant((o)->tt) == LUA_TFUNCTION, &((cast_u(o))->cl))
#define gco2t(o)  check_exp((o)->tt == LUA_TTABLE, &((cast_u(o))->h))
#define gco2p(o)  check_exp((o)->tt == LUA_TPROTO, &((cast_u(o))->p))
#define gco2th(o)  check_exp((o)->tt == LUA_TTHREAD, &((cast_u(o))->th))
```
出现了一个新的联合体:`GCUnion`，这个是啥？展开看：
```
/*
** Union of all collectable objects (only for conversions)
*/
union GCUnion {
  GCObject gc;  /* common header */
  struct TString ts;
  struct Udata u;
  union Closure cl;
  struct Table h;
  struct Proto p;
  struct lua_State th;  /* thread */
};
```
哦，原来这个是上一篇的补丁。所有的GCObject都有共同的头部，也就是CommonHeader，所以可以把GCObject转为GCUnion之后，再转为具体类型的指针，安全性由check_exp保证。

反之，又具体类型转为GCObject也应该提供：
```
/* macro to convert a Lua object into a GCObject */
#define obj2gco(v) \
	check_exp(novariant((v)->tt) < LUA_TDEADKEY, (&(cast_u(v)->gc)))
```

# set value
显然，设置TValue的值，应该同时设置Value和tag type。
```
/* Macros to set values */
#define settt_(o,t)	((o)->tt_=(t))  // 必须每次赋值都重新设置具体类型

#define setfltvalue(obj,x) \
  { TValue *io=(obj); val_(io).n=(x); settt_(io, LUA_TNUMFLT); }

#define setivalue(obj,x) \
  { TValue *io=(obj); val_(io).i=(x); settt_(io, LUA_TNUMINT); }

#define setnilvalue(obj) settt_(obj, LUA_TNIL)

#define setfvalue(obj,x) \
  { TValue *io=(obj); val_(io).f=(x); settt_(io, LUA_TLCF); }

#define setpvalue(obj,x) \
  { TValue *io=(obj); val_(io).p=(x); settt_(io, LUA_TLIGHTUSERDATA); }

#define setbvalue(obj,x) \
  { TValue *io=(obj); val_(io).b=(x); settt_(io, LUA_TBOOLEAN); }

#define setgcovalue(L,obj,x) \
  { TValue *io = (obj); GCObject *i_g=(x); \
    val_(io).gc = i_g; settt_(io, ctb(i_g->tt)); }

#define setsvalue(L,obj,x) \
  { TValue *io = (obj); TString *x_ = (x); \
    val_(io).gc = obj2gco(x_); settt_(io, ctb(x_->tt)); \
    checkliveness(G(L),io); }

#define setuvalue(L,obj,x) \
  { TValue *io = (obj); Udata *x_ = (x); \
    val_(io).gc = obj2gco(x_); settt_(io, ctb(LUA_TUSERDATA)); \
    checkliveness(G(L),io); }

#define setthvalue(L,obj,x) \
  { TValue *io = (obj); lua_State *x_ = (x); \
    val_(io).gc = obj2gco(x_); settt_(io, ctb(LUA_TTHREAD)); \
    checkliveness(G(L),io); }

#define setclLvalue(L,obj,x) \
  { TValue *io = (obj); LClosure *x_ = (x); \
    val_(io).gc = obj2gco(x_); settt_(io, ctb(LUA_TLCL)); \
    checkliveness(G(L),io); }

#define setclCvalue(L,obj,x) \
  { TValue *io = (obj); CClosure *x_ = (x); \
    val_(io).gc = obj2gco(x_); settt_(io, ctb(LUA_TCCL)); \
    checkliveness(G(L),io); }

#define sethvalue(L,obj,x) \
  { TValue *io = (obj); Table *x_ = (x); \
    val_(io).gc = obj2gco(x_); settt_(io, ctb(LUA_TTABLE)); \
    checkliveness(G(L),io); }

#define setdeadvalue(obj)	settt_(obj, LUA_TDEADKEY)


#define setobj(L,obj1,obj2) \
	{ TValue *io1=(obj1); *io1 = *(obj2); \
	  (void)L; checkliveness(G(L),io1); }
```

这组宏在内部都先重新引用下参数再操作，我想是为了避免宏展开的副作用，到处加小括号，例如：
```
#define setsvalue(L,obj,x) \
  { TValue *io = (obj); TString *x_ = (x); \
    val_(io).gc = obj2gco(x_); settt_(io, ctb(x_->tt)); \
    checkliveness(G(L),io); }
```
这里把`obj`和`x`分别先赋值给`io`和`x_`，再使用，但下面的x_则因为只用了一次就没做这个赋值动作：
```
#define setivalue(obj,x) \
  { TValue *io=(obj); val_(io).i=(x); settt_(io, LUA_TNUMINT); }
```
此外，这组宏里面，对GCObject，都要通过obj2gco把具体类型转型后再赋值给val_(io).gc字段。

# 待续
下一篇希望分析下Lua State对象，也就是所谓的Thread。