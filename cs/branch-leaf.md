<hr/>
**控制结构目录**：[1](http://www.cnblogs.com/math/p/control-structure-001.html) [2](http://www.cnblogs.com/math/p/control-structure-002.html) [3](http://www.cnblogs.com/math/p/control-structure-003.html) [4](http://www.cnblogs.com/math/p/control-structure-004.html)
<hr/>

## 典型代码：

```javascript
function doSomething1(){
  // ...
}
function doSomething2(){
  // ...
}
function doSomething3(){
  // ...
}
function doSomething(){
  doSomething1();
  doSomething2();

  // some dirty codes
  for(let i in works){
    let work = works[i];
    work();
  }

  doSomething4();
}
```

## 结构分析

此处的问题，// some dirty codes 后面的一段代码逻辑和doSomethins 系列位于同一个抽象层上，但是写代码的人没有意识到这点，于是把细节代码在此处直接展开。改写的方式是：

```javascript
function doSomething1(){
  // ...
}
function doSomething2(){
  // ...
}
function doSomething3(){
  // ...
}
function doSomething4(){
  // some dirty codes
  for(let i in works){
    let work = works[i];
    work();
  }
}
function doSomething(){
  doSomething1();
  doSomething2();
  doSomething3();
  doSomething4();
}
```

## 语义分析

这里的核心是，把函数分层，A->B->C->D，同一层函数只做这层的一件事，上一层的函数则做对下一层函数的组织，每一层为了组织下一层，都会用到顺序、分支、循环三种控制结构。

#### `sequence`形式的层组织结构
```javascript
function doSomething(){
  doSomething1();
  doSomething2();
  doSomething3();
  doSomething4();
}
```

#### `if/else`形式的层组织结构
```javascript
function doSomething(){
  if(condition1){
    doSomething1();
  }else{
    doSomething2();
  }
}
```

#### `for`形式的层组织结构
```javascript
function doSomething(){
  doSomething1();
  doSomething2();
  for(let i in somethings){
     let something = somethings[i];
     something();
  }
  doSomething4();
}
```

你可以尝试把上面三种结构里，把某个doSomethingi();的代码就地展开，你会发现代码的组织变的不清晰，理解的时候就会累（阅读的时候，你会倾向于认为一行代码和一行代码之间是等价粒度的，但是显然，展开某个doSomethingi()后，doSomethingi()的细节代码的任意一行和doSomethingj()之间的粒度不同，如果你随机展开其中部分doSomethingi()，代码会变的更糟糕，看上去「繁杂」，其实这是不必要的。

在A->B->C->D这样的结构里，任意两层可以看作是`分枝/叶子`双层结构。

例如A->B，那么，如果你把应该在B层的逻辑代码写到A层的函数里，A层的一个函数里就会同时存在下面两种类型代码的混合：
1.  对B层函数的调用代码
2.  一段本来应该在B层做的逻辑代码的直接展开

这样的代码能运行，但是可读性差，不利于阅读和维护，因为每次通过阅读把一个函数里的上述两种逻辑代码“理解”出来，这增加了成本。而如果分层良好，阅读的时候就会流畅，理解清晰，也更不容易出BUG，你只需要关注：
1. A／B层分别在自己那层只做一件事。
2. 当前层的`if/else/for`基本控制结构是否合理。
3. 如果A层函数里需要知道某个B层的细节，去B层函数查找即可。
4. 不追求一层函数内部的代码量少，而是层次清晰，只做这层的一件事这个语义的实施。