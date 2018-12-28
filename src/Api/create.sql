CREATE TABLE [dbo].[Camera](
	[CameraId] [int] IDENTITY(1,1) NOT NULL,
	[Name] [varchar](50) NOT NULL,
	[RtspUrl] [varchar](250) NOT NULL,
	[CreatedAt] [datetime] not null default getutcdate(),
 CONSTRAINT [PK_Camera] PRIMARY KEY CLUSTERED 
(
	[CameraId] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF) ON [PRIMARY],
 CONSTRAINT [UQ_Camera_Name] UNIQUE NONCLUSTERED 
(
	[Name] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
