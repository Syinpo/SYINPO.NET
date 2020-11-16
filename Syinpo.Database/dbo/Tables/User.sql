CREATE TABLE [dbo].[User] (
    [Id]                INT              IDENTITY (1, 1) NOT NULL,
    [PartnerId]   int NOT NULL,
    [ParentUserId]     INT              NOT NULL,
    [UserGuid]      UNIQUEIDENTIFIER NOT NULL,
    [Username]          NVARCHAR (1000)  NOT NULL,
    [Password]          NVARCHAR (1000)  NOT NULL,
    [DisplayName]       NVARCHAR (1000)  NULL,
    [RealName]       NVARCHAR (1000)  NULL,
    [Email]             NVARCHAR (1000)  NULL,
    [Mobile]            NVARCHAR (1000)  NULL,
    [Approved]          BIT              NOT NULL,
    [Deleted]           BIT              NOT NULL,
    [CreateTime]        DATETIME         NOT NULL,
    [UpdateTime]        DATETIME         NOT NULL,
    [LastLoginDate]     DATETIME         NULL,
    [IsAdmin] BIT NOT NULL, 
    [IsHttpUse] BIT NOT NULL, 
    CONSTRAINT [PK_User] PRIMARY KEY CLUSTERED ([Id] ASC)
);


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'主键', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'User', @level2type = N'COLUMN', @level2name = N'Id';


GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'对外使用主键',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'User',
    @level2type = N'COLUMN',
    @level2name = N'UserGuid'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'用户名',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'User',
    @level2type = N'COLUMN',
    @level2name = N'Username'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'密码',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'User',
    @level2type = N'COLUMN',
    @level2name = N'Password'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'显示名称',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'User',
    @level2type = N'COLUMN',
    @level2name = N'DisplayName'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'电子邮件',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'User',
    @level2type = N'COLUMN',
    @level2name = N'Email'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'手机号码',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'User',
    @level2type = N'COLUMN',
    @level2name = N'Mobile'
GO

GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'批准',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'User',
    @level2type = N'COLUMN',
    @level2name = N'Approved'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'删除',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'User',
    @level2type = N'COLUMN',
    @level2name = N'Deleted'
GO

GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'创建时间',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'User',
    @level2type = N'COLUMN',
    @level2name = N'CreateTime'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'更新时间',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'User',
    @level2type = N'COLUMN',
    @level2name = N'UpdateTime'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'最后登录时间',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'User',
    @level2type = N'COLUMN',
    @level2name = N'LastLoginDate'
GO

GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'父用户主键',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'User',
    @level2type = N'COLUMN',
    @level2name = 'ParentUserId'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'合作伙伴主键',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'User',
    @level2type = N'COLUMN',
    @level2name = N'PartnerId'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'真实姓名',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'User',
    @level2type = N'COLUMN',
    @level2name = N'RealName'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'是否超级管理员',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'User',
    @level2type = N'COLUMN',
    @level2name = N'IsAdmin'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'对外用户',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'User',
    @level2type = N'COLUMN',
    @level2name = N'IsHttpUse'