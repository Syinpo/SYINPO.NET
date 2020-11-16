CREATE TABLE [dbo].[ExceptionSnap]
(
	[Id] INT NOT NULL PRIMARY KEY IDENTITY,
	[TraceId] NVARCHAR(500) NOT NULL,
    [ErrorSource] NVARCHAR(MAX) NULL,
    [ErrorDetail] NVARCHAR(MAX) NULL,
    [CreateTime] DATETIME NOT NULL
)

GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'主键',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'ExceptionSnap',
    @level2type = N'COLUMN',
    @level2name = N'Id'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'跟踪Id',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'ExceptionSnap',
    @level2type = N'COLUMN',
    @level2name = N'TraceId'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'错误源',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'ExceptionSnap',
    @level2type = N'COLUMN',
    @level2name = N'ErrorSource'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'错误详情',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'ExceptionSnap',
    @level2type = N'COLUMN',
    @level2name = N'ErrorDetail'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'入库时间',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'ExceptionSnap',
    @level2type = N'COLUMN',
    @level2name = N'CreateTime'