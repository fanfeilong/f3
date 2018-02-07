/**************************************
 * 使用Promise的函数
 **************************************/

// promise方式返回异步结果
function promiseFunc(arg){
	return new Promise((resolve,reject)=>{

		// 即使这里不是个异步函数，直接resolve
		// Promise依然保证会异步执行
		resolve({err:0,result:arg});
	});
}

// 调用：ES7之后，增加了async，可以直接await去调用, 任何调用了await的函数，本身必须是async函数
async function test1(){
	let value = await promiseFunc('call promiseFunc await');
	console.log(value.err,value.result);
}

console.log('before test1');
test1();
test1();
console.log('after test1');


// 参考：
// 1. http://js.walfud.com/macrotask-microtask/
// 2. https://jakearchibald.com/2015/tasks-microtasks-queues-and-schedules/
// 
// 在NodeJs里，异步任务被分为了2大类，MacroTask和MicroTask，

// - Macrotask 包括:
//     - setImmediate
//     - setTimeout
//     - setInterval
// - Microtask 包括:
//     - process.nextTick
//     - Promise
//     - Object.observe
//     - MutaionObserver

// 其中，process.nextTick又细分叫做tickTask, 这些不同Task
// 在NodeJs的eventloop里使用不同的队列，执行的大概顺序如下：
 
// for (macroTask of macroTaskQueue) {

//     // 1. Handle current MACRO-TASK
//     handleMacroTask();

//     // 2. Handle all NEXT-TICK
//     for (nextTick of nextTickQueue) {
//         handleNextTick(nextTick);
//     }

//     // 3. Handle all MICRO-TASK
//     for (microTask of microTaskQueue) {
//         handleMicroTask(microTask);
//     }
// }
