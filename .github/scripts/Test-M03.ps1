param(
    [ValidateSet('all','inventory','3.1','3.2','3.3','3.4','3.5')]
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

function Invoke-Build31([string]$Root,[string]$Context) {
    Invoke-Checked -WorkingDirectory $Root -FilePath 'dotnet' -ArgumentList @('build','AceriaData.sln','--configuration','Release') -Context $Context | Out-Null
}

function Invoke-Run31([string]$Root,[string]$Context) {
    return Invoke-Checked -WorkingDirectory $Root -FilePath 'dotnet' -ArgumentList @('run','--project','src/AceriaData.Console/AceriaData.Console.csproj','--configuration','Release','--no-build') -Context $Context
}

function Test-M03Inventory {
    Write-Section 'M03 · inventario canónico'

    $manifestPath = Join-Path $RepoRoot 'M03\PRACTICA\M03_TRAZABILIDAD_E2E.json'
    $practicePath = Join-Path $RepoRoot 'M03\PRACTICA\M03_PRACTICA.md'

    $manifest = Get-Content $manifestPath -Raw | ConvertFrom-Json
    $practice = Get-Content $practicePath -Raw

    if ($manifest.counts.points -ne 12) { throw 'M03: el manifiesto debe contener 12 puntos.' }
    if ($manifest.counts.main_steps -ne 120) { throw 'M03: el manifiesto debe contener 120 pasos principales.' }
    if ($manifest.counts.paso10_retos -ne 12) { throw 'M03: deben existir 12 retos de Paso 10.' }
    if ($manifest.counts.laboratorios_adicionales -ne 12) { throw 'M03: deben existir 12 laboratorios adicionales.' }
    if ($manifest.counts.trace_units -ne 144) { throw 'M03: el inventario completo debe contener 144 unidades trazables.' }

    for ($n = 1; $n -le 12; $n++) {
        $point = "3.$n"
        $pattern = '(?ms)^## Punto ' + [regex]::Escape($point) + '\b.*?(?=^## Punto 3\.\d+\b|\z)'
        $section = [regex]::Match($practice,$pattern).Value

        if ([string]::IsNullOrWhiteSpace($section)) {
            throw "M03: no se localiza $point en la práctica."
        }

        $steps = [regex]::Matches($section,'(?m)^### Paso (\d+):')
        if ($steps.Count -ne 10) {
            throw "M03: $point debe contener exactamente 10 pasos y contiene $($steps.Count)."
        }

        for ($step = 1; $step -le 10; $step++) {
            if ($section -notmatch ('(?m)^### Paso ' + $step + ':')) {
                throw "M03: falta $point/Paso $step."
            }
        }

        if ($section -notmatch '(?m)^### Laboratorio adicional del punto ') {
            throw "M03: $point no contiene Laboratorio adicional."
        }
        if ($section -notmatch '\*\*Reto:\*\*') {
            throw "M03: $point no contiene el reto del Paso 10."
        }

        Write-Host "PASS inventario $point · 10 pasos + laboratorio + reto"
    }

    Write-Host 'PASS inventario M03: 120 pasos + 12 retos Paso 10 + 12 laboratorios.'
}

function Test-M031 {
    Write-Section 'M03 · 3.1 Fundamentos de LINQ to Entities'

    $root = Join-Path $RepoRoot 'M03\PROYECTO\3.1'
    $useCaseRel = 'src\AceriaData.Application\ConsultasLinqUseCase.cs'
    $programRel = 'src\AceriaData.Console\Program.cs'
    $interfacesRel = 'src\AceriaData.Application\Interfaces.cs'
    $reposRel = 'src\AceriaData.Infrastructure\Repositories\Repositories.cs'

    Invoke-Checked -WorkingDirectory $root -FilePath 'dotnet' -ArgumentList @('restore','AceriaData.sln') -Context '3.1/Paso 1 restore' | Out-Null
    Write-Host 'PASS 3.1/Paso 1'

    $migrations = Invoke-Checked -WorkingDirectory $root -FilePath 'dotnet' -ArgumentList @('ef','migrations','list','--project','src/AceriaData.Infrastructure','--startup-project','src/AceriaData.Console','--configuration','Release') -Context '3.1/Paso 2 migrations list'
    Assert-TextContains -Text $migrations -Tokens @('M2_2_12_Architecture') -Context '3.1/Paso 2'
    if ($migrations -match '(?m)^\S*M3_') {
        throw '3.1/Paso 2: aparecen migraciones M3 y la práctica indica que M3 no cambia el esquema.'
    }
    Write-Host 'PASS 3.1/Paso 2'

    $interfaces = Get-Content (Join-Path $root $interfacesRel) -Raw
    $repos = Get-Content (Join-Path $root $reposRel) -Raw
    foreach ($token in @('IQueryable<OrdenFabricacion> Consulta()','string ObtenerSqlFundamentos()')) {
        if (-not $interfaces.Contains($token)) {
            throw "3.1/Paso 3: falta en el puerto $token."
        }
    }
    foreach ($token in @('public IQueryable<OrdenFabricacion> Consulta()','public string ObtenerSqlFundamentos()','ToQueryString()')) {
        if (-not $repos.Contains($token)) {
            throw "3.1/Paso 3: falta en Infrastructure $token."
        }
    }
    Write-Host 'PASS 3.1/Paso 3'

    $temp4 = New-PedagogicalCopy -Source $root -Name 'm03-3-1-paso4'
    Enable-LineCommentWholeFileCopy -Path (Join-Path $temp4 $useCaseRel) -Marker 'FRAGMENTO PDF M03 3.1 - PASO 4'
    Invoke-Build31 -Root $temp4 -Context '3.1/Paso 4 build variante PDF'
    $out4 = Invoke-Run31 -Root $temp4 -Context '3.1/Paso 4 run variante PDF'
    Assert-TextContains -Text $out4 -Tokens @('=== FUNDAMENTOS DE LINQ TO ENTITIES ===','Enumerable: 3 | IQueryable: 3','3.1 OK') -Context '3.1/Paso 4'
    Write-Host 'PASS 3.1/Paso 4 · copia PDF activada, compilada y ejecutada'

    $temp5 = New-PedagogicalCopy -Source $root -Name 'm03-3-1-paso5'
    Enable-LineCommentWholeFileCopy -Path (Join-Path $temp5 $programRel) -Marker 'FRAGMENTO PDF M03 3.1 - PASO 5'
    Invoke-Build31 -Root $temp5 -Context '3.1/Paso 5 build composition root PDF'
    $out5 = Invoke-Run31 -Root $temp5 -Context '3.1/Paso 5 run composition root PDF'
    Assert-TextContains -Text $out5 -Tokens @('3.1 OK') -Context '3.1/Paso 5'
    Write-Host 'PASS 3.1/Paso 5 · composition root PDF activado'

    Invoke-Build31 -Root $root -Context '3.1/Paso 6 build final'
    $appProject = Get-Content (Join-Path $root 'src\AceriaData.Application\AceriaData.Application.csproj') -Raw
    if ($appProject -match 'EntityFrameworkCore') {
        throw '3.1/Paso 6: Application referencia EntityFrameworkCore.'
    }
    Write-Host 'PASS 3.1/Paso 6'

    $out7 = Invoke-Run31 -Root $root -Context '3.1/Paso 7 run final'
    Assert-TextContains -Text $out7 -Tokens @('Enumerable: 3 | IQueryable: 3','3.1 OK') -Context '3.1/Paso 7'
    Write-Host 'PASS 3.1/Paso 7'

    foreach ($token in @('SELECT','WHERE','ORDER BY','Cliente')) {
        if ($out7 -notmatch [regex]::Escape($token)) {
            throw "3.1/Paso 8: la salida no contiene evidencia SQL '$token'."
        }
    }
    Write-Host 'PASS 3.1/Paso 8'

    $useCase = Get-Content (Join-Path $root $useCaseRel) -Raw
    foreach ($token in @('ObtenerTodas().Where(o => o.Cliente == "Constructora del Norte").ToList()','Consulta().Where(o => o.Cliente == "Constructora del Norte").OrderBy(o => o.FechaCreacion)','var enSql = consulta.ToList()')) {
        if (-not $useCase.Contains($token)) {
            throw "3.1/Paso 9: falta evidencia del diagnóstico '$token'."
        }
    }
    Write-Host 'PASS 3.1/Paso 9'

    $temp10 = New-PedagogicalCopy -Source $root -Name 'm03-3-1-reto'
    Enable-RetoBlock -Path (Join-Path $temp10 $interfacesRel) -Marker 'RETO M03 3.1 - PUERTO SQL OPCIONAL'
    Enable-RetoBlock -Path (Join-Path $temp10 $reposRel) -Marker 'RETO M03 3.1 - SQL OPCIONAL Y PROYECCION MINIMA'
    Enable-RetoBlock -Path (Join-Path $temp10 $useCaseRel) -Marker 'RETO M03 3.1 - FILTRO OPCIONAL SIN MATERIALIZAR'

    Invoke-Build31 -Root $temp10 -Context '3.1/Paso 10 reto build'
    $out10 = Invoke-Run31 -Root $temp10 -Context '3.1/Paso 10 reto run'
    Assert-TextContains -Text $out10 -Tokens @('Reto 3.1 OK | Filas: 2','ORDER BY','NumeroOrden','Cliente','3.1 OK') -Context '3.1/Paso 10'

    $retoMarker = 'Reto 3.1 OK | Filas: 2'
    $retoIndex = $out10.IndexOf($retoMarker)
    if ($retoIndex -lt 0) {
        throw '3.1/Paso 10: no se puede aislar la salida SQL del reto.'
    }
    $retoSql = $out10.Substring($retoIndex)

    if ($retoSql -notmatch 'WHERE') {
        throw '3.1/Paso 10: el SQL del reto no contiene WHERE.'
    }
    if ($retoSql -match '\[o\]\.\[Observaciones\]') {
        throw '3.1/Laboratorio: la proyección SQL del reto no es mínima; incluye Observaciones.'
    }

    Write-Host 'PASS 3.1/Paso 10 + laboratorio adicional'
    Write-Host 'PASS 3.1 COMPLETO'
}


function Test-M032 {
    Write-Section 'M03 · 3.2 Consultas básicas: Where, OrderBy y ThenBy'

    $root = Join-Path $RepoRoot 'M03\PROYECTO\3.2'
    $useCaseRel = 'src\AceriaData.Application\ConsultasBasicasUseCase.cs'
    $programRel = 'src\AceriaData.Console\Program.cs'
    $interfacesRel = 'src\AceriaData.Application\Interfaces.cs'
    $reposRel = 'src\AceriaData.Infrastructure\Repositories\Repositories.cs'

    Invoke-Checked -WorkingDirectory $root -FilePath 'dotnet' -ArgumentList @('restore','AceriaData.sln') -Context '3.2/Paso 1 restore' | Out-Null
    Write-Host 'PASS 3.2/Paso 1'

    $migrations = Invoke-Checked -WorkingDirectory $root -FilePath 'dotnet' -ArgumentList @('ef','migrations','list','--project','src/AceriaData.Infrastructure','--startup-project','src/AceriaData.Console','--configuration','Release') -Context '3.2/Paso 2 migrations list'
    Assert-TextContains -Text $migrations -Tokens @('M2_2_12_Architecture') -Context '3.2/Paso 2'
    if ($migrations -match '(?m)^\S*M3_') {
        throw '3.2/Paso 2: aparecen migraciones M3 y la práctica indica que M3 no cambia el esquema.'
    }
    Write-Host 'PASS 3.2/Paso 2'

    $interfaces = Get-Content (Join-Path $root $interfacesRel) -Raw
    $repos = Get-Content (Join-Path $root $reposRel) -Raw
    foreach ($token in @(
        'ObtenerPendientesPorCliente',
        'ObtenerPorEstadoOrdenadasPorFecha',
        'ObtenerPorRangoDeFechas',
        'ObtenerPorClienteOrdenadas',
        'ObtenerPorClienteYRangoDeFechas',
        'ObtenerSqlConsultaBasica'
    )) {
        if (-not $interfaces.Contains($token)) {
            throw "3.2/Paso 3: falta en el puerto $token."
        }
        if (-not $repos.Contains($token)) {
            throw "3.2/Paso 3: falta en Infrastructure $token."
        }
    }
    foreach ($token in @(
        '.Where(o => o.Cliente == cliente && o.Estado == "Pendiente")',
        '.OrderByDescending(o => o.FechaCreacion)',
        '.Where(o => o.FechaCreacion >= desde && o.FechaCreacion <= hasta)',
        '.OrderBy(o => o.Cliente).ThenByDescending(o => o.FechaCreacion)',
        '.OrderBy(o => o.Estado).ThenByDescending(o => o.FechaCreacion)'
    )) {
        if (-not $repos.Contains($token)) {
            throw "3.2/Paso 3: falta la composición LINQ esperada '$token'."
        }
    }
    Write-Host 'PASS 3.2/Paso 3'

    $temp4 = New-PedagogicalCopy -Source $root -Name 'm03-3-2-paso4'
    Enable-LineCommentWholeFileCopy -Path (Join-Path $temp4 $useCaseRel) -Marker 'FRAGMENTO PDF M03 3.2 - PASO 4'
    Invoke-Build31 -Root $temp4 -Context '3.2/Paso 4 build variante PDF'
    $out4 = Invoke-Run31 -Root $temp4 -Context '3.2/Paso 4 run variante PDF'
    Assert-TextContains -Text $out4 -Tokens @('=== WHERE, ORDERBY Y THENBY ===','Norte pendientes: 2 | Pendientes: 3 | Rango: 5 | Norte ordenadas: 3','3.2 OK') -Context '3.2/Paso 4'
    Write-Host 'PASS 3.2/Paso 4 · copia PDF activada, compilada y ejecutada'

    $temp5 = New-PedagogicalCopy -Source $root -Name 'm03-3-2-paso5'
    Enable-LineCommentWholeFileCopy -Path (Join-Path $temp5 $programRel) -Marker 'FRAGMENTO PDF M03 3.2 - PASO 5'
    Invoke-Build31 -Root $temp5 -Context '3.2/Paso 5 build composition root PDF'
    $out5 = Invoke-Run31 -Root $temp5 -Context '3.2/Paso 5 run composition root PDF'
    Assert-TextContains -Text $out5 -Tokens @('3.2 OK') -Context '3.2/Paso 5'
    Write-Host 'PASS 3.2/Paso 5 · composition root PDF activado'

    Invoke-Build31 -Root $root -Context '3.2/Paso 6 build final'
    $appProject = Get-Content (Join-Path $root 'src\AceriaData.Application\AceriaData.Application.csproj') -Raw
    if ($appProject -match 'EntityFrameworkCore') {
        throw '3.2/Paso 6: Application referencia EntityFrameworkCore.'
    }
    Write-Host 'PASS 3.2/Paso 6'

    $out7 = Invoke-Run31 -Root $root -Context '3.2/Paso 7 run final'
    Assert-TextContains -Text $out7 -Tokens @(
        '=== WHERE, ORDERBY Y THENBY ===',
        'Norte pendientes: 2 | Pendientes: 3 | Rango: 5 | Norte ordenadas: 3',
        '3.2 OK'
    ) -Context '3.2/Paso 7'
    Write-Host 'PASS 3.2/Paso 7'

    $countMarker = 'Norte pendientes: 2 | Pendientes: 3 | Rango: 5 | Norte ordenadas: 3'
    $countIndex = $out7.IndexOf($countMarker)
    $okIndex = $out7.IndexOf('3.2 OK', $countIndex)
    if ($countIndex -lt 0 -or $okIndex -lt 0) {
        throw '3.2/Paso 8: no se puede aislar el SQL de la consulta básica.'
    }
    $sql8 = $out7.Substring($countIndex, $okIndex - $countIndex)
    Assert-TextContains -Text $sql8 -Tokens @('SELECT','WHERE','ORDER BY','DESC','Estado','Cliente','FechaCreacion') -Context '3.2/Paso 8'
    Write-Host 'PASS 3.2/Paso 8 · SQL real aislado y comprobado'

    $temp9 = New-PedagogicalCopy -Source $root -Name 'm03-3-2-error-orderby'
    Enable-RetoBlock -Path (Join-Path $temp9 $useCaseRel) -Marker 'ERROR CONTROLADO M03 3.2 - SEGUNDO ORDERBY SUSTITUYE EL PRIMERO'
    Invoke-Build31 -Root $temp9 -Context '3.2/Paso 9 build error controlado'
    $out9 = Invoke-Run31 -Root $temp9 -Context '3.2/Paso 9 run error controlado'
    Assert-TextContains -Text $out9 -Tokens @(
        'Error controlado 3.2 OK',
        'Correcto: OF-2024-0003,OF-2024-0004,OF-2024-0001',
        'Segundo OrderBy: OF-2024-0004,OF-2024-0003,OF-2024-0001',
        '3.2 OK'
    ) -Context '3.2/Paso 9'
    Write-Host 'PASS 3.2/Paso 9 · segundo OrderBy sustituye el orden anterior'

    $temp10 = New-PedagogicalCopy -Source $root -Name 'm03-3-2-reto'
    Enable-RetoBlock -Path (Join-Path $temp10 $interfacesRel) -Marker 'RETO M03 3.2 - PUERTO SQL PARAMETRIZADO'
    Enable-RetoBlock -Path (Join-Path $temp10 $reposRel) -Marker 'RETO M03 3.2 - SQL PARAMETRIZADO CLIENTE Y RANGO'
    Enable-RetoBlock -Path (Join-Path $temp10 $useCaseRel) -Marker 'RETO M03 3.2 - CLIENTE RANGO ORDEN Y SQL PARAMETRIZADO'

    Invoke-Build31 -Root $temp10 -Context '3.2/Paso 10 reto build'
    $out10 = Invoke-Run31 -Root $temp10 -Context '3.2/Paso 10 reto run'
    $retoMarker = 'Reto 3.2 OK | Orden: OF-2024-0003,OF-2024-0004,OF-2024-0001'
    Assert-TextContains -Text $out10 -Tokens @($retoMarker,'3.2 OK') -Context '3.2/Paso 10'

    $retoIndex = $out10.IndexOf($retoMarker)
    if ($retoIndex -lt 0) {
        throw '3.2/Paso 10: no se puede aislar la salida SQL del reto.'
    }
    $retoSql = $out10.Substring($retoIndex)
    Assert-TextContains -Text $retoSql -Tokens @('@__cliente','@__desde','@__hasta','WHERE','ORDER BY','DESC','Estado','FechaCreacion') -Context '3.2/Laboratorio'

    $reposReto = Get-Content (Join-Path $temp10 $reposRel) -Raw
    if ($reposReto -match 'desde\.ToString|hasta\.ToString|\+\s*desde|\+\s*hasta') {
        throw '3.2/Laboratorio: se detectó concatenación manual de fechas.'
    }

    Write-Host 'PASS 3.2/Paso 10 + laboratorio adicional'
    Write-Host 'PASS 3.2 COMPLETO'
}


function Test-M033 {
    Write-Section 'M03 · 3.3 Proyecciones con Select y tipos anónimos'

    $root = Join-Path $RepoRoot 'M03\PROYECTO\3.3'
    $useCaseRel = 'src\AceriaData.Application\ProyeccionesUseCase.cs'
    $programRel = 'src\AceriaData.Console\Program.cs'
    $interfacesRel = 'src\AceriaData.Application\Interfaces.cs'
    $reposRel = 'src\AceriaData.Infrastructure\Repositories\Repositories.cs'
    $dtosRel = 'src\AceriaData.Application\Dtos.cs'

    Invoke-Checked -WorkingDirectory $root -FilePath 'dotnet' -ArgumentList @('restore','AceriaData.sln') -Context '3.3/Paso 1 restore' | Out-Null
    Write-Host 'PASS 3.3/Paso 1'

    $migrations = Invoke-Checked -WorkingDirectory $root -FilePath 'dotnet' -ArgumentList @('ef','migrations','list','--project','src/AceriaData.Infrastructure','--startup-project','src/AceriaData.Console','--configuration','Release') -Context '3.3/Paso 2 migrations list'
    Assert-TextContains -Text $migrations -Tokens @('M2_2_12_Architecture') -Context '3.3/Paso 2'
    if ($migrations -match '(?m)^\S*M3_') {
        throw '3.3/Paso 2: aparecen migraciones M3 y la práctica indica que M3 no cambia el esquema.'
    }
    Write-Host 'PASS 3.3/Paso 2'

    $interfaces = Get-Content (Join-Path $root $interfacesRel) -Raw
    $repos = Get-Content (Join-Path $root $reposRel) -Raw
    $dtos = Get-Content (Join-Path $root $dtosRel) -Raw
    foreach ($token in @('ObtenerClientesUnicos','ObtenerResumenes','ObtenerResumenesPorEstado','ObtenerOrdenesConTotales','ObtenerSqlProyeccion')) {
        if (-not $interfaces.Contains($token)) { throw "3.3/Paso 3: falta en el puerto $token." }
        if (-not $repos.Contains($token)) { throw "3.3/Paso 3: falta en Infrastructure $token." }
    }
    foreach ($token in @(
        '.Select(o => o.Cliente).Distinct().OrderBy(c => c).ToList()',
        'new OrdenResumenDto',
        'new OrdenConTotalesDto',
        '.Select(o => new { o.NumeroOrden, o.Cliente }).ToQueryString()'
    )) {
        if (-not $repos.Contains($token)) { throw "3.3/Paso 3: falta la proyección esperada '$token'." }
    }
    foreach ($token in @('public sealed class OrdenResumenDto','NumeroOrden','Cliente','Estado','FechaCreacion')) {
        if (-not $dtos.Contains($token)) { throw "3.3/Paso 3: falta el contrato DTO '$token'." }
    }
    Write-Host 'PASS 3.3/Paso 3'

    $temp4 = New-PedagogicalCopy -Source $root -Name 'm03-3-3-paso4'
    Enable-LineCommentWholeFileCopy -Path (Join-Path $temp4 $useCaseRel) -Marker 'FRAGMENTO PDF M03 3.3 - PASO 4'
    Invoke-Build31 -Root $temp4 -Context '3.3/Paso 4 build variante PDF'
    $out4 = Invoke-Run31 -Root $temp4 -Context '3.3/Paso 4 run variante PDF'
    Assert-TextContains -Text $out4 -Tokens @('=== PROYECCIONES CON SELECT ===','Clientes: Constructora del Este, Constructora del Norte, Constructora del Sur | Resúmenes: 5','3.3 OK') -Context '3.3/Paso 4'
    Write-Host 'PASS 3.3/Paso 4 · copia PDF activada, compilada y ejecutada'

    $temp5 = New-PedagogicalCopy -Source $root -Name 'm03-3-3-paso5'
    Enable-LineCommentWholeFileCopy -Path (Join-Path $temp5 $programRel) -Marker 'FRAGMENTO PDF M03 3.3 - PASO 5'
    Invoke-Build31 -Root $temp5 -Context '3.3/Paso 5 build composition root PDF'
    $out5 = Invoke-Run31 -Root $temp5 -Context '3.3/Paso 5 run composition root PDF'
    Assert-TextContains -Text $out5 -Tokens @('3.3 OK') -Context '3.3/Paso 5'
    Write-Host 'PASS 3.3/Paso 5 · composition root PDF activado'

    Invoke-Build31 -Root $root -Context '3.3/Paso 6 build final'
    $appProject = Get-Content (Join-Path $root 'src\AceriaData.Application\AceriaData.Application.csproj') -Raw
    if ($appProject -match 'EntityFrameworkCore') {
        throw '3.3/Paso 6: Application referencia EntityFrameworkCore.'
    }
    Write-Host 'PASS 3.3/Paso 6'

    $out7 = Invoke-Run31 -Root $root -Context '3.3/Paso 7 run final'
    Assert-TextContains -Text $out7 -Tokens @(
        '=== PROYECCIONES CON SELECT ===',
        'Clientes: Constructora del Este, Constructora del Norte, Constructora del Sur | Resúmenes: 5',
        '3.3 OK'
    ) -Context '3.3/Paso 7'
    Write-Host 'PASS 3.3/Paso 7'

    $marker7 = 'Clientes: Constructora del Este, Constructora del Norte, Constructora del Sur | Resúmenes: 5'
    $start8 = $out7.IndexOf($marker7)
    $end8 = $out7.IndexOf('3.3 OK', $start8)
    if ($start8 -lt 0 -or $end8 -lt 0) { throw '3.3/Paso 8: no se puede aislar el SQL de proyección.' }
    $sql8 = $out7.Substring($start8, $end8 - $start8)
    Assert-TextContains -Text $sql8 -Tokens @('SELECT','FROM','WHERE','ORDER BY','NumeroOrden','Cliente') -Context '3.3/Paso 8'
    $select8 = [regex]::Match($sql8,'(?is)SELECT\s+(.*?)\s+FROM').Groups[1].Value
    if ($select8 -match 'Observaciones|Estado|FechaCreacion') {
        throw "3.3/Paso 8: el SELECT mínimo contiene columnas no proyectadas: $select8"
    }
    Write-Host 'PASS 3.3/Paso 8 · SELECT mínimo aislado y comprobado'

    $temp9 = New-PedagogicalCopy -Source $root -Name 'm03-3-3-error-anonimo'
    Enable-RetoBlock -Path (Join-Path $temp9 $interfacesRel) -Marker 'ERROR CONTROLADO M03 3.3 - TIPO ANONIMO COMO CONTRATO PUBLICO'
    Invoke-ExpectedFailure -WorkingDirectory $temp9 -FilePath 'dotnet' -ArgumentList @('build','AceriaData.sln','--configuration','Release') -Context '3.3/Paso 9 contrato anónimo' -ExpectedTokens @('CS0825') | Out-Null
    Write-Host 'PASS 3.3/Paso 9 · el contrato público con var falla de forma controlada'

    $temp10 = New-PedagogicalCopy -Source $root -Name 'm03-3-3-reto'
    Enable-RetoBlock -Path (Join-Path $temp10 $interfacesRel) -Marker 'RETO M03 3.3 - PUERTO SQL ENTIDAD COMPLETA'
    Enable-RetoBlock -Path (Join-Path $temp10 $reposRel) -Marker 'RETO M03 3.3 - SQL ENTIDAD COMPLETA PARA COMPARAR SELECT'
    Enable-RetoBlock -Path (Join-Path $temp10 $useCaseRel) -Marker 'RETO M03 3.3 - PROYECCION MINIMA Y CLIENTES UNICOS'

    Invoke-Build31 -Root $temp10 -Context '3.3/Paso 10 reto build'
    $out10 = Invoke-Run31 -Root $temp10 -Context '3.3/Paso 10 reto run'
    $retoMarker = 'Reto 3.3 OK | Clientes: Constructora del Este, Constructora del Norte, Constructora del Sur | DTOs: 5'
    Assert-TextContains -Text $out10 -Tokens @($retoMarker,'SQL_MINIMO_INICIO','SQL_MINIMO_FIN','SQL_ENTIDAD_INICIO','SQL_ENTIDAD_FIN','3.3 OK') -Context '3.3/Paso 10'

    $minMatch = [regex]::Match($out10,'(?ms)SQL_MINIMO_INICIO\s*(.*?)\s*SQL_MINIMO_FIN')
    $fullMatch = [regex]::Match($out10,'(?ms)SQL_ENTIDAD_INICIO\s*(.*?)\s*SQL_ENTIDAD_FIN')
    if (-not $minMatch.Success -or -not $fullMatch.Success) {
        throw '3.3/Laboratorio: no se pueden aislar ambos SQL para comparar el shape.'
    }

    $minSql = $minMatch.Groups[1].Value
    $fullSql = $fullMatch.Groups[1].Value
    $minSelect = [regex]::Match($minSql,'(?is)SELECT\s+(.*?)\s+FROM').Groups[1].Value
    $fullSelect = [regex]::Match($fullSql,'(?is)SELECT\s+(.*?)\s+FROM').Groups[1].Value

    Assert-TextContains -Text $minSelect -Tokens @('NumeroOrden','Cliente') -Context '3.3/Laboratorio SELECT mínimo'
    if ($minSelect -match 'Observaciones|Estado|FechaCreacion|Id') {
        throw "3.3/Laboratorio: el SELECT mínimo arrastra columnas de entidad: $minSelect"
    }
    Assert-TextContains -Text $fullSelect -Tokens @('Id','NumeroOrden','Cliente','Estado','FechaCreacion','Observaciones') -Context '3.3/Laboratorio entidad completa'

    Write-Host 'PASS 3.3/Paso 10 + laboratorio adicional'
    Write-Host 'PASS 3.3 COMPLETO'
}


function Test-M034 {
    Write-Section 'M03 · 3.4 Proyecciones a DTOs'

    $root = Join-Path $RepoRoot 'M03\PROYECTO\3.4'
    $useCaseRel = 'src\AceriaData.Application\ProyeccionesDtoUseCase.cs'
    $programRel = 'src\AceriaData.Console\Program.cs'
    $interfacesRel = 'src\AceriaData.Application\Interfaces.cs'
    $reposRel = 'src\AceriaData.Infrastructure\Repositories\Repositories.cs'
    $dtosRel = 'src\AceriaData.Application\Dtos.cs'

    Invoke-Checked -WorkingDirectory $root -FilePath 'dotnet' -ArgumentList @('restore','AceriaData.sln') -Context '3.4/Paso 1 restore' | Out-Null
    Write-Host 'PASS 3.4/Paso 1'

    $migrations = Invoke-Checked -WorkingDirectory $root -FilePath 'dotnet' -ArgumentList @('ef','migrations','list','--project','src/AceriaData.Infrastructure','--startup-project','src/AceriaData.Console','--configuration','Release') -Context '3.4/Paso 2 migrations list'
    Assert-TextContains -Text $migrations -Tokens @('M2_2_12_Architecture') -Context '3.4/Paso 2'
    if ($migrations -match '(?m)^\S*M3_') {
        throw '3.4/Paso 2: aparecen migraciones M3 y la práctica indica que M3 no cambia el esquema.'
    }
    Write-Host 'PASS 3.4/Paso 2'

    $interfaces = Get-Content (Join-Path $root $interfacesRel) -Raw
    $repos = Get-Content (Join-Path $root $reposRel) -Raw
    $dtos = Get-Content (Join-Path $root $dtosRel) -Raw

    foreach ($token in @('ObtenerOrdenesConPlanchas','ObtenerOrdenesConDetalle','ObtenerOrdenesCompletas','ObtenerSqlProyeccionNavegacion')) {
        if (-not $interfaces.Contains($token)) { throw "3.4/Paso 3: falta en el puerto $token." }
        if (-not $repos.Contains($token)) { throw "3.4/Paso 3: falta en Infrastructure $token." }
    }
    foreach ($token in @(
        'List<PlanchaDto> Planchas',
        'DetalleDto? Detalle',
        'public sealed class OrdenCompletaDto'
    )) {
        if (-not $dtos.Contains($token)) { throw "3.4/Paso 3: falta el shape DTO '$token'." }
    }
    foreach ($token in @(
        'Detalle = o.Detalle == null ? null : new DetalleDto',
        'Planchas = o.Planchas.Select(p => new PlanchaDto',
        '.Select(o => new { o.NumeroOrden, Planchas = o.Planchas.Select'
    )) {
        if (-not $repos.Contains($token)) { throw "3.4/Paso 3: falta la proyección esperada '$token'." }
    }
    Write-Host 'PASS 3.4/Paso 3'

    $temp4 = New-PedagogicalCopy -Source $root -Name 'm03-3-4-paso4'
    Enable-LineCommentWholeFileCopy -Path (Join-Path $temp4 $useCaseRel) -Marker 'FRAGMENTO PDF M03 3.4 - PASO 4'
    Invoke-Build31 -Root $temp4 -Context '3.4/Paso 4 build variante PDF'
    $out4 = Invoke-Run31 -Root $temp4 -Context '3.4/Paso 4 run variante PDF'
    Assert-TextContains -Text $out4 -Tokens @('=== PROYECCIONES A DTOs ===','OF-2024-0001 -> planchas: 2, detalle: C 0.20%; Mn 0.80%','3.4 OK') -Context '3.4/Paso 4'
    Write-Host 'PASS 3.4/Paso 4 · copia PDF activada, compilada y ejecutada'

    $temp5 = New-PedagogicalCopy -Source $root -Name 'm03-3-4-paso5'
    Enable-LineCommentWholeFileCopy -Path (Join-Path $temp5 $programRel) -Marker 'FRAGMENTO PDF M03 3.4 - PASO 5'
    Invoke-Build31 -Root $temp5 -Context '3.4/Paso 5 build composition root PDF'
    $out5 = Invoke-Run31 -Root $temp5 -Context '3.4/Paso 5 run composition root PDF'
    Assert-TextContains -Text $out5 -Tokens @('3.4 OK') -Context '3.4/Paso 5'
    Write-Host 'PASS 3.4/Paso 5 · composition root PDF activado'

    Invoke-Build31 -Root $root -Context '3.4/Paso 6 build final'
    $appProject = Get-Content (Join-Path $root 'src\AceriaData.Application\AceriaData.Application.csproj') -Raw
    if ($appProject -match 'EntityFrameworkCore') {
        throw '3.4/Paso 6: Application referencia EntityFrameworkCore.'
    }
    Write-Host 'PASS 3.4/Paso 6'

    $out7 = Invoke-Run31 -Root $root -Context '3.4/Paso 7 run final'
    Assert-TextContains -Text $out7 -Tokens @(
        '=== PROYECCIONES A DTOs ===',
        'OF-2024-0001 -> planchas: 2, detalle: C 0.20%; Mn 0.80%',
        '3.4 OK'
    ) -Context '3.4/Paso 7'
    Write-Host 'PASS 3.4/Paso 7'

    $marker7 = 'OF-2024-0001 -> planchas: 2, detalle: C 0.20%; Mn 0.80%'
    $start8 = $out7.IndexOf($marker7)
    $end8 = $out7.IndexOf('3.4 OK', $start8)
    if ($start8 -lt 0 -or $end8 -lt 0) { throw '3.4/Paso 8: no se puede aislar el SQL de navegación.' }
    $sql8 = $out7.Substring($start8, $end8 - $start8)
    Assert-TextContains -Text $sql8 -Tokens @('SELECT','NumeroOrden','Espesor','Peso') -Context '3.4/Paso 8'
    Write-Host 'PASS 3.4/Paso 8 · SQL de proyección de navegación comprobado'

    $temp9 = New-PedagogicalCopy -Source $root -Name 'm03-3-4-error-null'
    Enable-RetoBlock -Path (Join-Path $temp9 $useCaseRel) -Marker 'ERROR CONTROLADO M03 3.4 - RELACION OPCIONAL SIN COMPROBAR NULL'
    Invoke-Build31 -Root $temp9 -Context '3.4/Paso 9 build error controlado'
    Invoke-ExpectedFailure -WorkingDirectory $temp9 -FilePath 'dotnet' -ArgumentList @('run','--project','src/AceriaData.Console/AceriaData.Console.csproj','--configuration','Release','--no-build') -Context '3.4/Paso 9 relación opcional' -ExpectedTokens @('NullReferenceException') | Out-Null
    Write-Host 'PASS 3.4/Paso 9 · NullReferenceException demostrada de forma controlada'

    $temp10 = New-PedagogicalCopy -Source $root -Name 'm03-3-4-reto'
    Enable-RetoBlock -Path (Join-Path $temp10 $useCaseRel) -Marker 'RETO M03 3.4 - DTO CON PLANCHAS Y DETALLE OPCIONAL'
    Invoke-Build31 -Root $temp10 -Context '3.4/Paso 10 reto build'
    $out10 = Invoke-Run31 -Root $temp10 -Context '3.4/Paso 10 reto run'
    Assert-TextContains -Text $out10 -Tokens @(
        'Reto 3.4 OK | OF-2024-0001 planchas: 2 | OF-2024-0005 detalle: null',
        '3.4 OK'
    ) -Context '3.4/Paso 10'

    if ($dtos -match 'OrdenFabricacion|PlanchaAcero|DetalleOrden') {
        throw '3.4/Laboratorio: los DTO de Application exponen entidades de dominio.'
    }

    Write-Host 'PASS 3.4/Paso 10 + laboratorio adicional'
    Write-Host 'PASS 3.4 COMPLETO'
}


function Test-M035 {
    Write-Section 'M03 · 3.5 Consultas de agregación'

    $root = Join-Path $RepoRoot 'M03\PROYECTO\3.5'
    $useCaseRel = 'src\AceriaData.Application\AgregacionesUseCase.cs'
    $programRel = 'src\AceriaData.Console\Program.cs'
    $interfacesRel = 'src\AceriaData.Application\Interfaces.cs'
    $reposRel = 'src\AceriaData.Infrastructure\Repositories\Repositories.cs'

    Invoke-Checked -WorkingDirectory $root -FilePath 'dotnet' -ArgumentList @('restore','AceriaData.sln') -Context '3.5/Paso 1 restore' | Out-Null
    Write-Host 'PASS 3.5/Paso 1'

    $migrations = Invoke-Checked -WorkingDirectory $root -FilePath 'dotnet' -ArgumentList @('ef','migrations','list','--project','src/AceriaData.Infrastructure','--startup-project','src/AceriaData.Console','--configuration','Release') -Context '3.5/Paso 2 migrations list'
    Assert-TextContains -Text $migrations -Tokens @('M2_2_12_Architecture') -Context '3.5/Paso 2'
    if ($migrations -match '(?m)^\S*M3_') {
        throw '3.5/Paso 2: aparecen migraciones M3 y la práctica indica que M3 no cambia el esquema.'
    }
    Write-Host 'PASS 3.5/Paso 2'

    $interfaces = Get-Content (Join-Path $root $interfacesRel) -Raw
    $repos = Get-Content (Join-Path $root $reposRel) -Raw
    foreach ($token in @(
        'ContarOrdenes',
        'ContarOrdenesPorEstado',
        'ExisteAlgunaOrden',
        'TodasLasOrdenesTienenEstado',
        'ObtenerPesoTotalDePlanchas',
        'ObtenerPesoPromedioDePlanchas',
        'ObtenerPesoMinimoDePlanchas',
        'ObtenerPesoMaximoDePlanchas',
        'ObtenerResumenPorCliente',
        'ObtenerResumenPorEstado',
        'ObtenerResumenMensual'
    )) {
        if (-not $interfaces.Contains($token)) { throw "3.5/Paso 3: falta en el puerto $token." }
        if (-not $repos.Contains($token)) { throw "3.5/Paso 3: falta en Infrastructure $token." }
    }
    foreach ($token in @(
        '.Count()',
        '.Count(o => o.Estado == estado)',
        '.Any()',
        '.All(o => o.Estado != "")',
        '.Sum() ?? 0m',
        '.Average() ?? 0m',
        '.Min() ?? 0m',
        '.Max() ?? 0m',
        '.GroupBy(o => o.Cliente)',
        '.GroupBy(o => o.Estado)',
        '.GroupBy(o => new { o.FechaCreacion.Year, o.FechaCreacion.Month })'
    )) {
        if (-not $repos.Contains($token)) { throw "3.5/Paso 3: falta la agregación esperada '$token'." }
    }
    Write-Host 'PASS 3.5/Paso 3'

    $temp4 = New-PedagogicalCopy -Source $root -Name 'm03-3-5-paso4'
    Enable-LineCommentWholeFileCopy -Path (Join-Path $temp4 $useCaseRel) -Marker 'FRAGMENTO PDF M03 3.5 - PASO 4'
    Invoke-Build31 -Root $temp4 -Context '3.5/Paso 4 build variante PDF'
    $out4 = Invoke-Run31 -Root $temp4 -Context '3.5/Paso 4 run variante PDF'
    Assert-TextContains -Text $out4 -Tokens @('=== AGREGACIONES ===','Órdenes: 5 | Pendientes: 3','3.5 OK') -Context '3.5/Paso 4'
    Write-Host 'PASS 3.5/Paso 4 · copia PDF activada, compilada y ejecutada'

    $temp5 = New-PedagogicalCopy -Source $root -Name 'm03-3-5-paso5'
    Enable-LineCommentWholeFileCopy -Path (Join-Path $temp5 $programRel) -Marker 'FRAGMENTO PDF M03 3.5 - PASO 5'
    Invoke-Build31 -Root $temp5 -Context '3.5/Paso 5 build composition root PDF'
    $out5 = Invoke-Run31 -Root $temp5 -Context '3.5/Paso 5 run composition root PDF'
    Assert-TextContains -Text $out5 -Tokens @('3.5 OK') -Context '3.5/Paso 5'
    Write-Host 'PASS 3.5/Paso 5 · composition root PDF activado'

    Invoke-Build31 -Root $root -Context '3.5/Paso 6 build final'
    $appProject = Get-Content (Join-Path $root 'src\AceriaData.Application\AceriaData.Application.csproj') -Raw
    if ($appProject -match 'EntityFrameworkCore') {
        throw '3.5/Paso 6: Application referencia EntityFrameworkCore.'
    }
    Write-Host 'PASS 3.5/Paso 6'

    $out7 = Invoke-Run31 -Root $root -Context '3.5/Paso 7 run final'
    Assert-TextContains -Text $out7 -Tokens @('=== AGREGACIONES ===','Órdenes: 5 | Pendientes: 3','3.5 OK') -Context '3.5/Paso 7'
    Write-Host 'PASS 3.5/Paso 7'

    $temp8 = New-PedagogicalCopy -Source $root -Name 'm03-3-5-sql'
    Enable-RetoBlock -Path (Join-Path $temp8 $interfacesRel) -Marker 'APOYO M03 3.5 - METODOS PEDAGOGICOS DE AGREGACION'
    Enable-RetoBlock -Path (Join-Path $temp8 $reposRel) -Marker 'APOYO M03 3.5 - METODOS PEDAGOGICOS DE AGREGACION'
    Enable-RetoBlock -Path (Join-Path $temp8 $useCaseRel) -Marker 'INSPECCION M03 3.5 - SQL AGREGADOS'
    Invoke-Build31 -Root $temp8 -Context '3.5/Paso 8 build inspección SQL'
    $out8 = Invoke-Run31 -Root $temp8 -Context '3.5/Paso 8 run inspección SQL'

    $agg8 = [regex]::Match($out8,'(?ms)SQL_AGREGADOS_INICIO\s*(.*?)\s*SQL_AGREGADOS_FIN')
    $monthly8 = [regex]::Match($out8,'(?ms)SQL_MENSUAL_INICIO\s*(.*?)\s*SQL_MENSUAL_FIN')
    if (-not $agg8.Success -or -not $monthly8.Success) {
        throw '3.5/Paso 8: no se puede aislar el SQL pedagógico de agregados.'
    }
    Assert-TextContains -Text $agg8.Groups[1].Value -Tokens @('COUNT(','SUM(','AVG(','MIN(','MAX(') -Context '3.5/Paso 8 agregados SQL'
    Assert-TextContains -Text $monthly8.Groups[1].Value -Tokens @('COUNT(','GROUP BY') -Context '3.5/Paso 8 resumen mensual SQL'
    Write-Host 'PASS 3.5/Paso 8 · COUNT/SUM/AVG/MIN/MAX/GROUP BY comprobados en SQL real'

    $temp9 = New-PedagogicalCopy -Source $root -Name 'm03-3-5-error-vacio'
    Enable-RetoBlock -Path (Join-Path $temp9 $interfacesRel) -Marker 'APOYO M03 3.5 - METODOS PEDAGOGICOS DE AGREGACION'
    Enable-RetoBlock -Path (Join-Path $temp9 $reposRel) -Marker 'APOYO M03 3.5 - METODOS PEDAGOGICOS DE AGREGACION'
    Enable-RetoBlock -Path (Join-Path $temp9 $useCaseRel) -Marker 'ERROR CONTROLADO M03 3.5 - AGREGACION VACIA SIN ESTRATEGIA'
    Invoke-Build31 -Root $temp9 -Context '3.5/Paso 9 build error controlado'
    $out9 = Invoke-Run31 -Root $temp9 -Context '3.5/Paso 9 run error controlado'
    Assert-TextContains -Text $out9 -Tokens @('Error controlado 3.5 OK | Seguro: 0 | Sin estrategia:','3.5 OK') -Context '3.5/Paso 9'
    Write-Host 'PASS 3.5/Paso 9 · estrategia explícita para conjunto vacío demostrada'

    $temp10 = New-PedagogicalCopy -Source $root -Name 'm03-3-5-reto'
    Enable-RetoBlock -Path (Join-Path $temp10 $interfacesRel) -Marker 'APOYO M03 3.5 - METODOS PEDAGOGICOS DE AGREGACION'
    Enable-RetoBlock -Path (Join-Path $temp10 $reposRel) -Marker 'APOYO M03 3.5 - METODOS PEDAGOGICOS DE AGREGACION'
    Enable-RetoBlock -Path (Join-Path $temp10 $useCaseRel) -Marker 'RETO M03 3.5 - RESUMEN MENSUAL Y SQL DE AGREGADOS'
    Invoke-Build31 -Root $temp10 -Context '3.5/Paso 10 reto build'
    $out10 = Invoke-Run31 -Root $temp10 -Context '3.5/Paso 10 reto run'
    Assert-TextContains -Text $out10 -Tokens @(
        'Reto 3.5 OK | Meses: 5 | Norte: 3 | Pendiente: 3',
        'RETO_SQL_AGREGADOS_INICIO',
        'RETO_SQL_MENSUAL_INICIO',
        '3.5 OK'
    ) -Context '3.5/Paso 10'

    $agg10 = [regex]::Match($out10,'(?ms)RETO_SQL_AGREGADOS_INICIO\s*(.*?)\s*RETO_SQL_AGREGADOS_FIN')
    $monthly10 = [regex]::Match($out10,'(?ms)RETO_SQL_MENSUAL_INICIO\s*(.*?)\s*RETO_SQL_MENSUAL_FIN')
    if (-not $agg10.Success -or -not $monthly10.Success) {
        throw '3.5/Laboratorio: no se puede aislar el SQL del reto.'
    }
    Assert-TextContains -Text $agg10.Groups[1].Value -Tokens @('COUNT(','SUM(','AVG(','MIN(','MAX(') -Context '3.5/Laboratorio agregados'
    Assert-TextContains -Text $monthly10.Groups[1].Value -Tokens @('COUNT(','GROUP BY') -Context '3.5/Laboratorio mensual'

    Write-Host 'PASS 3.5/Paso 10 + laboratorio adicional'
    Write-Host 'PASS 3.5 COMPLETO'
}

if ($Suite -in @('all','inventory')) {
    Test-M03Inventory
}

if ($Suite -in @('all','3.1')) {
    Test-M031
}

if ($Suite -in @('all','3.2')) {
    Test-M032
}

if ($Suite -in @('all','3.3')) {
    Test-M033
}

if ($Suite -in @('all','3.4')) {
    Test-M034
}

if ($Suite -in @('all','3.5')) {
    Test-M035
}

Write-Section 'M03 · RESULTADO'
Write-Host "PASS suite '$Suite'."
