404 NOT Found
=============
在程序的世界里，有一大类问题都可以划分到`404 NOT Found`的类别里，一大票问题都可以归结于`查表找不到`...

## 构造函数

```
class Dispatch{
  Dispatch(int i){}
};
Dispatch a;// Compile Error
```

最早学C++的时候，总是被这个问题搞的莫名奇妙，原来C++编译器会给一个class偷偷添加`默认构造函数`，
但是，如果用户自己定义了一个，则编译器不再提供`默认构造函数`。此时上述代码要通过的话需要我们
自己给Dispatch添加一个无参数构造函数，方可通过。

```
class Dispatch{
  Dispatch(){}
  Dispatch(int i){}
};
void DoSomething(){
  Dispatch a;// Compile Error
}
```

我们可以说

```
404 NOT Found：您调用的构造函数不存在
```

- 添加explicit关键字的单参数构造函数，刻意让想要隐式转换并调用构造函数的时候，404 NOT Found...:
see:[what does the explicit keyword in c mean](http://stackoverflow.com/questions/121162/what-does-the-explicit-keyword-in-c-mean)


- 至于当时为什么我会在没有无参构造函数的时候写`Dispatch a;`这样的语句，应该是以为所有的变量都可以这样声明吧。可惜C++里这里的行为就是在栈上分配了内存，调用了无参构造函数。


- `默认构造函数`和`无参构造函数`是两个概念。默认行为往往是坑，一旦使用方的需求和默认行为不匹配，而使用者又对默认行为不清楚，就会出现各种`诡异`的问题。
see:[Do the parentheses after the type name make a difference with new?](http://stackoverflow.com/questions/620137/do-the-parentheses-after-the-type-name-make-a-difference-with-new)

- 比如说当使用STL的时候，class的拷贝构造函数、赋值操作符，小于比较操作符等都在模板算法里被`使用`，一个学习STL的新手一定会在不了解模板的`duck type interface`原理时被这种`默认行为`坑到。
see:[What is The Rule of Three?](http://stackoverflow.com/questions/4172722/what-is-the-rule-of-three)

- C++03里构造函数不能调用别的构造函数，因为class还没构造完呢。。C++11添加了语法糖，支持了...

## 数组名和指针
C的数组和指针的区别在于，数组名只是数组起始地址的别名，编译器编译后就被替换成了数组的首地址，而一个指向数组的指针则是一个变量，是运行期行为。下面的代码：

```
char array_place[100] = "don't panic";
char* ptr_place = "don't panic";

int main()
{
    char a = array_place[7];
    char b = ptr_place[7];

    return 0;
}
```

编译后的汇编是

```
    char a = array_place[7];

0041137E  mov  al,byte ptr [_array_place+7 (417007h)]
00411383  mov  byte ptr [a],al

    char b = ptr_place[7];

00411386  mov  eax,dword ptr [_ptr_place (417064h)]
0041138B  mov  cl,byte ptr [eax+7]
0041138E  mov  byte ptr [b],cl
```
数组名示意图：
![](https://github.com/fanfeilong/f3/tree/master/imgs/242004478926843.png?raw=true)
指向数组的指针示意图：
![](https://github.com/fanfeilong/f3/tree/master/imgs/242005205495435.png?raw=true)

所以，而一个函数的数组参数

```
void foo(char arr_arg[], char* ptr_arg)
{
    char a = arr_arg[7];
    char b = ptr_arg[7];
}
```

编译后是：

```
char a = arr_arg[7];

00412DCE  mov  eax,dword ptr [arr_arg]
00412DD1  mov  cl,byte ptr [eax+7]
00412DD4  mov  byte ptr [a],cl

    char b = ptr_arg[7];

00412DD7  mov  eax,dword ptr [ptr_arg]
00412DDA  mov  cl,byte ptr [eax+7]
00412DDD  mov  byte ptr [b],cl
```

则是一模一样的，这是因为编译的时候，函数并没有被调用，所以编译器并不知道arr_arg的实际地址是什么，所以编译器就只能把它向指针一样处理。
这部分内容源自：[are pointer and arrays equivalent in c?](http://eli.thegreenplace.net/2009/10/21/are-pointers-and-arrays-equivalent-in-c)

我们可以说

```
404 NOT Found：函数编译时，数组的实际地址找不到,请看：https://lkml.org/lkml/2015/9/3/428，“because array arguments in C don't
actually exist”
```

## 野指针
一个class的指针成员变量，如果未被初始化，则是一个野指针，它不是NULL。
于是在运行的时候，它指向的内存（并不是有效的该类数据）被`解码`成这个类的数据，此时实际上是乱码的。
这样的乱码数据运行下去，就会有运行期的[未定义行为](http://stackoverflow.com/questions/2397984/undefined-unspecified-and-implementation-defined-behavior)。

我们可以说
```
404 NOT Found：野指针，指针指向的数据并不是有效的对象数据，您要的数据不存在
```

又：函数里返回局部栈上变量的引用或者指针，函数调用完的时候，函数的Stack被ret掉，局部变量的引用或指针就指向了已经被ret了的内存，
在后续的stack变来边去的时候，那块内存的数据早就不是原来的了。
```
404 NOT Found:您指向的栈地址的数据早就不是当初那个东西了...
```
此处有人还长篇大论：[Can a local variable's memory be accessed outside its scope?](http://stackoverflow.com/questions/6441218/can-a-local-variables-memory-be-accessed-outside-its-scope)

## 未定义符号
C/C++ 编译的时候，出现未定义符号，原因可能是这个符号所在的头文件并没有被包含。
为了找到它，可能的行为包括
1. 指定Include查找目录
2. 添加必要的头文件

我们可以说
```
404 NOT Found：符号所定义的文件找不到
```

## 未解决的符号
C/C++链接的时候，出现未解决的符号，原因可能是这个符号的定义虽然在编译的时候找到了，但是链接的时候没发现它的实现。
为了找到它，可能的行为包括
0. 比如说函数在头文件里实现了，在.c或者.cpp里却没有实现，那么实现它
1. 添加Lib查找的目录
2. 添加需要连接的Lib文件

我们可以说
```
404 NOT Found：符号没有被实现或者找不到obj，或者找不到库（一堆obj的合集）
```

又：它的反面是同一个符号有多个实现的版本，比如多个同时链接了多个不同实现的C运行时。此时不是找不到，而是找到了多个不知道用哪个，解决也很简单，明确指定要用的是哪个。
又：还有一种是同时只有多个弱符号，没有一个强符号，则不知道用哪个。

这两种情况，可以说
```
404 NOT Found: 有多个版本，找不到一个最强的来用
```

## 模板的具现化
```
#define type_zero 0
#define type_one 1
#define type_two 2

template <int type>
struct trait;          //Declare

template <>
struct trait<type_zero>{ //A
  enum {value=0}; 
};

template <>
struct trait<type_one>{ //B
  enum {value=1};
};

template <>
struct trait<type_two>{ //B
  enum {value=2};
}

void DoSomething(){
  std::cout<<trait<2>::value<<std::endl;//（1） 
  std::cout<<trait<3>::value<<std::endl;//（2），Compile Error
}
```

对于（1）：
1. 编译器尝试使用A版本具现化，不匹配，错误A，先不出错，下一步；
2. 编译器尝试使用B版本具现化，不匹配，错误B，先不出错，下一步；
3. 编译器尝试使用C版本具现化，匹配，忽略错误A和错误B

对于（2）：
1. 编译器尝试使用A版本具现化，不匹配，错误A，先不出错，下一步；
2. 编译器尝试使用B版本具现化，不匹配，错误B，先不出错，下一步；
2. 编译器尝试使用C版本具现化，不匹配，错误C，抛出编译错误。

如果编译器在错误A的时候就直接编译错误，那就没什么好说了，但编译器会尝试找重载的模板尝试具现化，直到所有的尝试都失败时才认为是真的失败了。

我们可以说
```
404 NOT Found:找不到可以具现化的模板
```

在尝试具现化的过程中遇到失败的情况先不抛出的特性也被起了个名字：
```
SFINAE: "Substitution Failure Is Not An Error"
```
再来一个例子：
```
struct example
{
    template <typename T>
    static void pass_test(typename T::inner_type); // A

    template <typename T>
    static void pass_test(T); // B

    template <typename T>
    static void fail_test(typename T::inner_type); // C
};

int main()
{
    // enumerates all the possible functions to call: A and B
    // tries A, fails with error; error withheld to try others
    // tries B, works without error; previous error ignored
    example::pass_test(5);

    // enumerates all the possible functions to call: C
    // tries C, fails with error; error withheld to try others
    // no other functions to try, call failed: emit error
    example::fail_test(5);
}
```
see: [Substitution Failure Is Not An Error](http://stackoverflow.com/questions/6850149/substitution-failure-is-not-an-error-sfinae-question-with-static-cast)


## 竞态条件
```
class Dispatch{

public:
  void AddObserver(Observerable* o){
      AutoSpinLock lock(m_lock);
      ...  
  }
  void RemoveObserver(Observerable* o){
      AutoSpinLock lock(m_lock);
      ...
  }
  void NotifyObservers(){
     AutoSpinLock lock(m_lock);
     Observers::iterator it=m_observers.begin();
     while(it!=m_observers.end()){
       Observer* o = *it;
       o.DoSomething();
       ++it;
     }
  }
private:
  typedef std::vector<Observerable*> Observers;   
  SpinLock m_lock;
  Observers m_observers;
}
```
发生的情况
1. 线程A：NotifyObservers。
2. 线程B：Observer要delete之前，调用Remove，成功，然后就析构自己。
3. 此时A线程的DoSomething还在过程中，崩溃。


此时我们可以说
```
404 NOT Found：您要通知的对象已被析构...
```
解决的办法就是用引用计数智能指针+弱引用智能指针
```
class Dispatch{

public:
  void AddObserver(weak_ptr<Observerable> o){
      AutoSpinLock lock(m_lock);
      ...  
  }
  void RemoveObserver(weak_ptr<Observerable> o){
      AutoSpinLock lock(m_lock);
      ...
  }
  void NotifyObservers(){
     AutoSpinLock lock(m_lock);
     Observers::iterator it=m_observers.begin();
     while(it!=m_observers.end()){
       shared_ptr<Observer> obj(it->lock());// 如果存活，增持，避免被析构
       if(obj){
         o.DoSomething();
         ++it;
       }else{
         it = observers.erase(it);
       }
     }
  }
private:
  typedef std::vector<std::weak_ptr<Observerable>> Observers;   
  SpinLock m_lock;
  Observers m_observers;
}
```
see：[Linux多线程服务端编程](http://book.douban.com/subject/20471211/)

如果不用智能指针，不带引用计数，那么，可以在NotifyObservers的时候，不立刻执行DoSomething，而是投递到目标线程去执行。假设Observer的工作线程是B，Dispatch在线程A，则NotifyObservers可以投递到线程B，在线程B才做真正的遍历触发，则保证Observer的Add、Remove、Find都在线程B，从而避免线程问题。

## 切换脚本语言
一个同学写了一段时间的lua代码

```
for i,v in pairs(nodes) do
  --//do something
end
```

有一天切换成到Python环境下写代码，同样遍历字典

```
for i,v in pairs(nodes):
  ## do something
```

`404 NOT Found：paris是个什么鬼...`

去掉

```
for i,v in nodes:
  ## do something
```

`404 NOT Found：没有迭代器...`

好吧：

```
for i,v in nodes.items():
  ## do something
```

## 声明的时候还没有定义

#### C的例子

```
typedef struct{
  ...
  list_node *next; // error
} list_node;
```
解决：

```
typedef struct list_node list_node;
struct list_node{
  ...
  list_node *next; 
};
```
ps，[C和C++的tag](http://www.embedded.com/electronics-blogs/programming-pointers/4024450/Tag-vs-Type-Names)

#### C++的例子
典型的C++ 前置声明用来做PIMPL (Private Implementation) 惯用法，避免循环include头文件
```
class A;
class B;
class C{
public:
  A* GetA();
  B* GetB();
}
```

#### Lua的例子

```
function fac()
  print(fac())--//error, 递归定义，此时fac还不存在
end
```

解决：

```
local fac
function fac()
  print(fac())
end
```
语法糖：
```
local function fac()
  print(fac())
end
```

#### scheme的例子

Y Combinator:http://mvanier.livejournal.com/2897.html
Y Combinator想要使用lambda搞出递归，最后办法就是把代码拷贝一份传递进去..

我们可以说
```
404 NOT Found:定义还没完成呢，想要用它的话，加个中介待定系数法吧
```

作为反例，switch-case的case里不能直接定义变量，因为case只是个可以goto的label，如果没有被goto到，这样的变量就只定义而没有初始化...，see:[Why can't variables be declared in a switch statement?](http://stackoverflow.com/questions/92396/why-cant-variables-be-declared-in-a-switch-statement)

## 版本

- A：哥，测一个
- B：好的，马上布下新服务，
- 10秒过去..
- A：哥，协议不对啊，好像你还是旧的协议
- B：咦，怎么上传的还是旧的

```
404 NOT Found: 需要的协议版本不对，测试部署最好也做版本号区分
```

## 心跳
https://en.wikipedia.org/wiki/Heartbeat_(computing)
https://en.wikipedia.org/wiki/Keepalive

无论是Tcp还是Udp，连接中都会使用心跳包，每隔n秒发送一次心跳包给服务器，每隔m秒判断是否有回包。如果没有回包，则判断连接断开。在某些情况下，允许中间断开一段时间，这样会在稍后重试心跳。程序如下：
1. 开始心跳，如果心跳失败，每个n秒重试，
2. 连续重试m次如果失败，就会等待一个大的时间x秒，x秒后重新开始1

利用心跳，可以做到：
- 在客户端，如果心跳失败，说明要么网络出问题，要么服务器出问题，则客户端可以连不上服务器，在P2P网络里则该客户端甚至无法与其他节点做打洞或者反连。
- 在服务端，根据心跳可以判断在线节点的个数，如果出现大面积不在线，则要么网络出问题，要么客户端出现某种未知异常。

```
404 NOT Found: 心跳失败，既死亡
```

## 分支闭合
分支似乎不用说。编程里最基础的便是if else。然而我们流行一句话: 很多bug最后定位到的时候往往发现是一个很2的问题。
为什么会这样呢？根据我的经验，我觉得还是因为if else没做到处处闭合（封闭性）。编程语言并不强调有if就要有else，这是语法级别的。而且我们很多时候为了避免代码嵌套过深，采用卫语句的方式提前返回。当情况复杂时，就容易漏掉某个分支。
所以，编程还是回归到了最最基本的逻辑，在语义上要强调分支的闭合，有if就要有else，即使你不写出来。而且工程健壮的程序，就是在处理各种**错误分支**，好的程序对所有的错误分支都是有意识并且有处理的，缺一不可。所谓测试，核心其实就是对分支闭合的测试，这也是体现工程师逻辑是否严密的地方。

```
404 NOT Found: 这个分支你没处理
```

## IP and Port
Ip和端口构成了一个EndPoint。

网络的世界里，两个EndPoint，A和B之间唯一定位彼此的是靠Ip和Port。可是，中间是各种墙（[NAT](https://en.wikipedia.org/wiki/Network_address_translation)，Router），只要任何一个地方把src和dest的Ip,Port封掉，它们之间就无法通信。网络上的可靠通信，基本原理是，三次握手打开连接，四次挥手关闭连接。放开关闭不说，单说握手。
```
1. A send syn to B
2. B send ack to A
3. A send ackack to B
```
三个步骤中的syn、ack、ackack任何一个包发送失败，都会导致该连接不能建立。而一般来说，如何把syn包从A发给B就是一个难题.

首先要对NAT分类：

[see:wiki-NAT](https://en.wikipedia.org/wiki/Network_address_translation)

1. Full-cone NAT, also known as one-to-one NAT. 
    - Once an internal address (iAddr:iPort) is mapped to an external address (eAddr:ePort), any packets from iAddr:iPort are sent through eAddr:ePort. 
    - Any external host can send packets to iAddr:iPort by sending packets to eAddr:ePort.

2. (Address)-restricted-cone NAT.
    - Once an internal address (iAddr:iPort) is mapped to an external address (eAddr:ePort), any packets from iAddr:iPort are sent through eAddr:ePort.
    - An external host (hAddr:any) can send packets to iAddr:iPort by sending packets to eAddr:ePort only if iAddr:iPort has previously sent a packet to hAddr:any. "Any" means the port number doesn't matter.

3. Port-restricted cone NAT. 
    - Like an address restricted cone NAT, but the restriction includes port numbers.
    - Once an internal address (iAddr:iPort) is mapped to an external address (eAddr:ePort), any packets from iAddr:iPort are sent through eAddr:ePort.
    - An external host (hAddr:hPort) can send packets to iAddr:iPort by sending packets to eAddr:ePort only if iAddr:iPort has previously sent a packet to hAddr:hPort.

4. Symmetric NAT
    - Each request from the same internal IP address and port to a specific destination IP address and port is mapped to a unique external source IP address and port; if the same internal host sends a packet even with the same source address and port but to a different destination, a different mapping is used.
    - Only an external host that receives a packet from an internal host can send a packet back.

根据情况，1,2,3都可以利用规则把完成语义上的syn的动作，如果两端都是Symmetric NAT就没救了。在NAT后面的，要想连接，就得trick NAT的映射机制。

其次对A和B分类，其中，2和3是同一种类型：
1. A和B都在外网
2. A在外网，B在内网（NAT后面）
3. A在内网（NAT后面），B在外网
4. A和B都在内网（NAT后面）

根据情况，1可以直连，2和3可以反连，4可以通过打洞方式，完成syn动作。如果把1,2,3,4做merge，即实现了一个混合多路发syn的connector。

再次，对使用的协议做分类：
1. 使用Tcp
2. 使用Udp

根据情况，1可以做直连和反连，打洞相对困难，但也不是不可以做，用Tcp不用自己做错误处理、拥赛控制。2可以做直连、反连、打洞，但是用Udp需要自己重新实现各种错误处理、拥赛控制以实现可靠连接。

最后，对Ip和端口的恶劣场景做分类
1. 频繁换IP，双网卡
2. 某些端口被NAT和Router封杀

对于这两点，只能通过：
1. 重新绑定Ip和端口
2. 随机化端口，但如果用户有Upnp的情况下，随机的端口可能未必能用。

以上没有说明的一点是A和B在应用层是用什么互相标识的。举例常见的Id情况：直接用Ip，用域名，P2P网络里用PeerId。无论是直连、反连还是打洞、本质上都是在做Id到可达EndPoint之间的转换，在域名系统里，这就是DNS的职责，常见的网络污染就发生在DNS这个环节。

```
404 NOT Found: Ip和端口不对。
```

Function
==================

很多时候，我们都在做翻译工作。

## 跨语言函数调用
比如说跨语言的函数调用，就是在不同语言之间通过在某一个语言端的中介的API做函数调用翻译工作。

- JNI，通过Java Native Interface，我们的本地代码直接调用JVM（Java Virtual Machine）的指针和Java代码之间跨语言交互。实际上对于JNI代码来说，并不是和Java代码交互，而是和JNIEnv的API交互。
- Lua，通过Lua的C函数接口，我们可以给Lua注册一堆的C接口。我们的本地代码直接调用的lua_State* 和Lua代码之间跨语言交互。实际上对于Lua的C接口函数来说，并不是和Lua代码交互，而是在lua_State的API交互。
- P/Inovke，通过.NET P/Invoke，我们可以在.NET里调用C的动态链接库。本地代码只要是标准C导出接口即可。 

对比之下，区别在于谁来做
- Lua是固定了接口，参数传递基于lua_State的栈去传递。lua_State做查找工作，程序员手工做了翻译以及参数的出栈、入栈。
- JNI是让C++接口去声明参数类型信息。编译器根据用户在Java和C端声明，做了查找工作。
- P/Invoke是让.NET自己去声明参数类型信息。编译器根据用户的声明做了自动查找翻译工作。

这三者做做了两件事情
1. 约定参数类型映射。
    - JNI是在C++端声明Java的参数类型所映射的本地类型。
    - P/Invoke是在.NET端声明本地类型到.NET类型的映射
    - Lua是在C/C++端让程序员直接`隐式假设`lua_State栈上每个槽应该放的是什么类型的数据。
2. 约定参数入栈顺序
    - JNI和P/Invoke遵循C/C++的调用约定
    - Lua则规定了参数的出栈入栈规则

回顾一下和函数调用约定相关的有cdcel、stdcall、fastcall等。下面的图在汇编层演示了这种过程。
![](https://github.com/fanfeilong/f3/tree/master/imgs/252213156891582.png?raw=true)

所以，逻辑上跨语言调用做的是在语言之间规定怎样调用对方的函数的规则，是属于函数调用的范畴。

我们考虑每一个函数都有输入参数和返回值。函数可以有单参数和多参数，而多参数可以通过柯里化变成单参数。
所以，函数问题可以简化为单参数单返回值函数的问题。可以用符号表示 `r=f(a)`。

这假设了f总是顺序执行。我们可以通过`Continuation`的概念把函数表示为`f(a,c)`的形式，
f接收a，同步或者异步执行完毕后把结果传递给`Continuation`,也就是c去处理，c的一个例子是
`function(r) print(r) end`


## 网络协议
任何和约定相关的，都可以称之为`协议`。和`协议`伴生的一般就是`协议包`，和`协议包`伴生的一般就是协议包的`编码`和`解码`。可以看下典型的网络协议。

- TCP协议和TCP协议包；
- Udp协议和Udp协议包；
- 基于TCP的Http协议和Http协议包。
- 基于Udp的P2P协议和P2P协议包；

一个网络协议是建立在`发送一个包a，返回一个包r`这样的基本操作之上的。比如说TCP握手：
```
client a         client b
send syn   ----> recv syn
recv ack   <---- send ack
send ackack----> recv ackack     
```
我们可以定义函数
send(request,function(response) end)

则TCP握手可以表示为

```
--client a                       
send(syn,function(ack) 
  send(ack,ignoreresponse)
  send(data,function(dataack)
     send(data,function(dataack)
      ...
     end)  
  end)
end)

--client b
send(nothing,function(syn)
  send(ack,function(ackack)
     send(data,function(dataack)
        ...
     end) 
  end)
end)
```

Udp协议和TCP协议的区别在于，TCP处理了每个函数调用的异常，例如`超时没有返回`，我们可以调整send函数原型
```
send(request,function(response) 
     --// 返回了期待的值 
  end,function(timeout) 
     --// 没收到期待的值，某种重发机制
  end,function(error)
     --// 彻底超时了，失败
  end)
```

所以，网络上发包和收包可以规约为函数调用。

ps：一个网络协议包可以拆开成包头和包体，而一条汇编指令、一条.NET的IL指令，一条JVM的bytecode，一个Lua的指令，也同样可以有进一步拆开成opcode+register。可以把一个网络协议包看成一条`网络汇编指令`。
网络协议包的包头一般包含有packagetype，可以当作是opcode，包体则是register里的数据。
ps：编程语言的指令，都喜欢把int拆开去使用，前几个位表示什么，接着几个位表示什么。网络协议包也类似。我觉的对bit的操作是编程世界里的最低粒度的`封装`和`抽象`。
![](https://github.com/fanfeilong/f3/tree/master/imgs/252309493613030.png?raw=true) ![](https://github.com/fanfeilong/f3/tree/master/imgs/252309546585611.png?raw=true)
![](https://github.com/fanfeilong/f3/tree/master/imgs/252311295802513.png?raw=true)



## RPC

基于网络协议，就可以实现网络通信。在应用层，我们更喜欢把网络通信当成是函数调用了。于是有了RPC. 
无论是Apache Thrift还是Google Protocol Buffer还是.NET WCF,都用某种代码生成器去生成网络通信封装成函数的事情。

## 函数、对象和闭包

无论是函数还是对象，都是一堆数据。不能闭包的语言里，函数只有代码数据。可以闭包的语言里，闭包了的函数除了代码数据还有被捕获的变量的数据（所谓upvalue，up是是upstream，也就是上游，可见只能捕获自己上游作用域的变量）。
在命令式语言里，对象是一个状态机，状态的维护在并发的时候遇到了data race的问题。在纯函数式语言里，数据是不可变的，无副作用的，并发的时候就消除了data race。在一个命令式语言里的函数式特性里还是会遇到data race。

## 重入


## references
- http://www.cs.virginia.edu/~evans/cs216/guides/x86.html
- https://en.wikipedia.org/wiki/Transmission_Control_Protocol
- http://luaforge.net/docman/83/98/ANoFrillsIntroToLua51VMInstructions.pdf
- http://thrift.apache.org/
- https://github.com/google/protobuf
- https://msdn.microsoft.com/en-us/library/vstudio/ms735119(v=vs.90).aspx
- https://github.com/Microsoft/rDSN

Burn Down chart
==================

#### S型的燃尽图
在一次milestone开发过程中，开发者会持续编辑issue列表，每个issue都有自己的生命周期。燃尽图预期这些issues会被线性的消灭掉，所以从第一天直接到最后一天画个直线表示预期进度变化，然而实际开发会遇到各种困难，所以实际的进度变化曲线往往不是线性变化的，下面这篇文章给出了S型燃尽图：
https://sandofsky.com/blog/the-s-curve.html

#### 三进制
计算机是基于二进制的，有一句经典的台词是：这个世界上有两种人，一种是懂10进制的，另一种是懂10进制的。但是在开发中，我们常常要接触三进制。程序开发中有一个Keep it simple, stupid的所谓KISS原则，就是说我们写程序，在没有必要的情况下，不要增加不必要的抽象层。例如很多初学者在刚接触了设计模式之后，就会在很简单的程序上不必要或者不正确的使用各种模式去写代码，但实际上那种程度的代码只需要把面向过程代码写好，做好函数这个级别的抽象即可。函数简洁但并不就简单，和通常的初学者相反，有经验的程序员更愿意用简洁的代码解决问题，而不会为了用上一堆所谓特性把代码写成花拳绣腿。一个函数需要考虑入口条件，出口条件，是否可重入，是否线程安全，所依赖的状态变化…等等。所以，有时如果你不放弃装饰，你就得放弃本质。

但是，程序从开发的第一天开始就被持续迭代。有一句话说：程序从被写下的第一天开始就开始腐烂的过程，这是说程序会因为需求的不断变化而需要不断的被重构，一开始几行的代码可能会膨胀，再压缩，再膨胀，再压缩。不同地方的代码会出现重复，各种各样的重复。于是，光有KISS是不够的，我们需要另外一个东西:Do Not Repeat Yourself,也就是所谓的DRY原则。怎样才能不自我重复呢？这就是所谓的三进制原则，同样一件事，重复做三次，就可以考虑批处理，计算机是最擅长做批量处理的事情，例如for循环，递归等。批处理的前提是结构的一致性，结构的一致为将他们装到容器里创造了可能。所以，为了批处理，我们常常需要在结构上做抽象。一致性可能是数据上的，也可能是行为上的，根据程序即数据的原理，数据的一致性可以用行为的一致性表示，所以我们可以只对行为的一致性考虑。在面向过程语言里，可能就是在函数这个级别考虑，在面向对象语言里，则更多可以在接口层体现。把共同的结构装到容器里的过程，就是组合的过程。

好了，三进制大概说到这里。

#### 我们需要专职的TA么？
从我一开始进入程序员这个行业开始，我就也会被软件开发中的人员配置问题困扰。一个团队，是否要有专职的项目经理，美工，测试，产品设计，以及并不讨人喜欢的行政，人力资源？从程序员的角度来说，属于技术偏好型，能用技术解决的，绝不愿意通过增加一个人去解决，能用工具解决的，一定批量处理之。先进技术和工具，常常在效率上是数量级的压倒性优势。
回到小题，通过几年的过程，我想角色上很多时候这些角色都有其必要性，但是，一个角色要有多个人还是一个人甚至直接由工具取代，则是可以因团队而不同。怎样取舍和适应？我认为关键在于：是否有效率的解决问题，例如开发和测试，如果开发和测试两个人不能很好的协作，共同有效率解决问题，而一个两者皆做的更有效率，那么明显后者更好，反之则选择前者。
因此，角色分离，但人员可以合一。也就是人剑合一。

#### 持续迭代
前面说燃尽图，软件开发，常常是一个长期过程。不是一锤子买卖，一个基础类库开发加一个上层产品开发就可能两年。但市场常常不等人，因此我们要紧凑的过程和质量，那么就需要持续迭代。持续迭代体现在一个milestone之内的每一天之间，需要每天跟踪昨天，今天，明天的进度，解决的issue，新产生的issue，代码的提交，测试的通过率，发布的版本，用户的反馈等等。持续迭代同时体现在milestone和milestone之间，粒度会更大一些。
一个理解持续迭代的地方是解bug，任何bug，我们都需要首先尝试去重现它，做二分排除定位，做单元测试刷豆子，搜集证据链，一步步掐头去尾缩小范围，最后解决。这也需要持续迭代。
持续迭代不仅在节奏上保持紧凑，而且把过程渐进展开，我们都知道软件开发的敌人是黑盒子，不能预期的时间和进度，所以持续迭代的过程，我们通过燃尽图，issue管理，bug跟踪，反复把过程dump出来，让进度可视化，促进软件开发的正面化学反应和收敛。
```
while(rest>e){ step(); }
```

#### 充分必要
软件开发，涉及到很多需要推理的地方，例如每天的解bug，这里需要逻辑，也就需要充分必要。一个论点，它的论据是否是充分必要的，就是体现逻辑的地方。
其他地方，例如做案例分析，我们训练思辨和分析，我想也应当是基于逻辑的。例如做需求分析，也是应该以是否符合逻辑为核心，如果连自己都说服不了，泛泛而谈的形式化需求分析，多半是需要被重新做的。所以，原则应当是，写完自己读一遍，根据是否充分必要去复审，也许需要补充数据，真实的数据很重要，有数据就可以拿数据说话，一个断言在有数据支撑的情况下，更可能有效。但使用数据要避免单样本统计学家式的武断，同时也要不失管中窥豹的洞察力，这大概需要反复锻炼，反复成功或者失败。

#### 工具
程序员是带工具属性的，工程上，每个小事情都可能有相关的工具可以利用，如果没有，我们就造工具。同样的管理源代码，我们有svn,git，当我们赤手空拳，我们合并别人的代码，可能会先临时拷贝自己新改动的代码到某个临时文件夹S，然后把别人的代码覆盖进来，完了再把S和其diff，再修改。用git，这个过程就是git stash，git pull,git stash pop，所以有时候只要想一下一个操作过程当自己没有工具是是怎么做的，可能工具也大概就是这么做的，只是变成了更有效率的命令。工具也是符合逻辑的。
两个团队A和B。A没做事，B不用工具做事，B比A强；A不用工具做事，B用工具做事，期待的是B比A更有效率的解决问题，否则B并不就比A强；如果两队都做事也都有效率使用工具，则可以开始比内容。所以把那些提高效率的工具都用到习以为常，瓶颈不卡在那些上面，我们就进入比拼真正的最终目标上。反复的训练和使用，可以达到这点。

#### 重试
一个任务，耗时估计的靠谱度跟做过这件事的次数有关，一般来说做过两三次额估计就靠谱点。这也是有经验公式可用。如果它是一个普遍原理，我们可以利用它。从概率上来说，一个事情一次的成功率为a，那么，多次之后的成功率是可计算的，手机打字就不展开了。可以验证，重试可以增加成功率。所以，很多事第一次做，可以预估会有失败和错误，所以失败和犯错时冷静的保存数据，证据，记录，探测的这些点，都可以用来在重试时做可能的规避。不过很多人只尝试了一次，这就回到了经典论述：写100个helloworld不如对一个程序迭代100次。当然，我们要考虑到人是复杂的，喜新厌旧是常态，并不能一概而论。因为，每个人都是独立的个体，选择的自由正是这种平均值。重试要注意两次之间是否收敛，如果不收敛，则重试只会导致队列堆积，甚至引起雪崩，此时要检查更深的问题。

#### 基本和到位
我常常做过一件事以后，发现很多事情并不需要各种奇葩的创新点子之类。做着做着就会看到，做好一件事最需要的并不是创新什么的，而是：概念界定之后的基本功。说概念界定，是因为很多人对自己的职业角色定位不清，就会导致做的事情不能抓住基本点。而一旦界定了角色边界，明确了基本点，那么工作就变成如何完美的把那些基本点做到位上面。例如，保持节奏，从不拖延，持续迭代，细节的完备，以及坚持不越界的做不应该自己做(你得让该做这件事的人去做，有原则对彼此都是一种轻松)。把基本功的细节做到位，就是一种职业素养，和专业主义。我想，这是重要的。我最怕角色不清，要么认为你应该什么都做，要么认为你什么都不要做，角色不清，系统的调度会混乱而没有效率。一个人可以做不同的事情，但最好在角色的语义上区分之。

因此，我也觉得很多事，其实能做到这些的人自己也都可以做好，未必得找别人。这和我经历过到处找控件来组装，到掌握组合控件的基本原理后不认为有魔法控件一样，省去了拿来主义。

#### 提前量和惰性求值
当我们预估，我们会没有充裕的时间做完整的过程，我们可以打时间差。我们说对一个系统做性能优化，不外乎在时间和空间上做优化。如果一个系统或者一个过程要到达预期质量所要消耗的最低时间已经不可再压缩，并且预估到时间整体上将呈现下滑趋势，那么就应该在一开始时间还足够充裕的情况下打提前量，通过一开始的紧凑有序的节奏把大部分脚手架都搭建起来，那么越到后期越会从容和有条不紊，反之则会手忙脚乱。凡事预则立，不预则废。

相反的做法是，对事情做惰性求值，直到我们真正需要一个事情的结果时，才会去做这件事，惰性求值有时候能最大限度减少当前要做的事情。但是，惰性求值相当于一次异步投递任务到队列的过程，队列里等待被做的事情会堆积，当需要在限定时间内求某个事情的结果时，队列可能已经雪崩了。所以有队列的地方就要防雪崩，队列不能过长（根据优先级丢弃），保证你的队列可以在最低粒度时间片内被有序求值。如果你不想你必须要做的事情被丢弃，那么那些事情可以通过打时间差提前做掉，达到超出预期的效果。再说异步任务，异步任务都可能由于各种原因而不会有返回值，所以有异步就要有定时器，做超时监听，一旦超时，做对应的处理。

#### 版本号
无论是10进制，还是10进制，亦或是点分10进制。它们都是版本号。我们的人生就是在年轮的版本之间迭代。认真对待每个版本，麻雀虽小，五脏俱全。每个版本开始，明确本次版本的目标，屏蔽其他，只指目标，当其达成，测试通过，打包，发布，写上：

版本 0.0.3
- 修订了xxx
- 移除了yyy 
- 改进了zzz
- 新增了abc 

版本和版本之间，贵在持续，节奏均匀。版本的发布日志，就是一个diff。两个集合，A和B的对称差，就是发版的日志。这让我想起，从写程序到现在，在各种场合写过两个数组，区间列表等的diff。很多时候，我们都在做diff，我常常从程序处理的过程得到一些概念，借以类比或者理解生活的事情，然而并不将其泛化，人太容易只看到类比事物之间共性的地方，而其实事物的复杂更在于那些不同的地方。手里有一把锤子，就到处找钉子的事情，永远都有。

#### 拥塞控制
经典的生产者，消费者情景是这样的：
- 过程中会不断产生任务，加入任务队列，这是生产者
- 处理器会定时刷帧，每一帧都会检查当前的窗口，在窗口内的任务就会被执行
- 每个任务执行完都会更新系统的资源状态，这些状态用来更新窗口，超时的任务可能没执行完需要重新投递到队列里。

这样的过程，不断执行的任务，获取或者释放系统资源，而系统资源变化可以反馈到调度器的允许执行窗口，每一帧则从队列里取出窗口内的任务执行。

我们说，这是一个拥塞控制。不做拥塞控制的情况下，在执行的任务可能太少而没有充分利用系统资源，或者在执行的任务太多而把系统资源竞争到卡死，无论怎样，系统的资源利用率最饱和和任务的执行最多，是一个优化目标。所以，一个健壮的系统，有队列的情况下，要做拥塞控制。

拥赛控制的核心法则是：多还少补，在慢启动、拥赛避免、快速恢复3个阶段来回切换。

#### 分段燃尽
从瀑布到敏捷，有的人说我是classical的，严格的瀑布有序经典；有的人说，我是删繁就简的，小步敏捷迭代，蚂蚁搬家。
就像我认为角色需要分开，人员可以合一。无论是学理上还是实践上，过程的不同阶段是存在的，但是具体实施上可能会压缩和合并。
Plan, Build,Test，基本的控制点在于这三者，侧重点各不相同。全局一次，还是反复多次，只是调度策略的不同。
假设过程是一条插值曲线，那么分段插值曲线比全局插值曲线有更好的局部性，学过数值分析的应该都知道全局插值曲线在控制点多了之后，插值多项式的次数高，会有振荡现象，就是由于局部性不好。


State of Machine
==================
许多东西都可以看成是状态机，我们只看有限状态机的维基：
[Finite-state_machine](https://en.wikipedia.org/wiki/Finite-state_machine)
>A finite-state machine (FSM) or finite-state automaton (plural: automata), or simply a state machine, is a mathematical model of computation used to design both computer programs and sequential logic circuits. It is conceived as an abstract machine that can be in one of a finite number of states. The machine is in only one state at a time; the state it is in at any given time is called the current state. It can change from one state to another when initiated by a triggering event or condition; this is called a transition. A particular FSM is defined by a list of its states, and the triggering condition for each transition.

在非函数式语言里，一个结构体+一组API，或者一个类的接口，都可以看作是一组操作一个状态机的函数。

在网络编程中，一个发包和一个回包，是最基本的收发操作（Atom Operation）。当我们引入状态机，将连续的收、发包操作组装起来，就构成了某个网络协议，一组有效的收发包要么让这个状态机保持某种状态，要么切换到另一个状态。

最早接触状态机是在写OpenGL的时候，知道了OpenGL是一个全局状态机：
[OpenGL是一个状态机](http://blog.csdn.net/pizi0475/article/details/5388084)
>OpenGL是一个状态机，它维持自己的状态，并根据用户调用的函数来改变自己的状态。根据状态的不同，调用同样的函数也可能产生不同的效果。

为什么OpenGL的状态机是全局的呢？
[Why does OpenGL be designed as a state machine originally？](http://stackoverflow.com/questions/15194002/why-does-opengl-be-designed-as-a-state-machine-originally)
>The reason for this kind of "state machine" is simple: that's how the hardware generally works.
>Because originally, back in OpenGL 1.0, there were no objects (besides display lists). Yes, not even texture objects. When they decided that this was stupid and that they needed objects, they decided to implement them in a backwards compatible way. Everyone was already using functions that operated on global state. So they just said that by binding an object, you override the global state. Those functions that used to change global state now change the object's state.

OpenGL已经很久没用来写代码干点什么。但是OpenGL的几个模式却常见。

## 开关
```
if( glIsEnabled(GL_BLEND) ) {
     // 当前开启了混合功能
} else {
     // 当前没有开启混合功能
}
```

- 照相机或者其他设备，常常需要打开某种开关后，某些操作才有效。
- VIM编辑器，某些操作需要先打开一些开关后，才有效。

## 出栈、入栈
```
glPushMatrix();
  // 里面干什么都不会影响到外面的矩阵世界
glPopMatrix();
```

- C函数的调用栈
- Lua的扁平展开的栈
- Call/CC说的是Stack在Functional语言里的形式
- GUI渲染的栈
- 写代码生成器，嵌套的block栈让事情变的简单：
```
.Line("int test()")
.PushLine("{")
    .Code()
.PopLine("}");
```

## 模式
```
glViewport(0, 0, w, h);
glMatrixMode(GL_PROJECTION); 
glLoadIdentity();
gluPerspective(60.0, (GLfloat) w/(GLfloat) h, 1.0, 20.0);
glMatrixMode(GL_MODELVIEW);
```
我记得小时候，去别人家看录像机，大人们操作几次之后，一帮小孩把大人们按哪个键开关、播放、切换什么的都会记得清晰。后面好几年，接触电子设备少，偶尔来一个相机都不知道那么多按钮是干嘛用的，为什么一会儿这个按钮是干这个用的，一会儿又是那个效果。绕了一圈，后面学了VIM，VIM的每个快捷键的是什么效果会取决于编辑器当前处于什么模式，例如编辑模式、命令模式、可视模式等等。后面我再次用相机的时候，一下就想明白了，电子设备的按钮不可能无限多个，所以引入“模式”即可解决。某个按钮用来控制当前的模式，切换到不同模式后，同一个按钮的作用可以不同。OpenGL的glMatrixMode这个套路在很多场景下都一再的被使用。

写一个代码生成器，先给一组最简单的渲染操作：
```
private static int indent=0;
public static StringBuilder Push(this StringBuilder sb) {
    indent++;
    return sb;
}
public static StringBuilder Pop(this StringBuilder sb) {
    indent--;
    return sb;
}
public static StringBuilder Line(this StringBuilder sb, string text, bool ignoreIndent=false) {
    if (ignoreIndent) {
        sb.AppendLine(text);
    } else {
        var space=new string('\t', indent);
        sb.AppendFormat("{0}{1}\n", space, text);
    }
    return sb;
}
public static StringBuilder Line(this StringBuilder sb) {
    sb.AppendLine();
    return sb;
}
public static StringBuilder FormatLine(this StringBuilder sb, string format, params object[] args) {
    var space=new string('\t', indent);
    var text=string.Format(format, args);
    sb.AppendFormat("{0}{1}\n", space, text);
    return sb;
}
public static StringBuilder PushLine(this StringBuilder sb, string text) {
    return sb.Line(text).Push();
}
public static StringBuilder PopLine(this StringBuilder sb, string text) {
    return sb.Pop().Line(text);
}
public static StringBuilder PopLine(this StringBuilder sb) {
    return sb.Pop().Line();
}
```
其中Push和Pop正是借用Stack的概念，来简化block块代码的生成。我们可以如下使用：
```
var sb = new StringBuilder();
sb.Line("#include \"./PackageUtil.h\"")
  .Line("PACKAGE_HANDLE PackageUtil::CreateEmptyPackage( PackageHeader& header )")
  .PushLine("{")
  .Line("int ret = RESULT_FAILED;")
  .Line("PackageInitData initData = {PackageInitDataType_Header,&header};")
  .Line("switch(header.PackageType)")
  .PushLine("{");

foreach (var p in protocols) {
    sb.FormatLine("case XXX_PACKAGE_{0}:",p.Name)
      .Push()
      .FormatLine("return CreatePackage(PackageTrait<XXX_PACKAGE_{0}>::GetClassName(),&initData,&ret);", p.Name.ToUpper())
      .Pop();
}

sb.Line("default:")
  .Push().Line("return NULL;").Pop()
  .PopLine("}")
  .PopLine("}");
```

然而，这里的foreach打断了代码的链式形式，实际上这样也就够用了，但我们可以再折腾一下，做点无聊的事情：
```
public class EachBuilder<T> {
    public IEnumerable<T> List;
    public StringBuilder Builder;
    public List<Action<T>> Actions;
}
public static EachBuilder<T> BeginEach<T>(this StringBuilder sb, IEnumerable<T> list) {
    return new EachBuilder<T>(){
        List = list,
        Builder = sb
    };
}
public static EachBuilder<T> Push<T>(this EachBuilder<T> sbe) {
    var action=new Action<T>(t => sbe.Builder.Push());
    sbe.Actions.Add(action);
    return sbe;
}
public static EachBuilder<T> Line<T>(this EachBuilder<T> sbe, string text, bool ignoreIndent=false) {
    var action=new Action<T>(t => sbe.Builder.Line(text, ignoreIndent));
    sbe.Actions.Add(action);
    return sbe;
}
public static EachBuilder<T> Line<T>(this EachBuilder<T> sbe) {
    var action=new Action<T>(t => sbe.Builder.Line());
    sbe.Actions.Add(action);
    return sbe;
}
public static EachBuilder<T> PushLine<T>(this EachBuilder<T> sbe, string text){
    var action=new Action<T>(t => sbe.Builder.PushLine(text));
    sbe.Actions.Add(action);
    return sbe;
}
public static EachBuilder<T> PopLine<T>(this EachBuilder<T> sbe, string text) {
    var action=new Action<T>(t => sbe.Builder.PopLine(text));
    sbe.Actions.Add(action);
    return sbe;
}
public static EachBuilder<T> PopLine<T>(this EachBuilder<T> sbe) {
    var action=new Action<T>(t => sbe.Builder.PopLine());
    sbe.Actions.Add(action);
    return sbe;
}
public static EachBuilder<T> Pop<T>(this EachBuilder<T> sbe) {
    var action=new Action<T>(t => sbe.Builder.Pop());
    sbe.Actions.Add(action);
    return sbe;
}
public static EachBuilder<T> FormatLine<T>(this EachBuilder<T> sbe, string format, Func<T,string> args){
    var action=new Action<T>(t => sbe.Builder.FormatLine(format, args(t)));
    sbe.Actions.Add(action);
    return sbe;
}
public static EachBuilder<T> FormatLine<T>(this EachBuilder<T> sbe, string format, Func<T, string> args1, Func<T, string> args2) {
    var action=new Action<T>(t => sbe.Builder.FormatLine(format, args1(t),args2(t)));
    sbe.Actions.Add(action);
    return sbe;
}
public static EachBuilder<T> FormatLine<T>(this EachBuilder<T> sbe, string format, Func<T, string> args1, Func<T, string> args2, Func<T, string> args3) {
    var action=new Action<T>(t => sbe.Builder.FormatLine(format, args1(t), args2(t), args3(t)));
    sbe.Actions.Add(action);
    return sbe;
}
public static StringBuilder EndEach<T>(this EachBuilder<T> sbe){
    foreach (var item in sbe.List){
        foreach (var action in sbe.Actions){
            action(item);
        }
    }
    return sbe.Builder;
}
```
我们把StringBuilder通过通过BeginEach绑定一个IEnumearble变成一个EachBuilder，然后为EachBuilder提供一组同之前同语义的操作，这些操作内部只是把要做的事情通过Action的形式收集起来，**保持顺序**。在EndEach的时候，再一次性对之前绑定的List做操作。这里我希望使用者感觉这些API和之前的版本没有太大差别。当EndEach完成的时候，EachBuilder就再次转回到StringBuilder。从而，我们可以把代码写成：
```
sb.Line("#include \"./PackageUtil.h\"")
    .Line("PACKAGE_HANDLE PackageUtil::CreateEmptyPackage( PackageHeader& header )")
    .PushLine("{")
        .Line("int ret = RESULT_FAILED;")
        .Line("PackageInitData initData = {PackageInitDataType_Header,&header};")
        .Line("switch(header.PackageType)")
        .PushLine("{")
        .BeginEach(protocols)
            .FormatLine("case XXX_PACKAGE_{0}:", p => p.Name)
            .Push()
            .FormatLine("return CreatePackage(PackageTrait<XXX_PACKAGE_{0}>::GetClassName(),&initData,&ret);", 
               p => p.Name.ToUpper())
            .Pop()
        .EndEach()
        .Line("default:")
        .Push().Line("return NULL;").Pop()
        .PopLine("}")
    .PopLine("}");
```
这样，通过切换模式的上下文，就达到了对API语义的切换，使之适用于批处理。可以看到上面的代码的结构和目标生成代码之间具有结构的一致性。如果我们再无聊点，折腾一些模板语言来“声明性”表达这个过程，再写一个parser把它转换为上述代码的逻辑，我们就可以定义一个DSL，这是后话。通过同样的方式，我们可以为树形结构设计BeginTree、EndTree。为图结构设计BeginGraphics、EndGraphics，and so on。

## 渲染
从上面的过程，我们可以说代码生成器和渲染器（至少OpenGl）有诸多共同的地方，我们可以说代码生成就是在做文本渲染。开头说过网络协议本身是一个状态机，传统上，认为写网络层代码是一件“难”的事情，然而，网络协议状态机是一种适合严格的形式化方法的场景。协议包可以被生成、协议的状态机可以被生成、这样我们可以把精力放在如何设计协议、如何设计好的拥塞控制算法等等真正需要数学和脑力的地方。