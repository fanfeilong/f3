<hr/>
**控制结构目录**：[1](http://www.cnblogs.com/math/p/control-structure-001.html) [2](http://www.cnblogs.com/math/p/control-structure-002.html) [3](http://www.cnblogs.com/math/p/control-structure-003.html) [4](http://www.cnblogs.com/math/p/control-structure-004.html) [5](http://www.cnblogs.com/math/p/control-structure-005.html) [6](http://www.cnblogs.com/math/p/control-structure-006.html)
<hr/>

基于语言提供的基本控制结构，更好地组织和表达程序，需要良好的控制结构。

>There are only two hard things in computer science: cache invalidation and naming things.
--Phil Karlton

## 前情回顾

上一次，我们写了资源打开关闭自动化的控制结构。有熟悉ObjectC语言的朋友补充了一个苹果的做法: [autorelease pool](https://developer.apple.com/library/content/documentation/Cocoa/Conceptual/MemoryMgmt/Articles/mmAutoreleasePools.html)

比如说Lock，可以这么声明：

```Objective-C
@autoreleasepool{
	id lock = [Lock lock]; // autorelease 对象, 创建后 lock, dealloc 时释放
}
```

它的主要作用是，延迟对象的释放。对一个对象调用 autorelease 方法，那么它就被保存到最里层的 autorelease pool。当 autorelease pool 块结束的时候，对里面所有的对象发送一次 release。Objective-C 里有大量的对象都是通过 autorelease pool 管理。

也可以通过autoreleasepool做性能优化：

```Objective-C
for(int i=0; i<100; i++){
	create_lots_autoreleased_objects();
}
```

这段代码可以被优化成：

```Objective-C
for(int i=0; i<100; i++){
	@autoreleasepool{
		create_lots_autoreleased_objects();
	}	
}
```

这个相当于是加速了内存的释放流程，降低了峰值内存占用。那么，其他语言有这种便利么？

## 典型代码

```javascript
function send(pkg, onComplete){
	let connection = new Connection(name);
	connection.open(function(err){
		connection.send(pkg, function(err){
			connection.close();
			onComplete();
		});
	})	
}
```

## 结构分析

又是open，close。当然可以使用前一节讲到的方法。但这不是本节的主题，前一节实际上做了一个潜在假设，就是存在一个连接池子，下面的open和close里面获取资源和释放资源都是从资源池里取出和放回的:

```javascript
open(onComplete) {
    let self = this;
    ／**get from pool*／
    self.m_database.getPOOL().getConnection(function(err, conn) {
        if (err) {
            return onComplete(err);
        }
        self.m_conn = conn;
        onComplete(0);
    });
}

close() {
    let self = this;
    ／**release to pool*／
    self.m_conn.release();
}
```

如果你的资源不是个数据库，例如一个TCP连接，那么open和close是实实在在的回打开和关闭连接，频繁操作就有性能问题。所以我们需要自己定制一个连接池。例如下面的实现：

```javascript
class ConnectionPool{
	constructor(capacity,ttl){
		this.m_pool = {};
		this.m_capacity = capacity;
		this.m_ttl = ttl;
	}	
	
	init(){
		let self = this;

		base.setTimer(function(){
			self.trimeByTimeout();
		}, 30*1000);
	}

	trimeByTimeout(){
		/**剔除超时的连接*/
		for(let name in self.m_pool){
			let item = self.m_pool[name];
			let elapse = base.getNow()-item.updateTime;
			if(elapse>self.ttl){
				self.remove(item);
			}
		}
	}

	getConnection(name,onComplete){
		let self = this;
		let item = self.peek();
		self.open(item,(err){
			onComplete(err, item.conn);
		});
	}

	peek(name){
		let self = this;

		let cache = self.getCache(name);
		if(cache){
			return cache;
		}

		self.trimeByLRU();

		let conn = self.create(name);
		let item = self.insert(name, conn);

		return item;
	}

	getCache(name){
		/**获取缓存，也可以加入使用频次的统计，根据使用频次来选取*/
		let cache = self.m_pool[name];
		if(cache){
			cache.updateTime = base.getNow();
			return cache;
		}else{
			return null;
		}
	}

	trimeByLRU(){
		let self = this;

		/**剔除最不活跃的连接*/
		if(self.overflow()){
			let minItem = self.findMinItem();
			if(minItem!=null){
				self.remove(item);
			}
		}
	}

	overflow(){
		let length = Object.keys(self.m_pool).length;
		return length > self.m_capacity;
	}

	findMinItem(){
		let min = Infinity;
		let minItem = null;
		for(let name in slef.m_pool){
			let item = self.m_pool[name];
			if(item.updateTime<min){
				min = item.upddateTime;
				minItem = item;
			}
		}
		return minItem;
	}

	create(name){
		let conn = new Connection(name);
		return conn;
	}

	open(item, onComplete){
		let self = this;
		if(item.refCount===0){
			item.conn.open(onComplete);
		}else{
			item.refCount++;
			onComplete(0);
		}
	}

	release(item){
		let self = this;
		item.refCount--;
		if(item.refCount===0){
			if(item.markRemove){
				delete self.m_pool[item.name];
				item.conn.close();
			}
		}else{
			// ignore
		}
	}

	insert(name, conn){
		let self = this;
		let item = {
			updateTime: base.getNow(),
			conn: conn,
			refCount: 0,
			markRemove: false,
			name: name,
		}

		/**给conn添加release方法*/
		item.conn.release = function(){
			self.release(item);
		}

		self.m_pool[name] = item;
		return item;
	}

	remove(item){
		let self = this;
		if(item.refCount>0){
			item.markRemove = true;
		}else{
			delete self.m_pool[item.name];
			item.conn.close();
		}
	}
}
```

有了连接池，就可以像常规一样使用Connection：

```javascript
function test(){
	let pool = new ConnectionPool(32,30*1000);
	pool.getConnection(function(err, conn)=>{
		conn.connect(function(err){
			//...
			conn.release();
		})
	});
}
```

## 语义分析

Least Recently Used, 简称[LRU](https://en.wikipedia.org/wiki/Cache_replacement_policies#LRU)，是一种常用的缓存思路。基本思想是从资源池里剔除`最近最少使用`的资源条目。

编程中经常有优化的需求，例如缓存。但是很容易出现所谓的`碎片优化`，例如在需要使用资源的地方，每个地方都去手工持有资源的缓存，就不是一种好的方法，如果有多个协作者，还会导致每个人都自己做了一份缓存，或者在源代码的多个地方对同一类资源重复做缓存。

正确的做法应该是：
1. 使用资源的地方保持资源使用的最简单形式：打开，使用，关闭（当然你可以使用自动化手法）
2. 提供统一并且单点的资源池实现，在一个地方把资源的缓存做好。

















