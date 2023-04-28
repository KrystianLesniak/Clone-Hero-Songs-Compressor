![Application logo](src/CloneHeroSongsCompressor/wwwroot/images/logo_transparent.png?raw=true)
***
Clone Hero Songs Compressor is a tool that significantly reduces the size of your Clone Hero songs library (by approximately 45%).<br>

![Application screenshoot](docs/img/app-screenshoot-darkmode.png?raw=true)


Note that this process is very resource hungry and can take up to several hours on slower PC. At my personal PC (i9-7900x) library **compression from size 11.4GB to 5.4GB took 9 minutes.** It is of course example and results may vary depending on the user's hardware and library size.

***
## Download & Usage Instructions

Note: Clone Hero Songs Compressor currently only supports Windows 64-bit OS.

1. Download the latest archive of the application from the "Release" page on the right-hand side of the page. Extract into seperate directory.

2. Run the executable "Clone Hero Songs Compressor.exe" in root directory, and select the "Add New Directory" button to specify the directory where your Clone Hero songs are located.

3. Select the "Start" button located at the bottom of the page to begin the compression process. The progress of the compression will be displayed on the application interface.

4. Wait until the compression process is complete. The amount of time this takes will depend on the size of your library.

5. Once the process is complete, close the application and open the settings menu in Clone Hero and rescan your songs. Your compressed songs will now be available to play in Clone Hero.

If you encounter any issues do not hesitate to create an [issue ticket via GitHub](https://github.com/KrystianLesniak/Clone-Hero-Songs-Compressor/issues)


***
## About
Such size reduction is achieved through the use of advanced compression and conversion techniques, such as those used by the Opus audio codec and JPEG image format.

Converting Audio files to Opus & PNG to JPG is handled by [FFmpeg](https://ffmpeg.org/), builded thanks to [media-autobuild_suite](https://github.com/m-ab-s/media-autobuild_suite) and wrapper [FFMpegCore](https://github.com/rosenbjerg/FFMpegCore)
<br>
UI is done with [MudBlazor](https://mudblazor.com/)

**** 
<sub>
DISCLAIMER: The authors of this project are not responsible for any loss of data or damages that may occur as a result of using this project. Users of this project are solely responsible their data. The authors of this project make no warranties or representations, express or implied, regarding the use or performance of this project. Use of this project is at your own risk.
</sub>

