## Formal Lanaguage L（判定问题）

这篇讲知识证明的wiki([1]):
https://en.wikipedia.org/wiki/Proof_of_knowledge

里面有一句话:
>Let x be a language element of language  L in NP

这篇讲NPC的文章([2])
http://www.cs.princeton.edu/courses/archive/spr11/cos423/Lectures/NP-completeness.pdf

里面提到Decision Problem([3],[4)，Decision Problem就是判定yes/no的问题]，一个Decision Problem `Q`可以等价于一个Formal language `L`：
 L = {x ∈ {0,1}* : Q(x) = 1}

可以理解为Q则是判定算法，该判定算法能确定一个具体的Decision Problem，其结果是yes还是no。
那么，L是一个集合，其中每个x都是被Q判定为yes的Decison Problem的实例。

那么，P问题等价于如下的语言集合：
P = {L \subseteq {0, 1}* : exist an algorithm A that decides L in p-time}

我们可以理解为，P是由所有能在多项式时间内确定yes/no的Q所导出的语言L组成。是不是很绕？分解一下：
* Q是一个判定算法。
* Q是一个能在多项式时间内判定yes/no的判定算法。
* L是被Q判定结果为yes的所有输入x的集合。
* L是被Q判定结果为yes的所有输入x的集合所表示的形式语言。

## P（判定问题集合/语言集合）
在[3]里面描述：
>P is the set of languages whose memberships are decidable by a Turing Machine that makes a polynomial number of steps.

这里增加了Turing Machine的限定，也就是Q是在Turing Machine上执行的。参考链接[5],[6]里是对确定性图灵机和非确定性图灵机的描述。

**例如**，下面的k路径问题，属于P ([2])：
PATH = {< G, u, v, k > : G = (V, E) is an undirected
graph, u,v ∈ V, k ≥ 0 is an integer, and exist a path
from u to v in G with ≤ k edges}

## certificate/certifier
算法A验证了语言L，iff([2])：
L = {x ∈ {0, 1}* : exist y ∈ {0, 1}* s.t. A(x, y) = 1}

通过例子理解Certificate，例如下面的Independent Set属于NP([3])：
Given a graph G, is there set S of size ≥ k such that no two nodes in S are connected by an edge?
* Finding the set S is hard
* But if I give you a set S∗, checking whether S∗ is the answer is easy
* S∗ acts as a **certificate** that ⟨G,k⟩ is a yes instance of Independent Set

Certificate是否有效(efficient)([3])：
An algorithm B is an efficient certifier for problem X if:
* B is a polynomial time algorithm that takes two input strings I (instance of X) and C (a certificate)
* B outputs either yes or no.
* There is a polynomial p(n) such that for every string I: I ∈ X if and only if there exists string C of length ≤ p(|I|) such that B(I,C) = yes.

Certification的含义是“帮手”([3])：
>B is an algorithm that can decide whether an instance I is a yes instance if it is given some “help” in the form of a polynomially long certificate.

注意，C是Certificate，而算法B是certifier
>Let’s say you had an efficient certifier B for the Independent Set problem.

怎样找到Certificate呢？
>Try every string C of length ≤ p(|I|) and ask is B(I,C) = yes?

## NP（判定问题集合/语言集合）
那么，NP问题等价于如下的语言集合([2])：
NP = {L \subseteq {0, 1}* : exist a certificate y, |y| = O(|x|^k), and an algorithm A s.t. A(x, y) = 1}

在[3]里面描述NP：
>NP is the set of languages for which there exists an efficient certifier.

区别于P：
>P is the set of languages for which there exists an efficient certifier that ignores the certificate.

就是说P和NP问题都能在多项式时间内判定，但是NP问题的判定需要certificate的帮助:
>A problem is in P if we can decided them in polynomial time. It is in NP if we can decide them in polynomial time, if we are given the right certificate.

我们可以理解为，如果：
* 存在长度是字符串x的多项式倍的字符串y
* 存在验证算法A
* 使得A(x,y)=1

那么：
* 能通过A(x,y)=1的所有x构成的语言是L
* 所有L的集合是NP语言集合

**例如**，下面的子集求和问题，属于NP：
SUBSET-SUM: Given finite set S of integers, is there a subset whose sum is exactly t?

## P vs NP vs NPC
中间讨论了P、NP、NPC问题，简单说：
* P \subseteq NP \subseteq NP-hard
* Co-NP = {L': L ∈ NP}
* NPC=NP中最难的集合，并且他们等价

证明P \subseteq NP是很容易的：
>Proof. Suppose X ∈ P. Then there is a polynomial-time algorithm A for X.
To show that X ∈ NP, we need to design an efficient certifier B(I,C).
JusttakeB(I,C)=A(I).

## Reduction and NPC Reduction
下面是对L语言进行规约，两个步骤：

Reduce language L1 to L2 via function f:
1. Convert input x of L1 to instance f(x) of L2
2. Apply decision algorithm for L2 to f(x)

则L2的判定时间>=L1的判定时间，即：L1≤L2

引入`p≤`的概念：

L1is p-time reducible to L2, or L1 p≤ L2, if exist a ptime
computable function f : {0, 1}* -> {0, 1}* s.t. for all x ∈ {0, 1}*, x ∈ L1
iff f(x) ∈ L2

从而，要判定一个形式语言是否是P的，使用夹逼法：
If L1 p≤ L2 and L2 ∈ P, then L1 ∈ P

进一步，要判定一个形式语言是否是NPC的，使用夹逼法：

A language L ∈ {0, 1}* is NP-complete if:
1. L ∈ NP, and
2. L’ p≤ L for every L’ ∈ NP, i.e. L is NP-hard

利用其他已知的NPC问题，就可以夹逼：
1. If L is language s.t. L’ p≤L where L’ ∈ NPC, then L is NP-hard. 
2. If L ∈ NP, then L ∈ NPC.

这样就得到证明NPC的步骤：
This gives us a recipe for proving any L ∈ NPC:
1. Prove L ∈ NP
2. Select L’ ∈ NPC
3. Describe algorithm to compute f mapping every input x of L’ to input f(x) of L
4. Prove f satisfies x ∈ L’ iff f(x) ∈ L, for all x ∈ {0, 1}*
5. Prove computing f takes p-time

最后，如果P=NP，那么....:
>“If P = NP, then the world would be a profoundly different
place than we usually assume it to be. There would be no
special value in "creative leaps," no fundamental gap
between solving a problem and recognizing the solution once
it's found. Everyone who could appreciate a symphony would
be Mozart; everyone who could follow a step-by-step
argument would be Gauss...”
— Scott Aaronson, MIT

但是证明或者证伪P=NP是很难的。

## wait to be continue...

我们最开始是为了理解在Proof of knowledge ([1]) 里面这句话的作用：
>Let x be a language element of language  L in NP

下次再分解。

## references
[1] https://en.wikipedia.org/wiki/Proof_of_knowledge
[2] http://www.cs.princeton.edu/courses/archive/spr11/cos423/Lectures/NP-completeness.pdf
[3] https://www.cs.cmu.edu/~ckingsf/bioinfo-lectures/np.pdf
[4] http://www.ccs.neu.edu/home/rjw/csu390-sp06/LectureMaterials/Decision-Problems.pdf
[5] https://en.wikipedia.org/wiki/Turing_machine
[6] https://en.wikipedia.org/wiki/Non-deterministic_Turing_machine
