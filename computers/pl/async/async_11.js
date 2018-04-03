function query(sql,callback){
	setTimeout(()=>{
		console.log(sql);
		callback(0);
	},1);
}

function multiQuery(sqls,callback){
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
				callback(result);
			}
		})
	}
}

function test(){
	let sqls = ['select * from non_exsit_table_1','select * from non_exsit_table_2'];
	multiQuery(sqls,(r)=>{
		console.log('result from multiQuery:',r);
	});
}

test();



