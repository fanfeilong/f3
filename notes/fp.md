#### [Why Functional Programming Matters](https://www.cs.kent.ac.uk/people/staff/dat/miranda/whyfp90.pdf)

>As software becomes more and more complex, it is more and more
important to structure it well. **Well-structured software is easy to write
and to debug, and provides a collection of modules that can be reused
to reduce future programming costs**. In this paper we show that two features
of functional languages in particular, **higher-order functions** and **lazy
evaluation**, can **contribute significantly to modularity**. 

- Functional programs contain no assignment statements, so variables, once given a value, never change. 
- More generally, functional programs contain no side-effects at all. 
- A function call can have no effect other than to compute its result.

>Since expressions can be evaluated at any time, one can freely replace variables by
their values and vice versa.

>The most important difference between structured and unstructured programs is that structured
programs are designed in a modular way. Modular design brings with
it great productivity improvements

- First of all, small modules can be coded quickly and easily. 
- Second, general-purpose modules can be reused, leading to faster development of subsequent programs. 
- Third, the modules of a program can be tested independently, helping to reduce the time spent debugging

>When writing a modular program to
solve a problem, one first divides the problem into subproblems, then solves the
subproblems, and finally combines the solutions

>The ways in which one can
divide up the original problem depend directly on the ways in which one can glue
solutions together. Therefore, to increase one’s ability to modularize a problem
conceptually, one must provide new kinds of glue in the programming language.

— smaller and simpler 
- more general modules
- glued together with the new glues.

NOTE:
- tree是**统一的数据结构**，带来针对tree结构的算法的**强通用性**
- 无副作用（状态）+延迟性（幂等），带来**强可组合**

#### Y-Combinator

什么是Combinator?

1. 首先必须是个lambda表达式
2. 其次所有的变量都必须是lambda表达式的参数，也就是没有自由变量

**例子**:
```
(lambda (x) x) //是
(lambda (x) y) //不是，有自由变量y
(lambda (x) (lambda (y) x)) //是
(lambda (x) (lambda (y) (x y))) //是
(x (lambda (y) y)) //不是，非lambda表达式
((lambda (x) x) y) //不是，非lambda表达式
```

如何用lambda做递归?

1. 传入自己(待递归的lambda表达式)的拷贝。
2. 延迟（传入自己拷贝后的lambda表达式在调用自己拷贝处的）求值，以免整个lambda还没被使用就死递归在无限求值上。
  
所以这样得到的lambda递归会具有如下形式  
```
(define (part-factorial self)
    ((lambda (f)
       (lambda (n)
         (if (= n 0)
             1
             (* n (f (- n 1)))))) //此处f是(self,self)，如果不延迟求值，会无限递归
     (self self)))

(define factorial (part-factorial part-factorial))
```

剥离Y Combinator，以便复用
```
(define (Y f)
    ((lambda (x) (x x))       //这个是对下面那个lamba的(x x)
     (lambda (x) (f (x x))))) //这个是对目标lambda的(x x)
```

这种剥离只是为了复用，如果你不想复用，每次都像上面的factorial一样写那个结构也可以，理解这点才是重点。这就跟C语言的宏一样，很多用宏做的东西，你展开后看很简单，用宏只是在理解的基础上的复用。学习算法和数据结构也是一样，泛型容器和算法只是在你理解基础上的复用，但你要会手写才真的知道其中关键点。复用只是为了减少代码量，手写结构，并理解才是基础。

Y Combinator可以写成更直观：
```
(define (Y f)
    ((lambda (y) (y y))       //这个是对下面那个lamba的(y y)
     (lambda (x) (f (x x))))) //这个是对目标lambda的(x x)
```

全部都是x的那个版本，只是为了装代数符号的逼格而把上面的小y也写成x绕晕你。

Y Combinator刚好符合函数不动点性质
```
(Y f) = (f (Y f))
```

#### curry function
使用curry化，可以将多参数函数转成单参数函数，例如：
```
function fg(x,y)
  return x+y
end

fg(1,2)
```
可以写成：
```
function f(x)
  return function(y)
    return x+y
  end
end
f(1)(2)
```

#### lambda calculus
- lambda calculus是最小的图灵完备语言
  - variable：x
  - application:EE'
  - lambda abstraction: lambda.x.E
  - grammar: E ::= x|EE'|lambda.x.E
  - lambda：binding operator
    - lambda.x.xy, variable x is bound, variable y is free

- axioms of lambda calculus
  - a-Equivalence: lambda.x.E=lambda.y.E[y/x], Change of bound variable name
  - beta-Equivalence: (lambda.x.E)y = E[y/x], Application of Function to arguments
  - yita-Equivalence: lambda.x.Ex = E, Elimination of Redundant lambda abstractions

#### let-clause
- let x = E in E'
- (let x=E) in E'
- (ambda.x.E')E

#### Hindley-Milner Type Inference
- 为什么命令式语言里都没怎么看到这个东西？
http://csharpindepth.com/ViewNote.aspx?NoteID=70&printable=true
- HM type inference is not guaranteed to terminate in a reasonable amount of time; our type inference algorithm guarantees progress every time through the loop and therefore runs in polynomial time. (Though of course, overload resolution can then be exponential in C#, which is unfortunate.)
- HM type inference works poorly in a language which has class inheritance; it was designed for functional languages like Haskell with pattern matching rather than inheritance.

#### Monad
>In pure functional programming languages you aren't allowed to cause side effects. The only way a function can 'interact' with the outside world is to return something. So if you have a function that needs to return a number, but has a certain side effect, the only way to do this is to return the number and return some data 'containing' the side effect. But that's a pain in the ass - you have to write all of your code to return multiple values, and you have to write plumbing code to pass these side-effect containing values around. What monads can do is give a uniform API to side-effect containing data so you can just concentrate on the number that you want to return, and have the side-effect carried along automatically in the background. What's neat about monads is the same API can be used for many different problems, not just handling side-effects.

基于集合论的结构：群、环、域、模、拓扑空间、算子、单子等。唯一目的就是寻找这样的集合，其元素在给定几条公理化操作下，保持某些不变的性质。
这样的集合结构上可以建立公理、推导引理、推导定理，使得只要是这种结构的集合，就可以使用这些公理、引理、定理证明其拥有怎样的性质和结论。

单子(Monad)，正是这样一种结构，只是其刚好被这帮搞lisp的人用在了以函数为元素的集合上，并且这样的一种结构刚好能用解决函数级别的抽象封装问题。
想清楚这点，就不至于陷入细节而不知其所以然。

- [Promise是Monad吗?](https://cnodejs.org/topic/594a1823325c502917ef0ca0)

## refernces
- [why-mit-switched-from-scheme-to-python](https://www.wisdomandwonder.com/link/2110/why-mit-switched-from-scheme-to-python)
- [How to implement a programming language in JavaScript](http://lisperator.net/pltut/)
- [lisp-in-less-than-200-lines-of-c](https://carld.github.io/2017/06/20/lisp-in-less-than-200-lines-of-c.html)


