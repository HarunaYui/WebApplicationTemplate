# TestWebApplication
## 自用.Net8 WebApi模板
### 集成事件回退：
  默认为自动开启,在控制器上标注[NotRateTransaction]可不启用时间回退
### 接口访问限制：
  默认自动开启接口访问限制,时间为1秒,缓存为10秒，在控制器上标注[NotRateTransaction]可不启用访问限制
### 添加JWT身份验证:
  需在Token前添加Bearer
### NLog日志系统：
  日志写在本地项目logs底下
### Fluent数据验证：
  具体见代码
### SignalR通讯：
  （未完成）
