CREATE TABLE [dbo].[TrafficStatist]
(
	[Id] INT NOT NULL PRIMARY KEY IDENTITY,
    [DateTime] BIGINT NOT NULL,
    [TotalReq] BIGINT NOT NULL,
    [TotalHttpReq] BIGINT NOT NULL,
    [TotalSignalrReq] BIGINT NOT NULL,
    [ExceptionReq] BIGINT NOT NULL,
    [TotalSignalrConnectReq] BIGINT NOT NULL,
    [TotalSignalrDisconnectReq] BIGINT NOT NULL,
    [TotalOnlineDevice] BIGINT NOT NULL,
    [ServerCpuAvgRate] DECIMAL(18, 2) NOT NULL,
    [ServerCpuTopRate] DECIMAL(18, 2) NOT NULL,
    [ServerRamAvgRate] DECIMAL(18, 2) NOT NULL,
    [ServerRamTopRate] DECIMAL(18, 2) NOT NULL,
    [AppliationCpuAvgRate] DECIMAL(18, 2) NOT NULL,
    [AppliationCpuTopRate] DECIMAL(18, 2) NOT NULL,
    [AppliationRamAvgRate] DECIMAL(18, 2) NOT NULL,
    [AppliationRamTopRate] DECIMAL(18, 2) NOT NULL,
    [AppliationRamAvgSize] DECIMAL(18, 2) NOT NULL,
    [CreateTime] DATETIME NOT NULL
)

GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'主键',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TrafficStatist',
    @level2type = N'COLUMN',
    @level2name = N'Id'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'请求组',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TrafficStatist',
    @level2type = N'COLUMN',
    @level2name = 'DateTime'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'总流量',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TrafficStatist',
    @level2type = N'COLUMN',
    @level2name = N'TotalReq'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'Http流量',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TrafficStatist',
    @level2type = N'COLUMN',
    @level2name = N'TotalHttpReq'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'Signalr流量',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TrafficStatist',
    @level2type = N'COLUMN',
    @level2name = N'TotalSignalrReq'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'创建时间',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TrafficStatist',
    @level2type = N'COLUMN',
    @level2name = N'CreateTime'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'异常流量',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TrafficStatist',
    @level2type = N'COLUMN',
    @level2name = N'ExceptionReq'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'服务器平均CPU比率',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TrafficStatist',
    @level2type = N'COLUMN',
    @level2name = N'ServerCpuAvgRate'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'服务器最大CPU比率',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TrafficStatist',
    @level2type = N'COLUMN',
    @level2name = N'ServerCpuTopRate'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'服务器平均内存比率',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TrafficStatist',
    @level2type = N'COLUMN',
    @level2name = N'ServerRamAvgRate'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'服务器最大内存比率',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TrafficStatist',
    @level2type = N'COLUMN',
    @level2name = N'ServerRamTopRate'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'程序平均CPU比率',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TrafficStatist',
    @level2type = N'COLUMN',
    @level2name = N'AppliationCpuAvgRate'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'程序最大CPU比率',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TrafficStatist',
    @level2type = N'COLUMN',
    @level2name = N'AppliationCpuTopRate'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'程序平均内存比率',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TrafficStatist',
    @level2type = N'COLUMN',
    @level2name = N'AppliationRamAvgRate'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'程序最大内存比率',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TrafficStatist',
    @level2type = N'COLUMN',
    @level2name = N'AppliationRamTopRate'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'程序平均占用内存大小',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TrafficStatist',
    @level2type = N'COLUMN',
    @level2name = N'AppliationRamAvgSize'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'在线的设备',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TrafficStatist',
    @level2type = N'COLUMN',
    @level2name = N'TotalOnlineDevice'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'Signalr连接流量',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TrafficStatist',
    @level2type = N'COLUMN',
    @level2name = N'TotalSignalrConnectReq'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'Signalr断开流量',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TrafficStatist',
    @level2type = N'COLUMN',
    @level2name = N'TotalSignalrDisconnectReq'