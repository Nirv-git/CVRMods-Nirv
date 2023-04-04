**Mods are NOT against CVR's TOS. However be smart when using them, and if you want to submit a bug report to CVR, test with --no-mods first**

**This was made in my free time, and is provided AS IS without warranty of any kind, express or implied. Any use is at your own risk and you accept all responsibility**


# PortableMirror
This mod allows the user to locally spawn mirrors for themselves in any world.   

The mirror can be configured:
  * Allow/disallow mirror pickup
  * Toggle between full/optimized/cutout or transparent
	  * Subtypes
		  * Solo - This shows just you, available for both Cutout and Transparent | *Click Cutout/Trans Type button twice to access.*
		  * Combo - This shows a cutout mirror for you and transparent for others | *Click Trans Type again after Solo to access.*
  * Configurable mirror size and distance from you
  * Standard, 45 degree, ceiling, small, transparent and ~~calibration~~ mirrors 
	  * (Cal mirror is included in base game now!)
  * Optional feature to disable far avatars from rendering in the mirror
  * Weird experimental features!
	  * **Custom Pickup** for Base/Micro/Trans Mirrors when in VR
		  * Default off in MelonPrefs - When active the pickup icon on the menu will have a gear on it
		  * Grab by holding both the Trigger and Grip past 50%
		  * Features of this custom method is, integrated pickup push/pull with distance scaling. 
			  * You can disable the custom pickup laser in settings
	  * **Follow Gaze**
		  * Default off in MelonPrefs, after enabling this can be toggled by clicking the 'Anchor to Player' button twice. 
		  * This will make the mirror reposition itself to face the direction you are looking, hence 'gaze' 
		  * The Base mirror how has another weird experimental Follow Gaze mode, it will stay in a position till you turn a certain angle away from it for a certain number of seconds, it then will move to your current gaze direction and stay there after it settles. All customizable in settings, see below for more info
    
### Todo
  * Make the menu less ugly? BTKUI soon maybe?!

UI isn't the most pretty, but it works! And that is better than no UI. (And to answer why it is so far away from the QM, because the canvas for the QM is bigger then it looks and will eat interactions if you overlap it)
![image](https://user-images.githubusercontent.com/81605232/193378409-fb6ce679-db51-4cc9-a3fc-2cf23eace120.png)![image](https://user-images.githubusercontent.com/81605232/193378446-9ee438da-8e36-4493-9e84-3cc003ff1efe.png)




There are a few MelonPrefs that can be customized - I recommend using [UI Expansion Kit](https://api.cvrmg.com/v1/mods/download/90)

| MelonPref Name | Default Value | Desc |
|--|--|--|
| QuickMenu (QM) Starts Maximized | false  | |
| QM Position (0=Right, 1=Top, 2=Left) | 0  |  If the mirror menu is on the Left/Right/Top side of your QuickMenu |
| QM is smaller | false | Mirror menu is smaller (Mainly for Desktop) |
| Enabled color for QM (0=Orange, 1=Yellow, 2=Pink) | 0 |  |
| High Precision Distance Adjustment Value | 0.05 | Distance to use for adjustments when High Precision mode is toggled |
| Collider Depth | 0.001 | No clue why this is still an option... |
| Show frame when mirror is pickupable | false | Grey frame around the mirror when you can pick it up |
| Enable 'Follow Gaze' (FG) by clicking Anchor to Tracking button twice | false |  |
| FG Movement Speed | 0.5 | Lower is faster |
| FG DeadBand - Enabled (Base Mirror Only) | true | Enable/Disables the DeadBand mode |
| FG DeadBand - Break Angle | 60 | If you are looking greater then this angle away from the mirror it will 'break' from it's position.  |
| FG DeadBand - Seconds to wait after break (0 to disable) | 3 | How long you need to keep looking away from the mirror before it breaks away from it's position. |
| FG DeadBand - Settle Angle | 3 | Angle between your camera and mirror if it is less then this it considers it in front of you and stops moving |
| FG DeadBand - Settle Seconds (0 to disable) | 0.5 | How long to keep moving after being within the angle |
| Use custom mirror pickup in VR (Base/Micro/Trans mirrors) | false | Overrides the pickup function for Base/Micro/Trans mirrors. Grab by holding both the Trigger and Grip past 50% |
| Custom pickup push/pull speed | 5 |  |
| Custom pickup line | true  | Enables a LineRenderer to show pickup direction when  |
| Use PixelLights for mirrors | false | Lights will look better in mirrors, but cost more performance |
| Pickups snap to hand - Global for all mirrors  | false | When not using custom pickup, when grabbing a mirror it will move to your hand |
| Transparent Mirror transparency - Higher is more transparent - Global for all mirrors | 0.4 |  |
| Disable avatars > than a distance from showing in Mirrors | false | Switches distant avatars to another layer to avoid seeing them in the mirror |
| Disable Distance in meters | 3 |  |
| Disable Update interval | 0.5 |  |
| **PortableMirror Misc Settings** |  | Each mirror has the following settings that control it's size and type |
| Mirror Scale X | |  |
| Mirror Scale Y |  |  |
| Mirror Distance  |  | Distance mirror spawns from you |
| Mirror Type |  | Mirror type (Full, Optimized, Transparent, Cutout, ect) |
| Can Pickup Mirror |  |  |
| Position mirror based on view angle | | Will spawn mirror based on where you are looking instead of vertically in front of you |
| Mirror Follows You | | Mirror is parented to your player object, will move with you |
|Follow Gaze Enabled | | Uses Follow Gaze mode described above |
	

# NearClippingPlaneAdjuster
This is a mod for adjusting the near clipping plane for the player's camera. This can allow you to get much closer to objects before they start clipping and be used for seeing your limbs with a VERY small avatar. 

This mod requires [UI Expansion Kit](https://api.cvrmg.com/v1/mods/download/90)  
 **Supports: [BTKUILib](https://api.cvrmg.com/v1/mods/download/113)**

You can manually change the near clipping plane using the buttons in the UIX Mod Settings panel
![image](https://user-images.githubusercontent.com/81605232/219507900-2d4dd6c1-f53c-46be-9704-807655c47e65.png)   
Or using BTKUI with the NirvMisc or default Misc page. (Toggle in MelonPrefs)     
![image](https://user-images.githubusercontent.com/81605232/227616814-d0807e41-f3c0-4b23-990a-2c1315f40fe0.png)       
*There is a MelonPref to disable the mod from putting buttons here, if you desire.* 
![image](https://user-images.githubusercontent.com/81605232/219508687-ae13dd0e-7dbc-49f9-a8ce-ed7cc120cf2c.png)

The mod now has a (default enabled) option to **blacklist** certain worlds from automatically getting their Nearclipping planes adjusted for the 0.01 and 0.001 values. This is to stop the mod from harming user's experience on worlds that really need a further clipping plane to look good. _You can report any worlds needing to be blacklisted by opening a ticket._

This mod will set the near plane clipping distance to .01 15 seconds after you load into a world.  (Reason for the delay is that we can't know exactly when the world's referenceCamera's settings are copied onto the player's camera, ~~or maybe we can and I am just lazy~~)

Includes an option to adjust the Nearclipping Plane up to 0.05. This was suggested as a compatibility option for very large worlds where the Far Clipping Plane was getting pulled in too close by 0.01

* Keyboard Shortcuts - Enables keyboard shortcuts to set your clipping plane to the smallest or largest values.  **[** for 0.001 and  **]** for 0.05
* Smaller Default - Sets a smaller value on World Change - 0.001 vs 0.01   

# HeadLightMod
Requires [BTKUILib](https://api.cvrmg.com/v1/mods/download/113)
  
This mod adds a standalone headlight to your player. Included are options to swap between a Spot/Point light source and adjust the brightness, angle, range and color. 

You can open the HeadLight Menu using BTKUI with the NirvMisc or default Misc page. (Toggle in MelonPrefs)      
![image](https://user-images.githubusercontent.com/81605232/227616814-d0807e41-f3c0-4b23-990a-2c1315f40fe0.png)    
![image](https://user-images.githubusercontent.com/81605232/219509246-f255fd77-3edc-462b-b472-ce7e80a1dcbc.png)


# SitLaydownMod
This mod is meant to reproduce some of the functionality for custom [Sit/Laydown animations that could be added to an avatar in VRC](https://github.com/Dervali-git/VRC-Tips/blob/main/LaySittingPrefab.md) for half body users, however I didn't want to figure out how to do it with avatar animators and am now abusing chairs!  ~~Also this works on all avatars and I don't have to support people configuring it on their own one~~     

Current animations are, 'Lay down on back', 'Sit', 'Sit Crossed Legs', 'Sit Legs Down'   
   
![image](https://user-images.githubusercontent.com/81605232/187961673-05d763c9-0b06-4135-9e5b-aa374bd20d14.png)     
  
### Todo
  * Tweak starting pose offset in VR. Some animations start in the floor and need to be moved up with movement controls.      
  
Pressing the toggle chair button will place you into a chair at your current location with the selected sitting animation. The movement controls lets you adjust the location of the chair object. Rotation Lock stops your body from rotating in the chair when your head turns.      

![image](https://user-images.githubusercontent.com/81605232/187956905-3948aba9-3ff4-442b-8d85-e6cc08012fc9.png)     
**>>>>>>>>>> Use SeatExitController, seriously do it <<<<<<<<<<**  

If you find it too easy to leave chairs you can use [SeatExitController](https://api.cvrmg.com/v1/mods/download/49)   

**>>>>>>>>>> Use SeatExitController, seriously do it <<<<<<<<<<**  
> Prevents you from falling out of seats accidentally. Press both triggers in VR, or q and e in desktop to leave seats.   

**>>>>>>>>>> Use SeatExitController, seriously do it <<<<<<<<<<**  

There are a few MelonPrefs that can be customized that aren't included on the menu. (I recommend using https://github.com/sinai-dev/MelonPreferencesManager | For CVR use the Mono version)    
  * QuickMenu Starts Maximized - Exactly as it says, default state of the mirror menu attached to your QuickMenu     
  * QuickMenu Position (0=Right, 1=Top, 2=Left) - If the menu is on the Left/Right/Top side of your QuickMenu    
  * Enabled color for QuickMenu items (0=Orange, 1=Yellow, 2=Pink)    
  * Position Offset for settings toggle button - This offsets the toggle button that opens the setting menu, negative moves it further away from the center.     

# NoPlayerSelectWithQM

Simple mod, this will enable/disable other player's selection colliders when only your QuickMenu is open. Comes with a MelonPref to enable/disable the mod. 

# LocalLightMod
Have you ever thought 'This world's lighting sucks' or 'Its too dark' well I have the solution for you! This mod lets you configure and spawn local (Spot|Point|Directional) light sources with near full customizability and provides functionality for saving and loading presets.
  
  Now you may think 'just use props!' and that is true, but sometimes you want more control or the world doesn't allow them.
  
You can access the settings using BTKUI with the NirvMisc or default Misc page. (Toggle in MelonPrefs)      
![image](https://user-images.githubusercontent.com/81605232/227616814-d0807e41-f3c0-4b23-990a-2c1315f40fe0.png)    
![image](https://user-images.githubusercontent.com/81605232/226430837-04d0b56f-f5d3-4a0f-97c3-5abb203e8915.png)    
   
![image](https://user-images.githubusercontent.com/81605232/141375999-974173df-b6ca-4b5c-9350-88589d3e8106.png)    

# IKpresets
Provides an alternative menu for editing IK settings, includes options for automatically loading IK settings for specific avatars and 16 general slots to store presets. 
  
You can access the settings using BTKUI with the NirvMisc or default Misc page. (Toggle in MelonPrefs)               
![image](https://user-images.githubusercontent.com/81605232/227616814-d0807e41-f3c0-4b23-990a-2c1315f40fe0.png)     
![image](https://user-images.githubusercontent.com/81605232/226432554-af50ec94-3e17-4f32-97ec-a52052a81828.png)  

# WorldPropListMod
Provides a list of all the props in a world with details such as the player that spawned it, distance and location. You can delete + block individual props and highlight or create a line towards their location.     
     
You can access the settings using BTKUI with the 'Props' page.    
![image](https://user-images.githubusercontent.com/81605232/229889735-8e482ca6-1c86-464f-aa65-1eafcd53f332.png)   
     
      
