@echo off
cd /d "%~dp0"
dotnet run --project Pruebas\Pruebas.csproj -c Release
pause
