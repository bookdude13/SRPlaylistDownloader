
set BUILT_VERSION="1.0.0"
set MOD_NAME="SRPlaylistDownloader"

set RELEASE_BUILD_DIR=".\%MOD_NAME%\%MOD_NAME%\bin\Release"
set MAIN_DLL="%RELEASE_BUILD_DIR%\%MOD_NAME%.dll"

set OUTPUT_DIR=".\build\%MOD_NAME%_v%BUILT_VERSION%"
mkdir %OUTPUT_DIR%

copy %MAIN_DLL% %OUTPUT_DIR%
