@echo off
REM === Batch file to run ConverterCore.py with preset arguments ===
REM Workspace ID
set WID=fe4f891cbcd547458b1679f0c642fe9b
REM Project ID
set PID=e5cb29b26e09465eb75cc6279bd1193b
REM Version ID
set VID=e0dbc224baac4c9180e6c5a340fcf4a3

echo Running ConverterCore.py...
python .\ConverterCore.py --w %WID% --p %PID% --v %VID%

echo.
echo Conversion process completed.
pause
