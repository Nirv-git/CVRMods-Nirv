**Mods are NOT against CVR's TOS. However be smart when using them, and if you want to submit a bug report to CVR, test with --no-mods first**

**This was made in my free time, and is provided AS IS without warranty of any kind, express or implied. Any use is at your own risk and you accept all responsibility**

# Mods not published on CVRMG

[HideDragonWingsMod](HideDragonWingsMod) [Download](https://github.com/Nirv-git/CVRMods-Nirv/releases/download/BTKUI_2-Updates/HideDragonWings.dll)     
[QMShutdownOptionsMod](QMShutdownOptionsMod) [Download](https://github.com/Nirv-git/CVRMods-Nirv/releases/download/BTKUI_2-Updates/QMShutdownOptionsMod.dll)    
[WorldDetailsPage](WorldDetailsPage) [Download](https://github.com/Nirv-git/CVRMods-Nirv/releases/download/BTKUI_2-Updates/WorldDetailsPage.dll) (Mostly outdated after 2023r173, but still works)     
[PlayerLocator](PlayerLocator) [Download](https://github.com/Nirv-git/CVRMods-Nirv/releases/download/WorldProp0.5.15_NoHeadShrink0.5.6_WorldDetails0.0.1_PlayerLoc0.0.1_RemChairs0.0.1/PlayerLocator.dll)            
[ShrinkOtherHeads](ShrinkOtherHeads) [Download]()    
[VisemeValue](VisemeValue) [Download](https://github.com/Nirv-git/CVRMods-Nirv/releases/download/ViseValue0.0.2_LocalLight0.7.2_ViewPointTweaks0.7.2_IKpreset0.7.2_PortMirror2.1.16_WorldProp0.7.2/VisemeValue.dll)[PersonalGravity](PersonalGravity) [Download]()    
[FlightBinding](FlightBinding) [Download]()           

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

**Farclipping Plane** off by default, there is an option for adjusting the far clipping plane in the BTKUI menu. The mod will limit the ratio betwee Near and Far Planes to prevent Unity from exploding.

* Keyboard Shortcuts - Enables keyboard shortcuts to set your clipping plane to the smallest or largest values.  **[** for 0.001 and  **]** for 0.05
* Smaller Default - Sets a smaller value on World Change - 0.001 vs 0.01   

# HeadLightMod
Requires [BTKUILib](https://api.cvrmg.com/v1/mods/download/113)
  
This mod adds a standalone headlight to your player. Included are options to swap between a Spot/Point light source and adjust the brightness, angle, range and color. 

You can open the HeadLight Menu using BTKUI with the NirvMisc or default Misc page. (Toggle in MelonPrefs)      
![image](https://user-images.githubusercontent.com/81605232/227616814-d0807e41-f3c0-4b23-990a-2c1315f40fe0.png)    
![image](https://user-images.githubusercontent.com/81605232/219509246-f255fd77-3edc-462b-b472-ce7e80a1dcbc.png)

# SitLaydownMod
This mod is meant to reproduce some of the functionality for custom [Sit/Laydown animations that could be added to an avatar in VRC](https://github.com/Dervali-git/VRC-Tips/blob/main/LaySittingPrefab.md) for half body users, however I didn't want to figure out how to do it with avatar animators and am now abusing chairs!  Also this works on all avatars and I don't have to support people configuring it on their own one!

Current animations are, 'Lay down on back', 'Sit', 'Sit Crossed Legs', 'Sit Legs Down'   
   
![image](https://user-images.githubusercontent.com/81605232/187961673-05d763c9-0b06-4135-9e5b-aa374bd20d14.png)     
Pressing the toggle chair button will place you into a chair at your current location with the selected sitting animation. The movement controls lets you adjust the location of the chair object.

![image](https://github.com/Nirv-git/CVRMods-Nirv/assets/81605232/68648ca1-b509-4fdc-9310-bfeab78a08f5)

There are a few MelonPrefs that can be customized that aren't included on the menu.
  * Joystick movement multiplier (1-10)
  * Joystick rotation multiplier (1-10)
  * Prevent leaving the seat unless you use the menu or respawn - This keeps the big menu button from making you leave the seat. You must toggle off the seat in the Mod UI
  * Disables adjusting offsets 120 seconds after last adjustment - Will disable to 'Adjust Offsets' option for VR users 120 seconds after the last input. 


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
Provides a list of all the props in a world with details such as the player that spawned it, distance and location, tags and more. You can delete + block individual props and highlight or create a line towards their location.     
     
You can access the settings using BTKUI with the 'Props' page. (Toggle in MelonPrefs to switch to NirvMisc Page)      
![image](https://user-images.githubusercontent.com/81605232/229889735-8e482ca6-1c86-464f-aa65-1eafcd53f332.png)   


# NoHeadShrinkMod
By default your local avatar's head is shrunk down to make it not visible in the normal first person camera.      

This mod adds options for disabling your local avatar's head from shrinking completely, or to auto un-shrink it if your camera is a certain distance from your avatar.     
**Functionality is disabled by default**   
This is a niche mod, but can be very useful in certain circumstances.     
           
Optional support for BTKUI with settings in the NirvMisc or default Misc page. (Toggle in MelonPrefs)                
![image](https://user-images.githubusercontent.com/81605232/233871748-ecc13889-4849-44fe-b284-2669e04be5f8.png)


# RemoveChairs
Buttons to simply disable or enable active chairs in the world. (For those worlds covered in chair with no easy toggle to disable them)
_You understand that disabling stuff in a world can break stuff_ 
 
You can access the settings using BTKUI with the NirvMisc or default Misc page. (Toggle in MelonPrefs)   
![image](https://github.com/Nirv-git/CVRMods-Nirv/assets/81605232/a851bc77-659b-4542-bef7-f86367bbb240)


# ViewPointTweaks
**[VR Only]**
This mod provides the ability to change your avatar's viewpoint position+rotation (really the camera) live in game, along with the much more niche feature of adjusting the scale of the VR camera to adjust it's 'IPD'.     
  
You can save these settings per avatar and have them autoload.   
 
You can access the settings using BTKUI with the NirvMisc or default Misc page. (Toggle in MelonPrefs)     
![image](https://github.com/Nirv-git/CVRMods-Nirv/assets/81605232/9e16a67a-e2c0-49d5-8649-a704e955c671)    

The way this mod functions is a _tad_ hacky, it grew from an odd start. 


# NoMenuTouch
CVR be default allows you to 'touch' your MainMenu and QuickMenu as an input option, while neat this can result in unintentional inputs specially when you open your QuickMenu with your hands near each other.    
**This mod simply disables that feature with a toggle in UIX/MelonPrefs.**    
![image](https://github.com/Nirv-git/CVRMods-Nirv/assets/81605232/fc713e2d-3152-46e4-9f27-1d8d4eba70d3) 


# CVRCameraDontOverrender

Stops the Camera from over rendering your local player. Now you can see your hands when interacting with it!    

*Yes I know there is the experimental option to have your player overrender the UI, but that is the ENTIRE UI.*       

This toggle can be accessed from UIX/Melonprefs         
![image](https://github.com/Nirv-git/CVRMods-Nirv/assets/81605232/e40b99a5-a038-42a6-999b-c370e5ad08dd)  

# VisemeValue
This mod exposes your current viseme states for use in avatars. (Much like the Parameter Stream for Viseme Loudness)        

Settings:       
- `Attempt to Drive Viseme Parameters if they exists`  default value `true`
	- Enables the mod
- `Update Rate (ms)` default value `10`
	- Update rate, however your animator only syncs over the network at a rate of 20hz (every 50ms)
- `Use 'VisemeMod_Value' (int) for current Viseme`  default value `true`
	- Drives this parameter name to the current loudest viseme, see below for the reference chart.
- `Use 'Viseme' (int) for current Viseme`  default value `false`
	- Drives this parameter name to the current loudest viseme, see below for the reference chart. (Same as above, just uses the generic 'Viseme' parameter.
	- Off by default to prevent isses for avatars that don't expect this parameter to be driven.
- `Use 'Use 'VisemeMod_Level' (float) for current Intensity`  default value `true`
- This should be the same as the native Parameter Stream for Viseme Loudness.
- `Individual Parameters 'VisemeMod_xx' (float) for all possible visems 0-14 (sil must exist)`  default value `true`
	- This will drive all 15 visemes as individual parameters. If you use this option, please optimize your animator, [Direct Blend Trees](https://notes.sleightly.dev/dbt-combining/) are a good option.
 
 You will need a `Face Mesh` defined and `Use Lip Sync` checked. You do not need any visemes selected        
![image](https://github.com/Nirv-git/CVRMods-Nirv/assets/81605232/4b4c8c0e-2e8d-4f43-9e3d-3092a1246b21)


For reference this uses the [Oculus Lipsync Visemes](https://developer.oculus.com/documentation/unity/audio-ovrlipsync-viseme-reference)      
|Viseme Parameter | Viseme|
|--|--|
|0	|sil|
|1	|pp|
|2	|ff|
|3	|th|
|4	|dd|
|5	|kk|
|6	|ch|
|7	|ss|
|8	|nn|
|9	|rr|
|10	|aa|
|11	|e|
|12	|i|
|13	|o|
|14	|u|
       
