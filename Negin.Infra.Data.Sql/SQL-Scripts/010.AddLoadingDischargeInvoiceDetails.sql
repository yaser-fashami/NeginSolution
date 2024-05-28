IF NOT EXISTS (
SELECT *
FROM INFORMATION_SCHEMA.TABLES
WHERE 
TABLE_NAME = 'LoadingDischarge' and 
TABLE_SCHEMA = 'Operation')
BEGIN
	CREATE TABLE [Operation].[LoadingDischarge](
	[Id] [decimal](20, 0) IDENTITY(1,1) NOT NULL,
	[VesselStoppageId] [decimal](20, 0) NOT NULL,
	[Method] [char](4) NOT NULL,
	[Tonage] [float] NOT NULL,
	[LoadingDischargeTariffDetailId] [int] NOT NULL,
	[HasCrane] [bit] NOT NULL,
	[HasInventory] [bit] NOT NULL,
	[CreatedById] [nvarchar](450) NULL,
	[CreateDate] [datetime2](7) NOT NULL,
	[ModifiedById] [nvarchar](450) NULL,
	[ModifiedDate] [datetime2](7) NULL,
	[IsDelete] [bit] NOT NULL
	 CONSTRAINT [PK_Billing.LoadingDischarge] PRIMARY KEY CLUSTERED 
	(
		[Id] ASC
	)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
	) ON [PRIMARY]

	ALTER TABLE [Operation].[LoadingDischarge] ADD  DEFAULT (CONVERT([bit],(0))) FOR [IsDelete];

	ALTER TABLE [Operation].[LoadingDischarge]  WITH CHECK ADD  CONSTRAINT [FK_LoadingDischarge_VesselStoppage_VesselStoppageId] FOREIGN KEY([VesselStoppageId])
	REFERENCES [Operation].[VesselStoppage] ([Id])
	ON DELETE CASCADE

	ALTER TABLE [Operation].[LoadingDischarge]  WITH CHECK ADD  CONSTRAINT [FK_LoadingDischarge_AspNetUsers_CreatedById] FOREIGN KEY([CreatedById])
	REFERENCES [dbo].[AspNetUsers] ([Id]);

	ALTER TABLE [Operation].[LoadingDischarge] CHECK CONSTRAINT [FK_LoadingDischarge_AspNetUsers_CreatedById];

	ALTER TABLE [Operation].[LoadingDischarge]  WITH CHECK ADD  CONSTRAINT [FK_LoadingDischarge_AspNetUsers_ModifiedById] FOREIGN KEY([ModifiedById])
	REFERENCES [dbo].[AspNetUsers] ([Id]);

	ALTER TABLE [Operation].[LoadingDischarge] CHECK CONSTRAINT [FK_LoadingDischarge_AspNetUsers_ModifiedById];

	ALTER TABLE [Operation].[LoadingDischarge] CHECK CONSTRAINT [FK_LoadingDischarge_VesselStoppage_VesselStoppageId];

	ALTER TABLE [Operation].[LoadingDischarge]  WITH CHECK ADD  CONSTRAINT [FK_LoadingDischarge_LoadingDischargeTariffDetails_Id] FOREIGN KEY([LoadingDischargeTariffDetailId])
	REFERENCES [Basic].[LoadingDischargeTariffDetails] ([Id]);

END

IF NOT EXISTS (
SELECT *
FROM INFORMATION_SCHEMA.TABLES
WHERE 
TABLE_NAME = 'LoadingDischargeInvoice' and 
TABLE_SCHEMA = 'Billing')
BEGIN
	CREATE TABLE [Billing].[LoadingDischargeInvoice](
	[Id] [decimal](20, 0) IDENTITY(1,1) NOT NULL,
	[InvoiceNo] [varchar](15) NOT NULL,
	[InvoiceDate] [datetime2](7) NOT NULL,
	[Status] [tinyint] NOT NULL,
	[LoadingDischargeId] [decimal](20, 0) NOT NULL,
	[LoadingDischargeTariffId] [int] NOT NULL,
	[ShippingLineCompanyId] [bigint] NOT NULL,
	[DiscountPercent] [tinyint] NOT NULL,
	[Tonage] [float] NOT NULL,
	[LDCostR] [decimal](20, 0) NOT NULL,
	[CraneCostR] [decimal](20, 0) NULL,
	[InventoryCostR] [decimal](20, 0) NULL,
	[VatPercent] [tinyint]  NULL,
	[VatCostR] [decimal](20, 0) NULL,
	[TotalPriceR] [decimal](20, 0) NOT NULL,
	[CreatedById] [nvarchar](450) NULL,
	[CreateDate] [datetime2](7) NOT NULL,
	[ModifiedById] [nvarchar](450) NULL,
	[ModifiedDate] [datetime2](7) NULL,
	[IsDelete] [bit] NOT NULL
	 CONSTRAINT [PK_Billing.LoadingDischargeInvoice] PRIMARY KEY CLUSTERED 
	(
		[Id] ASC
	)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
	) ON [PRIMARY]

	ALTER TABLE [Billing].[LoadingDischargeInvoice] ADD  DEFAULT (CONVERT([bit],(0))) FOR [IsDelete];

	ALTER TABLE [Billing].[LoadingDischargeInvoice] ADD  DEFAULT ((0)) FOR [DiscountPercent];

	ALTER TABLE [Billing].[LoadingDischargeInvoice]  WITH CHECK ADD  CONSTRAINT [FK_LoadingDischargeInvoice_LoadingDischarge_LoadingDischargeId] FOREIGN KEY([LoadingDischargeId])
	REFERENCES [Operation].[LoadingDischarge] ([Id]);

	ALTER TABLE [Billing].[LoadingDischargeInvoice]  WITH CHECK ADD  CONSTRAINT [FK_LoadingDischargeInvoice_AspNetUsers_CreatedById] FOREIGN KEY([CreatedById])
	REFERENCES [dbo].[AspNetUsers] ([Id]);

	ALTER TABLE [Billing].[LoadingDischargeInvoice] CHECK CONSTRAINT [FK_LoadingDischargeInvoice_AspNetUsers_CreatedById];

	ALTER TABLE [Billing].[LoadingDischargeInvoice]  WITH CHECK ADD  CONSTRAINT [FK_LoadingDischargeInvoice_AspNetUsers_ModifiedById] FOREIGN KEY([ModifiedById])
	REFERENCES [dbo].[AspNetUsers] ([Id]);

	ALTER TABLE [Billing].[LoadingDischargeInvoice] CHECK CONSTRAINT [FK_LoadingDischargeInvoice_AspNetUsers_ModifiedById];

	ALTER TABLE [Billing].[LoadingDischargeInvoice]  WITH CHECK ADD  CONSTRAINT [FK_LoadingDischargeInvoice_LoadingDischargeTariff_LoadingDischargeTariffId] FOREIGN KEY([LoadingDischargeTariffId])
	REFERENCES [Basic].[LoadingDischargeTariff] ([Id]);

	ALTER TABLE [Billing].[LoadingDischargeInvoice]  WITH CHECK ADD  CONSTRAINT [FK_LoadingDischargeInvoice_ShippingLineCompany_ShippingLineCompanyId] FOREIGN KEY([ShippingLineCompanyId])
	REFERENCES [Basic].[ShippingLineCompany] ([Id]);

	ALTER TABLE [Billing].[LoadingDischargeInvoice] CHECK CONSTRAINT [FK_LoadingDischargeInvoice_LoadingDischargeTariff_LoadingDischargeTariffId];

	CREATE UNIQUE NONCLUSTERED INDEX [IX_LoadingDischargeInvoice_LoadingDischarge] ON [Billing].[LoadingDischargeInvoice]
	(
		[LoadingDischargeId] ASC
	)
	WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]

END