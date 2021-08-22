@echo off

REM Requires dotnet CLI
REM Copy this file to where you want to build a Machina Project, then run it with the desired project name

if [%1]==[] goto usage

REM Create repo
git init
git submodule add https://github.com/notexplosive/machina.git

REM MonoGame first time setup
dotnet new --install MonoGame.Templates.CSharp
dotnet tool install --global dotnet-mgcb-editor
mgcb-editor --register

REM Build default OpenGL project
dotnet new mgdesktopgl -o %1

REM Create an sln and add projects to it
dotnet new sln
dotnet sln add %1
dotnet sln add .\machina\Machina
dotnet sln add .\machina\TestMachina

REM Add Machina as a dependency on your project
dotnet add %1 reference .\machina\Machina\

REM Pull in the default gitignore and readme.md
xcopy .\machina\.gitignore .
echo "F" | xcopy .\machina\game-readme.md readme.md
git add .
git commit -m "(Machina:Automated) Initial Commit"
pause
goto :eof

:usage
@echo Usage: %0 ^<NewProjectName^>
pause
exit /B 1
