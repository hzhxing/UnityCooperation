@echo on
%~d0
cd %~dp0\ProjectDev\Assets\Project
IF EXIST "Art" (del Art)
mklink /d Art ..\..\..\ProjectArt\Assets\Project\Art
@pause