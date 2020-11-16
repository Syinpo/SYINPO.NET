CREATE TABLE [dbo].[ResponseSnap]
(
	[Id] INT NOT NULL PRIMARY KEY IDENTITY, 
    [TraceId] NVARCHAR(500) NOT NULL, 
    [StatusCode] INT NULL, 
    [Success] BIT NOT NULL, 
    [ContentLength] BIGINT NOT NULL, 
    [ResponseBody] NVARCHAR(MAX) NULL, 
    [Elapsed] BIGINT NOT NULL, 
    [CreateTime] DATETIME NOT NULL
)

GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'主键',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'ResponseSnap',
    @level2type = N'COLUMN',
    @level2name = N'Id'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'主键',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'ResponseSnap',
    @level2type = N'COLUMN',
    @level2name = N'TraceId'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'是否成功',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'ResponseSnap',
    @level2type = N'COLUMN',
    @level2name = 'Success'
GO

GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'数据大小',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'ResponseSnap',
    @level2type = N'COLUMN',
    @level2name = N'ContentLength'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'响应体',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'ResponseSnap',
    @level2type = N'COLUMN',
    @level2name = N'ResponseBody'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'耗时',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'ResponseSnap',
    @level2type = N'COLUMN',
    @level2name = N'Elapsed'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'创建时间',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'ResponseSnap',
    @level2type = N'COLUMN',
    @level2name = N'CreateTime'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'状态码',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'ResponseSnap',
    @level2type = N'COLUMN',
    @level2name = N'StatusCode'