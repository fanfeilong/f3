<hr/>
**控制结构目录**：[1](http://www.cnblogs.com/math/p/control-structure-001.html) [2](http://www.cnblogs.com/math/p/control-structure-002.html) [3](http://www.cnblogs.com/math/p/control-structure-003.html) [4](http://www.cnblogs.com/math/p/control-structure-004.html) [5](http://www.cnblogs.com/math/p/control-structure-005.html)
<hr/>

基于语言提供的基本控制结构，更好地组织和表达程序，需要良好的控制结构。

## 前情回顾
上一次，我们说到状态机结构(state machine)，事实上，编程语言中有很多所谓的“高级”控制结构，内部都是状态机结构，差别只在于，这是编程语言内置提供的还是外置类库做。后面我们会有机会一点点展开它们，状态机的原理简洁，理解了它，再去理解那些高级控制结构的时候，才会觉的自然。再次回顾下状态机的原理：

- define states
- function `dowork` by switch states
- in branch methods or events
    - check state
    - do real work
    - change state
    - reentry `dowork` function

## 典型代码

#### `try-catch` 情景

```javascript
function loadPackage(packageName){
	try{
		let packageInfo = getPackageInfo(packageName);
	    let configPath = packageInfo.path + '/config.json';
	    let package = JSON.parse(fs.readFileSync(configPath));
	    return package;
   	}catch(err){
        return null;
    }
}
```

#### `lock` 情景

```c++
typedef std::map<K*,V*> Dict;
Dict* dict;
Lock* lock;

int Add(K* key, V* value){
	lock->lock();
	Dict::iterator it = dict.find(key);
	if (it == Dict.end()){
		key->AddRef();
		value->AddRef();
		dict.insert(std::make_pair(key, value));
	}
	lock->unlock();
	return RESULT.SUCCESS;	
}

int Remove(K* key){
	lock->lock();
	K* oldKey = NULL;
	V* oldValue = NULL;

	Dict::iterator it = dict.find(key);
	if (it != dict.end()){
		oldKey = it->first;
		oldValue = it->second;
		dict.erase(it);
	}

	if (oldKey){
		oldKey->Release();
	}

	if (oldValue){
		oldValue->Release();
	}

	lock->unlock();

	return RESULT.SUCCESS;
}

V* Find(K * key){
	lock->lock();

	K* oldKey = NULL;
	V* oldValue = NULL;

	dict::iterator it = dict.find(pDataBlock);
	if (it != dict.end())
	{
		oldKey = it->first;
		oldValue = it->second;	
	}

	oldValue->AddRef();

	lock->unlock();
	return oldValue;
}

```

## 结构分析

上述两个示例代码，一个用到了try-catch，另一个用到了lock。例子里的代码不复杂，但是在编程中，也是一种常见的问题。那就是try-catch和lock的作用域是在一整段函数范围内都起作用。我们先缩小下范围：

#### `try-catch` 情景

```javascript
function loadPackage(packageName){
	let packageInfo = getPackageInfo(packageName);
	let configPath = packageInfo.path + '/config.json';

	try{
	    let package = JSON.parse(fs.readFileSync(configPath));
	    return package;
   	}catch(err){
        return null;
    }
}
```

#### `lock` 情景

```c++
typedef std::map<K*,V*> Dict;
Dict* dict;
Lock* lock;

int Add(K* key, V* value){
	lock->lock();
	Dict::iterator it = dict.find(key);
	if (it == Dict.end()){
		key->AddRef();
		value->AddRef();
		dict.insert(std::make_pair(key, value));
	}
	lock->unlock();
	return RESULT.SUCCESS;	
}

int Remove(K* key){
	
	K* oldKey = NULL;
	V* oldValue = NULL;

	// lock scope
	{
		lock->lock();
		Dict::iterator it = dict.find(key);
		if (it != dict.end()){
			oldKey = it->first;
			oldValue = it->second;
			dict.erase(it);
		}
		lock->unlock();
	}
	
	if (oldKey){
		oldKey->Release();
	}

	if (oldValue){
		oldValue->Release();
	}

	return RESULT.SUCCESS;
}

V* Find(K * key){
	

	K* oldKey = NULL;
	V* oldValue = NULL;

	{
		lock->lock();
		dict::iterator it = dict.find(pDataBlock);
		if (it != dict.end())
		{
			oldKey = it->first;
			oldValue = it->second;	
		}
		lock->unlock();
	}
		
	oldValue->AddRef();

	return oldValue;
}

```


## 语义分析

在上述两种情景下，关键是要区分出：“一个保护范围的目标是什么”。

try-catch只需要捕获文件读写和Json转换的异常，而不应该把其他代码包含进来。特别是getPackageInfo是一个函数调用，一个函数调用里面什么都可能发生，不应该被语义不明确地包含在此处的异常捕获里。

lock则要明确保护的是`数据`还是`行为`。此处lock应该保护的是dict容器在多线程里读写的互斥，所以不应该把除了对dict操作的其他逻辑包含进去。特别是`oldKey->Release(), oldValue->Release()`是函数调用，里面什么都可能发生，不应该被语义不正确地包含在此处的加锁范围内。







