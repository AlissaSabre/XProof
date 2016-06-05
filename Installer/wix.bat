
rem
echo off
set PATH=%WIX%bin;%PATH%
set TARGET=XProof-X.X.X-setup.msi
cd "%1"bin\Release
candle ..\..\setup.wsx
light setup.wixobj
Installer setup.msi XProof.exe