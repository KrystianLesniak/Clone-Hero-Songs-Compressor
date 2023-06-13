# TODO: Refactor it to use variables

# Publish an application
if (Test-Path "publish") {
    Remove-Item "publish" -Recurse -Force
}

dotnet publish "SongsCompressor.WPF/SongsCompressor.WPF.csproj" -c Release -r win-x64 --self-contained false --property:PublishDir="../publish/CH Songs Compressor"

# Compress an application into zip
if (Test-Path "application.zip") {
    Remove-Item "application.zip"
}

Compress-Archive -Path "publish/*" -DestinationPath "application.zip"