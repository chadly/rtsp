CREATE TABLE [dbo].[Camera](
	[CameraId] [varchar](25) NOT NULL,
	[Name] [varchar](150) NOT NULL,
	[NickNames] [varchar](500) NULL,
	[RtspUrl] [varchar](500) NULL,
	[CreatedAt] [datetime] not null default getutcdate(),
 CONSTRAINT [PK_Camera] PRIMARY KEY CLUSTERED 
(
	[CameraId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
