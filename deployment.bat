@echo off
setlocal enabledelayedexpansion
echo ================================================================================
echo deployment.bat

echo --------------------------------------------------------------------------------
set CURRENT_DIRECTORY=%cd%
set SOURCE_DIRECTORY=%CURRENT_DIRECTORY%\bin\Release\netstandard2.1

echo --------------------------------------------------------------------------------
echo CURRENT_DIRECTORY: %CURRENT_DIRECTORY%
echo SOURCE_DIRECTORY: %SOURCE_DIRECTORY%

echo --------------------------------------------------------------------------------
:: for %%F in (%COPYFILES%) do (
:: 	set FILEPATH=%SOURCE_DIRECTORY%\%%F
:: 	echo FILEPATH: !FILEPATH!
:: 	xcopy /y "!FILEPATH!" "%DESTINATION_DIRECTORY%\"
:: )
set DESTINATION_DIRECTORY=D:\Github\Crockhead.Unity.Workbench\Assets\Plugins\Crockhead
for %%F in (%*) do (
 	set FILEPATH=%%F
 	echo FILEPATH: !FILEPATH!
 	xcopy /y "!FILEPATH!" "!DESTINATION_DIRECTORY!\"
)
set DESTINATION_DIRECTORY=D:\Github\MillenniumOfCultivation\Assets\Plugins\Crockhead
for %%F in (%*) do (
 	set FILEPATH=%%F
 	echo FILEPATH: !FILEPATH!
 	xcopy /y "!FILEPATH!" "%DESTINATION_DIRECTORY%\"
)

echo ================================================================================
endlocal