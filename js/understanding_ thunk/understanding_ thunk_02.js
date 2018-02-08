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

// 手工执行
var g = gen('hello Thunk!');
var r1 = g.next();
r1.value(function(err, data){
  if (err) throw err;
  var r2 = g.next([err,data]);
  r2.value(function(err, data){
    if (err) throw err;
    g.next([err,data]);
  });
});











