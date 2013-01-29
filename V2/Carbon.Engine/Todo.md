[Todo] - Have Content Manager use and connect to SQL Lite database
[Todo] - Finish Criterion Criterion Result and test
[Todo] - Test Server connection and send / receive protocoll buffer message
[Todo] - Have Carbed use the Content Manager's new Entry functionality
[Todo] - Store results in LevelDB
[Todo] - Log to file in Server with protocoll buffer, set up filters and proper file rotation
[Todo] - Test and extend logging within the engine
[Todo] - Add Unit tests

/*February 01*/
[Milestone] - Add Playfield resource with test data to replace V2Test project




[Resource Loader]

	* Finish up SQLite part connection and make it modular to allow for several running at the same time
	* Create schema / tables on demand if they don't exist by reflectin on type, should we scan the assembly to do this??
	* loading a ContentEntry
	* saving a ContentEntry
	* Evaluate ResourceLinks in ContentEntry on load by replacing them with id columns into the table for resource links
	
	* Use Content Transaction table to figure out if a resource was changed since the current transaction state
		- maybe there is a quick way to get new rows from sqlite ...
	
[Data Table layout]
	-- ResourceLink
		* Id
		* Hash
		* SourceFile // This can be empty if we have the hash
		
	-- ContentLink
		* Id
		* ContentId // This is the id of the target content
		* TypeName // This will be reflected upon to find the object to query
		
	-- ContentTransactionTable
		* ContentLink
		* ResourceLink
			- Either of those two will contain what was changed
			- multiple rows will point to the same resource, only the last one that is committed counts
			
		* Committed // wether this change is active
		* Date // Date this change was commited
