
rem
echo off
set PATH=C:\Program Files (x86)\WiX Toolset v3.10\bin;%PATH%
set TARGET=XProof-X.X.X-setup.msi
cd "%1"bin\Release
candle ..\..\installer.wsx
light installer.wixobj
if exist "%TARGET%" del "%TARGET%"
ren installer.msi "%TARGET%"
