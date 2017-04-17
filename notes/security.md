## 单向哈希函数
- [wiki:MD5](http://en.wikipedia.org/wiki/MD5)
  - 1992年，MIT的`Ronald Rivest`教授（RSA中的R就是他，图灵奖获得者，算法导论作者之一）提出MD5算法用以替代MD4
  - 1993年，`Den Boer`和`Bosselaers`发现MD5`伪碰撞`：两个不同的初始化向量可以产生相同的MD5摘要
  - 1996年，`Dobbertin`发现了一个MD5的碰撞
  - 2004年，三月份，`MD5CRK`项目用以演示针对MD5算法的`生日攻击（Birthday Attach）`
  - 2004年，八月份，`王小云（Xiaoyun Wang）`教授的团队对MD4、MD5、HAVAL-128和RIPEMD等四个著名算法实现了加速后的杂凑碰撞
  - 2005年，三月份，`Arjen Lenstra`, `Xiaoyun Wang`, 和 `Benne de Weger` 演示了两个不同公钥的X.509证书的MD5哈希值相同
  - 2005年，不久以后，` Vlastimil Klima`改进了算法，可以在单台笔记本电脑上用几个小时构造MD5碰撞。
  - 2006年，三月份，`Klima`公布了一个隧道（tunneling）算法，可以在一台笔记本上在一分钟内找到MD5碰撞。 
  - 2010年，十二月，`谢涛（Tao Xie）`和`冯登国（Denguo Feng）`宣布第一个单块（512位）MD5碰撞，
    他们向社区发出挑战，奖励在2013年1月之前找到不同的64字节碰撞算法的人。
  - 2012年，`Marc Stevens`响应该挑战，公布了单块（512位）消息碰撞，以及相关的碰撞算法和源码。

- [RFC:MD5](http://tools.ietf.org/html/rfc1321)
  - 假设数据序列是[m_0,m_1,...,m_{b-1}]
  - 将数据序列对齐，先填一个1，其余填0，直到序列元素总个数对512做模运算的余数是448
  - 将序列原始长度b表示成64位，如果b的二进制表示超过64位则取低64位，将这64位加到序列尾部
  - 则该序列总长度恰好是512的整数倍，而每512位都可再分为16个字节，也就是16个32位数。`M[0 ... N-1]`
  - 初始化Message Digital的初始值：[A，B，C，D]，其中A、B、C、D分别是32位寄存器
  ```
	A: 01 23 45 67
	B: 89 ab cd ef
	C: fe dc ba 98
	D: 76 54 32 10
  ```
   - 定义四个三参数辅助函数，参数都是32位数
  ```
	F(X,Y,Z) = XY v not(X) Z
	G(X,Y,Z) = XZ v Y not(Z)
	H(X,Y,Z) = X xor Y xor Z
	I(X,Y,Z) = Y xor (X v not(Z))
  ``` 
  - 定义T[0,...64];T[i]=4294967296*abs(sin(i)的整数部分.
  - 对每16个分组:M[i],M[i+1]...M[i+15],定义：
  ```
	FF(a ,b ,c ,d ,Mj ,s ,ti ) 操作为 a = b + ( (a + F(b,c,d) + Mj + ti) << s)
	GG(a ,b ,c ,d ,Mj ,s ,ti ) 操作为 a = b + ( (a + G(b,c,d) + Mj + ti) << s)
	HH(a ,b ,c ,d ,Mj ,s ,ti) 操作为 a = b + ( (a + H(b,c,d) + Mj + ti) << s)
	II(a ,b ,c ,d ,Mj ,s ,ti) 操作为 a = b + ( (a + I(b,c,d) + Mj + ti) << s)
	注意：“<<”表示循环左移位，不是左移位。
  ```
  - 对每16个分组，执行下列程序
  ```
  /* Process each 16-word block. */
   For i = 0 to N/16-1 do

     /* Copy block i into X. */
     For j = 0 to 15 do
       Set X[j] to M[i*16+j].
     end /* of loop on j */

     /* Save A as AA, B as BB, C as CC, and D as DD. */
     AA = A
     BB = B
     CC = C
     DD = D

     /* Round 1. */
     /* Let [abcd k s i] denote the operation
          a = b + ((a + F(b,c,d) + X[k] + T[i]) <<< s). */
     /* Do the following 16 operations. */
     [ABCD  0  7  1]  [DABC  1 12  2]  [CDAB  2 17  3]  [BCDA  3 22  4]
     [ABCD  4  7  5]  [DABC  5 12  6]  [CDAB  6 17  7]  [BCDA  7 22  8]
     [ABCD  8  7  9]  [DABC  9 12 10]  [CDAB 10 17 11]  [BCDA 11 22 12]
     [ABCD 12  7 13]  [DABC 13 12 14]  [CDAB 14 17 15]  [BCDA 15 22 16]

     /* Round 2. */
     /* Let [abcd k s i] denote the operation
          a = b + ((a + G(b,c,d) + X[k] + T[i]) <<< s). */
     /* Do the following 16 operations. */
     [ABCD  1  5 17]  [DABC  6  9 18]  [CDAB 11 14 19]  [BCDA  0 20 20]
     [ABCD  5  5 21]  [DABC 10  9 22]  [CDAB 15 14 23]  [BCDA  4 20 24]
     [ABCD  9  5 25]  [DABC 14  9 26]  [CDAB  3 14 27]  [BCDA  8 20 28]
     [ABCD 13  5 29]  [DABC  2  9 30]  [CDAB  7 14 31]  [BCDA 12 20 32]

     /* Round 3. */
     /* Let [abcd k s t] denote the operation
          a = b + ((a + H(b,c,d) + X[k] + T[i]) <<< s). */
     /* Do the following 16 operations. */
     [ABCD  5  4 33]  [DABC  8 11 34]  [CDAB 11 16 35]  [BCDA 14 23 36]
     [ABCD  1  4 37]  [DABC  4 11 38]  [CDAB  7 16 39]  [BCDA 10 23 40]
     [ABCD 13  4 41]  [DABC  0 11 42]  [CDAB  3 16 43]  [BCDA  6 23 44]
     [ABCD  9  4 45]  [DABC 12 11 46]  [CDAB 15 16 47]  [BCDA  2 23 48]

     /* Round 4. */
     /* Let [abcd k s t] denote the operation
          a = b + ((a + I(b,c,d) + X[k] + T[i]) <<< s). */
     /* Do the following 16 operations. */
     [ABCD  0  6 49]  [DABC  7 10 50]  [CDAB 14 15 51]  [BCDA  5 21 52]
     [ABCD 12  6 53]  [DABC  3 10 54]  [CDAB 10 15 55]  [BCDA  1 21 56]
     [ABCD  8  6 57]  [DABC 15 10 58]  [CDAB  6 15 59]  [BCDA 13 21 60]
     [ABCD  4  6 61]  [DABC 11 10 62]  [CDAB  2 15 63]  [BCDA  9 21 64]

     /* Then perform the following additions. (That is increment each
        of the four registers by the value it had before this block
        was started.) */
     A = A + AA
     B = B + BB
     C = C + CC
     D = D + DD

   end /* of loop on i */
  ```
  - MD5的结果就是经过上述算法算出来的最终[A B C D]
 
- [MD6](http://en.wikipedia.org/wiki/MD6)
  - 2008年，十二月，`Douglas Held`发现原始MD6哈希算法实现的缓冲区溢出的BUG。
  - 2009年，二月份，`Ron Rivest`修正了该缓冲区溢出。
  - 2009年，七月份，`Ron Rivest`给`NIST`提交了一个报告，说明MD6还不适合做为SHA-3的候选哈希算法，因为在MD6对差分攻击的防御的证明里有一个漏洞。
  - 2011年，9月份，一篇论文改进了上述证明。
  - SHA-3最后选中了Keccak算法，而非MD6
- [NIST hash function competition](http://en.wikipedia.org/wiki/NIST_hash_function_competition)
  - SHA-3竞争：51个候选算法进入第一轮评估；这当中14个晋级第二轮；第三轮候选算法只剩下5个；并从这5个中Keccak被宣布为获胜者。
    - MD6没有进入第2轮。
- [SHA-1](http://en.wikipedia.org/wiki/SHA-1)
- [SHA-2](http://en.wikipedia.org/wiki/SHA-2)
- [SHA-3](http://en.wikipedia.org/wiki/SHA-3)

## 对称密码
- DES
- AES
- Rijndael

## 中间人攻击

## 非对称密码
- 公/私钥
- RSA
    - [RSA算法原理1](http://www.ruanyifeng.com/blog/2013/06/rsa_algorithm_part_one.html)
    - [RSQ算法原理2](http://www.ruanyifeng.com/blog/2013/07/rsa_algorithm_part_two.html)

## 消息认证
- 摘要
- 哈希验证
- [Passphrase FAQ](https://www.unix-ag.uni-kl.de/~conrad/krypto/passphrase-faq.html)

## 数字签名
- 证书
    - [X509](https://www.bearssl.org/x509.html)
- PKI
- 数字签名的漏洞

## 伪随机数生成器
- 线性同余算法

## PGP
- 一个人的开发

## SSL/TLS
网络传输协议里，在可靠传输协议和应用层协议之间有单独的安全层：
>Transport Layer Security (TLS) and its predecessor, Secure Sockets Layer (SSL), both frequently referred to as "SSL", are cryptographic protocols that provide communications security over a computer network.[1] Several versions of the protocols find widespread use in applications such as web browsing, email, Internet faxing, instant messaging, and voice-over-IP (VoIP). Websites use TLS to secure all communications between their servers and web browsers.

目前的版本有，最新广泛使用的是TLS2.0：
- SSL 1.0
- SSL 2.0
- SSL 3.0
- TLS 1.0
- TLS 2.0 
- TLS 3.0

#### 复杂的握手协议

- [图解SSL/TLS协议](http://www.ruanyifeng.com/blog/2014/09/illustration-ssl.html)

## OAuthor

## OpenID