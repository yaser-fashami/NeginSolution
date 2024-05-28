IF NOT EXISTS (
SELECT *
FROM INFORMATION_SCHEMA.TABLES
WHERE 
TABLE_NAME = 'LoadingDischargeTariff' and 
TABLE_SCHEMA = 'Basic')
BEGIN
	CREATE TABLE [Basic].[LoadingDischargeTariff](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Description] [nvarchar](100) NULL,
	[EffectiveDate] [smalldatetime] NOT NULL,
	[CreatedById] [nvarchar](450) NOT NULL,
	[CreateDate] [datetime2](7) NOT NULL,
	[ModifiedById] [nvarchar](450) NULL,
	[ModifiedDate] [datetime2](7) NULL,
	 CONSTRAINT [PK_Basic.LoadinDischargeTariff] PRIMARY KEY CLUSTERED 
	(
		[Id] ASC
	)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
	) ON [PRIMARY]

	SET IDENTITY_INSERT [Basic].[LoadingDischargeTariff] ON 
	INSERT [Basic].[LoadingDischargeTariff] ([Id], [Description], [EffectiveDate], [CreatedById], [CreateDate], [ModifiedById], [ModifiedDate]) VALUES (1, N'تعرفه سال 1402', CAST(N'2023-03-21T00:00:00' AS SmallDateTime), N'4656cc2c-f0df-4d07-b5fd-9a3aa3efdba9', CAST(N'2023-03-21T00:00:00.0000000' AS DateTime2), NULL, NULL)
	SET IDENTITY_INSERT [Basic].[LoadingDischargeTariff] OFF

END

IF NOT EXISTS (
SELECT *
FROM INFORMATION_SCHEMA.TABLES
WHERE 
TABLE_NAME = 'LoadingDischargeTariffDetails' and 
TABLE_SCHEMA = 'Basic')
BEGIN
	CREATE TABLE [Basic].[LoadingDischargeTariffDetails](
		[Id] [int] IDENTITY(1,1) NOT NULL,
		[LoadingDischargeTariffId] [int] NOT NULL,
		[GroupName] [nvarchar](50) NOT NULL,
		[Goods] [nvarchar](500) NULL,
		[Price] [int] NOT NULL,
	 CONSTRAINT [PK_LoadingDischargeTariffDetails] PRIMARY KEY CLUSTERED 
	(
		[Id] ASC
	)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
	) ON [PRIMARY]

	ALTER TABLE [Basic].[LoadingDischargeTariffDetails]  WITH CHECK ADD  CONSTRAINT [FK_LoadingDischargeTariffDetails_LoadingDischargeTariff] FOREIGN KEY([LoadingDischargeTariffId])
	REFERENCES [Basic].[LoadingDischargeTariff] ([Id])

	ALTER TABLE [Basic].[LoadingDischargeTariffDetails] CHECK CONSTRAINT [FK_LoadingDischargeTariffDetails_LoadingDischargeTariff]

	SET IDENTITY_INSERT [Basic].[LoadingDischargeTariffDetails] ON 
	INSERT [Basic].[LoadingDischargeTariffDetails] ([Id], [LoadingDischargeTariffId], [GroupName], [Goods], [Price]) VALUES (1, 1, N'فله خشک', N'سنگ گچ,شن و ماسه, سنگ آهک,سنگ نمک, سرباره دانه دانه', 339000)
	INSERT [Basic].[LoadingDischargeTariffDetails] ([Id], [LoadingDischargeTariffId], [GroupName], [Goods], [Price]) VALUES (2, 1, N'فله خشک', N'انواع سنگ آهن,بال کلی,خاک کائولین,سیمان فله,خرده سنگ,پودر کلینکر', 608000)
	INSERT [Basic].[LoadingDischargeTariffDetails] ([Id], [LoadingDischargeTariffId], [GroupName], [Goods], [Price]) VALUES (3, 1, N'فله خشک', N'ذغال سنگ,بریکت,سنگ منگنز,پودر پتاسیم,پودر آلومینیوم,مواد شیمیایی,غیره', 628000)
	INSERT [Basic].[LoadingDischargeTariffDetails] ([Id], [LoadingDischargeTariffId], [GroupName], [Goods], [Price]) VALUES (4, 1, N'فله خشک', N'غلات,شکر,مواد غیرمعدنی,مواد غیر شیمیایی', 211000)
	INSERT [Basic].[LoadingDischargeTariffDetails] ([Id], [LoadingDischargeTariffId], [GroupName], [Goods], [Price]) VALUES (5, 1, N'بارگیری بوسیله کرین ساحلی', NULL, 678600)
	INSERT [Basic].[LoadingDischargeTariffDetails] ([Id], [LoadingDischargeTariffId], [GroupName], [Goods], [Price]) VALUES (6, 1, N'انبارداری', NULL, 5400)
	SET IDENTITY_INSERT [Basic].[LoadingDischargeTariffDetails] OFF

END