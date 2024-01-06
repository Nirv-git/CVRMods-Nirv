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