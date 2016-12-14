NOTES 4: `<<The Art Of Multiprocess Programming>>`

## 原理

### 互斥

**时间**：

- 分析并发计算的实质就是分析时间。有时希望事件同时发生，有时希望事件在不同时间发生。需要对各种复杂情形进行分析，包括多个时间片应该怎样交叉重叠，或者相互之间不允许重叠
- 所有线程共享一个共同的时间（不必是一个公共时钟）。一个线程是一个状态机，其状态的转换称为事件
- 事件是瞬时的：它们在单个瞬间发生。线程A产生了一个事件序列a0,a1,...
- aij表示ai事件的第j次发生
- 事件a在事件b之前发生，则称a先于b，记作a->b
- 令a0和a1表示事件，且a0->a1，则IA=interval(a0,a1)表示a0和a1之间的间隔。
- 如果a0->a1->b0->b1,則interval(a0,a1)->interval(b0,b1)，即(I,A)->(I,B)
- I(A,j)表示(I,A)的第j次执行

**临界区**:
- 两个线程同时访问`start of danger zone`和`end of danger zone`之间的共享变量，就出现资源竞争
- 某个时刻仅能允许一个线程访问`start of danger zone`和`end of danger zone`之间的资源，则构成了一个临界区
	1. 一个临界区只和一个唯一的Lock对象相关联
	2. 线程准备进入临界区时调用该对象的Lock方法
	3. 当线程离开临界区时调用UnLock方法
- 令CS(A,j)是A第j次执行临界区的时间段

**锁**：
- 互斥：不同线程的临界区之间没有重叠。对于线程A、B以及整数j、k，或者CS(A,k)->CS(B,j)或者CS(B,j)->CS(A,k)
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
- 假设CS(A)和CS(B)会同时发生
- write(A,flag[A]=true)->read(A,flag[B]==false)->CS(A), A线程进入临界区必然会有前面的write和read操作
- write(B,flag[B]=true)->read(B,flag[A]==false)->CS(B)，B线程进入临界区必然会有前面的write和read操作
- 根据假设CS(A)和CS(B)能同时发生，则必然有read(A,flag[B]==false)->write(B,flag[B]=true)，否则flag[B]=true后，无法进入CS(A)
- 但是，此时上述三个时序将推导出最后一次调用lock时：write(A,flag[A]=true)->read(A,flag[B]==false)->write(B,flag[B]=true)->read(B,flag[A]==false)
- 上述时序中，flag[A]不可能从true变成false，矛盾


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
- 假设CS(A)和CS(B)会同时发生
- write(A,victim=A)->read(A,victim==B)->CS(A)，A线程进入临界区必然会有前面的时序
- write(B,victim=B)->read(B,victim==A)->CS(B)，B线程进入临界区必然会有前面的时序
- 那么，A线程在获得victim==B之前，必然需要B线程将victim写成B，所以有write(A,victim=A)->write(B,victim=B)->read(A,victim==B)
- 然而，此时B线程将无法得到victim==A的结果，从而CS(B)无法进入，矛盾

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
- 假设CS(A)和CS(B)会同时发生
- write(A,flag[A]=true)->write(A,victim=A)->read(A,flag[B]==false)->read(A,victim==B)->CS(A)，A线程进入临界区必然会有前面的时序
- write(B,flag[B]=true)->write(B,victim=B)->read(B,flag[A]==false)->read(B,victim==A)->CS(B)，B线程进入临界区必然会有前面的时序
- 假设线程A先进入临界区，那么一定有write(B,victim=B)->write(A,victim=A),但是此时
- write(A,flag[A]=true)->write(B,victim=B)->write(A,victim=A)->read(B,flag[A]==false)
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
1. 门廊区，其执行区间D(A)由有限个操作组成。
2. 等待区，其执行区间W(A)可能包括无穷个操作步。
由此，引出**公平性**定义：
>满足下面条件的锁称之为先来先服务的：如果线程A门廊区的结束在线程B门廊区的开始之前完成，那么线程A必定不会被线程B赶超。也就是说，对于线程A、B及整数j、k：
若D(A,j)->D(B,k)，则CS(A,j)->CS(B,k)

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
- 正在等待的线程中，必定存在某个线程A具有唯一的最小(label[a],A)，那么这个线程决不会等待其他线程。

FIFO证明：
- 如果A的门廊先于B的门廊，D(A)->D(B)，那么A的label必然小于B的label，因为
- write(A,label[A])->read(B,label[A])->write(B,label[B])->read(B,flag[A])
- 所以，当flag[A]为true时，B被封锁在外面。

互斥证明：
- 假设CS(A)和CS(B)同时发生
- 不妨，假设A和B进入临界区的时，(label[A],A)<<(label[B],B)
- 则B进入临界区要么flag[A]==false,要么(label[B],B)<<(label[A],A)，则只能是flag[A]=false
- 从而，write(B,label[B])->read(B,flag[A])->write(A,flag[A])->write(A,label[A])
- 但此时，(label[B],B)<<(label[A],A)，矛盾

**有界时间戳**：

**存储单元数量的下届**：

### 并发对象

### 共享存储基础

### 同步原子操作的相对能力

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