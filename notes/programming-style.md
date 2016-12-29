#### 常量以k开头命名的原因
>"c" was the tag for type "char", so it couldn't also be used for "const"; so "k" was chosen, since that's the first letter of "konstant" in German, and is widely used for constants in mathematics.

>http://stackoverflow.com/questions/5016622/where-does-the-k-prefix-for-constants-come-from

>It's a historical oddity, still common practice among teams who like to blindly apply coding standards that they don't understand.

>Long ago, most commercial programming languages were weakly typed; automatic type checking, which we take for granted now, was still mostly an academic topic. This meant that is was easy to write code with category errors; it would compile and run, but go wrong in ways that were hard to diagnose. To reduce these errors, a chap called Simonyi suggested that you begin each variable name with a tag to indicate its (conceptual) type, making it easier to spot when they were misused. Since he was Hungarian, the practise became known as "Hungarian notation".

>Some time later, as typed languages (particularly C) became more popular, some idiots heard that this was a good idea, but didn't understand its purpose. They proposed adding redundant tags to each variable, to indicate its declared type. The only use for them is to make it easier to check the type of a variable; unless someone has changed the type and forgotten to update the type, in which case they are actively harmful.

>The second (useless) form was easier to describe and enforce, so it was blindly adopted by many, many teams; decades later, you still see it used, and even advocated, from time to time.

>"c" was the tag for type "char", so it couldn't also be used for "const"; so "k" was chosen, since that's the first letter of "konstant" in German, and is widely used for constants in mathematics.

#### 匈牙利命名法
- [匈牙利命名法wik](http://zh.wikipedia.org/zh/%E5%8C%88%E7%89%99%E5%88%A9%E5%91%BD%E5%90%8D%E6%B3%95)
- [应用型匈牙利命名法 vs 系统型匈牙利命名法](http://www.cnblogs.com/xuxn/archive/2012/05/16/real-hungarian-notation.html)

**评价:**
- 匈牙利命名法这种编码方式并不见得就带来更好的阅读信息。以Google的Chromium为例，其命名风格就完全抛弃了匈牙利命名法。至今还在用匈牙利命名法风格的，都是品味和惯性问题。实际上很多工程上的老古董都是不思上进的结果，那些被这些老古董虐待过的，并以此为荣，觉得懂这些老古董才是高级货的，都是没见过世面的，以为非此路不可。可惜了，思维的局限限制了进步。

#### 多个提前返回时的资源释放
```
xxx x = new xxx();
bool quit=false;
do{
  if(...){
    quit=true;
    break;  
  }

  if(...){
    quit=true;
    break;  
  }
}while(false);
if(quit){
  delete x;
  return;
}
//other code
```

#### 转轴
很多时候，一个代码有好多个分支，写代码的时候需要使用`if else if esle if else`之类的繁琐的分支语句，我们可以通过`Enum+Translate+SwitchCase`的方式让代码更扁平化点，我称之为`转轴`。
```
typedef enum tagXXXState{
  XXXState_1,
  XXXState_2
}XXXState;
XXXState TranslateState(condition){
  if(condition){
    return XXXState_1;
  }else{
    return XXXState_2;
  }
}
void Func(condition){
  switch(TranslateState(condition)){
    case XXXState_1:break;
    case XXXState_2:break;
    default:break;
  }
}
```
此处实例的分支只有2个，也没有嵌套，自然是看上去多余的，不过如果分支很多，又有很多个嵌套，则使用这种`转轴`代码会把嵌套的if else转换成扁平的switch-case代码。

#### 明确加锁范围
加锁的范围要明确，利用C++的RAII，使用自动锁，同时最小化局部范围
```
{ 
  AutoThreadLock lock(m_lock); 
  //Data
}
```

#### 状态机的一种风格
- define states
- function `dowork` by switch states
- in branch methods or events
    - check state
    - do work
    - change state
    - reentry `dowork` function


