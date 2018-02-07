
async function queryAsync(sql){
	return new Promise((resolve, reject) => {
		setTimeout(()=>{
			console.log(sql);
			resolve(0);
		},1);	
	});
}

// 
// [描述]：
// for循环await 采用的是「异步链式」执行，
// 只有一个queryAsync执行完毕才会接着执行下一个queryAsync
// 
async function multiQueryAsyncSequence(sqls) {
	let result = {
		errorCount:0,
		successCount:0
	};

	console.log('begin multiQueryAsyncSequence');
	for(let sql of sqls){
		let err = await queryAsync(sql);
		if(err){
			result.errorCount++;
		}else{
			result.successCount++;
		}
	}
	console.log('end multiQueryAsyncSequence');

	return result;
}

async function test(){
	let sqls = ['select * from non_exsit_table_1','select * from non_exsit_table_2'];
	let r = await multiQueryAsyncSequence(sqls);
	console.log('result from multiQueryAsyncSequence:',r);
}

test();