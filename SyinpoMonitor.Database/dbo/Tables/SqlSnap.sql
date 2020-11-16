CREATE TABLE [dbo].[SqlSnap]
(
	[Id] INT NOT NULL PRIMARY KEY IDENTITY,
	[TraceId] NVARCHAR(500) NOT NULL,
    [SqlRaw] NVARCHAR(MAX) NULL,
    [SqlBody] NVARCHAR(MAX) NULL,
    [Elapsed] BIGINT NOT NULL,
    [CreateTime] DATETIME NOT NULL
)

GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'主键',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'SqlSnap',
    @level2type = N'COLUMN',
    @level2name = N'Id'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'跟踪Id',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'SqlSnap',
    @level2type = N'COLUMN',
    @level2name = N'TraceId'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'原始Sql',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'SqlSnap',
    @level2type = N'COLUMN',
    @level2name = N'SqlRaw'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'格式化后的Sql',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'SqlSnap',
    @level2type = N'COLUMN',
    @level2name = N'SqlBody'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'耗时',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'SqlSnap',
    @level2type = N'COLUMN',
    @level2name = N'Elapsed'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'入库时间',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'SqlSnap',
    @level2type = N'COLUMN',
    @level2name = N'CreateTime'