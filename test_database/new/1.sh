#!/bin/bash
cd bus_sign
count=1
for file in *.jpg
do
  mv "$file" "bus_sign$count.jpg"
  ((count++))
done