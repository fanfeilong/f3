
/*1. Continuation-Passing Style*/
function cps(exp1,exp2){
	return (k)=>{
		exp1(res1=>{
			exp2(res1, res2=>{
				k(res2);
			});
		});
	};	
}

function cps_test(){
	let read=>(k){
		k('read');
	}

	let write=>(v,k){
		k(v+', write');
	}

	let e = cps(read,write);
	e((r)=>{
		console.log(r);
	});
}
cps_test();

/*2. Accumulator-Passing Style*/
function seed(){
	return new Date().getTime();
}

function rand(){
	let ans=>(seed,k){
		k(16807*seed%2147483647);
	};
	let 
}














