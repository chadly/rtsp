function GetVersion {
	param([string] $projectName)

	$xml = [Xml] (Get-Content src\$projectName\$projectName.csproj)
	$version = [String] $xml.Project.PropertyGroup.Version
	$version = $version.Trim()
	return $version
}

function Publish {
	param([string] $projectName, [string] $version)

	dotnet publish src\$projectName -c Release -r win10-x64
	dotnet publish src\$projectName -c Release -r ubuntu-x64
}

$version = GetVersion Streaming
Publish Streaming $version

Compress-Archive -Force -Path bin\Release\netcoreapp2.2\win10-x64\publish\* -DestinationPath streaming-v${version}-win10-x64.zip
Compress-Archive -Force -Path bin\Release\netcoreapp2.2\ubuntu-x64\publish\* -DestinationPath streaming-v${version}-ubuntu-x64.zip
