**Mods are NOT against CVR's TOS. However be smart when using them, and if you want to submit a bug report to CVR, test with --no-mods first**

**This was made in my free time, and is provided AS IS without warranty of any kind, express or implied. Any use is at your own risk and you accept all responsibility**


# PortableMirror
This mod allows the user to locally spawn mirrors for themselves in any world.   

The mirror can be configured:
  * Allow/disallow mirror pickup
  * Toggle between full/optimized/cutout or transparent
	  * Subtypes
		  * Solo - This shows just you, available for both Cutout and Transparent | Click Cutout/Trans Type button twice to access.
		  * Combo - This shows a cutout mirror for you and transparent for others | Click Trans Type again after Solo to access.
  * Configurable mirror size and distance from you
  * Standard, 45 degree, ceiling, small and calibration mirrors
	  * Calibration mirror can be configured to remain on for a few seconds after calibrating, this can be enabled with a MelonPref
  * Optional feature to disable far avatars from rendering in the mirror
    
### Todo
  * Make the menu less ugly?

UI isn't the most pretty, but it works! And that is better than no UI. (And to answer why it is so far away from the QM, because the canvas for the QM is bigger then it looks and will eat interactions if you overlap it)
![image](https://user-images.githubusercontent.com/81605232/193378409-fb6ce679-db51-4cc9-a3fc-2cf23eace120.png)![image](https://user-images.githubusercontent.com/81605232/193378446-9ee438da-8e36-4493-9e84-3cc003ff1efe.png)




There are a few MelonPrefs that can be customized - I recommend using [UI Expansion Kit](https://api.cvrmg.com/v1/mods/download/90)
  * QuickMenu Starts Maximized - Exactly as it says, default state of the mirror menu attached to your QuickMenu
  * QuickMenu Position (0=Right, 1=Top, 2=Left) - If the mirror menu is on the Left/Right/Top side of your QuickMenu
  * Enabled color for QuickMenu items (0=Orange, 1=Yellow, 2=Pink)
  * QuickMenu is smaller - Mirror menu is smaller (Mainly for Desktop)
  * Enabled Mirror Keybind - If you want the hell that is the keymap I made for it! [image](https://user-images.githubusercontent.com/81605232/184995574-2e2cc5a6-4265-4e1b-97e5-d7a5eb304519.png)
	

# NearClippingPlaneAdjuster
This is a mod for adjusting the near clipping plane for the player's camera. This can allow you to get much closer to objects before they start clipping and be used for seeing your limbs with a VERY small avatar. 

This mod requires [UI Expansion Kit](https://api.cvrmg.com/v1/mods/download/90)  
 **Supports: [BTKUILib](https://api.cvrmg.com/v1/mods/download/113)**

You can manually change the near clipping plane using the buttons in the UIX Mod Settings panel
![image](https://user-images.githubusercontent.com/81605232/219507900-2d4dd6c1-f53c-46be-9704-807655c47e65.png)   
Or you can use the BTKUI Misc page. 
*There is a MelonPref to disable the mod from putting buttons here, if you desire.* 
![image](https://user-images.githubusercontent.com/81605232/219508687-ae13dd0e-7dbc-49f9-a8ce-ed7cc120cf2c.png)

The mod now has a (default enabled) option to **blacklist** certain worlds from automatically getting their Nearclipping planes adjusted for the 0.01 and 0.001 values. This is to stop the mod from harming user's experience on worlds that really need a further clipping plane to look good. _You can report any worlds needing to be blacklisted by opening a ticket._

This mod will set the near plane clipping distance to .01 15 seconds after you load into a world.  (Reason for the delay is that we can't know exactly when the world's referenceCamera's settings are copied onto the player's camera, ~~or maybe we can and I am just lazy~~)

Includes an option to adjust the Nearclipping Plane up to 0.05. This was suggested as a compatibility option for very large worlds where the Far Clipping Plane was getting pulled in too close by 0.01

* Keyboard Shortcuts - Enables keyboard shortcuts to set your clipping plane to the smallest or largest values.  **[** for 0.001 and  **]** for 0.05
* Smaller Default - Sets a smaller value on World Change - 0.001 vs 0.01   

# LocalHeadLightMod
Requires [BTKUILib](https://api.cvrmg.com/v1/mods/download/113)
  
This mod adds a standalone headlight to your player. Included are options to swap between a Spot/Point light source and adjust the brightness, angle, range and color. 

You can open the HeadLight Menu with the BTKUI Misc page.
![image](https://user-images.githubusercontent.com/81605232/219509246-f255fd77-3edc-462b-b472-ce7e80a1dcbc.png)



# SitLaydownMod
This mod is meant to reproduce some of the functionality for custom [Sit/Laydown animations that could be added to an avatar in VRC](https://github.com/Dervali-git/VRC-Tips/blob/main/LaySittingPrefab.md) for half body users, however I didn't want to figure out how to do it with avatar animators and am now abusing chairs!  ~~Also this works on all avatars and I don't have to support people configuring it on their own one~~     

Current animations are, 'Lay down on back', 'Sit', 'Sit Crossed Legs', 'Sit Legs Down'   
   
![image](https://user-images.githubusercontent.com/81605232/187961673-05d763c9-0b06-4135-9e5b-aa374bd20d14.png)     
  
### Todo
  * Tweak starting pose offset in VR. Some animations start in the floor and need to be moved up with movement controls.      
  
Pressing the toggle chair button will place you into a chair at your current location with the selected sitting animation. The movement controls lets you adjust the location of the chair object. Rotation Lock stops your body from rotating in the chair when your head turns.      

![image](https://user-images.githubusercontent.com/81605232/187956905-3948aba9-3ff4-442b-8d85-e6cc08012fc9.png)     

If you find it too easy to leave chairs you can use [SeatExitController](https://api.cvrmg.com/v1/mods/download/49)   

> Prevents you from falling out of seats accidentally. Press both triggers in VR, or q and e in desktop to leave seats.   

**>>>>>>>>>> Use SeatExitController, seriously do it <<<<<<<<<<**

There are a few MelonPrefs that can be customized that aren't included on the menu. (I recommend using https://github.com/sinai-dev/MelonPreferencesManager | For CVR use the Mono version)    
  * QuickMenu Starts Maximized - Exactly as it says, default state of the mirror menu attached to your QuickMenu     
  * QuickMenu Position (0=Right, 1=Top, 2=Left) - If the menu is on the Left/Right/Top side of your QuickMenu    
  * Enabled color for QuickMenu items (0=Orange, 1=Yellow, 2=Pink)    
  * Position Offset for settings toggle button - This offsets the toggle button that opens the setting menu, negative moves it further away from the center.     

# NoPlayerSelectWithQM

Simple mod, this will enable/disable other player's selection colliders when only your QuickMenu is open. Comes with a MelonPref to enable/disable the mod. 

# ~~NamePlateWithQMmod~~
Outdated 10/1/2022 - Added to game

~~Simple mod, this will enable/disable Nameplates when your QuickMenu is open, similar to VRC. 
There is a MelonPref that lets you disable this behavior.~~
~~'Show Nameplates' in CVR settings must be enabled~~
(https://user-images.githubusercontent.com/81605232/185835628-ff2e0b0c-cd8a-429e-b58b-7502fb594846.png)

# ~~VoiceFalloffAdjMod~~
Outdated 9/17/2022 - Added to game

~~A simple mod that adds the ability to change the max voice distance for other players in a range from 1-14.5 meters. *The real max is 10meters, I assume as a bandwidth saving feature(?), however I have the slider max out at 14.5 so you can hear someone at that 10meter point)~~
~~The UI is the same as PortableMirror and is aligned not to conflict with it. In Melon Prefs you can change the position of the menu. (I recommend using https://github.com/sinai-dev/MelonPreferencesManager | For CVR use the Mono version)~~   
  * ~~QuickMenu Position (0=Right, 1=Top, 2=Left) - If the mirror menu is on the Left/Right/Top side of your QuickMenu~~
  * ~~Enabled color for QuickMenu items (0=Orange, 1=Yellow, 2=Pink)~~
(https://user-images.githubusercontent.com/81605232/185814627-5f02b27c-abb6-4e48-ac7e-d26888e1b90e.png)
