@echo off
if [%1]==[] goto usage

git init
git submodule add https://github.com/notexplosive/machina.git
dotnet new mgdesktopgl -o %1
dotnet new sln
dotnet sln add %1
dotnet sln add .\machina\Machina
dotnet sln add .\machina\TestMachina
dotnet add %1 reference .\machina\Machina\
call .\machina\on-import.bat
xcopy .\machina\.gitignore ..
git add .
git commit -m "(Machina:Automated) Initial Commit"
pause
goto :eof

:usage
@echo Usage: %0 ^<DesiredProjectName^>
pause
exit /B 1