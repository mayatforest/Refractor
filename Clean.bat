@echo off

rmdir /S /Q ".\Refractor\obj" >NUL

rmdir /S /Q ".\Refractor.Plugins.ILDecompiler\obj" >NUL

rmdir /S /Q ".\Refractor.Plugins.ILDiagrams\obj" >NUL

rmdir /S /Q ".\Refractor.Plugins.JScript\obj" >NUL


del /Q ".\bin\*.dll" >NUL
del /Q ".\bin\*.exe" >NUL
del /Q ".\bin\*.pdb" >NUL

del /Q ".\Refractor\*.db
del /Q ".\Refractor\*.suo
del /Q ".\Refractor\*.user




pause

