
CREATE TABLE [dbo].[App_BasicInfo] (
    [Id]               INT              IDENTITY (1, 1) NOT NULL,
    [RunVersion]       NVARCHAR (255)   NULL,
    [AppName]          NVARCHAR (255)   NULL,
    [AppType]          INT              NOT NULL,
    [Status]           INT              NOT NULL,
    [AppId]            NVARCHAR (255)   NOT NULL,
    [CreateTime]       DATETIME         NULL,
    [LastUpdateTime]   DATETIME         NULL,
    [OnlineTime]       DATETIME         NULL,
    [GitRepository]    NVARCHAR (255)   NULL,
    [MacAddress]       NVARCHAR (255)   NULL,
    [IpAddress]        NVARCHAR (255)   NULL,
    [ProcessDirectory] NVARCHAR (255)   NULL,
    [ComputerName]     NVARCHAR (255)   NULL,
    [LastHeartTime]    DATETIME         NULL,
    [StoreId]          UNIQUEIDENTIFIER NULL
);

GO

CREATE TABLE [dbo].[App_PackInfo] (
    [Id]           INT              IDENTITY (1, 1) NOT NULL,
    [Name]         NVARCHAR (50)    NOT NULL,
    [CustomerId]   UNIQUEIDENTIFIER NULL,
    [AppType]      INT              NOT NULL,
    [Version]      NVARCHAR (50)    NOT NULL,
    [PublishTime]  DATETIME         NOT NULL,
    [MajorVersion] INT              NOT NULL,
    [Author]       NVARCHAR (50)    NULL,
    [GitUrl]       NVARCHAR (255)   NULL,
    [DownUrl]      NVARCHAR (255)   NULL,
    [Description]  NVARCHAR (2000)  NULL
);

GO

CREATE TABLE [dbo].[ConfigCenter_ClusterComputer] (
    [IpAddress]      NVARCHAR (50)  NOT NULL,
    [Port]           INT            NOT NULL,
    [LanIpAddress]   NVARCHAR (50)  NULL,
    [LanPort]        INT            NULL,
    [LanMqttPort]    INT            NULL,
    [MqttPort]       INT            NULL,
    [Name]           NVARCHAR (50)  NOT NULL,
    [IsMaster]       BIT            NOT NULL,
    [Description]    NVARCHAR (255) NULL,
    [AdminUrl]       NVARCHAR (255) NULL,
    [AccessUsername] NVARCHAR (50)  NULL,
    [AccessPassword] NVARCHAR (50)  NULL,
    [ComputerType]   INT            NOT NULL,
    [Weight]         INT            NOT NULL,
    [Id]             INT            IDENTITY (1, 1) NOT NULL,
    [DelFlag]        BIT            NOT NULL,
    [Status]         INT            NOT NULL,
    [LastUpdateTime] DATETIME       NULL
);

GO

CREATE TABLE [dbo].[ConfigCenter_ModuleSettings] (
    [CustomerId]  UNIQUEIDENTIFIER NOT NULL,
    [ModuleId]    UNIQUEIDENTIFIER NOT NULL,
    [JsonContent] NTEXT            NULL,
    [CreateTime]  DATETIME         NOT NULL
);



GO

CREATE TABLE [dbo].[ConfigCenter_MqListener] (
    [Id]               INT             IDENTITY (1, 1) NOT NULL,
    [ClientId]         NVARCHAR (255)  NULL,
    [MacAddress]       NVARCHAR (255)  NULL,
    [ProcessDirectory] NVARCHAR (255)  NULL,
    [IpAddress]        NVARCHAR (1000) NULL,
    [CreateTime]       DATETIME        NOT NULL,
    [UpTime]           DATETIME        NULL,
    [Status]           INT             NOT NULL,
    [ComputerName]     NVARCHAR (255)  NULL,
    [DownTime]         DATETIME        NULL,
    [LastHeartTime]    DATETIME        NULL,
    [MsgCount]         BIGINT          NULL,
    [MsgSize]          BIGINT          NULL,
    [TotalMsgCount]    BIGINT          NULL,
    [totalMsgSize]     BIGINT          NULL
);



GO

CREATE TABLE [dbo].[ConfigCenter_MqTable] (
    [CreateTime]   DATETIME         NOT NULL,
    [StoreId]      UNIQUEIDENTIFIER NOT NULL,
    [MqInstanceId] INT              NULL,
    [MqListenerId] INT              NULL,
    [Expired]      INT              NOT NULL
);



GO

CREATE TABLE [dbo].[ConfigCenter_QueueAndTopic] (
    [Id]           UNIQUEIDENTIFIER NOT NULL,
    [Name]         NVARCHAR (50)    NOT NULL,
    [Description]  NVARCHAR (255)   NULL,
    [IsTopic]      BIT              NOT NULL,
    [CreateTime]   DATETIME         NULL,
    [CreateUserId] UNIQUEIDENTIFIER NULL
);

GO

CREATE TABLE [dbo].[ConfigCenter_RedisCacheKey] (
    [CacheKey]        NVARCHAR (255) NOT NULL,
    [CreateTime]      DATETIME       NOT NULL,
    [Expired]         INT            NOT NULL,
    [RedisInstanceId] INT            NOT NULL
);



GO

CREATE TABLE [dbo].[Device_BasicInfo] (
    [Id]               UNIQUEIDENTIFIER NOT NULL,
    [Name]             NVARCHAR (50)    NOT NULL,
    [DeviceType]       INT              NOT NULL,
    [IpAddress]        NVARCHAR (255)   NULL,
    [MacAddress]       NVARCHAR (255)   NULL,
    [Description]      NVARCHAR (2000)  NULL,
    [Status]           INT              NOT NULL,
    [OSVersion]        NVARCHAR (255)   NULL,
    [DriverVersion]    NVARCHAR (255)   NULL,
    [RemoteAccessInfo] NVARCHAR (255)   NULL,
    [WorkTimePeriod]   NVARCHAR (2000)  NULL,
    [CreateTime]       DATETIME         NULL,
    [CreateUserId]     UNIQUEIDENTIFIER NULL,
    [LastUpdateTime]   DATETIME         NULL,
    [LastUpdateUserId] UNIQUEIDENTIFIER NULL,
    [StoreId]          UNIQUEIDENTIFIER NULL,
    [CustomerId]       UNIQUEIDENTIFIER NOT NULL
);

GO

CREATE TABLE [dbo].[Sys_CacheVersion] (
    [Id]      INT IDENTITY (1, 1) NOT NULL,
    [Version] INT NOT NULL
);
