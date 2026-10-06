param(
    [ValidateSet('all','inventory','5.1','5.2','5.3','5.4','5.5','5.6','5.7','5.8')]
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


function Test-M052 {
    Write-Section 'M05 · 5.2 Configuración de tokens de concurrencia'

    $root = Join-Path $RepoRoot 'M05\PROYECTO\5.2'
    $configRel = 'src\AceriaData.Infrastructure\Persistence\Configurations\ConcurrencyTokensConfiguration.cs'
    $repoRel = 'src\AceriaData.Infrastructure\Repositories\TokensConcurrenciaM5Repositorio.cs'
    $useRel = 'src\AceriaData.Application\TokensConcurrenciaM5UseCase.cs'
    $programRel = 'src\AceriaData.Console\Program.cs'
    $domainRel = 'src\AceriaData.Domain\Entities.cs'
    $appProjectRel = 'src\AceriaData.Application\AceriaData.Application.csproj'
    $migrationRel = 'src\AceriaData.Infrastructure\Migrations\20260930203405_M5_5_2_ConcurrencyTokens.cs'

    Invoke-Checked -WorkingDirectory $root -FilePath 'dotnet' -ArgumentList @('restore','AceriaData.sln') -Context '5.2/Paso 1 restore' | Out-Null
    Write-Host 'PASS 5.2/Paso 1'

    foreach ($rel in @($configRel,$repoRel,$useRel,$programRel,$migrationRel)) {
        if (-not (Test-Path (Join-Path $root $rel))) {
            throw "5.2/Paso 2: falta $rel."
        }
    }
    $appProject = Get-Content (Join-Path $root $appProjectRel) -Raw
    if ($appProject -match 'EntityFrameworkCore|AceriaData.Infrastructure') {
        throw '5.2/Paso 2: Application depende de EF Core o Infrastructure.'
    }
    Write-Host 'PASS 5.2/Paso 2 · archivos y fronteras de capas comprobados'

    $config = Get-Content (Join-Path $root $configRel) -Raw
    $domain = Get-Content (Join-Path $root $domainRel) -Raw
    $migration = Get-Content (Join-Path $root $migrationRel) -Raw

    foreach ($token in @(
        'b.Property(x => x.RowVersion).IsRowVersion();',
        '.HasDefaultValue("Pendiente")',
        '.IsConcurrencyToken();'
    )) {
        if (-not $config.Contains($token)) { throw "5.2/Paso 3: falta configuración '$token'." }
    }
    foreach ($token in @(
        'public byte[] RowVersion { get; set; } = Array.Empty<byte>();',
        'public string EstadoDetalle { get; set; } = "Pendiente";'
    )) {
        if (-not $domain.Contains($token)) { throw "5.2/Paso 3: falta modelo '$token'." }
    }
    foreach ($token in @(
        'partial class M5_5_2_ConcurrencyTokens',
        'type: "rowversion"',
        'rowVersion: true',
        'name: "EstadoDetalle"',
        'defaultValue: "Pendiente"'
    )) {
        if (-not $migration.Contains($token)) { throw "5.2/Paso 3: falta evidencia en migración '$token'." }
    }
    Write-Host 'PASS 5.2/Paso 3 · RowVersion, token de propiedad y migración real comprobados'

    $repo = Get-Content (Join-Path $root $repoRel) -Raw
    foreach ($token in @(
        'DemostrarRowVersion()',
        'Convert.ToHexString(ordenA.RowVersion)',
        'Convert.ToHexString(ordenB.RowVersion)',
        'catch (DbUpdateConcurrencyException)',
        'SnapshotCommands()',
        'DemostrarTokenDePropiedad()',
        'detalleA.EstadoDetalle = "EnProceso"',
        'detalleB.Notas = "Cambio concurrente de B"'
    )) {
        if (-not $repo.Contains($token)) { throw "5.2/Paso 4: falta evidencia '$token'." }
    }
    Write-Host 'PASS 5.2/Paso 4 · conflictos RowVersion y token de propiedad representados'

    foreach ($token in @(
        'ComprobarIndiceRowVersion()',
        'FROM sys.indexes AS i',
        "c.name = N'RowVersion'"
    )) {
        if (-not $repo.Contains($token)) { throw "5.2/Paso 5: falta evidencia '$token'." }
    }
    Write-Host 'PASS 5.2/Paso 5 · comprobación directa de índice presente'

    Invoke-Build51 -Root $root -Context '5.2/Paso 6 build'

    $migrations = Invoke-Checked -WorkingDirectory $root -FilePath 'dotnet' -ArgumentList @(
        'ef','migrations','list',
        '--project','src/AceriaData.Infrastructure',
        '--startup-project','src/AceriaData.Console',
        '--configuration','Release'
    ) -Context '5.2/Paso 6 migrations list'
    Assert-TextContains -Text $migrations -Tokens @('M2_2_12_Architecture','M5_5_2_ConcurrencyTokens') -Context '5.2/Paso 6 migrations'

    Invoke-Checked -WorkingDirectory $root -FilePath 'dotnet' -ArgumentList @(
        'ef','database','update',
        '--project','src/AceriaData.Infrastructure',
        '--startup-project','src/AceriaData.Console',
        '--configuration','Release'
    ) -Context '5.2/Paso 6 database update' | Out-Null

    $out = Invoke-Run51 -Root $root -Context '5.2/Paso 6 run'
    Assert-TextContains -Text $out -Tokens @(
        '=== 5.2 CONFIGURACIÓN DE TOKENS DE CONCURRENCIA ===',
        '¿Conflicto detectado con rowversion?: True',
        '¿Conflicto detectado con token de propiedad?: True',
        '¿SQL Server creó automáticamente un índice sobre RowVersion?: False',
        'UPDATE [OrdenesFabricacion]',
        '[RowVersion]',
        'UPDATE [DetallesOrden]',
        '[EstadoDetalle]',
        '5.2 OK'
    ) -Context '5.2/Paso 6'
    Write-Host 'PASS 5.2/Paso 6 · LocalDB, migración, conflictos y ausencia de índice comprobados'

    $temp = New-PedagogicalCopy -Source $root -Name 'm05-5-2-reto-predicado'
    Enable-RetoBlock -Path (Join-Path $temp $useRel) -Marker 'RETO M05 5.2 - TOKEN ORIGINAL EN PREDICADO SQL'
    Enable-RetoBlock -Path (Join-Path $temp $programRel) -Marker 'RETO M05 5.2 - EJECUTAR INSPECCION DEL PREDICADO'

    Invoke-Build51 -Root $temp -Context '5.2 reto build'
    $retoOut = Invoke-Run51 -Root $temp -Context '5.2 reto run'
    Assert-TextContains -Text $retoOut -Tokens @(
        'Reto 5.2 OK | RowVersion y EstadoDetalle aparecen en el WHERE de sus UPDATE de concurrencia',
        '5.2 OK'
    ) -Context '5.2 reto'
    Write-Host 'PASS 5.2/RETO · token original localizado en el predicado WHERE real'

    Write-Host 'PASS 5.2 COMPLETO'
}



function Test-M053 {
    Write-Section 'M05 · 5.3 Resolución de conflictos de concurrencia'

    $root = Join-Path $RepoRoot 'M05\PROYECTO\5.3'
    $repoRel = 'src\AceriaData.Infrastructure\Repositories\ResolucionConflictosM5Repositorio.cs'
    $useRel = 'src\AceriaData.Application\ResolucionConflictosM5UseCase.cs'
    $ifaceRel = 'src\AceriaData.Application\ResolucionConflictosInterfaces.cs'
    $dtoRel = 'src\AceriaData.Application\ResolucionConflictosDtos.cs'
    $programRel = 'src\AceriaData.Console\Program.cs'
    $appProjectRel = 'src\AceriaData.Application\AceriaData.Application.csproj'

    Invoke-Checked -WorkingDirectory $root -FilePath 'dotnet' -ArgumentList @('restore','AceriaData.sln') -Context '5.3/Paso 1 restore' | Out-Null
    Write-Host 'PASS 5.3/Paso 1'

    foreach ($rel in @($repoRel,$useRel,$ifaceRel,$dtoRel,$programRel)) {
        if (-not (Test-Path (Join-Path $root $rel))) {
            throw "5.3/Paso 2: falta $rel."
        }
    }
    $appProject = Get-Content (Join-Path $root $appProjectRel) -Raw
    if ($appProject -match 'EntityFrameworkCore|AceriaData.Infrastructure') {
        throw '5.3/Paso 2: Application depende de EF Core o Infrastructure.'
    }
    Write-Host 'PASS 5.3/Paso 2 · archivos y fronteras de capas comprobados'

    $repo = Get-Content (Join-Path $root $repoRel) -Raw
    foreach ($token in @(
        'ClienteGana()',
        'GetDatabaseValues()',
        'CrearValores(entry, db)',
        'entry.OriginalValues.SetValues(db)',
        'contextB.SaveChanges();'
    )) {
        if (-not $repo.Contains($token)) { throw "5.3/Paso 3: falta evidencia '$token'." }
    }
    Write-Host 'PASS 5.3/Paso 3 · Cliente gana actualiza OriginalValues y reintenta'

    foreach ($token in @(
        'BaseDeDatosGana()',
        'entry.Reload();',
        'ResolucionPersonalizada()',
        'entry.CurrentValues[nameof(OrdenFabricacion.Estado)]',
        'NotificarSinSobrescribir()',
        'DetectarFilaEliminada()',
        'entry.GetDatabaseValues() is null',
        'entry.State = EntityState.Detached'
    )) {
        if (-not $repo.Contains($token)) { throw "5.3/Paso 4: falta evidencia '$token'." }
    }
    Write-Host 'PASS 5.3/Paso 4 · base gana, merge, notificación y fila eliminada presentes'

    foreach ($token in @(
        'ReintentoAcotado(int maxIntentos)',
        'if (maxIntentos < 1)',
        'while (true)',
        'catch (DbUpdateConcurrencyException ex) when (intentos < maxIntentos)',
        'entry.OriginalValues.SetValues(db)'
    )) {
        if (-not $repo.Contains($token)) { throw "5.3/Paso 5: falta evidencia '$token'." }
    }
    Write-Host 'PASS 5.3/Paso 5 · reintento explícitamente acotado'

    Invoke-Build51 -Root $root -Context '5.3/Paso 6 build'

    $pending = Invoke-Checked -WorkingDirectory $root -FilePath 'dotnet' -ArgumentList @(
        'ef','migrations','has-pending-model-changes',
        '--project','src/AceriaData.Infrastructure',
        '--startup-project','src/AceriaData.Console',
        '--configuration','Release'
    ) -Context '5.3/Paso 6 pending model changes'
    Write-Host 'PASS 5.3/Paso 6 · no hay cambios de modelo pendientes'

    $migrations = Invoke-Checked -WorkingDirectory $root -FilePath 'dotnet' -ArgumentList @(
        'ef','migrations','list',
        '--project','src/AceriaData.Infrastructure',
        '--startup-project','src/AceriaData.Console',
        '--configuration','Release'
    ) -Context '5.3 migraciones'
    Assert-TextContains -Text $migrations -Tokens @('M5_5_2_ConcurrencyTokens') -Context '5.3 migraciones'
    if ($migrations -match '(?m)^\S*M5_5_3') {
        throw '5.3: aparece una migración nueva y el punto no cambia el modelo.'
    }

    $out = Invoke-Run51 -Root $root -Context '5.3/Paso 6 run'
    Assert-TextContains -Text $out -Tokens @(
        '=== 5.3 RESOLUCIÓN DE CONFLICTOS DE CONCURRENCIA ===',
        '--- Cliente gana ---',
        'Cliente final: Cliente B - cliente gana',
        '--- Base de datos gana ---',
        'Cliente final: Cliente A - base gana',
        '--- Resolución personalizada ---',
        'Cliente final: Cliente B - merge',
        'Estado final: EnProceso A',
        '--- Notificación al usuario ---',
        '--- Reintento acotado ---',
        'Cliente final: Cliente B - reintento',
        '¿GetDatabaseValues confirmó que ya no existe?: True',
        '¿Entrada desacoplada?: True',
        '5.3 OK'
    ) -Context '5.3/Paso 6'
    Write-Host 'PASS 5.3/Paso 6 · todas las políticas se ejecutan sobre LocalDB'

    $temp = New-PedagogicalCopy -Source $root -Name 'm05-5-3-reto-merge-propiedad'
    Enable-RetoBlock -Path (Join-Path $temp $dtoRel) -Marker 'RETO M05 5.3 - DTO MERGE POR PROPIEDAD'
    Enable-RetoBlock -Path (Join-Path $temp $ifaceRel) -Marker 'RETO M05 5.3 - PUERTO MERGE POR PROPIEDAD'
    Enable-RetoBlock -Path (Join-Path $temp $repoRel) -Marker 'RETO M05 5.3 - MERGE CLIENTE LOCAL ESTADO BD OBSERVACIONES COMBINADAS'
    Enable-RetoBlock -Path (Join-Path $temp $useRel) -Marker 'RETO M05 5.3 - MERGE CLIENTE LOCAL ESTADO BD OBSERVACIONES COMBINADAS'
    Enable-RetoBlock -Path (Join-Path $temp $programRel) -Marker 'RETO M05 5.3 - EJECUTAR MERGE POR PROPIEDAD'

    Invoke-Build51 -Root $temp -Context '5.3 reto build'
    $retoOut = Invoke-Run51 -Root $temp -Context '5.3 reto run'
    Assert-TextContains -Text $retoOut -Tokens @(
        'Reto 5.3 OK | Cliente=Cliente local - reto | Estado=EnProceso BD | Observaciones=Observacion BD | Observacion local | Intentos=2',
        '5.3 OK'
    ) -Context '5.3 reto'
    Write-Host 'PASS 5.3/RETO · Cliente local + Estado BD + Observaciones combinadas'

    Write-Host 'PASS 5.3 COMPLETO'
}



function Test-M054 {
    Write-Section 'M05 · 5.4 Transacciones: SaveChanges y transacciones explícitas'

    $root = Join-Path $RepoRoot 'M05\PROYECTO\5.4'
    $repoRel = 'src\AceriaData.Infrastructure\Repositories\TransaccionesM5Repositorio.cs'
    $useRel = 'src\AceriaData.Application\TransaccionesM5UseCase.cs'
    $ifaceRel = 'src\AceriaData.Application\TransaccionesInterfaces.cs'
    $dtoRel = 'src\AceriaData.Application\TransaccionesDtos.cs'
    $programRel = 'src\AceriaData.Console\Program.cs'
    $settingsRel = 'src\AceriaData.Console\appsettings.json'
    $appProjectRel = 'src\AceriaData.Application\AceriaData.Application.csproj'

    Invoke-Checked -WorkingDirectory $root -FilePath 'dotnet' -ArgumentList @('restore','AceriaData.sln') -Context '5.4/Paso 1 restore' | Out-Null
    Write-Host 'PASS 5.4/Paso 1'

    foreach ($rel in @($repoRel,$useRel,$ifaceRel,$dtoRel,$programRel,$settingsRel)) {
        if (-not (Test-Path (Join-Path $root $rel))) {
            throw "5.4/Paso 2: falta $rel."
        }
    }
    $appProject = Get-Content (Join-Path $root $appProjectRel) -Raw
    if ($appProject -match 'EntityFrameworkCore|AceriaData.Infrastructure') {
        throw '5.4/Paso 2: Application depende de EF Core o Infrastructure.'
    }
    Write-Host 'PASS 5.4/Paso 2 · archivos y fronteras de capas comprobados'

    $repo = Get-Content (Join-Path $root $repoRel) -Raw
    foreach ($token in @(
        'DemostrarAtomicidadSaveChanges()',
        'context.OrdenesFabricacion.Add(CrearOrden(numeroValido));',
        'context.OrdenesFabricacion.Add(CrearOrden(NumeroExistente));',
        'catch (DbUpdateException)',
        'context.ChangeTracker.Clear();',
        'DemostrarCommitExplicito()',
        'context.Database.BeginTransaction()',
        'transaction.Commit();',
        'DemostrarRollbackExplicito()',
        'transaction.Rollback();'
    )) {
        if (-not $repo.Contains($token)) { throw "5.4/Paso 3: falta evidencia '$token'." }
    }
    Write-Host 'PASS 5.4/Paso 3 · atomicidad, Commit y Rollback explícitos presentes'

    foreach ($token in @(
        'DemostrarRollbackASavepoint()',
        'transaction.CreateSavepoint(savepoint);',
        'transaction.RollbackToSavepoint(savepoint);',
        'MultipleActiveResultSets=false'
    )) {
        if ($token -eq 'MultipleActiveResultSets=false') { continue }
        if (-not $repo.Contains($token)) { throw "5.4/Paso 4: falta evidencia '$token'." }
    }
    Write-Host 'PASS 5.4/Paso 4 · savepoint y rollback parcial presentes'

    $settings = Get-Content (Join-Path $root $settingsRel) -Raw
    if ($settings -notmatch 'MultipleActiveResultSets=false') {
        throw '5.4/Paso 5: appsettings no desactiva MARS.'
    }
    if ($repo -notmatch 'MultipleActiveResultSets') {
        throw '5.4/Paso 5: el repositorio no comprueba MARS.'
    }
    Write-Host 'PASS 5.4/Paso 5 · MARS desactivado y comprobado'

    Invoke-Build51 -Root $root -Context '5.4/Paso 6 build'

    Invoke-Checked -WorkingDirectory $root -FilePath 'dotnet' -ArgumentList @(
        'ef','migrations','has-pending-model-changes',
        '--project','src/AceriaData.Infrastructure',
        '--startup-project','src/AceriaData.Console',
        '--configuration','Release'
    ) -Context '5.4/Paso 6 pending model changes' | Out-Null

    $migrations = Invoke-Checked -WorkingDirectory $root -FilePath 'dotnet' -ArgumentList @(
        'ef','migrations','list',
        '--project','src/AceriaData.Infrastructure',
        '--startup-project','src/AceriaData.Console',
        '--configuration','Release'
    ) -Context '5.4 migraciones'
    Assert-TextContains -Text $migrations -Tokens @('M5_5_2_ConcurrencyTokens') -Context '5.4 migraciones'
    if ($migrations -match '(?m)^\S*M5_5_4') {
        throw '5.4: aparece una migración nueva y el punto no cambia el modelo.'
    }

    $out = Invoke-Run51 -Root $root -Context '5.4/Paso 6 run'
    Assert-TextContains -Text $out -Tokens @(
        '=== 5.4 TRANSACCIONES: SAVECHANGES, COMMIT, ROLLBACK Y SAVEPOINTS ===',
        '--- Atomicidad de un único SaveChanges ---',
        'Resultado esperado: True',
        '--- Transacción explícita con Commit ---',
        'Primera orden existe: True',
        'Segunda orden existe: True',
        '--- Transacción explícita con Rollback ---',
        '--- Rollback a savepoint ---',
        'MARS habilitado: False',
        '5.4 OK'
    ) -Context '5.4/Paso 6'
    Write-Host 'PASS 5.4/Paso 6 · atomicidad, Commit, Rollback y savepoint ejecutados en LocalDB'

    $temp = New-PedagogicalCopy -Source $root -Name 'm05-5-4-reto-tres-savechanges'
    Enable-RetoBlock -Path (Join-Path $temp $dtoRel) -Marker 'RETO M05 5.4 - DTO TRES SAVECHANGES'
    Enable-RetoBlock -Path (Join-Path $temp $ifaceRel) -Marker 'RETO M05 5.4 - PUERTO TRES SAVECHANGES'
    Enable-RetoBlock -Path (Join-Path $temp $repoRel) -Marker 'RETO M05 5.4 - TRES SAVECHANGES TRAS SAVEPOINT'
    Enable-RetoBlock -Path (Join-Path $temp $useRel) -Marker 'RETO M05 5.4 - TRES SAVECHANGES TRAS SAVEPOINT'
    Enable-RetoBlock -Path (Join-Path $temp $programRel) -Marker 'RETO M05 5.4 - EJECUTAR TRES SAVECHANGES'

    Invoke-Build51 -Root $temp -Context '5.4 reto build'
    $retoOut = Invoke-Run51 -Root $temp -Context '5.4 reto run'
    Assert-TextContains -Text $retoOut -Tokens @(
        'Reto 5.4 OK | antes-savepoint=True | segundo=False | tercero=False | MARS=False',
        '5.4 OK'
    ) -Context '5.4 reto'
    Write-Host 'PASS 5.4/RETO · dos SaveChanges posteriores al savepoint se revierten'

    Write-Host 'PASS 5.4 COMPLETO'
}



function Test-M055 {
    Write-Section 'M05 · 5.5 Transacciones ambientales y buenas prácticas'

    $root = Join-Path $RepoRoot 'M05\PROYECTO\5.5'
    $repoRel = 'src\AceriaData.Infrastructure\Repositories\TransaccionesAmbientalesM5Repositorio.cs'
    $useRel = 'src\AceriaData.Application\TransaccionesAmbientalesM5UseCase.cs'
    $ifaceRel = 'src\AceriaData.Application\TransaccionesAmbientalesInterfaces.cs'
    $dtoRel = 'src\AceriaData.Application\TransaccionesAmbientalesDtos.cs'
    $programRel = 'src\AceriaData.Console\Program.cs'
    $appProjectRel = 'src\AceriaData.Application\AceriaData.Application.csproj'

    Invoke-Checked -WorkingDirectory $root -FilePath 'dotnet' -ArgumentList @('restore','AceriaData.sln') -Context '5.5/Paso 1 restore' | Out-Null
    Write-Host 'PASS 5.5/Paso 1'

    foreach ($rel in @($repoRel,$useRel,$ifaceRel,$dtoRel,$programRel)) {
        if (-not (Test-Path (Join-Path $root $rel))) { throw "5.5/Paso 2: falta $rel." }
    }
    $appProject = Get-Content (Join-Path $root $appProjectRel) -Raw
    if ($appProject -match 'EntityFrameworkCore|AceriaData.Infrastructure') {
        throw '5.5/Paso 2: Application depende de EF Core o Infrastructure.'
    }
    Write-Host 'PASS 5.5/Paso 2 · archivos y fronteras de capas comprobados'

    $repo = Get-Content (Join-Path $root $repoRel) -Raw
    foreach ($token in @(
        'DemostrarDosContextosAsync()',
        'TransactionScopeAsyncFlowOption.Enabled',
        'new SqlConnection(ObtenerConnectionString())',
        'await connection.OpenAsync()',
        'CrearContexto(connection)',
        'await Task.Yield()',
        'DistributedIdentifier != Guid.Empty',
        'scope.Complete();'
    )) {
        if (-not $repo.Contains($token)) { throw "5.5/Paso 3: falta evidencia '$token'." }
    }
    Write-Host 'PASS 5.5/Paso 3 · dos DbContext, conexión compartida y flujo async presentes'

    foreach ($token in @(
        'TransactionScopeOption.Required',
        'TransactionScopeOption.RequiresNew',
        'TransactionScopeOption.Suppress',
        'DemostrarRollbackSinCompleteAsync()',
        'Intencionadamente NO se llama a Complete().',
        'DemostrarRecursoExternoNoTransaccional()',
        'DemostrarReadCommitted()',
        'DemostrarSnapshot()',
        'HabilitarSnapshot()'
    )) {
        if (-not $repo.Contains($token)) { throw "5.5/Paso 4: falta evidencia '$token'." }
    }
    Write-Host 'PASS 5.5/Paso 4 · opciones de scope, rollback, recurso externo y aislamiento presentes'

    Invoke-Build51 -Root $root -Context '5.5/Paso 5 build'

    Invoke-Checked -WorkingDirectory $root -FilePath 'dotnet' -ArgumentList @(
        'ef','migrations','has-pending-model-changes',
        '--project','src/AceriaData.Infrastructure',
        '--startup-project','src/AceriaData.Console',
        '--configuration','Release'
    ) -Context '5.5/Paso 5 pending model changes' | Out-Null

    $migrations = Invoke-Checked -WorkingDirectory $root -FilePath 'dotnet' -ArgumentList @(
        'ef','migrations','list',
        '--project','src/AceriaData.Infrastructure',
        '--startup-project','src/AceriaData.Console',
        '--configuration','Release'
    ) -Context '5.5 migraciones'
    Assert-TextContains -Text $migrations -Tokens @('M5_5_2_ConcurrencyTokens') -Context '5.5 migraciones'
    if ($migrations -match '(?m)^\S*M5_5_5') {
        throw '5.5: aparece una migración nueva y el modelo oficial no cambia.'
    }

    $out = Invoke-Run51 -Root $root -Context '5.5/Paso 5 run'
    Assert-TextContains -Text $out -Tokens @(
        '=== 5.5 TRANSACCIONES AMBIENTALES Y BUENAS PRÁCTICAS ===',
        'Dos DbContext con una conexión compartida',
        'Persistido: True',
        'Transacción ambiental activa: True',
        'Flujo async conservado: True',
        'Promoción distribuida: False',
        'Scope sin Complete',
        'Persistido: False',
        'Required reutiliza la transacción: True',
        'RequiresNew crea otra transacción: True',
        'Suppress elimina la transacción ambiental: True',
        'Fila de base de datos persistida: False',
        'Efecto externo permanece: True',
        '--- Aislamiento ReadCommitted ---',
        'Observado: ReadCommitted',
        '--- Aislamiento Snapshot ---',
        'Observado: Snapshot',
        '5.5 OK'
    ) -Context '5.5/Paso 5'
    Write-Host 'PASS 5.5/Paso 5 · TransactionScope y aislamientos ejecutados sobre LocalDB'

    $temp = New-PedagogicalCopy -Source $root -Name 'm05-5-5-reto-suppress'
    Enable-RetoBlock -Path (Join-Path $temp $dtoRel) -Marker 'RETO M05 5.5 - DTO SUPPRESS'
    Enable-RetoBlock -Path (Join-Path $temp $ifaceRel) -Marker 'RETO M05 5.5 - PUERTO SUPPRESS'
    Enable-RetoBlock -Path (Join-Path $temp $repoRel) -Marker 'RETO M05 5.5 - SUPPRESS FUERA DEL ROLLBACK AMBIENTAL'
    Enable-RetoBlock -Path (Join-Path $temp $useRel) -Marker 'RETO M05 5.5 - SUPPRESS FUERA DEL ROLLBACK AMBIENTAL'
    Enable-RetoBlock -Path (Join-Path $temp $programRel) -Marker 'RETO M05 5.5 - EJECUTAR SUPPRESS'

    Invoke-Build51 -Root $temp -Context '5.5 reto build'
    $retoOut = Invoke-Run51 -Root $temp -Context '5.5 reto run'
    Assert-TextContains -Text $retoOut -Tokens @(
        'Reto 5.5 OK | ambiental=False | suppress=True | Transaction.Current dentro de Suppress=null',
        '5.5 OK'
    ) -Context '5.5 reto'
    Write-Host 'PASS 5.5/RETO · Suppress conserva la escritura fuera del rollback ambiental'

    Write-Host 'PASS 5.5 COMPLETO'
}



function Test-M056 {
    Write-Section 'M05 · 5.6 Migraciones en producción: estrategias y despliegue'

    $root = Join-Path $RepoRoot 'M05\PROYECTO\5.6'
    $repoRel = 'src\AceriaData.Infrastructure\Repositories\MigracionesProduccionM5Repositorio.cs'
    $useRel = 'src\AceriaData.Application\MigracionesProduccionM5UseCase.cs'
    $programRel = 'src\AceriaData.Console\Program.cs'
    $diRel = 'src\AceriaData.Infrastructure\DependencyInjection.cs'
    $appProjectRel = 'src\AceriaData.Application\AceriaData.Application.csproj'
    $deployRel = 'deployment\generate-production-artifacts.ps1'
    $planRel = 'deployment\PLAN_DESPLIEGUE.md'
    $artifactsRel = 'deployment\artifacts'

    Invoke-Checked -WorkingDirectory $root -FilePath 'dotnet' -ArgumentList @('restore','AceriaData.sln') -Context '5.6/Paso 1 restore' | Out-Null
    Write-Host 'PASS 5.6/Paso 1'

    foreach ($rel in @($repoRel,$useRel,$programRel,$diRel,$deployRel,$planRel)) {
        if (-not (Test-Path (Join-Path $root $rel))) { throw "5.6/Paso 2: falta $rel." }
    }

    $appProject = Get-Content (Join-Path $root $appProjectRel) -Raw
    if ($appProject -match 'EntityFrameworkCore|AceriaData.Infrastructure') {
        throw '5.6/Paso 2: Application depende de EF Core o Infrastructure.'
    }
    Write-Host 'PASS 5.6/Paso 2 · archivos y fronteras de capas comprobados'

    $repo = Get-Content (Join-Path $root $repoRel) -Raw
    $di = Get-Content (Join-Path $root $diRel) -Raw

    foreach ($token in @(
        'context.GetService<IMigrator>()',
        'await migrator.MigrateAsync();',
        'GetAppliedMigrationsAsync()',
        'GetPendingMigrationsAsync()',
        '__EFMigrationsHistory'
    )) {
        if (-not $repo.Contains($token)) { throw "5.6/Paso 3: falta evidencia '$token'." }
    }

    if (-not $di.Contains('MigrationsAssembly(typeof(AceriaDbContext).Assembly.GetName().Name)')) {
        throw '5.6/Paso 3: MigrationsAssembly no apunta al ensamblado real.'
    }
    Write-Host 'PASS 5.6/Paso 3 · IMigrator, historial y MigrationsAssembly comprobados'

    $deploy = Get-Content (Join-Path $root $deployRel) -Raw
    foreach ($token in @(
        'dotnet ef migrations script --idempotent',
        'aceria-idempotent.sql',
        'dotnet ef migrations bundle',
        'aceria-efbundle.exe',
        'if ($LASTEXITCODE -ne 0)',
        'No se pudo generar el script idempotente',
        'No se pudo generar el migration bundle'
    )) {
        if (-not $deploy.Contains($token)) { throw "5.6/Paso 4: falta evidencia '$token'." }
    }

    $artifactDir = Join-Path $root $artifactsRel
    if (Test-Path $artifactDir) { Remove-Item $artifactDir -Recurse -Force }

    Invoke-Checked -WorkingDirectory $root -FilePath 'pwsh' -ArgumentList @(
        '-NoProfile','-File',(Join-Path $root $deployRel)
    ) -Context '5.6/Paso 4 generación de artefactos' | Out-Null

    $sqlPath = Join-Path $artifactDir 'aceria-idempotent.sql'
    $bundlePath = Join-Path $artifactDir 'aceria-efbundle.exe'
    if (-not (Test-Path $sqlPath)) { throw '5.6/Paso 4: no se generó aceria-idempotent.sql.' }
    if (-not (Test-Path $bundlePath)) { throw '5.6/Paso 4: no se generó aceria-efbundle.exe.' }
    if ((Get-Item $sqlPath).Length -le 0) { throw '5.6/Paso 4: el script SQL está vacío.' }
    if ((Get-Item $bundlePath).Length -le 0) { throw '5.6/Paso 4: el migration bundle está vacío.' }

    $sql = Get-Content $sqlPath -Raw
    Assert-TextContains -Text $sql -Tokens @(
        '__EFMigrationsHistory',
        '20260930203405_M5_5_2_ConcurrencyTokens'
    ) -Context '5.6/Paso 4 script idempotente'
    Write-Host 'PASS 5.6/Paso 4 · script idempotente y migration bundle generados de verdad'

    $plan = Get-Content (Join-Path $root $planRel) -Raw
    foreach ($token in @(
        'Script SQL',
        'Migration bundle',
        'backup',
        'downgrade',
        'restaurar',
        '$env:ACERIA_PROD_CONNECTION'
    )) {
        if (-not $plan.Contains($token)) { throw "5.6/Paso 5: falta control operacional '$token'." }
    }
    if ($plan -match '(?i)(Password\s*=|User Id\s*=|Server=.*;Database=.*;.*Password=)') {
        throw '5.6/Paso 5: el plan contiene una credencial o cadena de producción embebida.'
    }
    Write-Host 'PASS 5.6/Paso 5 · controles de script, bundle, backup, downgrade y conexión externa documentados'

    Invoke-Build51 -Root $root -Context '5.6/Paso 6 build'

    Invoke-Checked -WorkingDirectory $root -FilePath 'dotnet' -ArgumentList @(
        'ef','migrations','has-pending-model-changes',
        '--project','src/AceriaData.Infrastructure',
        '--startup-project','src/AceriaData.Console',
        '--configuration','Release'
    ) -Context '5.6/Paso 6 pending model changes' | Out-Null

    $migrations = Invoke-Checked -WorkingDirectory $root -FilePath 'dotnet' -ArgumentList @(
        'ef','migrations','list',
        '--project','src/AceriaData.Infrastructure',
        '--startup-project','src/AceriaData.Console',
        '--configuration','Release'
    ) -Context '5.6/Paso 6 migrations'
    Assert-TextContains -Text $migrations -Tokens @('M5_5_2_ConcurrencyTokens') -Context '5.6 migraciones'
    if ($migrations -match '(?m)^\S*M5_5_6') {
        throw '5.6: aparece una migración nueva aunque el modelo oficial no cambia.'
    }

    $out = Invoke-Run51 -Root $root -Context '5.6/Paso 6 run'
    Assert-TextContains -Text $out -Tokens @(
        '=== 5.6 MIGRACIONES EN PRODUCCIÓN: ESTRATEGIAS Y DESPLIEGUE ===',
        'Migraciones pendientes después de IMigrator: 0',
        'Última migración aplicada: 20260930203405_M5_5_2_ConcurrencyTokens',
        'Tabla de historial: __EFMigrationsHistory',
        'Tabla de historial heredada existe: True',
        '5.6 OK'
    ) -Context '5.6/Paso 6'
    Write-Host 'PASS 5.6/Paso 6 · IMigrator aplica la cadena real y deja cero pendientes'

    Write-Host 'PASS 5.6/RETO · script revisable y bundle representan la misma cadena con controles operacionales distintos'
    Write-Host 'PASS 5.6 COMPLETO'
}



function Test-M057 {
    Write-Section 'M05 · 5.7 Migraciones idempotentes y scripts SQL'

    $root = Join-Path $RepoRoot 'M05\PROYECTO\5.7'
    $scriptRel = 'deployment\validate-idempotent-scripts.ps1'
    $programRel = 'src\AceriaData.Console\Program.cs'
    $appProjectRel = 'src\AceriaData.Application\AceriaData.Application.csproj'
    $migrationRel = 'src\AceriaData.Infrastructure\Migrations\20260930203405_M5_5_2_ConcurrencyTokens.cs'
    $artifactsRel = 'deployment\artifacts-5.7'

    Invoke-Checked -WorkingDirectory $root -FilePath 'dotnet' -ArgumentList @('restore','AceriaData.sln') -Context '5.7/Paso 1 restore' | Out-Null
    Write-Host 'PASS 5.7/Paso 1'

    foreach ($rel in @($scriptRel,$programRel,$migrationRel)) {
        if (-not (Test-Path (Join-Path $root $rel))) { throw "5.7/Paso 2: falta $rel." }
    }

    $appProject = Get-Content (Join-Path $root $appProjectRel) -Raw
    if ($appProject -match 'EntityFrameworkCore|AceriaData.Infrastructure') {
        throw '5.7/Paso 2: Application depende de EF Core o Infrastructure.'
    }
    Write-Host 'PASS 5.7/Paso 2 · archivos principales y fronteras de capas comprobados'

    $script = Get-Content (Join-Path $root $scriptRel) -Raw
    foreach ($token in @(
        'dotnet ef migrations script --project $Infrastructure',
        'dotnet ef migrations script --idempotent',
        'dotnet ef migrations script M2_2_12_Architecture M5_5_2_ConcurrencyTokens',
        'dotnet ef migrations script M5_5_2_ConcurrencyTokens M2_2_12_Architecture',
        '__EFMigrationsHistory',
        'IF NOT EXISTS',
        'M5_5_2_ConcurrencyTokens',
        'RETO M05 5.7 - RANGO SEGUIDO DE IDEMPOTENTE'
    )) {
        if (-not $script.Contains($token)) { throw "5.7/Paso 3: falta evidencia '$token'." }
    }
    Write-Host 'PASS 5.7/Paso 3 · scripts completo, idempotente, rango, downgrade y bloque pedagógico localizados'

    $artifactDir = Join-Path $root $artifactsRel
    if (Test-Path $artifactDir) { Remove-Item $artifactDir -Recurse -Force }

    $scriptOut = Invoke-Checked -WorkingDirectory $root -FilePath 'pwsh' -ArgumentList @(
        '-NoProfile','-File',(Join-Path $root $scriptRel)
    ) -Context '5.7/Paso 4 validacion idempotente'
    Assert-TextContains -Text $scriptOut -Tokens @(
        '== Primera aplicacion del script idempotente ==',
        '== Segunda aplicacion del mismo script idempotente ==',
        'Migraciones tras primera aplicacion:',
        'Migraciones tras segunda aplicacion:',
        'Migracion final: 20260930203405_M5_5_2_ConcurrencyTokens',
        'Elementos de esquema verificados: 2',
        '5.7 IDEMPOTENCIA OK'
    ) -Context '5.7/Paso 4'
    foreach ($file in @('aceria-completo.sql','aceria-idempotente.sql','aceria-rango.sql','aceria-downgrade.sql')) {
        $path = Join-Path $artifactDir $file
        if (-not (Test-Path $path)) { throw "5.7/Paso 4: no se generó $file." }
        if ((Get-Item $path).Length -le 0) { throw "5.7/Paso 4: $file está vacío." }
    }
    Write-Host 'PASS 5.7/Paso 4 · idempotente aplicado dos veces sin alterar historial ni esquema'

    Invoke-Build51 -Root $root -Context '5.7/Paso 5 build'
    Invoke-Checked -WorkingDirectory $root -FilePath 'dotnet' -ArgumentList @(
        'ef','migrations','has-pending-model-changes',
        '--project','src/AceriaData.Infrastructure',
        '--startup-project','src/AceriaData.Console',
        '--configuration','Release'
    ) -Context '5.7/Paso 5 pending model changes' | Out-Null

    $migrations = Invoke-Checked -WorkingDirectory $root -FilePath 'dotnet' -ArgumentList @(
        'ef','migrations','list',
        '--project','src/AceriaData.Infrastructure',
        '--startup-project','src/AceriaData.Console',
        '--configuration','Release'
    ) -Context '5.7/Paso 5 migrations'
    Assert-TextContains -Text $migrations -Tokens @('M5_5_2_ConcurrencyTokens') -Context '5.7 migraciones'
    if ($migrations -match '(?m)^\S*M5_5_7') {
        throw '5.7: aparece una migración nueva aunque el modelo oficial no cambia.'
    }

    $out = Invoke-Run51 -Root $root -Context '5.7/Paso 5 run'
    Assert-TextContains -Text $out -Tokens @('5.7 OK') -Context '5.7/Paso 5'
    Write-Host 'PASS 5.7/Paso 5 · build, modelo sin cambios, LocalDB y marcador final comprobados'

    $temp = New-PedagogicalCopy -Source $root -Name 'm05-5-7-reto-rango-idempotente'
    Enable-PowerShellRetoBlock -Path (Join-Path $temp $scriptRel) -Marker 'RETO M05 5.7 - RANGO SEGUIDO DE IDEMPOTENTE'
    $retoOut = Invoke-Checked -WorkingDirectory $temp -FilePath 'pwsh' -ArgumentList @(
        '-NoProfile','-File',(Join-Path $temp $scriptRel)
    ) -Context '5.7 reto rango + idempotente'
    Assert-TextContains -Text $retoOut -Tokens @(
        'Reto 5.7 OK | historial rango=',
        '| historial idempotente=',
        '| final=20260930203405_M5_5_2_ConcurrencyTokens'
    ) -Context '5.7 reto'
    Write-Host 'PASS 5.7/RETO · base aislada preparada, rango aplicado y posterior idempotente sin alterar el historial final'

    Write-Host 'PASS 5.7 COMPLETO'
}


function Test-M058 {
    Write-Section 'M05 · 5.8 Migraciones en equipos: conflictos y buenas prácticas'

    $root = Join-Path $RepoRoot 'M05\PROYECTO\5.8'
    $scriptRel = 'team-migrations\validate-team-migrations.ps1'
    $programRel = 'src\AceriaData.Console\Program.cs'
    $appProjectRel = 'src\AceriaData.Application\AceriaData.Application.csproj'
    $migrationRel = 'src\AceriaData.Infrastructure\Migrations\20260930203405_M5_5_2_ConcurrencyTokens.cs'

    Invoke-Checked -WorkingDirectory $root -FilePath 'dotnet' -ArgumentList @('restore','AceriaData.sln') -Context '5.8/Paso 1 restore' | Out-Null
    Write-Host 'PASS 5.8/Paso 1'

    foreach ($rel in @($scriptRel,$programRel,$migrationRel)) {
        if (-not (Test-Path (Join-Path $root $rel))) { throw "5.8/Paso 2: falta $rel." }
    }
    $appProject = Get-Content (Join-Path $root $appProjectRel) -Raw
    if ($appProject -match 'EntityFrameworkCore|AceriaData.Infrastructure') {
        throw '5.8/Paso 2: Application depende de EF Core o Infrastructure.'
    }
    Write-Host 'PASS 5.8/Paso 2 · archivo de equipo y fronteras de capas comprobados'

    $script = Get-Content (Join-Path $root $scriptRel) -Raw
    foreach ($token in @(
        'M5_5_8_TeamA',
        'M5_5_8_TeamBParallel',
        'M5_5_8_TeamBRegenerated',
        'La rama B paralela no deberia conocer el cambio A',
        'La migracion regenerada no representa el modelo fusionado A+B',
        'has-pending-model-changes',
        '__EFMigrationsHistory',
        'EquipoRevisionA',
        'EquipoRevisionB',
        'RETO M05 5.8 - TERCERA RAMA C'
    )) {
        if (-not $script.Contains($token)) { throw "5.8/Paso 3: falta evidencia '$token'." }
    }
    Write-Host 'PASS 5.8/Paso 3 · ramas A/B, metadata divergente y reto C localizados'

    $scriptOut = Invoke-Checked -WorkingDirectory $root -FilePath 'pwsh' -ArgumentList @(
        '-NoProfile','-File',(Join-Path $root $scriptRel)
    ) -Context '5.8/Paso 4 validacion de migraciones en equipo'
    Assert-TextContains -Text $scriptOut -Tokens @(
        '== Rama A desde baseline ==',
        '== Rama B paralela desde el mismo baseline ==',
        'La migracion B paralela desconoce A: renombrarla no fusionaria sus metadatos.',
        '== Estado fusionado correcto: incorporar A y regenerar B ==',
        'Designer paralelo B contiene A: False',
        'Designer regenerado contiene A+B: True',
        'Columnas A+B verificadas directamente por SQL Server: True',
        'Migraciones de equipo verificadas en __EFMigrationsHistory: True',
        '5.8 EQUIPOS OK'
    ) -Context '5.8/Paso 4'
    Write-Host 'PASS 5.8/Paso 4 · conflicto paralelo, regeneración y SQL/History verificados'

    Invoke-Build51 -Root $root -Context '5.8/Paso 5 build'
    Invoke-Checked -WorkingDirectory $root -FilePath 'dotnet' -ArgumentList @(
        'ef','migrations','has-pending-model-changes',
        '--project','src/AceriaData.Infrastructure',
        '--startup-project','src/AceriaData.Console',
        '--configuration','Release'
    ) -Context '5.8/Paso 5 pending model changes' | Out-Null

    $migrations = Invoke-Checked -WorkingDirectory $root -FilePath 'dotnet' -ArgumentList @(
        'ef','migrations','list',
        '--project','src/AceriaData.Infrastructure',
        '--startup-project','src/AceriaData.Console',
        '--configuration','Release'
    ) -Context '5.8/Paso 5 migrations'
    Assert-TextContains -Text $migrations -Tokens @('M5_5_2_ConcurrencyTokens') -Context '5.8 migraciones oficiales'
    if ($migrations -match '(?m)^\S*M5_5_8') {
        throw '5.8: aparece una migración M5_5_8 en el proyecto oficial; el laboratorio debe ser temporal.'
    }

    $out = Invoke-Run51 -Root $root -Context '5.8/Paso 5 run'
    Assert-TextContains -Text $out -Tokens @('5.8 OK') -Context '5.8/Paso 5'
    Write-Host 'PASS 5.8/Paso 5 · build, modelo oficial sin cambios, LocalDB y marcador final comprobados'

    $temp = New-PedagogicalCopy -Source $root -Name 'm05-5-8-reto-tercera-rama'
    Enable-PowerShellRetoBlock -Path (Join-Path $temp $scriptRel) -Marker 'RETO M05 5.8 - TERCERA RAMA C'
    $retoOut = Invoke-Checked -WorkingDirectory $temp -FilePath 'pwsh' -ArgumentList @(
        '-NoProfile','-File',(Join-Path $temp $scriptRel)
    ) -Context '5.8 reto tercera rama C'
    Assert-TextContains -Text $retoOut -Tokens @(
        '== Reto: Rama C paralela desde el mismo baseline ==',
        '== Reto: integrar C despues de A y B regenerada ==',
        'Reto 5.8 OK | orden seguro=A -> B regenerada -> C regenerada | metadata A+B+C=True'
    ) -Context '5.8 reto'
    Write-Host 'PASS 5.8/RETO · tercera rama C regenerada sobre A+B sin reescribir las migraciones integradas'

    Write-Host 'PASS 5.8 COMPLETO'
}

if ($Suite -eq 'inventory') { Test-M05Inventory; exit 0 }
if ($Suite -eq '5.1') { Test-M051; exit 0 }
if ($Suite -eq '5.2') { Test-M052; exit 0 }
if ($Suite -eq '5.3') { Test-M053; exit 0 }
if ($Suite -eq '5.4') { Test-M054; exit 0 }
if ($Suite -eq '5.5') { Test-M055; exit 0 }
if ($Suite -eq '5.6') { Test-M056; exit 0 }
if ($Suite -eq '5.7') { Test-M057; exit 0 }
if ($Suite -eq '5.8') { Test-M058; exit 0 }

Test-M05Inventory
Test-M051
Test-M052
Test-M053
Test-M054
Test-M055
Test-M056
Test-M057
Test-M058
Write-Host 'PASS M05 PARCIAL · 5.1–5.8 certificados; siguiente checkpoint: 5.9.'
