/**************************************
 * MacroTask/MicroTask/tickTask
 **************************************/

function promiseFunc(arg){
	return new Promise((resolve,reject)=>{
		resolve({err:0,result:arg});
	});
}

async function test2(){
	// 同步打印
	console.log('execute asycn test2');

	// Timer是一种MacroTask，顺序排在tickTask/MicroTask之后
	// 请观察日志顺序
	setTimeout(()=>{
		console.log('this is a macro task, [call in test2] setTimeout');
	},1);

	// process.nextTick是tickTask，它们会在'sync-end'被打印后优先于所有的
	// Promise被执行，可见tickTask优先于MicroTask
	// 请观察日志顺序
	process.nextTick(()=>{
		console.log('this is a tick task, [call in test2] process.nextTick');
	})

	// 这个Promise的打印在下面的async-return-promise之前执行，
	// 请观察日志顺序
	promiseFunc('this is a microtask, [call in test2] which is a Promise').then(v=>console.log(v.result));

	// 返回值虽然看上去是同步的，但是aync会封装成一个Promise返回，我们假设为 async-return-promise
	return 'this is a microtask, [call in test2] return asycn test2'
}

console.log('before test2');
test2().then(v=>console.log(v));
test2().then(v=>console.log(v));
console.log('after test2'); // 注意这后面的打印日志

promiseFunc('this is a microtask, [call after test2]').then(v=>console.log(v.result));

console.log('sync-end');

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
