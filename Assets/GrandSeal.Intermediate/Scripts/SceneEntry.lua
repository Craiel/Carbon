--[[
	Entry scene main script, this will setup the main data like fallbacks etc
	Afterwards we will transition into loading and main menu scene
]]--

-- Setup the basic resources and fallback data
-- Setup graphic settings
-- Setup sound settings
-- Setup basic control scheme's

-- Transition into the Main Menu
SceneTransition("MainMenu", "{Resource Scripts\SceneMainMenu.lua}");
