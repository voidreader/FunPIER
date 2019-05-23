@echo off

FontImgGenerator -sf kysp_symbols1.txt -ffn UnGungseo.ttf -fs 36 -m 10 -c 0
FontImgGenerator -sf kysp_symbols2.txt -ffn NanumBarunpenB.ttf -fs 42 -m 12 -c 0


move UnGungseo-10.png GrecoStd-DB.png
move UnGungseo.idx GrecoStd-DB.idx
copy GrecoStd-DB.png C:\Git\PierCorp-Unity\KYSP_EN\Assets\Fonts
copy GrecoStd-DB.idx C:\Git\PierCorp-Unity\KYSP_EN\Assets\Fonts\GrecoStd-DB.txt
move NanumBarunpenB-12.png Kurita.png
move NanumBarunpenB.idx Kurita.idx
copy Kurita.png C:\Git\PierCorp-Unity\KYSP_EN\Assets\Fonts
copy Kurita.idx C:\Git\PierCorp-Unity\KYSP_EN\Assets\Fonts\Kurita.txt

