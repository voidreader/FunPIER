Animated GIF Player

Main features:

-Plays animated GIFS on any texture.
-Can load GIFs from local storage or web
-Easy to use
-GIFs are decoded in a seperate thread for performance
-Caching of animated frames
-Examples included


Updates:


V1.14.0
	-Various bugfixes
	-Added compatibility mode to allow playback of all GIF encoding methods
V1.13.5
	-Fixed looping when using the threaded decoder and caching frames
V1.13.4
	-Fixed a potential memory leak when changing caching settings during run time
V1.13.3
	-Fixed crash when reloading a GIF when using the threaded decoder
V1.13.2
	-Fixed crash when switching GIF images if the first image wasn't done loading
V1.13.1
	-Fixed Android builds on Unity 2017.2 and higher
	-Fixed tmp texture not being destroyed when loading a new gif with the same Animated Gif Player component
V1.13
	-Fixed issue with Gif files not loading when using the Unity editor on Mac Os
	-Fixed bug where in some cases the first frame of the Gif was incorrect
	-Sprites are no longer resized after starting Gif playback on them
	-Gif target sprites are created as full rect instead of tight mesh
	-Gif target textures are no longer created twice. This should improve startup performance somewhat for large Gifs
V1.12
	-Gifs can now be loaded from: Application.streamingAssetsPath (default), Application.persistentDataPath, Application.temporaryCachePath as well as http and https
V1.11
	-Added controls for playback speed under advanced options
V1.1
	-Changed namespace to OldMoatGames. If you are updating from 1.0 change "using AnimatedGifPlayer;" to "using OldMoatGames;" in your code
	-Can load GIFs from web instead of just StreamingAssetsFolder. Only usable from code. See the code example scene for more info 
	-Fixed the duration of the visibility of the first frame when resuming playback with the Play() method
	-Added OnLoadError event
	-Added Width and Height vars that return the size of the GIF once loaded
V1.02
	-Fixed playback on WebGL
V1.01
	-Bug fixes
V1.0
	-First release