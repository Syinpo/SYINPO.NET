CREATE TABLE [dbo].[DeviceSms] (
    [Id]              INT            IDENTITY (1, 1) NOT NULL,
    [DeviceId] INT            NOT NULL,
    [FromPhone]      NVARCHAR (50) NOT NULL,
    [ToPhone]         NVARCHAR(50)       NOT NULL,
    [Sent] BIT NOT NULL, 
    [Content] NVARCHAR(MAX) NOT NULL, 
    [SmsReceiveTime] DATETIME NOT NULL, 
    [SmsCreateTime] DATETIME NOT NULL, 
    [CreateTime] DATETIME NOT NULL, 
    [SensitiveWord] NVARCHAR(1000) NULL, 
    CONSTRAINT [PK_DeviceSms] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_DeviceSms_Device] FOREIGN KEY ([DeviceId]) REFERENCES [Device]([Id])
);


GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'主键',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'DeviceSms',
    @level2type = N'COLUMN',
    @level2name = N'Id'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'设备主键',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'DeviceSms',
    @level2type = N'COLUMN',
    @level2name = 'DeviceId'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'从哪个号码',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'DeviceSms',
    @level2type = N'COLUMN',
    @level2name = 'FromPhone'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'到哪个号码',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'DeviceSms',
    @level2type = N'COLUMN',
    @level2name = 'ToPhone'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'是否发送，否则接收',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'DeviceSms',
    @level2type = N'COLUMN',
    @level2name = N'Sent'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'短信内容',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'DeviceSms',
    @level2type = N'COLUMN',
    @level2name = N'Content'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'设备短信收到时间',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'DeviceSms',
    @level2type = N'COLUMN',
    @level2name = 'SmsReceiveTime'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'设备短信创建时间',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'DeviceSms',
    @level2type = N'COLUMN',
    @level2name = 'SmsCreateTime'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'创建时间',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'DeviceSms',
    @level2type = N'COLUMN',
    @level2name = N'CreateTime'
GO

CREATE INDEX [IX_DeviceSms_FromPhone] ON [dbo].[DeviceSms] ([FromPhone])

GO

CREATE INDEX [IX_DeviceSms_ToPhone] ON [dbo].[DeviceSms] ([ToPhone])

GO

CREATE INDEX [IX_DeviceSms_CreateTime] ON [dbo].[DeviceSms] ([CreateTime])INCLUDE([DeviceId],[SmsCreateTime],[FromPhone],[ToPhone])

GO

CREATE INDEX [IX_DeviceSms_SmsCreateTime] ON [dbo].[DeviceSms] ([SmsCreateTime])

GO

CREATE INDEX [IX_DeviceSms_DeviceId] ON [dbo].[DeviceSms] ([DeviceId])

GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'敏感词',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'DeviceSms',
    @level2type = N'COLUMN',
    @level2name = 'SensitiveWord'