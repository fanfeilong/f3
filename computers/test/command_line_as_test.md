

[CLAT]编写可命令行单元测试的程序
====

许多人，所不知道的是，每一种编程语言都有其对应的单元测试框架。我在实际动手编写程序许久之后才听说“单元测试”、“模块测试”、“集成测试”这三个重要的测试阶段。从一个程序的角色来说，“单元测试”、“模块测试”、“集成测试”这三个部分就是最最最最重要的核心的测试环节。断言，就像任何阶梯型技术一样，通常情况下“单元测试”、“模块测试”、和“集成测试”三者的最佳比例应该保持7:2:1的黄金比例（[1], 如果不对，请你提出更合理的比例，并论证）。抛开你一定会做的集成测试不说，断言，优秀的程序应该像猎人一样对单元测试和模块测试保持敏感。回到开头，每一种编程语言都有其对应的单元测试框架，你可以从这些编程语言相关的单元测试框架茫茫多的文档里，从HelloWorld开始学习单元测试。断言，基本上你也就只会学了HelloWorld的单元测试框架，然后就再也不看这些写的十分啰嗦的文档了。

那么，本文就是为你这样重视单元测试和模块测试的优秀的程序提供的一个独特的的视角。开门见山，我们会以最直接的方式展示游离在单元测试和模块测试之间的边界地带。即，如何一本万利的掌握并实践单元测试和模块测试；这种方式应该被作为一种基本的思想深深植入你的编程思维里面，并成为你编写可靠程序的利器。是的，呼应标题，我们很直接的，就是要通过命令行的方式测试程序，把通过命令行测试程序作为一种基本的手段天天使用。

准备素材
----

由于不希望长篇累醉，我们可以复用网络上的针对传统单元测试写的入门级教程作为基础，例如下面两个素材：
1. https://www.cnblogs.com/SivilTaram/p/software_pretraining_java.html
2. http://www.cnblogs.com/SivilTaram/p/software_pretraining_cpp.html

准备环境
----
1. 安装现代编辑器：https://code.visualstudio.com/ ，实际上你使用什么编辑器/IDE，都是可以的，正如Microsoft收购了Github之后，Github的新CEO Nat Friedman 所说的：
“开发者都有自己的喜好，选择哪个编辑器完全取决于他们自己。”[2].
2. 在你的本机安装好Java环境和C++开发环境，这部分步骤通常是充满配置感的，如果你遇到了防火墙的问题，应当通过各种途径绕过它。
3. 拥有一个Github账号。

HelloWorld
----
配置好了环境，接下来这个步骤是清晰可见的。你只要：
1. 已经从素材的教程里学会了git的操作方法（如果还不熟悉，这里有一打简明指南：https://www.cnblogs.com/math/p/git.html ）
2. 从https://github.com/SivilTaram/Calculator 上fork代码到自己的github仓库，并clone到自己的本机。

现在，你的本地工程目录应该如下：
```
.
├── LICENSE
├── README.md
└── src
    └── Main.java
```

通过观察，很难相信如今还能有这么简单的只有一个Java源代码的程序。但这正如我们希望的简单可处理：
* 选择，不使用Java IDE去处理该程序。
* 选择，使用`java`,`javac`等命令行来处理该程序。这么做的理由十分充分：使用IDE/编辑器开发Java代码十分常见，但基本上你也可以选择同时使用命令行来做各种编程工作的切口。

通过诸如查阅“Java命令行如何使用”，“如何在命令行下使用Java”，“如何在命令行下执行Java程序”，“How to run java program in command line”，“java”，“jav
ac”之类的关键字，你很快上手编译并测试了该程序：
1. `javac Main.java`
2. `java Main`

输出：
```
40*89-81
40*89-81=3479
```

修改代码
------
已有的程序可以正确执行并输出结果这十分重要，如果素材里提供的程序是有BUG的或者环境耦合严重，则我们可能在这个步骤会话费数倍、数十倍的时间。也许此时你已经在所需的编辑器、源代码管理工具、语言开发环境上耗费了诸多时间。

原文素材里面使用了一个叫做JUnit的单元测试框架对源代码里`Main.java`里的方法`public static String Solve(String formula)`做了单元测试，并能在IDE里调试。而，本文抛弃了单元测试框架，并且不打让你F11到单步调试这种常规工具里。我们会使用日志诊断代码。[3]

阅读Main.java想必不会花费你的很多时间，不过我们只需关心`public static void main(String[] args)`方法即可，源代码如下：
```java
public static void main(String[] args) {
    String question = MakeFormula();  
    System.out.println(question);
    String ret = Solve(question);
    System.out.println(ret);
}
```

这个代码的逻辑，用人类语言表达就是：
- 生成四则运算题目
	- 输出四则运算题目
- 计算四则运算题目
	- 输出四则运算结果

修改这个程序，用人类语言表达就是：
- **从命令行参数接受四则运算题目**
	- 输出四则运算题目
- 计算四则运算题目
	- 输出四则运算结果

翻译会Java代码就是：
```java
public static void main(String[] args) {
	// 1. 从命令行直接输入题目  
    String question = String.join(""，args);    

    // 2. 修改输出日志，以和之前的形式做区分
    System.out.println("question from commandline:",question); 

    String ret = Solve(question);
    System.out.println(ret);
}
```

重新编译并执行该程序，输入之前已经验证过能正确执行的四则运算：
1. `javac Main.java`
2. `java Main "40*89-81"`

可以得到期待中的结果：
```
question from commandline:40*89-81
40*89-81=3479
```

测试用例
-----

上述的`java Main "40*89-81"` 就是在使用命令行测试程序时的一个例子，即，测试用例。这个测试唯一的作用就是表明该程序能接受简单的输入。我们来尝试输入各种奇怪的输入，看看效果怎样。

先看看这个程序是否是个toy程序：
* 测试除零：`java Main "1/0"`

输出：
```
question from commandline:1/0
Exception in thread "main" java.lang.ArithmeticException: / by zero
	at Main.Solve(Main.java:87)
	at Main.main(Main.java:10)
```

很显然，立刻就验证了这是一个toy程序。“代码写的这么渣，一看就是小学没毕业，不知道不能除零么！！”。
当然，要修改代码，就需要读懂原来代码的逻辑。很显然`Solve`函数里对除数为零并没有做防御处理，实用的程序会根据上下文里对异常处理的需求做相应的处理。此处我们可以仅仅简单处理：
1. 捕获除法异常
2. 结束Solve程序，抛出更友好的结果。

修改代码如下：
```
try{
    calcStack.push(String.valueOf(a1 / b1));
}catch(ArithmeticException e){
    return "ERROR:"+a1+"/ 0 is not allowed.";
}
```

再次测试下代码：
* 测试除零：`java Main "1/0"`

输出：
```
question from commandline:1/0
ERROR:0/ 0 is not allowed.
```

结果看上去更好了么？

分析代码
-----
其实是有问题的，新输出的错误信息：“ERROR:0/ 0 is not allowed.” 暴露了四则运算算法的潜在问题。不用看算法的细节，我们的输入是“1/0”，出现异常的地方应该期待的是“a1=1”，怎么会输出“0/0”的错误信息呢？

通过对方法Solve的分析，我们可以看到代码明显的分为了两部分。如果你对编译技术十分熟悉，或者部分熟悉，你也可以猜得出这代码大致分为parser-executor两部分。

**parser**部分：
```java
Stack<String> tempStack = new Stack<>();//Store number or operator
Stack<Character> operatorStack = new Stack<>();//Store operator
int len = formula.length();
int k = 0;
for(int j = -1; j < len - 1; j++){
    char formulaChar = formula.charAt(j + 1);
    if(j == len - 2 || formulaChar == '+' || formulaChar == '-' || formulaChar == '/' || formulaChar == '*') {
        if (j == len - 2) {
            tempStack.push(formula.substring(k));
        }
        else { // NOTE(fanfeilong): 此处我们吐槽下原作者留下的代码风格不统一的问题
            if(k < j){
                tempStack.push(formula.substring(k, j + 1));
            }
            if(operatorStack.empty()){
                operatorStack.push(formulaChar); //if operatorStack is empty, store it
            }else{
                char stackChar = operatorStack.peek();
                if ((stackChar == '+' || stackChar == '-')
                        && (formulaChar == '*' || formulaChar == '/')){
                    operatorStack.push(formulaChar);
                }else {
                    tempStack.push(operatorStack.pop().toString());
                    operatorStack.push(formulaChar);
                }
            }
        }
        k = j + 2;
    }
}

while (!operatorStack.empty()){ // Append remaining operators
    tempStack.push(operatorStack.pop().toString());
}
```

**executor**部分:
```java
Stack<String> calcStack = new Stack<>();
for(String peekChar : tempStack){ // Reverse traversing of stack
    if(!peekChar.equals("+") && !peekChar.equals("-") && !peekChar.equals("/") && !peekChar.equals("*")) {
        calcStack.push(peekChar); // Push number to stack
    }else{
        int a1 = 0;
        int b1 = 0;
        if(!calcStack.empty()){
            b1 = Integer.parseInt(calcStack.pop());
        }
        if(!calcStack.empty()){
            a1 = Integer.parseInt(calcStack.pop());
        }
        switch (peekChar) {
            case "+":
                calcStack.push(String.valueOf(a1 + b1));
                break;
            case "-":
                calcStack.push(String.valueOf(a1 - b1));
                break;
            case "*":
                calcStack.push(String.valueOf(a1 * b1));
                break;
            default:
                try{
                    calcStack.push(String.valueOf(a1 / b1));
                }catch(ArithmeticException e){
                    return "ERROR:"+a1+"/ 0 is not allowed.";
                }
                
                break;
        }
    }
}
return formula + "=" + calcStack.pop();
```

**executor**部分一眼可以（或者两眼）看出来是在做什么事情，大概是类似这样的cpu：
```
   [],[135+-]
-> [1],[35+-]
-> [1,3],[5+-]
-> [1,3,5],[+-]
-> [1,3,5],+,[-]
-> [1],(5+3),[-]
-> [1,8],[-]
-> [1,8],-
-> [],(8-1)
-> [7]
-> 7
```

拨的一手好算盘。那么显然**parser**部分的作用就是把"3+5-1"转换成"135+-"。不过根据本节开头的证据，显然parser部分是有问题的。

日志系统
-------
测试暴露出来的蛛丝马迹都是不能放过的。是时候放出那句话了：
>找BUG这件事，只要读代码+理解代码就好了。

当然，可以对其稍加阐释，以便转化为更可操作性的步骤。
1. 对代码的真正理解，是解决BUG的核心要义。
2. 对关键路径添加日志，使得数据和指令的流动清晰可见。

第1句是同一重复，第2句话才是本节想要引入的。在不查阅四则运算算法的时候（事实上很多代码并没有一个公开清晰的文档可以查阅，阅读看似难以理解的代码是一个合格程序日常的重要组成部分），我们希望真正理解代码就要分析代码。当然你可以使用现代IDE提供的调试技术，便利的对代码深入深出。

现在，采用原始的方式，希望直接在parser部分代码里添加日志。通过观察“数据”的流动，parser部分的核心数据流动是在`tempStack`和`operatorStack`两个栈上。我们的日志应该重点跟踪这两个数据的流动。定制一个简单的针对Stack的日志函数，它满足：
1. 能直接打印Stack
2. 带有时间戳、文件名、行号

现在就自己动手加一个：
```java
public static void DumpStack(String tip, Stack stack){
    // now 
    Date date = new Date();
    long times = date.getTime();
    SimpleDateFormat formatter = new SimpleDateFormat("yyyy-MM-dd HH:mm:ss");
    String now = formatter.format(date);

    // file+lineNo
    int lineNo = Thread.currentThread().getStackTrace()[2].getLineNumber();
    String fileName = Thread.currentThread().getStackTrace()[2].getFileName();
    String location = ""+fileName+":"+lineNo;

    // output
    System.out.println("["+now+"]"+tip+Arrays.toString(stack.toArray())+","+location);
}
```

现在就用来跟踪parser的数据流动：
```java
for(int j = -1; j < len - 1; j++){
    char formulaChar = formula.charAt(j + 1);
    if(j == len - 2 || formulaChar == '+' || formulaChar == '-' || formulaChar == '/' || formulaChar == '*') {
        String index = "[j:"+j+",k:"+k+",char:"+formulaChar+"]";
        if (j == len - 2) {
            tempStack.push(formula.substring(k));
            DumpStack(index+"tempStack:",tempStack);
        }
        else {
            if(k < j){
                tempStack.push(formula.substring(k, j + 1));
                DumpStack(index+"tempStack:",tempStack);
            }
            if(operatorStack.empty()){
                operatorStack.push(formulaChar); //if operatorStack is empty, store it
                DumpStack(index+"operatorStack:",operatorStack);
            }else{
                char stackChar = operatorStack.peek();
                if ((stackChar == '+' || stackChar == '-')
                        && (formulaChar == '*' || formulaChar == '/')){
                    operatorStack.push(formulaChar);
                    DumpStack(index+"operatorStack:",operatorStack);
                }else {
                    tempStack.push(operatorStack.pop().toString());
                    DumpStack(index+"tempStack:",tempStack);

                    operatorStack.push(formulaChar);
                    DumpStack(index+"operatorStack:",operatorStack);
                }
            }
        }
        k = j + 2;
    }
}

while (!operatorStack.empty()){ // Append remaining operators
    tempStack.push(operatorStack.pop().toString());
    DumpStack("tempStack:",tempStack);  // <7>
}
```

我们从<1>到<7>添加了7处数据流动的日志。现在就来编译-测试一下：
1. `javac Main.java`
2. `java Main.java "40*89-81"` 

输出：
```
question from commandline:40*89-81
[2018-06-20 01:03:33][j:1,k:0,char:*]tempStack:[40],Main.java:63
[2018-06-20 01:03:33][j:1,k:0,char:*]operatorStack:[*],Main.java:67
[2018-06-20 01:03:33][j:4,k:3,char:-]tempStack:[40, 89],Main.java:63
[2018-06-20 01:03:33][j:4,k:3,char:-]tempStack:[40, 89, *],Main.java:76
[2018-06-20 01:03:33][j:4,k:3,char:-]operatorStack:[-],Main.java:79
[2018-06-20 01:03:33][j:6,k:6,char:1]tempStack:[40, 89, *, 81],Main.java:58
[2018-06-20 01:03:33]tempStack:[40, 89, *, 81, -],Main.java:89
40*89-81=3479
```

很好，这个纯收工打造的日志函数麻雀虽小，五脏俱全。每一条日志包含时间、日志信息、文件名、行号。
有了日志系统，我们就可以开始分析parser的数据流动，这就能便利的理解代码。可以看到：
* 程序从左往右扫描表达式：`"40*89-81"`
	* 一直扫描到操作符OP，把操作符前面的数字全部丢进tempStack
	* 如果operatorStack为空，直接把操作符丢进operatorStack
	* 如果operatorStack不为空，则：
		* 如果，operatorStack栈顶的操作符的优先级低于当前遇到的OP，则当前操作符也直接丢进operatorStack
		* 否则，operatorStack栈顶的操作符弹出进入tempStack，当前操作符则丢进operatorStack

如果观察上面的日志行号，添加空行之后，可以看的更清晰：
```
question from commandline:40*89-81
[2018-06-20 01:03:33][j:1,k:0,char:*]tempStack:[40],Main.java:63
[2018-06-20 01:03:33][j:1,k:0,char:*]operatorStack:[*],Main.java:67

[2018-06-20 01:03:33][j:4,k:3,char:-]tempStack:[40, 89],Main.java:63
[2018-06-20 01:03:33][j:4,k:3,char:-]tempStack:[40, 89, *],Main.java:76
[2018-06-20 01:03:33][j:4,k:3,char:-]operatorStack:[-],Main.java:79

[2018-06-20 01:03:33][j:6,k:6,char:1]tempStack:[40, 89, *, 81],Main.java:58

[2018-06-20 01:03:33]tempStack:[40, 89, *, 81, -],Main.java:89
40*89-81=3479
```

那么，为什么"1/0"出问题呢？跑下日志：
* `java Main.java "1/0"` 

输出：
```
question from commandline:1/0
[2018-06-20 01:10:19][j:0,k:0,char:/]operatorStack:[/],Main.java:67
[2018-06-20 01:10:19][j:1,k:2,char:0]tempStack:[0],Main.java:58
[2018-06-20 01:10:19]tempStack:[0, /],Main.java:89
ERROR:0/ 0 is not allowed.
```

对比一下，立刻可以看出，首次数据流动发生在67行，而之前的首次是在62行，62行是什么代码呢？如下：
```java
if(k < j){
    tempStack.push(formula.substring(k, j + 1));
    DumpStack(index+"tempStack:",tempStack);
}
```

可以看到，原来的parser在首次遇到操作符时的索引变量是："[j:0,k:0,char:/]"，此时,k===j，因此不会进入62行的逻辑，也就是：
* 一直扫描到操作符OP，**把操作符前面的数字全部丢进tempStack**


重现BUG
-------
可以预计，类似的问题可以在同类型测试用例上出现，例如：
* `java Main.java "1/1"`

输出：
```
question from commandline:1/1
[2018-06-20 01:17:29][j:0,k:0,char:/]operatorStack:[/],Main.java:67
[2018-06-20 01:17:29][j:1,k:2,char:1]tempStack:[1],Main.java:58
[2018-06-20 01:17:29]tempStack:[1, /],Main.java:89
1/1=0
``` 

但是稍加偏移就不会出现：
* `java Main.java "100/1"`

输出：
```
question from commandline:100/1
[2018-06-20 01:17:21][j:2,k:0,char:/]tempStack:[100],Main.java:63
[2018-06-20 01:17:21][j:2,k:0,char:/]operatorStack:[/],Main.java:67
[2018-06-20 01:17:21][j:3,k:4,char:1]tempStack:[100, 1],Main.java:58
[2018-06-20 01:17:21]tempStack:[100, 1, /],Main.java:89
100/1=100
```

进一步，只要遇到一位数，程序必然出现BUG，以至于会出现十分荒谬的结果，差之毫厘，谬以千里：
* `java Main "100+2-3/10"`

输出：
```
question from commandline:100+2-3/10
[2018-06-20 01:27:26][j:2,k:0,char:+]tempStack:[100],Main.java:63
[2018-06-20 01:27:26][j:2,k:0,char:+]operatorStack:[+],Main.java:67
[2018-06-20 01:27:26][j:4,k:4,char:-]tempStack:[100, +],Main.java:76
[2018-06-20 01:27:26][j:4,k:4,char:-]operatorStack:[-],Main.java:79
[2018-06-20 01:27:26][j:6,k:6,char:/]operatorStack:[-, /],Main.java:73
[2018-06-20 01:27:26][j:8,k:8,char:0]tempStack:[100, +, 10],Main.java:58
[2018-06-20 01:27:26]tempStack:[100, +, 10, /],Main.java:89
[2018-06-20 01:27:26]tempStack:[100, +, 10, /, -],Main.java:89
100+2-3/10=-10

```

可见，BUG产生的原因是变量`j`只前进里一次就碰到了操作符，此时`j`等于0,同时`k`保持不动也等于0，从而未能正确处理。

断言
------

至此，可以对源代码做直接的BUG修正。但是可以多加思考一下，62行处的代码，真的有必要`if(k<j)`么？

严密的逻辑，一个程序就是一个状态机。每一个状态的改变，都会导致程序向下一个状态转换。如何证明一个程序的状态切换是
正确的呢？有一种方式是像数学一样严格证明程序的正确性，对程序做形式验证。但是，通常程序的互联网程序开发并不会如此做，
这有其本身的学习曲线和成本问题。但是，理解程序状态机的内在语义，则十分有助于形成严密的逻辑。[4]

我们关心可操作性胜过理论和完美，在开发实践中，使用断言来严格的检查程序的状态是十分有用的。因此，纯收工制造一个断言函数，该函数满足：
* 判断条件是否满足
* 如果不满足，直接让程序崩溃

代码如下：
```java
public static void Assert(boolean condition, String errorLog){
    if(!condition){
        // now 
        Date date = new Date();
        long times = date.getTime();
        SimpleDateFormat formatter = new SimpleDateFormat("yyyy-MM-dd HH:mm:ss");
        String now = formatter.format(date);

        // file+lineNo
        StackTraceElement[] elements = Thread.currentThread().getStackTrace();
        int lineNo = elements[2].getLineNumber();
        String fileName = elements[2].getFileName();
        String location = ""+fileName+":"+lineNo;

        System.out.println("["+now+"]"+errorLog+","+location);
        
        for(int i=0; i<elements.length; i++) {
            System.out.println(elements[i]);
        }
        
        System.exit(1);
    }
}
```

根据分析，当遇到操作符并且不是末尾的时候，一定是要把前面的非操作符丢进tempStack的，所以62行前后的代码修改为：
```java
Assert(k<j,"k is not less then j, [k:"+k+",j:"+j+"]");
tempStack.push(formula.substring(k, j + 1));
DumpStack(index+"tempStack:",tempStack);
```

编译，测试非边界用例：
1. `javac Main.java`
2. `java Main "100+2-30/10"`

输出：
```
question from commandline:100+20-30/10
[2018-06-20 01:56:00][j:2,k:0,char:+]tempStack:[100],Main.java:88
[2018-06-20 01:56:00][j:2,k:0,char:+]operatorStack:[+],Main.java:92
[2018-06-20 01:56:00][j:5,k:4,char:-]tempStack:[100, 20],Main.java:88
[2018-06-20 01:56:00][j:5,k:4,char:-]tempStack:[100, 20, +],Main.java:101
[2018-06-20 01:56:00][j:5,k:4,char:-]operatorStack:[-],Main.java:104
[2018-06-20 01:56:00][j:8,k:7,char:/]tempStack:[100, 20, +, 30],Main.java:88
[2018-06-20 01:56:00][j:8,k:7,char:/]operatorStack:[-, /],Main.java:98
[2018-06-20 01:56:00][j:10,k:10,char:0]tempStack:[100, 20, +, 30, 10],Main.java:83
[2018-06-20 01:56:00]tempStack:[100, 20, +, 30, 10, /],Main.java:114
[2018-06-20 01:56:00]tempStack:[100, 20, +, 30, 10, /, -],Main.java:114
100+20-30/10=117
```

测试边界用例：
* `java Main "100+2-3/10"`

输出：
```
question from commandline:100+20-3/10
[2018-06-20 01:56:03][j:2,k:0,char:+]tempStack:[100],Main.java:88
[2018-06-20 01:56:03][j:2,k:0,char:+]operatorStack:[+],Main.java:92
[2018-06-20 01:56:03][j:5,k:4,char:-]tempStack:[100, 20],Main.java:88
[2018-06-20 01:56:03][j:5,k:4,char:-]tempStack:[100, 20, +],Main.java:101
[2018-06-20 01:56:03][j:5,k:4,char:-]operatorStack:[-],Main.java:104
[2018-06-20 01:56:03]k is not less then j, [k:7,j:7],Main.java:86
java.lang.Thread.getStackTrace(Thread.java:1556)
Main.Assert(Main.java:57)
Main.Solve(Main.java:86)
Main.main(Main.java:14)
```

可见，使用了断言，可以让错误在尽量靠近错误现场的地方被探测到。


BUG修正
-----

测试、重现、添加日志、分析、使用断言，是时候修正一下当前的BUG了。前面的断言应该被改成正确的版本：
```java
Assert(k<=j,"k is not less then j, [k:"+k+",j:"+j+"]");
tempStack.push(formula.substring(k, j + 1));
DumpStack(index+"tempStack:",tempStack);
```

编译：
* `javac Main.java`

测试1:
* `java Main "1/1"`

输出：
```
question from commandline:1/1
[2018-06-20 02:03:37][j:0,k:0,char:/]tempStack:[1],Main.java:88
[2018-06-20 02:03:37][j:0,k:0,char:/]operatorStack:[/],Main.java:92
[2018-06-20 02:03:37][j:1,k:2,char:1]tempStack:[1, 1],Main.java:83
[2018-06-20 02:03:37]tempStack:[1, 1, /],Main.java:114
1/1=1
```

测试2:
* `java Main "100+2-3/10"`

输出：
```
question from commandline:100+20-3/10
[2018-06-20 02:02:11][j:2,k:0,char:+]tempStack:[100],Main.java:88
[2018-06-20 02:02:11][j:2,k:0,char:+]operatorStack:[+],Main.java:92
[2018-06-20 02:02:11][j:5,k:4,char:-]tempStack:[100, 20],Main.java:88
[2018-06-20 02:02:11][j:5,k:4,char:-]tempStack:[100, 20, +],Main.java:101
[2018-06-20 02:02:11][j:5,k:4,char:-]operatorStack:[-],Main.java:104
[2018-06-20 02:02:11][j:7,k:7,char:/]tempStack:[100, 20, +, 3],Main.java:88
[2018-06-20 02:02:11][j:7,k:7,char:/]operatorStack:[-, /],Main.java:98
[2018-06-20 02:02:11][j:9,k:9,char:0]tempStack:[100, 20, +, 3, 10],Main.java:83
[2018-06-20 02:02:11]tempStack:[100, 20, +, 3, 10, /],Main.java:114
[2018-06-20 02:02:11]tempStack:[100, 20, +, 3, 10, /, -],Main.java:114
100+20-3/10=120
```

练习：
-----
1. 修复了一个BUG么，拔出萝卜带出泥，新的问题是没有正确处理分数的情况。
2. 这算不上BUG，可以算是不支持的Feature，请你添加代码支持分数的情况。


批量测试：
-----

在经历了相对精细的一组分析之后，我们还是只能一个一个通过命令行测试程序。有没办法方便的添加测试用例，批量执行呢？

对于命令行程序而言，做到这点很简单。例如在该程序里，使用你熟悉的语言添加一个命令行批量执行的程序即可。

使用Java的版本如下：
1. 创建Test.java
2. 编写如下简单易懂的代码：

```java
import java.lang.Exception;
import java.io.BufferedReader;
import java.io.InputStreamReader;

public class Test {
    public static void main(String[] args) {
        String[] tests = new String[]{
            "0+1",
            "0-1",
            "0*0",
            "1/0",
            "100+20-3/10",
            "100+20-30/10"
        };

        int successCount=0;
        System.out.println("TEST BEGIN");
        System.out.println("----------");
        for(int i=0;i<tests.length;i++){
            int ret = runTest(tests[i]);
            if(ret==0){
                successCount++;
                System.out.println("[SUCCESS]:"+tests[i]);
            }else{
                System.out.println("[FAILED]:"+tests[i]+", ret:"+ret);
            }   
        }
        int failedCount = tests.length-successCount;

        System.out.println("----------");
        System.out.println("TEST END, "+successCount+" success, "+failedCount+" failed.");
    }

    private static int runTest(String exp) {

		StringBuffer output = new StringBuffer();
        int ret=0;
		Process p;
		try {
			p = Runtime.getRuntime().exec(new String[]{"java","Main",exp});
			
			BufferedReader reader = new BufferedReader(new InputStreamReader(p.getInputStream()));

            String line = "";			
			while ((line = reader.readLine())!= null) {
				output.append(line + "\n");
			}

            ret = p.waitFor();
		} catch (Exception e) {
			e.printStackTrace();
            ret = -1;
		}

	    System.out.println(output.toString());
        return ret;
	}
}
```

编译，测试：
1. javac Teat.java
2. java Test

输出：
```
TEST BEGIN
----------
[Ljava.lang.String;@7852e922
question from commandline:0+1
[2018-06-20 02:57:27][j:0,k:0,char:+]tempStack:[0],Main.java:88
[2018-06-20 02:57:27][j:0,k:0,char:+]operatorStack:[+],Main.java:92
[2018-06-20 02:57:27][j:1,k:2,char:1]tempStack:[0, 1],Main.java:83
[2018-06-20 02:57:27]tempStack:[0, 1, +],Main.java:114
0+1=1

[SUCCESS]:0+1
[Ljava.lang.String;@7852e922
question from commandline:0-1
[2018-06-20 02:57:27][j:0,k:0,char:-]tempStack:[0],Main.java:88
[2018-06-20 02:57:27][j:0,k:0,char:-]operatorStack:[-],Main.java:92
[2018-06-20 02:57:27][j:1,k:2,char:1]tempStack:[0, 1],Main.java:83
[2018-06-20 02:57:27]tempStack:[0, 1, -],Main.java:114
0-1=-1

[SUCCESS]:0-1
[Ljava.lang.String;@7852e922
question from commandline:0*0
[2018-06-20 02:57:27][j:0,k:0,char:*]tempStack:[0],Main.java:88
[2018-06-20 02:57:27][j:0,k:0,char:*]operatorStack:[*],Main.java:92
[2018-06-20 02:57:27][j:1,k:2,char:0]tempStack:[0, 0],Main.java:83
[2018-06-20 02:57:27]tempStack:[0, 0, *],Main.java:114
0*0=0

[SUCCESS]:0*0
[Ljava.lang.String;@7852e922
question from commandline:1/0
[2018-06-20 02:57:28][j:0,k:0,char:/]tempStack:[1],Main.java:88
[2018-06-20 02:57:28][j:0,k:0,char:/]operatorStack:[/],Main.java:92
[2018-06-20 02:57:28][j:1,k:2,char:0]tempStack:[1, 0],Main.java:83
[2018-06-20 02:57:28]tempStack:[1, 0, /],Main.java:114
ERROR:1/ 0 is not allowed.

[SUCCESS]:1/0
[Ljava.lang.String;@7852e922
question from commandline:100+20-3/10
[2018-06-20 02:57:28][j:2,k:0,char:+]tempStack:[100],Main.java:88
[2018-06-20 02:57:28][j:2,k:0,char:+]operatorStack:[+],Main.java:92
[2018-06-20 02:57:28][j:5,k:4,char:-]tempStack:[100, 20],Main.java:88
[2018-06-20 02:57:28][j:5,k:4,char:-]tempStack:[100, 20, +],Main.java:101
[2018-06-20 02:57:28][j:5,k:4,char:-]operatorStack:[-],Main.java:104
[2018-06-20 02:57:28][j:7,k:7,char:/]tempStack:[100, 20, +, 3],Main.java:88
[2018-06-20 02:57:28][j:7,k:7,char:/]operatorStack:[-, /],Main.java:98
[2018-06-20 02:57:28][j:9,k:9,char:0]tempStack:[100, 20, +, 3, 10],Main.java:83
[2018-06-20 02:57:28]tempStack:[100, 20, +, 3, 10, /],Main.java:114
[2018-06-20 02:57:28]tempStack:[100, 20, +, 3, 10, /, -],Main.java:114
100+20-3/10=120

[SUCCESS]:100+20-3/10
[Ljava.lang.String;@7852e922
question from commandline:100+20-30/10
[2018-06-20 02:57:28][j:2,k:0,char:+]tempStack:[100],Main.java:88
[2018-06-20 02:57:28][j:2,k:0,char:+]operatorStack:[+],Main.java:92
[2018-06-20 02:57:28][j:5,k:4,char:-]tempStack:[100, 20],Main.java:88
[2018-06-20 02:57:28][j:5,k:4,char:-]tempStack:[100, 20, +],Main.java:101
[2018-06-20 02:57:28][j:5,k:4,char:-]operatorStack:[-],Main.java:104
[2018-06-20 02:57:28][j:8,k:7,char:/]tempStack:[100, 20, +, 30],Main.java:88
[2018-06-20 02:57:28][j:8,k:7,char:/]operatorStack:[-, /],Main.java:98
[2018-06-20 02:57:28][j:10,k:10,char:0]tempStack:[100, 20, +, 30, 10],Main.java:83
[2018-06-20 02:57:28]tempStack:[100, 20, +, 30, 10, /],Main.java:114
[2018-06-20 02:57:28]tempStack:[100, 20, +, 30, 10, /, -],Main.java:114
100+20-30/10=117

[SUCCESS]:100+20-30/10
----------
TEST END, 6 success, 0 failed.
```

到这里，你可以任意改进你的测试程序。例如，从配置文件里读取你的测试用例等等。

适用性
------

通过为程序核心功能编写命令行接口，然后针对命令行接口进行测试，可以在单元测试/模块测试的交集地带获得良好的回报。
其中一个好处在于，针对命令行程序的测试具有很好的适用性（或者取一个更好的描述）。无论你的程序是单进程程序、多进程程序；
是客户端软件还是服务端软件，都可以通过提供命令行接口进行测试。并且具有良好的跨语言能力，你不必拘泥于语言限定的测试框架，
在实际开发中，整个系统的组建很多时候是混合式的，使用了多种语言，跨越了不同设备。而命令行的方式能更普遍的适用于
各种情况。

测试狗
------
命令行程序可以轻易的用后台服务或者定时程序定时、自动化的跑测试用例，而这是持续集成的基本要素：在程序运行期间、程序新发布版本、...的时候，都有自动运行的测试。

eat your dog food, always run.


提交修改
------
查看代码变动：`git status`

```
On branch java
Your branch is up to date with 'origin/java'.

Changes not staged for commit:
  (use "git add <file>..." to update what will be committed)
  (use "git checkout -- <file>..." to discard changes in working directory)

	modified:   src/Main.java

Untracked files:
  (use "git add <file>..." to include in what will be committed)

	src/Test.java

no changes added to commit (use "git add" and/or "git commit -a")
```

添加：`git add .`+`git commit -m 'add log and test'`

提交：`git push`

git url: https://github.com/fanfeilong/Calculator/commit/5901c0a8d43cecaf9a2a5cb53f17ccc3b6fb7653


参考
---
- [1] https://testing.googleblog.com
- [2] https://news.cnblogs.com/n/598772/
- [3] https://blog.codingnow.com/2018/05/ineffective_debugger.html
- [4] http://lamport.azurewebsites.net/video/intro.html

