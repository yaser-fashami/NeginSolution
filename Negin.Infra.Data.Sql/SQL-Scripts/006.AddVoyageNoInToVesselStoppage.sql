IF COL_LENGTH('Operation.VesselStoppage','VoyageNoIn') IS NULL
	ALTER TABLE Operation.VesselStoppage
	Add VoyageNoIn varchar(20) NOT NULL DEFAULT '---';

	SELECT Id,VoyageNoIn 
	INTO #temp
	FROM Basic.Voyages

	UPDATE Operation.VesselStoppage
	SET VoyageNoIn = #temp.VoyageNoIn
	FROM #temp
	WHERE Operation.VesselStoppage.VoyageId = #temp.Id;

	DROP TABLE #temp;

IF COL_LENGTH('Basic.Voyages','IsActive') IS NULL
	ALTER TABLE Basic.Voyages
	ADD IsActive bit NOT NULL
	CONSTRAINT DF_DiscountPercent DEFAULT (1)
	WITH VALUES;

IF COL_LENGTH('Basic.Voyages','VoyageNoIn') IS NOT NULL  
	DROP INDEX IX_Voyages_VoyageNoIn_VesselId ON Basic.Voyages;
	ALTER TABLE Basic.Voyages
	DROP COLUMN VoyageNoIn, VoyageNoOut;

	--Before delete you must update voyageId in invoice and vesselStoppage 

	--DELETE FROM Basic.Voyages
	--WHERE
	--Id NOT IN 
	--( 
	--	SELECT  MIN(Id) FROM Basic.Voyages
	--	GROUP BY VesselId
	--);

	CREATE UNIQUE INDEX IX_Voyages_VesselId_IsActive
	ON Basic.Voyages (VesselId, IsActive)
	WHERE IsActive = 1;
