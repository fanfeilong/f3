[完美散列函数]( https://zh.wikipedia.org/wiki/%E5%AE%8C%E7%BE%8E%E6%95%A3%E5%88%97 )

假设，写一个SQL语句解析器，词法分析对SQL语句解析，把语句分成了多个token，一般这个时候会需要查询这个token是否是一个关键字token。

例如keywords表和tokens表分别如下：
```
keywords = ["AS", "FROM", "INSERT", "SELECT", "WHERE"];
tokens = [As, From, Insert, Select, Where];

```

查询代码：
```
 let token_raw = "FROM";
 let index = keywords.binary_search(token_raw);
 let token = tokens[index];
```

这个地方查询index的时候，keywords是一个有序数组，做了二分查询，算法复杂度是O(log(N))，由于SQL语句里有大量的关键字，解析的时候会有大量这样的查询，显然这是一个可以优化的点，一种简单的做法是把keywords做成哈希表，这可以让查询速度接近O(1)。

但只是接近O(1)还是不够的，由于关键字是已知的，是固定长度的，有一种叫做“完美哈希函数”的算法，可以对固定长度的集合S，生成一个专用的哈希函数，这个哈希函数可以把S映射到一个对应长度的整数集合I，这个哈希映射的性质是：没有碰撞！使用完美哈希，可以使得上述查询速度完美为O(1)。

PGSQL和SQLite的实现里分别有采用这个策略。

[1] https://zh.wikipedia.org/wiki/%E5%AE%8C%E7%BE%8E%E6%95%A3%E5%88%97
[2] http://ilan.schnell-web.net/prog/perfect-hash/
[3] https://www.postgresql.org/message-id/flat/E1ghOVt-0007os-2V%40gemulon.postgresql.org
[4] https://sqlite.org/src/artifact/1f7f2ac1d9f262c0
[5] https://news.ycombinator.com/item?id=18879185 

