
## 服务不可用问题
* 服务雪崩：服务连锁不能响应
* 服务熔断（下游服务不响应时，暂时采用本地实现，过一段时间后放一些试试，可以了不再熔断）
* 服务降级（做一些开关，必要的时候关闭非核心和非必要的服务，采用简化实现）

## 缓存失效问题
* 缓存雪崩：大M个key同时过期，同时去请求数据库；设置过期时间随机解决。
* 缓存击穿：1个热点Key，同时大量请求数据库；设置热点Key不过期解决。
* 缓存穿透：不存在的Key，同时大量请求数据库；严格的参数校验和算法判断大概率不存在的Key过滤解决。

## 流量控制问题
* 令牌桶流量控制算法：定时间隔产生令牌，拿到令牌才请求。平滑流量。
* 漏桶流量控制算法：请求进入漏桶，超过一定容量和时间，益处的请求丢弃。
* 窗口流量控制算法：固定窗口和滑动窗口，类似TCP的拥塞控制。