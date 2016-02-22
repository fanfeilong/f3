
------------------------------------------------
--// using coroutine to synchronize callback
------------------------------------------------
local post = coroutine.wrap(function(arg)
  local connection = ...
  
  local cookie = listen(connection,arg)
  connection:BeginOpen()

  local error,r = coroutine:yield()
  if r.state=="open" then
      local pkg = r.value
        connection:PostPackage(pkg)
      coroutine:yield()
  elseif r.state=="close" then
      connection:RemoveStateChangeListener(cookie)
      connectionManager:Remove(cookie)
  else 
      assert(false)
  end
end)

function listen(connection,v)
  local self = coroutine.running()
  assert(self)
  
  local cookie = connection:AttachStateChangeListener(function(connection_,oldState,newState,errorCode)
    if newState=="connected" then
         coroutine.resume(self({state="open",value=v}))            
    elseif newState=="closed" then
         coroutine.resume(self({state="close"}))
    end
  end)

  return cookie
end

post(pkg)

------------------------------------------------
--// lua source code: lobject.h
--[0 000 0000]
--[6 4-5 0-3]
--[collectable variant-bits actual-tag]
------------------------------------------------
typedef enum tagLuaType{
  LuaType_None          =-1;
  LuaType_Nil           = 0;
  LuaType_Boolean       = 2;
  LuaType_LightUserData = 3;
  LuaType_String        = 4;
  LuaType_Function      = 5;
  LuaType_UserData      = 7;
  LuaType_Thread        = 8;
  LuaType_NumTags       = 9;
}LuaType;

typedef enum tagLuaFunctionType{
  LuaFunctionType_LuaClosure     = 0<<4;
  LuaFunctionType_LightCFunction = 1<<4;
  LuaFunctionType_CClosure       = 2<<4;
}LuaFunctionType; 

typedef enum tagLuaStringType{
  LuaStringType_Short = 0<<4;
  LuaStringType_Long  = 1<<4; 
}LuaStringType;


typedef union tagValue{
  GCObject* gc;
  void* p;
  int b;
  lua_CFunction f;
  lua_Integer i;
  lua_Number n;   
}Value;

#define TValuefields Value value_;int tt_
typedef struct taglua_TValue{
  TValuefields; // Value value_;int tt_; 
}TValue;
typedef TValue* StkId;


#define CommonHeader GCObject* next;lua_byte tt; lua_byte marked
typedef struct GCObject{
  CommonHeader; // GCObject* next;lua_byte tt; lua_byte marked;
};

typedef struct TString{
  CommonHeader; // GCObject* next;lua_byte tt; lua_byte marked;
  lua_byte extra;
  unsigned int hash;
  size_t len;
  struct TString* hnext;
}TString;

typedef union { double u; void *s; lua_Integer i; long l; } L_Umaxalign;
typedef union UTString{
  L_Umaxalign dummy;
  TString tsv; 
}UTString;

typedef struct Udata{
  CommonHeader;
  lua_byte ttuv_;// user value's tag
  struct Table* metatable;
  size_t len;
  union Value user_; //user value
}Udata;

typedef union UUdata{
  L_Umaxalign dummy;
  Udata uv;
}UUdata;

typedef struct Upvaldesc{
  TString* name;
  lu_byte instack;
  lu_byte idx;
}Upvaldesc;

typedef struct LocVar{
  TString* name;
  int startpc;
  int endpc;
}LocVar;

typedef struct Proto{
  CommonHeader;
  
  lu_byte numparams;
  lu_byte is_vararg;
  lu_byte maxstacksize;
  
  int sizecode;
  Instruction* code;

  int linedefined;
  int lastlinedefined;
  TString* source;

  int sizelineinfo;
  int* lineinfo;

  int sizelocalvars;
  LocVar* locvars;

  int sizek;
  TValue* k;
  
  int sizeupvalues;
  Upvaldesc* upvalues;

  int sizep;
  struct Proto** p;
  struct LClosure* cache;
    
  GCObject* gclist;
}Proto;

typedef struct UpVal{
  TValue* v;
  lu_mem refcount;
  union{
    struct {
      UpVal* next;
      int touched;
    }open;
    TValue value;
  }u;
}
}UpVal;

#define ClosureHeader CommonHeader;lu_byte nupvalues;GCObject* gclist;
typedef struct CClosure {
  ClosureHeader;
  lua_CFunction f;
  TValue upvalue[1];
}CClosure;

typedef struct LClosure{
  ClosureHeader;
  struct Proto* p;
  UpVal* upvals[1];  
}LClosure;

typedef union Closure{
  CClosure c;
  LClosure l;
}Closure;

typedef union TKey{
  struct {
    TValuefields;
    int next;
  }nk;
  TValue tvk;
}

typedef struct Node{
    TValue i_val;
    TKey i_key;
}Node;

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


type operators
==============

1. tag and value 
/* raw type tag of a TValue */
#define rttype(o)    ((o)->tt_)

/* tag with no variants (bits 0-3) */
#define novariant(x)    ((x) & 0x0F)

/* type tag of a TValue (bits 0-3 for tags + variant bits 4-5) */
#define ttype(o)    (rttype(o) & 0x3F)

/* type tag of a TValue with no variants (bits 0-3) */
#define ttnov(o)    (novariant(rttype(o)))

=>
/* raw type tag of a TValue */
#define rttype(o)    ((o)->tt_)

/* type tag of a TValue (bits 0-3 for tags + variant bits 4-5) */
#define ttype(o)    (rttype(o) & 0x3F)

/* type tag of a TValue with no variants (bits 0-3) */
#define ttnov(o)    (rttype(o) & 0x0F)

=>
/* raw type tag of a TValue */
#define rttype(o)    ((o)->tt_)

/* type tag of a TValue (bits 0-3 for tags + variant bits 4-5) */
#define ttype(o)    ((o)->tt_ & 0x3F) 

/* type tag of a TValue with no variants (bits 0-3) */
#define ttnov(o)    ((o)->tt_ & 0x0F)

=>
#define checktag(o,t)    (rttype(o) == (t))
#define checktype(o,t)    (ttnov(o) == (t))

=>
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

=>
/* Bit mark for collectable types */
#define BIT_ISCOLLECTABLE    (1 << 6)

/* mark a tag as collectable */
#define ctb(t)            ((t) | BIT_ISCOLLECTABLE)

#define checktag(o,t)         (rttype(o) == (t))
#define checktype(o,t)         (ttnov(o) == (t))

#define ttisnumber(o)         checktype((o), LUA_TNUMBER)
#define ttisfloat(o)         checktag((o), LUA_TNUMFLT)
#define ttisinteger(o)         checktag((o), LUA_TNUMINT)
#define ttisnil(o)             checktag((o), LUA_TNIL)
#define ttisboolean(o)         checktag((o), LUA_TBOOLEAN)
#define ttislightuserdata(o) checktag((o), LUA_TLIGHTUSERDATA)
#define ttisstring(o)         checktype((o), LUA_TSTRING)
#define ttisshrstring(o)     checktag((o), ctb(LUA_TSHRSTR))
#define ttislngstring(o)     checktag((o), ctb(LUA_TLNGSTR))
#define ttistable(o)         checktag((o), ctb(LUA_TTABLE))
#define ttisfunction(o)         checktype(o, LUA_TFUNCTION)
#define ttisclosure(o)         ((rttype(o) & 0x1F) == LUA_TFUNCTION)
#define ttisCclosure(o)         checktag((o), ctb(LUA_TCCL))
#define ttisLclosure(o)         checktag((o), ctb(LUA_TLCL))
#define ttislcf(o)             checktag((o), LUA_TLCF)
#define ttisfulluserdata(o)     checktag((o), ctb(LUA_TUSERDATA))
#define ttisthread(o)         checktag((o), ctb(LUA_TTHREAD))
#define ttisdeadkey(o)         checktag((o), LUA_TDEADKEY)


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
