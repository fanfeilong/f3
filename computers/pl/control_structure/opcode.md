## 发现问题

在一个正式项目的开发周期中，除了源代码版本控制外，还存在着项目的配置／编译／打包／发布等各种高频但非“核心”的脚本代码。职业程序员常常在写项目正式代码的时候，有着良好的习惯，包括编码规范／模块化／...等等。然而，当场景切换到配置、编译、打包、发布等脚本代码时，往往会写出蹩脚的代码。例如：全局变量满天飞、路径随便拼接、没有函数封装的裸奔代码、无任何注释和文档...

在这个过程中，“破窗效应”常常悄无声息在起作用。一个典型的现象是随着脚本的拷贝粘贴，项目根目录下会出现各种单一功能的脚本，例如：
- pub.sh
- pubGroup1.sh
- pubCI.sh
- pub.cmd
- pubCI.cmd
- clean.sh
- clean.cmd
- pubAndClean.cmd
- start.sh
- start.cmd
- stop.sh
- stop.cmd
- startAll.sh
- startCI.sh
- ....

小心！当这些“一键搞定”的“便利脚本”出现在项目的根目录下时，就应该引起你的警觉！软件项目在这些复制粘贴的脚本出现的时候，代码的规模也在大量膨胀；工程师发现和解决的BUG也在一波一波的来袭；工程师之间的骂声也在升级：“这个脚本怎么用？”“为什么你那边可以，我这边不可以”“不行，还是挂了”...


## 需求分析

那么，这个问题应该怎么解决呢？通过设计解决问题。不过，在设计之前要仔细分析下这些脚本的特征。我们看下`pub.sh`会做的事情：

```
node deploy/deploy.js stop -config config.json -group group1
node deploy/deploy.js stop -config config.json -group group2                                     
node deploy/deploy.js stop -config config.json -group group3
node deploy/deploy.js pub -config config.json -group group1
node deploy/deploy.js pub -config config.json -group group2
node deploy/deploy.js pub -config config.json -group group3
node deploy/deploy.js start -config config.json -group group1
node deploy/deploy.js start -config config.json -group group2
node deploy/deploy.js start -config config.json -group group3
```

结论1:
> 发布脚本可能会针对多个组分别做`stop`,`pub`,`start`动作，并且这个`pub`的名字被不恰当的用来做为整个脚本的名字。

接下来来看`pubGroup1.sh`做的事情：
```
node deploy/deploy.js stop -config config.json -group group1
node deploy/deploy.js pub -config config.json -group group1
node deploy/deploy.js start -config config.json -group group1
```

结论2:
> 发布脚本需要支持针对不同的分组操作。

然后，我们看下`pubCI.sh`做的事情：
```
node deploy/makeconfig.js machinelist_ci.json 
node deploy/deploy.js stop -config config.json -group group1
node deploy/deploy.js stop -config config.json -group group2  
...
```

结论3:
> 发布脚本会针对不同的集群从机器列表生成对应集群所需的配置

再分析下脚本的不同类型，有的是做pub的，有的是做stop的，有的是做start的。从而有

结论4:
> 发布脚本针对一个环境的指定分组有不同的操作需求。

## 程序设计

经过上述分析，我们基本搞清楚了这些看上去每个都是“一键”搞定的一个事情的脚本背后，有着一组复合的需求。在理清了这些需求之后，我们首先改变这些想法：“这是一次性脚本”；“这个脚本一键操作很方便”；“我复制一份改一下”。

在此之前，经过一番思考，并参考设计精良的命令行范本：`git`，我们重新确定目标：
1. 合并为单一脚本。
2. 通过简洁的命令行参数满足不同的需求，命令行参数以操作做分组依据。
3. 设计出简洁的配置指定不同的集群、不同的分组。
4. 自由地忽略不需要的操作。
5. 安全的预览即将执行的命令，避免误操作。

我们把这个单一脚本命名为`dev.js`。文件的后缀名说明了我们将使用nodejs作为脚本的组织语言。


#### 子系统配置设计

上述5个目标中，第3个是首先需要搞定的，如果配置的结构清晰，程序只是对配置所决定的结构的执行。配置的结构如下：
```
{
	system = {
		basic: {
			push:[
				{group:"zookeeper":, action:"stop"},
				{group:"sleep":, action:"10"},

				{group:"zookeeper":, action:"start"},
				{group:"sleep":, action:"10"},

				{group:"zookeeper":, action:"create"},
				{group:"sleep":, action:"10"}
			],
			stop:[
				{group:"zookeeper":, action:"stop"},
				{group:"sleep":, action:"10"}
			],
			start:[
				{group:"zookeeper":, action:"start"},
				{group:"sleep":, action:"10"}
			],
			check:[
				{group:"zookeeper":, action:"check"}
			]
		},
		docker_image:{
	        push:[
	            { group: "docker_image", action: "stop" },
	            { group: "docker_image", action: "start" },
	        ],
	        start:[
	            { group: "docker_image", action: "start" },
	        ],
	        stop:[
	            { group: "docker_image", action: "stop" },
	        ],
	        check:[
	        	{ group: "docker_image", action: "check" },
	        ]
	    },
	    servers:[
	    	push:[
	            { group: "server_1", action: "stop" },
	            { group: "server_2", action: "stop" },
	            { group: "server_3", action: "stop" },

	            { group: "server_1", action: "pub" },
	            { group: "server_2", action: "pub" },
	            { group: "server_3", action: "pub" },

	            { group: "server_1", action: "start" },
	            { group: "server_2", action: "start" },
	            { group: "server_3", action: "start" },
	        ],
	        start:[
	            { group: "server_1", action: "start" },
	            { group: "server_2", action: "start" },
	            { group: "server_3", action: "start" }
	        ],
	        stop:[
	            { group: "server_1", action: "stop" },
	            { group: "server_2", action: "stop" },
	            { group: "server_3", action: "stop" },
	        ],
	        check:[
	        	{ group: "server_1", action: "check" },
	            { group: "server_2", action: "check" },
	            { group: "server_3", action: "check" }
	        ]
	    ]
	}
}
```

在这个配置结构里，有两层设计：
1. 可以配置不同的`子系统`, 并且根据常见需求预定义内置的子系统，当某些情况下不满强需求时，可以增加子系统。
2. 每个子系统下定义四种操作：`push`,`start`,`stop`,`check`。

为什么要定义出一个叫做`push`的操作呢？这是因为经过分析，下层的脚本提供了四个基本的原子操作：
- pub：将一个group拷贝到目标机器上。
- stop: 停止目标机器上的一个group
- start：开始目标机器上的一个group
- check：执行测试脚本检测一个group在目标机器上的基本运行情况。

那么，我们实际在部署的过程中，`stop`,`start`,`check` 三个操作都相对来说比较单一，不会有太多副作用。但是，`pub`动作一般来说就是部署新版本的服务上去，此时：
1. 它需要和其他的操作组合使用。
2. 多个group之间会有相互依赖关系。

因此，我们将这样的一组复杂操作组合起来，统一用一个新的概念：`push`来命名。对于子系统这样的层级，只提供`push`，而不提供单独的pub操作。

设计了子系统配置之后，我们就可以满足不同的分组配置需求，这里的一个关键地方在于每个子系统的每个操作那边具体执行那些原子指令是可以通过配置进行`编排`。

#### 命令行参数设计

定义了上述子系统配置之后，接下来考虑命令行参数的设计，根据设计目标，学习git的命令行参数，我们以操作为中心组织options。

```
node dev.js push -system basic 
node dev.js start -system basic 
node dev.js stop -system basic 
node dev.js check -system basic 
```

上述命令已经可以这对子系统执行四个不同的操作。但是有时候我们希望只针对某个单一group做操作，因此，命令行将支持直接针对group的操作：

```
node dev.js push -group server1
node dev.js pub -group server1
node dev.js start -group server1
node dev.js stop -group server1
node dev.js check -group server1
```

在针对group的操作中，将group的`pub`操作也暴露出来，提供给精细控制的情景使用。


进而，考虑第4个需求：在针对子系统的操作里应该能临时忽略某些子操作。因此，我们增加`-i`选项：
```
node dev.js push -system basic -i start,stop
```

通过`-i op1,op2,op3` 这样的方式，我们可以忽略某些子操作。有时候，我们也希望忽略子系统的某些分组。那么可以增加一个新的忽略分组的命令行参数，但是，鉴于分组(group)的名字肯定不会和操作(op)的名字重合，我们可以直接让`-i`选项里能忽略分组：
```
node dev.js push -system server -i stop,server2
```

到此为止，还缺了一个部件，那就是针对不同的集群切换配置的选项。这可以轻松增加：
```
node dev.js push -system server -env ci
```

增加了的`env`参数变成一个必填参数，这样增加了可靠性：操作者必须知道自己针对哪个集群上部署，针对一些关键的集群，增加密码输入的要求。


#### 预览的重要性

在上面的设计里，为了满足针对不同集群，忽略某些操作，子系统执行序列的配置。当用户敲入命令行，输入回车的瞬间，到底哪些命令会被执行？

为了解决这个问题，dev.js的执行部分设计两种不同的模式：
```
function exe(script,mode){
	if(mode==='e'){
		// execute the script...
	else{
		console.log(script);
	}
}
```

而且，默认模式设置为`预览`模式，所以默认情况下，当用户敲入回车时，只会看到即将执行的命令预览：
```
scripts:

node deploy/deploy.js stop -config config.json -group group1
node deploy/deploy.js stop -config config.json -group group2                                     
node deploy/deploy.js stop -config config.json -group group3
node deploy/deploy.js pub -config config.json -group group1
node deploy/deploy.js pub -config config.json -group group2
node deploy/deploy.js pub -config config.json -group group3
node deploy/deploy.js start -config config.json -group group1
node deploy/deploy.js start -config config.json -group group2
node deploy/deploy.js start -config config.json -group group3

please use `-e` to execute it.

```


只有当用户输入`-e`选项时，才会真正执行，这样所有对自己的操作有疑惑的用户都可以放心的预览目标指令序列。


## 尾声

关于指令序列，有点像汇编的过程：
1. 提供原子的指令。
2. 提供组合原子指令的机制（例如分组）。

在程序设计的其他场景，例如测试用例的组织，异步链式逻辑的组织里，指令序列都有使用的机会。
























