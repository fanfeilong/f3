
最近阅读了酷壳上的一篇深度好文：[LINUX PID 1 和 SYSTEMD](http://coolshell.cn/articles/17998.html)。这篇文章介绍了systemd干掉sysvinit和UpStart的故事，作者在自己对三者深入理解的基础上对systemd所解决的问题做了精彩的解说。我也觉的systemd干的漂亮，把系统初始化过程中能并发的过程尽一切可能并发了，而且还能保持构架上的优雅，可见往往复杂事物的最佳解法本身也会是一个漂亮的结构。我也去阅读了下systemd作者对各种质疑的答疑文章：[The Biggest Myths](http://0pointer.de/blog/projects/the-biggest-myths.html)，从文章中的描述来看，systemd非但没有违背Unix的管道（pipeline)思想，如果说一个组件是一个齿轮，那么它把那么多的齿轮完美的拼接在一起，不正是管道思想的最佳体现么？

>In Unix-like computer operating systems, a pipeline is a sequence of processes chained together by their standard streams, so that the output of each process (stdout) feeds directly as input (stdin) to the next one.

现在的工作中，在工程实践上，我会比较注意去思考那些看上去随意的工作怎样用经过设计的内涵解决方案的脚步系统／命令行工具统一解决，使得一个团队工作中日常运行每天都要流转的那些部分被经过设计的机器逻辑替代，从而规避低水平人工操作，消除低水平人工操作，通过解决问题进而提高效率。当然，不是所有人都看得到这些部分，有时候，你需要把东西做出来，切实改变了工程师团队的痛点和效率，工程师们才会体会到经过设计的工具所带来的好处。积累多了之后，以后可以慢慢再展现这些过程。

日常工作代码里，实际上也有许多在函数级别可以提现的管道思想，例如下面这段改造自实际代码的示例（原始代码在需求变更，BUG解决中琐碎地多，也跟我挺长时间没写非C++代码，导致有些地方重复出现以前犯过的低阶错误有关）：

```
function onResponse(err, obj){
	if(this.bindable()){
		if(err){
			this.unCommitBindInfo(obj.bindInfo);
			this.unCommitInfo(obj.info);
			doComplete(err);
			return;
		}

		this.commitBindInfo(obj.bindInfo,(err)=>{
			if(err){
				this.unCommitBindInfo(obj.bindInfo);
				doComplete(err);
				return;
			}

			this.commitInfo(obj.info, (err)=>{
				if(err){
					this.unCommitInfo(obj.info);
				}
				doComplete(err);
			});
		});
	}else{
		if(err){
			this.unCommitInfo(obj.info);
			doComplete(err);
			return;
		}
		this.commitInfo(obj.info, (err)=>{
			if(err){
				this.unCommitInfo(obj.info);
			}
			doComplete(err);
		});
	}
}

function doComplete(err, obj){
	if(err){
		this.m_complete({result:err});
	}else{
		this.m_complete({result:RESULT.SUCCESS, obj:obj})
	}
}
```

在切换到动态语言的过程中，我发现一个意思的事情。就是很长一段时间写C++代码的程序员，在切到动态语言的时候，会经历一个返祖现象。以前在静态语言里锻炼的模块化／代码洁癖会在一段时间内因为动态语言带来的便利而出现暂时性丢失，例如：不用class，随手写出一个充满全局函数的能工作的“脚本”；随处使用魔数，拼接路径而不是集中配置管理...；当然，毕竟不是新手，在大家发现问题的时候，及时同步了下共通的问题后，大部分人还是能及时纠正过来。

上述代码是一种常见的逻辑分枝：在一段处理逻辑里有两种可能的大分枝，例如此处是被`if(this.bindable())`所分开的，第一个分支里会处理obj.bindinfo相关的逻辑后再处理obj.info相关的逻辑；第二个分支里则只处理obj.info相关的逻辑，那么其实两个分支处理obj.info部分的逻辑是重复的。一不小心就会直接用if-else把两个分支分开写，然后随着需求变更，两个分支里的碎片代码会变的繁杂，一段时间之后自己会觉得不可维护。

想清楚这点后，我们可以改变控制结构，使用管道（pipeline）的思想来改进。那就是，在抽象概念上，让这个流程变成一定先处理obj.bindinfo，再处理obj.info的流式处理。如果有更多的流程，可以继续串下去，这样它们就构成一个管道。

改进后的代码如下：


```
function onResponse(err, obj){
	this.processBindInfo(err,obj,(err)=>{
		this.processInfo(err,obj,(err)=>{
			this.doComplete(err);
		});
	})
}

function processBindInfo(err,obj,callback){
	this.tryCommitBindInfo(err,obj,(err)=>{
		if(err){
			this.unCommitBindInfo(obj.bindInof);
		}
		callback(err);
	})
}

function processInfo(err,obj,callback){
	this.tryCommitInfo(err,obj,(err)=>{
		if(err){
			this.unCommitInfo(obj.info);
		}
		callback(err);
	});
}

function tryCommitBindInfo(err, obj, callback){
	if(!this.bindable()){ // 1. 消灭在这里
		callback(err);
		return;
	}

	if(err){              // 2. 错误处理
		callback(err);
		return;
	}

	this.commitBindInfo(obj.bindInfo,(err)=>{
		callback(err);
	});
}

function tryCommitInfo(err, obj, callback){
	if(err){              // 2‘. 错误处理
		callback(err);
		return;
	}
	
	this.commitInfo(obj.info,(err)=>{
		callback(err);
	});
}

function doComplete(err, obj){
	if(err){
		this.m_complete({result:err});
	}else{
		this.m_complete({result:RESULT.SUCCESS, obj:obj})
	}
}
```

上述代码里，`processBindInfo`和`processInfo`两个处理分支分别只处理自己哪个分支的事情，如果不需要处理，则让数据流（err）通过callback`透传`（pass-through）下去即可。可以看到在`tryCommitBindInfo`的`1`这个地方，就把本来做大分枝用的bindable消灭掉了。而在`tryCommitBindInfo`和`tryCommitInfo`两个分支里的`2`位置以及`2‘`位置，则分别是两个处理流程里对错误的处理。

有时候，我觉的很多重构过程似成相识，应该是以前也干过同样的事情。所以，我想记录下来是比较重要的，记录下来常常review。




