#### Basic
- [Tutorials about development for Android.](http://www.vogella.com/tutorials/android.html)

#### Tips
- HashMap<int,xxx>，HashMap<long,xxx> are both invalid, because HashMap only accept key type that implements Comparable<K> interface
  What's the Fuck!!! we must use HashMap<Interger,xxx> and HashMap<Long,xxx>. Java also provide another generic collection to solve this   problem: LongSparseArray<E>.

#### NDK
- meta(important!!)
  - keep ndk version is the same as android os version, which will avoid lots exceptions
  - any assert code will lead to segment fault error when call java `System.loadlibary() `and excute jni functions
- tools
  - [os-windows: cygwin](https://cygwin.com/install.html)
  - [androik-ndk](http://developer.android.com/tools/sdk/ndk/index.html)
  - [android-ndk-googlegroup](https://groups.google.com/forum/#!forum/android-ndk)
  - [ndk-build](http://www.kandroid.org/ndk/docs/NDK-BUILD.html)
  
  >'ndk-build' itself is a tiny wrapper around GNU Make, its purpose is simply to invoke the right NDK build script, it is equivalent to;
   ```$GNUMAKE -f $NDK/build/core/build-local.mk [parameters]```
   Where '$GNUMAKE' points to GNU Make 3.81 or later, and $NDK points to your NDK installation directory.
  
  - [adb/push/pull](http://www.droidviews.com/push-pull-files-android-using-adb-commands/)
  location at `sdk\platform-tools`
    - push multifiles to android in windows:`for %i in (*.so) do adb push %i /system/lib/`
- jni
  - [jni-tutorial](http://www3.ntu.edu.sg/home/ehchua/programming/java/JavaNativeInterface.html)
  - [jni-tips](http://developer.android.com/training/articles/perf-jni.html)
  - [introduce-to-android-jni-1](http://dotnetslackers.com/articles/net/Introduction-to-Android-JNI-development-Using-NDK-Part-1.aspx)
  - [introduce-to-android-jni-2](http://dotnetslackers.com/articles/net/Introduction-to-Android-JNI-Development-Using-NDK-Part-2.aspx)
  - [jni-functions](http://docs.oracle.com/javase/1.5.0/docs/guide/jni/spec/functions.html)
  - [Java Native Interface Programming](http://journals.ecs.soton.ac.uk/java/tutorial/native1.1/implementing/index.html)
  - [Debugging Android native shared libraries](http://blog.dornea.nu/2015/07/01/debugging-android-native-shared-libraries/)
  - build
    1. create a new android project in eclipse
    2. add jni directory
    3. add Application.mk
      - add `APP_PROJECT_PATH := $(call my-dir)` line
    4. add Android.mk
      - add`LOCAL_PATH := $(call my-dir)`
      - add`JNI_PATH := $(APP_PROJECT_PATH)/jni` because sometimes ndk has bug that `LOCAL_PATH` is incorrect, we use custome `JNI_PATH`

#### Build Android JNI with Eclipse in windows, Step by Step.
- Download 
  - adt-bundle-windows which include
    - eclispe
    - sdk
    - SDK Mangaer.exe
  - android-ndk-rxy(where xy is version number)
  - Java SDK
  - after this step, the tools directory is:
  
  ```
  d:\android
    elipse
    sdk
    ndk
      ndk-build.cmd
    SDK Manager.exe
  ```

- Create Project, name as hellojni
  - Create New Android Project in Eclispe
  - Create package to declare native methods like

  ```
  package com.xxx.yyy
  class ZZZ{
    static{
      System.loadLibrary("hello")
    }
    public native int echo(String msg);
  }
  
  ```

  - Create jni subdirectory in the project:

  ```
  hellojni
    jni
      inc 
      lib
      src    
  ```
  then, add c/c++ header file in inc directory, add native so files in lib directory 

  - Use the following command to generate jni header files:

  ```javah -d jni/src -classpath src com.xxx.yyy.ZZZ```
  which will gen
  
  ```
  hellojni
    jni
      inc 
      lib
      src    
        com_xxx_yyy_ZZZ.h
  ```  
  
  - Create com_xx_yyy_ZZZ.cpp file in jni/src, implement the jni functions
  - Create an Application.mk file in hellojni directory, and add text:
  
  ```
  APP_ABI := armeabi
  APP_PLATFORM := android-7
  APP_PROJECT_PATH := $(call my-dir)
  ```
  
  - Create an Android.mk file in hellojni/jni/ directory, and add text:
  
  ```
  JNI_PATH := ${local} #this variable is define in elipse builder Environment tab
  
  # include native so
  include $(CLEAR_VARS)
  LOCAL_MODULE := somenative
  LOCAL_SRC_FILES := $(JNI_PATH)/lib/somenative.so
  include $(PREBUILT_SHARED_LIBRARY)
  
  include $(CLEAR_VARS)
  LOCAL_MODULE := hello
  LOCAL_C_INCLUDES := $(JNI_PATH)/inc 
  LOCAL_SRC_FILES := $(JNI_PATH)/src/com_xxx_yyy_ZZZ.cpp

  LOCAL_SHARED_LIBRARIES += somenative
  LOCAL_LDLIBS += -lz -llog
  LOCAL_CFLAGS += -O2 -MD -DLINUX -D_ANDROID_LINUX -DMOBILE_PHONE -DDOWNLOAD_PLATFORM_PROJ_BT_EMULE
  include $(BUILD_SHARED_LIBRARY)
  
  ``` 
  
  - Create Eclispe NDK_Builder
    - Open Project Properties dialog in eclipse
    - Goto Builder menu
    - Create a new Builder, Name as 'NDK_Bulder'
    - Edit NDK_Builder
    - Select [Main] Tab
      - Set Location as 'd:\android\ndk\ndk-build.cmd'
      - Set Working Directory as '${workspace_loc:/hellojni}'
    - Select [Refresh] Tab
      - Check "Refresh resource upon completion"
      - Check "The entire workspace"
      - Check "Recuresively include sub-foulders"
    - Select [Build options] Tab
      - Check "After a 'Clean'"
      - Check "During manual builds"
      - Check "During auto builds"
      - Check "Specify working set of relevant resources"
    - Select [Environment] Tab
      - Add a new variable, Name:local Value:${workspace_loc:/hellojni/jni}

  - Back to Eclipse, Build Project.  
 
#### JNI Signatures
- Type System

| Java Type  | JNI Type    |JNI ArrayType | C Type       | C99 Type     |JNI Signatures    |JNI Array Signatures    |
|:----------:|:-----------:|:------------:|:------------:|:------------:|:------------:|:------------------:|
| boolean    | jboolean    | jbooleanArray|unsigned char |uint8_t       |Z             |[I                  |
| byte       | jbyte       | jbyteArray   |signed char   |int8_t        |B             |[B                  |
| char       | jchar       | jcharArray   |unsigned short|uint16_t      |C             |[C                  |
| short      | jshort      | jshortArray  |short         |int16_t       |S             |[S                  |
| int        | jint        | jintArray    |int           |int32_t       |I             |[I                  |
| long       | jlong       | jlongArray   |long          |int64_t       |J             |[J                  |
| float      | jfloat      | jfloatArray  |float         |float         |F             |[F                  |
| double     | jdouble     | jdoubleArray |double        |double        |D             |[D                  |
| void       | void        |    x         |void          |void          |V             |                    |

- Example
  1. `boolean isLenOn(void)` `->` `()Z`
  2. `void setLedOn(int lenNo)` `->` `(I)`
  3. `String substr(String str,int idx,int count)` `->` `(Ljava/lang/String;II)Ljava/lang/String`

##### Reference Types
  - jobject (all java objects)
    - jclass (java.lang.Class objects)
    - jstring (java.lang.String objects)
    - jarray  (arrays)
      - jobjectArray (object arrays)
      - jbooleanArray (boolean arrays)
      - jbyteArray (byte arrays)
      - jcharArray (char arrays)
      - jshortArray (short arrays)
      - jintArray (int arrays)
      - jlongArray (long arrays)
      - jfloatArray (float arrays)
      - jdoubleArray (double arrays)
    - jthrowable (java.lang.Throwable objects)

##### Native Built in Type
- jobject/jclass 
  - in c program language:

  ```
  typedef jobject jclass;
  ```

  - in c++ program language

  ```
  class _jobject{};
  class _jclass : public _jobject{};
  ...
  typedef _jobject* jobject;
  typedef _jclass* jclass;
  ```

- field/method ids

```
struct _jfieldID;
typedef struct _jfieldID* jfieldID;
struct _jmethodID;
typedef struct _jmethodID* jmethodID;
```

- Value Type

```
typedef union jvalue {
  jboolean z;
  jbyte b;
  jchar c;
  jshort s;
  jint i;
  jlong j;
  jfloat f;
  jdouble d;
  jobject l;
}
```


- jni 注册实例
  - [com.android.server.connectivity.Vpn](https://android.googlesource.com/platform/frameworks/base/+/android-4.4_r1/services/jni/com_android_server_connectivity_Vpn.cpp)
 
#### Use gdb and gdbserver to debug your native program in android
1. connect your android to your computer
    - assum that the android ip is: 192.168.91.100
    - assum that the computer ip is: 192.168.91.101
2. push gdbserver into android 
     ```adb push $ANDROID_NDK_ROOT/prebuilt/android-arm/gdbserver/gdbserver /data```
3. push all native program into android    
    ```adb push ./app/bin/debug/xxx /data/tmp```
    where `./app/bin/debug/xxx` is your program file
    where `/data/tmp` is directory in android
4. pull android lib to local, which will be used in gdbserver
    ```adb pull /system/lib ./debugging/lib```
    ```adb pull /system/bin/linker ./debugging/lib```
5. run app in android
    ```adb shell gdbserver :12345 /data/tmp/app```
6. run ndk's gdb in your computer by the following command
    ```
    $ANDROID_NDK/toolchains/arm-linux-androideabi-4.4.3/prebuild/darwin-x86/bin/arm-linux-androideabi- gdb
    ```
    then you will enter the gdb interact enviroment, go to next step
7. tell gdb where to find your so lib
    ```(gdb) set solid-serarh-path ./debugging/lib```
8. tell gdb the native program you'd like to debug
    ```(gdb) file ./app/bin/debug/xxx```
    then gdb will load the symbols from so files
9. connect to gdbserver which is running on the android 
    ```(gdb) target remote 192.168.91.100:12345``` 
10. begin to debug your program with gdb, for example
    ```(gdb) b main```
    ```(gdb) c```
    where command `b` is used to set break point
    where command `c` is tell gdb to continue 
11. that's all, maybe you should learn more gdb command from manual, and gdbserver is just a spy in android to communication with gdb in your computer.
