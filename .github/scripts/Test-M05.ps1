param(
    [ValidateSet('all','inventory','5.1')]
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
    Write-Host ""
    Write-Host "============================================================"
    Write-Host $Name
    Write-Host "============================================================"
}

function Invoke-Build51([string]$Root,[string]$Context) {
    Invoke-Checked -WorkingDirectory $Root -FilePath 'dotnet' -ArgumentList @('build','AceriaData.sln','--configuration','Release') -Context $Context | Out-Null
}

function Invoke-Run51([string]$Root,[string]$Context) {
    return Invoke-Checked -WorkingDirectory $Root -FilePath 'dotnet' -ArgumentList @('run','--project','src/AceriaData.Console/AceriaData.Console.csproj','--configuration','Release','--no-build') -Context $Context
}

function Test-M05Inventory {
    Write-Section 'M05 · inventario canónico'

    $practicePath = Join-Path $RepoRoot 'M05\PRACTICA\M05_PRACTICA.md'
    $practice = Get-Content $practicePath -Raw
    $expectedSteps = @{
        '5.1' = 5; '5.2' = 6; '5.3' = 6; '5.4' = 6;
        '5.5' = 5; '5.6' = 6; '5.7' = 5; '5.8' = 5;
        '5.9' = 5; '5.10' = 7; '5.11' = 7; '5.12' = 6
    }

    foreach ($point in $expectedSteps.Keys | Sort-Object {[version]($_ -replace '^5\.','5.')}) {
        $pattern = '(?ms)^## Punto ' + [regex]::Escape($point) + '\b.*?(?=^## Punto 5\.\d+\b|\z)'
        $section = [regex]::Match($practice,$pattern).Value
        if ([string]::IsNullOrWhiteSpace($section)) {
            throw "M05: no se localiza $point en la práctica."
        }

        $steps = [regex]::Matches($section,'(?m)^### Paso (\d+):')
        if ($steps.Count -ne $expectedSteps[$point]) {
            throw "M05: $point debe contener $($expectedSteps[$point]) pasos y contiene $($steps.Count)."
        }

        for ($step = 1; $step -le $expectedSteps[$point]; $step++) {
            if ($section -notmatch ('(?m)^### Paso ' + $step + ':')) {
                throw "M05: falta $point/Paso $step."
            }
        }

        if ($section -notmatch '(?m)^### Reto') { throw "M05: $point no contiene reto." }
        if ($section -notmatch '(?m)^### Errores comunes\s*$') { throw "M05: $point no contiene Errores comunes." }

        Write-Host "PASS inventario $point · $($expectedSteps[$point]) pasos + reto + errores comunes"
    }
}

function Test-M051 {
    Write-Section 'M05 · 5.1 Concurrencia optimista: concepto y necesidad'

    $root = Join-Path $RepoRoot 'M05\PROYECTO\5.1'
    $repoRel = 'src\AceriaData.Infrastructure\Repositories\ConcurrenciaOptimistaM5Repositorio.cs'
    $useRel = 'src\AceriaData.Application\ConcurrenciaOptimistaM5UseCase.cs'
    $ifaceRel = 'src\AceriaData.Application\ConcurrenciaInterfaces.cs'
    $dtoRel = 'src\AceriaData.Application\ConcurrenciaDtos.cs'
    $programRel = 'src\AceriaData.Console\Program.cs'
    $domainRel = 'src\AceriaData.Domain\Entities.cs'
    $appProjectRel = 'src\AceriaData.Application\AceriaData.Application.csproj'

    Invoke-Checked -WorkingDirectory $root -FilePath 'dotnet' -ArgumentList @('restore','AceriaData.sln') -Context '5.1/Paso 1 restore' | Out-Null
    Write-Host 'PASS 5.1/Paso 1'

    $repo = Get-Content (Join-Path $root $repoRel) -Raw
    $use = Get-Content (Join-Path $root $useRel) -Raw
    $iface = Get-Content (Join-Path $root $ifaceRel) -Raw
    $domain = Get-Content (Join-Path $root $domainRel) -Raw

    foreach ($token in @(
        'ConcurrenciaOptimistaM5Repositorio',
        'ConcurrenciaOptimistaM5UseCase',
        'IConcurrenciaOptimistaM5Repositorio',
        'DemostrarActualizacionPerdidaMismaPropiedad',
        'DemostrarCambiosEnPropiedadesDistintas'
    )) {
        if (($repo + $use + $iface) -notmatch [regex]::Escape($token)) {
            throw "5.1/Paso 2: falta $token."
        }
    }

    $appProject = Get-Content (Join-Path $root $appProjectRel) -Raw
    if ($appProject -match 'EntityFrameworkCore|AceriaData.Infrastructure') {
        throw '5.1/Paso 2: Application depende de EF Core o Infrastructure.'
    }
    Write-Host 'PASS 5.1/Paso 2 · capas y archivos principales comprobados'

    foreach ($token in @(
        'using var scopeA = _scopeFactory.CreateScope();',
        'using var scopeB = _scopeFactory.CreateScope();',
        'ordenA.Cliente = "Cliente actualizado por A";',
        'ordenB.Cliente = "Cliente actualizado por B";',
        'contextA.SaveChanges();',
        'contextB.SaveChanges();',
        '.AsNoTracking()',
        'SnapshotCommands()'
    )) {
        if (-not $repo.Contains($token)) { throw "5.1/Paso 3: falta evidencia '$token'." }
    }
    if ($domain -match 'RowVersion|Timestamp|ConcurrencyCheck') {
        throw '5.1/Paso 3: 5.1 no debe tener todavía token de concurrencia.'
    }
    Write-Host 'PASS 5.1/Paso 3 · actualización perdida sin token representada con dos contextos'

    foreach ($token in @(
        'ordenA.Cliente = "Cliente actualizado por A";',
        'ordenB.Estado = "EnProceso";',
        'Select(o => new { o.Cliente, o.Estado })',
        'resultado.Cliente == ordenA.Cliente && resultado.Estado == ordenB.Estado'
    )) {
        if (-not $repo.Contains($token)) { throw "5.1/Paso 4: falta evidencia '$token'." }
    }
    Write-Host 'PASS 5.1/Paso 4 · cambios concurrentes en propiedades distintas representados'

    $migrations = Invoke-Checked -WorkingDirectory $root -FilePath 'dotnet' -ArgumentList @('ef','migrations','list','--project','src/AceriaData.Infrastructure','--startup-project','src/AceriaData.Console','--configuration','Release') -Context '5.1 migraciones heredadas'
    Assert-TextContains -Text $migrations -Tokens @('M2_2_12_Architecture') -Context '5.1 migraciones'
    if ($migrations -match '(?m)^\S*M5_') {
        throw '5.1: aparece una migración M5 aunque el modelo no cambia todavía.'
    }

    Invoke-Build51 -Root $root -Context '5.1/Paso 5 build'
    $out = Invoke-Run51 -Root $root -Context '5.1/Paso 5 run'

    Assert-TextContains -Text $out -Tokens @(
        '=== 5.1 CONCURRENCIA OPTIMISTA: CONCEPTO Y NECESIDAD ===',
        'Cliente final en base de datos: Cliente actualizado por B',
        'Cliente final: Cliente actualizado por A',
        'Estado final: EnProceso',
        'UPDATE [OrdenesFabricacion] SET [Cliente]',
        'UPDATE [OrdenesFabricacion] SET [Estado]',
        '5.1 OK'
    ) -Context '5.1/Paso 5'
    Write-Host 'PASS 5.1/Paso 5 · build, LocalDB, SQL observado y resultado final comprobados'

    $temp = New-PedagogicalCopy -Source $root -Name 'm05-5-1-reto-tercera-escritura'
    Enable-RetoBlock -Path (Join-Path $temp $dtoRel) -Marker 'RETO M05 5.1 - DTO TERCERA ESCRITURA'
    Enable-RetoBlock -Path (Join-Path $temp $ifaceRel) -Marker 'RETO M05 5.1 - PUERTO TERCERA ESCRITURA'
    Enable-RetoBlock -Path (Join-Path $temp $repoRel) -Marker 'RETO M05 5.1 - TERCERA ESCRITURA CONCURRENTE'
    Enable-RetoBlock -Path (Join-Path $temp $useRel) -Marker 'RETO M05 5.1 - TERCERA ESCRITURA CONCURRENTE'
    Enable-RetoBlock -Path (Join-Path $temp $programRel) -Marker 'RETO M05 5.1 - EJECUTAR TERCERA ESCRITURA CONCURRENTE'

    Invoke-Build51 -Root $temp -Context '5.1 reto build'
    $retoOut = Invoke-Run51 -Root $temp -Context '5.1 reto run'
    Assert-TextContains -Text $retoOut -Tokens @(
        'Reto 5.1 OK | final=Cliente actualizado por C | comandos=7',
        '5.1 OK'
    ) -Context '5.1 reto'
    Write-Host 'PASS 5.1/RETO · tercera escritura concurrente validada con 7 comandos reales'

    Write-Host 'PASS 5.1 COMPLETO'
}

if ($Suite -eq 'inventory') { Test-M05Inventory; exit 0 }
if ($Suite -eq '5.1') { Test-M051; exit 0 }

Test-M05Inventory
Test-M051
Write-Host 'PASS M05 PARCIAL · 5.1 certificado; siguiente checkpoint: 5.2.'
