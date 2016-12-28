回顾下单机操作系统的核心：

- 输入输出设备
- 存储器
- CPU，对进程调度

作为调度器（Scheduler），对应的是CPU的角色，CPU的简易模型：

- 计算资源：多处理器
- 调度对象：进程任务

根据消耗资源的类型，分类如下：

- 计算密集型（CPU bound)
- IO密集型（IO bound）

两个核心指标：

- 性能指标（Performance）：turn around time = job complete time -  job arrive time
- 公平指标（Fairness）：response time = job start - job arrive

考虑单处理器的情况，对进程任务属性的假设

- 假设1：所有的Job耗时相同
- 假设2：所有的Job同时到达
- 假设3：所有的Job一旦执行，就得执行完。
- 假设4：所有的Job只使用CPU（没有IO等其他资源消耗）
- 假设5：所有的Job运行的耗时时长已知

逐步松弛，考察不同假设下的调度策略

- 5个假设全部满足，并且只考虑turnaround time指标，得到First In First Out (FIFO) 调度策略，average turn around time 不理想。
- 放开假设1，允许A、B、C的耗时不同，得到Shortest Job First (SJF) 调度策略，性能指标有所改进，但是当耗时任务先到时，性能指标变差
- 放开假设3，允许Job没执行完就被暂停，得到Shortest Time-to-Completion First (STCF)调度策略，改进了上一个策略的确定后，性能指标得到改进

然而，这三种调度策略的response time都不理想，公平性不好

- 引入Round Robin(RR)策略，通过timer interrupt为每个Job执行一个时间片，然后执行下一个，直到所有的任务都完成，但是RR的性能指标则不怎么样
- 引入Overlap策略，放开假设4，允许有IO操作，在两个CPU操作之间的间隙，可以运行IO密集型任务

综合上述，合并后的调度策略是多层反馈队列（Multi-Level Feedback Queue）：

- Rule 1: If Priority(A) > Priority(B), A runs (B doesn’t).
- Rule 2: If Priority(A) = Priority(B), A & B run in RR.
- Rule 3: When a job enters the system, it is placed at the highest priority (the topmost queue).
- Rule 4: Once a job uses up its time allotment at a given level (regardless of how many times it has given up the CPU), its priority is reduced (i.e., it moves down one queue).
- Rule 5: After some time period S, move all the jobs in the system to the topmost queue.

问题的核心是，如何给一个Task定义优先级：
>The key to MLFQ scheduling therefore lies in how the scheduler sets
priorities. Rather than giving a fixed priority to each job, MLFQ varies
the priority of a job based on its observed behavior

上面的规则就是通过观察Task的运行状态调整Task的优先级。

现在考虑多处理器的情况，运行队列的设计

- 多个处理器共享一个运行队列，粒度太大，造成单点瓶颈

首先，改进版本：

- 每个处理器独立一个运行队列，极大减少了阻塞

其次，双缓存优先级队列的设计

- 根据优先级的激活队列
- 激活队列里的task时间片耗尽，进入过期队列
- 如果激活队列为空，则交换过期队列和激活队列
- 或者，对过期队列调度，适式让过期队列里的task进入激活队列

然后，两种新的调度策略

- Rotating Staircase Deadline (RSDL) 
- Staircase Deadline (SD)

SD最重要的贡献是：
>His new schedulers proved the fact that **fair scheduling** among processes can be **achieved without any complex computation**.

进而，有：

- Completely Fair Scheduler (CFS)

>CFS scheduler was a big improvement over the existing scheduler not only in its performance and interactivity but also in simplifying the scheduling logic and putting more modularized code into the scheduler. 

For what:

- the CFS maintains the amount of time provided to a given task in what's called the virtual runtime
- The smaller a task's virtual runtime—meaning the smaller amount of time a task has been permitted access to the processor
- the higher its need for the processor.
- The CFS also includes the concept of sleeper fairness to ensure that tasks that are not currently runnable

How to:

- the CFS maintains a time-ordered red-black tree

# references
- http://pages.cs.wisc.edu/~remzi/OSTEP/cpu-sched.pdf
- http://pages.cs.wisc.edu/~remzi/OSTEP/cpu-sched-mlfq.pdf
- http://www.cs.montana.edu/~chandrima.sarkar/AdvancedOS/CSCI560_Proj_main/
