CREATE TABLE [dbo].[EventQueue]
(
	[Id] INT NOT NULL PRIMARY KEY IDENTITY,
    [Version] NVARCHAR(200) NOT NULL,
    [RouteName] NVARCHAR(400) NOT NULL,
    [Heads] NVARCHAR(MAX) NOT NULL,
    [Content] NVARCHAR(MAX) NOT NULL,
    [Retry] INT NOT NULL,
    [OutTime] DATETIME NULL,
    [Note] NVARCHAR(MAX) NULL,
    [CreateTime] DATETIME NOT NULL
)

GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'主键',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'EventQueue',
    @level2type = N'COLUMN',
    @level2name = N'Id'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'隔离号',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'EventQueue',
    @level2type = N'COLUMN',
    @level2name = N'Version'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'事件名称',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'EventQueue',
    @level2type = N'COLUMN',
    @level2name = N'RouteName'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'数据',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'EventQueue',
    @level2type = N'COLUMN',
    @level2name = N'Content'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'重试次数',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'EventQueue',
    @level2type = N'COLUMN',
    @level2name = N'Retry'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'成功时间',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'EventQueue',
    @level2type = N'COLUMN',
    @level2name = N'OutTime'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'备注',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'EventQueue',
    @level2type = N'COLUMN',
    @level2name = N'Note'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'创建时间',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'EventQueue',
    @level2type = N'COLUMN',
    @level2name = N'CreateTime'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'头部信息',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'EventQueue',
    @level2type = N'COLUMN',
    @level2name = N'Heads'