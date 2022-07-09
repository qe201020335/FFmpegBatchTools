# FFmpegBatchTools
Some small tools I use to do stuff on videos in batch. (not necessarily using ffmpeg)

## General Usage
Drag video files on to the `exe` executable on Windows. A default config json file may be created (if I coded one) in the same directory as the executable upon first launch.

**No Linux and Mac support** at the moment due to a dependency on Windows Forms and hardcoded process executable names. 

## BatchCompress
Encode video with H.264 NVEnc and delete the original file if output file is smaller. Detail behavior configurable in config json.

**Require [`NVEncC64.exe`](https://github.com/rigaya/NVEnc) accessible on PATH**

## BatchExtract
Extract some tracks of the input file to a single output file. No config file at the moment.


**Require [`ffmpeg.exe`](https://ffmpeg.org/) accessible on PATH**
