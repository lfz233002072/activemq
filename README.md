高性能消息监控无服务器，使用ActiveMQ消息中间件实现，包括windows服务、控制台启动等多种启动方式

### 实现功能

1、消息订阅与发布
2、消息集群管理
3、Redis缓存管理
4、自动更新实现
5、在线客户端登记、查看


### 使用技术

1、activemq （Apache.NMS.ActiveMQ 1.7）
2、redis （ServiceStack.Redis 3.9）
3、Nlog 3.1
4、SharpCompress 1.2
5、Json.NET 6.0
6、NHibernate 4.0


使用Vistor模式处理客户端与服务器业务处理，消息以封装未不同的命令
