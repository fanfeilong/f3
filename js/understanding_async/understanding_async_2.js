function doSometingAsync(arg, callback){
	setTimeout(()=>{
		callback(0,arg);
	},1000);
}

/**************************************
 * 使用Promise的函数
 **************************************/

// 2. promise方式返回异步结果
function promiseFunc(arg){
	return new Promise((resolve,reject)=>{
		doSometingAsync(arg,(err,result)=>{
			if(err===0){
				// JavaScript标准规定，resolve只接受一个返回值
				resolve({err:err,result:result});
			}else{
				reject(err);
			}
		})
	});
}

// 调用1: 有两种方式调用，一种是使用Promise的then/catch方法分别响应resolve和reject
function test1(){
	promiseFunc('call promiseFunc then').then((value)=>{
		console.log(value.err,value.result);
	}).catch(err=>{
		console.error(err);
	});
}

// 调用2：ES7之后，增加了async，可以直接await去调用, 任何调用了await的函数，本身必须是async函数
async function test2(){
	let value = await promiseFunc('call promiseFunc await');
	console.log(value.err,value.result);
}

test1();
test2();