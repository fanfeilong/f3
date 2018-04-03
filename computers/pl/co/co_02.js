
let p1 = new Promise((resolve, reject) => {
	console.log('p1');
	resolve('p11~~');
});

let p2 = new Promise((resolve, reject) => {
	console.log('p2');
	resolve('p22~~');
});

let results=[];
let p11 = p1.then(v=>{results.push(v);return v;});
let p22 = p2.then(v=>{results.push(v);return v;});

p11.then(v=>console.log(v));
p22.then(v=>console.log(v));

let p = Promise.all([p11,p22]).then(()=>{
	return results;
});

p.then(v=>{
	console.log('all',results);
});