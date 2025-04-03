# VoiceFallOffCurveAdj
   
This mod allows you to edit the Audio Falloff Curve for other users voice communication. This can allow you to adjust the falloff so further players are still audible, but much quieter. 

Settings are available in the UIX Mod Settings and the **UI IS BAD**  

 - *Change Volume Curve** will enable this mod's custom curve when checked   
 - Then you can specify three points on the falloff curve. 
	- Each point has a Position which must be between 0.15 (closest to player) and .95 (furthest away).
		 - Each following point must be a higher Position then the previous
			 - Example Positions: Point 1: 0.5, Point 2: 0.75, Point 3: 0.8
	 - Each Point has a Value, this is used for volume on the curve
		 - Each following point must be a lower Value then the previous
			 - Example Values: Point 1: 0.75, Point 2: 0.25, Point 3: 0.01
   - The curve has three preset points for the start/end of the curve
     - Position 0 Value 1, Position 0.15 Value 1, Position 1 Value 0

A curve will then be generated to fit your points.    

![Image](https://github.com/user-attachments/assets/682a4004-39ca-49ff-acc1-67b28dcf4024)

After making changes, or togging to use the Custom Curve a visualization will print in your Melon loader console window. This shows an example of a custom curve and the points used to make it.     

![Image](https://github.com/user-attachments/assets/2ec014ee-af38-486d-86a1-d7897cd45126)