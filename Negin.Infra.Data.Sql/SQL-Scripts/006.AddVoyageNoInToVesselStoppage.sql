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

IF COL_LENGTH('Basic.Voyages','VoyageNoIn') IS NOT NULL  
	DROP INDEX IX_Voyages_VoyageNoIn_VesselId ON Basic.Voyages;
	ALTER TABLE Basic.Voyages
	DROP COLUMN VoyageNoIn, VoyageNoOut;

	DELETE FROM Basic.Voyages
	WHERE
	Id NOT IN 
	( 
		SELECT  MIN(Id) FROM Basic.Voyages
		GROUP BY VesselId
	);

	CREATE UNIQUE INDEX IX_Voyages_VesselId_OwnerId
	ON Basic.Voyages (VesselId, OwnerId);
