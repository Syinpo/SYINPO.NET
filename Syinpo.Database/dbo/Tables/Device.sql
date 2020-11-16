CREATE TABLE [dbo].[Device] (
    [Id]  INT NOT NULL IDENTITY,
    [TrackingId]   NVARCHAR(50) NOT NULL,
    [DeviceUuid] NVARCHAR (1000)  NOT NULL,
    [Mobile]     NVARCHAR (1000)  NULL,
    [Brand]      NVARCHAR (1000)  NULL,
    [Model]      NVARCHAR (1000)  NULL,
    [Os]      NVARCHAR (1000)  NULL,
    [OsVersion]      NVARCHAR (1000)  NULL,
    [WeiXinVersion]      NVARCHAR (1000)  NULL,
    [AssistantVersion]      NVARCHAR (1000)  NULL,
    [ListType]      INT NOT NULL DEFAULT 0,
    [Longitude]      DECIMAL(18, 6)  NULL,
    [Latitude]      DECIMAL(18, 6)  NULL,
    [Memo]      NVARCHAR(500)  NULL,
    [IsOnline]      BIT  NOT NULL,
    [Approved]   BIT              NOT NULL,
    [Deleted]    BIT              NOT NULL,
    [LastSyncContactTime] DATETIME         NULL,
    [CreateTime] DATETIME         NOT NULL,
    [UpdateTime] DATETIME         NOT NULL,
    CONSTRAINT [PK_Device] PRIMARY KEY CLUSTERED ([Id] ASC)
);


GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'主键',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'Device',
    @level2type = N'COLUMN',
    @level2name = N'Id'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'对外跟踪的设备Id',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'Device',
    @level2type = N'COLUMN',
    @level2name = 'TrackingId'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'设备IMIE',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'Device',
    @level2type = N'COLUMN',
    @level2name = N'DeviceUuid'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'手机号码',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'Device',
    @level2type = N'COLUMN',
    @level2name = N'Mobile'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'手机品牌',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'Device',
    @level2type = N'COLUMN',
    @level2name = N'Brand'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'手机型号',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'Device',
    @level2type = N'COLUMN',
    @level2name = N'Model'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'批准',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'Device',
    @level2type = N'COLUMN',
    @level2name = N'Approved'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'删除',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'Device',
    @level2type = N'COLUMN',
    @level2name = N'Deleted'
GO

GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'创建时间',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'Device',
    @level2type = N'COLUMN',
    @level2name = N'CreateTime'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'更新时间',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'Device',
    @level2type = N'COLUMN',
    @level2name = N'UpdateTime'
GO

GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'手机操作系统',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'Device',
    @level2type = N'COLUMN',
    @level2name = N'Os'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'手机操作系统版本号',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'Device',
    @level2type = N'COLUMN',
    @level2name = N'OsVersion'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'手机微信版本号',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'Device',
    @level2type = N'COLUMN',
    @level2name = N'WeiXinVersion'
GO

GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'经度',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'Device',
    @level2type = N'COLUMN',
    @level2name = N'Longitude'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'纬度',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'Device',
    @level2type = N'COLUMN',
    @level2name = N'Latitude'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'最后同步手机联系人的时间',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'Device',
    @level2type = N'COLUMN',
    @level2name = N'LastSyncContactTime'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'备注',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'Device',
    @level2type = N'COLUMN',
    @level2name = N'Memo'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'设备是否在线',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'Device',
    @level2type = N'COLUMN',
    @level2name = 'IsOnline'
GO

GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'手机助手版本号',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'Device',
    @level2type = N'COLUMN',
    @level2name = N'AssistantVersion'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'所处名单 （0 无，1 在黑名单，2 在白名单）',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'Device',
    @level2type = N'COLUMN',
    @level2name = N'ListType'
GO
