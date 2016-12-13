### 数学（Mathematics）

#### 书籍
---------
- `<<什么是数学>>`

#### 分支
---------
- 代数学
  - 高等代数
  - 抽象代数
    - 群
    - 环
    - 域
    - 模

- 几何学
  - 平面几何
  - 立体几何
  - 解析几何
  - 仿射几何
  - 射影几何
  - 黎曼几何

- 分析学
  - 实分析
  - [复分析](http://zh.wikipedia.org/zh/%E8%A4%87%E5%88%86%E6%9E%90)
    - [亚纯函数](http://zh.wikipedia.org/wiki/%E4%BA%9A%E7%BA%AF%E5%87%BD%E6%95%B0)

- 拓扑学
- 概率学
- 统计学
  - 数理统计
  - 贝叶斯统计
- 计算数学
  - 数值代数
  - 数值微分
  - 计算几何
  - 计算机几何造型
- 密码学
  * 对称加密
  * 非对称加密
    * RSA:
    ```
    N=pXq
F(N)=(p-1)*(q-1)
1-F(N)=><F(N),E>
E*D =1(% F(N))
public KEY: N,E，Encode: m^E%N=e1
private KEY:N,D，Decode: e1^D%N=m
    ```

- 数理逻辑, [mathematical logic](https://en.wikipedia.org/wiki/Mathematical_logic)
  - 子类别
    - 集合论，[set theory](https://en.wikipedia.org/wiki/Set_theory)
    - 模型论，[model theory](https://en.wikipedia.org/wiki/Model_theory)
    - 递归论，[recursion theory](https://en.wikipedia.org/wiki/Recursion_theory)
    - 证明论，[proof theory](https://en.wikipedia.org/wiki/Proof_theory)
      - 构造数学，[constructive mathematics](https://en.wikipedia.org/wiki/Constructive_mathematics)
  - 欧几里得公理，[Euclid's axioms]()
    - 平行公设,[parallel postulate](https://en.wikipedia.org/wiki/Parallel_postulate)
      - 罗巴切夫斯基几何(1826)，[Nikolai Lobachevsky](https://en.wikipedia.org/wiki/Nikolai_Lobachevsky)
    - 希尔伯特几何公理(1899)，[axioms for geometry](https://en.wikipedia.org/wiki/Hilbert%27s_axioms)
      - Pasch's axiom(1882)，[previous work](https://en.wikipedia.org/wiki/Pasch%27s_axiom)
  - 皮亚诺算数公理(1889)，[Peano axioms](https://en.wikipedia.org/wiki/Peano_axioms)
  - 维尔斯特拉斯，[arithmetization of analysis](https://en.wikipedia.org/wiki/Arithmetization_of_analysis)
  - [Bolzano(1817)](https://en.wikipedia.org/wiki/Bernard_Bolzano)，[(ε-δ)-definition](https://en.wikipedia.org/wiki/%28%CE%B5,_%CE%B4%29-definition_of_limit) and [continuous functions](https://en.wikipedia.org/wiki/Continuous_function)，[Cauchy(1821)](https://en.wikipedia.org/wiki/Cauchy)，[Dedekin cuts 1858](https://en.wikipedia.org/wiki/Dedekind_cuts)
  - 康托集合论，[Georg Cantor](https://en.wikipedia.org/wiki/Georg_Cantor)
    - 势，[cardinality](https://en.wikipedia.org/wiki/Cardinality)
    - [Cantor's theorem](https://en.wikipedia.org/wiki/Cantor%27s_theorem)
    - 选择公理，[axiom of choice 1904](https://en.wikipedia.org/wiki/Axiom_of_choice)，[Ernst zermelo](https://en.wikipedia.org/wiki/Ernst_Zermelo)
      - gave a proof that every set could be well-ordered, a result Cantor had been unable to obtain
      - [Zermelo 1908a](https://en.wikipedia.org/wiki/Mathematical_logic#CITEREFZermelo1908a)
      - [zermelo 1908b](https://en.wikipedia.org/wiki/Mathematical_logic#CITEREFZermelo1908b)
        - [axiom of replacement](https://en.wikipedia.org/wiki/Axiom_of_replacement) by [Abraham Fraenkel](https://en.wikipedia.org/wiki/Abraham_Fraenkel)
        - => [Zermelo-Fraenkel set theory (ZF)](https://en.wikipedia.org/wiki/Zermelo%E2%80%93Fraenkel_set_theory)
          - avoid Russell's paradox
          - [Fraenkel 1922](https://en.wikipedia.org/wiki/Mathematical_logic#CITEREFFraenkel1922) proved that the axiom of choice cannot be proved from the remaining axioms of Zermelo's set theory with [urelements](https://en.wikipedia.org/wiki/Urelements)
            - [Paul Cohen 1966](https://en.wikipedia.org/wiki/Paul_Cohen_(mathematician)) showed that the addition of urelements is not needed, and the axiom of choice is unprovable in ZF.
              - [forcing](https://en.wikipedia.org/wiki/Forcing_%28mathematics%29)
    - [Hilbert 23 problems](https://en.wikipedia.org/wiki/Hilbert%27s_problems)
    - 悖论
      - [Burali-Forti paradox 1897](https://en.wikipedia.org/wiki/Burali-Forti_paradox)
        - the collection of all ordinal numbers cannot form a set
      - [Russell's paradox 1901](https://en.wikipedia.org/wiki/Russell%27s_paradox)
        - 数学原理，[Principia Mathematica](https://en.wikipedia.org/wiki/Principia_Mathematica) by Russell and [Alfred North Whitehead](https://en.wikipedia.org/wiki/Alfred_North_Whitehead)
      - [Richard's paradox 1905](https://en.wikipedia.org/wiki/Richard%27s_paradox)
      - [Leopold Lowenheim 1915](https://en.wikipedia.org/wiki/Leopold_L%C3%B6wenheim) and [thoralf Skolem 1920](https://en.wikipedia.org/wiki/Mathematical_logic#CITEREFSkolem1920)
        - first-order logic cannot control the cardinalities of infinite structures
        - [Skolem's paradox](https://en.wikipedia.org/wiki/Skolem%27s_paradox)
    - [Kurt Gödel](https://en.wikipedia.org/wiki/Kurt_G%C3%B6del), 
      - [completeness theorem 1929](https://en.wikipedia.org/wiki/Completeness_theorem)
        - establishes a correspondence between syntax and semantics in first-order logic.
        - use completeness theorem to prove the [compactness theorem](https://en.wikipedia.org/wiki/Compactness_theorem)
        - establish first-order logic as the dominant logic used by mathematicans
      - [Gödel's incompleteness theorem 1931](https://en.wikipedia.org/wiki/G%C3%B6del%27s_incompleteness_theorem)
        - incompleteness (in a different meaning of the word) of all sufficiently strong, effective first-order theories
        - the impossibility of providing a consistency proof of arithmetic within any formal theory of arithmetic
        - a consistency proof of any sufficiently strong, effective axiom system cannot be obtained in the system itself
    - [Nicolas Bourbaki](https://en.wikipedia.org/wiki/Nicolas_Bourbaki)
    - [computable function](https://en.wikipedia.org/wiki/Computable_function)

#### Approximation Theory
数值逼近里有：拉格朗日插值、牛顿插值、样条函数逼近、最佳一致逼近、最佳平分逼近（最小二乘），非线性逼近这些基础的。样条里，有beizier和b-spline逼近。线性空间上的逼近简单说就是把目标函数在指定希尔伯特空间的基函数上求一个线性展开（表出），让这个逼近函数和原函数在某种范数下的差距足够小。例如，可以在傅里叶基函数上展开，可以在小波基函数上展开，还有小波变换的各种变体，例如斜小波之类的，这种变体一般一组unit之间不再是线性无关的基函数，而是带有一定的冗余，冗余的好处是不必所有基函数上都有权重，适合在一些压缩感知领域上使用。像神经网络能计算任意能表示成傅里叶基函数展开的函数，是因为神经网络可以计算傅里叶函数的基函数，那么再线性组合也不是难事。跟逼近相关的数学方向有：逼近论、数值逼近、计算几何等，例如这里有逼近论会用到的各种概念的一个小字典：http://www.emis.de/journals/SAT/concepts.pdf ，核心其实就是基函数、误差范数、各种线性表出恒等式等。

- [History of Approximation Theory](http://www.math.technion.ac.il/hat/)
- [Journal of Approximation Theory](https://people.math.osu.edu/nevai.1/JAT/)

#### 参考资料
-------------
- [Gallery of animations that explain math ideas](https://en.wikipedia.org/wiki/User:LucasVB/Gallery)
- `<<黎曼猜想漫谈>>`,卢仓海著，[->豆瓣](http://book.douban.com/subject/11506872/)
- [黎曼](http://zh-yue.wikipedia.org/wiki/%E9%BB%8E%E6%9B%BC)
- [休.蒙哥马利](http://en.wikipedia.org/wiki/Hugh_Montgomery_%28mathematician%29)
    - 假如你是一个魔鬼，引诱数学家用自己的灵魂来换取一个定理的证明。多数数学家想要换取的会是什么定理呢？我想会是黎曼猜想。
- [wiki:Smoothing](http://en.wikipedia.org/wiki/Smoothing)
- [wiki:Curve Fitting](http://en.wikipedia.org/wiki/Curve_fitting)
- [wiki:spline](http://en.wikipedia.org/wiki/Cubic_splines)

<style class="fallback">body{white-space:pre;font-family:monospace}</style><script src="markdeep.min.js"></script><script src="http://casual-effects.com/markdeep/latest/markdeep.min.js"></script>