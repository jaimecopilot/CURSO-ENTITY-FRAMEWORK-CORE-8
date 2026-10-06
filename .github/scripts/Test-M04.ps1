param(
    [ValidateSet('all','inventory','4.1','4.2','4.3')]
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

if ($Suite -eq 'inventory') {
    Test-M04Inventory
    exit 0
}

if ($Suite -eq '4.1') {
    Test-M041
    exit 0
}

if ($Suite -eq '4.2') {
    Test-M042
    exit 0
}

if ($Suite -eq '4.3') {
    Test-M043
    exit 0
}

Test-M04Inventory
Test-M041
Test-M042
Test-M043
Write-Host 'PASS M04 PARCIAL · 4.1–4.3 certificados; siguiente checkpoint: 4.4.'
