try
{
$NetWindowsVer = "net7.0-windows10.0.22621.0"
$NewBinLocation = "..\..\..\CHSongCompressorPublish\win10-x64"

dotnet publish CloneHeroSongsCompressor.csproj -c Release --os win10 -a x64 --self-contained -f $NetWindowsVer -p:WindowsPackageType=None -p:WindowsAppSDKSelfContained=true -p:PublishReadyToRun=false -p:DebugType=embedded

Remove-Item $NewBinLocation -Recurse
Copy-Item -Path "bin\Release\$NetWindowsVer\win10-x64\publish" -Destination "..\..\..\CHSongCompressorPublish\win10-x64" -Recurse

Get-ChildItem -Path $NewBinLocation *.mui -Recurse | foreach { Remove-Item -Path $_.FullName }

$folders = Get-ChildItem -Path $NewBinLocation -Directory -Recurse |
Where-Object { 
    (Get-ChildItem -Path $_.FullName -File -Recurse).Count -eq 0
}
$folders | Remove-Item -Force -Recurse

}
catch
{
    Write-Error $_.Exception.ToString()
    Read-Host -Prompt "The above error occurred. Press Enter to exit."
}

Read-Host "Press any key to continue..."