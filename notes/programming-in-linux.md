## gdb
---------------
+ [gdb Debugging Full Example (Tutorial): ncurses](http://www.brendangregg.com/blog/2016-08-09/gdb-example-ncurses.html)
+ [debugging with gdb](http://sourceware.org/gdb/current/onlinedocs/gdb/)
  - [A Sample gdb Session](https://sourceware.org/gdb/current/onlinedocs/gdb/Sample-Session.html#Sample-Session)
    - start 
      - `gdb program` 
      - `gdb program core_dump`
      - `gdb -p processid`
    - break and step into
      - `break functionname`
      - `n`(next, F10 for visualstudio)
      - `s`(next.step, F11 for visualstudio)
      - `bt`(summary of the stack)
        - `f i` : goto the stack frame at i level
      - `p(print) variable`
      - `l`(list ten lines of source surrrouding the current line)
      - `c`(continue, F5 for visualstudio)
    - end
      - `Ctrl+d` : giving it an EOF as input to interrupt program
      - `quit` : exit gdb
  - [gdb with lua](https://github.com/mkottman/lua-gdb-helper)
    - `wget https://github.com/mkottman/lua-gdb-helper/blob/master/luagdb.txt`
    - gdb to find the lua runtime thread
    - `source luagdb.txt`
    - `luatrace`
+ coredump
  - `cat /proc/sys/kernel/core_pattern`

## trace
---------
- strace program, [history of strace](https://en.wikipedia.org/wiki/Strace)
- dtrace, [dtrace4linux](https://github.com/dtrace4linux/linux)
- pstack processid

## path
---------
- add current directory to library search path: `export LD_LIBRARY_PATH=$LD_LIBRARY_PATH:$PWD`

## iptable
---------
+ chkconfig
  - chkconfig iptables on
  -  chkconfig iptables off
+ restart 
  - service iptables stop
  - service iptables start
  - service iptables restart
+ modify
  - `vim /etc/sysconfig/iptables`
  - `-A RH-Firewall-1-INPUT -m state --state NEW -m multiport -p tcp --dport from:to -j ACCEPT` 
  - `-A RH-Firewall-1-INPUT -m state --state NEW -m multiport -p udp --dport from:to -j ACCEPT`

## tcp
---------
+ reuse tcp port: 
  - `vim /etc/sysctl`
  - `net.ipv4.tcp_tw_reuse = 1`    
  - `net.ipv4.tcp_tw_recycle = 1`  
  - `net.ipv4.tcp_fin_timeout = 30` 
  - `sysctl -p`
+ tcpdump
  - `tcpdump -i eth1 -vnn udp and host destip -w output.cap`

## shell
-----------------
- short path in shell: `export PS1='[\u@\h \W]\$ '`
- change file permission:`su chmod 777 file`
- enable excute:`su chmod +x file.sh`
- [wiki:chmod](http://en.wikipedia.org/wiki/Chmod)
- view so info
  - `readelf -a file.so #show all information` 
  - `readelf -d file.so #show dependence so`
  - `readelf -h file.so #show header information`
- find and kill process, for example, when device busy
  - `ps aux | grep tun0`
  - `kill pid`
- create a TUN interface
  ```
  echo 1 > /proc/sys/net/ipv4/ip_forward
  iptables -t nat -A POSTROUTING -s 10.0.0.0/8 -o eth1 -j MASQUERADE
  
  # IF ubuntu
  # ip tuntap add dev tun0 mode tun
  
  # IF centos, yum install tunctl, then
  tunctl -p -t br0p0
  tunctl -n -t tun0
  
  ifconfig tun0 10.0.0.1 dstaddr 10.0.0.2 up
  ```
- Sed 
  - [An Introduction and Tutorial by Bruce Barnett](http://www.grymoire.com/Unix/sed.html)
- Grep 
  - [An introduction to grep and egrep. - by Bruce Barnett](http://www.grymoire.com/Unix/Grep.html)
- Awk 
  - [Awk - A Tutorial and Introduction - by Bruce Barnett](http://www.grymoire.com/Unix/Awk.html)
  - [Awk in 20 minutes](http://ferd.ca/awk-in-20-minutes.html)

- count file 
  - `ll -t|grep -|wc -l`

## script
---------
- [passing arguments to a shell script](http://osr600doc.sco.com/en/SHL_automate/_Passing_to_shell_script.html)
- [if then else fi](http://www.dreamsyssoft.com/sp_ifelse.jsp)
- tell linux which program to run script:`#! /bin/sh`
- array: `array=('one' 'two')`
- loop array:`for e in @{arrary[@]} do echo e done` 
  `for i in {5..10}; do echo $i; done`  `for i in $(seq 10); do echo $i; done`
- copy:`cp a b`
- force remove file:`rm -f file`
- if else:  `if [ "$1""x" = "publicx" ]; then else fi`
- [if else tutorial](http://www.dreamsyssoft.com/unix-shell-scripting/ifelse-tutorial.php)

## remote shell
----------
- security crt
  - file transfer
    - `rz -be`
    - `sz -be`
  - use vba macro


## command line as os
<div>典型场景是：服务器远程登陆，<u>只有命令行界面，此时不得不用命令行工具处理日常工作任务，所以就用，边用边查手册，用久了所以熟悉</u>。<br><br>日常任务有：<br><ul><li><b>文件上传、下载</b>。一般来说就是sz,rz,wget,curl,scp等。</li><ul><li>for循环wget，分分钟多进程下载。</li><li>wget -c 分分钟断点续传。</li></ul><li><b>压缩、解压</b>。所以得会用tar，zip等命令。我一开始的时候，问一个老手tar cvzf，tar xvzf后面一排参数啥意思，老手说：”早就忘记了，天天都这么用“。所以我认为它和一个按钮一个菜单没什么区别，命令行参数的选项和参数一般是经常用就熟悉，久不用就容易忘记，此时复查是正常的事情。顺便吐槽下windows下7z的菜单里直接解压tar.gz要解压两次真是麻烦事，命令行当然可以一次搞定。<br></li><ul><li>为什么后缀名是tar.gz? 可以参考这个知乎答案：<a href="https://www.zhihu.com/question/37019479" class="internal">为什么linux的包都是.tar.gz？要解压两次 - Linux</a></li><li>了解压缩算法：</li><ul><li><a href="//link.zhihu.com/?target=http%3A//www.cs.cmu.edu/%7Eguyb/realworld/compression.pdf" class=" external" target="_blank" rel="nofollow noreferrer"><span class="invisible">http://www.</span><span class="visible">cs.cmu.edu/~guyb/realwo</span><span class="invisible">rld/compression.pdf</span><span class="ellipsis"></span><i class="icon-external"></i></a><br></li></ul></ul><li><b>目录和文件的创建、移除、拷贝、移动</b>。基本命令是：mkdir,rm,mv等。这是最基本的常规操作了，一般来说需要注意的有：</li><ul><li>非空目录的移除，</li><li>递归删除时拼接路径的安全隐患（递归删除根目录就一点都不好玩了）</li><li>删除时文件被占用</li><ul><li>如果可能只是短时被占用，可以sleep几十毫秒再删，重试n次退出。</li></ul><li>考虑采用”删除xxx.old, 移动xxx到xxx.old, 拷贝yyy到xxx“的方式代替”直接删除xxx，移动yyy到xxx“的方案、</li><li>文件系统</li><ul><li>理解硬盘驱动，冗余磁盘阵列，inode，文件和目录，缓存策略等。</li></ul></ul><li><b>文本显示、查找、替换、统计</b>。在命令行下分析程序的日志，必备的技能就是文本操作。基本的命令有cat,tail,less,head,grep,sed,awk,wc,xargs等，这组命令行基本要通过管道来配合使用。我个人觉的awk的设计有点古老，awk的脚本我是一点都不觉得写起来流畅和爽快，sed的稍微好点，但也一般。如果是在windows下，这种流式过滤的工作，我是宁可开个LinqPAD写Linq来做，其实很多时候写脚本我都实际上觉的用正式点的高级语言做会更好。很多shell脚本写的人天马行空，别人读起来就不好玩了，毕竟写shell的人不写注释，遍地意大利面条。吐槽归吐槽，现实上我们不得不妥协，该用还得用。毕竟一切数据都可以输出成文本，所以文本的流式分析是基本技能。</li><ul><li>我认为这组命令的组合使用能让人体会到管道的好处。管道让多进程之间的流式操作流畅进行。顺便我认为shell的一个特点是，它天生是多进程程序，一个shell里随便一堆命令就是多进程操作，可能很多人写着的时候并没有意识到这点哦。进一步，多主机之间ssh授权后，shell里的ssh远程命令的组织又天生带有点分布式属性，也许这能带来一些启发。</li><li>理解管道的另一个要素是，需要理解fork，vfork、clone，exec；需要理解标准输入输出。fork之后，在exec子进程之前重定向父进程的标准输出，重定向子进程的标准输入，这样它们就被流式串在一起。</li></ul><li><b>文本编辑。</b><br></li><ul><li>vi基本技能。shell下编辑文本只能用这个了，我认为vi或者vim里最重要的技能就三个：导航、查找、替换，带着这个思维去记忆那些命令会明确很多。我觉的任何一个程序的核心功能就几个，每个核心功能的核心命令也就几个，其他的就按需查用即可。~~</li><ul><li>理解模式的概念，同样的键盘按钮，在不同vim模式下，操作代表不同的语义。想象一下相机是如何由有限的几个按钮做到多组不同功能的支持。</li></ul></ul><li><b>资源监控</b>。输出的信息都可以配合grep等使用，比如ps aux|grep processname.</li><ul><li>任务管理器型：vmstat, top, htop, </li><li>进程：ps</li><ul><li>理解OS对process的调度：<a href="//link.zhihu.com/?target=https%3A//en.wikipedia.org/wiki/Scheduling_%28computing%29" class=" wrap external" target="_blank" rel="nofollow noreferrer">Scheduling (computing)<i class="icon-external"></i></a></li><ul><li>FIFO，SJF，STCF，RR，MLFQ</li><li>理解os调度process的话，再去看协程，用户态协程需要自己做调度:）</li><li>IO密集和CPU密集？</li></ul></ul><li>磁盘：du, iotop, iostat</li><li>文件句柄：lsof</li><ul><li>比如查看一个进程打开的文件描述符，反向查看哪些进程打开了某个文件。</li></ul></ul><li><b>调试和Hook</b>。开发的话会需要调试程序和Hook程序等。</li><ul><li>通过ulimit -c unlimited让程序崩溃时产生coredump，然后用gdb调试，看崩溃堆栈，变量、单步等，这是gdb的常规操作。linux下开发必备技能了。ulimit 还可以限制其他资源，比如-n限制文件句柄个数，-s限制堆栈大小等，具体查文档。</li><li>系统调用、堆栈等的Hook：strace，dtrace、pstack等，这组算是lowlevel的神器，在很多时候能起到一针见血的效果。</li><ul><li>顺便，可以理解下系统调用的原理，中断的实现原理，这样你在使用strace监控一个程序的系统调用的时候，脑子里会有更清晰的思路。系统调用并不是一个简单的函数调用这件事么..</li><ul><li><a href="//link.zhihu.com/?target=https%3A//rcrowley.org/2010/01/06/things-unix-can-do-atomically.html" class=" wrap external" target="_blank" rel="nofollow noreferrer">Things UNIX can do atomically<i class="icon-external"></i></a> 多进程之间可以对文件和文件夹加锁<br></li><li>感兴趣？：<a href="//link.zhihu.com/?target=http%3A//www.ostep.org/" class=" wrap external" target="_blank" rel="nofollow noreferrer">Operating Systems: Three Easy Pieces<i class="icon-external"></i></a></li></ul></ul><li>export LD_LIBRARY_PATH=$PWD;</li><ul><li>理解系统查找共享库的搜索路径顺序。</li></ul><li>readelf: 都说了任何东西都可以输出成文本来看了，看个依赖库路径还是容易的。</li><ul><li><a href="//link.zhihu.com/?target=https%3A//en.wikipedia.org/wiki/COFF" class=" wrap external" target="_blank" rel="nofollow noreferrer">COFF<i class="icon-external"></i></a><br></li><ul><li><a href="//link.zhihu.com/?target=https%3A//en.wikipedia.org/wiki/Executable_and_Linkable_Format" class=" wrap external" target="_blank" rel="nofollow noreferrer">Executable and Linkable Format<i class="icon-external"></i></a></li><ul><li><a href="//link.zhihu.com/?target=https%3A//en.wikipedia.org/wiki/Portable_Executable" class=" wrap external" target="_blank" rel="nofollow noreferrer">Portable Executable<i class="icon-external"></i></a></li></ul></ul></ul></ul><li><b>网络。</b></li><ul><li>tcpdump抓包是一定要掌握的，抓tcp、udp包是必备的技能。输出pcap格式文件也是要的。如果是有图形界面的话，wireshake在windows和linux下都能用，如果是windows上还可以用microsoft networkmonitor。说实话，我觉的microsoft networkmonitor做的比wireshake好用。</li><ul><li>一个tcp连接，三次握手、四次挥手；建立在tcp连接之上的http连接，http request包在什么时候发，seq号是怎样变动的；诸如此类，都可以在这个地方查看。</li><li>一台机子有多个网卡(ifconfig)，或者单网卡，多ip，绑定到0.0.0.0的时候，收发包是怎样的情况？</li><li>理解tcp和udp的区别</li><ul><li>协议森林：<a href="//link.zhihu.com/?target=http%3A//www.cnblogs.com/vamei/archive/2012/12/05/2802811.html" class=" wrap external" target="_blank" rel="nofollow noreferrer">协议森林 - Vamei<i class="icon-external"></i></a></li><ul><li>拥塞控制算法，根据历史数据记录估算未来数据，这是一种学习（统计）</li><ul><li>Deep Learning，NN</li></ul></ul></ul><li>windows版本：<a href="//link.zhihu.com/?target=https%3A//www.winpcap.org/windump/" class=" wrap external" target="_blank" rel="nofollow noreferrer">WinDump - Home<i class="icon-external"></i></a></li></ul><li>nc命令用来探测主机之间的tcp端口或者udp端口是否联通。</li><li>netstat查看tcp和udp连接情况。</li><li>iptable的配置，配置tcp、udp端口啥的都要用到。</li><li>iptraf: 实时网络监控，比如监控某个网卡上的Ip包流量。</li></ul><li><b>数据库操作</b>。命令行下，操作mysql和sqlite是常见的事情，这个基本就是常规的sql操作语句。<br></li><ul><li>将数据库dump出SQL文本格式，到别的机子上导入什么的。。</li><ul><li>以前有个人打开了这么一份文本，惊讶说：“哇，谁写的SQL创建代码，这么整洁，连注释啥的都很规整...”</li></ul><li>理解关系型数据库的范式，事务，一致性。</li><ul><li>理解B+树、Block。</li><li>基于行存储Block的数据库，基于列存储Block的数据库，各自的优缺点</li></ul><li>NoSQL的也操作一些。</li><ul><li>Map-Reduce</li><li>Apache系列。。</li></ul></ul><li><b>版本管理等</b>。</li><ul><li>svn、git，这个本来就要会的，只是很多人用图形界面的工具多了后，可能命令行使用并不熟悉，但是在Linux shell下，这也算基本技能。</li><ul><li>理解“围绕数据结构设计接口”，git是关于commit的链表的数据结构的一组api。</li></ul></ul><li><b>元数据分布式同步</b>，ZoopKeeper<br></li><ul><ul><li>Fast PaxOS协议。</li><ul><li><a href="//link.zhihu.com/?target=http%3A//research.microsoft.com/pubs/64624/tr-2005-112.pdf" class=" external" target="_blank" rel="nofollow noreferrer"><span class="invisible">http://</span><span class="visible">research.microsoft.com/</span><span class="invisible">pubs/64624/tr-2005-112.pdf</span><span class="ellipsis"></span><i class="icon-external"></i></a> Leslie Lampor原作<br></li></ul></ul></ul><li><b>其他选择</b>：</li><ul><li>更现代的shell：fish</li><li>更好的http请求：httpie</li></ul><li><b>windows上的使用</b></li><ul><li>cygwin太大了，可以选用下面的组合，这样在windows的命令行下也可以使用上述大部分命令行。还是有很多便利的。</li><ul><li><a href="//link.zhihu.com/?target=https%3A//github.com/msys2" class=" wrap external" target="_blank" rel="nofollow noreferrer">msys2 · GitHub<i class="icon-external"></i></a></li><li><a href="//link.zhihu.com/?target=https%3A//github.com/bmatzelle/gow/wiki" class=" wrap external" target="_blank" rel="nofollow noreferrer">Home · bmatzelle/gow Wiki · GitHub<i class="icon-external"></i></a></li></ul></ul><li>手册类：the art of command line，BUT，并不是所有命令在所有环境下都能用。</li><ul><li>e版：<a href="//link.zhihu.com/?target=https%3A//github.com/jlevy/the-art-of-command-line" class=" wrap external" target="_blank" rel="nofollow noreferrer">GitHub - jlevy/the-art-of-command-line: Master the command line, in one page<i class="icon-external"></i></a></li><li>z版：<a href="//link.zhihu.com/?target=https%3A//github.com/jlevy/the-art-of-command-line/blob/master/README-zh.md" class=" wrap external" target="_blank" rel="nofollow noreferrer">the-art-of-command-line-zh-cn at master · jlevy/the-art-of-command-line · GitHub<i class="icon-external"></i></a></li></ul></ul></div>