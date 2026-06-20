# framework-dependent
dotnet publish AutoCEMI.CLI/AutoCEMI.CLI.csproj -c Release -o ./publish/CLI
dotnet publish AutoCEMI.GUI/AutoCEMI.GUI.csproj -c Release -o ./publish/GUI

# self-contained
dotnet publish AutoCEMI.CLI/AutoCEMI.CLI.csproj -c Release -o ./publish/CLI-sc -p:PublishSingleFile=true -p:PublishReadyToRun=true -p:PublishTrimmed=true
dotnet publish AutoCEMI.GUI/AutoCEMI.GUI.csproj -c Release -o ./publish/GUI-sc --self-contained true -r win-x64 -p:PublishReadyToRun=true