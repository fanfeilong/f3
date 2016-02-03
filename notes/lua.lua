
------------------------------------------------
--// using coroutine to synchronize callback
------------------------------------------------
local post = coroutine.wrap(function(arg)
  local connection = ...
  
  local cookie = listen(connection,arg)
  connection:BeginOpen()

  local error,r = coroutine:yield()
  if r.state=="open" then
      local pkg = r.value
	  connection:PostPackage(pkg)
      coroutine:yield()
  elseif r.state=="close" then
      connection:RemoveStateChangeListener(cookie)
      connectionManager:Remove(cookie)
  else 
      assert(false)
  end
end)

function listen(connection,v)
  local self = coroutine.running()
  assert(self)
  
  local cookie = connection:AttachStateChangeListener(function(connection_,oldState,newState,errorCode)
    if newState=="connected" then
	     coroutine.resume(self({state="open",value=v}))			
    elseif newState=="closed" then
	     coroutine.resume(self({state="close"}))
    end
  end)

  return cookie
end

post(pkg)
