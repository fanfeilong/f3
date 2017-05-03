<hr/>
**控制结构目录**：[1](http://www.cnblogs.com/math/p/control-structure-001.html) [2](http://www.cnblogs.com/math/p/control-structure-002.html) [3](http://www.cnblogs.com/math/p/control-structure-003.html) [4](http://www.cnblogs.com/math/p/control-structure-004.html)
<hr/>

基于语言提供的基本控制结构，更好地组织和表达程序，需要良好的控制结构。

## 前情回顾

上次分析了guard 控制结构，有人说：“可是阅读很多开源代码也是if/else层层嵌套的”。是的，guard控制结构只是一种风格，语言并没有规定你一定要这么写，也没必要。两种风格都能写出逻辑清晰的代码，但是还是有细微差别，嵌套模式下，要写出清晰的代码需要更好的做好函数的分层（见第1节），例如，在分支里只做函数的转发调用，那么，代码也是清晰可读的。

```javascript
function doSomething(){
	if(condition1){
		call1();
	}else{
		if(condition2){
			call2();
		}else{
			call3();
		}
	}	
}
```

我们来看一个新的结构。

## 典型代码：

```javascript

let opening = false;
let opened = false;
let closing = false;
let closed = false;
let step1 = false;

function open(onComplete){
	if(closing||closed){
		return onComplete(RESULT.CLOSED)
	}

	if(opening){
		return onComplete(RESULT.PENDING);
	}

	if(opened){
		return onComplete(RESULT.SUCCESS);
	}
	

	opening = true;
	doOpen(function(err){
		onComplete(err);
		opening = false;
		opened = true;
	});
}

function close(onComplete){
	if(!opened){
		return onComplete(RESULT.INVALID_OP);
	}

	if(closing||closed){
		return onComplete(RESULT.PENDING);
	}

	closing = true;
	doClose(function(err){
		onComplete(err);
		closing = false;
		closed = true;
	})
}

function start(onComplete){
	open((err)=>{
		if(err){
			return onComplete(err);
		}
		step1((err)=>{
			if(err){
				return onComplete(err);
			}		
			step2((err)=>{
				onComplete(err);
				close((err)=>{
					log(`close, ret:${err}`);
				})
			})
		})
	})
}

function step1(onComplete){
	if(!opened){
		return onComplete(RESULT.NOT_OPEN);
	}

	doStep1(function(err){
		step1 = true;
		onComplete(err);
	});
}

function step2(onComplete){
	if(!opened){
		return onComplete(RESULT.NOT_OPEN);
	}

	if(!step1){
		return onComplete(RESULT.INVALID_STEP);
	}

	doStep2(function(err){
		onComplete(err);
	});
}

```

## 结构分析

这段代码使用了多个bool变量来做状态标记，为了安全有序的做好每个异步动作，需要在适当的地方对每个bool标志变量做判断。如果这个流程继续复杂起来，bool变量之间的状态组合会变的更加繁杂，也很容易一不小心就漏掉状态的组合判断，导致难以定位的BUG。此时，我们可以采用一种重要的控制结构。

...

是的，状态机(state machine)结构。我们先来看看使用状态机控制结构改写后的代码：

```javascript

/**所有的状态*/
const STATE = {
	INIT: 0,
	OPENING: 1,
	OPENED: 2,
	STEP1: 3,
	STEP2: 4,
	CLOSING: 5,
	CLOSED: 6,
	ERROR: 7
} 

/**每个状态都只允许从指定状态转移过来*/
const stateTransfer = {
	STATE.INIT: [],
	STATE.OPENING: [STATE.INIT],
	STATE.OPENED: [STATE.OPENING],
	STATE.STEP1: [STATE.OPENED],
	STATE.STEP2: [STATE.STEP1],
	STATE.CLOSING: [STATE.INIT, STATE.OPENING, STATE.OPENED, STATE.STEP1, STATE.STEP2],
	STATE.CLOSED: [STATE.CLOSING],
	STATE.ERROR: [STATE.INIT, STATE.OPENING, STATE.OPENED, STATE.STEP1, STATE.STEP2],
}

let state = STATE.INIT;
let stateChangeEvent = null;

function moveTo(err, newState){
	
	let nextState = err ? STATE.ERROR : newState;

	if(stateTransfer[nextState].indexOf(state)===-1){
		return RESULT.INVALID_STATE_TRANSFER;
	}

	let oldState = state;
	state = nextState;
	stateChangeEvent(oldState, state, err);

	do();

	return RESULT.SUCCESS;
}

function do(){
	switch(state){
		case STATE.INIT:
			moveTo(0, STATE.OPENING);
			break;
		case STATE.OPENING:
			doOpen((err)=>{
				moveTo(err, STATE.OPENED);
			});
			break;
		case STATE.OPENED:
			doStep1((err)=>{
				moveTo(err, STATE.STEP1);
			});
			break;
		case STATE.STEP1:
			doStep2((err)=>{
				moveTo(err, STATE.STEP2);
			});
			break;
		case STATE.STEP2:
			moveTo(0, STATE.CLOSING);
			break;
		case STATE.CLOSING:
			doClose((err)=>{
				moveTo(err, STATE.CLOSED);
			});
			break;
		case STATE.CLOSED:
			break;
		default:
			break;
	}
}

function start(onStateChange){
	stateChangeEvent = onStateChange;
	do();
}

```

## 语义分析

看上去好像多了很多代码。然而在复杂的异步处理流程中，状态机能清晰地表达逻辑，保证逻辑代码的时序，并在形式推理上发现和消灭漏洞。编程语言提供了基本的状态标志位：bool变量。这便于让程序员在编程中随意地使用bool变量标记各种状态，但0/1状态标示在稍微复杂一点的状态变化中，就会显得不好用：

- 出现组合爆炸，你需要对多个bool变量做组合判断，这很容易漏掉某个状态组合。
- 设计上的失误，导致bool标志位之间不是正交的，因而任一时刻，程序的状态不能确保唯一，这会导致程序出现未知的时序问题。

事实上，任何程序在运行中本身就是一个状态机：

- 当前在第i行执行
- 根据条件，进入第j行执行

这里的状态就是：

- pc, 也就是program counter, 程序计数器
- condition，改变pc的条件

如果想更清楚地理解这点，可以观看Lamport制作的视频，要看到第2集：http://lamport.azurewebsites.net/video/intro.html
别被TLA+这个名词以及英文给吓唬到了，Lamport制作的视频很棒，口齿清晰，字幕到位，视频也很风趣，这个视频总共才做了3集，只要看到第2集就可以看到程序是一个状态机这个点。






