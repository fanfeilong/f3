function doSometingAsync(arg, callback){
	setTimeout(()=>{
		callback(0,arg);
	},1000);
}

/**************************************
 * 使用Promise的函数2
 **************************************/


// 需要注意的是，如果函数内有多层异步调用，Promise必须在最外层就创建，你在哪里返回Promise
// 起作用的就是那层的函数，例如
function promiseFunc(arg){
	return new Promise((resolve,reject)=>{

		// 异步调用
		doSometingAsync(arg,(err, result1)=>{            

			if(err){    
				// 这里reject对应的Promise属于promiseFunc的
				reject(err);                            
				return;
			}

			// 嵌套的异步调用
			doSometingAsync(arg,(err, result2)=>{       

				if(err===0){ 
					let value = {
						err:err, 
						result:result1+', '+result2
					};        
					// 这里resolve对应的Promise属于promiseFunc的      
					resolve(value);                     
				}else{
					reject(err);
				}

			});
		})
	});
}

// 调用2：ES7之后，增加了async，可以直接await去调用, 任何调用了await的函数，本身必须是async函数
async function test1(){
	let value = await promiseFunc('call promiseFunc await');
	console.log(value.err,value.result);
}

test1();
