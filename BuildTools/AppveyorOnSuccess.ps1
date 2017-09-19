# build should be successful if we get this far
# create a text file that contains the version number
write-host "Creating version file for https://medlaunch.info website"



if ($env:APPVEYOR_REPO_BRANCH -eq "dev")
{
	$verFile = "LatestDevVersion.txt"    
}
if ($env:APPVEYOR_REPO_BRANCH -eq "master")
{
	$verFile = "LatestMasterVersion.txt" 
}

$verNo = $env:APPVEYOR_BUILD_NUMBER
New-Item $verFile -ItemType file

$verNo | Set-Content $verFile

# upload to medlaunch downloads ftp
$ftp = $env:FTP_ADDRESS_ONE + $verFile
$webclient = New-Object System.Net.WebClient
$uri = New-Object System.Uri($ftp)
write-host "Uploading $verFile to medlaunch downloads site"
$webclient.UploadFile($uri, $verFile)

$ftp = $env:FTP_ADDRESS_TWO + $verFile
$webclient = New-Object System.Net.WebClient
$uri = New-Object System.Uri($ftp)
write-host "Uploading $verFile to medlaunch website"
$webclient.UploadFile($uri, $verFile)