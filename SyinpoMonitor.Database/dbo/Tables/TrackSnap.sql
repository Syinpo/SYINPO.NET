CREATE TABLE [dbo].[TrackSnap]
(
	[Id] INT NOT NULL PRIMARY KEY IDENTITY,
	[TraceId] NVARCHAR(500) NOT NULL,
	[TraceName] NVARCHAR(MAX) NULL,
    [TraceData] NVARCHAR(MAX) NULL,
    [Elapsed] BIGINT NOT NULL,
    [CreateTime] DATETIME NOT NULL
)

GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'主键',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TrackSnap',
    @level2type = N'COLUMN',
    @level2name = N'Id'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'跟踪Id',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TrackSnap',
    @level2type = N'COLUMN',
    @level2name = N'TraceId'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'跟踪名称',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TrackSnap',
    @level2type = N'COLUMN',
    @level2name = N'TraceName'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'跟踪上下文数据',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TrackSnap',
    @level2type = N'COLUMN',
    @level2name = N'TraceData'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'耗时',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TrackSnap',
    @level2type = N'COLUMN',
    @level2name = N'Elapsed'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'创建时间',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TrackSnap',
    @level2type = N'COLUMN',
    @level2name = N'CreateTime'