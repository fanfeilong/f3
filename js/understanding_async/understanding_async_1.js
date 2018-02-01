function doSometingAsync(arg, callback){
	setTimeout(()=>{
		callback(0,arg);
	},1000);
}

/**************************************
 * 普通回调函数
 **************************************/

// 1. 普通回调方式返回异步结果
function normalFunc(arg,callback){
	doSometingAsync(arg,(err, result)=>{
	   callback(err,result);
	})
}

function test1(){
	normalFunc('call normalFunc',(err,result)=>{ 
		console.log(err, result); 
	});	
}

test1();

