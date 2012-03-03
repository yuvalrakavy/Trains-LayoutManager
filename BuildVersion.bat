if not exist LayoutManagerFiles mkdir LayoutManagerFiles
del /q LayoutManagerFiles

if not exist LayoutManagerDistribution mkdir LayoutManagerDistribution

copy LayoutManager\bin\debug\LayoutManager.exe LayoutManagerFiles
copy LayoutManager\bin\Debug\*.dll LayoutManagerFiles
copy LayoutManager\bin\debug\*.pdb LayoutManagerFiles
copy Intellibox\bin\debug\Intellibox.* LayoutManagerFiles
copy MarklinDigital\bin\debug\MarklinDigital.* LayoutManagerFiles
copy LayoutEventDebugger\bin\debug\LayoutEventDebugger.* LayoutManagerFiles
copy LayoutEmulation\bin\debug\LayoutEmulation.* LayoutManagerFiles
copy Wizard\bin\debug\LayoutEmulation.* LayoutManagerFiles

cd LayoutManagerFiles
c:\bin\zip\zip ..\LayoutManagerDistribution\%1 *.*
cd ..
