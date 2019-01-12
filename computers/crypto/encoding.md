#### 字符编码
-------------
* ASCII
* EASCII
* ISO 8859
  * ISO 8859-n(n=1,2,3,...,11,13,...,16)
  * Latin-1==ISO8859-1
* GB2312
* GBK > GB2312
* BIG5 (small conflict with GB2312)
* GB18030 > GB2312
* Unicode (NOT Compatible with GBXXX)
  * UCS2 
  * UTF-16 (Extend UCS2)
  * UCS4 
  * UTF-32 (Currently a subset of UCS4, But ability to encode more unicode characters)
  * UTF-8
    * BOM
    * No BOM
    

#### 反码
---------

* [1-反码(Ones's_complement)](http://en.wikipedia.org/wiki/Ones'_complement)
* [2-反码(Two's_complement)](http://en.wikipedia.org/wiki/Two%27s_complement)

本来用1反码表示负数就够了，但是有缺陷
* 0必须有-0表示
  * 这是因为在1-反码的情况下，为了在1-反码的二进制加减法规则下让N+(-N)=N-N，才需要定义-0 
* 二进制加减法会有溢出，需要对溢出的字节做环回处理
* 表示范围只能是[-2^(n-1)-1,2^(n-1)]

使用2-反码（等于1-反码+1）表示负数则
* 0只需要唯一表示
* 二进制加减法不需要对溢出的字节做处理
* 表示范围是[-2^(n-1),2^(n-1)]

比如IPV4的Checksum：

> The checksum field is the 16-bit one's complement of the one's complement sum of all 16-bit words in the header.

实际计算算法：
  1. 把头部的每16个位直接按32bit数加起来得到一个32位数
  2. 把得到的32位数拆成高16位和低16位，把高16位加到低16位
  3. 取反

> The checksum field is the 16-bit one's complement of the one's complement sum of all 16-bit words in the header.

这句话的`sum of all 16-bit words`代表的是计算步骤的1-2步
这句话的`the 16-bit one's complement of the one's complement..`代表的是计算步骤的第3步

#### 喷泉码
- [C#实现](http://www.codeproject.com/Articles/425456/Your-Digital-Fountain)

#### Church Encode
```
lambda.x.f(x) 
lambda.x.
	lambda.y.f(y)
...
```

由于lambda即可被嵌套repeat，又可以被求值，所以可以用来信息的编码和解码：

- [Church_encoding](https://en.wikipedia.org/wiki/Church_encoding)
- [邱奇数](https://zh.wikipedia.org/wiki/%E9%82%B1%E5%A5%87%E6%95%B0)

如果只能编码没什么，任何可以表示为0和1，并且可以repeat的都能编码，Church Code的特点是解码行为本身也可以用自己编码。

#### string
- [netstring](http://cr.yp.to/proto/netstrings.txt)

##### 参考资料
--------------
* [字符编码常识及常见问题解析 ](http://mp.weixin.qq.com/s?__biz=MzA5MTY2NTcwNw==&mid=201226425&idx=1&sn=5a9846e6cc18012ef5b1f5216c2addbd#rd)
* [阮一峰:字符编码笔记：ASCII，Unicode和UTF-8](http://www.ruanyifeng.com/blog/2007/10/ascii_unicode_and_utf-8.html)
* [wiki:UTF16](http://en.wikipedia.org/wiki/UTF16)
* [wiki:UTF32](http://en.wikipedia.org/wiki/UTF32)
* [wiki:UTF8](http://en.wikipedia.org/wiki/UTF8)
* [Information Geometry](http://math.ucr.edu/home/baez/information/)

#### 趣味编码
- [单词拼写都是错的也能读](http://geon.github.io/programming/2016/03/03/dsxyliea)
- [眼球Parser](http://www.kuaidula.com/)
- [0.30000000000000004](http://0.30000000000000004.com)


http://davidlowryduda.com/a-brief-notebook-on-cryptography/
