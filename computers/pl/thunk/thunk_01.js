
// http://www.ruanyifeng.com/blog/2015/05/thunk.html

// 1. thunk是"传名调用"的一种实现策略，用来替换某个表达式
// function f(m){
//   return m * 2;     
// }

// f(x + 5);

// // 等同于

// var thunk = function (x) {
//   return x + 5;
// };

// function f(thunk){
//   return thunk() * 2;
// }

// console.log(f(thunk));


// 2. 在 JavaScript 语言中，Thunk 函数替换的不是表达式，而是多参数函数，将其替换成单参数的版本，且只接受回调函数作为参数
// 
// 正常版本的readFile（多参数版本）
fs.readFile(fileName, callback);

// Thunk版本的readFile（单参数版本）
var Thunk = function (fileName){
  return function (callback){
    return fs.readFile(fileName, callback); 
  };
};

Thunk(fileName)(callback);

// 3. Thunk生成器
var Thunk = function(fn){
  return function (){
    var args = Array.prototype.slice.call(arguments);
    return function (callback){
      args.push(callback);
      return fn.apply(this, args);
    }
  };
};


var readFileThunk = Thunk(fs.readFile);
readFileThunk(fileA)(callback);

// 4. 确保只运行一次
function thunkify(fn){
  return function(){
    var args = new Array(arguments.length);
    var ctx = this;

    for(var i = 0; i < args.length; ++i) {
      args[i] = arguments[i];
    }

    return function(done){
      var called;

      args.push(function(){
        if (called) return;
        called = true;
        done.apply(null, arguments);
      });

      try {
        fn.apply(ctx, args);
      } catch (err) {
        done(err);
      }
    }
  }
};



