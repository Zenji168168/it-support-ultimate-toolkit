Add-Type -AssemblyName System.Drawing

$srcPath = "C:\Users\Monkey D. Zenji\.gemini\antigravity\brain\62b8e6c4-94e8-4d93-8b76-05ccce342ce5\.user_uploaded\media_1789389887719.png"
$targetIco = "d:\MY IT AGENT TOOl\app.ico"
$targetPng = "d:\MY IT AGENT TOOl\app-icon.png"

# Copy original PNG directly as web icon / asset
Copy-Item -Path $srcPath -Destination $targetPng -Force

$srcBmp = [System.Drawing.Bitmap]::FromFile($srcPath)

$sizes = @(16, 32, 48, 64, 128, 256)
$pngStreams = @()

foreach ($sz in $sizes) {
    $bmp = New-Object System.Drawing.Bitmap($sz, $sz)
    $g = [System.Drawing.Graphics]::FromImage($bmp)
    $g.InterpolationMode = [System.Drawing.Drawing2D.InterpolationMode]::HighQualityBicubic
    $g.SmoothingMode = [System.Drawing.Drawing2D.SmoothingMode]::HighQuality
    $g.PixelOffsetMode = [System.Drawing.Drawing2D.PixelOffsetMode]::HighQuality
    $g.Clear([System.Drawing.Color]::Transparent)
    $g.DrawImage($srcBmp, 0, 0, $sz, $sz)
    $g.Dispose()
    
    $ms = New-Object System.IO.MemoryStream
    $bmp.Save($ms, [System.Drawing.Imaging.ImageFormat]::Png)
    $bmp.Dispose()
    $pngStreams += $ms
}
$srcBmp.Dispose()

# Write ICO file with multi-resolution PNG frames
$icoFs = [System.IO.File]::Create($targetIco)
$bw = New-Object System.IO.BinaryWriter($icoFs)

# Header: reserved(0), type(1=icon), count
$bw.Write([uint16]0)
$bw.Write([uint16]1)
$bw.Write([uint16]$sizes.Count)

$offset = 6 + ($sizes.Count * 16)

for ($i = 0; $i -lt $sizes.Count; $i++) {
    $sz = $sizes[$i]
    $w = if ($sz -ge 256) { 0 } else { [byte]$sz }
    $h = if ($sz -ge 256) { 0 } else { [byte]$sz }
    $len = [uint32]$pngStreams[$i].Length

    $bw.Write([byte]$w)
    $bw.Write([byte]$h)
    $bw.Write([byte]0)   # Color palette count
    $bw.Write([byte]0)   # Reserved
    $bw.Write([uint16]1)  # Color planes
    $bw.Write([uint16]32) # Bits per pixel
    $bw.Write([uint32]$len)
    $bw.Write([uint32]$offset)
    $offset += $len
}

for ($i = 0; $i -lt $sizes.Count; $i++) {
    $bytes = $pngStreams[$i].ToArray()
    $bw.Write($bytes, 0, $bytes.Length)
    $pngStreams[$i].Dispose()
}

$bw.Close()
$icoFs.Close()

Write-Output "Successfully generated app.ico and app-icon.png!"
Get-Item $targetIco, $targetPng | Select-Object Name, Length
