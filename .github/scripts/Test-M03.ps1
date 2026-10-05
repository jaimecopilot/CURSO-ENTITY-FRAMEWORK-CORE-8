param(
    [ValidateSet('all','inventory','3.1')]
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

    if ($out10 -notmatch 'WHERE') {
        throw '3.1/Paso 10: el SQL del reto no contiene WHERE.'
    }
    if ($out10 -match '\[o\]\.\[Observaciones\]') {
        throw '3.1/Laboratorio: la proyección SQL no es mínima; incluye Observaciones.'
    }

    Write-Host 'PASS 3.1/Paso 10 + laboratorio adicional'
    Write-Host 'PASS 3.1 COMPLETO'
}

if ($Suite -in @('all','inventory')) {
    Test-M03Inventory
}

if ($Suite -in @('all','3.1')) {
    Test-M031
}

Write-Section 'M03 · RESULTADO'
Write-Host "PASS suite '$Suite'."
