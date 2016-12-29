#### C的数组下标是指针偏移的缩写
[With C arrays, why is it the case that a[5] == 5[a] ?](http://stackoverflow.com/questions/381542/with-c-arrays-why-is-it-the-case-that-a5-5a)
The C standard defines the [] operator as follows:
`a[b] == *(a + b)`
Therefore a[5] will evaluate to:
`*(a + 5)`
and 5[a] will evaluate to
`*(5 + a)`
and from elementary school math we know those are equal.

This is the direct artifact of arrays behaving as pointers, "a" is a memory address. "a[5]" is the value that's 5 elements further from "a". The address of this element is "a + 5". This is equal to offset "a" from "5" elements at the beginning of the address space (5 + a).

**评价**：
指针计算是最基本的计算，其他的各种操作都是建立在指针计算的基础上重新定义的计算，这样就有了可推导的性质。

#### 位操作
[How do you set, clear and toggle a single bit in C/C++?](http://stackoverflow.com/questions/47981/how-do-you-set-clear-and-toggle-a-single-bit-in-c-c)
```
& 
0 0 0
1 0 0
0 1 0
1 1 1
```

```
|
0 0 0
0 1 1
1 0 1
1 1 1
```

```
^
0 0 0
0 1 1
1 0 1
1 1 0
```

=>

Setting a bit:
`number|=(1<<n)`

Clearing a bit:
`number&=~(1<<n)`

Toggling a bit:
`number^=1<<n`

Checking a bit:
`bit=number&(1<<n)`

**评价:**
高级编程语言完全可以把对位的操作封装成普通函数操作，比如
```
bit.set(number,i)
bit.clear(number,i)
bit.toggle(number,i)
bit.get(number,i)
```
然后通过编译器或者解释器的优化来达到同样的性能。这样就可以去掉这样的语法噪音
发明`&`,`|`,`^`完全是增加语法噪音，估计是被数学的`符号发明症`所影响，问题是数学的符号一般
都只是用来做逻辑上的推理和解释，并不需要被天天写和运行。

- [Low Level Bit Hacks You Absolutely Must Know](http://www.catonmat.net/blog/low-level-bit-hacks-you-absolutely-must-know/)
- [Bit Twiddling Hacks](http://graphics.stanford.edu/~seander/bithacks.html)

