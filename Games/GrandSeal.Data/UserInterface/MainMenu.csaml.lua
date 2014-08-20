local currentActive = -1;

method Initialize()
{
	TextVersion.Text = host.Version;
}

method ChangeActiveMenu()
{
	currentActive = host.ActiveMenu;
	
	' Update the Source Images for our menu accordingly
	if currentActive == MENU_START:
			MenuStart.Source = "{Resource Textures\UI\MenuStart_Active.png}";
		else:
			MenuStart.Source = "{Resource Textures\UI\MenuStart_Inactive.png}";
		end;
	if currentActive == MENU_LOAD:
			MenuLoad.Source = "{Resource Textures\UI\MenuLoad_Active.png}";
		else:
			MenuLoad.Source = "{Resource Textures\UI\MenuLoad_Inactive.png}";
		end;
	if currentActive == MENU_OPTIONS:
			MenuOptions.Source = "{Resource Textures\UI\MenuOptions_Active.png}";
		else:
			MenuOptions.Source = "{Resource Textures\UI\MenuOptions_Inactive.png}";
		end;
	if currentActive == MENU_EXIT:
			MenuExit.Source = "{Resource Textures\UI\MenuExit_Active.png}";
		else:
			MenuExit.Source = "{Resource Textures\UI\MenuExit_Inactive.png}";
		end;
}

method Update(gameTime)
{
	if host.ActiveMenu != currentActive:
		ChangeActiveMenu();
		end;
		
	TextFPS.Text = host.FrameRate;
}
