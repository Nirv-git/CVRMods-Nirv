**Mods are NOT against CVR's TOS. However be smart when using them, and if you want to submit a bug report to CVR, test with --no-mods first**

**This was made in my free time, and is provided AS IS without warranty of any kind, express or implied. Any use is at your own risk and you accept all responsibility**


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

