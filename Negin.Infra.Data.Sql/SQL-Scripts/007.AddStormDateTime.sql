
IF COL_LENGTH('Operation.VesselStoppage','StartStorm') IS NULL
	ALTER TABLE Operation.VesselStoppage
	ADD StartStorm smalldatetime NULL 

IF COL_LENGTH('Operation.VesselStoppage','EndStorm') IS NULL
	ALTER TABLE Operation.VesselStoppage
	ADD EndStorm smalldatetime NULL 

