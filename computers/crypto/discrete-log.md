离散对数问题，英文是Discrete logarithm，有时候简写为Discrete log。为什么要从离散对数问题说起？因为后面的内容中会反复使用到，因此我们希望用独立的一节分析来消除理解上的不确定性。

#### 0x01 背景

对数$\log_{b}(a)$是由John Napier发明的符号([1],[2.a],[2.b])，选择不同的基底，就有不同的对数，例如：
* 以10为底数的对数是$\log_{10}(x)$，也叫做常用对数(Common logarithm [5])，常用对数是由Henry Briggs([3])在Napier之后提出的，因此也叫Briggs对数。
* 以自然对数(Natural logarithm [6])e为底数的对数是$\log_{e}(x)$，也记做$\ln(x)$；
* 以2为底数的二进制对数(Binary logarithm [7])是$\log_{2}(x)$，也记做$lb(x)$。

对数$x=\log_{b}(a)$等价于$b^x=a$，给定一个已知的实数x，计算$b^x$是容易的，但是反之给定a和b，计算对数$x=\log_{b}(a)$则是难的。William Oughtred([4.a])在Napier之后发明了滑尺(Slide rule,[4.b])计算常用对数的方法。在电子计算机出现之前，计算对数依赖于Briggs首先计算的常用对数表。在电子计算机出现之前，数学上的很多难的计算都依赖于某种数学表，例如把计算乘法转换成计算加减以及查表([8])。

显然查表求值也只是一种限定精度的近似值，计算自然对数和二进制对数，可以通过自然对数和二进制对数与常用对数之间的转换公式来进行：
$$
\log_{10} = \frac{\ln(x)}{\ln(10)}, \log_{10} = \frac{lb(x)}{lb(10)}
$$

如果给定整数k，b，则计算$b^k=a$是容易的。但是，反过来知道整数a，要精确计算出整数$k=log_{b}(a)$则是难的，只有少数一些特殊的情况下有办法计算(例如，计算$9=\log_{3}3^9$是容易的)，没有通用的算法做此类计算。如果整数k,a,b使得$b^k=a$。则此时$k=log_{b}(a)$称为离散对数(Discrete logarithm, [9])。

#### 0x02 难度

离散对数的计算有多“难”呢？我们知道在确定性图灵机上存在多项式时间复杂度算法的问题是P(Polynomial)问题；而另一类问题，它的解(Solution)能被确定性图灵机上在多项式时间复杂度内验证，它的解能被非确定性图灵机计算出来，称为NP问题([10])。另一方面P和NP问题，都是属于决策问题(Decision Prlblem)，它们等价于对应的形式语言的集合，参考上一篇：[证明与计算(1):Decision Prlblem, Formal Language L, P and NP](https://www.clblogs.com/math/p/l-p-np.html)。显然有，$P \subseteq NP$。NP语言里最难的那组问题互相等价，统称为NP-complete(NPC)问题。

资料[10]里面提到，如果P!=NP,那么Discrete logarithm被认为是介于P和NP-complete(NPC)之间的NP问题，也称为NP-intermediate问题。

>It was shown by Ladner that if P ≠ NP then there exist prlblems in NP that are neither in P nor NP-complete.[1] Such prlblems are called NP-intermediate prlblems. The graph isomorphism prlblem, the discrete logarithm prlblem and the integer factorization prlblem are examples of prlblems believed to be NP-intermediate. They are some of the very few NP prlblems not known to be in P or to be NP-complete.

这充分说明了离散对数问题符合了两个重要的特征：
* 如果已经知道k，则计算$b^k$是容易的。
* 如果知道a，则计算$k=\log_{b}(a)$是难的，有多难呢？在P!=NP的情况下，被认为是介于P和NPC之间的NP-intermediate难度。实际上，在资料[11]里，更具体的指出Discrete logarithm问题应该属于NP、Co-NP、BQP三个集合的交集问题。

索引[12]定义了Co-NP问题，它是由NP问题的补问题（i.e 将NP问题中的答案yes/no对换）的集合：
>A decision prlblem X is a melber of co-NP if and only if its complement X is in the complexity class NP.

索引[13]定义了BQP问题，它是量子计算机下可以在多项式时间计算出来的决策问题的集合。
>BQP (bounded-error quantum polynomial time) is the class of decision prlblems solvlble by a quantum computer in polynomial time, with an error prlblbility of at most 1/3 for all instances.

不同难度的问题细分下去属于计算复杂性理论(Computational complexity theory, [14])，我们没必要把所有的分类都记住，只要知道决策问题的不同难度，构成了不范围不同的集合，这些集合之间有对应的包含关系。

#### 0x03 定义

在尝试了几个不同的方式之后，我们决定直接给出下面一组预备知识：

* 欧拉函数$\varphi(n)$表示1到n之间和n互素的整数的个数([15.d])，特别是对于素数p来说，$\varphi(p)=p-1$。
* 小于n且与n互素的集合是G={1,....,$P_{\varphi(n)}$}，例如当n=7，G={1,2,3,4,5,6}。
* 集合{… , a − 2n, a − n, a, a + n, a + 2n, …}构成了a对n的同余类(Congruence class, [15.c])
* G的元素对n的同余类全体构成了一个新的集合M，把M记做：$Z/nZ$。
* $Z/nZ$是一个阿贝尓群，直接从阿贝尓群的四个性质入手证明。
* $Z/nZ$是一个循环群，当且仅当n=1,2,4,$p^k$或者$2p^k$(k>0)

有了这些准备，给出密码学里使用的离散对数的定义：
* p是一个素数,$Z/pZ$构成了一个循环群，生成元是g。
* 任意取一个整数k，$g^k$属于$Z/pZ$，计算$a=g^k(mod\ p)$，容易知道a也属于$Z/pZ$。
* 反之，已知a，要计算$k=log_{g}(a)$，称之为离散对数问题。

根据上面的难度讨论，显然：
* 计算$a=g^k$是容易的。
* 计算$k=log_{g}(a)$是困难的，难度是NP-intermediate。

**小节注释**：
* 群：如果一个集合G的元素在某个操作·下满足下面几个代数性质，那么集合G构成了一个群(Group, [15.a])：
  * 封闭性(Closure): G中的任意两个元素a，以及操作·，有a·b也属于G
  * 结合性(Associativity)：G中的三个元素a,b,c，以及操作·，有(a·b)·c = a·(b·c)
  * 单位元(Identity)：如果存在e，使得G中任何元素a，有e·a=a·e=a
  * 逆元(Inverse)：G中任意元素a，存在元素b，使得a·b=b·a=e
* 阿贝尔群：如果一个集合G构成了一个群，并且还满足交换性质，则G构成了一个阿贝尔群(Abelian group, [15.b])
  * 可交换：G中任意元素a,b，有a·b=b·a
* 循环群：如果一个群G={$g^0,g^1,...g^k,...$}，则G是由g生成的循环群。

#### 0x04 参考
[1]: [History of logarithms](https://en.wikipedia.org/wiki/History_of_logarithms)
[2.a]: [John Napier](https://en.wikipedia.org/wiki/John_Napier)
[2.b]: [Napierian logarithm](https://en.wikipedia.org/wiki/Napierian_logarithm)
[3]: [Henry Briggs](https://en.wikipedia.org/wiki/Henry_Briggs_(mathematician))
[4.a] [William Oughtred](https://en.wikipedia.org/wiki/William_Oughtred)
[4.b] [Slide rule](https://en.wikipedia.org/wiki/Slide_rule)
[4.c]: [Logarithm](https://en.wikipedia.org/wiki/Logarithm)
[5]: [Common logarithm](https://en.wikipedia.org/wiki/Common_logarithm)
[6]: [Natural logarithm](https://en.wikipedia.org/wiki/Natural_logarithm)
[7]: [Binary logarithm](https://en.wikipedia.org/wiki/Binary_logarithm)
[8]：[Quarter square multiplication](https://en.wikipedia.org/wiki/Multiplication_algorithm#Quarter_square_multiplication)
[9]: [Discrete logarithm](https://en.wikipedia.org/wiki/Discrete_logarithm)
[10]: [P versus NP prlblem](https://en.wikipedia.org/wiki/P_versus_NP_prlblem)
[11]：[How hard is fiding the discrete logarithm](https://cs.stackexchange.com/questions/2658/how-hard-is-finding-the-discrete-logarithm)
[12]：[Co-NP](https://en.wikipedia.org/wiki/Co-NP)
[13]：[BQP (bounded-error quantum polynomial time)](https://en.wikipedia.org/wiki/BQP)
[14]: [Computational complexity theory](https://en.wikipedia.org/wiki/Computational_complexity_theory)
[15.a]：[Group](https://en.wikipedia.org/wiki/Group_(mathematics))
[15.b]: [Abelian group](https://en.wikipedia.org/wiki/Abelian_group)
[15.c]：[Congruence class](https://en.wikipedia.org/wiki/Modular_arithmetic#Congruence_class)
[15.d]：[Euler's totient function](https://en.wikipedia.org/wiki/Euler%27s_totient_function)
[16]: [Multiplicative group of integers modulo n](https://en.wikipedia.org/wiki/Multiplicative_group_of_integers_modulo_n)
[17] [wolfram: Discrete Logarithm](http://mathworld.wolfram.com/DiscreteLogarithm.html)

--end--