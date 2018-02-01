/**************************************
 * 使用async函数
 **************************************/

// 例子1
async function asyncFunc1(arg){
	return arg;  //可直接return
}


async function test(i){
	let v = await asyncFunc1(`asyncFunc${i}`); // 注意这是异步的，不是同步调用
	console.log(v);	
	return v;
}

// 由于是异步调用，下面的输出值未必是按顺序的

// 只是调用
test(1);

// 调用并使用then接受返回值
test(2).then(v=>console.log(v));


// await调用，此时必须在async环境里
async function warpper(){
	let v = await test(3);
	console.log(v);	
}

warpper();