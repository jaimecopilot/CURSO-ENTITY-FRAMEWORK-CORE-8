param()
Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'
$RepoRoot = (Resolve-Path (Join-Path $PSScriptRoot '..\..')).Path
$practicePath = Join-Path $RepoRoot 'M04\PRACTICA\M04_PRACTICA_CANONICA.md'
if (-not (Test-Path $practicePath)) { throw 'M04: falta M04_PRACTICA_CANONICA.md.' }
$practice = (Get-Content $practicePath -Raw).Replace([char]13,'')
$expectedBlocks = @{
  '4.1'=12; '4.2'=11; '4.3'=12; '4.4'=11; '4.5'=12; '4.6'=12;
  '4.7'=12; '4.8'=10; '4.9'=14; '4.10'=12; '4.11'=12; '4.12'=12
}
$expectedMain = @{
  '4.1'=10; '4.2'=10; '4.3'=10; '4.4'=10; '4.5'=10; '4.6'=10;
  '4.7'=10; '4.8'=10; '4.9'=11; '4.10'=10; '4.11'=11; '4.12'=10
}
$total = 0
for ($n=1; $n -le 12; $n++) {
  $point = "4.$n"
  $pattern = '(?ms)^## Punto ' + [regex]::Escape($point) + '\b.*?(?=^## Punto 4\.\d+\b|\z)'
  $section = [regex]::Match($practice,$pattern).Value
  if ([string]::IsNullOrWhiteSpace($section)) { throw "M04: falta $point en la práctica canónica." }
  $errorsIndex = $section.IndexOf('### Errores comunes del ejercicio')
  if ($errorsIndex -lt 0) { throw "M04: $point no contiene errores comunes." }
  $main = $section.Substring(0,$errorsIndex)
  $steps = [regex]::Matches($main,'(?m)^### Paso \d+:')
  if ($steps.Count -ne $expectedMain[$point]) { throw "M04: $point pasos principales=$($steps.Count), esperado=$($expectedMain[$point])." }
  if ($section -notmatch '(?m)^### Reto resuelto:') { throw "M04: $point no contiene reto resuelto." }
  if ($section -notmatch '(?m)^### Resultado esperado\s*$') { throw "M04: $point no contiene resultado esperado." }
  $matches = [regex]::Matches($section,'(?ms)^```csharp\s*\n(.*?)^```\s*$')
  if ($matches.Count -ne $expectedBlocks[$point]) { throw "M04: $point bloques C#=$($matches.Count), esperado=$($expectedBlocks[$point])." }
  $storePath = Join-Path $RepoRoot ("M04\PROYECTO\" + $point + "\CanonicalPdfBlocks.cs")
  if (-not (Test-Path $storePath)) { throw "M04: falta $storePath." }
  $store = (Get-Content $storePath -Raw).Replace([char]13,'')
  for ($i=0; $i -lt $matches.Count; $i++) {
    $num = '{0:D2}' -f ($i+1)
    $marker = "CANONICAL PDF M04 $point - BLOCK $num"
    if (-not $store.Contains($marker)) { throw "M04: falta marcador $marker." }
    $code = $matches[$i].Groups[1].Value.TrimEnd([char]10)
    $commentedLines = [System.Collections.Generic.List[string]]::new()
    foreach ($line in $code.Split([char]10)) {
      if ($line -eq '') { $commentedLines.Add('//') } else { $commentedLines.Add('// ' + $line) }
    }
    $commented = [string]::Join([char]10,$commentedLines)
    if (-not $store.Contains($commented)) { throw "M04: bloque $point/$num no está literal y comentado." }
  }
  $total += $matches.Count
  Write-Host "PASS canónico $point · $($steps.Count) pasos principales · $($matches.Count) bloques C#"
}
if ($total -ne 142) { throw "M04: total de bloques C#=$total, esperado=142." }
Write-Host 'PASS M04 CANÓNICO · 12 puntos · 142 bloques C# preservados literalmente y comentados.'
