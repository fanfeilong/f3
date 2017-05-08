<hr/>
**控制结构目录**：[1](http://www.cnblogs.com/math/p/control-structure-001.html) [2](http://www.cnblogs.com/math/p/control-structure-002.html) [3](http://www.cnblogs.com/math/p/control-structure-003.html) [4](http://www.cnblogs.com/math/p/control-structure-004.html) [5](http://www.cnblogs.com/math/p/control-structure-005.html)
<hr/>

基于语言提供的基本控制结构，更好地组织和表达程序，需要良好的控制结构。很多时候，是因为不了解，因此没有意识到。

## 前情回顾

上一周，我们谈到了`分支`／`卫语句`／`状态机`／`局部化`。它们是相互补充协作的关系，并且我们都只使用函数就达到了说明的目的。为什么仅仅使用函数来说明呢？回到第一篇提到的`分枝／叶子`，可以看到，无论上层代码怎样组织，在对象层面做了怎样的抽象封装，最终是要在函数这个级别实现具体的调用动作的。在对象层面的组织，有很多书和文章，但是无论是老手还是新手，都有许多程序员不能良好的组织函数内代码，使得其具有更好的可读／可维护。我们看一个小例子：

```javascript
function initApp(){
    let appMetaPath = path.join(__dirname,'appmeta.json');
    let packageDir = path.join(__dirname, 'test/packages');
    let appMeta = JSON.parse(fs.readFileSync(appMetaPath));
    
    let uid = appMeta.uid;
    let token = appMeta.token;

    let app = new Application();
    let appInfo = JSON.parse(fs.readFileSync(path.join(__dirname,'test/packages/app.json')));
    let packageConfig = JSON.parse(fs.readFileSync(path.join(packageDir,'app.json')));
    app.init(appInfo, function(err,info){
        // ... 用到相关信息
    });

}
```

很短的一个`叶子`代码，简单调整顺序改进下：

```javascript
function initApp(){
    let appMetaPath = path.join(__dirname,'appmeta.json');
    let appMeta = JSON.parse(fs.readFileSync(appMetaPath));

    let appConfigPath = path.join(__dirname,'test/packages/app.json');
    let appInfo = JSON.parse(fs.readFileSync(appConfigPath));
    
    let pacakgeConfigPath = path.join(__dirname, 'test/packages/config.json');
    let packageConfig = JSON.parse(fs.readFileSync(pacakgeConfigPath));
    
    let app = new Application();
    app.init(appInfo,function(err,info){
        //... 用到相关信息
    });
}
```

其实不复杂，代码如果读起来是顺序结构就更好读，也更利于维护。 

## 典型代码

- 例子1

```C++
int write(char* buffer){
    thisLock.lock();
    //....
    if(err1){
        thisLock.unlock();
        return RESULT.ERROR1;
    }
    //...
    if(err2){
        thisLock.unlock();
        return RESULT.ERROR2;
    }
    //...
    thisLock.unlock();
    return RESULT.SUCCESS;
}
```

- 例子2

```javascript
function findItemByBindedGroupID(groupID, onComplete){
    var mysql = require('mysql');
    var pool  = mysql.createPool(...);

    pool.getConnection(function(err, connection) {
      connection.query('SELECT * FROM group where ?', {groupID: groupID}, 
          function (error, groups) {
            if (error||groups.length===0){
                connection.release();
                return onComplete(1);
            }

            let group=groups[0];
            let itemID = group.bindedItemID;
            connection.query('SELECT * FROM item where ?', {itemID:itemID}, 
                function(error, items){
                    connection.release();
                    if (error||items.length===0){
                        onComplete(1);
                    }else{
                        onComplete(0, items[0]);
                    }
                });
          });
    });
}
```

## 结构分析

这是一段典型的`打开资源`、`读／写`、`关闭资源`的操作，问题在于当你要写很多这样的代码时，代码就会显得繁琐，在每个返回分支都要记得关闭资源也是一个很容易被忘记的动作，于是就会出现典型的资源泄露。在不同语言里，如何对资源做自动释放，在日常开发中出现的频率很高。不同语言有不同的做法。例如C++语言里的Lock代码，有多种方式改进它：

- `goto`方式

````C++
int write(char* buffer){
    thisLock.lock();
    int ret = 0;

    //....
    if(err){
        ret = RESULT.ERROR1;
        goto quit;
    }
    //...
    if(err2){
        ret = RESULT.ERROR2;
        goto quit;
    }
    //...
    ret = RESULT.SUCCESS;

    quit:
    thisLock.unlock();
    return ret;
}
````

- `do-while`方式

```C++
int write(char* buffer){
    thisLock.lock();

    do{
        //...
        if(err){
            ret = RESULT.ERROR1;
            break;
        }
        //...
        if(err2){
            ret = RESULT.ERROR2;
            break;
        }
        // ...
        ret = RESULT.SUCCESS;
    }while(true);
    
    thisLock.unlock();
}
```

可以看到，很多人限制了不能用goto，但do-while并不比goto少多少代码，还多了一层嵌套。最后，就是用C++的对象方式（RAII）解决：

- `RAII`方式

```C++
class AutoLock{
    AutoLock(lock){
        this.m_lock = lock;
        this.m_lock.lock();
    }
    ~AutoLock(){
        this.m_lock.unlock();
    }
}
int write(char* buffer){
    AutoLock lock(thisLock); //对象析构的时候自动unlock
    //...
    if(err){
        return RESULT.ERROR1;
    }
    //....
    if(err2){
        return RESULT.ERROR2;
    }    
    //...
    return RESULT.SUCCESS;
}
```

我们再看下JavaScript的例子，同样JavaScript里也有多种做法，例如使用之前提到过的状态机方式，不过此次我们希望只用基本的控制结构和函数来封装，同时不改变代码的通常读法。

```javascript
class Connection{
    constructor(){
        this.m_database = ...
    }

    open(onComplete) {
        let self = this;
        self.m_database.getPOOL().getConnection(function(err, conn) {
            if (err) {
                return onComplete(err);
            }
            self.m_conn = conn;
            onComplete(0);
        });
    }

    close() {
        let self = this;
        self.m_conn.release();
    }

    executeQuery(action, sql, values, onComplete) {
        let self = this;
        let r = self.m_conn.query(sql, values, function (err, results) {
            if (err) {
                log.error(`do ${action} failed, err:${err}`);
                onComplete(err, results);
            } else {
                log.info(`do ${action} success.`);
                onComplete(err, results);
            }
        });
        log.info(`action: ${action}, sql: ${r.sql}`);
    }

    usingQuery(action, onComplete){
        let self = this;

        /**对onComplete做一层wrapper，调用之前先关闭连接*/
        let hasClose = false;
        let theComplete = function (err, results) {
            if (hasClose) {
                return;
            }
            self.close();
            onComplete(err, results);
        };

        /**只打开连接一次*/
        let hasOpen = false;
        let open = function(callback){
            if (hasOpen){
                return callback(0);
            }
            hasOpen = true;
            self.open(function(err){
                callback(err);
            });
        };

        /**通过返回一个在首次query时自动open对query接口，并把theComplete返回*/
        let context = {
            onComplete: theComplete,
            query: function(sql, values, callback){
                open(function(err){
                    if(err){
                        callback(err);
                    }else{
                        self.executeQuery(action, sql, values, callback);
                    }
                });
            }
        };

        return context;
    }
}
```

于是，我们可以这样使用，现在只需关心查询本身的逻辑即可：

```javascript
function findItemByBindedGroupID(groupID, onComplete){
    let conn = new Connection();
    let context = conn.using('findItemByBindedGroupID', onComplete);

    context.query('SELECT * FROM group where ?', {groupID: groupID}, 
        function (error, groups) {
            if (error||groups.length===0){
                return context.onComplete(1);
            }

            let group=groups[0];
            let itemID = group.bindedItemID;
            context.query('SELECT * FROM item where ?', {itemID:itemID}, 
                function(error, items){
                    if (error||items.length===0){
                        return context.onComplete(1);
                    }

                    context.onComplete(0, items[0]);
                });
        });
}
```

## 语义分析

打开资源，对资源做一些操作，关闭资源。看上去很简单的一组动作，如果遇到中间有多次操作，每次操作都可能有错误，每次错误的时候都需要释放资源，即容易忘记，又繁琐。

我们只要能识别函数退出路径里必经之地，对必经之地做一层浅浅的hook，就能实现资源自动释放动作。

例如，在C++里，函数超出作用域之后，一定会调用栈上对象的析构函数，语言提供了这种保证，我们就可以在这个函数退出必经之地做自动释放。

例如，在JavaScript里，异步调用的时候，我们可以依赖于一个前置假设：“异步接口一定通过回掉函数退出”，那么我们就可以对这个异步回调的必经之地做一个浅封装，达到资源的自动释放目的，同时又不会剧烈改变代码的直观逻辑。

例如，C#语言内置提供了using语句，使得在using作用域退出的时候能自动释放实现了IDispose接口的对象。

