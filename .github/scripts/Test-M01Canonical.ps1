param()
Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'

$RepoRoot = (Resolve-Path (Join-Path $PSScriptRoot '..\..')).Path
$practicePath = Join-Path $RepoRoot 'M01\PRACTICA\M01_PRACTICA_CANONICA.md'
if (-not (Test-Path $practicePath)) { throw 'M01: falta M01_PRACTICA_CANONICA.md.' }

$practice = (Get-Content $practicePath -Raw) -replace "`r",""

$expectedSteps = @{
  '1.1'=16; '1.2'=13; '1.3'=17; '1.4'=13; '1.5'=15; '1.6'=12;
  '1.7'=9; '1.8'=10; '1.9'=10; '1.10'=16; '1.11'=15; '1.12'=12
}
$expectedBlocks = @{
  '1.1'=2; '1.2'=7; '1.3'=6; '1.4'=8; '1.5'=7; '1.6'=6;
  '1.7'=4; '1.8'=5; '1.9'=5; '1.10'=3; '1.11'=3; '1.12'=7
}

$totalBlocks = 0
$totalSteps = 0

for ($n=1; $n -le 12; $n++) {
  $point = "1.$n"
  $pattern = '(?ms)^## Punto ' + [regex]::Escape($point) + '\b.*?(?=^## Punto 1\.\d+\b|\z)'
  $section = [regex]::Match($practice,$pattern).Value

  if ([string]::IsNullOrWhiteSpace($section)) {
    throw "M01: falta $point en la práctica canónica."
  }

  $steps = [regex]::Matches($section,'(?m)^### Paso \d+:')
  if ($steps.Count -ne $expectedSteps[$point]) {
    throw "M01: $point pasos=$($steps.Count), esperado=$($expectedSteps[$point])."
  }

  if ($section -notmatch '(?m)^### Reto(?: resuelto| de lectura):') {
    throw "M01: $point no contiene reto."
  }

  if ($section -notmatch '(?m)^### Resultado esperado\s*$') {
    throw "M01: $point no contiene cierre Resultado esperado."
  }

  $matches = [regex]::Matches($section,'(?ms)^```csharp\s*\n(.*?)^```\s*$')
  if ($matches.Count -ne $expectedBlocks[$point]) {
    throw "M01: $point bloques C#=$($matches.Count), esperado=$($expectedBlocks[$point])."
  }

  $storePath = Join-Path $RepoRoot ("M01\PROYECTO\" + $point + "\CanonicalPdfBlocks.cs")
  if (-not (Test-Path $storePath)) {
    throw "M01: falta $storePath."
  }
  $store = (Get-Content $storePath -Raw) -replace "`r",""

  for ($i=0; $i -lt $matches.Count; $i++) {
    $num = '{0:D2}' -f ($i+1)
    $marker = "CANONICAL PDF M01 $point - BLOCK $num"
    if (-not $store.Contains($marker)) {
      throw "M01: falta marcador $marker."
    }

    $code = $matches[$i].Groups[1].Value.TrimEnd([char]10)
    $commentedLines = [System.Collections.Generic.List[string]]::new()
    foreach ($line in $code.Split([char]10)) {
      if ($line -eq '') { $commentedLines.Add('//') }
      else { $commentedLines.Add('// ' + $line) }
    }
    $commented = [string]::Join([char]10,$commentedLines)

    if (-not $store.Contains($commented)) {
      throw "M01: bloque $point/$num no está preservado literal y comentado."
    }
  }

  $totalBlocks += $matches.Count
  $totalSteps += $steps.Count
  Write-Host "PASS canónico $point · $($steps.Count) pasos · $($matches.Count) bloques C#"
}

if ($totalBlocks -ne 63) { throw "M01: total bloques C#=$totalBlocks, esperado=63." }
if ($totalSteps -ne 158) { throw "M01: total pasos=$totalSteps, esperado=158." }

# Continuidad estructural del dominio.
for ($n=2; $n -le 12; $n++) {
  $programPath = Join-Path $RepoRoot ("M01\PROYECTO\1.$n\Program.cs")
  $program = Get-Content $programPath -Raw

  if ($n -ge 2 -and $program -notmatch 'class\s+OrdenFabricacion') {
    throw "M01 1.${n}: falta OrdenFabricacion."
  }
  if ($n -ge 2 -and $program -notmatch 'class\s+PlanchaAcero') {
    throw "M01 1.${n}: falta PlanchaAcero; la evolución acumulativa se ha roto."
  }
  if ($n -ge 3 -and $program -notmatch 'class\s+Aleacion') {
    throw "M01 1.${n}: falta Aleacion."
  }
  if ($n -ge 4 -and $program -notmatch 'class\s+EstadoOrden') {
    throw "M01 1.${n}: falta EstadoOrden."
  }
  if ($n -ge 2 -and $program -notmatch 'List<PlanchaAcero>\s+Planchas') {
    throw "M01 1.${n}: falta navegación OrdenFabricacion.Planchas."
  }
}

# EnsureCreated sólo pertenece a la fase anterior a migraciones.
$p12 = Get-Content (Join-Path $RepoRoot 'M01\PROYECTO\1.2\Program.cs') -Raw
if ($p12 -notmatch 'EnsureCreated\(') { throw 'M01 1.2: falta EnsureCreated().' }
if ($p12 -match 'Database\.Migrate\(') { throw 'M01 1.2: no debe usar Migrate todavía.' }

for ($n=3; $n -le 12; $n++) {
  $program = Get-Content (Join-Path $RepoRoot ("M01\PROYECTO\1.$n\Program.cs")) -Raw
  if ($program -match 'EnsureCreated\(') {
    throw "M01 1.${n}: no se debe mezclar EnsureCreated con migraciones."
  }
  if ($program -notmatch 'Database\.Migrate\(\)') {
    throw "M01 1.${n}: falta Database.Migrate()."
  }
}

# Base oficial única: AceriaDB.
$projectText = [string]::Join("`n",(
  Get-ChildItem (Join-Path $RepoRoot 'M01\PROYECTO') -Recurse -File |
  Where-Object { $_.Extension -in @('.cs','.json','.md','.csproj') } |
  ForEach-Object { Get-Content $_.FullName -Raw }
))
if ($projectText -match 'AceriaDB_CP\d+') {
  throw 'M01: quedan nombres de base checkpoint AceriaDB_CPxx no presentes en el contrato.'
}

# La historia canónica de migraciones debe mantenerse.
foreach ($point in @('1.10','1.11','1.12')) {
  $root = Join-Path $RepoRoot ("M01\PROYECTO\" + $point)
  foreach ($name in @(
    '20260927000100_InitialCreate.cs',
    '20260927000200_AddAleacion.cs',
    '20260927000300_AddEstadoOrden.cs',
    'AceriaDbContextModelSnapshot.cs'
  )) {
    if (-not (Test-Path (Join-Path $root ("Migrations\" + $name)))) {
      throw "M01 ${point}: falta migración $name."
    }
  }
}

# 1.11: alternativas sólo de lectura, nunca operativas.
$p111 = Get-Content (Join-Path $RepoRoot 'M01\PROYECTO\1.11\Program.cs') -Raw
$proj111 = Get-Content (Join-Path $RepoRoot 'M01\PROYECTO\1.11\AceriaData.Console.csproj') -Raw
if ($p111 -match 'UseSqlite|UseNpgsql|UseInMemoryDatabase|Microsoft\.Data\.Sqlite') {
  throw 'M01 1.11: existe proveedor alternativo ejecutable.'
}
if ($proj111 -match 'Sqlite|Npgsql|InMemory') {
  throw 'M01 1.11: existe paquete de proveedor alternativo.'
}
if ($p111 -notmatch 'UseSqlServer') { throw 'M01 1.11: falta UseSqlServer.' }

# Conceptos activos mínimos que no pueden desaparecer.
$checks = @{
  '1.6'=@('AsNoTracking','Find(','InsertarPlancha');
  '1.7'=@('ChangeTracker','DetectChanges','Clear()','ReportarCambios');
  '1.8'=@('Attach(','Update(','IsModified','ActualizarSoloCliente');
  '1.9'=@('SaveChangesAsync','DbUpdateException','InsertarOrdenConPlanchas');
  '1.10'=@('Database.Migrate()','EnableRetryOnFailure','CommandTimeout');
  '1.11'=@('ToQueryString','UseSqlServer');
  '1.12'=@('AddDbContext','AddScoped<IOrdenRepositorio','AddScoped<IServicioOrdenes','CreateScope')
}
foreach ($point in $checks.Keys) {
  $program = Get-Content (Join-Path $RepoRoot ("M01\PROYECTO\" + $point + "\Program.cs")) -Raw
  foreach ($token in $checks[$point]) {
    if (-not $program.Contains($token)) {
      throw "M01 ${point}: falta concepto activo '$token'."
    }
  }
}

Write-Host 'PASS M01 CANÓNICO · 158 pasos · 63 bloques C# literales/comentados · continuidad técnica validada.'
