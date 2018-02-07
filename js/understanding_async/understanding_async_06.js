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

// 例子3
async function asyncFunc3(arg){
	let v = await promiseFunc(arg);    // 对Promise await再作为async函数的返回值
	return v;
}



async function test(){
	let v = await asyncFunc3('asyncFunc3'); // 注意这是异步的，不是同步调用
	console.log(v.err, v.result);
}

test();