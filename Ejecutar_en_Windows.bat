@echo off
cd /d "%~dp0"
dotnet run --project Aplicacion\Aplicacion.csproj -c Release
if errorlevel 1 pause
