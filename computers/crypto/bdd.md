上一篇：[证明与计算(2):Discrete logarithm](https://www.cnblogs.com/math/p/discrete-log.html)

## 0x01 布尔代数(Boolean algebra)

大名鼎鼎鼎的stephen wolfram在2015年的时候写了一篇介绍George Boole的文章：[George Boole: A 200-Year View](http://blog.stephenwolfram.com/2015/11/george-boole-a-200-year-view/)。

怎样用数学公理重新表达经典逻辑？George Boole在19世纪的时候开始思考这件事，在他的书《The Mathematical Analysis of Logic》里面George Boole首次展示了使用符号加运算符的方式表示逻辑，例如“And”是“xy”，“NOT”是“1-x”，“OR”是“x+y-2xy”。但是x^2是什么呢？命题x加上命题x还是命题x，George Boole又规定了x^2=x。在George Boole的另一本书《An Investigation of the Laws of Thought》里面，Gerge Boole认为自己在发明一个“science of intellectual powers”，区别于牛顿发明微积分是“physical science”。

从George Boole开始，在经过Frege, Peano, Hilbert, Whitehead, Russell, Gödel 和 Turing等一系列数学家的工作后，直到克劳德·香农( Claude Shannon )才开始让布尔代数实用。

“这个过程是必然会发生的么？”stephen wolfram思考到。布尔代数的历史显示了一个从复杂问题中慢慢产生的简单的形式化思想，偶然间就被大规模使用了，流行了起来。
>Most often what happens is that at some moment the ideas become relevant to technology, and quickly then go from curiosities to mainstream.

stephen wolfram的书《[A New Kind of Science](http://www.wolframscience.com/nks/)》收集了很多这种“simple formal idea”，其中有些已经流行，有很多还没有，其中会有下一个技术奇点么？

## 0x01 布尔函数(Boolean function)

如果一个函数F(x1,x2,...,xn)的输入参数xi取值于{0,1}集合，并且函数的输出也是布尔值{0,1}，那么这个函数是一个布尔函数(Boolean function, [5])。布尔函数在密码学的对称加密算法中有重要的作用，特别是在对称加密算法的置换-代换算法中，设计置换盒子的过程中。

**定义3.1**:布尔函数的形式化定义
<hr/>
* F:${0,1}^k->{0,1}$

如果一个布尔函数有k个参数个数，称F(x1,x2,...,xk)为k元布尔函数。英语专有名词是[k-ary](https://en.wikipedia.org/wiki/Arity),例如常见的有：
* Nulary：F()=0，也就是常量布尔函数
* Unary: F(x)=x
* Binary: F(x,y)=x&&y
* Ternary: F(x,y,z)= x?y:z
* Quaternary
* ...
* k-ary: F(x1,x2,...,xn) = 2*$\prod_{i=1}^{k}x_i$
* Variable arity: 可变(数量)参数

这虽然不是一个很复杂的事情，但是知道这些专有名词有助于在代码中的命名。如果不仅仅关心名字，那么考虑一个问题：
>k-ary Boolan function有多少个具体的不同函数？

那么，首先需要理解什么是“不同”的布尔函数。这涉及到数理逻辑的一些基本概念，我们采用步步陈述的方式展现数理逻辑中下面的一组关系。
* 基本定义：
	* 个体词：就是表示对象
	* 命题(Propositional)：是指判断一件事的陈述句，返回true/false。命题包含了“原命题”，“逆命题”，“否命题”，“逆否命题”等等。
	* 谓词(Predicat)：表示个体词之间的关系
* 关键操作：
	* 断言(Logical assertion)：简单说就是把命题函数化。
	* 变量(Variable)：简单说就是变量名
	* 量化(Quantifiers)：使用全称量词$\forall$和存在量词$\exists$来量化。
* 从而分类：
	* 谓词逻辑(Predicate logic)：谓词逻辑中原子逻辑被切分成个体词和谓词。
	* 命题逻辑(Propositional logic)：只有陈述性命题，不能使用断言、量化，也叫零阶逻辑([Zeroth order logic, ZOL](https://en.wikipedia.org/wiki/Zeroth-order_logic))
	* 一阶逻辑(First Order Logic, FOL)：可以使用断言、可以对个体词量化。
	* 高阶逻辑(High Order Logic, HOL)：可以对命题和谓词也量化。

我们关心的是命题逻辑，可见它是处于零阶逻辑这个位置。命题逻辑使用命题公式(Propositional formula, [5.a])表达，命题公式由原子命题以及命题连接词组合而成。我们看下有哪些连接词：
* 一元否定连接词(unary negation connective)
* 经典二元连接词：与(And)、或(Or)、蕴含(->)，等价(<->)
* 其他二元连接词：NAND, NOR, XOR
* 三元连接词：IF ... THEN...ELSE..., C?A:B
* 常量{T,F}, {1,0}

考虑布尔公式的完备性(Completeness)，下面的连接词都可以完备的表示命题公式：
* {AND, NOT} (see, [Stephen Wolfram's A New Kind of Science:p773](http://www.wolframscience.com/nks/p773--implications-for-mathematics-and-its-foundations/))
* {NAND} (see, [Stephen Wolfram's A New Kind of Science:p808](http://www.wolframscience.com/nks/p808--implications-for-mathematics-and-its-foundations/))
* {NOR}
* IF … THEN … ELSE 

现在我们可以定义“相同”的布尔函数：k-ary布尔函数F(x1,x2,...,xk)实际上可以表示为关于变量(x1,x2,...,xk)的命题公式。两个命题公式等价当前近当这两个命题公式表示的是同一个布尔函数。

那么，k-ary的布尔函数，可能有多少个不同的命题公式呢？一个k-ary的布尔函数F(x1,x2,...,xk)的所有可能参数输入有$2^k$个，因此只要这$2^k$的值不同，就定义了一个不同的布尔函数。由于其中每一个具体的输入有0和1两种可能的输出，则$2^k$个输出一共有$2^{2^k}$种可能的不同值，也就是k-ary的布尔函数有$2^{2^k}$个！

一阶逻辑(First Order Logic)、高阶逻辑(High Order Logic)等概念的细节可以详细阅读数里逻辑的书籍，此处不再展开。

## 0x01 布尔表达式(Boolean expression)

在编程语言里，对应命题逻辑的是布尔表达式(Boolean expression)，常用的连接词是$&&,||,!$，分别是"And","Or","Not"。

Boolean expression的产生式，也就是所谓的重写规则(rewrite rule)如下：
* t::=x|0|1|$\neg t$| t $\wedge$ t|t $\vee$ t|t $\rightarrow$ t|t $\Rightarrow$ t|t $\Leftrightarrow$ t

这些产生式使得布尔表达式可以对基本的布尔变量做组合，从而表达命题逻辑。我们在编程中早已经对布尔表达式不再陌生。但是布尔表达式的求值和四则运算系统又不是一类的，四则表达式的运算是加减乘除，布尔表达式的基本运算则是“AND”，“NOT”，“OR”,“$\Rightarrow$”, “\Leftrightarrow”，它们的计算依赖于真值表（True Table)，例如“AND”的真值表如下：

|p|q|p ∧ q|
|--|--|--|
|1|1|1|
|1|0|0|
|0|1|0|
|1|1|1|

布尔表达式的写法各种各样，不同的形式形成了不同的范式(Normal Form)，下面3种范式是我们考虑的：
* 析取范式(Disjunctive Normal Form, DNF)，由一个或多个合取公式的析取构成
	* 例如这些是DNF：$A$, $(A \wedge B) \vee C$, $(A \wedge B) \vee (C \wedge D)$
	* 但是这些不是：$\neg (A \vee B)$, $(A \wedge B) \vee (C \wedge (D \vee E))$
* 合取范式(Conjunctive Normal Form, CNF), 由一个或多个析取公式的合取构成
	* 例如这些是CNF：$A$, $(A \vee B) \wedge C$, $(A \vee B) \wedge (C \vee D)$
	* 但是这些不是：$\neg (A \wedge B)$, $(A \vee B) \wedge (C \vee (D \wedge E))$
* 条件范式(IF-ELSE-THEN Normal Form, INF)，由布尔变量和条件公式构成
	* 例如：`t->y0,y1`表示如果t是1则结果为y0否则结果为y1。
	* 一个布尔表达式t，假设x是它的变量，那么可以表示成INF：`x->t[1/x], t[0/x]`
	* 如果从编程里的if控制结构来看会更易于理解，写成公式就需要拐个弯

上面3种范式，都是完备的，简单说：
* 任何布尔表达式可以写成DNF
* 任何布尔表达式可以写成CNF
* 任何布尔表达式可以写成INF

如果给一个布尔表达式的变量(x1,x2,...,xk)指定具体的值，例如(0,0,...,0)，再根据真值表的规则，就可以算出该布尔表达式在指定输入下的结果。这个过程叫做真值指派(Truth assignment)。真值指派实际上就是对布尔表达式进行求值。根据布尔表达式的求值结果是否为真，有下面两个重要的分类。
* 如果一个布尔表达式在任意的真值指派下，结果都是1，那么该布尔表达式被称为重言式(Tautology)，或者叫恒真命题。
* 如果一个布尔表达式至少存在一个真值指派，其结果为1，那么该布尔表达式被称为可满足的(Satisfiable)。

很不幸的，判定一个任意给定的布尔表达式是否是可满足的(Satisfiable)是NP-Complete问题。事实上，如果限定到DNF和CNF，有下面的结果：
* 判定一个CNF是否是可满足的，是NP-Complete问题。
* 判定一个CNF是否是恒真命题，是一个P问题，可以在多项式复杂度内判定。
* 判定一个DNF是否是可满足的，是一个P问题，可以在多项式复杂度内判定。
* 判定一个DNF是否是恒真命题，是一个Co-NP-Complete问题。
* 转换任意CNF到DNF是指数集复杂度的，这是致命的。

连最简单的布尔函数中都存在NP问题，可见计算复杂性无处不在。

**小节思考题**：
* 根据定义，使用数学归纳法证明任意布尔表达式可以表达成INF。
* 查阅资料，说明Tautology和Satisfiable的判定分别有什么用呢？

## 0x02 二分决策图(Binary desision diagram)

布尔函数可以用INF来表示，我们需要一个具体的例子来说明，考察布尔表达式: t=(x1&&x2)||x3，我们从x1开始反复使用x->t[0/x],t[1/x]这个规则来替换。那么t步步等价于下面的INF：
* t: x1->t[1/x1], t[0/x1]
	* t[1/x1]: x2->t[1/x1,1/x2], t[1/x1,0/x2]，也就是x2->1, t[1/x1,0/x2]
		* t[1/x1,0/x2]: x3->t[1/x1,0/x2, 1/x3], t[1/x1,0/x2,0/x3]，也就是x3->1,0
	* t[0/x1]: x2->t[0/x1,1/x2], t[0/x1, 0/x2]
		* t[0/x1,1/x2]: x3->t[0/x1,1/x2,1/x3],t[0/x1,1/x2,0/x3]，也就是x3->1,0
		* t[0/x1, 0/x2]: x3->t[0/x1, 0/x2, 1/x3], t[0/x1, 0/x2, 0/x3]，也就是x3->1,0

上述过程是递归的，把上述替换xi为0，1的过程用二分决策树(Binary Decision Tree)的方式表示出来，如图所示
![](https://images2018.cnblogs.com/blog/121186/201808/121186-20180807171805826-1825144986.png)

实际上，上图不只是一个二分决策树，而是一个有向无环图(Directed Acyclic Graph, DAG)，我们称为Binary Decision Diagram(BDD)。BDD在1978年由S. B. Akers([1])提出，BDD具有典范性质(Cannonical)，就是说可以让布尔函数和对应的BDD之间有唯一映射，立刻得到的好处是要判定两个布尔函数是否等价只要判断它们对应的BDD是否是同一个即可。

1986年R.E. Bryant([2])给出了BDD的一个有效的图表示及其相关的算法，根据维基百科的介绍，Bryant的这篇论文在科学文献索引[CiteSeerX](https://en.wikipedia.org/wiki/CiteSeerX)里计算机分类里的被引用率第一的文章。Bryant在2009年获得了被认为是电子设计自动化界的诺贝尔奖的菲尔·卡夫曼奖。

Bryant认为BDD的构造过程中，变量的顺序是关键的，例如上面的例子，我们使用x3->x2->x1的顺序，得到的BDD如下：
![](https://images2018.cnblogs.com/blog/121186/201808/121186-20180807171818053-952209766.png)

事实上，k-ary布尔表达式，其变量的顺序会导致BDD的节点个数上的巨大差异，有的排序导出k的多项式个数的BDD，有的顺序会导出k的指数级别个数的BDD。例如布尔表达式$x_1x_2+...+x_{2n-1}x_{2n}$需要2n+2个顶点，而布尔表达式$x_1x_{n+1}+...+x_{n}x_{2n}$需要$2^{n+1}$个顶点。示例如下：
![](https://images2018.cnblogs.com/blog/121186/201808/121186-20180807171829308-1674533165.png)

进一步，存在布尔表达式，对任意的变量排序，BDD节点的个数都是k的指数级别。再次，不幸的是找到一个布尔表达式的最优BDD(节点个数最少)是一个NP问题。

**定义3.2**: Orderd Binary Decision Diagram
<hr/>
一个BDD是关于x1<x2<...<xn有序的，当且仅当任意从根节点到终端(terminal)节点路径上的变量都保持x1<x2<...<xn的顺序。

观察上述图形，可以看到：
* 存在冗余的节点，例如终端节点(terminal)只有两个值0和1，但是重复了多次。
* 存在冗余子图，例如两个非叶子结点(non-terminal)后面的子图是一样的结构。

为了精确定义BDD里的冗余结构，先对没有冗余结构的BDD节点编号：
* k-ary布尔函数的输入变量是x1,x2,...,xk;
* 终端节点0编号为0，终端节点1编号为1；
* 一个非终端节点u，定义low(u)为u节点取0时的输出节点；
* 一个非终端节点u，定义high(u)为u节点取1时的输出节点；

如下图([18])所示从终端节点开始对BDD编号：
![](https://images2018.cnblogs.com/blog/121186/201808/121186-20180807171911109-672445663.png)

定义了节点的编号u之后，对u节点定义变量函数var(u):
* 一个终端节点u，如果是0节点，则定义var(u)=0；如果是1节点，则定义var(u)=1;
* 一个非终端节点u，定义var(u)=i，如果u上面的判定变量是xi;

把节点编号u和变量下标函数var放在一起观察，可以避免混淆它们：
![](https://images2018.cnblogs.com/blog/121186/201808/121186-20180807171842677-60501820.png)

从而，通过上述编号可以描述消除BDD冗余的算法。

**算法3.1**: Reduced BDD
<hr/>
* 消除冗余的终端节点(Remove duplicate terminals)
	* 在BDD里面，终端节点就最多只能有两个，0和1.
* 消除冗余的非终端节点(Remove duplicate nonterminals)
	* 如果var(u)=var(v)，并且low(u)=low(v),high(u)=high(v)，就消除v，v的输入都定向到u
* 消除冗余的测试(Remove redundant tests)
	* 如果low(u)=high(u)，那么消除u和high(u)，输入都定向到low(u)

一个BDD，是有序的(Orderd)，以及精简的(Reduced)，就称为Reduced Orderd Binay Decision Diagram，简写为ROBDD。事实上现在说BDD的时候，就默认指ROBDD。

**定义3.3**: Reduced Orderd Binay Decision Diagram, ROBDD([18])
<hr/>
* 只有1个或者2个终端节点，0和1
* 非终端节点只有2个子节点low(u)和high(u)
* 每个节点上可以定义一个变量函数var(u),var(u)等于u上判定变量xi的下标i
* 从根节点到终端节点的路径上的变量顺序都保持x1<x2<...<xk的顺序
* **唯一性(uniqueness)**。不存在两个节点u和v使得var(u)=var(v), low(u)=low(v), high(u)=high(v)
* **非冗余测试性(non-redenced-test)**。不存在节点u使得low(u)=low(v)。

有了RODD之后，我们可以在RODD的节点编号u上面递归定义如下函数$t^u$：
* $t^0$=0
* $t^1$=1
* $t^u=var(u)->t^{high(u)},t^{low(u)}$，如果var(u)对应的变量取值1，则$t^u=t^{high(u)}$，否则$t^u=t^{low(u)}$。

从而，我们可以最终描述RODD的重要性质，典范(Cannonical)性质：对于一个布尔函数F:${0,1}^k->{0,1}$，只存在唯一的一个关于x1<x2<...<xk的RODD，使得$f^{u}$=f(x1,x2,...,xk)。

**练习题**：
* 使用数学归纳法，证明递归函数$f^u$的上述典范性质。

## 0x03 BDD算法和工具包

斐波那契数列是递归结构的典型，在斐波那契数列的递归计算里面，存在经典的冗余计算问题，如下图：
![](https://images2018.cnblogs.com/blog/121186/201808/121186-20180807171926218-736041795.png)

解决斐波那契数列的计算，一个方式就是使用动态编程(Dymanic Programming)，也就是缓存已经计算的结果，遇到求同样节点的时候直接返回该节点的值。

动态编程的核心思想，被用来构造一个ROBDD，引入两个关键的表结构T和H，分别直接用它们的ADT来描述这两个表的作用。

表格T定义了节点u和三元组(var(u)=i,low(u)=l,high(u)=h)之间的映射，提供如下的接口：
* init(T),初始化的时候只有节点0和1及其对应的三元组
* u<-add(T,i,l,h)，添加节点
* var(u),low(u),high(u)，计算u的三元组

表格H是表格T的逆向索引：
* init(H)，空表
* member(H,i,l,h)，检查(i,l,h)是不是已经在表里面
* lookup(H,i,l,h), 找到(i,l,h)对应的u
* insert(H,i,l,h,u)，插入(i,l,h)对u的映射

有了T表格和H表格，首先给出动态给BDD节点编号的算法，可以看到该算法弥补了上一节没有解释的节点编号的规则应该是怎样的：
**算法3.2**：MK_H_T(i,l,h)，输入i,j,k，返回对应的节点编号u
<hr/>
```
function MK_H_T(i,l,h){
	if(l==h){
		return l;
	}else if(member(H,i,l,h)){
		return lookup(H,i,l,h)
	}else{
		u = add(T,i,l,h);
		insert(H,i,l,h,u);
		return u;
	}
}
```

有了编号算法，就可以动态的构造ROBDD:
**算法3.3**: BUILD_T_H(t)
<hr/>
```
function BUILD_T_H(t,n){
	function build(t,i,n){
		if(i>n){
			return t?1:0;
		}else{
			l = build(t[1/x1],i+1,n);
			h = build(t[0/x1],i+1,n);

			return MK_H_T(i,l,h);
		}
	}	
	return build(t,1,n);
}
```

如果彻底理解了u,var(u),low(u),high(u),T,H,MK_T_H,BUILD_T_H这组符号和算法之后，再去理解ROBDD的其他所有算法([16])都没有本质上的困难，都是递归结构上的编程。

核心的算法有：
* Apply,在节点u上应用某个操作。
* Restrict，限定某些变量为常数的情况下的BDD。
* SatCount，计算所有从根节点到终端节点1的不同路径数。给定ROBDD的情况下，计算可满足性是容易的。
* AnyStat，找出一条从根节点到终端节点1的路径。
* AllStat，找出所有的从根节点到终端节点1的路径。
* Simplify，给定节点d和u，如果存在u'使得$t^d&&t^u=t^d&&t^{u'}$，那么消除u'.

ROBDD有各种语言实现的工具包，这个github仓库上收集了所有语言的BDD工具库：
https://github.com/johnyf/tool_lists/blob/master/bdd.md

## 0x04 BDD的应用场景

ROBDD在下面的很多场合中应用：
* 8皇后问题，由于8皇后问题可以表示为析取式(DNF)，因此可以用ROBDD计算。
* 组合电路(Combinational Circunit)的正确性验证.
* 组合电路的等价性，ROBDD唯一表示布尔函数为此提供了方案。
* 形式验证(formal verification)中判定一个系统是否拥有性质P，实际上是解决可满足性问题。
* 最优化问题的解决
* 编程语言上的应用([22])

## 0x06 参考
[1] [Aker78] S. B. Akers, "Binary Decision Diagrams", IEEE Transactions on Computers, vol. c-27, no. 6, June 1978. https://www.computer.org/csdl/trans/tc/1978/06/01675141.pdf
[2] [Brya86] R.E. Bryant, "Graph-Based Algorithms for Boolean Function Manipulation", IEEE Transactions on Computers, vol. c-35, no.8, Aug. 1986. https://www.cs.cmu.edu/~bryant/pubdir/ieeetc86.pdf
[5] [wiki:Boolen function](https://en.wikipedia.org/wiki/Boolean_function)
[5.a] [wiki:Propositional formula](https://en.wikipedia.org/wiki/Propositional_formula)
[5.c] [wiki:Boolan expression](https://en.wikipedia.org/wiki/Boolean_expression)
[6] [wiki:Binary decision diagram](https://en.wikipedia.org/wiki/Binary_decision_diagram)
[7] [wiki:Boolean satisfiability problem](https://en.wikipedia.org/wiki/Boolean_satisfiability_problem)
[8] [wiki:Propositional directed acyclic graph](https://en.wikipedia.org/wiki/Propositional_directed_acyclic_graph)
[9] [wiki:Negation normal form](https://en.wikipedia.org/wiki/Negation_normal_form)
[10] [wiki:Circuit complexity](https://en.wikipedia.org/wiki/Circuit_complexity#Complexity_classes)
[11.a] [wiki:Boolean circuit](https://en.wikipedia.org/wiki/Boolean_circuit)
[11.b] [wiki:Functional completeness](https://en.wikipedia.org/wiki/Functional_completeness)
[12] [cs.cmu.edu(15122):Binary Decision Diagrams](https://www.cs.cmu.edu/~fp/courses/15122-f10/lectures/19-bdds.pdf)
[13] [cmi.ac.in:An Introduction to Binary Decision Diagrams](http://www.cmi.ac.in/~madhavan/courses/verification-2011/andersen-bdd.pdf)
[14] [BuDDy - A Binary Decision Diagram Package](http://vlsicad.eecs.umich.edu/BK/Slots/cache/www.itu.dk/research/buddy/)
[15] [cs.cmu.edu(15817):Model Checking I:Binary Decision Diagrams](http://www.cs.cmu.edu/~emc/15817-f09/lecture1.pdf)
[16] [algorithm on Binary Decision Diagrams](http://www.di.univr.it/documenti/OccorrenzaIns/matdid/matdid813066.pdf)
[18] [cs.ox.ac.uk: An Introduction to Binary Decision Diagrams](https://www.cs.ox.ac.uk/files/4298/bdd98.pdf)
[21] [eecs.berkeley.edu:Binary Decision Diagrams](https://people.eecs.berkeley.edu/~sseshia/219c/lectures/BinaryDecisionDiagrams.pdf)
[22] [csail.mit.edu:Using Datalog with Binary Decision Diagrams for Program Analysis(bddbddb)](https://people.csail.mit.edu/mcarbin/papers/aplas05.pdf)