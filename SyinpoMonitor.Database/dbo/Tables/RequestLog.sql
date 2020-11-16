CREATE TABLE [dbo].[RequestLog]
(
	[Id] INT NOT NULL PRIMARY KEY IDENTITY,
    [TraceId] NVARCHAR(500) NOT NULL,
    [RemoteIpAddress] NVARCHAR(500) NOT NULL,
    [RemotePort] INT NOT NULL,
    [IdentityIsAuthenticated] BIT NOT NULL,
    [IdentityName] NVARCHAR(500) NULL,
    [RequestMethod] NVARCHAR(500) NULL,
    [RequestScheme] NVARCHAR(500) NULL,
    [RequestPath] NVARCHAR(MAX) NULL,
    [RequestQueryString] NVARCHAR(MAX) NULL,
    [RequestContentType] NVARCHAR(MAX) NULL,
    [RequestContentLength] BIGINT NULL,
    [RequestHost] NVARCHAR(MAX) NULL,
    [RequestHead] NVARCHAR(MAX) NULL,
    [RequestBody] NVARCHAR(MAX) NULL,
    [RequestTime] DATETIME NOT NULL,
    [RequestType] INT NOT NULL,
    [RequestGroup] BIGINT NOT NULL,
	[Operation]  NVARCHAR(MAX) NULL,
	[ServerName]  NVARCHAR(500) NULL,
    [CreateTime] DATETIME NOT NULL
)

GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'主键',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'RequestLog',
    @level2type = N'COLUMN',
    @level2name = N'Id'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'跟踪标识',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'RequestLog',
    @level2type = N'COLUMN',
    @level2name = N'TraceId'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'客户端IP',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'RequestLog',
    @level2type = N'COLUMN',
    @level2name = N'RemoteIpAddress'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'客户端连接端口',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'RequestLog',
    @level2type = N'COLUMN',
    @level2name = N'RemotePort'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'是否授权',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'RequestLog',
    @level2type = N'COLUMN',
    @level2name = N'IdentityIsAuthenticated'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'用户名或者设备名',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'RequestLog',
    @level2type = N'COLUMN',
    @level2name = N'IdentityName'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'请求方法',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'RequestLog',
    @level2type = N'COLUMN',
    @level2name = N'RequestMethod'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'请求协议',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'RequestLog',
    @level2type = N'COLUMN',
    @level2name = N'RequestScheme'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'请求路径',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'RequestLog',
    @level2type = N'COLUMN',
    @level2name = N'RequestPath'
GO

GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'请求URL参数',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'RequestLog',
    @level2type = N'COLUMN',
    @level2name = N'RequestQueryString'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'请求ContentType',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'RequestLog',
    @level2type = N'COLUMN',
    @level2name = N'RequestContentType'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'请求字节长度',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'RequestLog',
    @level2type = N'COLUMN',
    @level2name = N'RequestContentLength'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'请求域名',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'RequestLog',
    @level2type = N'COLUMN',
    @level2name = N'RequestHost'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'请求体',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'RequestLog',
    @level2type = N'COLUMN',
    @level2name = N'RequestBody'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'请求时间',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'RequestLog',
    @level2type = N'COLUMN',
    @level2name = N'RequestTime'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'入库时间',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'RequestLog',
    @level2type = N'COLUMN',
    @level2name = N'CreateTime'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'请求类型，10-http，20-signalr',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'RequestLog',
    @level2type = N'COLUMN',
    @level2name = N'RequestType'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'请求组',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'RequestLog',
    @level2type = N'COLUMN',
    @level2name = N'RequestGroup'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'操作类型',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'RequestLog',
    @level2type = N'COLUMN',
    @level2name = N'Operation'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'请求头',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'RequestLog',
    @level2type = N'COLUMN',
    @level2name = N'RequestHead'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'服务器名称',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'RequestLog',
    @level2type = N'COLUMN',
    @level2name = N'ServerName'