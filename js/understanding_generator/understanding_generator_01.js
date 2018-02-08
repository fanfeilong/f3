
function* gen(){
	yield 1;
	yield 2;
	yield 3;
	return 4;
}

let g = gen();
console.log(g.next().value);
console.log(g.next().value);
console.log(g.next().value);
console.log(g.next().value);


