
rem
echo off
set PATH=%WIX%bin;%PATH%
set TARGET=XProof-X.X.X-setup.msi
cd "%1"bin\Release
candle ..\..\installer.wsx
light installer.wixobj
if exist "%TARGET%" del "%TARGET%"
ren installer.msi "%TARGET%"
