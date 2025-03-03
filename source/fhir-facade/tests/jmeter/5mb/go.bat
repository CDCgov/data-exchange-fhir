@echo off
setlocal enabledelayedexpansion

:: Set up the accent-to-normal replacement mappings
set "replaceChars=á=a é=e í=i ó=o ú=u ñ=n"

:: Loop through each file in the current directory
for %%f in (*.*) do (
    set "filename=%%f"
    set "newname=%%f"
    
    :: Replace accented characters with normal characters
    for %%r in (%replaceChars%) do (
        set "charToReplace=%%r"
        set "search=!charToReplace:~0,1!"
        set "replace=!charToReplace:~2!"
        
        :: Replace the characters in the filename
        set "newname=!newname:%search%=%replace%!"
    )

    :: If the filename has changed, rename the file
    if not "!filename!"=="!newname!" (
        echo Renaming "!filename!" to "!newname!"
        ren "!filename!" "!newname!"
    )
)

endlocal
