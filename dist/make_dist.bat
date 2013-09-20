
set OUTDIR=UserVoice_1_5_0
set INDIR1=..\UserVoice\bin\Release
set INDIR2=..\ankoUserVoice\bin\Release

rm -rf %OUTDIR%

mkdir %OUTDIR%

copy readme.txt %OUTDIR%
copy %INDIR1%\UserVoice.dll %OUTDIR%
copy %INDIR1%\WPFToolkit.dll %OUTDIR%
copy %INDIR2%\ankoUserVoice.dll %OUTDIR%

mkdir %OUTDIR%\Data
xcopy %INDIR1%\Data %OUTDIR%\Data /E /Y

zip -r %OUTDIR%.zip %OUTDIR%

pause
