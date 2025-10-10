@echo off
echo ================================================================================
echo distribution.bat
setlocal enabledelayedexpansion
set CURRENT_DIRECTORY=%cd%
set SOURCE_DIRECTORY=%CURRENT_DIRECTORY%\bin\Release
set SOURCE_FILE_PATTERN=%SOURCE_DIRECTORY%\*.nupkg
set NUGET_API_URL=https://api.nuget.org/v3/index.json
if exist "nugetapikey" (
	set /p NUGET_API_KEY=<"nugetapikey"
) else (
	set NUGET_API_KEY=
)

echo --------------------------------------------------------------------------------
echo CURRENT_DIRECTORY: %CURRENT_DIRECTORY%
echo SOURCE_DIRECTORY: %SOURCE_DIRECTORY%
echo SOURCE_FILE_PATTERN: %SOURCE_FILE_PATTERN%
echo NUGET_API_URL: %NUGET_API_URL%
echo NUGET_API_KEY: %NUGET_API_KEY%

echo --------------------------------------------------------------------------------
::for %%F in (%SOURCE_FILE_PATTERN%) do (
::	set FILEPATH=%%F
::	echo FILEPATH: !FILEPATH!
::
::	dotnet nuget push "!FILEPATH!" --api-key %NUGET_API_KEY% --source %NUGET_API_URL% --skip-duplicate --force-english-output
::)

endlocal
echo ================================================================================