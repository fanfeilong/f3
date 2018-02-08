const fs = require('fs');

var Thunk = function(fn){
  return function (){
    var args = Array.prototype.slice.call(arguments);
    return function (callback){
      args.push(callback);
      return fn.apply(this, args);
    }
  };
};


var readFile = Thunk(fs.readFile);

var gen = function* (x){
  console.log(x);
  var [err,r1] = yield readFile(__filename);
  console.log(r1.toString());
  var [err,r2] = yield readFile(__filename);
  console.log(r2.toString());
};

// 自动递归执行
function simpleCo(fn) {
  return function(...args){
  	var gen = fn(...args);

  	console.log('1');
  	function next(...args) {
  		console.log('~');
  		var result = gen.next([...args]);

  		console.log('3', result.done);
  		if (result.done) return;

  		console.log('4');
  		result.value(next);
  	}

  	console.log('2');
  	next();	
  }
}

simpleCo(gen)('hello Thunk!');










