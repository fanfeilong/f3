(1) 状态机拜占庭系统
====================

特点
------
同一时刻只能执行一个请求，因此服务器之间执行请求顺序要求完全一致。

协议
------
- PBFT(Practical Byzantine Fault Tolerance),实用拜占庭容错，由Castro 和 Liskov 在 1999年提出。

在 N ≥ 3F + 1 的情况下一致性是可能解决。其中，N为计算机总数，F为有问题计算机总数。信息在计算机间互相交换后，各计算机列出所有得到的信息，以大多数的结果作为解决办法。

(2) Quorum投票系统
=====================

特点
-----
指共享数据存储在一组服务器上，通过访问服务器的一个大小恒定的子集(quorum)来提供读/写操作。这类协议都含有一个特性：规定访问的子集大小后，任何一个这样的子集都包含最新的数据，并且一定可以读出来。

协议
------
- paxos
  - [How Paxos works](http://rystsov.info/2015/09/16/how-paxos-works.html)
  - [consensus-protocols-two-phase-commit](http://the-paper-trail.org/blog/consensus-protocols-two-phase-commit/)
  - [Understanding Paxos](https://understandingpaxos.wordpress.com/)
- Zookeeper, ZAB
- Viewstamped Replication(VR)
- Facebook, raft
- multi-paxos
- [Single Decree Paxos](http://rystsov.info/2017/02/15/simple-consensus.html)

(4) 分布式系统
- [system-design-primer](https://github.com/donnemartin/system-design-primer)
- [TLA+](http://lamport.azurewebsites.net/video/intro.html)
- [paxos code](http://nil.csail.mit.edu/6.824/2015/notes/paxos-code.html)

(5) dist lock
- [Distributed locks with Redis](https://redis.io/topics/distlock)

参考资料
==============
- [Awesome Courses,(Systems,PL-Compile,Algorithm)](https://github.com/prakhar1989/awesome-courses/blob/master/README.md)
- [wiki:Paxos](http://en.wikipedia.org/wiki/Paxos_(computer_science))
  - [wiki:Leslie Lamport](http://en.wikipedia.org/wiki/Leslie_Lamport)
  - [wiki:Byzantine fault tolerance](http://en.wikipedia.org/wiki/Byzantine_fault_tolerance)
  - [wiki:拜占庭将军问题](http://zh.wikipedia.org/wiki/%E6%8B%9C%E5%8D%A0%E5%BA%AD%E5%B0%86%E5%86%9B%E9%97%AE%E9%A2%98)
  - [research.micfosoft.com:The Byzantine Generals Problem ](http://research.microsoft.com/en-us/um/people/lamport/pubs/byz.pdf)
  - [wiki:Two Generals' Problem](http://en.wikipedia.org/wiki/Two_Generals%27_Problem)
  - [wiki:Paxos算法](http://zh.wikipedia.org/wiki/Paxos%E7%AE%97%E6%B3%95)
  - [NEAT ALGORITHMS - PAXOS](http://harry.me/blog/2014/12/27/neat-algorithms-paxos/?hn=1)
  - [wiki:List of Distributed Computing Projects](http://en.wikipedia.org/wiki/List_of_distributed_computing_projects)
  - [wiki:Quorum](https://en.wikipedia.org/wiki/Quorum_(distributed_computing))
- [Principles of Distributed Computing (lecture collection)](http://dcg.ethz.ch/lectures/podc_allstars/)
  - [Distributed Systems Syllabus](http://www.cs.cmu.edu/~dga/15-440/F12/syllabus.html)
- [wiki:MapReduce](http://zh.wikipedia.org/zh/MapReduce)
  - [HadOOP](http://en.wikipedia.org/wiki/Apache_Hadoop)
  - [HBase](http://hbase.apache.org/)
  - [grp:Simplified Data Processing on Large Clusters](http://research.google.com/archive/mapreduce.html)
  - [gcu:Introduction to Parallel Programming and MapReduce](http://code.google.com/intl/zh-CN/edu/parallel/mapreduce-tutorial.html)
  - [gae:MapReduce Overview](https://developers.google.com/appengine/docs/python/dataprocessing/overview)
  - [Data-Intensive Text Processing with MapReduce](http://lintool.github.com/MapReduceAlgorithms/index.html)
  - [Hadoop快速入门](http://hadoop.apache.org/common/docs/r0.19.2/cn/quickstart.html)
- [Spark](http://netscientium.com/in/course/apache-spark/)
- Vitualization
  - [wiki:Hypervisor](http://en.wikipedia.org/wiki/Hypervisor)
  - [wiki:LXC](http://en.wikipedia.org/wiki/LXC)
  - [wiki:cgroups](http://en.wikipedia.org/wiki/Cgroups)
  - [wiki:Virual Machine](http://en.wikipedia.org/wiki/Virtual_machine)
  - [wiki:chroot](http://en.wikipedia.org/wiki/Chroot)
  - [wiki:FreeBSD Jail](http://en.wikipedia.org/wiki/FreeBSD_jail)
  - [wiki:OpenVZ](http://en.wikipedia.org/wiki/OpenVZ)
  - [wiki:Linux-VServer](http://en.wikipedia.org/wiki/Linux-VServer)
  - [wiki:Docker](http://en.wikipedia.org/wiki/Docker_(software))
- DataBase
  - [wiki:SQL](http://en.wikipedia.org/wiki/SQL)
  - [wiki:NOSQL](http://en.wikipedia.org/wiki/NoSQL)
  - [Apache CouchDB](http://couchdb.apache.org/)
  - [Apache Nutch](http://nutch.apache.org/)
  - [redis](http://redis.io/)
  - [LINQ2LDAP](http://linqtoldap.codeplex.com/)
  - [wiki:levelDB](http://zh.wikipedia.org/wiki/LevelDB)
  - [SQLite](http://www.sqlite.org/)
  - [Dblinq](http://dblinq.codeplex.com/)
  - [Consus](http://consus.io/)

- Consistent
  - [raft animation](http://thesecretlivesofdata.com/raft/)

- Test
  - [Testing Distributed Systems for Linearizability](http://www.anishathalye.com/2017/06/04/testing-distributed-systems-for-linearizability/)



High availability
=====
- [MySQL High Availability at GitHub](https://githubengineering.com/mysql-high-availability-at-github/)



