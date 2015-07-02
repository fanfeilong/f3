####<<编译原理>>笔记
--------------------

#####第一章 编译原理简介
-----------------------
* 阶段
	* 前端：词法分析、语法分析、中间代码生成
	* 后端：代码优化、目标机器代码生成
* 使用
	* 编译器
	* 文本转换
	* 智能提示
	* 重构
	* 类型推导

#####第二章 简单的一遍翻译
--------------------------
* 上下文无关文法
	* 符号集、终结点符号、非终结点符号、初始符号、产生式
	* 文法的歧义(有多于一种语法分析树）
	* 操作符、操作数；前缀、中缀、后缀；优先级、左结合、右结合；
	* 左递归、右递归、左递归树、右递归数
	* 左递归消除技术
* 抽象语法树
* 具体语法树（分析树）
* 递归下降算法Lex
	* 双缓存LookUp

递归可枚举语言，上下文相关语言，上下文无关语言，正规语言

#####第三章 词法分析
* lex and parser
	* token and identity
	* linearly sequence `->` tree
* 正规表达式
	* 并（concatenation）
	* 或（alternation）
	* 克林闭包（Kleene star）
	* 正规表达式转NFA
		* `Thompson`构造法
		* 将正规表达式分解为最基本的子表达式，分别构造其NFA，然后组合
			* e: `-[s]->(i)-[e]->(f)`
			* a: `-[s]->(i)-[a]->(f)`
			* s|t: `-[s]->(i)` (`-[e]->(()N(s)())-[e]`|`-[e]->(()N(t)())-[e]`) `->(f)`
			* st: `-[s]->(i)N(s)()N(t)(f)`
			* s*: `-[s]->(i)-[e]->(()N(s)())-[e]->(f)`&`(i)-[e]->(f)`&`()-[e]->()`
* 最小化（压缩）状态转表
* NFA
	* e-clousure(s)、e-clousure(T)、move(T,a) 
	* 子集构造法->DFA
	```
	function NFA2DFA(inputs,s0,Dstates,Dtrans)
		A=e-closure({s0})
		Dstates.Add(A,false)
		while T in Dstates.PopUnlableSet() do
			for c in inputs do
				U = e-closure(move(T,c))
				if not Dstates.Contain(U) then
					Dstates.Add(U,false)
				end
				Dtrans[T,c]=U
			end
		end
	end
	function e-closure(states,T)
		closure=T
		stack=T
		while not stack.IsEmpty() do
			t = stack.top();
			stack.pop();
			for u in states do
				if edege(t,u)==e then
					if not closure.Contain(u) then
						closure.add(u)
						stack.push(u)
					end
				end
			end
		end
	end
	```
	* 双堆栈模型
	```
	function (S,F,a,s0,nextchar,e-closure,move)
		S=e-closure({s0})
		a=nextchar()
		while not a=eof do
			S=e-closure(move(S,a)
			a=nextchar()
		end
		if not S.Intersect(F).IsEmpty() then return "yes" end
		return "no"
	end
	```
	* 基于NFA 模式匹配
* DFA
	* 状态集
	* 初始状态
	* 接受状态
	* 基于DFA 模式匹配
		* NFA的重要状态：有标记为非e的出边的状态
		* 两个子集是等同的iff它们的重要状态相同并且同时包含或者不包含NFA的接受状态
		* 将NFA的重要状态与正规表达式的符号相关联，只有字母表上一个符号出现在正规表达式中，Thompson构造法才创建一个重要状态。
		* NFA有且只有一个接受状态，但该状态不是重要的，因为没有出边，通过在正规表达式`r`右边链接一个结束符`#`，
		  使得接受状态成为重要状态。非重要的NFA状态用大写字母表示。
		* 用语法树扩展正规表达式，叶子节点是符号表中的符号或者e标记，内节点表示操作符，分别有`cat-`节点，`or-`或者`star-`节点。
		  对每个非e的叶子节点分配唯一的整数位置。
		* 正规表达式节点位置函数：nullable(n),firstpos(n),lastpos(n),followpos(n)
		```
		function nullable(node)
			if node.IsLeaf() and node.IsE() then 
				return true 
			end
			if node.IsLeaf() and (not node.IsE()) then
				return false
			end
			if node.IsOrNode() then
				c1=node.Left()
				c2=node.Right()
				return nullable(c1) or nullable(c2)
			end
			if node.IsCatNode() then
				c1=node.Left()
				c2=node.Right()
				return nullable(c1) and nullable(c2)
			end
			if node.IsStarNode() then
				return true
			end
		end
		```
		```
		function firstpos(node)
			if node.IsLeaf() and node.IsE() then 
				return {}
			end
			if node.IsLeaf() and (not node.IsE()) then
				i=node.pos()
				return {i}
			end
			if node.IsOrNode() then
				c1=node.Left()
				c2=node.Right()
				return firstpos(c1).Union(firstpos(c2))
			end
			if node.IsCatNode() then
				c1=node.Left()
				c2=node.Right()
				if nullable(c1) then
					return firstpos(c1).Union(firstpos(c2))
				else
					return firstpos(c1)
				end
			end
			if node.IsStarNode() then
				return true
			end
		end
		```
		lastpos的计算和firstpos类似，只需调换c1和c2，则计算followpos的股则如下：
			1. 如果n是cat节点，具有左子节点c1和右子节点c2，并且i是lastpos(c1)中的一个位置，
			   则firstpos(c2)的所有位置在follow(i)中。
			2. 如果n是star节点，并且i是lastpos中的一个位置，则所有firstpost(n)中的位置都在followpos(i)中。
		* 从正规表达式构造DFA
		```
		function (r,inputs)
			r=root(concat('(',r,')#'))
			Dstates={firstpos(r))
			while Dstates.HasUnLabel() do
				T=Dstates.GetNextUnLable()
				for a in inputs do
					for p in T do
						U=followpos(p)
						a=getnode(p).Symbol()
						if not U.IsEmpty() and (not Dstates.Contain(U)) then
							Dstates.Add(U,unlable=true)
						end
						Dtran[T,a]=U
					end
				end
			end
		end
		```
		* 最小化DFA的状态数

#####第四章 语法分析
--------------------
* 典型语法分析算法
	* Cocke-Younger-Kasami算法和Earley算法，能分析任何文法，但生成编译器效率低
	* 自顶向下法
	* 自底向上法
* 语法错误处理
	* 错误类型
		* 词法错误：标识符、关键字、操作符拼写错误
		* 语法错误：括号不匹配、少了分号
		* 语义错误：操作符作用于不相容的操作数
		* 逻辑错误：死循环、无限递归
	* 错误恢复
		* 紧急错误恢复
		* 短语级错误恢复
		* 出错产生式
		* 全局纠正
* 上下文无关文法
	* 分析树和推导
	* 二义性
	* 正规表达式和上下文无关文法比较
		* 使用正规表达式的场景
			* 语言的词法规则通常比较简单，不必动用文法
			* 对于记号，正规表达式更简洁易于理解
			* 正规表达式易于自动构造词法分析器，文法构造词法分析器难
			* 把语法分为词法和非词法，利于编译器前端的模块化
		* 功能差别
			* 正规表达式通常描述标识符、常数、关键字
			* 文法在描述括号配对、begin-end配对、if-then-else等`嵌套结构`，正规表达式此时无力
	* 验证文法所产生的语言：归纳法
	* 消除二义性：改写
	* 消除左递归
		* 消除直接左递归：`A->Aα1|...|Aαm|β1|...|βn => A->β1A'|...|βnA' + A'->α1A'|...αmA'|e`
		* 消除隐式左递归  
	```
	function(A1,A2,...,An)
		for i=1:n do
			for j=1:i-1 do
				Ai->Ajγ+Aj->δ1|δ2|...|δk => Ai->δ1γ1|δ2γ2|...|δkγk
			end
			Ai->Aiα1|...|Aiαm|β1|...|βn => Ai->β1Ai'|...|βnAi' + Ai'->α1Ai'|...αmAi'|e
		end
	end
	```
	* 提取左因子  
	```
	function(A1,A2,...,An)
		for i=1:n do
			Ai->αiβi1|...|αiβin|γ => Ai->αiAi'|γ+Ai'->βi1|...|βin
		end
	end
	```
	* 非上下文无关语言结构
		* 不可用上下文无关文法表示的例子：
			* `L1={wcw|w \in (a|b)*}` 标识符的声明先于引用
			* `L2={a^nb^mc^nd^m|m>=1,n>=1}` 过程声明的形参个数和过程引用的实参个数应该一致
			* `L3={a^nb^nc^n|n>=0}` 在键盘上敲打字母键，退格同样次数，敲打同样次数下划线：下划线标记的单词集合，正规表达式(abc)*则可以描述之
		* 可用上下文无关文法表示的例子：
			* `L1'={wcw^R|w \in (a|b)*,w^R表示w的逆序}`
			* `L2'={a^nb^nc^md^m|m>=1,n>=1}`
			* `L3'={a^nb^n|n>=1}` 这个例子不能用正规表达式表示
		* 两个性质
			* 有穷自动机不能计数，如上L3'不能用正规表达式
			* 上下文无关文法能计2项的数，不能计3项的数，如上L3和L3'的对比
* 预测语法分析
```
functions look(products,inputs)
	a=inputs.begin();
	<s,t>=products.states.begin()
	while a and <s,t> do
		if a is terminal then
			if match(s,t,a) then 
				a=inputs.next()
				<s,t>=products.states.next() 
			else
				return false
			end
		else
			store.a=a
			store.inputs=inputs
			subproducts = a.getproducts()
			for subproduct in subproducts do//递归下降
				if look(a.getproducts(),inputs.rest()) then
					<s,t>=products.states.next()
					a=inputs.next()
					break
				else//回朔
					a=store.a
					inputs=store.inputs
				end	
			end
		end
	end
	return products.states.finish()
end
```
* 非递归的预测语法分析（表格驱动），对比NFA双堆栈模型
```
function look(stack,table,inputs,outputs)
	--stack是文法符号序列，栈底是$,初始时stack含有文法的开始符号和下边的$
	--table是分析表M,value=M[A,a],A是非终结符，a是终结符或$
	--inputs是输入符号序列,末尾是$
	--outputs是输出的语法树
	ip=inputs.First()
	while true do
		X,a=stack.Top(),ip.Symbol()
		if X.IsTerminalOr$() then
			if X==a then
				stack.Pop(),ip=inputs.Next()
			else
				error()
			end
		else
			if not M[X,a]==error then --M[X,a]={X={Y1,Y2,...,Yk}}，表示X->Y1Y2...Yk
				stack.Pop()
				Y=M[x,a][X]
				outpus.Dump(X).Dump('->')
				for i=1,k do 
					stack.Push(Y[k-i])
					outpus.Dump(Y[i])
				end
			else
				error()
			end
		end
		if X==$ then break end
	end
end
```


#### <<程序设计语言基础>>笔记
-----------------------------

#####第一章 引言
----------------
- λ记法和λ演算系统
	* `λx:σ.M`
		* α等价:`λx:σ.M->λy:σ[y/x]M`
		* β等价:`(λx:σ.M)N->[N/x]M`
		* 同余规则:`(M1=M2,N1=N2)/M1N1=M2N2`
		* Church-Roosser汇聚性、强正则化性质
- 类型与类型系统简述
	* 类型：一个类型是共享某种性质的值的集合，是一个Collection（集聚）
	* 值：类型的成员
	* 论域、阶（Order）、种类（Kind）：以类型为成员
	* 避免：所有类型的类型，以免`罗素悖论`
	* 编译器类型检查
		* 较早发现错误
		* 编译文档
		* 保证优化得以实施
		* 绝大多数编译时类型检查基于简单的一些算法，它们拒绝接受带有运行时类型错误的程序，但同时也拒绝了可能不包含运行时错误的某些程序。这是基本递归论的一个不可避免的后果：语言运行时的类型错误是程序的一种不可判定的性质。
	* 运行期类型检查
- 集合论基本知识
	* 集合论是数学的机器语言
        * 空集、相等；序偶、笛卡尔乘积 
        * 罗素悖论
        * 朴素集合定义`(x|P(x))`，不会导致悖论的集合定义`(x \in A|P(x))`。
        * 幂集和幂集的幂集
        * 序偶，三元组，K元组
		* 良序（基）：集合A上的二元关系良序，其性质是不存在无限下降序列a0>a1>a2>...
			* <是A上的良序， iff A 的每个非空子集有最小元。
	* 关系
        * 自反、对称、传递；反对称、排中律；
        * 等价关系、偏序关系、全序关系；
	* 函数
        * 单射、满射、投影、双射；
        * 部分函数、全部函数；
        * 定义域、值域；
- 上下文无关文法
	* 参考`<<编译原理>>`笔记第二章
- 归纳法基本知识
	* 自然数归纳法，形式1：欲证明对每一个自然数n，P(n)为真，则只需先证明P(0)和对任意的自然数m，若P(m)为真，则P(m+1)亦然。
	* 自然数归纳法，形式2：欲证明P(n)对每一个自然数n为真，只要证明对任意自然数m，若P(i)对所有的i<m为真，则P(m)也必为真。
	* 结构归纳法，形式1：欲证明P(e)对于某文法产生的每个表达式e为真，只要证明P(e)对每个原子表达式成立，并且证明对任何带有直接子表达式e1,...,ek的复合表达式e，若P(ei)对于i=1,...,k成立，则P(e)也成立。
	* 结构归纳法，形式2：欲证明P(e)对由某种文法产生动每一个表达式e为真，只要证明对任何表达式e，若P(e'')对于每个e的子表达式e''成立，则P(e)成立。
	* 公理与推理规则通常写成模式，借以表达一个给定形式的所有公式或证明步骤。
		* 自反性公理: `e=e`，每个形如`e=e`的等式为公理。
		* 推理规则：`(A1 ... An)/B`,若有形如A1,...,An的公式的证明，则可组合它们得到对应公式B的证明。
		* 等式的传递的推理规则：`(e1=e2,e2=e3)/(e1=e3)`
	* 形式化而言，一个证明可定义为一个公式序列，其中每个公式或者证明是一个公理，不然则由前面的公式通过单一推理规则而得。由于每个推理规则通常有一系列前件和一个后件，很容易将证明看成一颗以公式标记的叶和内节点的形式的树。
		* 每个公理认为是可能的叶
		* 每条推理规`(A1 ... An)/B`作为可能的内部的分支节点，其子树比为A1,...,An的证明。
	* 证明上帝结构归纳法：欲证明P(π)对某一证明系统中的每个证明π为真，只要证明P对于该证明系统的每条公理成立，并且接着假定P对π1,...,πk等证明成立，则对任何以一条推理规则扩展π1,...,πk等证明之中的一个或多个而结束的任意证明，证明P(π)均成立。
	* 良序归纳法：设<是集合A上的良序二元关系，并且设P为A的某性质。若当P(A)对所有b<a成立时P(a)都成立，则P(a)对所有`a \in A`成立。
	

#####第二章 PCF语言
-------------------
- PCF语法
	* 每个PCF表达式都有唯一类型
		* 自然数、布尔值:`nat` `bool`，值:`true` `false`
		* 序偶、参数：笛卡尔乘机`σxτ`、函数`σ->τ`（以σ为定义域，τ为值域），函数是右结合的。
	* 表达式文法
		* `boolexp`=`true`|`false`|`if boolexp then boolexp else boolexp`|`Eq? natexp natexp`
		* `natexp`=`digital*`|`natexp+natexp`|`natexp x natexp`|`if boolexp then natexp else natexp`
		* `digital`=`0`|`1`|`2`|`3`|`4`|`5`|`6`|`7`|`8`|`9`
		* `σexp`=`if boolexp then σexp then σexp`
	* 公理
		* `0`+`0`=`0`,`0`+`1`=`1`,`1`+`0`=`1`,`1`+`1`=`2`,...
		* `if true then M else N=M`,`if false then M else N=N`
		* `Eq? n n = true`, `Eq? m n = false (m,n not equal)`
- 配对及其函数
	* 配对
		* 定义：若`M`具有类型`σ`，`N`具有类型`τ`，则`<M,N>` 具有类型 `σxτ`
		* 公理：
			* proj: `Proj1<M,N>=M`  `Proj2<M,N>=N`
			* sp: `<Proj1<M,N>,Proj2<M,N>>=<M,N>` 满射配对
	* 函数:
		* 高阶函数:`comp def= λf:nat->nat.λg:nat->nat.λx:nat.f(g(x))`
		* 公理：
			* α：`λx:σ.M=λy:σ.[y/x]M, y在M中不是自由的`
			* β: `(λx.σ.M)N=[N/x]M`
				* `[N/x]x=N`
				* `[N/x]a=a` `a`是常量或`a`不等`x`
				* `[N/x](PQ)=([N/x]P)([N/x]Q)`
				* `[N/x]λx:σ.M=λx:σ.M`
				* `[N/x]λy:σ.M=λy.σ.[N/x]M` `x`不等`y`且`y`不属于`FV(N)`
				* `[N/x]λy:σ.M=λz.σ.[N/x][z/y]M` `z`,`y`不等`x`且`z`不属于`FV(MN)`
			* η：`λx.σ.Mx=M` `x`不属于`FV(M)`
	* 参数分离：
		* `Curry=λf:(natxnat)->nat.λx:nat.λy:nat.f<x,y>`
		* `add=λp:natxnat.(Proj1p)+(Proj2p)`
		```  
		Curry(add)=(λf:(natxnat)->nat.λx:nat.λy:nat.f<x,y>)add  
		    =λx:nat.λy:nat.add<x,y>  
		    =λx:nat.λy:nat.(Proj1<x,y>)+(Proj2<x,y>)  
		    =λx:nat.λy:nat.x+y  
		```
- 惯用形化
	* 定义：`let x:σ=M in N def= (λx:σ.N)M`
	* 例子+去惯用形+规约：
	```
	let compose=λf:nat->nat.λg:nat->nat.λx:nat.f(g x) in
	  let h=λx:nat.x+x in
	    compose h h 3

	(λcompse:(nat->nat)->(nat->nat)->nat->nat.
	  (λh:nat->nat.compose h h 3) λx:nat.x+x)
	   λf:nat->nat.λg:nat->nat.λx:nat.f(g x)

	->(λh:nat->nat.(λf.nat->nat.λg:nat->nat.λx:nat.f(g x)) h h 3) λx:nat.x+x
	->(λf.nat->nat.λg:nat->nat.λx:nat.f(g x))(λx:nat.x+x)(λx:nat.x+x) 3
	->(λg:nat->nat.λx:nat.(λy:nat.y+y)(g x))(λx:nat.x+x) 3
	->(λx:nat.(λy:nat.y+y)((λz:nat.z+z) x) 3
	->(λy:nat.y+y)((λz:nat.z+z) 3)
	->(λy:nat.y+y)(3+3)
	->(3+3)+(3+3)
	->12
	```
* 不动点
	* 引入
	```
	letrec f:σ->σ=M in N
	let f:nat->nat=λy:nat.(if Eq? y 0 then 1 else y*f(y-1)) in f 5
	F def= λf:nat->nat.λy:nat.if Eq? y 0 then 1 else y*f(y-1)
	
	fixσ:(σ->σ)->(σ->σ)
	letrec f:σ->σ=M in N def= let f:σ->σ=(fixσ λf:σ.M) in N
	```

	* 公理:  
	```
	fix: fixσ=λf:σ->σ.f(fixσ f)
	fixσ M = M(fixσ M)
	```
	
	```
	fact n = (fix F) n
	      -->-> F(fix F)n
	       = (λf:nat->nat.λy:nat.if Eq? y 0 then 1 else y*f(y-1)) (fix F) n
	      -->-> if Eq? n 0 then 1 else n*(fix F)(n-1)
	```
	* 联立递归：
	```
	f = F f g
	g = G f g
	fixσxτ =λ<f,g>:<σ,τ>.<F f g,G f g>
	```
* 公理语义
	* 基本公理
		* `add` `Eq?` `cond` `proj` `sp` `α` `β` `η` `fix`
	* 推理规则
		* 等价(对称、传递) `(M=N)/(N=M)` `(M=N,N=P)=(M=P)`
		* 同余(nat、bool) `(M=N,P=Q)/(M+P=N+Q)` `(M=N,P=Q)/(Eq? M P=Eq? N Q)` 
		`(M1=M2,N1=N2,P1=P2)/(if M1 then N1 else P1=if M2 then N2 else P2)`
		* 序偶 `(M=N)/(ProjiM=ProjiN)` `(M=N,P=Q)/(<M,P>=<N,Q>)`
		* 函数  `(M=N)/(λx:σ.M=λx:σ.N)` `(M=N,P=Q)/(MP=NQ)`
		

#### 参考资料
-------------
1. [So yuo want to learn type theory](http://purelytheoretical.com/sywtltt.html)
2. [Logic, Languages, Compilation, and Verification](https://www.cs.uoregon.edu/research/summerschool/summer12/curriculum.html)
3. [Constructive Logic ](http://www.cs.cmu.edu/~fp/courses/15317-f09/schedule.html)
4. [Programming in Martin-L¨of ’s Type Theory](http://www.cse.chalmers.se/research/group/logic/book/book.pdf)
5. [Beginner's Guide to Linkers](http://www.lurklurk.org/linkers/linkers.html)
6. [The difference between top-down parsing and bottom-up parsing](http://qntm.org/top)

