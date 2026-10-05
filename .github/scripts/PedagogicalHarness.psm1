Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'

function Assert-TextContains {
    param(
        [Parameter(Mandatory=$true)][string]$Text,
        [Parameter(Mandatory=$true)][string[]]$Tokens,
        [Parameter(Mandatory=$true)][string]$Context
    )
    foreach ($token in $Tokens) {
        if (-not $Text.Contains($token)) {
            throw "${Context}: falta '$token'."
        }
    }
}

function Invoke-Checked {
    param(
        [Parameter(Mandatory=$true)][string]$WorkingDirectory,
        [Parameter(Mandatory=$true)][string]$FilePath,
        [Parameter(Mandatory=$true)][string[]]$ArgumentList,
        [Parameter(Mandatory=$true)][string]$Context
    )
    Push-Location $WorkingDirectory
    try {
        $output = & $FilePath @ArgumentList 2>&1
        $code = $LASTEXITCODE
        $text = $output | Out-String
        if ($code -ne 0) {
            throw ($Context + " falló con exit code " + $code + "." + [Environment]::NewLine + $text)
        }
        return $text
    }
    finally {
        Pop-Location
    }
}

function New-PedagogicalCopy {
    param(
        [Parameter(Mandatory=$true)][string]$Source,
        [Parameter(Mandatory=$true)][string]$Name
    )
    $target = Join-Path $env:RUNNER_TEMP $Name
    if (Test-Path $target) {
        Remove-Item $target -Recurse -Force
    }
    Copy-Item $Source $target -Recurse
    return $target
}

function Remove-ActiveRegionFromText {
    param(
        [Parameter(Mandatory=$true)][string]$Text,
        [Parameter(Mandatory=$true)][string]$Region
    )
    $pattern = '(?ms)^\s*//\s*' + [regex]::Escape($Region) + '\s+INICIO\s*\r?\n.*?^\s*//\s*' + [regex]::Escape($Region) + '\s+FIN\s*\r?\n?'
    if ($Text -notmatch $pattern) {
        throw "No se localiza la región activa '$Region'."
    }
    return [regex]::Replace($Text, $pattern, '', 1)
}

function Enable-BlockFragment {
    param(
        [Parameter(Mandatory=$true)][string]$Path,
        [Parameter(Mandatory=$true)][string[]]$Markers,
        [string[]]$ActiveRegions = @()
    )
    $text = Get-Content $Path -Raw

    foreach ($region in $ActiveRegions) {
        $text = Remove-ActiveRegionFromText -Text $text -Region $region
    }

    foreach ($marker in $Markers) {
        $pattern = '(?ms)(//\s*' + [regex]::Escape($marker) + '.*?^\s*)/\*\s*\r?\n(.*?)^\s*\*/'
        $match = [regex]::Match($text, $pattern)
        if (-not $match.Success) {
            throw "No se localiza el bloque activable '$marker' en $Path."
        }
        if ([string]::IsNullOrWhiteSpace($match.Groups[2].Value)) {
            throw "El bloque activable '$marker' está vacío."
        }
        $replacement = $match.Groups[1].Value + $match.Groups[2].Value
        $text = $text.Remove($match.Index, $match.Length).Insert($match.Index, $replacement)
    }

    Set-Content $Path -Value $text -Encoding utf8
}

function Enable-RetoBlock {
    param(
        [Parameter(Mandatory=$true)][string]$Path,
        [Parameter(Mandatory=$true)][string]$Marker
    )
    $text = Get-Content $Path -Raw
    $pattern = '(?ms)/\*\s*//\s*' + [regex]::Escape($Marker) + '\s*(.*?)\s*\*/'
    $match = [regex]::Match($text, $pattern)
    if (-not $match.Success) {
        throw "No se localiza el reto activable '$Marker' en $Path."
    }
    $text = $text.Remove($match.Index, $match.Length).Insert($match.Index, $match.Groups[1].Value)
    Set-Content $Path -Value $text -Encoding utf8
}

function Enable-LineCommentWholeFileCopy {
    param(
        [Parameter(Mandatory=$true)][string]$Path,
        [Parameter(Mandatory=$true)][string]$Marker
    )
    $lines = Get-Content $Path
    $markerIndex = -1
    for ($i = 0; $i -lt $lines.Count; $i++) {
        if ($lines[$i].Contains($Marker)) {
            $markerIndex = $i
            break
        }
    }
    if ($markerIndex -lt 0) {
        throw "No se localiza '$Marker' en $Path."
    }

    $separatorIndex = -1
    for ($i = $markerIndex; $i -lt $lines.Count; $i++) {
        if ($lines[$i] -match '^// -{20,}$') {
            $separatorIndex = $i
            break
        }
    }
    if ($separatorIndex -lt 0) {
        throw "No se localiza el inicio de la copia comentada '$Marker'."
    }

    $endIndex = -1
    for ($i = $separatorIndex + 1; $i -lt $lines.Count; $i++) {
        if ($lines[$i] -match '^// ={20,}$') {
            $endIndex = $i
            break
        }
    }
    if ($endIndex -lt 0) {
        throw "No se localiza el fin de la copia comentada '$Marker'."
    }

    $activated = [System.Collections.Generic.List[string]]::new()
    for ($i = $separatorIndex + 1; $i -lt $endIndex; $i++) {
        $line = $lines[$i]
        if ($line -eq '//') {
            $activated.Add('')
        }
        elseif ($line.StartsWith('// ')) {
            $activated.Add($line.Substring(3))
        }
        elseif ($line.StartsWith('//')) {
            $activated.Add($line.Substring(2))
        }
        else {
            throw "Línea no comentada dentro de '$Marker': $line"
        }
    }

    if ($activated.Count -eq 0) {
        throw "La copia comentada '$Marker' está vacía."
    }

    $activated[0] = $activated[0].TrimStart([char]0xFEFF)
    Set-Content $Path -Value $activated -Encoding utf8
}

function Enable-SnapshotFragmentByExactReplacement {
    param(
        [Parameter(Mandatory=$true)][string]$Path,
        [Parameter(Mandatory=$true)][string]$Marker
    )
    $text = Get-Content $Path -Raw
    $pattern = '(?ms)//\s*' + [regex]::Escape($Marker) + '.*?^\s*/\*\s*\r?\n(.*?)^\s*\*/'
    $match = [regex]::Match($text, $pattern)
    if (-not $match.Success) {
        throw "No se localiza el fragmento de snapshot '$Marker'."
    }

    $fragment = $match.Groups[1].Value
    $commentEnd = $match.Index + $match.Length
    $activeIndex = $text.IndexOf($fragment, $commentEnd)
    if ($activeIndex -lt 0) {
        throw "No se localiza el equivalente activo de '$Marker'."
    }

    $text = $text.Remove($activeIndex, $fragment.Length)
    $match = [regex]::Match($text, $pattern)
    $text = $text.Remove($match.Index, $match.Length).Insert($match.Index, $fragment)
    Set-Content $Path -Value $text -Encoding utf8
}

Export-ModuleMember -Function Assert-TextContains,Invoke-Checked,New-PedagogicalCopy,Enable-BlockFragment,Enable-RetoBlock,Enable-LineCommentWholeFileCopy,Enable-SnapshotFragmentByExactReplacement
