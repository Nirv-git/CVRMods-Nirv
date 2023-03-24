**Mods are NOT against CVR's TOS. However be smart when using them, and if you want to submit a bug report to CVR, test with --no-mods first**

**This was made in my free time, and is provided AS IS without warranty of any kind, express or implied. Any use is at your own risk and you accept all responsibility**


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

