param()
Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'
$RepoRoot = (Resolve-Path (Join-Path $PSScriptRoot '..\..')).Path
$practice = Join-Path $RepoRoot 'M05\PRACTICA\M05_PRACTICA.md'
$snapshot = Join-Path $RepoRoot 'M05\PRACTICA\M05_PRACTICA_CANONICA.md'
if (-not (Test-Path $practice) -or -not (Test-Path $snapshot)) { throw 'M05: falta práctica o snapshot canónico.' }
$sha = (& git hash-object $snapshot).Trim()
if ($sha -ne '5c38f46a7035e7a1249775293060010bb240796b') { throw "M05: snapshot canónico con SHA inesperado: $sha" }
$practiceText = (Get-Content $practice -Raw) -replace "`r",""
$snapshotText = (Get-Content $snapshot -Raw) -replace "`r",""
if ($practiceText -ne $snapshotText) { throw 'M05: M05_PRACTICA.md y el snapshot canónico divergen.' }
$expectedFences = @{
  '5.1'=5;
  '5.2'=6;
  '5.3'=6;
  '5.4'=6;
  '5.5'=5;
  '5.6'=6;
  '5.7'=16;
  '5.8'=27;
  '5.9'=25;
  '5.10'=20;
  '5.11'=19;
  '5.12'=17;
}
$expectedCSharp = @{
  '5.1'=2;
  '5.2'=3;
  '5.3'=3;
  '5.4'=2;
  '5.5'=2;
  '5.6'=1;
  '5.7'=0;
  '5.8'=3;
  '5.9'=19;
  '5.10'=14;
  '5.11'=11;
  '5.12'=14;
}
$totalFences = 0; $totalCSharp = 0
for ($n=1; $n -le 12; $n++) {
  $point = "5.$n"
  $pattern = '(?ms)^## Punto ' + [regex]::Escape($point) + '\b.*?(?=^## Punto 5\.\d+\b|\z)'
  $section = [regex]::Match($practiceText,$pattern).Value
  if ([string]::IsNullOrWhiteSpace($section)) { throw "M05: falta $point." }
  if ($section -notmatch '(?m)^### Reto') { throw "M05: $point no contiene reto." }
  if ($section -notmatch '(?m)^### Resultado esperado\s*$') { throw "M05: $point no contiene resultado esperado." }
  $fences = [regex]::Matches($section,'(?ms)^```[^\r\n]*\n.*?^```\s*$').Count
  $csharp = [regex]::Matches($section,'(?ms)^```csharp\s*\n(.*?)^```\s*$')
  if ($fences -ne $expectedFences[$point]) { throw "M05: $point fences=$fences esperado=$($expectedFences[$point])." }
  if ($csharp.Count -ne $expectedCSharp[$point]) { throw "M05: $point C#=$($csharp.Count) esperado=$($expectedCSharp[$point])." }
  $storePath = Join-Path $RepoRoot ("M05\PROYECTO\"+$point+"\CanonicalPdfBlocks.cs")
  $store = (Get-Content $storePath -Raw) -replace "`r",""
  for ($i=0; $i -lt $csharp.Count; $i++) {
    $num = '{0:D2}' -f ($i+1)
    $marker = "CANONICAL PDF M05 $point - BLOCK $num"
    if (-not $store.Contains($marker)) { throw "M05: falta $marker." }
    $code = $csharp[$i].Groups[1].Value.TrimEnd([char]10)
    $commentedLines = [System.Collections.Generic.List[string]]::new()
    foreach ($line in $code.Split([char]10)) { if ($line -eq '') { $commentedLines.Add('//') } else { $commentedLines.Add('// ' + $line) } }
    $commented = [string]::Join([char]10,$commentedLines)
    if (-not $store.Contains($commented)) { throw "M05: bloque literal $point/$num no preservado." }
  }
  $totalFences += $fences; $totalCSharp += $csharp.Count
  Write-Host "PASS canónico $point · fences=$fences · C#=$($csharp.Count)"
}
Write-Host "PASS M05 CANÓNICO · fences=$totalFences · C#=$totalCSharp · SHA=$sha"
