network notes

- [Computer architecture for network engineers](https://github.com/lukego/blog/issues/18)
- [How to transfer large amounts of data via network](http://moo.nac.uci.edu/~hjm/HOWTO_move_data.html)

## standard protocols
-----------------

#### core concept
- Network Layer
  * OSI 7 Layer Model:     
  `Application` `>` `Presentation` `>` `Session` `>` `Transport` `>` `Network`  `>` `DataLink` `>` `Physical`
  * Internet 4 Layer Model:     
    `Application` `>` `Transport` `>` `Network` `>` `Data Link`
    * Application
      * `HTTP` `HTTPS` `FTP` `DNS` `SSH` `SSL`
        * TCP Example:`(Ethernet(IP(TCP(HTTP)))`
        * UDP Example:`(Ethernet(IP(UDP(FTP))))`
		  * MAIL Protocol: `SMTP` `IMAP` `POP3`
		* `Telnet`
    * Transport: `TCP` `UDP` 
	  * Network: `IP` `ICMP` `IGMP` `ARP` `BGP` `DHCP`
	  * Data Link: Ethernet `PPP` `ADSL` `Optical-fiber`

- Network Topology
  * Topology Types:     
    `Point-to-Point` `Bus` `Ring` `Star` `Mesh(Full Connected,Partial Connected)` `Tree` `Hybrid` `Diasy` `chain`
  * Tables:     
    `Mac Table` `Arp Table` `Route Table`
  * [Windows操作系统路由表完全解析](http://vod.sjtu.edu.cn/help/Article_Print.asp?ArticleID=1948)
  * [详解网络传输中的三张表，MAC地址表、ARP缓存表以及路由表](http://dengqi.blog.51cto.com/5685776/1223132)
  * [wiki:路由表](http://zh.wikipedia.org/wiki/%E8%B7%AF%E7%94%B1%E8%A1%A8)

- Types OF Networks
  * `LAN`  Local Area Network
  * `WLAN` Wireless Local Area Network
  * `WAN`  Wide Area Network
  * `MAN`  Metropo

- NAT(Net Address Translate)
  * Con NAT
    * Full Cone NAT
    * Restricted Cone NAT
    * Port Restricted NAt
  * Symmetric NAT  

- Socket 
  - BSD Socket
    * server: bind listen accept read write
    * client: connect write read close
  - WinSocket 

- TCP Handshake
  1. Host A sends a TCP SYNchronize packet to Host B
  2. Host B receives A's SYN
  3. Host B sends a SYNchronize-ACKnowledgement
  4. Host A receives B's SYN-ACK
  5. Host A sends ACKnowledge
  6. Host B receives ACK.
  7. TCP socket connection is ESTABLISHED.
- TCP Retransmission
  - http://www.saminiir.com/lets-code-tcp-ip-stack-5-tcp-retransmission/

- QUIC(Quick Udp Internet Connection)
  - https://www.chromium.org/quic
  - https://docs.google.com/document/d/1gY9-YNDNAB1eip-RTPbqphgySwSNSDHLq9D5Bty4FSU/edit
  - https://docs.google.com/document/d/1WJvyZflAO2pq77yOLbp9NsGjC1CHetAXV8I0fQe-B_U/edit#
  - https://docs.google.com/document/d/1F2YfdDXKpy20WVKJueEf4abn_LVZHhMUMS5gX6Pgjl4/edit
  - https://tools.ietf.org/html/draft-tsvwg-quic-protocol-02

#### references
- [Network protocolsFor programmers who know at least one programming language](https://www.destroyallsoftware.com/compendium/network-protocols?share_key=97d3ba4c24d21147)
- [What is the Internet?](http://www.inetdaemon.com/tutorials/internet/index.shtml)
- [RFC: A TCP/IP Tutorial](http://www.rfcreader.com/#rfc1180)
- [wiki:互联网工程任务组（IETF）发布的征求修正意见书（RFC）](http://zh.wikipedia.org/wiki/RFC)
- [wiki:网络地址转换（NAT）](http://zh.wikipedia.org/wiki/网络地址转换)
- [wiki:NAT穿透](http://zh.wikipedia.org/wiki/NAT穿透)
- [wiki:域名系统/动态域名服务（DNS）](http://zh.wikipedia.org/wiki/域名系统)
- [wiki:网络传输协议](http://zh.wikipedia.org/wiki/網絡傳輸協議)
- [wiki:TCP/IP协议](http://zh.wikipedia.org/wiki/TCP/IP协议)
- [wiki:TCP拥赛控制](https://en.wikipedia.org/wiki/TCP_congestion_control)
- [wiki:OSI7层模型](http://zh.wikipedia.org/wiki/OSI模型)
- [wiki:超文本传输协议（HTTP）](http://zh.wikipedia.org/wiki/HTTP)
- [wiki:HTTPS协议](http://zh.wikipedia.org/wiki/HTTPS)
- [wiki:Goolge-SPDY协议](http://zh.wikipedia.org/wiki/SPDY)
- [wiki:DHCP](http://zh.wikipedia.org/wiki/DHCP)
- [wiki:Wi-Fi](http://zh.wikipedia.org/wiki/Wi-Fi)
- [wiki:地址解析协议（ARP）](http://zh.wikipedia.org/wiki/地址解析协议)
- [wiki:ARP欺騙](http://zh.wikipedia.org/wiki/ARP欺騙)
- [wiki:分散式阻斷服務攻擊（DDoS）](http://zh.wikipedia.org/wiki/分散式阻斷服務攻擊)
* [IPV4Packet](http://en.wikipedia.org/wiki/IPv4)
* [TCPPacket](http://en.wikipedia.org/wiki/Transmission_Control_Protocol)
  * [PUSH-ACK](https://ask.wireshark.org/questions/20423/pshack-wireshark-capture) 
* [HTTPPacket](http://en.wikipedia.org/wiki/Hypertext_Transfer_Protocol)
* [HTTP state codes](http://en.wikipedia.org/wiki/List_of_HTTP_status_codes)
* [Understanding tcp sequence acknowledgment numbers](http://packetlife.net/blog/2010/jun/7/understanding-tcp-sequence-acknowledgment-numbers/)
* [The TIME-WAIT state in TCP and Its Effect on Busy Servers](http://www.isi.edu/touch/pubs/infocomm99/infocomm99-web/)
* [How to Calculate IP/TCP/UDP Checksum–Part 1 Theory](http://www.roman10.net/how-to-calculate-iptcpudp-checksumpart-1-theory/)
* [Introduce to HyperText Transfer Protocol ](http://www.ntu.edu.sg/home/ehchua/programming/webprogramming/HTTP_Basics.html) 
* [CoolShell:TCP/IP那些事儿（上）](http://coolshell.cn/articles/11564.html) 
* [CoolShell:TCP/IP那些事儿（下）](http://coolshell.cn/articles/11609.html)
* [网络基本功（一）：细说网络传输](https://community.emc.com/thread/197851)
* [协议森林](http://www.cnblogs.com/vamei/tag/%E7%BD%91%E7%BB%9C/)
* [15 New TCP Implements](http://intronetworks.cs.luc.edu/current/html/newtcps.html)
* [Raw socket, Packet socket and Zero copy networking in Linux](http://yusufonlinux.blogspot.jp/2010/11/data-link-access-and-zero-copy.html)
* [using sockets rather like using files](http://www.cplusplus.com/forum/general/58677/)
* [How$speedy$is$SPDY?](https://www.usenix.org/sites/default/files/conference/protected-files/nsdi14_slides_wang.pdf)
* [Do routers verify UDP and TCP checksums?](http://serverfault.com/questions/644289/do-routers-verify-udp-and-tcp-checksums)
* [UDP vs TCP](http://gafferongames.com/networking-for-game-programmers/udp-vs-tcp/)
* [Characteristics of UDP Packet Loss: Effect of TCP Traffic](http://www.isoc.org/INET97/proceedings/F3/F3_1.HTM)
* [Exploiting the Linux kernel via packet sockets](https://googleprojectzero.blogspot.jp/2017/05/exploiting-linux-kernel-via-packet.html)

## operations

- 操作+计时器：
  - 提交一个操作
  - 开始一个计时器，用以检查是否超时或者用以重试操作。
- ping操作：an udp pingclient SHOULD contains two circle:
  - out circle to rebind socket.
  - inner circle to resend pingpackage using timer before response package has received.
  
## P2P
------------

#### PunchHole

A,B,SA,SB where SA and SB is A and B's SuperNode

synchonize:

 1. A send syn to B
 2. A send sncall to SA
 3. SA send sncallresp to A
 4. SA forward sncallresp to SB
 5. SB send sncalled to B
 6. B send sncalledresp to SB
 7. B send ack to A

concurrency:

 8. A receive ack from B
 8. B receive syn from A 

synchonize:

10. A send ackack to B
11. A enter establish state

12. B receive ackack from A
13. B enter establish state

Note that A may have multi local ip, A can bind to multi ip and then has multisockets,
if A has two difference addresses for difference IPS, for example ip1 and ip2, then 
A should bind two socket to ip1 and ip2, name as socket1 and socket2.

Then, when B send ack to socket1, A SHOULD send ackack and continues packages to B
by socket1, NOT the socket2.

#### references
- Basic
  - [wiki:peer 2 peer](http://en.wikipedia.org/wiki/Peer-to-peer)
  - [bittorrent protocol](http://www.bittorrent.org/beps/bep_0005.html)
  - [wiki:DHT](http://en.wikipedia.org/wiki/Distributed_hash_table)
  - [p2p NAT](http://www.brynosaurus.com/pub/net/p2pnat/)
- Freedomlayer
  - [Intro to the Internet and current issues](http://www.freedomlayer.org/articles/intro_internet.html)
  - [The Mesh Question](http://www.freedomlayer.org/articles/mesh_question.html)
  - [Intro to Distributed Hash Tables (DHTs)](http://www.freedomlayer.org/articles/dht_intro.html)
  - [Stabilizing Chord](http://www.freedomlayer.org/articles/chord_stabilize.html)
  - [Basic DHT security concepts](http://www.freedomlayer.org/articles/dht_basic_security.html)
  - [Sqrt(n) mesh routing](http://www.freedomlayer.org/articles/dht_basic_security.html)
  - [Experimenting with Virtual DHT Routing](http://www.freedomlayer.org/articles/exp_virtual_dht_routing.html)
  - [The Distributed Post Office: Instant hierarchy for mesh networks](http://www.freedomlayer.org/articles/dist_post_office.html)
- P2P Network
  - [P2P-01-Introduce](http://www.cs.helsinki.fi/u/jakangas/Teaching/PrintOuts/08s-P2P-01-Introduction.pdf)
  - [P2P-02-Systems](http://www.cs.helsinki.fi/u/jakangas/Teaching/PrintOuts/08s-P2P-02-Systems.pdf)
  - [P2P-03-Net+DHTs](http://www.cs.helsinki.fi/u/jakangas/Teaching/PrintOuts/08s-P2P-03-Net+DHTs.pdf)
  - [P2P-04-Storage](http://www.cs.helsinki.fi/u/jakangas/Teaching/PrintOuts/08s-P2P-04-Storage.pdf)
  - [P2P-05-Reliablility](http://www.cs.helsinki.fi/u/jakangas/Teaching/PrintOuts/08s-P2P-05-Reliability.pdf)
  - [P2P-06-ContentDistribution](http://www.cs.helsinki.fi/u/jakangas/Teaching/PrintOuts/08s-P2P-06-ContentDistribution.pdf)
  - [P2P-07-Issues](http://www.cs.helsinki.fi/u/jakangas/Teaching/PrintOuts/08s-P2P-07-Issues.pdf)
  - [P2P-08-Consession](http://www.cs.helsinki.fi/u/jakangas/Teaching/PrintOuts/08s-congestion-intro.pdf)
- P2P Application
  - [crawl in dht - zhcn](http://codemacro.com/2013/05/19/crawl-dht/)

## Congestion Controller
---------------------

#### understanding

- send timer
  - send packages
    - with timer
      - timout 
        - resend
        - bad feedback
- feedback
  - update history data
  - filter history data
  - do cc strategy
  	- good feedback -> award 
      - slow start, inc window or speed by step
      - congestion avoid, inc window or speed with damping
    - bad feedback  -> punish
      - fast resend packages
      - dec window or speed by means of discount
  - send packages

#### references
- [BBR:Measuring bottleneck bandwidth and round-trip propagation time](http://queue.acm.org/detail.cfm?id=3022184)
- [DCCP: The socket type you probably never heard of](https://www.anmolsarma.in/post/dccp/)

## FQA
------------
- [How to get udp socket port when bind to 0](http://stackoverflow.com/questions/1075399/how-to-bind-to-any-available-port):
- [How Does 127.0.0.1 Work?](http://www.tech-faq.com/127-0-0-1.html)


## Tools
- [A tcpdump Tutorial and Primer with Examples](https://danielmiessler.com/study/tcpdump/)




