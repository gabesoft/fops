@ECHO OFF

IF %1.==. GOTO USAGE

CLS
SET checkin_comment="%1"
SET build_target="Default"
IF NOT %2.==. SET build_target="%2"

CALL "Dependencies\Fake.1.42.23.0\Fake.exe" "build.fsx"
GOTO QUIT

:USAGE
ECHO "Usage: build [checkin comment] <build target>"
ECHO.

:QUIT
@ECHO ON