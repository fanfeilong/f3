function *foo(x) {
    var y = 2 * (yield (x + 1));
    var z = yield (y / 3);
    return (x + y + z);
}

var it = foo( 5 );

// note: not sending anything into `next()` here
// console.log( it.next() );       // { value:6, done:false }
// console.log( it.next( 12 ) );   // { value:8, done:false }
// console.log( it.next( 13 ) );   // { value:42, done:true }

// note: not sending anything into `next()` here
let v = it.next().value;
console.log(v);

v = it.next(v).value;
console.log(v);

v = it.next(v).value;
console.log(v);