## gdb
---------------
+ [debugging with gdb](http://sourceware.org/gdb/current/onlinedocs/gdb/)
  - [A Sample gdb Session](https://sourceware.org/gdb/current/onlinedocs/gdb/Sample-Session.html#Sample-Session)
    - start 
      - `gdb program` 
      - `gdb program core_dump`
      - `gdb -p processid`
    - break and step into
      - `break functionname`
      - `n`(next, F10 for visualstudio)
      - `s`(next.step, F11 for visualstudio)
      - `bt`(summary of the stack)
        - `f i` : goto the stack frame at i level
      - `p(print) variable`
      - `l`(list ten lines of source surrrouding the current line)
      - `c`(continue, F5 for visualstudio)
    - end
      - `Ctrl+d` : giving it an EOF as input to interrupt program
      - `quit` : exit gdb
  - [gdb with lua](https://github.com/mkottman/lua-gdb-helper)
    - `wget https://github.com/mkottman/lua-gdb-helper/blob/master/luagdb.txt`
    - gdb to find the lua runtime thread
    - `source luagdb.txt`
    - `luatrace`
+ coredump
  - `cat /proc/sys/kernel/core_pattern`

## trace
---------
- strace program, [history of strace](https://en.wikipedia.org/wiki/Strace)
- dtrace, [dtrace4linux](https://github.com/dtrace4linux/linux)
- pstack processid

## path
---------
- add current directory to library search path: `export LD_LIBRARY_PATH=$LD_LIBRARY_PATH:$PWD`

## iptable
---------
+ chkconfig
  - chkconfig iptables on
  -  chkconfig iptables off
+ restart 
  - service iptables stop
  - service iptables start
  - service iptables restart
+ modify
  - `vim /etc/sysconfig/iptables`
  - `-A RH-Firewall-1-INPUT -m state --state NEW -m multiport -p tcp --dport from:to -j ACCEPT` 
  - `-A RH-Firewall-1-INPUT -m state --state NEW -m multiport -p udp --dport from:to -j ACCEPT`

## tcp
---------
+ reuse tcp port: 
  - `vim /etc/sysctl`
  - `net.ipv4.tcp_tw_reuse = 1`    
  - `net.ipv4.tcp_tw_recycle = 1`  
  - `net.ipv4.tcp_fin_timeout = 30` 
  - `sysctl -p`
+ tcpdump
  - `tcpdump -i eth1 -vnn udp and host destip -w output.cap`

## shell
-----------------
- short path in shell: `export PS1='[\u@\h \W]\$ '`
- change file permission:`su chmod 777 file`
- enable excute:`su chmod +x file.sh`
- [wiki:chmod](http://en.wikipedia.org/wiki/Chmod)
- view so info
  - `readelf -a file.so #show all information` 
  - `readelf -d file.so #show dependence so`
  - `readelf -h file.so #show header information`
- find and kill process, for example, when device busy
  - `ps aux | grep tun0`
  - `kill pid`
- create a TUN interface
  ```
  echo 1 > /proc/sys/net/ipv4/ip_forward
  iptables -t nat -A POSTROUTING -s 10.0.0.0/8 -o eth1 -j MASQUERADE
  
  # IF ubuntu
  # ip tuntap add dev tun0 mode tun
  
  # IF centos, yum install tunctl, then
  tunctl -p -t br0p0
  tunctl -n -t tun0
  
  ifconfig tun0 10.0.0.1 dstaddr 10.0.0.2 up
  ```
- Sed 
  - [An Introduction and Tutorial by Bruce Barnett](http://www.grymoire.com/Unix/sed.html)
- Grep 
  - [An introduction to grep and egrep. - by Bruce Barnett](http://www.grymoire.com/Unix/Grep.html)
- Awk 
  - [Awk - A Tutorial and Introduction - by Bruce Barnett](http://www.grymoire.com/Unix/Awk.html)
  - [Awk in 20 minutes](http://ferd.ca/awk-in-20-minutes.html)

- count file 
  - `ll -t|grep -|wc -l`

## script
---------
- [passing arguments to a shell script](http://osr600doc.sco.com/en/SHL_automate/_Passing_to_shell_script.html)
- [if then else fi](http://www.dreamsyssoft.com/sp_ifelse.jsp)
- tell linux which program to run script:`#! /bin/sh`
- array: `array=('one' 'two')`
- loop array:`for e in @{arrary[@]} do echo e done` 
  `for i in {5..10}; do echo $i; done`  `for i in $(seq 10); do echo $i; done`
- copy:`cp a b`
- force remove file:`rm -f file`
- if else:  `if [ "$1""x" = "publicx" ]; then else fi`
- [if else tutorial](http://www.dreamsyssoft.com/unix-shell-scripting/ifelse-tutorial.php)

## remote shell
----------
- security crt
  - file transfer
    - `rz -be`
    - `sz -be`
  - use vba macro