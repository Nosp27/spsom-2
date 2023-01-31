#!/bin/sh

url=$1
name=$(sed "s;.*/\([^/]*\).aac;\1;g" <<< "$url")
echo $name
curl $url | ffmpeg -f aac -i - $name.wav
