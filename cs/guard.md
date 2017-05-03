<hr/>
**控制结构目录**：[1](http://www.cnblogs.com/math/p/control-structure-001.html) [2](http://www.cnblogs.com/math/p/control-structure-002.html) [3](http://www.cnblogs.com/math/p/control-structure-003.html)
<hr/>

基于语言提供的基本控制结构，更好地组织和表达程序，需要良好的控制结构。

## 典型代码：

- 同步版本

```javascript
function loadFunc(funcInfo){
    if(funcInfo){
        let funcObj = doParserFunc(funcInfo);
        if(funcObj){
            let package = doLoadPackage(funcObj.packageName);
            if(pacakge){
                let module = doLoadModule(pacakge,funcObj.moduleName);
                if(module){
                    let func = module[funcObj.functionName];
                    if(func){
                        return func;
                    }else{
                        // do something
                    }
                }else{
                    // do something
                }
            }else{
                // do something
            }
        } else {
            // do something
        }
    }
    return null;
}
```

- 异步版本

```javascript
function loadFunc(funcInfo, onComplete){
    if(funcInfo){
        let funObj = doParseFunc(funcInfo);
        if(funcObj){
            doLoadPackage(funcObj.packageName, function(err,package){
                if(package){
                    doLoadModule(package, funcObj.moduleName, function(err, module){
                        if(module){
                            let func = module[funcObj.functionName];
                            if(func){
                                onComplete(0,func);
                            }else{
                                onComplete(1);
                            }
                        }else{
                            onComplete(1);
                        }
                    })
                }else{
                    onComplete(1);
                }
            });
        }else{
            onComplete(1);
        }
    }else{
        onComplete(1);
    }
}
```

## 结构分析

无论是同步版本，还是异步版本，都存在嵌套持续变深的问题。随着开发的进行，需求的变更，代码会变的越发繁杂。一种方式是通过上一节的方式，合理组织函数的分层，让函数的组织表达更清晰。但是另一方面，在支持lambda表达式和匿名函数的语言里，编程的时候总是会大量使用语言提供的这种便利，写带有许多lambda表达式或匿名函数的逻辑代码。一种常见的方式是使用卫语句(guard clause)的方式提前返回，减少嵌套。

- 同步版本

```javascript
function loadFunc(funcInfo){
    if(!funcInfo){
        return null;
    }

    let funcObj = doParserFunc(funcInfo);
    if(!funcObj){
        return null;
    }

    let package = doLoadPackage(funcObj.packageName);
    if(!package){
        return null;
    }

    let module = doLoadModule(pacakge,funcObj.moduleName);
    if(!module){
        return null;
    }

    let func = module[funcObj.functionName];
    if(!func){
        return null;
    }

    return func;
}
```

- 异步版本

```javascript
function loadFunc(funcInfo, onComplete){
    if(!funcInfo){
        return onComplete(RESULT.INVALID_PARAMETER);
    }

    let funcObj = doParserFunc(funcInfo);
    if(!funcObj){
        return onComplete(RESULT.PARSE_ERROR);
    }

    
    doLoadPackage(funcObj.packageName, function(err,package){
        if(!package){
            return onComplete(RESULT.LOAD_PACKAGE_FAILED);
        }

        doLoadModule(package, funcObj.moduleName, function(err, module){
            if(!module){
                return onComplete(RESULT.LOAD_MODULE_FAILED);
            }
            
            let func = module[funcObj.functionName];
            if(!func){
                return onComplete(RESULT.FUNC_NOT_EXIST);
            }

            onComplete(RESULT.SUCCESS,func);
        });
    });
}
```

## 语义分析

编程语言提供的`if/else`结构，有两种基本的用法：

- 优先考虑满足条件就做什么
```javascript
if(condition){
    doSomething();
}else{
    logError();
}
```

- 优先考虑不满足条件就处理错误

```javascript
if(!condition){
    logError();
}else{
    doSomething();
}
```

这两种结构，各自都能写出逻辑严密的代码。例如，在前一种优先模式下，一种重要的方式把所有的`if`分支都写下它的`else`分支，保证逻辑上少漏洞。典型的控制结构是这样的：

```javascript
if(condition1){
    // <a>
    if(condition2){
        // <b>
        if(condition3){
            // <c>
            if(condition4){
                // <d>
                doSomething1();
            }else{
                // <e>
                doSomething2();
            }
        }else{
            // <f>
            doSomething3();
        }
    }else{
        // <g>
        doSomething4();
    }
}else{
    // <h>
    doSomething5();
}
```

这种结构确实也能更直接的和大脑里的流程结构对上，如果勤快一点画里流程图，也能直接对上。但是程序是会根据需求变化的，在需求变化的时候，很容易在上述`<a>`、`<b>`, ...处产生碎片代码，此时如果对上一节介绍的函数分层有比较好的实施，则代码依然保持良好的可读／可维护。

但是，在混合了同步、异步之后，即使有了良好的函数组织，也还是容易出现嵌套深的情况，此时，可以配合适当的guard结构去组织代码。使用guard语句，可以让代码更加线性化。具体在情景代码中应该使用哪种方式，就是一种编程中的选择问题。如果考虑一致性，最好一个模块保持一致。




