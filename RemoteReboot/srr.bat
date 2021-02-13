rem @echo off
set SVCNAME=RemoteReboot
sc.exe delete %SVCNAME%
sc.exe create %SVCNAME% binPath= %~dp0\%SVCNAME%.exe DisplayName= %SVCNAME% start= auto
sc.exe description %SVCNAME% "Allow remote reboot"