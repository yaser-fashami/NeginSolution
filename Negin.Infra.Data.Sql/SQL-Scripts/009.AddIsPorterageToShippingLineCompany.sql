
IF COL_LENGTH('Basic.ShippingLineCompany','IsPorterage') IS NULL
	ALTER TABLE Basic.ShippingLineCompany
	ADD IsPorterage bit NOT NULL
	CONSTRAINT DF_IsPorterage DEFAULT (0)
	WITH VALUES