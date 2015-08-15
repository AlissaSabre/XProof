
echo off
PATH=C:\Program Files (x86)\WiX Toolset v3.9\bin;%PATH%
cd "%1"bin\Release
candle ..\..\installer.wsx
light installer.wixobj
del XProof-X.X.X-setup.msi
ren installer.msi XProof-X.X.X-setup.msi
