@echo off
REM This script gets run by the auto-import script from the outside

REM Pull out the .gitignore, this will ignore a bunch of cruft as well as .demo files and the like
xcopy .gitignore ..