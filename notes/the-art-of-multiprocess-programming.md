NOTES 4: `<<The Art Of Multiprocess Programming>>`

## 原理

### 互斥

**时间**：

- 分析并发计算的实质就是分析时间。有时希望事件同时发生，有时希望事件在不同时间发生。需要对各种复杂情形进行分析，包括多个时间片应该怎样交叉重叠，或者相互之间不允许重叠
- 所有线程共享一个共同的时间（不必是一个公共时钟）。一个线程是一个状态机，其状态的转换称为事件
- 事件是瞬时的：它们在单个瞬间发生。线程A产生了一个事件序列`a(0),a(1),...`
- `a(i,j)`表示`a(i)`事件的第`j`次发生
- 事件`a`在事件`b`之前发生，则称`a`先于`b`，记作`a->b`
- 令`a(0)`和`a(1)`表示事件，且`a(0)->a(1)`，则`I(A)=interval(a(0),a(1))`表示`a(0)`和`a(1)`之间的间隔。
- 如果`a(0)->a(1)->b(0)->b(1)`,則`interval(a(0),a(1))->interval(b(0),b(1))`，即`I(A)->I(B)`
- `I(A,j)`表示`(I,A)`的第`j`次执行

**临界区**:
- 两个线程同时访问`start of danger zone`和`end of danger zone`之间的共享变量，就出现资源竞争
- 某个时刻仅能允许一个线程访问`start of danger zone`和`end of danger zone`之间的资源，则构成了一个临界区
	1. 一个临界区只和一个唯一的Lock对象相关联
	2. 线程准备进入临界区时调用该对象的Lock方法
	3. 当线程离开临界区时调用UnLock方法
- 令CS(A,j)是A第j次执行临界区的时间段

**锁**：
- 互斥：不同线程的临界区之间没有重叠。对于线程`A`、`B`以及整数`j`、`k`，或者`CS(A,k)->CS(B,j)`或者`CS(B,j)->CS(A,k)`
- 无死锁：如果一个线程正在尝试获得一个锁，那么， 总会成功地获得这个锁。若线程A调用Lock但无法获得锁，则一定存在其他线程正在无穷次地执行临界区。
- 无饥饿：每个试图获得锁的线程最终都能成功。每个Lock调用最终都能返回。

**各种锁**：

下述互斥、死锁的证明都是靠对时序采用反证法：假设两个线程能同时进入临界区，引出矛盾。

```
/* 先后执行满足双线程互斥，交叉执行时会出现死锁 */
class lock_one{ 
	bool flag[2];
	void lock(){
		int i = get_current_thread_id();
		int j = 1-i;
		flag[i] = true;
		while(flag[j]);
	}
	void unlock(){
		int i= get_current_thread_id();
		flag[i]=false;
	}
}
```

互斥证明：
- 假设`CS(A)`和`CS(B)`会同时发生
- `write(A,flag[A]=true)->read(A,flag[B]==false)->CS(A)`, A线程进入临界区必然会有前面的`write`和`read`操作
- `write(B,flag[B]=true)->read(B,flag[A]==false)->CS(B)`，B线程进入临界区必然会有前面的`write`和`read`操作
- 根据假设`CS(A)`和`CS(B)`能同时发生，则必然有`read(A,flag[B]==false)->write(B,flag[B]=true)`，否则`flag[B]=true`后，无法进入CS(A)
- 但是，此时上述三个时序将推导出最后一次调用lock时：`write(A,flag[A]=true)->read(A,flag[B]==false)->write(B,flag[B]=true)->read(B,flag[A]==false)`
- 上述时序中，`flag[A]`不可能从`true`变成`false`，矛盾


```
/* 先后执行出现死锁，交叉执行满足双线程互斥 */
class lock_two{ 
	int victim;
	void lock(){
		int i= get_current_thread_id();
		victim = i;
		while(victim==i);
	}
	void unlock(){}
}
```

互斥证明：
- 假设`CS(A)`和`CS(B)`会同时发生
- `write(A,victim=A)->read(A,victim==B)->CS(A)`，A线程进入临界区必然会有前面的时序
- `write(B,victim=B)->read(B,victim==A)->CS(B)`，B线程进入临界区必然会有前面的时序
- 那么，A线程在获得`victim==B`之前，必然需要B线程将victim写成B，所以有`write(A,victim=A)->write(B,victim=B)->read(A,victim==B)`
- 然而，此时B线程将无法得到`victim==A`的结果，从而`CS(B)`无法进入，矛盾

```
/* 混合lock one和lock two，得到无饥饿的双线程互斥算法 */
class lock_peterson{ 
	bool flag[2];
	int victim;
	void lock(){
		int i= get_current_thread_id();
		int j= 1-i;
		flag[i] = true;
		victim = i;
		while(flag[j]&&victim==i);
	}
	void unlock(){
		int i= get_current_thread_id();
		flag[i]=false;
	}
}
```

互斥证明：
- 假设`CS(A)`和`CS(B)`会同时发生
- `write(A,flag[A]=true)->write(A,victim=A)->read(A,flag[B]==false)->read(A,victim==B)->CS(A)`，A线程进入临界区必然会有前面的时序
- `write(B,flag[B]=true)->write(B,victim=B)->read(B,flag[A]==false)->read(B,victim==A)->CS(B)`，B线程进入临界区必然会有前面的时序
- 假设线程A先进入临界区，那么一定有`write(B,victim=B)->write(A,victim=A)`,但是此时
- `write(A,flag[A]=true)->write(B,victim=B)->write(A,victim=A)->read(B,flag[A]==false)`
- 矛盾

上面的都只能做两个线程之间的锁，泛化双线程锁，即可得到多线程锁，多线程锁的互斥、无饥饿证明也用反证法通过时序引出矛盾，但是由于引入多层，需要用数学归纳法。
```
class lock_filter(N){
	int level[N];
	int victim[N];
	bool level_busy(i){
		for(int k=0;k<N;k++){
			if(k!=i){
				if(level[k]>=i){
					return true;
				}
			}
		}
		return false;
	}
	void lock(){
		int me = get_current_thread_id();
		for(int i=1;i<N;i++){
			level[me]=i;
			victim[i]=me;
			while(level_busy(i) && victim[i]==me);
		}
	}
	void unlock(){
		int me = get_current_thread_id();
		level[me] = 0;
	}
}
```

互斥证明：
- 数学归纳法证明对于0到n-1种的整数j，层j上最多有n-j个线程
- 数学归纳法证明每个到达j+1或更高层的线程最终都能进入自己的临界区，推论是没有饥饿
- 无饥饿意味着无死锁

考虑公平性，无饥饿特性能保证每一个调用lock的线程都最终能进入临界区，但并不保证进入临界区需要多长时间。改进的做法是，将lock方法的代码划分为两部分：
1. 门廊区，其执行区间`D(A)`由有限个操作组成。
2. 等待区，其执行区间`W(A)`可能包括无穷个操作步。
由此，引出**公平性**定义：
>满足下面条件的锁称之为先来先服务的：如果线程A门廊区的结束在线程B门廊区的开始之前完成，那么线程A必定不会被线程B赶超。也就是说，对于线程A、B及整数j、k：
若`D(A,j)->D(B,k)`，则`CS(A,j)->CS(B,k)`

Bakery算法：每个线程在门廊区得到一个序号，然后一直等待，直到再没有序号比自己更早的线程尝试进入临界区为止。
```
class lock_bakery(N){
	bool flag[N];
	int label[N];
	void label_less(i,j){
		/**
		 * 定义label[A] label[B]的严格序：(label[A],A)<<(label[B],B)
		 * 可能同时两个线程进入门廊区，所以它们的label会相等，所以增加对下标的比较
		 */
		if(label[i]<label[j]){
			return true;
		}else if(label[i]==lebel[j]){
			return i<j;
		}else {
			return false;
		}
	}
	void label_busy(i){
		for(int k=0;k<N;k++){
			if(k!=i){
				if(flag[k] && label_less(k,i)) ){
					return true;
				}
			}
		}
		return false;
	}
	void lock(){
		int i = get_current_thread_id();

		/* 门廊区 */
		flag[i] = true;
		label[i] = max(label)+1; 

		/* 等待区，直到没有比自己序号更低的线程进入临界区 */
		while(label_busy(i));
	}
	void unlock(){
		/* 由于不重新设置label，所以label是严格递增的，类似面包店发号机 */
		int i = get_current_thread_id();
		flag[i] = false;
	}
}
```

无死锁证明：
- 正在等待的线程中，必定存在某个线程A具有唯一的最小`(label[a],A)`，那么这个线程决不会等待其他线程。

FIFO证明：
- 如果A的门廊先于B的门廊，`D(A)->D(B)`，那么A的label必然小于B的label，因为
- `write(A,label[A])->read(B,label[A])->write(B,label[B])->read(B,flag[A])`
- 所以，当`flag[A]`为true时，B被封锁在外面。

互斥证明：
- 假设`CS(A)`和`CS(B)`同时发生
- 不妨，假设A和B进入临界区的时，`(label[A],A)<<(label[B],B)`
- 则B进入临界区要么`flag[A]==false`,要么`(label[B],B)<<(label[A],A)`，则只能是`flag[A]=false`
- 从而，`write(B,label[B])->read(B,flag[A])->write(A,flag[A])->write(A,label[A])`
- 但此时，`(label[B],B)<<(label[A],A)`，矛盾

**有界时间戳**：
Bakery中的label严格递增，肯定会溢出，这个做法的本质是
- 读取其他线程的时间戳
- 为自己制定一个更晚的时间戳

这是一个时间戳系统：
- 有界
- 无等待

可以构造两类
- 无等待的并发时间戳系统
- 串行时间戳系统


**存储单元数量的下界**：

Bakery算法是简洁、优美且公平的，然而却不实用。核心问题在于：
>要读写n个不同的存储单元，其中n（可能非常大）是并发线程的最大个数。

然而：
>任意一种无死锁的Lock算法在最坏情况下至少要读写n个不同的存储单元；该结论非常重要，以至于我们在多处理器机器中，增加一些功能要比
读写操作更加强大的同步操作，并以这些操作作为互斥算法的基础。

Lock对象状态不一致:
>对于任意一个全局状态，若此刻有某个线程正在临界区内，而锁的状态却与一个没有线程在临界区内或正在尝试进入临界区的全局状态相符，则称该Lock对象
的状态s是不一致的。

>无死锁的算法不可能进入不一致状态，否则如果此时没有线程在临界区或正在尝试进入临界区，如果线程B想要进入临界区，由于算法是无死锁的，它一定能进入；
如果此时A在临界区内，则线程B要进入临界区必须一直阻塞直到A离开临界区，但是B无法确定A是否在临界区内，矛盾。

Lock对象的覆盖状态：
>至少有一个线程欲写所有的共享存储单元，而该Lock对象的存储单元看上去就好像临界区是空的（也就是说，这些存储单元的状态就像是既没有线程在临界区也没有线程正
尝试进入临界区）

3线程存储单元下界：
>任意采用读写存储器方式解决3线程无死锁互斥的Lock算法必须至少使用3个不同的存储单元。

反证法：
- 假设存在一种只使用两个存储单元解决3线程无死锁的Lock算法。
    - 若共享存储单元都是类似Bakery锁的单写存储单元
        1. 在初始状态中，临界区看上去是空的，于是任意线程进入临界区都需要至少写一个存储单元
        2. 则3线程至少需要3个不同的存储单元
    - 若类似Peterson算法的victim啊远多写者存储单元
        1. 令s是一个覆盖的Lock状态，其中A和B分别覆盖不同的存储单元。
        2. 令C单独运行，由于Lock的无死锁特性，C将最终进入临界区，然后让A和B分别修改它们的覆盖存储单元，使该Lock对象处于s'中
        3. 由于没有线程能判断C是否在临界区中，所以s'是非一致的。
    - 那么，如何让A和B进入覆盖状态。
    	1. 考虑B三次进入临界区的情况，每一次通过时，B必须写某个存储单元，所以考虑它进入临界区时第一次写的存储单元，由于只有两个存储单元，B必定对某个单元写了两次，称这个单元为LB
    	2. 让B运行到它准备第一次写LB，若A现在正在运行，则由于B还没写任何信息，因此A将进入临界区，A必须在进入临界区前写LA，否则如果A只写LB，则B写LB后，B无法判断A是否在临界区内，出现不一致。
    	3. 让A一直运行到它第一次写LA，这个状态不是一个覆盖状态，因为A可能已经对LB写了某些信息以提示线程C它要进入临界区。
    	4. 让B运行，冲掉A向LB写入的所有内容，最多三次进入临界区，且恰好在第二次写LB暂停
    	5. 在这种状态下，A准备写LA、B写LB，并且存储单元是一致的，正如覆盖状态所需的那样。

### 并发对象

**顺序**：
- 并发对象的正确性基于某种与顺序行为等价的概念，不同的概念适用于不同的系统
    - 静态一致性
    - 顺序一致性
    - 可线性化
- 在正确性保障的多个维度空间上，方法的不同实现提供了各种不同的演进保障
    - 可阻塞：任一线程能够延迟其他线程
    - 非阻塞：一个线程的延迟不能延迟其他线程
- 正确和性能
    - 如果能将并发执行转为顺序执行，则只需对该顺序执行进行分析，从而简化并发对象的分析。这是关于正确性的关键准则
    - Amdah1定律：持有互斥锁的并发对象，不如具有细粒度锁或者根本没有锁的对象令人满意

**方法调用**：
- 方法调用需要时间。方法调用是一段时间间隔，从调用事件开始直到响应事件结束。
- 并发线程的方法调用可以相互重叠，而单线程的方法调用总是顺序无重叠的。
- 如果一个方法的调用事件已发生，但其响应事件还未发生，则称这个方法调用是未决的。

**可复合**：
- 对于正确性P，如果系统中每个对象都满足P，则整个系统也满足P，那么P是可复合的。

**静态一致性**：
- 方法调用应呈现出以某种顺序执行且每个时刻只有一个调用发生。
- 由一系列静止状态分隔开的方法调用应呈现出与按照它们的实时调用次序相同的执行效果
- 静态一致性是可复合的。

例子：
>假设对于一个FIFO队列，A和B并发地将x和y入队，然后队列变为静态的，C再使z入队，队列中x和y的相对次序可能无法确定，但是可以肯定它们都在z的前面。


**顺序一致性**：
- 方法调用应呈现出以某种顺序执行且每个时刻只有一个调用发生。
- 方法调用应该呈现出按照程序次序调用的执行效果
- 静态一致性是不可复合的。

例子：
>线程A和线程B分别同时入队x和y，然后，A和B分别同时出队y和x。有两种可能的顺序说明它们的结果：
（1）A入队x，B入队y，B出队x，A出队y
（2）B入队y，A入队x，A出队y，B出队x
这两种次序都与程序次序一致，其中任意一种都足以说明该执行是顺序一致的。

不可复合：
```
A:--p.enq(x)--------q.enq(x)--------p.deq(y)----------
B:----------q.enq(y)--------p.enq(y)--------q.deq(x)--
```
p和q单独看都符合顺序一致，但，如果存在一种重排，使得p、q的复合是顺序一致的，也就是方法的调用能够以一种与程序次序相一致的次序进行排序，
我们用标记：`<p.enq(x),A> -> <q.deq(x),B>`表示任意的顺序执行必须使得A对p的入队x操作先于B对q的出队x操作

程序次序说明：
- `<p.enq(x),A> -> <q.enq(x),A>`
- `<q.enq(y),B> -> <p.enq(y),B>`

复合顺序一致要求：
- `<p.enq(y),B> -> <p.enq(x),A>` 因为p是FIFO且A从p中出队y，则y肯定在x之前入队
- `<q.enq(x),A> -> <q.enq(y),B>` 同理

但这四个构成一个环，矛盾

**对比**:
顺序一致性和静态一致性之间是不可比的：
- 存在非静态一致但却是顺序一致
- 存在非顺序一致但却是静态一致
静态一致性中不需要保持程序次序，而顺序一致也不受静止状态周期的影响

处理器：
- 大多数现代处理器系统结构中，存储器读写都不是顺序一致，允许重排
- 如果需要顺序一致，需要通过内存栅栏等显式申请

**可线性化**：
- 方法调用应呈现出以某种顺序执行且每个时刻只有一个调用发生。
- 方法调用应该呈现出一种与它的调用和响应之间的某个时刻的行为相同瞬时效果
- 静态一致性是可复合的。

用来说明并发对象是可线性化的一种常用办法就是对每个方法在它生效的那个地方指定一个可线性化点。
- 对于锁来说，每个方法的临界区可以当作它的可线性化点。
- 对那些不适用锁的实现，可线性化点通常是该方法调用的结果对其他方法调用可见时的那个操作步。

例子：
```
class WaitFreeQueue<T>{
	int head=0;
	int tail=0;
	T items[];
	int enq(T x){
		if(tail-head==items.length){
			return RESULT_FULL;	
		}else{
			items[tail%items.length]=x;
			tail++;
			return RESULT_SUCCESS;
		}
	}
	T deq(){
		if(tail-head==0){
			return NULL;
		}else{
			T x = items[head%items.length];
			head++;
			return x;
		}
	}
}
```
这个例子里，分析下deq的可线性化点，enq类似：
- 若队列非空，deq返回一个元素，则head++是可线性化点
- 若队列为空，deq返回NULL是可线性化点

可线性化非常适合大型系统的组建，在这种系统中各个组件必须独立地实现和验证。此外，并发对象所用的技术全都是可线性化的，可线性化是非阻塞又是可复合的。

**形式化**：
- 方法的一次调用：`<x.m(a*),A>`，其中x是对象，m是方法名，a*是参数，A是线程
- 方法调用的一次响应：`<x.t(r*),A>`，其中x是对象，t是响应，r*是结果序列，A是线程
- 若一次调用于一个响应都具有相同的对象和线程，则称这个响应匹配这次调用
- 并发系统的一次执行过程可以采用经历（History）模型来描述，经历是一方法调用事件和响应事件的一个有限序列
- 经历H的子经历就是H的事件序列的一个子序列
- 方法调用：经历H中的一个方法调用是一个二元组，它由H中的一个调用和一个紧接其后且与其相匹配的响应组成
- 在H中，若一个调用还没有与之匹配的响应，则该调用是未决的

可线性化隐含的基本思想就是：每一个并发经历都等价于一个顺序经历。基本准则：
- 如果一个方法调用先于另一个方法调用，则较早的调用必定在较晚的调用之前生效
- 如果两个方法调用彼此重叠，则它们的次序将无法确定，可以按照任意次序重排。

形式化描述：
>如果经历H存在一个扩展H',以及一个合法的顺序经历S，并使得
（1）`complete（H')`与S等价，且
（2）若H中方法调用m0先于m1，则在S中也成立
则称H是可线性化的。


### 共享存储基础

**顺序计算**:
>顺序计算基础是由Alan Turing和Alonzo Church在20世纪30年代所奠定的，他们各自独立地提出了`丘奇-图灵`论题：
能被计算的所有事情，都能由图灵机（或等价的丘奇lambda演算）计算。任何由图灵机不能解的问题（比如判定一个程序
对任意的输入是否会停机），对实际的计算设备也是不可解的。图灵论题是一个论题而不是定理，因为“什么是可计算”这一概念
不可能用精确的、数学上严谨的方式来定义。

顺序计算理论的发展过程：
- 有穷自动机
- 下推自动机
- 图灵机

一个共享存储器的计算由多个线程构成，每个线程自身是一个顺序程序，它们相互之间通过驻留在共享存储器中的对象进行通信。线程是异步的，它们各自以不同的速度执行，
且在任意时刻可以停止一个不可预测的时间间隔。

**寄存器空间**：
理解线程间通信的一种办法就是对硬件原语进行抽象，把通信看作是通过读/写`并发共享对象`实现。

一种说明方式就是依靠互斥来定义并发方法调用：使用互斥锁来保护每个寄存器，要求每次read和write调用都必须首先获得锁，然而不幸的是，在多处理器系统结构中，不能使用互斥来实现共享对象的并发调用：
- 上一章讲述了如何利用寄存器来实现互斥，因此再通过互斥来实现寄存器几乎没有任何意义
- 若通过互斥来实现寄存器，即使这种实现是无死锁或无饥饿的，计算的演进仍然依赖于OS的调度器

如果对象的每个方法调用都能在有限步内完成，并且每个方法调用执行都与它和其他并发的方法调用之间的交互无关，则称这个对象的实现是无等待的。

**原子寄存器**：
所谓原子寄存器就是顺序寄存器类的一种可线性化实现，原子寄存器每一个读操作都返回“最后”一次写入的值。各个线程通过读/写原子寄存器进行通信的模型很久以来一直作为并发计算的标准模型。
- 单读者-单写者：SRSW
- 多读者-多写者：MRMW

**安全寄存器**:
一个单写者-多读者寄存器的实现是“安全”的，如果：
- 与write调用不相重叠的read调用，其返回值是最近一次write调用所写入的值
- 如果read和write调用相互重叠，则read调用可以返回该寄存器所允许范围内的任意值

**规则寄存器**:
规则寄存器是一种多读者-单写者寄存器，其中写操作不是原子的，
- 当write调用正在执行的时候，若旧值还没有完全被新值所替代，那么读到的值可能在旧值和新值之间“闪动”
- 规则寄存器是安全的，因此任意一个不与write调用重叠的read调用都返回最近一次被写入的值
- 假设一个read调用与一个多个write调用重叠，另v^0是最后一次write调用所写入的值，v^1,v^2,...,v^k为重叠的write调用所写入的值的序列，那么read调用可能返回任意的v^i

那么：
- 规则寄存器必定是静态一致的，但反过来不成立
- 规则寄存器是单写者顺序寄存器

为了便于分析规则和原则寄存器的实现算法，从现在开始只考虑这样的经历：
- 各个read调用返回的值必定是被某个write调用写入的值
- W(i)->W(j)
- R(i)->R(j)

可以对规则寄存器和原子寄存器形式化：

1. 绝不可能存在R(i)->W(i)，读调用不会返回将来的值
2. 对于某个值j，绝不会存在W(i)->W(j)->R(i)，读调用不会返回更远的过去值
3. 若R(i)->R(j)，则i<=j

满足1,2的是规则寄存器，满足规则1,2,3的是原子寄存器

利用安全SRSW构造安全MRSW
```
class SafeMRSWRegister(capacity){
	bool s_table[capacity]; // array of safe SRSW registers
	bool read(){
		return s_table[get_current_thread_id()];
	}
	void write(bool x){
		for(int i=0;i<s_table.length;i++){
			s_table[i] = x;
		}
	}
}
```
- 若线程A的read调用不与任意write重叠，那么read返回最后一次写入s_table的值
- 若线程A的read调用和write重叠，则对多个SRSW的复合结果还是安全的

利用安全MRSW构造规则MRSW
```
class RegularMRSWRegister(capacity){
	therad_local<bool> last;
	bool s_value; // safe MRSW register
	bool read(){
		return s_value;
	}
	write(bool x){
		if(x!=last.get()){
			last.set(x);
			s_value = x;
		}
	}
}
```
对于bool寄存器而言，只有当要写入的新值x与久值一样时，安全的bool寄存器与规则的bool寄存器之间才会有区别：
- 规则寄存器只能返回x
- 安全寄存器可以返回任意一个bool值
因此，只要确保写入的新值与原先写入值不同时才允许修改，就可以解决这个问题
- 如果一个read带哦用不与任何write调用重叠，则返回最近一次写入的值
- 若调用有重叠，则要考虑
    - 如果要写入的值与最后一次写入的值相同，那么写者不对该安全寄存器写，从而确保读者读到正确的值
    - 如果要写入的值与最后一次写入的值不公，则由于是bool寄存器，那么值必定为true或false，并发的读者将
    返回寄存器取值范围呢id某个值，或者是true，或者是false，且都是正确的，因此是规则的。

NOTE：这个证明太精彩了！

进而，可以构造M-值MRSW规则寄存器
```
class RegularMRSWRegister(capacity){
	int RANGE = Byte.MAX_VALUE-Byte.MIN_VALUE+1;
	bool r_bit[RANGE]; // regular bool MRSW
	RegularMRSWRegister(){
		r_bit[0]=true;
	}
	Byte read(){
		for(int i=0;i<RANGE;i++){
			if(r_bit[i]){
				return i;
			}
		}
		return -1;
	}	
	void write(Byte x){
		r_bit[x] = true;
		for(int i=x-1;i>0;i--){
			r_bit[i]=false;
		}
	}
}
```
先证明read调用总能返回一个值，该值对应于0-M-1之间由某个write调用所设置的一个位。若一个读者正在读r_bit[j]，则必定有某个索引号大于等于j的位被一个write调用设置为true，这可以使用数学归纳法证明。

接着看如何使用SRSW规则寄存器构造SRSW原子寄存器。由于SRSW规则寄存器并不存在并发读，所以违背原子寄存器要求3的情景是：
两个读与一个写重叠且读到次序颠倒的值。为此，我们加入时间戳解决这个问题：
```
class AutomicSRSWRegister<T>{
	thread_local<long> last_stamp;
	thread_local<stamp<T>> last_read;
	stamp<T> r_value;  // regular SRSW timestmp-value pair
	T read(){
		stamp<T> value = r_value;
		stamp<T> last = last_read.get();
		stamp<T> result = max_bystamp(value,last); // get max stamp for current value and last read value
		last_read.set(result);                     // save last read value
		return result.value;
	}
	void write(T v){
		long now = last_stamp.get()+1;
		r_value = new stamp<T>(now,v); // save value and stamp
		last_stamp.set(now);           // save last write stamp
	}
}
```

MRSW原子寄存器，做法是通过2维度的时间戳矩阵
```
class AtomicMRSWRegister<T>(N){
	thread_local<long> last_stamp; // each entry is SRSW atomic
	stamp<T> a_table[N][N];
	T read(){
		int me = get_current_thread_id();
		stamp<T> value = a_table[me][me];
		for(int i=0;i<a_table.length;i++){
			value = max_bystamp(value,a_table[i][me]);
		}
		for(int i=0;i<a_table.length;i++){
			a_table[me][i] = value;
		}
		return value;
	}
	void write(T v){
		long stamp = last_stamp.get()+1;
		stamp<T> value = new stamp<T>(stamp,v);
		for(int i=0;i<a_table.length;i++){
			a_table[i][i] = value;
		}
	}
}
```

MRMW原子寄存器：
```
class AtomicMRMWRegister<T>(N){
	stamp<T> a_table[N]; // array of atomic MRSW register
	T read(){
		stamp<T> max = stamp.MIN_VALUE;
		for(int i=0;i<a_table.length;i++){
			max = max_bystamp(max,a_table[i]);
		}
		return max.value;
	}
	void write(T v){
		int me = get_current_thread_id();
		stamp<T> max = stamp.MIN_VALUE;
		for(int i=0;i<a_table.length;i++){
			max = max_bystamp(max,a_table[i]);
		}
		a_table[me] = new stamp<T>(max.stamp+1,v);
	}
}
```

抽象的力量，虽然这些实现并没有效率，但是在理论上，我们知道了如何一步步从两个维度层层逼近原子性：

1. 安全->规则->原子
2. 单写单读->单写多读->多读多写

在这两个维度做叉乘，步步逼近

**原子快照**:
同样的，既然搞定了单个寄存器的值，就接着搞原子地读多寄存器的值，称为原子快照，快照应该等价于如下的顺序规范
```
class SeqSnapshot<T>(N){
	T a_value[N];
	void update(T v){
		autolock();
		a_value[get_current_thread_id()] = v;
	}
	T[] scan(){
		autolock();
		T result[N];
		for(int i=0;i<a_value.length;i++){
			result[i]=a_value[i];
		}
		return result;
	}
}
```

无障碍快照：update无等待，sacn无障碍
```
class SimpleSnapshot<T>(N){
	stamp<T> a_table[N]; // array of atomic MRSW registers
	void update(T v){
		int me = get_current_thread_id();
		stamp<T> old = a_table[me];
		stamp<T> new = new stamp<T>(old.stamp+1,v);
		a_table[me]=new;
	}
	stamp<T>[] collect(){
		stamp<T> copy[N];
		for(int j=0;j<a_table.length;j++){
			copy[j] = a_table[j];
		}
		return copy;
	}
	T[] scan(){
		stamp<T> olds[N],news[N];
		olds = collect();
		while(true){
			news = collect();
			if(!olds.equal(news)){ // 比较时间戳
				olds = news;
				continue;
			}
			// 双重干净收集，可以返回
			T result[N];
			for(int i=0;i<a_table.length;i++){
				result[i]=news[i].value;
			}
			return result;
		}
	}
}
```

无等待快照：若一个正在扫描的线程A在它进行重复收集的同时看到线程B迁移两次，则B必定在A的scan过程中执行了一次完整的update调用，这样A可以正确的使用B的快照
```
class WFSnapshot<T>(N){
	stamp<T> a_table[N]; // array of atomic MRSW registers
	void update(T v){
		int me = get_current_thread_id();
		stamp<T> old = a_table[me];
		stamp<T> new = new stamp<T>(old.stamp+1,v);
		a_table[me]=new;
	}
	stamp<T>[] collect(){
		stamp<T> copy[N];
		for(int j=0;j<a_table.length;j++){
			copy[j] = a_table[j];
		}
		return copy;
	}
	T[] scan(){
		stamp<T> olds[N],news[N];
		bool moved[N];
		olds = collect();
		while(true){
			news = collect();
			for(int j=0;j<a_table.length;j++){
				if(olds[j].stamp!=news[j].stamp){ // 比较时间戳
					if(moved[j]){                 // 如果看到两次更新，就用它的快照
						return olds.snap;
					}else{
						moved[j]=true;            // 否则就先记录下
						olds = news;
						continue;
					}
				}
			}

			// 双重干净收集，可以返回
			T result[N];
			for(int i=0;i<a_table.length;i++){
				result[i]=news[i].value;
			}
			return result;
		}
	}
}
```

看到这里，感觉好精彩，欲罢不能，接着往下看好了。

### 同步原子操作的相对能力

下面这组话，几乎没法删减：

>假设你在负责设计一种新的多处理器，应该将那种原子指令包含进来呢？我们的目标是找出一组基本的同步操作原语，用于解决实际中可能出现的各种同步问题。

>如果一个并发对象的每次调用都能在有限步内完成，则称该并发对象的实现是无等待的。如果能保证某个方法的无限次调用都能在有限步内完成，则称该方法是无锁的。

>评测同步指令能力的一种方法就是评价同步指令对于共享对象（如队列、栈、树）的实现的支持程度如何。我们只评测那些能够保证状态的演进不依赖外界支持的解决方法。

>如果把同步原子指令看作是其对外的方法就是指令本身的对象，则可以证明存在一种由同步原语组成的无限层次的层次结构，任一层的原语都不能用在更高层原语的无等待或无锁实现中。

>定义一致数：在这种层次中，每个类都有一个相关的一致数，所谓的一致数就是这个类的对象解决基本的同步问题时所能针对的最大线程数。

>在一个有n个或更多个并发线程的系统中，不可能使用一致数小于n的对象构造一个一致数wein的对象的无等待或无锁实现。

**一致数**：
```
class Consensus<T>{
	T decide(T v);
}
```
每个线程以输入v最多调用decide一次，一致性对象的decide方法将返回一个满足下列条件的值
- 一致性：所有的线程都决定同一个值
- 有效性：这个共同决定值是某个线程的输入

换句话说并发一致性对象可以被线性化为一个串行一致性对象，其中值被选中的线程首先完成它的decide调用。
- 如果存在一种使用类C的任何数量的对象和原子寄存器的一致性协议，则能够解决n线程一致性问题
- 类C的一致数是指用这个类来解决n线程一致性时所能针对的最大n值，如果n值不存在，则称这个类的一致数是无限的。
- 假设类C的一个对象可以通过类D的一个或多个对象以及一定数量的原子寄存器实现，如果类C可以解决n线程一致性，那么类D也可以

**状态和价**:
每个线程在调用decide之前都处于一个状态，调用decide称之为一次迁移，初始状态是指所有线程开始迁移之前的协议状态，结束状态是指所有线程结束以后的协议状态。

考虑最简单的情形：双线程的二进制一致性（输入为0或1）
```
// 此处书上只画了个二叉示意图，不是很清楚每个节点的迁移，待确定
A(0)->A(1)->B(1)->B(1)
A(0)->B(1)->A(1)->B(1)
A(0)->B(1)->B(0)->A(0)

B(1)->A(1)->A(1)->B(1)
B(1)->A(1)->B(1)->A(1)
B(1)->B(1)->A(1)->A(1)
```

推论：
- 每个双线程一致性协议都存在一个二价的初始状态
- 每个n线程一致性协议都存在一个二价的初始状态
- 每个无等待的一致性协议都有一个临界状态（二价+如果任何一个线程迁移，改协议状态将变为单价的）

**原子寄存器的一致性为1**
证明：假设存在一个针对线程A、B的二进制一致性协议，只有三种可能
- A准备读，B可能读写同一个寄存器或者读写不同寄存器，1-s'和0-s''对B来说是不可区分的，A的读只改变自己的局部线程状态，对B是不可见的
```
s->A读->B执行一个操作->0-s''->B独奏->0
s->B执行一个操作->1-s'->B独奏->1
```
- A和B写不同的寄存器，A和B无法确定对方是否比自己先写，但结果却有两种，矛盾
```
s->A写r0->B写r1->0-s'
s->B写r1->A写r0->1-s'
```
- A和B写同一个寄存器，问题是B独奏的时候无法区分0-s''和1-s''，因为之前B都写过r，插除了A的痕迹，但结果却有两种，矛盾
```
s->A写r->B写r->0-s'->B独奏->0-s''
s->B写r->1-s''->B独奏->1-s''
```

**一致性协议**：
```
class ConsensusProtocol<T>(N){
	T proposed[N];
	
	// announce my input value to the other threads
	void propose(T v){
		proposed[get_current_thread_id()]=v;
	}

	// figure out which thread was first
	T decide(T v);
}
```

**FIFO**:
双出队列者FIFO队列类的一致性数至少为2，例如
```
class BinaryQueueConsensus<T>(N) : extends ConsensusProtocol<T>(N){
	int WIN =0;
	int LOSE = 1;
	Queue queue={WIN,LOSE};
	T decide(T v){
		propose(v); // 
		int status = queue.deq();
		int i = get_current_thread_id();
		if(status==WIN){
			return proposed[i];
		}else{
			return proposed[1-i];
		}
	}
}
```
If p.deq(WIN,A)->p.deq(LOSE<B), then decide v0 input from A, for example:
```
p.propose(v0,A)->p.propose(v1,B)->p.deq(WIN,A)->p.decide(v0,A)->p.deq(LOSE,B)->p.decide(v0,B)
```
ELSE decide input from B, for example:
```
p.propose(v0,A)->p.propose(v1,B)->p.deq(WIN,B)->p.decide(v1,B)->p.deq(LOSE,A)->p.decide(v1,A)
```

推论1：不能用一组原子寄存器构造出针对双出队者的FIFO队列的无等待实现
>证明：如果每个线程都返回它自己的输入，那么他们必须都让WIN出队，这将违背FIFO队列的定义
如果每个线程都返回其他线程的输入，那么它们必须都让LOSE出队，同样也违背了FIFO队列的定义
因此，让WIN出队的线程，在任何值出队之前必须已经把自己的输入存放在数组proposed[]中。（原书里就是这段话，此处不理解，
是否因为用了proposed数组，而这个数组是多值的，根据上一章，而原子快照只对单个域赋值对多个域原子读是有等待的，应该是这样的，书上这个不写明导致理解困难）

推论2：一组原子寄存器不可能构造队列、栈、优先级队列、集合或链表的无等待实现

FIFO队列的一致数为2，反证法：假设存在一个针对线程A、B、C的一致性协议，则该协议会有一个临界状态s，假设A的下一个迁移将使该协议达到一个0-一价状态，而B的下一个
迁移将使该协议达到一个1-一价状态

A和B的未决迁移不能交换，着意味着它们都将调用同一对象的方法，FIFO里只能调队列方法。

```
// A和B都掉deq，则C无法区分
s->A.deq->B.deq->0-s'->C独奏->0-s''
s->B.deq->A.deq->1-s'->C独奏->1->s''

// A和B分别掉enq和deq，
// 如果队列非空，由于enq和的deq可交换（在队列的开头和结尾），C无法区分
s->A.enq->B.deq->C独奏
s->A.enq->C独奏

// A和B分别掉enq和deq，
// 如果队列为空，则同样C不可区分，B的deq对C不可见
s->B.deq->A.enq->C独奏
s->A.enq->C独奏

// A和B都enq，则会有如下C不区分的分支
s->A.enq(a)->B.enq(b)->运行A直到deq(a)->运行B直到deq(b)->C独奏
s->B.enq(b)->A.enq(a)->运行A直到deq(b)->运行B直到deq(a)->C独奏

```

**多重赋值对象**：
令n>=m>1，给定一个具有n个域的对象，多重赋值接口如下：

- assign(T v0,T v1,...,T vm, int i0, int i1,...,int im);
- read(int i);

该问题是原子快照的对偶问题，在原子快照对象中，是对单个域赋值而对多个域进行原子地读，由于快照可以采用读写寄存器实现，所以隐含地说明
快照对象的一致数为1.

多重赋值对象的一个例子
```
class Assign23{
	int r[3];
	void assign(T v0, T v1, int i0, int i1){
		r[i0]=v0;
		r[i1]=v1;
	}
	int read(int i){
		return r[i];
	}
}
```

对任意的n>m>1,不存在通过原子寄存器构造的(m,n)-赋值对象的无等待实现。
证明:
>对于一个给定的(2,3)-赋值对象以及两个线程，只需证明能够解决双线程一致性即可。
>如下代码，A线程写0，1域，B线程写1，2域

```
class MultiConsensus<T>(N) : extends ConsensusProtocol<T>(N){
	Assign23 assign2;
	T decide(T v){
		int i = get_current_thread_id();
		int j = 1 - i;
		// double assignment
		int other = assign23.read((i+2)%3);
		if(other==NULL||other==assign23.read(1)){
			return proposed[i]; // I win
		}else{
			return proposed[j]; // I lose
		}
	}
}
```

对于n>1，原子的(n,n(n+1)/2)寄存器赋值的一致数至少为n
证明：
>同MultiConsensus做法，设置n个域，i线程写的寄存器标记为ri，一共有n(n-1)/2个域rij使得i和j线程都在写

**读-改-写操作**:
由多处理器硬件所提供的大多数经典同步操作可以表示为读-改-写（RMW）操作。考虑一个将整数值封装起来的RMW寄存器，令F为从整数到整数的映射函数集。
对于某个f in F，如果一个方法能够用f(v)原子的替换寄存器的当前值v，并返回寄存器的先前值，则称该方法是一个对于函数集F的RMW操作。
例如java.util.concurrent下的
- getAndSet
- getAndIncrement
- getAndAdd
- compareAndSet

由于RMW方法有可能成为潜在的硬件原语，所以人们十分关注这些被刻在硅片上而不是刻在石头上的RMW方法的研究工作。

如果在一个RMW方法的函数集中至少包含一个非恒等函数，那么该方法是非平凡的。
非平凡RMW寄存器的一致数至少为2
证明：
>考察双线程一致性协议，由于在F中必定存在f为非恒等函数，那么必定存在值v使得f(v)!=v。在decide方法中，propose(v)先将线程的输入v写入
数组proposed[]中，然后每个线程对一个共享寄存器调用该RMW方法，如果线程的调用返回v，那么它被线性化为第一个，并且决定自己的值，否则，它
被线性化为第二个，并且决定另一个线程的值。

```
class RMWConsensus<T>(N) extends ConsensusProtocol<T>(N){
	RMWRegister r;
	T decide(T v){
		propose(v);
		int i = get_current_thread_id();  // my index
		int j = 1-i;                      // other's index
		if(r.rmw()==v){ 
			return proposed[i];           // I win
		}else{
			return proposed[j];           // I lose
		}
	}
}
```

对于两个或两个以上的线程，使用原子寄存器不可能为它们构造出任意非平凡RMW方法的无等待实现。

**Common2 RMW 寄存器**:
这种寄存器是20世纪末大多数处理器所支持的常用同步原语，虽然Common2寄存器和非平凡的RMW寄存器一样，具有比原子寄存器更为
强大的能力，但仍能证明它的一致数恰好为2.

对于任意的值v以及函数集F中的函数fi和fj，如果它们满足下面的条件之一：
- fi和fj可交换：fi(fj(v))=fj(fi(v))
- 一个函数重写另一个函数：fi(fj(v))=fi(v)或fj(fi(v))=fj(v)
则称函数集F属于Common2

如果一个RMW寄存器的函数集F属于Common2，则该寄存器也属于Common2

getAndSet使用常量函数来重写任意的先前值，getAndIncrement和getAndAdd方法是用了可交换函数

Common2RMW寄存器不能解决3线程一致性问题：
第一个获胜线程总能识别出它是第一个，第二个和第三个失败线程也能够识别出它们是失败者，然而由于
用来定义Common2操作后的协议状态的函数是可交换或可重写的，因此失败者无法识别出其他线程中的哪个首先执行，又因为
协议是无等待的，所以它不可能一直等待直到找出哪个是获胜者为止。

**compareAndSet**：
或者compareAndSwap，是多种现代系统结构所支持的一种同步操作，将期望值和更新值作为参数，如果寄存器的当前值等于期望值，则用更新值替换，否则值不变。

能够支持compareAndSet和get方法的寄存器，其一致数是无限的。
```
class CASConsensus<T>{
	int FIRST=-1;
	AtomicInterger r(FIRST);
	T decide(T v){
		propose(v);
		int i = get_current_thread_id();
		if(r.compareAndSet(FIRST,i)){ // I won
			return proposed[i];
		}else{                        // I lose
			return proposed[r.get()];
		}
	}
}
```

仅支持compareAndSet方法的寄存器具有无限的一致数，这种原语操作的机器是顺序计算图灵机异步计算等价机器：
对于任意的并发对象，如果它是可实现的，则必定能在这种机器上以无等待的方式实现。

用Maurice Sendak的话说，compareAndSet是“万物之首”

NOTE：如果不看这些原理，compareAndSet也就只是囫囵吞枣过去了

### 一致性的通用性

## 实践

### 自旋锁与争用

### 管道和阻塞同步

### 链表：锁的作用

### 并行队列和ABA问题

### 并发栈和消除

### 计数、排序和分布式协作

### 并发哈希和固有并行

### 跳表和平衡查找

### 优先级队列

### 异步执行、调度和工作分配

### 障碍

### 事物内存