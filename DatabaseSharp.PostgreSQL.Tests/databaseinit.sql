DROP FUNCTION IF EXISTS NoArguments_SingleValue;
GO

CREATE FUNCTION NoArguments_SingleValue()
RETURNS TABLE(SingleValue int) AS $$ 
    SELECT 1 AS SingleValue;
$$ 
LANGUAGE SQL;
GO

DROP FUNCTION IF EXISTS Arguments_SingleValue;
GO

CREATE FUNCTION Arguments_SingleValue(ARGVALUE int)
RETURNS TABLE(SingleValue int) AS $$ 
    SELECT ARGVALUE AS SingleValue;
$$ 
LANGUAGE SQL;
GO