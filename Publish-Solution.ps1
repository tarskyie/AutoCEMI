# framework-dependent
dotnet publish AutoCEMI.CLI\AutoCEMI.CLI.csproj -c Release -o .\publish\cli-win-x64-framework-dependent -r win-x64 --self-contained false
dotnet publish AutoCEMI.GUI\AutoCEMI.GUI.csproj -c Release -o .\publish\gui-win-x64-framework-dependent -r win-x64 --self-contained false
Copy-Item -Path ".\publish\cli-win-x64-framework-dependent\*" -Destination ".\publish\gui-win-x64-framework-dependent"
$languages = @('cs', 'de', 'es', 'fr', 'it', 'ja', 'ko', 'pl', 'pt-BR', 'ru', 'tr', 'zh-Hans', 'zh-Hant')
foreach ($lang in $languages){
    Remove-Item -Path ".\publish\gui-win-x64-framework-dependent\$lang" -Force -Recurse
    Remove-Item -Path ".\publish\cli-win-x64-framework-dependent\$lang" -Force -Recurse
}

# self-contained
dotnet publish AutoCEMI.CLI\AutoCEMI.CLI.csproj -c Release -o .\publish\cli-win-x64-self-contained -p:PublishSingleFile=true -p:PublishReadyToRun=true -p:PublishTrimmed=true
dotnet publish AutoCEMI.GUI\AutoCEMI.GUI.csproj -c Release -o .\publish\gui-win-x64-self-contained --self-contained true -r win-x64 -p:PublishReadyToRun=true