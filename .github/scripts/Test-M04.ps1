param(
    [ValidateSet('all','inventory','4.1','4.2','4.3','4.4','4.5','4.6','4.7','4.8','4.9','4.10')]
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

function Invoke-Build41([string]$Root,[string]$Context) {
    Invoke-Checked -WorkingDirectory $Root -FilePath 'dotnet' -ArgumentList @('build','AceriaData.sln','--configuration','Release') -Context $Context | Out-Null
}

function Invoke-Run41([string]$Root,[string]$Context) {
    return Invoke-Checked -WorkingDirectory $Root -FilePath 'dotnet' -ArgumentList @('run','--project','src/AceriaData.Console/AceriaData.Console.csproj','--configuration','Release','--no-build') -Context $Context
}

function Test-M04Inventory {
    Write-Section 'M04 · inventario canónico'

    $manifestPath = Join-Path $RepoRoot 'M04\PRACTICA\M04_TRAZABILIDAD_E2E.json'
    $practicePath = Join-Path $RepoRoot 'M04\PRACTICA\M04_PRACTICA.md'

    $manifest = Get-Content $manifestPath -Raw | ConvertFrom-Json
    $practice = Get-Content $practicePath -Raw

    if ($manifest.counts.points -ne 12) { throw 'M04: el manifiesto debe contener 12 puntos.' }
    if ($manifest.counts.main_steps -ne 120) { throw 'M04: el manifiesto debe contener 120 pasos principales.' }
    if ($manifest.counts.retos_ampliacion -ne 12) { throw 'M04: deben existir 12 retos de ampliación.' }
    if ($manifest.counts.errores_comunes -ne 12) { throw 'M04: deben existir 12 bloques de errores comunes.' }
    if ($manifest.counts.trace_units -ne 144) { throw 'M04: el inventario completo debe contener 144 unidades trazables.' }

    for ($n = 1; $n -le 12; $n++) {
        $point = "4.$n"
        $pattern = '(?ms)^## Punto ' + [regex]::Escape($point) + '\b.*?(?=^## Punto 4\.\d+\b|\z)'
        $section = [regex]::Match($practice,$pattern).Value

        if ([string]::IsNullOrWhiteSpace($section)) {
            throw "M04: no se localiza $point en la práctica."
        }

        $steps = [regex]::Matches($section,'(?m)^### Paso (\d+):')
        if ($steps.Count -ne 10) {
            throw "M04: $point debe contener exactamente 10 pasos y contiene $($steps.Count)."
        }

        for ($step = 1; $step -le 10; $step++) {
            if ($section -notmatch ('(?m)^### Paso ' + $step + ':')) {
                throw "M04: falta $point/Paso $step."
            }
        }

        if ($section -notmatch '(?m)^### Reto de ampliación\s*$') {
            throw "M04: $point no contiene Reto de ampliación."
        }
        if ($section -notmatch '(?m)^### Errores comunes\s*$') {
            throw "M04: $point no contiene Errores comunes."
        }

        Write-Host "PASS inventario $point · 10 pasos + reto + errores comunes"
    }

    Write-Host 'PASS inventario M04: 120 pasos + 12 retos + 12 bloques de errores comunes.'
}

function Test-M041 {
    Write-Section 'M04 · 4.1 Análisis del SQL generado: ToQueryString y logging'

    $root = Join-Path $RepoRoot 'M04\PROYECTO\4.1'
    $repoRel = 'src\AceriaData.Infrastructure\Repositories\Rendimiento41.cs'
    $depRel = 'src\AceriaData.Infrastructure\DependencyInjection.cs'
    $useCaseRel = 'src\AceriaData.Application\AnalisisSqlUseCase.cs'
    $programRel = 'src\AceriaData.Console\Program.cs'
    $interfacesRel = 'src\AceriaData.Application\Interfaces.cs'

    Invoke-Checked -WorkingDirectory $root -FilePath 'dotnet' -ArgumentList @('restore','AceriaData.sln') -Context '4.1/Paso 1 restore' | Out-Null
    Write-Host 'PASS 4.1/Paso 1'

    $migrations = Invoke-Checked -WorkingDirectory $root -FilePath 'dotnet' -ArgumentList @('ef','migrations','list','--project','src/AceriaData.Infrastructure','--startup-project','src/AceriaData.Console','--configuration','Release') -Context '4.1/Paso 2 migrations list'
    Assert-TextContains -Text $migrations -Tokens @('M2_2_12_Architecture') -Context '4.1/Paso 2'
    if ($migrations -match '(?m)^\S*M4_') {
        throw '4.1/Paso 2: aparecen migraciones M4 y la práctica indica que M4 no crea migraciones vacías.'
    }
    Write-Host 'PASS 4.1/Paso 2'

    $interfaces = Get-Content (Join-Path $root $interfacesRel) -Raw
    $repo = Get-Content (Join-Path $root $repoRel) -Raw
    foreach ($token in @('ObtenerSqlConIncludeM4','ObtenerSqlConProyeccionM4','ObtenerSqlPendientesOrdenadasM4')) {
        if (-not $interfaces.Contains($token)) { throw "4.1/Paso 3: falta en el puerto $token." }
        if (-not $repo.Contains($token)) { throw "4.1/Paso 3: falta en Infrastructure $token." }
    }
    foreach ($token in @('.AsNoTracking()','.Where(o => o.Estado == "Pendiente")','.Include(o => o.Planchas)','.Select(o => new { o.NumeroOrden, o.Cliente })','.ToQueryString()')) {
        if (-not $repo.Contains($token)) { throw "4.1/Paso 3: falta la composición esperada '$token'." }
    }
    Write-Host 'PASS 4.1/Paso 3'

    $temp4 = New-PedagogicalCopy -Source $root -Name 'm04-4-1-paso4'
    Enable-LineCommentWholeFileCopy -Path (Join-Path $temp4 $repoRel) -Marker 'FRAGMENTO PDF M04 4.1 - PASO 4 - RENDIMIENTO41'
    Enable-LineCommentWholeFileCopy -Path (Join-Path $temp4 $depRel) -Marker 'FRAGMENTO PDF M04 4.1 - PASO 4 - DEPENDENCYINJECTION'
    Invoke-Build41 -Root $temp4 -Context '4.1/Paso 4 build variante PDF Infrastructure'
    Write-Host 'PASS 4.1/Paso 4 · Infrastructure del PDF activada y compilada'

    $temp5 = New-PedagogicalCopy -Source $root -Name 'm04-4-1-paso5'
    Enable-LineCommentWholeFileCopy -Path (Join-Path $temp5 $useCaseRel) -Marker 'FRAGMENTO PDF M04 4.1 - PASO 5'
    Invoke-Build41 -Root $temp5 -Context '4.1/Paso 5 build caso de uso PDF'
    $out5 = Invoke-Run41 -Root $temp5 -Context '4.1/Paso 5 run caso de uso PDF'
    Assert-TextContains -Text $out5 -Tokens @('=== 4.1 ANALISIS DEL SQL GENERADO ===','--- SQL pendientes ---','--- SQL Include ---','--- SQL proyeccion ---','4.1 OK') -Context '4.1/Paso 5'
    Write-Host 'PASS 4.1/Paso 5 · caso de uso del PDF activado, compilado y ejecutado'

    $temp6 = New-PedagogicalCopy -Source $root -Name 'm04-4-1-paso6'
    Enable-LineCommentWholeFileCopy -Path (Join-Path $temp6 $programRel) -Marker 'FRAGMENTO PDF M04 4.1 - PASO 6'
    Invoke-Build41 -Root $temp6 -Context '4.1/Paso 6 build composition root PDF'
    $out6 = Invoke-Run41 -Root $temp6 -Context '4.1/Paso 6 run composition root PDF'
    Assert-TextContains -Text $out6 -Tokens @('=== 4.1 ANALISIS DEL SQL GENERADO ===','4.1 OK') -Context '4.1/Paso 6'
    Write-Host 'PASS 4.1/Paso 6 · composition root del PDF activado'

    Invoke-Build41 -Root $root -Context '4.1/Paso 7 build final'
    $appProject = Get-Content (Join-Path $root 'src\AceriaData.Application\AceriaData.Application.csproj') -Raw
    if ($appProject -match 'EntityFrameworkCore') {
        throw '4.1/Paso 7: Application referencia EntityFrameworkCore.'
    }
    Write-Host 'PASS 4.1/Paso 7'

    $out8 = Invoke-Run41 -Root $root -Context '4.1/Paso 8 run LocalDB'
    Assert-TextContains -Text $out8 -Tokens @('=== 4.1 ANALISIS DEL SQL GENERADO ===','Executed DbCommand','--- SQL pendientes ---','--- SQL Include ---','--- SQL proyeccion ---','4.1 OK') -Context '4.1/Paso 8'

    $p0 = $out8.IndexOf('--- SQL pendientes ---')
    $p1 = $out8.IndexOf('--- SQL Include ---',$p0)
    $p2 = $out8.IndexOf('--- SQL proyeccion ---',$p1)
    $p3 = $out8.IndexOf('4.1 OK',$p2)
    if ($p0 -lt 0 -or $p1 -lt 0 -or $p2 -lt 0 -or $p3 -lt 0) {
        throw '4.1/Paso 8: no se pueden aislar los tres SQL.'
    }
    $pendSql = $out8.Substring($p0,$p1-$p0)
    $includeSql = $out8.Substring($p1,$p2-$p1)
    $projSql = $out8.Substring($p2,$p3-$p2)

    Assert-TextContains -Text $pendSql -Tokens @('SELECT','WHERE','ORDER BY','Estado') -Context '4.1/Paso 8 SQL pendientes'
    Assert-TextContains -Text $includeSql -Tokens @('SELECT','JOIN','PlanchasAcero','Cliente') -Context '4.1/Paso 8 SQL Include'
    Assert-TextContains -Text $projSql -Tokens @('SELECT','NumeroOrden','Cliente','WHERE','ORDER BY') -Context '4.1/Paso 8 SQL proyección'
    if ($projSql -match 'Observaciones') {
        throw '4.1/Paso 8: la proyección contiene Observaciones y deja de ser mínima.'
    }
    Write-Host 'PASS 4.1/Paso 8 · LocalDB + logging + SQL real comprobados'

    if ($includeSql -notmatch 'Observaciones') {
        throw '4.1/Paso 9: el SQL de entidad/grafo no permite observar columnas sobrantes frente a la proyección.'
    }
    if ($projSql -match 'Observaciones') {
        throw '4.1/Paso 9: la proyección conserva una columna que debía desaparecer.'
    }

    $temp9 = New-PedagogicalCopy -Source $root -Name 'm04-4-1-error-materializacion'
    Enable-RetoBlock -Path (Join-Path $temp9 $repoRel) -Marker 'ERROR CONTROLADO M04 4.1 - MATERIALIZAR ANTES DE TOQUERYSTRING'
    Invoke-ExpectedFailure -WorkingDirectory $temp9 -FilePath 'dotnet' -ArgumentList @('build','AceriaData.sln','--configuration','Release') -Context '4.1/Paso 9 materialización antes de ToQueryString' -ExpectedTokens @('ToQueryString') | Out-Null
    Write-Host 'PASS 4.1/Paso 9 · comparación de columnas + error controlado de materialización'

    $program = Get-Content (Join-Path $root $programRel) -Raw
    if ($program -match 'EnsureCreated') {
        throw '4.1/Paso 10: aparece EnsureCreated.'
    }
    if ($program -notmatch 'Database\.Migrate\(\)') {
        throw '4.1/Paso 10: falta Database.Migrate().'
    }

    $temp10 = New-PedagogicalCopy -Source $root -Name 'm04-4-1-reto'
    Enable-RetoBlock -Path (Join-Path $temp10 $interfacesRel) -Marker 'RETO M04 4.1 - PUERTO SQL DOS COLECCIONES'
    Enable-RetoBlock -Path (Join-Path $temp10 $repoRel) -Marker 'RETO M04 4.1 - DOS COLECCIONES Y SQL SIN MATERIALIZAR'
    Enable-RetoBlock -Path (Join-Path $temp10 $useCaseRel) -Marker 'RETO M04 4.1 - ANTICIPAR DOS COLECCIONES SIN CONCLUIR RENDIMIENTO'
    Enable-RetoBlock -Path (Join-Path $temp10 $programRel) -Marker 'RETO M04 4.1 - EJECUTAR DOS COLECCIONES SIN MEDIR AUN RENDIMIENTO'

    Invoke-Build41 -Root $temp10 -Context '4.1/Paso 10 reto build'
    $out10 = Invoke-Run41 -Root $temp10 -Context '4.1/Paso 10 reto run'
    Assert-TextContains -Text $out10 -Tokens @('Reto 4.1 OK | JOINs:','PlanchasAcero','OrdenesAleaciones','4.1 OK') -Context '4.1/Paso 10'
    Write-Host 'PASS 4.1/Paso 10 · reto de dos colecciones anticipado sin convertirlo aún en conclusión de rendimiento'

    Write-Host 'PASS 4.1 COMPLETO'
}


function Test-M042 {
    Write-Section 'M04 · 4.2 Tracking y No Tracking'

    $root = Join-Path $RepoRoot 'M04\PROYECTO\4.2'
    $repoRel = 'src\AceriaData.Infrastructure\Repositories\Rendimiento42.cs'
    $useCaseRel = 'src\AceriaData.Application\TrackingUseCase.cs'
    $programRel = 'src\AceriaData.Console\Program.cs'
    $interfacesRel = 'src\AceriaData.Application\Interfaces.cs'

    Invoke-Checked -WorkingDirectory $root -FilePath 'dotnet' -ArgumentList @('restore','AceriaData.sln') -Context '4.2/Paso 1 restore' | Out-Null
    Write-Host 'PASS 4.2/Paso 1'

    $migrations = Invoke-Checked -WorkingDirectory $root -FilePath 'dotnet' -ArgumentList @('ef','migrations','list','--project','src/AceriaData.Infrastructure','--startup-project','src/AceriaData.Console','--configuration','Release') -Context '4.2/Paso 2 migrations list'
    Assert-TextContains -Text $migrations -Tokens @('M2_2_12_Architecture') -Context '4.2/Paso 2'
    if ($migrations -match '(?m)^\S*M4_') {
        throw '4.2/Paso 2: aparecen migraciones M4 y el módulo no cambia el esquema en este punto.'
    }
    Write-Host 'PASS 4.2/Paso 2'

    $interfaces = Get-Content (Join-Path $root $interfacesRel) -Raw
    $repo = Get-Content (Join-Path $root $repoRel) -Raw
    foreach ($token in @('MedirConsultaConTrackingM4','MedirConsultaSinTrackingM4')) {
        if (-not $interfaces.Contains($token)) { throw "4.2/Paso 3: falta en el puerto $token." }
        if (-not $repo.Contains($token)) { throw "4.2/Paso 3: falta en Infrastructure $token." }
    }
    foreach ($token in @('_context.ChangeTracker.Clear()','.AsTracking()','.AsNoTracking()','.ToQueryString()','_context.ChangeTracker.Entries().Count()')) {
        if (-not $repo.Contains($token)) { throw "4.2/Paso 3: falta la evidencia '$token'." }
    }
    Write-Host 'PASS 4.2/Paso 3'

    $temp4 = New-PedagogicalCopy -Source $root -Name 'm04-4-2-paso4'
    Enable-LineCommentWholeFileCopy -Path (Join-Path $temp4 $repoRel) -Marker 'FRAGMENTO PDF M04 4.2 - PASO 4'
    Invoke-Build41 -Root $temp4 -Context '4.2/Paso 4 build variante PDF Infrastructure'
    Write-Host 'PASS 4.2/Paso 4 · Infrastructure del PDF activada y compilada'

    $temp5 = New-PedagogicalCopy -Source $root -Name 'm04-4-2-paso5'
    Enable-LineCommentWholeFileCopy -Path (Join-Path $temp5 $useCaseRel) -Marker 'FRAGMENTO PDF M04 4.2 - PASO 5'
    Invoke-Build41 -Root $temp5 -Context '4.2/Paso 5 build caso de uso PDF'
    $out5 = Invoke-Run41 -Root $temp5 -Context '4.2/Paso 5 run caso de uso PDF'
    Assert-TextContains -Text $out5 -Tokens @('=== 4.2 TRACKING Y NO TRACKING ===','Con tracking: filas=','Sin tracking: filas=','4.2 OK') -Context '4.2/Paso 5'
    Write-Host 'PASS 4.2/Paso 5 · caso de uso del PDF activado, compilado y ejecutado'

    $temp6 = New-PedagogicalCopy -Source $root -Name 'm04-4-2-paso6'
    Enable-LineCommentWholeFileCopy -Path (Join-Path $temp6 $programRel) -Marker 'FRAGMENTO PDF M04 4.2 - PASO 6'
    Invoke-Build41 -Root $temp6 -Context '4.2/Paso 6 build composition root PDF'
    $out6 = Invoke-Run41 -Root $temp6 -Context '4.2/Paso 6 run composition root PDF'
    Assert-TextContains -Text $out6 -Tokens @('=== 4.2 TRACKING Y NO TRACKING ===','4.2 OK') -Context '4.2/Paso 6'
    Write-Host 'PASS 4.2/Paso 6 · composition root del PDF activado'

    Invoke-Build41 -Root $root -Context '4.2/Paso 7 build final'
    $appProject = Get-Content (Join-Path $root 'src\AceriaData.Application\AceriaData.Application.csproj') -Raw
    if ($appProject -match 'EntityFrameworkCore') {
        throw '4.2/Paso 7: Application referencia EntityFrameworkCore.'
    }
    Write-Host 'PASS 4.2/Paso 7'

    $out8 = Invoke-Run41 -Root $root -Context '4.2/Paso 8 run LocalDB'
    $con = [regex]::Match($out8,'Con tracking: filas=(\d+), rastreadas=(\d+)')
    $sin = [regex]::Match($out8,'Sin tracking: filas=(\d+), rastreadas=(\d+)')
    if (-not $con.Success -or -not $sin.Success) {
        throw '4.2/Paso 8: no se puede leer la métrica de tracking.'
    }
    $conFilas = [int]$con.Groups[1].Value
    $conTracked = [int]$con.Groups[2].Value
    $sinFilas = [int]$sin.Groups[1].Value
    $sinTracked = [int]$sin.Groups[2].Value
    if ($conFilas -le 0 -or $conTracked -ne $conFilas) {
        throw "4.2/Paso 8: tracking inesperado filas=$conFilas rastreadas=$conTracked."
    }
    if ($sinFilas -ne $conFilas -or $sinTracked -ne 0) {
        throw "4.2/Paso 8: NoTracking inesperado filas=$sinFilas rastreadas=$sinTracked."
    }
    Assert-TextContains -Text $out8 -Tokens @('El SQL puede ser equivalente','4.2 OK') -Context '4.2/Paso 8'
    Write-Host 'PASS 4.2/Paso 8 · tracking y NoTracking medidos sobre datos reales'

    $temp9 = New-PedagogicalCopy -Source $root -Name 'm04-4-2-error-tracker-previo'
    Enable-RetoBlock -Path (Join-Path $temp9 $interfacesRel) -Marker 'ERROR CONTROLADO M04 4.2 - PUERTO SIN LIMPIAR TRACKER'
    Enable-RetoBlock -Path (Join-Path $temp9 $repoRel) -Marker 'ERROR CONTROLADO M04 4.2 - NOTRACKING SIN LIMPIAR ESTADO PREVIO'
    Enable-RetoBlock -Path (Join-Path $temp9 $useCaseRel) -Marker 'ERROR CONTROLADO M04 4.2 - ESTADO PREVIO DEL CHANGETRACKER'
    Enable-RetoBlock -Path (Join-Path $temp9 $programRel) -Marker 'ERROR CONTROLADO M04 4.2 - EJECUTAR ESTADO PREVIO'
    Invoke-Build41 -Root $temp9 -Context '4.2/Paso 9 build error controlado'
    $out9 = Invoke-Run41 -Root $temp9 -Context '4.2/Paso 9 run error controlado'
    Assert-TextContains -Text $out9 -Tokens @('Error controlado 4.2 OK | SQL equivalente: True | rastreadas heredadas=','4.2 OK') -Context '4.2/Paso 9'
    Write-Host 'PASS 4.2/Paso 9 · SQL equivalente y contaminación del ChangeTracker demostrados'

    $program = Get-Content (Join-Path $root $programRel) -Raw
    if ($program -match 'EnsureCreated') {
        throw '4.2/Paso 10: aparece EnsureCreated.'
    }
    if ($program -notmatch 'Database\.Migrate\(\)') {
        throw '4.2/Paso 10: falta Database.Migrate().'
    }

    $temp10 = New-PedagogicalCopy -Source $root -Name 'm04-4-2-reto-grafo'
    Enable-RetoBlock -Path (Join-Path $temp10 $interfacesRel) -Marker 'RETO M04 4.2 - PUERTO GRAFO TRACKING'
    Enable-RetoBlock -Path (Join-Path $temp10 $repoRel) -Marker 'RETO M04 4.2 - GRAFO CON Y SIN TRACKING'
    Enable-RetoBlock -Path (Join-Path $temp10 $useCaseRel) -Marker 'RETO M04 4.2 - GRAFO DE ENTIDADES RELACIONADAS'
    Enable-RetoBlock -Path (Join-Path $temp10 $programRel) -Marker 'RETO M04 4.2 - EJECUTAR GRAFO'
    Invoke-Build41 -Root $temp10 -Context '4.2/Paso 10 reto build'
    $out10 = Invoke-Run41 -Root $temp10 -Context '4.2/Paso 10 reto run'
    Assert-TextContains -Text $out10 -Tokens @('Reto 4.2 OK | filas=','| grafo rastreado=','| sin tracking=0','4.2 OK') -Context '4.2/Paso 10'
    Write-Host 'PASS 4.2/Paso 10 · grafo relacionado comparado con y sin tracking'

    Write-Host 'PASS 4.2 COMPLETO'
}


function Test-M043 {
    Write-Section 'M04 · 4.3 AsNoTracking y AsNoTrackingWithIdentityResolution'

    $root = Join-Path $RepoRoot 'M04\PROYECTO\4.3'
    $repoRel = 'src\AceriaData.Infrastructure\Repositories\Rendimiento43.cs'
    $useCaseRel = 'src\AceriaData.Application\IdentityResolutionUseCase.cs'
    $programRel = 'src\AceriaData.Console\Program.cs'
    $interfacesRel = 'src\AceriaData.Application\Interfaces.cs'

    Invoke-Checked -WorkingDirectory $root -FilePath 'dotnet' -ArgumentList @('restore','AceriaData.sln') -Context '4.3/Paso 1 restore' | Out-Null
    Write-Host 'PASS 4.3/Paso 1'

    $migrations = Invoke-Checked -WorkingDirectory $root -FilePath 'dotnet' -ArgumentList @('ef','migrations','list','--project','src/AceriaData.Infrastructure','--startup-project','src/AceriaData.Console','--configuration','Release') -Context '4.3/Paso 2 migrations list'
    Assert-TextContains -Text $migrations -Tokens @('M2_2_12_Architecture') -Context '4.3/Paso 2'
    if ($migrations -match '(?m)^\S*M4_') {
        throw '4.3/Paso 2: aparecen migraciones M4 y el punto no cambia el esquema.'
    }
    Write-Host 'PASS 4.3/Paso 2'

    $interfaces = Get-Content (Join-Path $root $interfacesRel) -Raw
    $repo = Get-Content (Join-Path $root $repoRel) -Raw
    foreach ($token in @('MedirNoTrackingSinResolucionM4','MedirNoTrackingConResolucionM4')) {
        if (-not $interfaces.Contains($token)) { throw "4.3/Paso 3: falta en el puerto $token." }
        if (-not $repo.Contains($token)) { throw "4.3/Paso 3: falta en Infrastructure $token." }
    }
    foreach ($token in @('_context.OrdenesAleaciones','.AsNoTracking()','.AsNoTrackingWithIdentityResolution()','.Select(oa => oa.Aleacion)','ReferenceEqualityComparer.Instance')) {
        if (-not $repo.Contains($token)) { throw "4.3/Paso 3: falta la evidencia '$token'." }
    }
    Write-Host 'PASS 4.3/Paso 3'

    $temp4 = New-PedagogicalCopy -Source $root -Name 'm04-4-3-paso4'
    Enable-LineCommentWholeFileCopy -Path (Join-Path $temp4 $repoRel) -Marker 'FRAGMENTO PDF M04 4.3 - PASO 4'
    Invoke-Build41 -Root $temp4 -Context '4.3/Paso 4 build variante PDF Infrastructure'
    Write-Host 'PASS 4.3/Paso 4 · Infrastructure del PDF activada y compilada'

    $temp5 = New-PedagogicalCopy -Source $root -Name 'm04-4-3-paso5'
    Enable-LineCommentWholeFileCopy -Path (Join-Path $temp5 $useCaseRel) -Marker 'FRAGMENTO PDF M04 4.3 - PASO 5'
    Invoke-Build41 -Root $temp5 -Context '4.3/Paso 5 build caso de uso PDF'
    $out5 = Invoke-Run41 -Root $temp5 -Context '4.3/Paso 5 run caso de uso PDF'
    Assert-TextContains -Text $out5 -Tokens @('=== 4.3 NO TRACKING E IDENTITY RESOLUTION ===','AsNoTracking: filas=4, claves=2, instancias=4','IdentityResolution: filas=4, claves=2, instancias=2','4.3 OK') -Context '4.3/Paso 5'
    Write-Host 'PASS 4.3/Paso 5 · caso de uso del PDF activado, compilado y ejecutado'

    $temp6 = New-PedagogicalCopy -Source $root -Name 'm04-4-3-paso6'
    Enable-LineCommentWholeFileCopy -Path (Join-Path $temp6 $programRel) -Marker 'FRAGMENTO PDF M04 4.3 - PASO 6'
    Invoke-Build41 -Root $temp6 -Context '4.3/Paso 6 build composition root PDF'
    $out6 = Invoke-Run41 -Root $temp6 -Context '4.3/Paso 6 run composition root PDF'
    Assert-TextContains -Text $out6 -Tokens @('AsNoTracking: filas=4, claves=2, instancias=4','IdentityResolution: filas=4, claves=2, instancias=2','4.3 OK') -Context '4.3/Paso 6'
    Write-Host 'PASS 4.3/Paso 6 · composition root del PDF activado'

    Invoke-Build41 -Root $root -Context '4.3/Paso 7 build final'
    $appProject = Get-Content (Join-Path $root 'src\AceriaData.Application\AceriaData.Application.csproj') -Raw
    if ($appProject -match 'EntityFrameworkCore') {
        throw '4.3/Paso 7: Application referencia EntityFrameworkCore.'
    }
    Write-Host 'PASS 4.3/Paso 7'

    $out8 = Invoke-Run41 -Root $root -Context '4.3/Paso 8 run LocalDB'
    Assert-TextContains -Text $out8 -Tokens @(
        '=== 4.3 NO TRACKING E IDENTITY RESOLUTION ===',
        'AsNoTracking: filas=4, claves=2, instancias=4',
        'IdentityResolution: filas=4, claves=2, instancias=2',
        '4.3 OK'
    ) -Context '4.3/Paso 8'
    Write-Host 'PASS 4.3/Paso 8 · 4 relaciones, 2 claves y referencias reales comprobadas'

    $temp9 = New-PedagogicalCopy -Source $root -Name 'm04-4-3-error-entidad-no-repetida'
    Enable-RetoBlock -Path (Join-Path $temp9 $interfacesRel) -Marker 'ERROR CONTROLADO M04 4.3 - PUERTO PLANCHA SIN REPETICION'
    Enable-RetoBlock -Path (Join-Path $temp9 $repoRel) -Marker 'ERROR CONTROLADO M04 4.3 - ENTIDAD SIN CLAVES REPETIDAS'
    Enable-RetoBlock -Path (Join-Path $temp9 $useCaseRel) -Marker 'ERROR CONTROLADO M04 4.3 - PLANCHA NO DEMUESTRA IDENTITY RESOLUTION'
    Enable-RetoBlock -Path (Join-Path $temp9 $programRel) -Marker 'ERROR CONTROLADO M04 4.3 - EJECUTAR ENTIDAD NO REPETIDA'
    Invoke-Build41 -Root $temp9 -Context '4.3/Paso 9 build contraejemplo'
    $out9 = Invoke-Run41 -Root $temp9 -Context '4.3/Paso 9 run contraejemplo'
    Assert-TextContains -Text $out9 -Tokens @('Error controlado 4.3 OK | Plancha no demuestra resolución |','4.3 OK') -Context '4.3/Paso 9'
    Write-Host 'PASS 4.3/Paso 9 · PlanchaAcero descartada como demostración inválida de Identity Resolution'

    $program = Get-Content (Join-Path $root $programRel) -Raw
    if ($program -match 'EnsureCreated') {
        throw '4.3/Paso 10: aparece EnsureCreated.'
    }
    if ($program -notmatch 'Database\.Migrate\(\)') {
        throw '4.3/Paso 10: falta Database.Migrate().'
    }

    $temp10 = New-PedagogicalCopy -Source $root -Name 'm04-4-3-reto-referencias'
    Enable-RetoBlock -Path (Join-Path $temp10 $useCaseRel) -Marker 'RETO M04 4.3 - COMPARAR REFERENCIAS DE ALEACION COMPARTIDA'
    Enable-RetoBlock -Path (Join-Path $temp10 $programRel) -Marker 'RETO M04 4.3 - EJECUTAR COMPARACION DE REFERENCIAS'
    Invoke-Build41 -Root $temp10 -Context '4.3/Paso 10 reto build'
    $out10 = Invoke-Run41 -Root $temp10 -Context '4.3/Paso 10 reto run'
    Assert-TextContains -Text $out10 -Tokens @('Reto 4.3 OK | 4 relaciones / 2 aleaciones | sin=4 referencias | con=2 referencias','4.3 OK') -Context '4.3/Paso 10'
    Write-Host 'PASS 4.3/Paso 10 · aleación compartida comparada por referencia'

    Write-Host 'PASS 4.3 COMPLETO'
}


function Test-M044 {
    Write-Section 'M04 · 4.4 Problema N+1: identificación y causas'

    $root = Join-Path $RepoRoot 'M04\PROYECTO\4.4'
    $repoRel = 'src\AceriaData.Infrastructure\Repositories\Rendimiento44.cs'
    $interceptorRel = 'src\AceriaData.Infrastructure\SqlCommandCounterInterceptor.cs'
    $depRel = 'src\AceriaData.Infrastructure\DependencyInjection.cs'
    $useCaseRel = 'src\AceriaData.Application\NMasUnoUseCase.cs'
    $programRel = 'src\AceriaData.Console\Program.cs'
    $interfacesRel = 'src\AceriaData.Application\Interfaces.cs'

    Invoke-Checked -WorkingDirectory $root -FilePath 'dotnet' -ArgumentList @('restore','AceriaData.sln') -Context '4.4/Paso 1 restore' | Out-Null
    Write-Host 'PASS 4.4/Paso 1'

    $migrations = Invoke-Checked -WorkingDirectory $root -FilePath 'dotnet' -ArgumentList @('ef','migrations','list','--project','src/AceriaData.Infrastructure','--startup-project','src/AceriaData.Console','--configuration','Release') -Context '4.4/Paso 2 migrations list'
    Assert-TextContains -Text $migrations -Tokens @('M2_2_12_Architecture') -Context '4.4/Paso 2'
    if ($migrations -match '(?m)^\S*M4_') {
        throw '4.4/Paso 2: aparecen migraciones M4 y el punto no cambia el esquema.'
    }
    Write-Host 'PASS 4.4/Paso 2'

    $interfaces = Get-Content (Join-Path $root $interfacesRel) -Raw
    $repo = Get-Content (Join-Path $root $repoRel) -Raw
    $interceptor = Get-Content (Join-Path $root $interceptorRel) -Raw
    $dependency = Get-Content (Join-Path $root $depRel) -Raw

    foreach ($token in @('EjecutarNMasUnoM4')) {
        if (-not $interfaces.Contains($token)) { throw "4.4/Paso 3: falta en el puerto $token." }
        if (-not $repo.Contains($token)) { throw "4.4/Paso 3: falta en Infrastructure $token." }
    }
    foreach ($token in @('SqlCommandCounterInterceptor.Instance.Reset()','.Select(o => new { o.Id, o.NumeroOrden })','foreach (var orden in ordenes)','.Count(p => p.OrdenId == orden.Id)','SqlCommandCounterInterceptor.Instance.Count')) {
        if (-not $repo.Contains($token)) { throw "4.4/Paso 3: falta la evidencia N+1 '$token'." }
    }
    foreach ($token in @('ReaderExecuting','ScalarExecuting','NonQueryExecuting','Interlocked.Increment')) {
        if (-not $interceptor.Contains($token)) { throw "4.4/Paso 3: interceptor incompleto; falta '$token'." }
    }
    if (-not $dependency.Contains('.AddInterceptors(SqlCommandCounterInterceptor.Instance)')) {
        throw '4.4/Paso 3: el interceptor no está registrado en el DbContext.'
    }
    Write-Host 'PASS 4.4/Paso 3'

    $temp4 = New-PedagogicalCopy -Source $root -Name 'm04-4-4-paso4'
    Enable-LineCommentWholeFileCopy -Path (Join-Path $temp4 $repoRel) -Marker 'FRAGMENTO PDF M04 4.4 - PASO 4 - RENDIMIENTO44'
    Enable-LineCommentWholeFileCopy -Path (Join-Path $temp4 $interceptorRel) -Marker 'FRAGMENTO PDF M04 4.4 - PASO 4 - INTERCEPTOR'
    Invoke-Build41 -Root $temp4 -Context '4.4/Paso 4 build variante PDF Infrastructure'
    Write-Host 'PASS 4.4/Paso 4 · repositorio e interceptor del PDF activados y compilados'

    $temp5 = New-PedagogicalCopy -Source $root -Name 'm04-4-4-paso5'
    Enable-LineCommentWholeFileCopy -Path (Join-Path $temp5 $useCaseRel) -Marker 'FRAGMENTO PDF M04 4.4 - PASO 5'
    Invoke-Build41 -Root $temp5 -Context '4.4/Paso 5 build caso de uso PDF'
    $out5 = Invoke-Run41 -Root $temp5 -Context '4.4/Paso 5 run caso de uso PDF'
    Assert-TextContains -Text $out5 -Tokens @('=== 4.4 PROBLEMA N+1 ===','Ordenes=5 | Planchas=5 | Consultas SQL=6','Lazy Loading permanece desactivado.','4.4 OK') -Context '4.4/Paso 5'
    Write-Host 'PASS 4.4/Paso 5 · caso de uso del PDF activado y N+1 observado'

    $temp6 = New-PedagogicalCopy -Source $root -Name 'm04-4-4-paso6'
    Enable-LineCommentWholeFileCopy -Path (Join-Path $temp6 $programRel) -Marker 'FRAGMENTO PDF M04 4.4 - PASO 6'
    Invoke-Build41 -Root $temp6 -Context '4.4/Paso 6 build composition root PDF'
    $out6 = Invoke-Run41 -Root $temp6 -Context '4.4/Paso 6 run composition root PDF'
    Assert-TextContains -Text $out6 -Tokens @('Ordenes=5 | Planchas=5 | Consultas SQL=6','4.4 OK') -Context '4.4/Paso 6'
    Write-Host 'PASS 4.4/Paso 6 · composition root del PDF activado'

    Invoke-Build41 -Root $root -Context '4.4/Paso 7 build final'
    $appProject = Get-Content (Join-Path $root 'src\AceriaData.Application\AceriaData.Application.csproj') -Raw
    if ($appProject -match 'EntityFrameworkCore') {
        throw '4.4/Paso 7: Application referencia EntityFrameworkCore.'
    }
    Write-Host 'PASS 4.4/Paso 7'

    $out8 = Invoke-Run41 -Root $root -Context '4.4/Paso 8 run LocalDB'
    Assert-TextContains -Text $out8 -Tokens @(
        '=== 4.4 PROBLEMA N+1 ===',
        'Ordenes=5 | Planchas=5 | Consultas SQL=6',
        'La demostracion genera N+1 de forma explicita; Lazy Loading permanece desactivado.',
        '4.4 OK'
    ) -Context '4.4/Paso 8'
    Write-Host 'PASS 4.4/Paso 8 · LocalDB confirma N=5 y N+1=6 comandos'

    $temp9 = New-PedagogicalCopy -Source $root -Name 'm04-4-4-error-contador'
    Enable-RetoBlock -Path (Join-Path $temp9 $interfacesRel) -Marker 'ERROR CONTROLADO M04 4.4 - PUERTO CONTADOR SIN RESET'
    Enable-RetoBlock -Path (Join-Path $temp9 $repoRel) -Marker 'ERROR CONTROLADO M04 4.4 - CONTADOR SIN RESET'
    Enable-RetoBlock -Path (Join-Path $temp9 $useCaseRel) -Marker 'ERROR CONTROLADO M04 4.4 - MEDICION CONTAMINADA POR ESTADO PREVIO'
    Enable-RetoBlock -Path (Join-Path $temp9 $programRel) -Marker 'ERROR CONTROLADO M04 4.4 - EJECUTAR CONTADOR SIN RESET'
    Invoke-Build41 -Root $temp9 -Context '4.4/Paso 9 build error controlado'
    $out9 = Invoke-Run41 -Root $temp9 -Context '4.4/Paso 9 run error controlado'
    Assert-TextContains -Text $out9 -Tokens @('Error controlado 4.4 OK | limpio=6 | sin reset=12','4.4 OK') -Context '4.4/Paso 9'
    Write-Host 'PASS 4.4/Paso 9 · cálculo N+1 y contaminación por no resetear el contador demostrados'

    $program = Get-Content (Join-Path $root $programRel) -Raw
    if ($program -match 'EnsureCreated') {
        throw '4.4/Paso 10: aparece EnsureCreated.'
    }
    if ($program -notmatch 'Database\.Migrate\(\)') {
        throw '4.4/Paso 10: falta Database.Migrate().'
    }

    $temp10 = New-PedagogicalCopy -Source $root -Name 'm04-4-4-reto-detalle'
    Enable-RetoBlock -Path (Join-Path $temp10 $interfacesRel) -Marker 'RETO M04 4.4 - PUERTO DETALLE POR ORDEN'
    Enable-RetoBlock -Path (Join-Path $temp10 $repoRel) -Marker 'RETO M04 4.4 - N+1 SOBRE DETALLE POR ORDEN'
    Enable-RetoBlock -Path (Join-Path $temp10 $useCaseRel) -Marker 'RETO M04 4.4 - DETALLE POR ORDEN'
    Enable-RetoBlock -Path (Join-Path $temp10 $programRel) -Marker 'RETO M04 4.4 - EJECUTAR DETALLE POR ORDEN'
    Invoke-Build41 -Root $temp10 -Context '4.4/Paso 10 reto build'
    $out10 = Invoke-Run41 -Root $temp10 -Context '4.4/Paso 10 reto run'
    Assert-TextContains -Text $out10 -Tokens @(
        'Reto 4.4 OK | Ordenes=5 | Detalles=4 | Consultas SQL=6',
        'una carga anticipada o una proyeccion puede evitar la consulta adicional por orden; se implementa en 4.5.',
        '4.4 OK'
    ) -Context '4.4/Paso 10'
    Write-Host 'PASS 4.4/Paso 10 · N+1 sobre DetalleOrden medido; solución reservada para 4.5'

    Write-Host 'PASS 4.4 COMPLETO'
}


function Test-M045 {
    Write-Section 'M04 · 4.5 Solución a N+1: Include, proyecciones y Split Queries'

    $root = Join-Path $RepoRoot 'M04\PROYECTO\4.5'
    $repoRel = 'src\AceriaData.Infrastructure\Repositories\Rendimiento45.cs'
    $useCaseRel = 'src\AceriaData.Application\SolucionesNMasUnoUseCase.cs'
    $programRel = 'src\AceriaData.Console\Program.cs'
    $interfacesRel = 'src\AceriaData.Application\Interfaces.cs'

    Invoke-Checked -WorkingDirectory $root -FilePath 'dotnet' -ArgumentList @('restore','AceriaData.sln') -Context '4.5/Paso 1 restore' | Out-Null
    Write-Host 'PASS 4.5/Paso 1'

    $migrations = Invoke-Checked -WorkingDirectory $root -FilePath 'dotnet' -ArgumentList @('ef','migrations','list','--project','src/AceriaData.Infrastructure','--startup-project','src/AceriaData.Console','--configuration','Release') -Context '4.5/Paso 2 migrations list'
    Assert-TextContains -Text $migrations -Tokens @('M2_2_12_Architecture') -Context '4.5/Paso 2'
    if ($migrations -match '(?m)^\S*M4_') {
        throw '4.5/Paso 2: aparecen migraciones M4 y el punto no cambia el esquema.'
    }
    Write-Host 'PASS 4.5/Paso 2'

    $interfaces = Get-Content (Join-Path $root $interfacesRel) -Raw
    $repo = Get-Content (Join-Path $root $repoRel) -Raw
    foreach ($token in @(
        'EjecutarIncludeContraNMasUnoM4',
        'EjecutarProyeccionContraNMasUnoM4',
        'EjecutarSplitQueryContraNMasUnoM4'
    )) {
        if (-not $interfaces.Contains($token)) { throw "4.5/Paso 3: falta en el puerto $token." }
        if (-not $repo.Contains($token)) { throw "4.5/Paso 3: falta en Infrastructure $token." }
    }
    foreach ($token in @(
        '.Include(o => o.Planchas)',
        '.Select(o => new { o.Id, TotalPlanchas = o.Planchas.Count })',
        '.AsNoTrackingWithIdentityResolution()',
        '.Include(o => o.OrdenesAleaciones)',
        '.ThenInclude(oa => oa.Aleacion)',
        '.AsSplitQuery()',
        'SqlCommandCounterInterceptor.Instance.Reset()'
    )) {
        if (-not $repo.Contains($token)) { throw "4.5/Paso 3: falta la evidencia '$token'." }
    }
    Write-Host 'PASS 4.5/Paso 3'

    $temp4 = New-PedagogicalCopy -Source $root -Name 'm04-4-5-paso4'
    Enable-LineCommentWholeFileCopy -Path (Join-Path $temp4 $repoRel) -Marker 'FRAGMENTO PDF M04 4.5 - PASO 4'
    Invoke-Build41 -Root $temp4 -Context '4.5/Paso 4 build variante PDF Infrastructure'
    Write-Host 'PASS 4.5/Paso 4 · las tres soluciones del PDF se activan y compilan'

    $temp5 = New-PedagogicalCopy -Source $root -Name 'm04-4-5-paso5'
    Enable-LineCommentWholeFileCopy -Path (Join-Path $temp5 $useCaseRel) -Marker 'FRAGMENTO PDF M04 4.5 - PASO 5'
    Invoke-Build41 -Root $temp5 -Context '4.5/Paso 5 build caso de uso PDF'
    $out5 = Invoke-Run41 -Root $temp5 -Context '4.5/Paso 5 run caso de uso PDF'
    Assert-TextContains -Text $out5 -Tokens @(
        '=== 4.5 SOLUCIONES AL N+1 ===',
        'Include: 1 consulta.',
        'Proyeccion: 1 consulta.',
        'SplitQuery: 3 consultas para evitar explosion cartesiana con dos colecciones.',
        '4.5 OK'
    ) -Context '4.5/Paso 5'
    Write-Host 'PASS 4.5/Paso 5 · caso de uso del PDF ejecutado sobre LocalDB'

    $temp6 = New-PedagogicalCopy -Source $root -Name 'm04-4-5-paso6'
    Enable-LineCommentWholeFileCopy -Path (Join-Path $temp6 $programRel) -Marker 'FRAGMENTO PDF M04 4.5 - PASO 6'
    Invoke-Build41 -Root $temp6 -Context '4.5/Paso 6 build composition root PDF'
    $out6 = Invoke-Run41 -Root $temp6 -Context '4.5/Paso 6 run composition root PDF'
    Assert-TextContains -Text $out6 -Tokens @('Include: 1 consulta.','Proyeccion: 1 consulta.','SplitQuery: 3 consultas','4.5 OK') -Context '4.5/Paso 6'
    Write-Host 'PASS 4.5/Paso 6 · composition root del PDF activado'

    Invoke-Build41 -Root $root -Context '4.5/Paso 7 build final'
    $appProject = Get-Content (Join-Path $root 'src\AceriaData.Application\AceriaData.Application.csproj') -Raw
    if ($appProject -match 'EntityFrameworkCore') {
        throw '4.5/Paso 7: Application referencia EntityFrameworkCore.'
    }
    Write-Host 'PASS 4.5/Paso 7'

    $out8 = Invoke-Run41 -Root $root -Context '4.5/Paso 8 run LocalDB'
    Assert-TextContains -Text $out8 -Tokens @(
        '=== 4.5 SOLUCIONES AL N+1 ===',
        'Include: 1 consulta.',
        'Proyeccion: 1 consulta.',
        'SplitQuery: 3 consultas para evitar explosion cartesiana con dos colecciones.',
        '4.5 OK'
    ) -Context '4.5/Paso 8'
    Write-Host 'PASS 4.5/Paso 8 · Include=1, proyección=1 y SplitQuery=3 medidos'

    $temp9 = New-PedagogicalCopy -Source $root -Name 'm04-4-5-error-no-todo-uno'
    Enable-RetoBlock -Path (Join-Path $temp9 $useCaseRel) -Marker 'ERROR CONTROLADO M04 4.5 - EVITAR N+1 NO IMPLICA UNA CONSULTA'
    Enable-RetoBlock -Path (Join-Path $temp9 $programRel) -Marker 'ERROR CONTROLADO M04 4.5 - EJECUTAR DIAGNOSTICO DE COMANDOS'
    Invoke-Build41 -Root $temp9 -Context '4.5/Paso 9 build diagnóstico'
    $out9 = Invoke-Run41 -Root $temp9 -Context '4.5/Paso 9 run diagnóstico'
    Assert-TextContains -Text $out9 -Tokens @(
        'Error controlado 4.5 OK | evitar N+1 no implica 1 comando | Include=1 | Proyeccion=1 | Split=3',
        '4.5 OK'
    ) -Context '4.5/Paso 9'
    Write-Host 'PASS 4.5/Paso 9 · evitar N+1 no se confunde con forzar una sola consulta'

    $program = Get-Content (Join-Path $root $programRel) -Raw
    if ($program -match 'EnsureCreated') {
        throw '4.5/Paso 10: aparece EnsureCreated.'
    }
    if ($program -notmatch 'Database\.Migrate\(\)') {
        throw '4.5/Paso 10: falta Database.Migrate().'
    }

    $temp10 = New-PedagogicalCopy -Source $root -Name 'm04-4-5-reto-grafo'
    Enable-RetoBlock -Path (Join-Path $temp10 $useCaseRel) -Marker 'RETO M04 4.5 - GRAFO COMPLETO SPLIT QUERY'
    Enable-RetoBlock -Path (Join-Path $temp10 $programRel) -Marker 'RETO M04 4.5 - EJECUTAR GRAFO COMPLETO'
    Invoke-Build41 -Root $temp10 -Context '4.5/Paso 10 reto build'
    $out10 = Invoke-Run41 -Root $temp10 -Context '4.5/Paso 10 reto run'
    Assert-TextContains -Text $out10 -Tokens @(
        'Reto 4.5 OK | Ordenes=5 | Relacionados=9 | Consultas SQL=3',
        'consulta raiz + colección Planchas + colección OrdenesAleaciones',
        '4.5 OK'
    ) -Context '4.5/Paso 10'
    Write-Host 'PASS 4.5/Paso 10 · grafo completo justifica los 3 comandos de SplitQuery'

    Write-Host 'PASS 4.5 COMPLETO'
}


function Test-M046 {
    Write-Section 'M04 · 4.6 Over-fetching: causas y soluciones'

    $root = Join-Path $RepoRoot 'M04\PROYECTO\4.6'
    $repoRel = 'src\AceriaData.Infrastructure\Repositories\Rendimiento46.cs'
    $useCaseRel = 'src\AceriaData.Application\OverFetchingUseCase.cs'
    $programRel = 'src\AceriaData.Console\Program.cs'
    $interfacesRel = 'src\AceriaData.Application\Interfaces.cs'

    Invoke-Checked -WorkingDirectory $root -FilePath 'dotnet' -ArgumentList @('restore','AceriaData.sln') -Context '4.6/Paso 1 restore' | Out-Null
    Write-Host 'PASS 4.6/Paso 1'

    $migrations = Invoke-Checked -WorkingDirectory $root -FilePath 'dotnet' -ArgumentList @('ef','migrations','list','--project','src/AceriaData.Infrastructure','--startup-project','src/AceriaData.Console','--configuration','Release') -Context '4.6/Paso 2 migrations list'
    Assert-TextContains -Text $migrations -Tokens @('M2_2_12_Architecture') -Context '4.6/Paso 2'
    if ($migrations -match '(?m)^\S*M4_') {
        throw '4.6/Paso 2: aparecen migraciones M4 y el punto no cambia el esquema.'
    }
    Write-Host 'PASS 4.6/Paso 2'

    $interfaces = Get-Content (Join-Path $root $interfacesRel) -Raw
    $repo = Get-Content (Join-Path $root $repoRel) -Raw
    foreach ($token in @(
        'ObtenerPendientesEntidadCompletaM4',
        'ObtenerPendientesProyectadasM4',
        'ObtenerSqlPendientesEntidadCompletaM4',
        'ObtenerSqlPendientesProyectadasM4'
    )) {
        if (-not $interfaces.Contains($token)) { throw "4.6/Paso 3: falta en el puerto $token." }
        if (-not $repo.Contains($token)) { throw "4.6/Paso 3: falta en Infrastructure $token." }
    }
    foreach ($token in @(
        'private IQueryable<OrdenFabricacion> PendientesM4()',
        '.AsNoTracking()',
        '.Where(o => o.Estado == "Pendiente")',
        '.OrderBy(o => o.FechaCreacion)',
        '.ThenBy(o => o.Id)',
        '.Select(o => new OrdenResumenDto',
        '.ToQueryString()'
    )) {
        if (-not $repo.Contains($token)) { throw "4.6/Paso 3: falta la evidencia '$token'." }
    }
    Write-Host 'PASS 4.6/Paso 3'

    $temp4 = New-PedagogicalCopy -Source $root -Name 'm04-4-6-paso4'
    Enable-LineCommentWholeFileCopy -Path (Join-Path $temp4 $repoRel) -Marker 'FRAGMENTO PDF M04 4.6 - PASO 4'
    Invoke-Build41 -Root $temp4 -Context '4.6/Paso 4 build variante PDF Infrastructure'
    Write-Host 'PASS 4.6/Paso 4 · Infrastructure del PDF activada y compilada'

    $temp5 = New-PedagogicalCopy -Source $root -Name 'm04-4-6-paso5'
    Enable-LineCommentWholeFileCopy -Path (Join-Path $temp5 $useCaseRel) -Marker 'FRAGMENTO PDF M04 4.6 - PASO 5'
    Invoke-Build41 -Root $temp5 -Context '4.6/Paso 5 build caso de uso PDF'
    $out5 = Invoke-Run41 -Root $temp5 -Context '4.6/Paso 5 run caso de uso PDF'
    Assert-TextContains -Text $out5 -Tokens @(
        '=== 4.6 OVER-FETCHING ===',
        'Filas equivalentes:',
        '--- SQL entidad completa ---',
        '--- SQL proyeccion ---',
        '4.6 OK'
    ) -Context '4.6/Paso 5'
    Write-Host 'PASS 4.6/Paso 5 · caso de uso del PDF activado y ejecutado'

    $temp6 = New-PedagogicalCopy -Source $root -Name 'm04-4-6-paso6'
    Enable-LineCommentWholeFileCopy -Path (Join-Path $temp6 $programRel) -Marker 'FRAGMENTO PDF M04 4.6 - PASO 6'
    Invoke-Build41 -Root $temp6 -Context '4.6/Paso 6 build composition root PDF'
    $out6 = Invoke-Run41 -Root $temp6 -Context '4.6/Paso 6 run composition root PDF'
    Assert-TextContains -Text $out6 -Tokens @('=== 4.6 OVER-FETCHING ===','Filas equivalentes:','4.6 OK') -Context '4.6/Paso 6'
    Write-Host 'PASS 4.6/Paso 6 · composition root del PDF activado'

    Invoke-Build41 -Root $root -Context '4.6/Paso 7 build final'
    $appProject = Get-Content (Join-Path $root 'src\AceriaData.Application\AceriaData.Application.csproj') -Raw
    if ($appProject -match 'EntityFrameworkCore') {
        throw '4.6/Paso 7: Application referencia EntityFrameworkCore.'
    }
    Write-Host 'PASS 4.6/Paso 7'

    $out8 = Invoke-Run41 -Root $root -Context '4.6/Paso 8 run LocalDB'
    $filas = [regex]::Match($out8,'Filas equivalentes:\s*(\d+)')
    if (-not $filas.Success -or [int]$filas.Groups[1].Value -le 0) {
        throw '4.6/Paso 8: no se obtuvo una cardinalidad positiva y comparable.'
    }

    $fullStart = $out8.IndexOf('--- SQL entidad completa ---')
    $projStart = $out8.IndexOf('--- SQL proyeccion ---',$fullStart)
    $end = $out8.IndexOf('4.6 OK',$projStart)
    if ($fullStart -lt 0 -or $projStart -lt 0 -or $end -lt 0) {
        throw '4.6/Paso 8: no se pueden aislar los dos SELECT.'
    }
    $fullSql = $out8.Substring($fullStart,$projStart-$fullStart)
    $projSql = $out8.Substring($projStart,$end-$projStart)

    Assert-TextContains -Text $fullSql -Tokens @('SELECT','NumeroOrden','Cliente','Estado','FechaCreacion','Observaciones','FechaEntrega') -Context '4.6/Paso 8 SQL completo'
    Assert-TextContains -Text $projSql -Tokens @('SELECT','NumeroOrden','Cliente','Estado','FechaCreacion') -Context '4.6/Paso 8 SQL proyectado'
    foreach ($extra in @('Observaciones','FechaEntrega')) {
        if ($projSql -match [regex]::Escape($extra)) {
            throw "4.6/Paso 8: el SELECT proyectado sigue incluyendo $extra."
        }
    }
    Write-Host 'PASS 4.6/Paso 8 · misma cardinalidad y menor shape SQL comprobados'

    $temp9 = New-PedagogicalCopy -Source $root -Name 'm04-4-6-error-materializacion'
    Enable-RetoBlock -Path (Join-Path $temp9 $interfacesRel) -Marker 'ERROR CONTROLADO M04 4.6 - PUERTO MATERIALIZACION TEMPRANA'
    Enable-RetoBlock -Path (Join-Path $temp9 $repoRel) -Marker 'ERROR CONTROLADO M04 4.6 - MATERIALIZAR ANTES DE PROYECTAR'
    Enable-RetoBlock -Path (Join-Path $temp9 $useCaseRel) -Marker 'ERROR CONTROLADO M04 4.6 - PROYECCION DEMASIADO TARDE'
    Enable-RetoBlock -Path (Join-Path $temp9 $programRel) -Marker 'ERROR CONTROLADO M04 4.6 - EJECUTAR MATERIALIZACION TEMPRANA'
    Invoke-Build41 -Root $temp9 -Context '4.6/Paso 9 build error controlado'
    $out9 = Invoke-Run41 -Root $temp9 -Context '4.6/Paso 9 run error controlado'
    Assert-TextContains -Text $out9 -Tokens @(
        'Error controlado 4.6 OK | Filas=',
        'ToList antes de Select conserva SELECT completo',
        '4.6 OK'
    ) -Context '4.6/Paso 9'
    Write-Host 'PASS 4.6/Paso 9 · proyección posterior a ToList demostrada como demasiado tardía'

    $program = Get-Content (Join-Path $root $programRel) -Raw
    if ($program -match 'EnsureCreated') {
        throw '4.6/Paso 10: aparece EnsureCreated.'
    }
    if ($program -notmatch 'Database\.Migrate\(\)') {
        throw '4.6/Paso 10: falta Database.Migrate().'
    }

    $temp10 = New-PedagogicalCopy -Source $root -Name 'm04-4-6-reto-columnas'
    Enable-RetoBlock -Path (Join-Path $temp10 $useCaseRel) -Marker 'RETO M04 4.6 - COLUMNAS ELIMINADAS POR LA PROYECCION'
    Enable-RetoBlock -Path (Join-Path $temp10 $programRel) -Marker 'RETO M04 4.6 - EJECUTAR COMPARACION DE COLUMNAS'
    Invoke-Build41 -Root $temp10 -Context '4.6/Paso 10 reto build'
    $out10 = Invoke-Run41 -Root $temp10 -Context '4.6/Paso 10 reto run'
    Assert-TextContains -Text $out10 -Tokens @(
        'Reto 4.6 OK | columnas eliminadas del SELECT: Observaciones,FechaEntrega',
        'Menos columnas transferidas implica menos datos que transportar y materializar para la misma cardinalidad.',
        '4.6 OK'
    ) -Context '4.6/Paso 10'
    Write-Host 'PASS 4.6/Paso 10 · columnas eliminadas relacionadas con transferencia y materialización'

    Write-Host 'PASS 4.6 COMPLETO'
}


function Test-M047 {
    Write-Section 'M04 · 4.7 Consultas ineficientes: traducción y frontera cliente/servidor'

    $root = Join-Path $RepoRoot 'M04\PROYECTO\4.7'
    $repoRel = 'src\AceriaData.Infrastructure\Repositories\Rendimiento47.cs'
    $useCaseRel = 'src\AceriaData.Application\TraduccionConsultasUseCase.cs'
    $programRel = 'src\AceriaData.Console\Program.cs'
    $interfacesRel = 'src\AceriaData.Application\Interfaces.cs'

    Invoke-Checked -WorkingDirectory $root -FilePath 'dotnet' -ArgumentList @('restore','AceriaData.sln') -Context '4.7/Paso 1 restore' | Out-Null
    Write-Host 'PASS 4.7/Paso 1'

    $migrations = Invoke-Checked -WorkingDirectory $root -FilePath 'dotnet' -ArgumentList @('ef','migrations','list','--project','src/AceriaData.Infrastructure','--startup-project','src/AceriaData.Console','--configuration','Release') -Context '4.7/Paso 2 migrations list'
    Assert-TextContains -Text $migrations -Tokens @('M2_2_12_Architecture') -Context '4.7/Paso 2'
    if ($migrations -match '(?m)^\S*M4_') {
        throw '4.7/Paso 2: aparecen migraciones M4 y el punto no cambia el esquema.'
    }
    Write-Host 'PASS 4.7/Paso 2'

    $interfaces = Get-Content (Join-Path $root $interfacesRel) -Raw
    $repo = Get-Content (Join-Path $root $repoRel) -Raw
    foreach ($token in @(
        'ContarConEvaluacionClienteExplicitaM4',
        'FiltroPersonalizadoNoTraducibleFallaM4',
        'ObtenerSqlClienteConFuncionM4',
        'ObtenerSqlClienteDirectoM4'
    )) {
        if (-not $interfaces.Contains($token)) { throw "4.7/Paso 3: falta en el puerto $token." }
        if (-not $repo.Contains($token)) { throw "4.7/Paso 3: falta en Infrastructure $token." }
    }
    foreach ($token in @(
        'EstadoCoincideM4',
        '.Where(o => EstadoCoincideM4(o.Estado, estado))',
        'catch (InvalidOperationException)',
        '.AsEnumerable()',
        '.Where(o => o.Cliente.ToLower() == normalizado)',
        '.Where(o => o.Cliente == cliente)',
        '.ToQueryString()'
    )) {
        if (-not $repo.Contains($token)) { throw "4.7/Paso 3: falta la evidencia '$token'." }
    }
    Write-Host 'PASS 4.7/Paso 3'

    $temp4 = New-PedagogicalCopy -Source $root -Name 'm04-4-7-paso4'
    Enable-LineCommentWholeFileCopy -Path (Join-Path $temp4 $repoRel) -Marker 'FRAGMENTO PDF M04 4.7 - PASO 4'
    Invoke-Build41 -Root $temp4 -Context '4.7/Paso 4 build variante PDF Infrastructure'
    Write-Host 'PASS 4.7/Paso 4 · Infrastructure del PDF activada y compilada'

    $temp5 = New-PedagogicalCopy -Source $root -Name 'm04-4-7-paso5'
    Enable-LineCommentWholeFileCopy -Path (Join-Path $temp5 $useCaseRel) -Marker 'FRAGMENTO PDF M04 4.7 - PASO 5'
    Invoke-Build41 -Root $temp5 -Context '4.7/Paso 5 build caso de uso PDF'
    $out5 = Invoke-Run41 -Root $temp5 -Context '4.7/Paso 5 run caso de uso PDF'
    Assert-TextContains -Text $out5 -Tokens @(
        '=== 4.7 TRADUCCION Y FRONTERA CLIENTE/SERVIDOR ===',
        'Filtro no traducible: InvalidOperationException observada.',
        'Evaluacion cliente explicita:',
        '--- SQL con funcion sobre columna ---',
        '--- SQL con comparacion directa ---',
        '4.7 OK'
    ) -Context '4.7/Paso 5'
    Write-Host 'PASS 4.7/Paso 5 · fallo de traducción y frontera explícita ejecutados'

    $temp6 = New-PedagogicalCopy -Source $root -Name 'm04-4-7-paso6'
    Enable-LineCommentWholeFileCopy -Path (Join-Path $temp6 $programRel) -Marker 'FRAGMENTO PDF M04 4.7 - PASO 6'
    Invoke-Build41 -Root $temp6 -Context '4.7/Paso 6 build composition root PDF'
    $out6 = Invoke-Run41 -Root $temp6 -Context '4.7/Paso 6 run composition root PDF'
    Assert-TextContains -Text $out6 -Tokens @(
        'Filtro no traducible: InvalidOperationException observada.',
        'Evaluacion cliente explicita:',
        '4.7 OK'
    ) -Context '4.7/Paso 6'
    Write-Host 'PASS 4.7/Paso 6 · composition root del PDF activado'

    Invoke-Build41 -Root $root -Context '4.7/Paso 7 build final'
    $appProject = Get-Content (Join-Path $root 'src\AceriaData.Application\AceriaData.Application.csproj') -Raw
    if ($appProject -match 'EntityFrameworkCore') {
        throw '4.7/Paso 7: Application referencia EntityFrameworkCore.'
    }
    Write-Host 'PASS 4.7/Paso 7'

    $out8 = Invoke-Run41 -Root $root -Context '4.7/Paso 8 run LocalDB'
    Assert-TextContains -Text $out8 -Tokens @(
        '=== 4.7 TRADUCCION Y FRONTERA CLIENTE/SERVIDOR ===',
        'Filtro no traducible: InvalidOperationException observada.',
        'Evaluacion cliente explicita: 3 filas coincidentes.',
        '--- SQL con funcion sobre columna ---',
        '--- SQL con comparacion directa ---',
        '4.7 OK'
    ) -Context '4.7/Paso 8'

    $funcStart = $out8.IndexOf('--- SQL con funcion sobre columna ---')
    $directStart = $out8.IndexOf('--- SQL con comparacion directa ---',$funcStart)
    $end = $out8.IndexOf('4.7 OK',$directStart)
    if ($funcStart -lt 0 -or $directStart -lt 0 -or $end -lt 0) {
        throw '4.7/Paso 8: no se pueden aislar los SQL comparados.'
    }
    $funcSql = $out8.Substring($funcStart,$directStart-$funcStart)
    $directSql = $out8.Substring($directStart,$end-$directStart)
    Assert-TextContains -Text $funcSql -Tokens @('WHERE','LOWER') -Context '4.7/Paso 8 SQL con función'
    Assert-TextContains -Text $directSql -Tokens @('WHERE','Cliente') -Context '4.7/Paso 8 SQL directo'
    if ($directSql -match '(?i)LOWER') {
        throw '4.7/Paso 8: la comparación directa contiene LOWER.'
    }
    Write-Host 'PASS 4.7/Paso 8 · traducción, evaluación cliente explícita y forma SQL comprobadas'

    $temp9 = New-PedagogicalCopy -Source $root -Name 'm04-4-7-error-frontera'
    Enable-RetoBlock -Path (Join-Path $temp9 $interfacesRel) -Marker 'ERROR CONTROLADO M04 4.7 - PUERTO FRONTERA CLIENTE TEMPRANA'
    Enable-RetoBlock -Path (Join-Path $temp9 $repoRel) -Marker 'ERROR CONTROLADO M04 4.7 - FRONTERA CLIENTE DEMASIADO PRONTO'
    Enable-RetoBlock -Path (Join-Path $temp9 $useCaseRel) -Marker 'ERROR CONTROLADO M04 4.7 - ASENUMERABLE ANTES DEL FILTRO'
    Enable-RetoBlock -Path (Join-Path $temp9 $programRel) -Marker 'ERROR CONTROLADO M04 4.7 - EJECUTAR FRONTERA CLIENTE TEMPRANA'
    Invoke-Build41 -Root $temp9 -Context '4.7/Paso 9 build error controlado'
    $out9 = Invoke-Run41 -Root $temp9 -Context '4.7/Paso 9 run error controlado'
    Assert-TextContains -Text $out9 -Tokens @(
        'Error controlado 4.7 OK | coincidencias=3 | SQL previo conserva filtros globales pero no filtra Pendiente',
        '4.7 OK'
    ) -Context '4.7/Paso 9'
    Write-Host 'PASS 4.7/Paso 9 · frontera cliente temprana conserva filtros globales pero no el predicado cliente'

    $program = Get-Content (Join-Path $root $programRel) -Raw
    if ($program -match 'EnsureCreated') {
        throw '4.7/Paso 10: aparece EnsureCreated.'
    }
    if ($program -notmatch 'Database\.Migrate\(\)') {
        throw '4.7/Paso 10: falta Database.Migrate().'
    }

    $temp10 = New-PedagogicalCopy -Source $root -Name 'm04-4-7-reto-formato'
    Enable-RetoBlock -Path (Join-Path $temp10 $interfacesRel) -Marker 'RETO M04 4.7 - PUERTO VALIDACION DE FORMATO TRADUCIBLE'
    Enable-RetoBlock -Path (Join-Path $temp10 $repoRel) -Marker 'RETO M04 4.7 - VALIDACION DE FORMATO TRADUCIBLE'
    Enable-RetoBlock -Path (Join-Path $temp10 $useCaseRel) -Marker 'RETO M04 4.7 - FORMATO TRADUCIBLE EN SERVIDOR'
    Enable-RetoBlock -Path (Join-Path $temp10 $programRel) -Marker 'RETO M04 4.7 - EJECUTAR FORMATO TRADUCIBLE'
    Invoke-Build41 -Root $temp10 -Context '4.7/Paso 10 reto build'
    $out10 = Invoke-Run41 -Root $temp10 -Context '4.7/Paso 10 reto run'
    Assert-TextContains -Text $out10 -Tokens @(
        'Reto 4.7 OK | formato traducible en SQL | filas=5',
        'WHERE',
        'LEN',
        '4.7 OK'
    ) -Context '4.7/Paso 10'
    Write-Host 'PASS 4.7/Paso 10 · validación de formato reescrita para permanecer en SQL'

    Write-Host 'PASS 4.7 COMPLETO'
}


function Test-M048 {
    Write-Section 'M04 · 4.8 Split Queries: cuándo y cómo usarlas'

    $root = Join-Path $RepoRoot 'M04\PROYECTO\4.8'
    $repoRel = 'src\AceriaData.Infrastructure\Repositories\Rendimiento48.cs'
    $useCaseRel = 'src\AceriaData.Application\SplitQueriesUseCase.cs'
    $programRel = 'src\AceriaData.Console\Program.cs'
    $interfacesRel = 'src\AceriaData.Application\Interfaces.cs'
    $depRel = 'src\AceriaData.Infrastructure\DependencyInjection.cs'

    Invoke-Checked -WorkingDirectory $root -FilePath 'dotnet' -ArgumentList @('restore','AceriaData.sln') -Context '4.8/Paso 1 restore' | Out-Null
    Write-Host 'PASS 4.8/Paso 1'

    $migrations = Invoke-Checked -WorkingDirectory $root -FilePath 'dotnet' -ArgumentList @('ef','migrations','list','--project','src/AceriaData.Infrastructure','--startup-project','src/AceriaData.Console','--configuration','Release') -Context '4.8/Paso 2 migrations list'
    Assert-TextContains -Text $migrations -Tokens @('M2_2_12_Architecture') -Context '4.8/Paso 2'
    if ($migrations -match '(?m)^\S*M4_') {
        throw '4.8/Paso 2: aparecen migraciones M4 y el punto no cambia el esquema.'
    }
    Write-Host 'PASS 4.8/Paso 2'

    $interfaces = Get-Content (Join-Path $root $interfacesRel) -Raw
    $repo = Get-Content (Join-Path $root $repoRel) -Raw
    foreach ($token in @(
        'MedirSingleQueryM4',
        'MedirSplitQueryM4',
        'ObtenerSqlSingleQueryM4',
        'ObtenerSqlSplitQueryM4'
    )) {
        if (-not $interfaces.Contains($token)) { throw "4.8/Paso 3: falta en el puerto $token." }
        if (-not $repo.Contains($token)) { throw "4.8/Paso 3: falta en Infrastructure $token." }
    }
    foreach ($token in @(
        'ConsultaDosColeccionesM4',
        '.AsNoTrackingWithIdentityResolution()',
        '.Include(o => o.Planchas)',
        '.Include(o => o.OrdenesAleaciones)',
        '.ThenInclude(oa => oa.Aleacion)',
        '.AsSingleQuery()',
        '.AsSplitQuery()',
        'SqlCommandCounterInterceptor.Instance.Reset()'
    )) {
        if (-not $repo.Contains($token)) { throw "4.8/Paso 3: falta la evidencia '$token'." }
    }
    Write-Host 'PASS 4.8/Paso 3'

    $temp4 = New-PedagogicalCopy -Source $root -Name 'm04-4-8-paso4'
    Enable-LineCommentWholeFileCopy -Path (Join-Path $temp4 $repoRel) -Marker 'FRAGMENTO PDF M04 4.8 - PASO 4'
    Invoke-Build41 -Root $temp4 -Context '4.8/Paso 4 build variante PDF Infrastructure'
    Write-Host 'PASS 4.8/Paso 4 · Infrastructure del PDF activada y compilada'

    $temp5 = New-PedagogicalCopy -Source $root -Name 'm04-4-8-paso5'
    Enable-LineCommentWholeFileCopy -Path (Join-Path $temp5 $useCaseRel) -Marker 'FRAGMENTO PDF M04 4.8 - PASO 5'
    Invoke-Build41 -Root $temp5 -Context '4.8/Paso 5 build caso de uso PDF'
    $out5 = Invoke-Run41 -Root $temp5 -Context '4.8/Paso 5 run caso de uso PDF'
    Assert-TextContains -Text $out5 -Tokens @(
        '=== 4.8 SINGLE QUERY VS SPLIT QUERY ===',
        'SingleQuery: 1 comando SQL.',
        'SplitQuery: 3 comandos SQL.',
        '--- ToQueryString SingleQuery ---',
        '--- ToQueryString SplitQuery ---',
        '4.8 OK'
    ) -Context '4.8/Paso 5'
    Write-Host 'PASS 4.8/Paso 5 · caso de uso del PDF ejecutado con 1 vs 3 comandos'

    $temp6 = New-PedagogicalCopy -Source $root -Name 'm04-4-8-paso6'
    Enable-LineCommentWholeFileCopy -Path (Join-Path $temp6 $programRel) -Marker 'FRAGMENTO PDF M04 4.8 - PASO 6'
    Invoke-Build41 -Root $temp6 -Context '4.8/Paso 6 build composition root PDF'
    $out6 = Invoke-Run41 -Root $temp6 -Context '4.8/Paso 6 run composition root PDF'
    Assert-TextContains -Text $out6 -Tokens @('SingleQuery: 1 comando SQL.','SplitQuery: 3 comandos SQL.','4.8 OK') -Context '4.8/Paso 6'
    Write-Host 'PASS 4.8/Paso 6 · composition root del PDF activado'

    Invoke-Build41 -Root $root -Context '4.8/Paso 7 build final'
    $appProject = Get-Content (Join-Path $root 'src\AceriaData.Application\AceriaData.Application.csproj') -Raw
    if ($appProject -match 'EntityFrameworkCore') {
        throw '4.8/Paso 7: Application referencia EntityFrameworkCore.'
    }
    Write-Host 'PASS 4.8/Paso 7'

    $out8 = Invoke-Run41 -Root $root -Context '4.8/Paso 8 run LocalDB'
    Assert-TextContains -Text $out8 -Tokens @(
        '=== 4.8 SINGLE QUERY VS SPLIT QUERY ===',
        'SingleQuery: 1 comando SQL.',
        'SplitQuery: 3 comandos SQL.',
        '--- ToQueryString SingleQuery ---',
        '--- ToQueryString SplitQuery ---',
        '4.8 OK'
    ) -Context '4.8/Paso 8'
    Write-Host 'PASS 4.8/Paso 8 · mismo grafo: Single=1 roundtrip, Split=3 roundtrips'

    $temp9 = New-PedagogicalCopy -Source $root -Name 'm04-4-8-diagnostico-toquerystring'
    Enable-RetoBlock -Path (Join-Path $temp9 $useCaseRel) -Marker 'ERROR CONTROLADO M04 4.8 - INFERIR ROUNDTRIPS DESDE TOQUERYSTRING'
    Enable-RetoBlock -Path (Join-Path $temp9 $programRel) -Marker 'ERROR CONTROLADO M04 4.8 - EJECUTAR DIAGNOSTICO TOQUERYSTRING'
    Invoke-Build41 -Root $temp9 -Context '4.8/Paso 9 build diagnóstico'
    $out9 = Invoke-Run41 -Root $temp9 -Context '4.8/Paso 9 run diagnóstico'
    Assert-TextContains -Text $out9 -Tokens @(
        'Diagnostico 4.8 OK | comandos reales=3 | SELECT visibles en ToQueryString=',
        'ToQueryString describe la forma SQL para diagnóstico; el interceptor mide los comandos realmente ejecutados.',
        '4.8 OK'
    ) -Context '4.8/Paso 9'
    Write-Host 'PASS 4.8/Paso 9 · ToQueryString no se usa como contador de roundtrips'

    $program = Get-Content (Join-Path $root $programRel) -Raw
    if ($program -match 'EnsureCreated') {
        throw '4.8/Paso 10: aparece EnsureCreated.'
    }
    if ($program -notmatch 'Database\.Migrate\(\)') {
        throw '4.8/Paso 10: falta Database.Migrate().'
    }

    $temp10 = New-PedagogicalCopy -Source $root -Name 'm04-4-8-reto-split-global'
    Enable-RetoBlock -Path (Join-Path $temp10 $interfacesRel) -Marker 'RETO M04 4.8 - PUERTO COMPORTAMIENTO GLOBAL'
    Enable-RetoBlock -Path (Join-Path $temp10 $repoRel) -Marker 'RETO M04 4.8 - CONSULTA SIN OVERRIDE EXPLICITO'
    Enable-RetoBlock -Path (Join-Path $temp10 $useCaseRel) -Marker 'RETO M04 4.8 - SPLITQUERY GLOBAL Y OVERRIDE LOCAL'
    Enable-RetoBlock -Path (Join-Path $temp10 $programRel) -Marker 'RETO M04 4.8 - EJECUTAR SPLIT GLOBAL'
    Enable-LineCommentWholeFileCopy -Path (Join-Path $temp10 $depRel) -Marker 'RETO M04 4.8 - DEPENDENCYINJECTION SPLIT GLOBAL'

    Invoke-Build41 -Root $temp10 -Context '4.8/Paso 10 reto build'
    $out10 = Invoke-Run41 -Root $temp10 -Context '4.8/Paso 10 reto run'
    Assert-TextContains -Text $out10 -Tokens @(
        'Reto 4.8 OK | Split global=3 | override AsSingleQuery=1 | grafo=5/5/4',
        'Señal diagnóstica: vigilar el número real de comandos y no convertir SplitQuery global en una regla ciega.',
        '4.8 OK'
    ) -Context '4.8/Paso 10'
    Write-Host 'PASS 4.8/Paso 10 · Split global medido y AsSingleQuery validado como override local'

    Write-Host 'PASS 4.8 COMPLETO'
}


function Test-M049 {
    Write-Section 'M04 · 4.9 Compiled Queries'

    $root = Join-Path $RepoRoot 'M04\PROYECTO\4.9'
    $repoRel = 'src\AceriaData.Infrastructure\Repositories\Rendimiento49.cs'
    $useCaseRel = 'src\AceriaData.Application\CompiledQueriesUseCase.cs'
    $programRel = 'src\AceriaData.Console\Program.cs'
    $interfacesRel = 'src\AceriaData.Application\Interfaces.cs'

    Invoke-Checked -WorkingDirectory $root -FilePath 'dotnet' -ArgumentList @('restore','AceriaData.sln') -Context '4.9/Paso 1 restore' | Out-Null
    Write-Host 'PASS 4.9/Paso 1'

    $migrations = Invoke-Checked -WorkingDirectory $root -FilePath 'dotnet' -ArgumentList @('ef','migrations','list','--project','src/AceriaData.Infrastructure','--startup-project','src/AceriaData.Console','--configuration','Release') -Context '4.9/Paso 2 migrations list'
    Assert-TextContains -Text $migrations -Tokens @('M2_2_12_Architecture') -Context '4.9/Paso 2'
    if ($migrations -match '(?m)^\S*M4_') {
        throw '4.9/Paso 2: aparecen migraciones M4 y el punto no cambia el esquema.'
    }
    Write-Host 'PASS 4.9/Paso 2'

    $interfaces = Get-Content (Join-Path $root $interfacesRel) -Raw
    $repo = Get-Content (Join-Path $root $repoRel) -Raw
    foreach ($token in @('ObtenerPorEstadoNormalM4','ObtenerPorEstadoCompiladoM4')) {
        if (-not $interfaces.Contains($token)) { throw "4.9/Paso 3: falta en el puerto $token." }
        if (-not $repo.Contains($token)) { throw "4.9/Paso 3: falta en Infrastructure $token." }
    }
    foreach ($token in @(
        'private static readonly Func<AceriaDbContext, string, IEnumerable<OrdenFabricacion>>',
        'EF.CompileQuery(',
        '.AsNoTracking()',
        '.Where(o => o.Estado == estado)',
        '.OrderBy(o => o.FechaCreacion)',
        '.ThenBy(o => o.Id)'
    )) {
        if (-not $repo.Contains($token)) { throw "4.9/Paso 3: falta la evidencia '$token'." }
    }
    Write-Host 'PASS 4.9/Paso 3'

    $temp4 = New-PedagogicalCopy -Source $root -Name 'm04-4-9-paso4'
    Enable-LineCommentWholeFileCopy -Path (Join-Path $temp4 $repoRel) -Marker 'FRAGMENTO PDF M04 4.9 - PASO 4'
    Invoke-Build41 -Root $temp4 -Context '4.9/Paso 4 build variante PDF Infrastructure'
    Write-Host 'PASS 4.9/Paso 4 · Infrastructure del PDF activada y compilada'

    $temp5 = New-PedagogicalCopy -Source $root -Name 'm04-4-9-paso5'
    Enable-LineCommentWholeFileCopy -Path (Join-Path $temp5 $useCaseRel) -Marker 'FRAGMENTO PDF M04 4.9 - PASO 5'
    Invoke-Build41 -Root $temp5 -Context '4.9/Paso 5 build caso de uso PDF'
    $out5 = Invoke-Run41 -Root $temp5 -Context '4.9/Paso 5 run caso de uso PDF'
    Assert-TextContains -Text $out5 -Tokens @(
        '=== 4.9 COMPILED QUERIES ===',
        'Normal:',
        'Compilada:',
        'Medicion observacional: no se exige que la compiled query gane',
        '4.9 OK'
    ) -Context '4.9/Paso 5'
    Write-Host 'PASS 4.9/Paso 5 · equivalencia y medición observacional del PDF ejecutadas'

    $temp6 = New-PedagogicalCopy -Source $root -Name 'm04-4-9-paso6'
    Enable-LineCommentWholeFileCopy -Path (Join-Path $temp6 $programRel) -Marker 'FRAGMENTO PDF M04 4.9 - PASO 6'
    Invoke-Build41 -Root $temp6 -Context '4.9/Paso 6 build composition root PDF'
    $out6 = Invoke-Run41 -Root $temp6 -Context '4.9/Paso 6 run composition root PDF'
    Assert-TextContains -Text $out6 -Tokens @('=== 4.9 COMPILED QUERIES ===','Medicion observacional:','4.9 OK') -Context '4.9/Paso 6'
    Write-Host 'PASS 4.9/Paso 6 · composition root del PDF activado'

    Invoke-Build41 -Root $root -Context '4.9/Paso 7 build final'
    $appProject = Get-Content (Join-Path $root 'src\AceriaData.Application\AceriaData.Application.csproj') -Raw
    if ($appProject -match 'EntityFrameworkCore') {
        throw '4.9/Paso 7: Application referencia EntityFrameworkCore.'
    }
    Write-Host 'PASS 4.9/Paso 7'

    $out8 = Invoke-Run41 -Root $root -Context '4.9/Paso 8 run LocalDB'
    Assert-TextContains -Text $out8 -Tokens @(
        '=== 4.9 COMPILED QUERIES ===',
        'Normal:',
        'Compilada:',
        'Medicion observacional: no se exige que la compiled query gane en un dataset pequeno; se valida equivalencia y reutilizacion.',
        '4.9 OK'
    ) -Context '4.9/Paso 8'
    Write-Host 'PASS 4.9/Paso 8 · resultado normal y compilado equivalentes sin umbral temporal'

    $temp9 = New-PedagogicalCopy -Source $root -Name 'm04-4-9-error-compilar-cada-vez'
    Enable-RetoBlock -Path (Join-Path $temp9 $interfacesRel) -Marker 'ERROR CONTROLADO M04 4.9 - PUERTO COMPILAR CADA VEZ'
    Enable-RetoBlock -Path (Join-Path $temp9 $repoRel) -Marker 'ERROR CONTROLADO M04 4.9 - COMPILAR EN CADA LLAMADA'
    Enable-RetoBlock -Path (Join-Path $temp9 $useCaseRel) -Marker 'ERROR CONTROLADO M04 4.9 - DELEGADO NO REUTILIZADO'
    Enable-RetoBlock -Path (Join-Path $temp9 $programRel) -Marker 'ERROR CONTROLADO M04 4.9 - EJECUTAR COMPILACION POR LLAMADA'
    Invoke-Build41 -Root $temp9 -Context '4.9/Paso 9 build error controlado'
    $out9 = Invoke-Run41 -Root $temp9 -Context '4.9/Paso 9 run error controlado'
    Assert-TextContains -Text $out9 -Tokens @(
        'Error controlado 4.9 OK | compilar por llamada crea delegados repetidos: 1->2',
        '4.9 OK'
    ) -Context '4.9/Paso 9'
    Write-Host 'PASS 4.9/Paso 9 · compilar el delegado en cada llamada se identifica como anti-patrón'

    $program = Get-Content (Join-Path $root $programRel) -Raw
    if ($program -match 'EnsureCreated') {
        throw '4.9/Paso 10: aparece EnsureCreated.'
    }
    if ($program -notmatch 'Database\.Migrate\(\)') {
        throw '4.9/Paso 10: falta Database.Migrate().'
    }

    $temp10 = New-PedagogicalCopy -Source $root -Name 'm04-4-9-reto-async'
    Enable-RetoBlock -Path (Join-Path $temp10 $interfacesRel) -Marker 'RETO M04 4.9 - PUERTO COMPILED ASYNC PROYECTADA'
    Enable-RetoBlock -Path (Join-Path $temp10 $repoRel) -Marker 'RETO M04 4.9 - COMPILED ASYNC QUERY PROYECTADA'
    Enable-RetoBlock -Path (Join-Path $temp10 $useCaseRel) -Marker 'RETO M04 4.9 - COMPILED ASYNC QUERY PROYECTADA'
    Enable-RetoBlock -Path (Join-Path $temp10 $programRel) -Marker 'RETO M04 4.9 - EJECUTAR COMPILED ASYNC QUERY'
    Invoke-Build41 -Root $temp10 -Context '4.9/Paso 10 reto build'
    $out10 = Invoke-Run41 -Root $temp10 -Context '4.9/Paso 10 reto run'
    Assert-TextContains -Text $out10 -Tokens @(
        'Reto 4.9 OK | CompileAsyncQuery proyectada | Filas=',
        'Coste evitado: parte de la preparación de EF; no elimina red, ejecución SQL ni materialización.',
        '4.9 OK'
    ) -Context '4.9/Paso 10'
    Write-Host 'PASS 4.9/Paso 10 · CompileAsyncQuery proyectada validada sin atribuirle costes de SQL Server'

    Write-Host 'PASS 4.9 COMPLETO'
}


function Test-M0410 {
    Write-Section 'M04 · 4.10 Paginación eficiente: Skip/Take y keyset'

    $root = Join-Path $RepoRoot 'M04\PROYECTO\4.10'
    $repoRel = 'src\AceriaData.Infrastructure\Repositories\Rendimiento410.cs'
    $useCaseRel = 'src\AceriaData.Application\PaginacionUseCase.cs'
    $programRel = 'src\AceriaData.Console\Program.cs'
    $interfacesRel = 'src\AceriaData.Application\Interfaces.cs'

    Invoke-Checked -WorkingDirectory $root -FilePath 'dotnet' -ArgumentList @('restore','AceriaData.sln') -Context '4.10/Paso 1 restore' | Out-Null
    Write-Host 'PASS 4.10/Paso 1'

    $migrations = Invoke-Checked -WorkingDirectory $root -FilePath 'dotnet' -ArgumentList @('ef','migrations','list','--project','src/AceriaData.Infrastructure','--startup-project','src/AceriaData.Console','--configuration','Release') -Context '4.10/Paso 2 migrations list'
    Assert-TextContains -Text $migrations -Tokens @('M2_2_12_Architecture') -Context '4.10/Paso 2'
    if ($migrations -match '(?m)^\S*M4_') { throw '4.10/Paso 2: aparecen migraciones M4.' }
    Write-Host 'PASS 4.10/Paso 2'

    $interfaces = Get-Content (Join-Path $root $interfacesRel) -Raw
    $repo = Get-Content (Join-Path $root $repoRel) -Raw
    foreach ($token in @('ObtenerPaginaOffsetM4','ObtenerPaginaKeysetM4')) {
        if (-not $interfaces.Contains($token)) { throw "4.10/Paso 3: falta $token en el puerto." }
        if (-not $repo.Contains($token)) { throw "4.10/Paso 3: falta $token en Infrastructure." }
    }
    foreach ($token in @('.OrderBy(o => o.FechaCreacion)','.ThenBy(o => o.Id)','.Skip((pagina - 1) * tamano)','.Take(tamano)','o.FechaCreacion > ultimaFecha','o.FechaCreacion == ultimaFecha && o.Id > ultimoId','.ToQueryString()')) {
        if (-not $repo.Contains($token)) { throw "4.10/Paso 3: falta evidencia '$token'." }
    }
    Write-Host 'PASS 4.10/Paso 3'

    $temp4 = New-PedagogicalCopy -Source $root -Name 'm04-4-10-paso4'
    Enable-LineCommentWholeFileCopy -Path (Join-Path $temp4 $repoRel) -Marker 'FRAGMENTO PDF M04 4.10 - PASO 4'
    Invoke-Build41 -Root $temp4 -Context '4.10/Paso 4 build Infrastructure PDF'
    Write-Host 'PASS 4.10/Paso 4 · Infrastructure del PDF activada'

    $temp5 = New-PedagogicalCopy -Source $root -Name 'm04-4-10-paso5'
    Enable-LineCommentWholeFileCopy -Path (Join-Path $temp5 $useCaseRel) -Marker 'FRAGMENTO PDF M04 4.10 - PASO 5'
    Invoke-Build41 -Root $temp5 -Context '4.10/Paso 5 build use case PDF'
    $out5 = Invoke-Run41 -Root $temp5 -Context '4.10/Paso 5 run use case PDF'
    Assert-TextContains -Text $out5 -Tokens @('=== 4.10 PAGINACION ===','--- OFFSET ---','--- KEYSET ---','4.10 OK') -Context '4.10/Paso 5'
    Write-Host 'PASS 4.10/Paso 5 · caso de uso del PDF ejecutado'

    $temp6 = New-PedagogicalCopy -Source $root -Name 'm04-4-10-paso6'
    Enable-LineCommentWholeFileCopy -Path (Join-Path $temp6 $programRel) -Marker 'FRAGMENTO PDF M04 4.10 - PASO 6'
    Invoke-Build41 -Root $temp6 -Context '4.10/Paso 6 build Program PDF'
    $out6 = Invoke-Run41 -Root $temp6 -Context '4.10/Paso 6 run Program PDF'
    Assert-TextContains -Text $out6 -Tokens @('=== 4.10 PAGINACION ===','4.10 OK') -Context '4.10/Paso 6'
    Write-Host 'PASS 4.10/Paso 6 · composition root del PDF activado'

    Invoke-Build41 -Root $root -Context '4.10/Paso 7 build final'
    $appProject = Get-Content (Join-Path $root 'src\AceriaData.Application\AceriaData.Application.csproj') -Raw
    if ($appProject -match 'EntityFrameworkCore') { throw '4.10/Paso 7: Application referencia EF Core.' }
    Write-Host 'PASS 4.10/Paso 7'

    $out8 = Invoke-Run41 -Root $root -Context '4.10/Paso 8 run LocalDB'
    Assert-TextContains -Text $out8 -Tokens @('=== 4.10 PAGINACION ===','--- OFFSET ---','OFFSET','FETCH NEXT','--- KEYSET ---','ORDER BY','4.10 OK') -Context '4.10/Paso 8'
    $keysetStart = $out8.IndexOf('--- KEYSET ---')
    $keysetEnd = $out8.IndexOf('4.10 OK',$keysetStart)
    $keysetSql = $out8.Substring($keysetStart,$keysetEnd-$keysetStart)
    foreach ($token in @('FechaCreacion','Id','ORDER BY')) {
        if ($keysetSql -notmatch [regex]::Escape($token)) { throw "4.10/Paso 8: SQL keyset sin $token." }
    }
    Write-Host 'PASS 4.10/Paso 8 · offset y keyset reales comprobados'

    $temp9 = New-PedagogicalCopy -Source $root -Name 'm04-4-10-error-cursor'
    Enable-RetoBlock -Path (Join-Path $temp9 $interfacesRel) -Marker 'ERROR CONTROLADO M04 4.10 - PUERTO CURSOR NO UNICO'
    Enable-RetoBlock -Path (Join-Path $temp9 $repoRel) -Marker 'ERROR CONTROLADO M04 4.10 - CURSOR NO UNICO'
    Enable-RetoBlock -Path (Join-Path $temp9 $useCaseRel) -Marker 'ERROR CONTROLADO M04 4.10 - CURSOR NO UNICO SALTA FILAS'
    Enable-RetoBlock -Path (Join-Path $temp9 $programRel) -Marker 'ERROR CONTROLADO M04 4.10 - EJECUTAR CURSOR NO UNICO'
    Invoke-Build41 -Root $temp9 -Context '4.10/Paso 9 build error cursor'
    $out9 = Invoke-Run41 -Root $temp9 -Context '4.10/Paso 9 run error cursor'
    Assert-TextContains -Text $out9 -Tokens @('Error controlado 4.10 OK | cursor no unico Estado | EnProceso total=','| vistos=','4.10 OK') -Context '4.10/Paso 9'
    Write-Host 'PASS 4.10/Paso 9 · cursor no único demuestra filas saltadas'

    $program = Get-Content (Join-Path $root $programRel) -Raw
    if ($program -match 'EnsureCreated') { throw '4.10/Paso 10: aparece EnsureCreated.' }
    if ($program -notmatch 'Database\.Migrate\(\)') { throw '4.10/Paso 10: falta Database.Migrate().' }

    $temp10 = New-PedagogicalCopy -Source $root -Name 'm04-4-10-reto-filtro'
    Enable-RetoBlock -Path (Join-Path $temp10 $interfacesRel) -Marker 'RETO M04 4.10 - PUERTO KEYSET FILTRADO'
    Enable-RetoBlock -Path (Join-Path $temp10 $repoRel) -Marker 'RETO M04 4.10 - KEYSET FILTRADO POR ESTADO CON CURSOR COMPUESTO'
    Enable-RetoBlock -Path (Join-Path $temp10 $useCaseRel) -Marker 'RETO M04 4.10 - FILTRO DE ESTADO + CURSOR COMPUESTO'
    Enable-RetoBlock -Path (Join-Path $temp10 $programRel) -Marker 'RETO M04 4.10 - EJECUTAR FILTRO DE ESTADO'
    Invoke-Build41 -Root $temp10 -Context '4.10/Paso 10 reto build'
    $out10 = Invoke-Run41 -Root $temp10 -Context '4.10/Paso 10 reto run'
    Assert-TextContains -Text $out10 -Tokens @('Reto 4.10 OK | Estado=Pendiente | cursor=FechaCreacion+Id | 5+5 filas sin repetición','WHERE','ORDER BY','4.10 OK') -Context '4.10/Paso 10'
    Write-Host 'PASS 4.10/Paso 10 · filtro de estado conserva cursor compuesto y orden determinista'

    Write-Host 'PASS 4.10 COMPLETO'
}

if ($Suite -eq 'inventory') { Test-M04Inventory; exit 0 }
if ($Suite -eq '4.1') { Test-M041; exit 0 }
if ($Suite -eq '4.2') { Test-M042; exit 0 }
if ($Suite -eq '4.3') { Test-M043; exit 0 }
if ($Suite -eq '4.4') { Test-M044; exit 0 }
if ($Suite -eq '4.5') { Test-M045; exit 0 }
if ($Suite -eq '4.6') { Test-M046; exit 0 }
if ($Suite -eq '4.7') { Test-M047; exit 0 }
if ($Suite -eq '4.8') { Test-M048; exit 0 }
if ($Suite -eq '4.9') { Test-M049; exit 0 }
if ($Suite -eq '4.10') { Test-M0410; exit 0 }

Test-M04Inventory
Test-M041
Test-M042
Test-M043
Test-M044
Test-M045
Test-M046
Test-M047
Test-M048
Test-M049
Test-M0410
Write-Host 'PASS M04 PARCIAL · 4.1–4.10 certificados; siguiente checkpoint: 4.11.'
