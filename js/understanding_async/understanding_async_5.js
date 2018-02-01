/**************************************
 * 使用async函数
 **************************************/

 function doSomethingAsync(arg, callback){
	setTimeout(()=>{
		callback(0,arg);
	},1000);
}

function promiseFunc(arg){
	return new Promise((resolve,reject)=>{
		doSomethingAsync(arg,(err,result)=>{
			if(err===0){
				resolve({err:err, result:result});
			}else{
				resolve(err);
			}
		})
	});
}

// 例子2， 返回Promise
async function asyncFunc2(arg){
	return promiseFunc(arg); 
}



async function test(){
	let v = await asyncFunc2('asyncFunc2'); // 注意这是异步的，不是同步调用
	console.log(v.err, v.result);
}

test();