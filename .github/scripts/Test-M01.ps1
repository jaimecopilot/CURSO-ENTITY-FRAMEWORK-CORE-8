param(
  [ValidateSet('all','inventory','1.1','1.2','1.3','1.4','1.5','1.6','1.7','1.8','1.9','1.10','1.11','1.12')]
  [string]$Suite = 'all'
)

Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'

if ([string]::IsNullOrWhiteSpace($env:RUNNER_TEMP)) {
  $env:RUNNER_TEMP = [System.IO.Path]::GetTempPath()
}

$RepoRoot = (Resolve-Path (Join-Path $PSScriptRoot '..\..')).Path
Import-Module (Join-Path $PSScriptRoot 'PedagogicalHarness.psm1') -Force

function Write-Section([string]$Name) {
  Write-Host ''
  Write-Host '============================================================'
  Write-Host $Name
  Write-Host '============================================================'
}

function Get-PointRoot([string]$Point) {
  return Join-Path $RepoRoot ("M01\PROYECTO\" + $Point)
}

function Build-And-Run([string]$Point) {
  $root = Get-PointRoot $Point
  Invoke-Checked -WorkingDirectory $root -FilePath 'dotnet' -ArgumentList @('restore','AceriaData.sln') -Context "$Point restore" | Out-Null
  Invoke-Checked -WorkingDirectory $root -FilePath 'dotnet' -ArgumentList @('build','AceriaData.sln','--configuration','Release','--no-restore') -Context "$Point build" | Out-Null
  return Invoke-Checked -WorkingDirectory $root -FilePath 'dotnet' -ArgumentList @('run','--project','AceriaData.Console.csproj','--configuration','Release','--no-build') -Context "$Point run"
}

function Get-Migrations([string]$Point) {
  $root = Get-PointRoot $Point
  return Invoke-Checked -WorkingDirectory $root -FilePath 'dotnet' -ArgumentList @(
    'ef','migrations','list',
    '--project','AceriaData.Console.csproj',
    '--startup-project','AceriaData.Console.csproj',
    '--configuration','Release'
  ) -Context "$Point migrations list"
}

function Test-Inventory {
  Write-Section 'M01 · inventario técnico'
  for ($n=1; $n -le 12; $n++) {
    $point = "1.$n"
    $root = Get-PointRoot $point
    foreach ($required in @('AceriaData.sln','AceriaData.Console.csproj','Program.cs','CanonicalPdfBlocks.cs')) {
      if (-not (Test-Path (Join-Path $root $required))) {
        throw "M01 ${point}: falta $required."
      }
    }
    Write-Host "PASS inventario $point"
  }
}

function Test-11 {
  Write-Section 'M01 1.1 · arranque del proyecto'
  $out = Build-And-Run '1.1'
  Assert-TextContains -Text $out -Tokens @(
    '=== ACERÍA DEL NORTE ===',
    'Sistema de gestión de órdenes de fabricación',
    'Proyecto: AceriaData',
    'Framework:',
    'Fecha:'
  ) -Context '1.1'
  Write-Host 'PASS 1.1 COMPLETO'
}

function Test-12 {
  Write-Section 'M01 1.2 · DbContext + relación Orden/Plancha + EnsureCreated'
  $out = Build-And-Run '1.2'
  Assert-TextContains -Text $out -Tokens @(
    'Orden: OF-001',
    'Plancha Id'
  ) -Context '1.2'
  $program = Get-Content (Join-Path (Get-PointRoot '1.2') 'Program.cs') -Raw
  foreach ($token in @('EnsureCreated()','Include(o => o.Planchas)','AddRange(plancha1, plancha2)','List<PlanchaAcero> Planchas')) {
    if (-not $program.Contains($token)) { throw "1.2: falta '$token'." }
  }
  Write-Host 'PASS 1.2 COMPLETO'
}

function Test-13 {
  Write-Section 'M01 1.3 · componentes + InitialCreate + AddAleacion'
  $out = Build-And-Run '1.3'
  Assert-TextContains -Text $out -Tokens @('OrdenFabricacion','PlanchaAcero','Aleacion') -Context '1.3 run'
  $m = Get-Migrations '1.3'
  Assert-TextContains -Text $m -Tokens @('InitialCreate','AddAleacion') -Context '1.3 migrations'
  if ($m.Contains('AddEstadoOrden')) { throw '1.3: AddEstadoOrden se ha adelantado.' }
  Write-Host 'PASS 1.3 COMPLETO'
}

function Test-14 {
  Write-Section 'M01 1.4 · DbContext + EstadoOrden'
  $out = Build-And-Run '1.4'
  Assert-TextContains -Text $out -Tokens @('OrdenFabricacion','PlanchaAcero','Aleacion','EstadoOrden') -Context '1.4 run'
  $m = Get-Migrations '1.4'
  Assert-TextContains -Text $m -Tokens @('InitialCreate','AddAleacion','AddEstadoOrden') -Context '1.4 migrations'
  Write-Host 'PASS 1.4 COMPLETO'
}

function Test-15 {
  Write-Section 'M01 1.5 · ciclo de vida del DbContext'
  $out = Build-And-Run '1.5'
  Assert-TextContains -Text $out -Tokens @('OF-001','OF-002','OF-003','Planchas insertadas:') -Context '1.5'
  $program = Get-Content (Join-Path (Get-PointRoot '1.5') 'Program.cs') -Raw
  foreach ($token in @('AceriaDbContextFactory','InsertarOrdenConPlanchas','Database.Migrate()')) {
    if (-not $program.Contains($token)) { throw "1.5: falta '$token'." }
  }
  Write-Host 'PASS 1.5 COMPLETO'
}

function Test-16 {
  Write-Section 'M01 1.6 · DbSet y operaciones básicas'
  $out = Build-And-Run '1.6'
  Assert-TextContains -Text $out -Tokens @(
    'Actualizada: True',
    'Existe OF-003: True',
    'Total de órdenes:',
    'Entidades rastreadas: 0',
    'Plancha insertada: True'
  ) -Context '1.6'
  Write-Host 'PASS 1.6 COMPLETO'
}

function Test-17 {
  Write-Section 'M01 1.7 · Change Tracker'
  $out = Build-And-Run '1.7'
  Assert-TextContains -Text $out -Tokens @(
    'Estado de entidad nueva:',
    'Estado tras Add:',
    'Estado tras SaveChanges:',
    'Estado tras modificar Cliente:',
    'Entidades rastreadas después de Clear: 0',
    'Cambios guardados.'
  ) -Context '1.7'
  Write-Host 'PASS 1.7 COMPLETO'
}

function Test-18 {
  Write-Section 'M01 1.8 · gestión de entidades'
  $out = Build-And-Run '1.8'
  Assert-TextContains -Text $out -Tokens @(
    'Estado antes de Add:',
    'Estado tras Update:',
    'Estado tras Attach:',
    'Estado tras Remove:',
    'Cliente actualizado para la orden'
  ) -Context '1.8'
  Write-Host 'PASS 1.8 COMPLETO'
}

function Test-19 {
  Write-Section 'M01 1.9 · SaveChanges y unidad de trabajo'
  $out = Build-And-Run '1.9'
  Assert-TextContains -Text $out -Tokens @(
    'Filas afectadas en la inserción:',
    'Órdenes en la base de datos tras el error:',
    'Filas afectadas con SaveChangesAsync:',
    'Filas afectadas en la unidad de trabajo:',
    'Plancha Id:'
  ) -Context '1.9'
  Write-Host 'PASS 1.9 COMPLETO'
}

function Test-110 {
  Write-Section 'M01 1.10 · migraciones reales'
  $root = Get-PointRoot '1.10'
  $out = Build-And-Run '1.10'
  Assert-TextContains -Text $out -Tokens @('--- Órdenes ---','OF-001','OF-002') -Context '1.10 run'

  $m = Get-Migrations '1.10'
  Assert-TextContains -Text $m -Tokens @('InitialCreate','AddAleacion','AddEstadoOrden') -Context '1.10 migrations'

  Invoke-Checked -WorkingDirectory $root -FilePath 'dotnet' -ArgumentList @(
    'ef','database','update','20260927000100_InitialCreate',
    '--project','AceriaData.Console.csproj','--startup-project','AceriaData.Console.csproj','--configuration','Release'
  ) -Context '1.10 reto rollback a InitialCreate' | Out-Null

  Invoke-Checked -WorkingDirectory $root -FilePath 'dotnet' -ArgumentList @(
    'ef','database','update','20260927000200_AddAleacion',
    '--project','AceriaData.Console.csproj','--startup-project','AceriaData.Console.csproj','--configuration','Release'
  ) -Context '1.10 reto reaplicar AddAleacion' | Out-Null

  Invoke-Checked -WorkingDirectory $root -FilePath 'dotnet' -ArgumentList @(
    'ef','database','update',
    '--project','AceriaData.Console.csproj','--startup-project','AceriaData.Console.csproj','--configuration','Release'
  ) -Context '1.10 reto volver al estado final' | Out-Null

  $script = Invoke-Checked -WorkingDirectory $root -FilePath 'dotnet' -ArgumentList @(
    'ef','migrations','script',
    '--project','AceriaData.Console.csproj','--startup-project','AceriaData.Console.csproj','--configuration','Release'
  ) -Context '1.10 migrations script'
  Assert-TextContains -Text $script -Tokens @('__EFMigrationsHistory','Aleaciones','EstadosOrden') -Context '1.10 script'
  Write-Host 'PASS 1.10 COMPLETO'
}

function Test-111 {
  Write-Section 'M01 1.11 · proveedor SQL Server único'
  $out = Build-And-Run '1.11'
  Assert-TextContains -Text $out -Tokens @(
    'Microsoft.EntityFrameworkCore.SqlServer',
    'SELECT',
    'WHERE'
  ) -Context '1.11'
  $program = Get-Content (Join-Path (Get-PointRoot '1.11') 'Program.cs') -Raw
  if ($program -match 'UseSqlite|UseNpgsql|UseInMemoryDatabase|Microsoft\.Data\.Sqlite') {
    throw '1.11: proveedor alternativo ejecutable.'
  }
  Write-Host 'PASS 1.11 COMPLETO'
}

function Test-112 {
  Write-Section 'M01 1.12 · DI + AddDbContext + repositorio + servicio'
  $out = Build-And-Run '1.12'
  Assert-TextContains -Text $out -Tokens @(
    '--- Órdenes ---',
    'OF-001',
    'OF-002',
    'Orden OF-001 | Cliente:',
    'Orden 999 no encontrada'
  ) -Context '1.12'
  $program = Get-Content (Join-Path (Get-PointRoot '1.12') 'Program.cs') -Raw
  foreach ($token in @('AddDbContext<AceriaDbContext>','AddScoped<IOrdenRepositorio, OrdenRepositorio>','AddScoped<IServicioOrdenes, ServicioOrdenes>','CreateScope()','Database.Migrate()')) {
    if (-not $program.Contains($token)) { throw "1.12: falta '$token'." }
  }
  Write-Host 'PASS 1.12 COMPLETO'
}

if ($Suite -eq 'inventory') { Test-Inventory; exit 0 }
if ($Suite -eq '1.1') { Test-11; exit 0 }
if ($Suite -eq '1.2') { Test-12; exit 0 }
if ($Suite -eq '1.3') { Test-13; exit 0 }
if ($Suite -eq '1.4') { Test-14; exit 0 }
if ($Suite -eq '1.5') { Test-15; exit 0 }
if ($Suite -eq '1.6') { Test-16; exit 0 }
if ($Suite -eq '1.7') { Test-17; exit 0 }
if ($Suite -eq '1.8') { Test-18; exit 0 }
if ($Suite -eq '1.9') { Test-19; exit 0 }
if ($Suite -eq '1.10') { Test-110; exit 0 }
if ($Suite -eq '1.11') { Test-111; exit 0 }
if ($Suite -eq '1.12') { Test-112; exit 0 }

Test-Inventory
Test-11
Test-12
Test-13
Test-14
Test-15
Test-16
Test-17
Test-18
Test-19
Test-110
Test-111
Test-112
Write-Host 'PASS M01 COMPLETO · 1.1–1.12 certificados.'
