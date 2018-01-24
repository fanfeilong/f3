- merge mp3
```
ffmpeg -i 1.mp3 -i 2.mp3 -filter_complex [0:a][1:a]concat=n=2:v=0:a=1 out.mp3
```

- auto detect media format
```
ffprobe -v error -select_streams v:0 -show_entries stream=codec_name -of default=nokey=1:noprint_wrappers=1 test.mp4 
```

- convert mpg to mp4
```
ffmpeg -i 1.hello.mp4 -strict experimental -f mp4 -vcodec libx264 -acodec aac -ab 160000 -ac 2 -preset veryslow -crf 10 1.hello.real.mp4
```

```
ffmpeg -i $in -c:v libx264 -c:a aac -crf 10 -preset:v veryslow $out
```

- split audio/vedio
```
ffmpeg -i input_file -vcodec copy -an output_file_video　　//split autio
ffmpeg -i input_file -acodec copy -vn output_file_audio　　//split vedio
```

- convert
```
ffmpeg –i test.mp4 –vcodec h264 –s 352*278 –an –f m4v test.264              //转码为码流原始文件
ffmpeg –i test.mp4 –vcodec h264 –bf 0 –g 25 –s 352*278 –an –f m4v test.264  //转码为码流原始文件
ffmpeg –i test.avi -vcodec mpeg4 –vtag xvid –qsame test_xvid.avi            //转码为封装文件
//-bf B帧数目控制，-g 关键帧间隔控制，-s 分辨率控制
```

- cut
```
ffmpeg –i test.avi –r 1 –f image2 image-%3d.jpeg        //提取图片
ffmpeg -ss 0:1:30 -t 0:0:20 -i input.avi -vcodec copy -acodec copy output.avi    //剪切视频
//-r 提取图像的频率，-ss 开始时间，-t 持续时间
```

- Play YUV
```
ffplay -f rawvideo -video_size 1920x1080 input.yuv
```

- YUV to AVI
```
ffmpeg –s w*h –pix_fmt yuv420p –i input.yuv –vcodec mpeg4 output.avi
```


主要参数： -i 设定输入流 -f 设定输出格式 -ss 开始时间 视频参数： -b 设定视频流量，默认为200Kbit/s -r 设定帧速率，默认为25 -s 设定画面的宽与高 -aspect 设定画面的比例 -vn 不处理视频 -vcodec 设定视频编解码器，未设定时则使用与输入流相同的编解码器 音频参数： -ar 设定采样率 -ac 设定声音的Channel数 -acodec 设定声音编解码器，未设定时则使用与输入流相同的编解码器 -an 不处理音频


- gen gif 
```
ffmpeg -i capx.mp4 -t 10 -s 320x240 -pix_fmt rgb24 jidu1.gif
```