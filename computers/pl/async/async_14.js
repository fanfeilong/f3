function query(sql,callback){
	setTimeout(()=>{
		console.log(sql);
		callback(0);
	},1);
}

// 混合例子：
// async内部for循环调用callback形式
async function multiQueryAsyncSequence2(sqls){

	console.log('enter multiQueryAsyncSequence2');
	return new Promise((resolve, reject) => {
		
		console.log('begin multiQueryAsyncSequence2');
		let result = {
			errorCount:0,
			successCount:0
		};

		let index = 0;
		for(let sql of sqls){
			query(sql,(err)=>{
				if(err){
					result.errorCount++;
				}else{
					result.successCount++;
				}
				index++;
				if(index===sqls.length){
					// 使用了Promise，则
					// resolve的时候async函数才真正返回
					resolve(result);
				}
			})
		}

		console.log('end multiQueryAsyncSequence2');
	});
}

async function test(){
	let sqls = ['select * from non_exsit_table_1','select * from non_exsit_table_2'];
	let r = await multiQueryAsyncSequence2(sqls);
	console.log('result from multiQueryAsyncSequence2:',r);
}

test();



