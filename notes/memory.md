#### malloc, calloc, realloc and free

##### 函数原型
[维基百科-C Dynamic memmory allocation](http://en.wikipedia.org/wiki/C_dynamic_memory_allocation)
```
void *malloc(size_t size);
void * calloc(size_t number, size_t size);
void * realloc(void *ptr, size_t size);
void free(void *ptr);
```

##### DESCRIPTION
- The malloc() function allocates size bytes of uninitialized memory.  The allocated space is suitably aligned (after possible pointer coercion) for storage of any type of object.
- The calloc() function allocates space for number objects, each size bytes in length.  The result is identical to calling malloc() with an argument of ``number * size'', with the exception that the allocated memory is explicitly initialized to zero bytes.
- The realloc() function changes the size of the previously allocated memory referenced by ptr to size bytes.  The contents of the memory are unchanged up to the lesser of the new and old sizes.  If the new size is larger, the contents of the newly allocated portion of the memory are undefined.  Upon success, the memory referenced by ptr is freed and a pointer to the newly allocated memory is returned.  Note that realloc() and reallocf() may move the memory allocation, resulting in a different return value than ptr.  If ptr is NULL, the realloc() function behaves identically to malloc() for the specified size.
- The free() function causes the allocated memory referenced by ptr to be made available for future allocations.  If ptr is NULL, no action occurs.

##### 内存分配原理
[猛击这个页面](http://blog.163.com/xychenbaihu@yeah/blog/static/132229655201210975312473/)

##### free(NULL)是合法的  
>Yes, it's safe. The language reference for the c language (and, I believe, c++ language by extension) indicate that no action will be taken.
This is OK in the sense that you can safely (re)free a pointer variable without worrying that it's already been deleted.
That's the language taking care of things just so you don't have to do stuff like
if (x) free(x);
You can just free(x) without worry. 

##### malloc(-1)
此时，由于malloc的参数是size_t，所以-1被转成SIZE_MAX..

##### malloc的时候，core dump
>Note that a crash in malloc or free is usually indicative of a bug which has previously corrupted the heap. 

##### malloc超过128k会发生什么
稍微大一点(通常是超过128k)的内存是通过mmap来分配的，小的内存是在heap上通过brk分配的
1、brk是将数据段(.data)的最高地址指针_edata往高地址推；
2、mmap是在进程的虚拟地址空间中（堆和栈中间，称为文件映射区域的地方）找一块空闲的虚拟内存。

#### MALLOC_CHECK_环境变量
>Recent  versions  of  Linux libc (later than 5.4.23) and GNU libc (2.x)
include a malloc implementation which is tunable via environment  vari-
ables.  When MALLOC_CHECK_ is set, a special (less efficient) implemen-
tation is used which is designed to be tolerant against simple  errors,
such as double calls of free() with the same argument, or overruns of a
single byte (off-by-one bugs).  Not all such errors  can  be  protected
against, however, and memory leaks can result.  If MALLOC_CHECK_ is set
to 0, any detected heap corruption is silently ignored; if set to 1,  a
diagnostic is printed on stderr; if set to 2, abort() is called immedi-
ately.  This can be useful because otherwise a crash  may  happen  much
later,  and  the  true cause for the problem is then very hard to track
down.

#### malloc & HeapAlloc
- [malloc-vs-heapalloc](http://stackoverflow.com/questions/8224347/malloc-vs-heapalloc)
- malloc依赖于CRT，是C标准。HeapAlloc是Windows API，Windows其他语言亦可使用。
- malloc在Windows上可能基于HeapAlloc，在Linux下可能基于sbrk和mmap
- Win16时代，有GlobleAlloc,LocalAlloc
- Win32时代，有HeapAlloc，可以指定在哪个Heap上分配内存，每个进程有一个default Heap，而malloc不能指定堆
- malloc是模块可见度，在一个模块内分配的只能在同一个模块内free

#### VisualAlloc
- VisualAlloc可以用来声明对某块虚拟内存的占用，或者提交
- 被提交过的虚拟内存可以再次被提交

#### Comparing Memory Allocation Methods
The following is a brief comparison of the various memory allocation methods:
- CoTaskMemAlloc
- GlobalAlloc
- HeapAlloc
- LocalAlloc
- malloc
- new
- VirtualAlloc

>Although the GlobalAlloc, LocalAlloc, and HeapAlloc functions ultimately allocate memory from the same heap, each provides a slightly different set of functionality. For example, HeapAlloc can be instructed to raise an exception if memory could not be allocated, a capability not available with LocalAlloc. LocalAlloc supports allocation of handles which permit the underlying memory to be moved by a reallocation without changing the handle value, a capability not available with HeapAlloc.
Starting with 32-bit Windows, GlobalAlloc and LocalAlloc are implemented as wrapper functions that call HeapAlloc using a handle to the process's default heap. Therefore, GlobalAlloc and LocalAlloc have greater overhead than HeapAlloc.
Because the different heap allocators provide distinctive functionality by using different mechanisms, you must free memory with the correct function. For example, memory allocated with HeapAlloc must be freed with HeapFree and not LocalFree or GlobalFree. Memory allocated with GlobalAlloc or LocalAlloc must be queried, validated, and released with the corresponding global or local function.
The VirtualAlloc function allows you to specify additional options for memory allocation. However, its allocations use a page granularity, so using VirtualAlloc can result in higher memory usage.
The malloc function has the disadvantage of being run-time dependent. The new operator has the disadvantage of being compiler dependent and language dependent.
The CoTaskMemAlloc function has the advantage of working well in either C, C++, or Visual Basic. It is also the only way to share memory in a COM-based application, since MIDL uses CoTaskMemAlloc and CoTaskMemFree to marshal memory.

#### COM内存管理规则
- The lifetime management of pointers to interfaces is always accomplished through the AddRef and Release methods found on every COM interface. (See "Reference-Counting Rules" below.)
- The following rules apply to parameters to interface member functions, including the return value, that are not passed "by-value":
    - For in parameters, the caller should allocate and free the memory.
    - The out parameters must be allocated by the callee and freed by the caller using the standard COM memory allocator.
    - The in-out parameters are initially allocated by the caller, then freed and re-allocated by the callee if necessary. As with out parameters, the caller is responsible for freeing the final returned value. The standard COM memory allocator must be used.
- If a function returns a failure code, then in general the caller has no way to clean up the out or in-out parameters. This leads to a few additional rules:
    - In error returns, out parameters must always be reliably set to a value that will be cleaned up without any action on the caller's part.
    - Further, it is the case that all out pointer parameters (including pointer members of a caller-allocate callee-fill structure) must explicitly be set to NULL. The most straightforward way to ensure this is (in part) to set these values to NULL on function entry.
    - In error returns, all in-out parameters must either be left alone by the callee (and thus remaining at the value to which it was initialized by the caller; if the caller didn't initialize it, then it's an out parameter, not an in-out parameter) or be explicitly set as in the out parameter error return case.

#### COM引用计数规则
- Rule 1: AddRef must be called for every new copy of an interface pointer, and Release called for every destruction of an interface pointer, except where subsequent rules explicitly permit otherwise.
The following rules call out common nonexceptions to Rule 1.
    - Rule 1a: In-out-parameters to functions. The caller must AddRef the actual parameter, since it will be Released by the callee when the out-value is stored on top of it.
    - Rule 1b: Fetching a global variable. The local copy of the interface pointer fetched from an existing copy of the pointer in a global variable must be independently reference counted, because called functions might destroy the copy in the global while the local copy is still alive.
    - Rule 1c: New pointers synthesized out of "thin air." A function that synthesizes an interface pointer using special internal knowledge, rather than obtaining it from some other source, must do an initial AddRef on the newly synthesized pointer. Important examples of such routines include instance creation routines, implementations of IUnknown::QueryInterface, and so on.
    - Rule 1d: Returning a copy of an internally stored pointer. After the pointer has been returned, the callee has no idea how its lifetime relates to that of the internally stored copy of the pointer. Thus, the callee must call AddRef on the pointer copy before returning it.

- Rule 2: Special knowledge on the part of a piece of code of the relationships of the beginnings and the endings of the lifetimes of two or more copies of an interface pointer can allow AddRef/Release pairs to be omitted.
    - From a COM client's perspective, reference-counting is always a per-interface concept. Clients should never assume that an object uses the same reference count for all interfaces.
    - The return values of AddRef and Release should not be relied upon, and should be used only for debugging purposes.
    - Pointer stability; see details in the OLE Help file under "Reference-Counting Rules," subsection "Stabilizing the this Pointer and Keeping it Valid."

#### references
- [The memory models that underlie programming languages](http://canonical.org/~kragen/memory-models/)
