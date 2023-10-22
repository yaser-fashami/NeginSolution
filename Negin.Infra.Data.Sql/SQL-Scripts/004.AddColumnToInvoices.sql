IF COL_LENGTH('Billing.Invoices','DiscountPercent') IS NULL
	ALTER TABLE Billing.Invoices
	ADD DiscountPercent tinyint NOT NULL
	CONSTRAINT DF_DiscountPercent DEFAULT (0)
	WITH VALUES
