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
	
