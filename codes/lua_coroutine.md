#### lua coroutine 的使用场景

- 异步回调转同步：

假设有如下的异步调用的lua代码
```
function(arg)
  async(arg,function(state,ret)
      if state=="case1" then
        process_case1(ret)
      elseif state=="case2" then
        process_case2(ret)
      end
  end)
end

```

可以通过lua的coroutine转成同步写法
```
- lua
coroutine.wrap(function(arg)   
   await_async(arg)

   local error,value = coroutine.yield()
   if value.state=="case1" then
      process_case1(value.ret)
      error,value = coroutine.yield()
   end

   if value.state=="case2" then
      process_case2(value.ret)
   else
      assert(false)
   end
   coroutine.yield()

end)

function await_async(arg)
   local self = coroutine.running()
   async(arg,function(state,ret)   
      if state=="case1" then
        coroutine.resume(self,{state,ret})
      elseif state=="case2" then
        coroutine.resume(self,{state,ret})
      end
   end)
end

```

看似写了更多代码，不过如果在合理的封装下，可以避免callback hell。

实际上，yield/resume的使用，和c里面使用信号量有点类似
```
local value = nil

async(funciton(state,ret)
    value = {state,ret}
    if state=="case1" then
      event.singal()
    elseif state=="case2" then
      event.singal()
    end
end)

event.waitonce()
if value.state=="case1" then
  process_case1()
  event.waitonece()
end

if value.state=="case2" then
  process_case2()
else
  assert(false)
end

```
当然，这里的event.waitonce()会把整个线程卡死，这个代码需要跑在单独的线程上。
这也是event.waitonce()和coroutine.yiled()的区别的地方，如果上fiber的话，则是一样的。

- 动画更新

一个动画引擎内部有一个死循环，不停的刷帧，在关键时间点上调用外部注入的不同时间点的update方法。
如果不用coroutine的情况下，这样写：
```
function update(time)
  switch(time)
    case time1:load() update1() save() break
    case time2:load() update2() sava() break
    case time3:load() update3() save() break
    defulat: break
  end
end
animat.add(update)
```

但是，如果updatei之间有很多依赖状态，这些中间状态如果都用load(),save()等保存的话，会逐渐变的繁杂。
换用coroutine之后可以是：
```
local co = coroutine.wrap(function(time)
  local error = nil
  if time==time1 then
    update1()
    error,time = coroutine.yield()
  end 

  if time==time1 then
    update1()
    error,time = coroutine.yield()
  end 
  
  if time==time2 then
    update1()
    error,time = coroutine.yield()
  end 

  if time==time3 then
    update1()
    error,time = coroutine.yield()
  end 
end)

function update(time)
  coroutine.resume(co,time)
end

animat.add(update)
```
