#### command line 
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

#### script
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

#### remote shell
- security crt
  - file transfer
    - `rz -be`
    - `sz -be`
  - use vba macro

