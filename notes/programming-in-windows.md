细节是魔鬼，整理所有的细节，并不断补充完整个地图，再发酵。

#### MSDN索引
-------------
* [Windows API Index](http://is.gd/22HU9t)
  * [Using Synchronization](http://is.gd/spuSxy) 
    * [Using Event Objects](http://is.gd/L2pxeL)
    * [Critical Section Objects](http://msdn.microsoft.com/en-us/library/windows/desktop/ms682530(v=vs.85).aspx)
    * [CriticalSectionWithOwner](http://blogs.msdn.com/b/oldnewthing/archive/2013/07/12/10433554.aspx)
  * [Using Processes and Threads](http://is.gd/s6nCKr)
    * [Create Process](http://is.gd/iEQ8YT)
      * [CreateProcess does not create additional console windows under Windows 7?]
        (http://stackoverflow.com/questions/14958276/createprocess-does-not-create-additional-console-windows-under-windows-7)
    * [Create Thread](http://is.gd/U1gh3c)
    * [Using Fibers](http://is.gd/e7inSU)
      * [When does it make sense to use Win32 Fibers?]
        (http://blogs.msdn.com/b/ericeil/archive/2008/05/18/when-does-it-make-sense-to-use-win32-fibers.aspx)
        这边文章解释了Fibres：So really fibers aren’t about performance at all, but about making manual context-switching easier.

#### P/Invoker
--------------
- [Platform Invoke Tutorial](http://msdn.microsoft.com/en-us/library/aa288468(v=vs.71).aspx#pinvoke_callingdllexport)
- [COM Interop Part 1: C# Client Tutorial](http://msdn.microsoft.com/en-us/library/aa645736(v=vs.71).aspx)
- [COM Interop Part 2: C# Server Tutorial](http://msdn.microsoft.com/en-us/library/aa645738(v=vs.71).aspx)
- [What is marshalling? What is happening when something is “marshalled?”]
  (http://stackoverflow.com/questions/5600761/what-is-marshalling-what-is-happening-when-something-is-marshalled)
- [A Closer Look at Platform Invoke](http://msdn.microsoft.com/en-us/library/aa719485(v=vs.71).aspx)
- [Using Attributes](http://msdn.microsoft.com/en-us/library/48zeb25s(v=vs.71).aspx)
- [DllImportAttribute Class](http://msdn.microsoft.com/en-us/library/system.runtime.interopservices.dllimportattribute(v=vs.71).aspx)
- [MarshalAsAttribute Class](http://msdn.microsoft.com/en-us/library/system.runtime.interopservices.marshalasattribute(v=vs.71).aspx)
- [StructLayoutAttribute Class]
  (http://msdn.microsoft.com/en-us/library/system.runtime.interopservices.structlayoutattribute(v=vs.71).aspx)
- [InAttribute Class](http://msdn.microsoft.com/en-us/library/system.runtime.interopservices.inattribute(v=vs.71).aspx)
- [OutAttribute Class](http://msdn.microsoft.com/en-us/library/system.runtime.interopservices.outattribute(v=vs.71).aspx)
- [pinvokestackimbalance-in-net-40i-beg](http://codenition.blogspot.com/2010/05/pinvokestackimbalance-in-net-40i-beg.html)
- [CLR Interop CodePlex Site](http://clrinterop.codeplex.com/)

#### MultiThread
----------------
- [Threading in C# (by Joseph Albahari)](http://www.albahari.com/threading/)
- [构架设计:生产者/消费者模式](http://blog.csdn.net/program_think/article/category/516119)
- [C#:Circular Buffer](http://www.codeproject.com/Articles/2880/Circular-Buffer)
- [C#综合揭秘-细说多线程（上）](http://www.cnblogs.com/leslies2/archive/2012/02/07/2310495.html)
- [C#综合揭秘-细说多线程（下）](http://www.cnblogs.com/leslies2/archive/2012/02/08/2320914.html)
- [C#综合揭秘-细说进程、线程、应用程序域、上下文的关系](http://www.cnblogs.com/leslies2/archive/2012/03/06/2379235.html)
- [msdn:Asynchronous Programming Design Patterns](http://msdn.microsoft.com/en-us/library/ms228969.aspx)
- [wiki:Thread pool pattern](http://en.wikipedia.org/wiki/Thread_pool_pattern)
- [Smart Thread Pool (codeproject)](http://www.codeproject.com/Articles/7933/Smart-Thread-Pool)
- [Smart Thread Pool (codeplex)](http://smartthreadpool.codeplex.com/)
- [.NET's ThreadPool Class - Behind The Scenes](http://www.codeproject.com/Articles/3813/NET-s-ThreadPool-Class-Behind-The-Scenes)
- [Asynchronous Code Blocks](http://www.codeproject.com/Articles/15783/Asynchronous-Code-Blocks)
- [Game Resource:理解I/O Completion Port](http://dev.gameres.com/Program/Control/IOCP.htm)
- [msdn:I/O Completion Port](http://msdn.microsoft.com/en-us/library/windows/desktop/aa365198%28v=vs.85%29.aspx)
- [Managed I/O Completion Ports Part I](http://www.codeproject.com/Articles/10280/Managed-I-O-Completion-Ports-IOCP)
- [Managed I/O Completion Ports Part II](http://www.codeproject.com/Articles/11609/Managed-I-O-Completion-Ports-IOCP-Part-2)
- [wiki:Compare-and-swap](http://en.wikipedia.org/wiki/Compare-and-swap)
- [wiki:Thread (computing)](http://en.wikipedia.org/wiki/Thread_(computer_science)#Multithreading)
- [.NET-IO完成端口以及FileStream.BeginRead](http://www.cnblogs.com/yuyijq/archive/2011/03/22/IO_Completion_port_with_beginRead.html)
- [一个Reactor模式的比喻](http://daimojingdeyu.iteye.com/blog/828696)
- [Reactor & Proactor: two I/O multiplexing approaches](http://www.artima.com/articles/io_design_patterns.html)
- [reactor & proactor简析](http://blog.csdn.net/tang_xiao_bin/article/details/7715700)
- [云风：纤程（fiber）](http://blog.codingnow.com/2005/10/fiber.html)

#### MSBuild
------------
- msbuild location：C:\Windows\Microsoft.NET\Framework\v4.0.30319
- [msbuild command line reference](http://msdn.microsoft.com/zh-cn/library/ms164311(v=vs.90).aspx)
- [msbuild logfile](http://msdn.microsoft.com/en-us/library/ms171470(v=vs.80).aspx)
- [msbuild sidekick](http://www.attrice.info/msbuild/index.htm)

#### 文章
---------
- [孙辉：十年MFC经历认识的Microsoft技术](http://blog.csdn.net/sunhui/article/details/319551)
