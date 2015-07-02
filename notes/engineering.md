#### 编程中的工程问题
---------------------
- 库项目的预编译头文件和单一对外头文件
  - 使用XXXPreHeader.h/XXXPreHeader.cs统一管理对外依赖
  - 使用XXX.h/XXX.cs统一提供对外头文件

- 使用宏定义切换Debug/Release/ProductRelease
  - 使用XXX_DEBUG宏设置调试版本配置
  - 使用XXX_LOG宏设置日志版本配置
  - 使用XXX_NLOG宏设置无日志版本配置
  - 使用XXX_EXPORT宏设置导出配置
  - Debug版带调试，Debug/Release版带日志，ProductRelease版无调试和日志

- 函数前置后置检查
  - Debug版函数使用CHECK/Assert做前置和后置条件检查，让错误尽早暴露
  - Check/Assert不能代替错误分支处理，必须添加Release下良好运行的错误分支处理
  - 为做为返回值的参数（引用、指针、指针的指针）做前置初始化处理

- 多线程/单线程问题
  - 多线程版Lock和Unlock做加锁处理，单线程版本Lock和Unlock做线程检测处理
  - 区分多线程和单线程语义的RefCount
  - 最小化锁的范围，尽量不在锁内调用不可预知的函数，减少死锁的概率
  - 区分同步初始化/反初始化和异步初始化/反初始化，为异步初始化/反初始化做特别处理
  - 区分单线程异步的时序
  - 区分同步循环和异步链式事件触发机制
  - 考虑多线程下的`闭包（引用计数，弱引用）`和函数对象问题
  - 理清多线程的`工作线程`和`后台线程`之间的关系
  - 为数据结构容器提供Copy和Swap方法。
  - 为数据结构容器提供多线程和单线程版本。
  - 区分加锁和PostTask两种线程安全方案，区分等待/队列

- 锁
  - 属性读
    - 原子读（int）
    - 加锁（实例级别锁，或分组；自旋锁）
  - 属性写
    - 加锁（实例级别锁，或分组；自旋锁）
  - 方法
    - 加锁，锁的粒度，尽量只用来保护数据
    - PostTask/PostMessage+Internal Methoed
  - 事件
    - 注册
        - Closure
    - 触发
        - 同步循环
        - 异步链式
            - Closure
            - PostTask
    - 响应
        - 回到初始线程响应
        - 异步线程响应
        - 状态改变，时序，防死循环

- 如何让代码更健壮（鲁棒？，工业级？）
  - assert/check
    - null
    - thread
    - state
    - reentry
    - refcount
  - level log
    - debug
    - release
    - pr
    - track
    - warning
    - error
  - consist code style
  - document
  - kiss
  - locality
  - module

- 如何写可测试代码
  - 基本原则：
    - 保持单元测试程序的可用性
    - 模块可替换性，所以一切「硬编码」的部分都不利于可测试
    - 模块局部性，耦合的其他模块越多，则越不利于单元测试
  - 可测试性差的例子：
    - 构造函数里硬编码创建子模块（关键字new等）
    - 全局对象，单例
    - 静态函数，依赖其他库的静态函数
    - 传递一个大模块，却只是为了获取某个小变量

- C++常见RAII问题
  - 构造里未初始化，野指针问题
  - 构造里分配内存/获取资源，析构里忘记释放内存和资源
  - new/delete,new[]/delete[],malloc/free,vitual alloc/vitual free,heap alloc/heap free,global alloc/global free,local allloc/local free未匹配
  - 引用计数不平衡，没有严格遵守引用计数规则
  - 指针重新赋值前忘记析构旧内存，资源重新指向前忘记释放旧资源
  - 对象被不必要的延迟释放
  - 释放后，未将指针NULL化，野指针问题
  - 函数有多个提前返回的分支，未使用do{}while(fale);或者AutoXXX等确保内存/资源的自动释放。

- [编程的关键步骤](http://www.drdobbs.com/cpp/why-is-software-so-hard-to-develop/240168832)
  1. You are confident that you know what you want the computer to do.
  2. Your knowledge is correct and complete.
  3. You can translate that knowledge accurately into code.
  4. That code does what you think it does.
  5. The computer itself works the way you expect it to work.
  6. In addition to doing what you want, your program does nothing that you do not want.
  7. The program performs well enough for its intended use.
  8. If the program's output is intended to be approximate, the approximation is close enough.
  9. The program behaves reasonably when it encounters absurd or malicious input.

- 初始化和反初始化
  - 确保所有的资源都有正确的初始化
  - 确保所有的资源都有正确的反初始化
  - 明确资源的所有权模式
  - 为非内存资源提供正确的打开和关闭行为
  
- 抽象
  - 编程是一项严格的活动。编写结构清晰、意图明显的代码是困难的。困难一部分源于要选择正确的抽象。
  - 为了对付复杂的情况，我们使用了“分而治之”（divide and conquer）的方法，我们把复杂的问题分解成简单一些的子问题，然后解决这些子问题。

#### 兵器谱
-----------
- [concurrent versions system](http://www.nongnu.org/cvs/)
- [subversion versions control](http://subversion.tigris.org/)
- [git distributed revision control](http://git-scm.com/)
- [wiki:cvs](http://en.wikipedia.org/Concurrent_Versions_System)
- [wiki:svn](http://en.wikipedia.org/wiki/Apache_Subversion)
- [wiki:git](http://en.wikipedia.org/wiki/Git_(software))
- [阮一峰：Git分支管理策略](http://www.ruanyifeng.com/blog/2012/07/git.html)
- [github: Online project hosting using Git](https://github.com/)
- [github for windows](http://windows.github.com/)
- [sf.net: Open Source applications and software directory](http://sourceforge.net/)
- [google code: Google's official developer site.](http://code.google.com)
- [codeplex: Microsoft's open source project hosting site](http://www.codeplex.com/)
- [Bugzilla: bug tracking](http://www.bugzilla.org/)
- [Trac: enhanced wiki and issue tracking](http://trac.edgewall.org/)
- [redmine: A flexible project management](http://www.redmine.org)
- [Onlne UML](http://yuml.me/)
