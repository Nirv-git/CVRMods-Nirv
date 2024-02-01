# PersonalGravity
   
This mod provides options for controlling your own gravity locally!    
   
 - Gravity control options,   
	 - Towards a world space coordinate   
		 - Y+, Y- (Normally down), X+, X-, Z+, Z-   
	 - Surface normal of a raycast hit onto a collider, setting the gravity direction to match the angle of the collider.   
		 - Desktop: Raycast out from your camera    
		 - VR: Raycast out from your right hand in VR   
			 - Click your trigger to select and press both grips to cancel   
	 - Towards the direction you are currently looking      
	 -  The down vector from your current look direction   
      
 - Settings    
	 - Gravity Strength (0-20)   
	 - Aligned with gravity: If your player will rotate to match the direction   
	 - Only Effect Players: If the gravity zone will just effect you, or also props/objects   
	 - Mix Type: Sets the Gravity zone to Override (replace world gravity) or Additive (add to existing gravity)   
	 - Gravity Priority: Priority of the zone, higher numbers take priority over lower ones   
	 - Auto Toggle with Raycast: If the gravity zone should automatically toggle on when you use the raycast option   
	 - Snap to Angle: For the 'Current Gaze' options, will snap to .2 angle values   

> Before anyone asks, no this is not intended to bypass any world   
> restrictions. The mod only functions in worlds that allow Flight or   
> Props, and gravity effecting objects is only allowed in worlds that   
> allow Props. Nothing this mod does is more than what a prop can do.   

You access this mod using BTKUI, either it's own custom page, or with the NirvMisc page. (Toggle in MelonPrefs)           
![image](https://github.com/Nirv-git/CVRMods-Nirv/assets/81605232/d10d3924-88e6-48a4-ad47-11c1059edf24)
