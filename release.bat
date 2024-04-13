@echo off

IF %1.==. GOTO KeyError
set apikey=%1

GOTO Begin

:KeyError
ECHO.
ECHO ERROR: No apikey was specified
ECHO.

GOTO End

:Begin

ECHO.
ECHO Cleaning up...
ECHO.

IF EXIST "%~dp0\ReleaseBuilds" (
    rmdir "%~dp0\ReleaseBuilds" /s /q
)

mkdir "%~dp0\ReleaseBuilds"

rem Cleaning Builds...
dotnet clean -c Release ReqIFSharp.sln

ECHO.
ECHO Packing nugets...
ECHO.

rem Packing New Versions...
dotnet pack -c Release -o ReleaseBuilds ReqIFSharp.sln

ECHO.
ECHO Pushing to nuget.org ...
ECHO.

dotnet nuget push ReleaseBuilds\*.nupkg -s api.nuget.org -k %apikey% --skip-duplicate

:End

ECHO.
ECHO Release Completed
ECHO.