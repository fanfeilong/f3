async function queryAsync(sql){
	return new Promise((resolve, reject) => {
		setTimeout(()=>{
			console.log(sql);
			resolve(0);
		},1);	
	});
}

async function queryAsyncWithResult(sql,result){
	let err = await queryAsync(sql);
	if(err){
		result.errorCount++;
	}else{
		result.successCount++;
	}
}

// 
// [描述]
// Promise.all 是当所有给定的可迭代完成时执行 resolve，或者任何  promises 失败时执行 reject。
// - 如果传递任何的 promises rejects ，所有的 Promise 的值立即失败，丢弃所有的其他 promises，如果它们未 resolved。
// - 如果传递任意的空数组，那么这个方法将立刻完成。
// see: https://developer.mozilla.org/zh-CN/docs/Web/JavaScript/Reference/Global_Objects/Promise/all
// 
// 注意：
// Promise.all 会并发执行Promise，并不保证按顺序逐个执行，这点与for循环await不同。
// 
async function multiQueryAsyncParallel(sqls) {
	let result = {
		errorCount:0,
		successCount:0
	};

	console.log('begin multiQueryAsyncParallel');
	let queryPromiseArray = sqls.map(sql=>queryAsyncWithResult(sql,result));
	await Promise.all(queryPromiseArray);
	console.log('end multiQueryAsyncParallel');

	return result;
}	

async function test(){
	let sqls = [1,2,3,4,5,6,7,8].map(i=>`select * from non_exsit_table_${i}`);
	let r = await multiQueryAsyncParallel(sqls);
	console.log('result from multiQueryAsyncParallel:',r);
}

test();