lua GCObject都有一个marked标志位，GC采用的是经典的标记清除算法，一个对象的标记有三种状态：white、black、gray。从lgc.h的下面这段注视也可以初步验证。

```
/*
** Collectable objects may have one of three colors: white, which
** means the object is not marked; gray, which means the
** object is marked, but its references may be not marked; and
** black, which means that the object and all its references are marked.
** The main invariant of the garbage collector, while marking objects,
** is that a black object can never point to a white one. Moreover,
** any gray object must be in a "gray list" (gray, grayagain, weak,
** allweak, ephemeron) so that it can be visited again before finishing
** the collection cycle. These lists have no meaning when the invariant
** is not being enforced (e.g., sweep phase).
*/
```
标志位的设置，清除，判断，切换，四种基本操作，我一直认为语言上应该提供对象式的方法去操作，编译器或者解释器大可做一下翻译，而不需要用`&|~^`这么低阶的表达式写。标字位的这四大操作，很好的体现了编码的内涵，一个int，无论是几个bit，每个bit才是最低粒度，计算机编码的精髓就在于，bit才是编码的“原子”。对bit的操作是最基本的“封装”。例如，lua源码里就提供了下面一组宏，封装了会用到的bit位常规操作。

```
/*
** some useful bit tricks
*/
#define resetbits(x,m) ((x) &= cast(lu_byte, ~(m)))  
#define setbits(x,m) ((x) |= (m))   
#define testbits(x,m) ((x) & (m))  

#define bitmask(b) (1<<(b)) 
#define bit2mask(b1,b2) (bitmask(b1) | bitmask(b2)) 
#define l_setbit(x,b) setbits(x, bitmask(b)) 
#define resetbit(x,b) resetbits(x, bitmask(b))
#define testbit(x,b) testbits(x, bitmask(b)) 
```

在这个基础上，定义了white0、white1、black、finalized四个bit位：
```
/* Layout for bit use in 'marked' field: */
#define WHITE0BIT	    0  /* object is white (type 0) */ 
#define WHITE1BIT	    1  /* object is white (type 1) */
#define BLACKBIT	    2  /* object is black */
#define FINALIZEDBIT	3  /* object has been marked for finalization */
/* bit 7 is currently used by tests (luaL_checkmemory) */

#define WHITEBITS	bit2mask(WHITE0BIT, WHITE1BIT)
```
可见，white有两种细分状态，进而，下面一组宏用来测试一个GCObject处于white、black、gray、dead状态，white0或者white1都素算white，既不是white也不是black就是gray，然而gray状态未必是finalize状态：
```
#define iswhite(x)      testbits((x)->marked, WHITEBITS)
#define isblack(x)      testbit((x)->marked, BLACKBIT)
#define isgray(x)       (!testbits((x)->marked, WHITEBITS | bitmask(BLACKBIT)))
#define tofinalize(x)	testbit((x)->marked, FINALIZEDBIT)
#define otherwhite(g)	((g)->currentwhite ^ WHITEBITS)
#define isdeadm(ow,m)	(!(((m) ^ WHITEBITS) & (ow)))
#define isdead(g,v)	    isdeadm(otherwhite(g), (v)->marked)
```
以及状态切换的宏：
```
#define changewhite(x)	((x)->marked ^= WHITEBITS)
#define gray2black(x)	l_setbit((x)->marked, BLACKBIT)
#define luaC_white(g)	cast(lu_byte, (g)->currentwhite & WHITEBITS)
```

因为有两种white类型，分别是01、10，所以gc->currentwhite表示的是当前采用哪种white，从而要判断一个GCObject是否死亡，只要判断当前的white标志位是否为0，这就是isdead这个宏的作用，这里的做法是同时对两个标志位中其作用的那个标志位为0的检测。