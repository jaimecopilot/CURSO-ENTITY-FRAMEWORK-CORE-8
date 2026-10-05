param(
    [ValidateSet('all','coverage','variants','retos','operational')]
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

function Get-PointRoot([string]$Point) {
    return Join-Path $RepoRoot ("M02\PROYECTO\" + $Point)
}

function Invoke-Build([string]$Root,[string]$Context) {
    Invoke-Checked -WorkingDirectory $Root -FilePath 'dotnet' -ArgumentList @('build','AceriaData.sln','--configuration','Release') -Context $Context | Out-Null
}

function Invoke-RunSingleProject([string]$Root,[string]$Expected,[string]$Context) {
    $text = Invoke-Checked -WorkingDirectory $Root -FilePath 'dotnet' -ArgumentList @('run','--project','AceriaData.Console.csproj','--configuration','Release','--no-build') -Context $Context
    Assert-TextContains -Text $text -Tokens @($Expected) -Context $Context
    return $text
}

function Test-Coverage {
    Write-Section 'M02 · cobertura canónica de 80 pasos'

    $practicePath = Join-Path $RepoRoot 'M02\PRACTICA\M02_PRACTICA.md'
    $practice = Get-Content $practicePath -Raw

    $expected = [ordered]@{
        '2.1' = 6
        '2.2' = 6
        '2.3' = 6
        '2.4' = 6
        '2.5' = 6
        '2.6' = 6
        '2.7' = 6
        '2.8' = 6
        '2.9' = 6
        '2.10' = 6
        '2.11' = 12
        '2.12' = 8
    }

    $total = 0
    foreach ($entry in $expected.GetEnumerator()) {
        $point = $entry.Key
        $count = [int]$entry.Value
        $pattern = '(?ms)^## Punto ' + [regex]::Escape($point) + '\b.*?(?=^## Punto 2\.\d+\b|\z)'
        $section = [regex]::Match($practice,$pattern).Value
        if ([string]::IsNullOrWhiteSpace($section)) {
            throw "No se encuentra el punto canónico $point."
        }

        $steps = [regex]::Matches($section,'(?m)^### Paso (\d+):')
        if ($steps.Count -ne $count) {
            throw "$point contiene $($steps.Count) pasos y debe contener $count."
        }

        for ($n = 1; $n -le $count; $n++) {
            if ($section -notmatch ('(?m)^### Paso ' + $n + ':')) {
                throw "$point no contiene Paso $n."
            }
            Write-Host "PASS trazabilidad: PDF $point / Paso $n"
            $total++
        }
    }

    if ($total -ne 80) {
        throw "El inventario suma $total pasos y debe sumar exactamente 80."
    }

    Write-Host "PASS cobertura M02: 80/80 pasos canónicos."
}

function Test-ProgramFragments {
    Write-Section 'M02 · variantes C# comentadas 2.1-2.10'

    $cases = @(
        @{ Point='2.1'; Markers=@('FRAGMENTO PDF M02 2.1 - PASO 3'); Active=@('ACTIVO FINAL M02 2.1 PASO 3'); Expected='Entidad:' },
        @{ Point='2.2'; Markers=@('FRAGMENTO PDF M02 2.2 - PASO 3A','FRAGMENTO PDF M02 2.2 - PASO 3B'); Active=@('ACTIVO FINAL M02 2.2 PASO 3A'); Expected='2.2 OK' },
        @{ Point='2.3'; Markers=@('FRAGMENTO PDF M02 2.3 - PASO 3'); Active=@(); Expected='2.3 OK' },
        @{ Point='2.4'; Markers=@('FRAGMENTO PDF M02 2.4 - PASO 3'); Active=@(); Expected='2.4 OK' },
        @{ Point='2.5'; Markers=@('FRAGMENTO PDF M02 2.5 - PASO 3'); Active=@(); Expected='2.5 OK' },
        @{ Point='2.6'; Markers=@('FRAGMENTO PDF M02 2.6 - PASO 3'); Active=@('ACTIVO FINAL M02 2.6 PASO 3'); Expected='2.6 OK' },
        @{ Point='2.7'; Markers=@('FRAGMENTO PDF M02 2.7 - PASO 3'); Active=@(); Expected='2.7 OK' },
        @{ Point='2.8'; Markers=@('FRAGMENTO PDF M02 2.8 - PASO 3'); Active=@(); Expected='2.8 OK' },
        @{ Point='2.9'; Markers=@('FRAGMENTO PDF M02 2.9 - PASO 3'); Active=@(); Expected='2.9 OK' },
        @{ Point='2.10'; Markers=@('FRAGMENTO PDF M02 2.10 - PASO 3A','FRAGMENTO PDF M02 2.10 - PASO 3B'); Active=@('ACTIVO FINAL M02 2.10 PASO 3B'); Expected='2.10 OK' }
    )

    foreach ($case in $cases) {
        $source = Get-PointRoot $case.Point
        $temp = New-PedagogicalCopy -Source $source -Name ("m02-" + $case.Point.Replace('.','-') + "-pdf-variant")
        $program = Join-Path $temp 'Program.cs'

        Enable-BlockFragment -Path $program -Markers $case.Markers -ActiveRegions $case.Active
        Invoke-Build -Root $temp -Context ("$($case.Point)/Paso 3 build de variante descomentada")
        Invoke-RunSingleProject -Root $temp -Expected $case.Expected -Context ("$($case.Point)/Paso 3 ejecución de variante descomentada") | Out-Null

        Write-Host "PASS variante descomentada: $($case.Point)/Paso 3"
    }
}

function Test-Retos {
    Write-Section 'M02 · retos comentados activados'

    $cases = @(
        @{ Point='2.1'; Marker='RETO 2.1 - DELETEBEHAVIOR'; Expected=@('DeleteBehavior:') },
        @{ Point='2.2'; Marker='RETO 2.2 - COMPROBAR PROPIEDADES'; Expected=@('Observaciones nullable: True','Peso precision/scale: 18/3') },
        @{ Point='2.3'; Marker='RETO 2.3 - DOS PLANCHAS CON INCLUDE'; Expected=@('Reto 2.3 planchas: 2') },
        @{ Point='2.4'; Marker='RETO 2.4 - CERTIFICADO ÚNICO'; Expected=@('Reto 2.4: segundo certificado rechazado') },
        @{ Point='2.5'; Marker='RETO 2.5 - DOS ALEACIONES CON THENINCLUDE'; Expected=@('Reto 2.5 aleaciones: 2','A1018 | Cantidad: 1000.250','A4140 | Cantidad: 500.750') },
        @{ Point='2.6'; Marker='RETO 2.6 - ANNOTATIONS VS FLUENT API'; Expected=@('Modelo efectivo OrdenesFabricacion','Modelo efectivo NumeroOrden MaxLength: 50') },
        @{ Point='2.7'; Marker='RETO 2.7 - MAXLENGTH DE ANNOTATION A FLUENT'; Expected=@('Reto 2.7 MaxLength Cliente: 200'); RemoveClientAnnotation=$true },
        @{ Point='2.8'; Marker='RETO 2.8 - NUMEROORDEN DUPLICADO'; Expected=@('Reto 2.8: NumeroOrden duplicado rechazado') },
        @{ Point='2.9'; Marker='RETO 2.9 - ESPESOR NEGATIVO'; Expected=@('Reto 2.9: Espesor negativo rechazado') },
        @{ Point='2.10'; Marker='RETO 2.10 - ORDEN CANCELADA E IGNOREQUERYFILTERS'; Expected=@('Reto 2.10','Visibles: 1','Sin filtro: 3') }
    )

    foreach ($case in $cases) {
        $source = Get-PointRoot $case.Point
        $temp = New-PedagogicalCopy -Source $source -Name ("m02-" + $case.Point.Replace('.','-') + "-reto")
        $program = Join-Path $temp 'Program.cs'

        if ($case.ContainsKey('RemoveClientAnnotation') -and $case.RemoveClientAnnotation) {
            $text = Get-Content $program -Raw
            $pattern = '(?m)^\s*\[MaxLength\(200\)\]\s*\r?\n(?=\s*public string Cliente)'
            if ($text -notmatch $pattern) {
                throw "2.7/reto: no se encuentra [MaxLength(200)] activo sobre Cliente."
            }
            $text = [regex]::Replace($text,$pattern,'',1)
            Set-Content $program -Value $text -Encoding utf8
        }

        Enable-RetoBlock -Path $program -Marker $case.Marker
        Invoke-Build -Root $temp -Context ("$($case.Point)/reto build")

        $text = Invoke-Checked -WorkingDirectory $temp -FilePath 'dotnet' -ArgumentList @('run','--project','AceriaData.Console.csproj','--configuration','Release','--no-build') -Context ("$($case.Point)/reto run")
        Assert-TextContains -Text $text -Tokens $case.Expected -Context ("$($case.Point)/reto")

        Write-Host "PASS reto activado: $($case.Point)"
    }

    $source212 = Get-PointRoot '2.12'
    $temp212 = New-PedagogicalCopy -Source $source212 -Name 'm02-2-12-reto'
    $program212 = Join-Path $temp212 'src\AceriaData.Console\Program.cs'
    Enable-RetoBlock -Path $program212 -Marker 'RETO 2.12 - CREAR ORDEN DESDE USE CASE'

    $domainProject = Get-Content (Join-Path $temp212 'src\AceriaData.Domain\AceriaData.Domain.csproj') -Raw
    $applicationProject = Get-Content (Join-Path $temp212 'src\AceriaData.Application\AceriaData.Application.csproj') -Raw
    $applicationCode = (Get-ChildItem (Join-Path $temp212 'src\AceriaData.Application') -Filter *.cs -Recurse | Get-Content -Raw) -join [Environment]::NewLine

    if ($domainProject -match 'EntityFrameworkCore') {
        throw '2.12/reto: Domain referencia EF Core.'
    }
    if ($applicationProject -match 'EntityFrameworkCore' -or $applicationCode -match 'Microsoft\.EntityFrameworkCore') {
        throw '2.12/reto: Application referencia EF Core.'
    }

    Invoke-Build -Root $temp212 -Context '2.12/reto build'
    $out212 = Invoke-Checked -WorkingDirectory $temp212 -FilePath 'dotnet' -ArgumentList @('run','--project','src/AceriaData.Console/AceriaData.Console.csproj','--configuration','Release','--no-build') -Context '2.12/reto run'
    Assert-TextContains -Text $out212 -Tokens @('Reto 2.12','OF-M2-HEX-RETO','Cliente Reto Arquitectura','Total: 2') -Context '2.12/reto'
    Write-Host 'PASS reto activado: 2.12'
}

function Test-MigrationsAndFinalStates21To210 {
    Write-Section 'M02 · pasos operativos y estados finales 2.1-2.10'

    $expectedMigrations = [ordered]@{
        '2.1' = 'AddEstadoOrden'
        '2.2' = 'M2_2_2'
        '2.3' = 'M2_2_3'
        '2.4' = 'M2_2_4'
        '2.5' = 'M2_2_5'
        '2.6' = 'M2_2_6'
        '2.7' = 'M2_2_7'
        '2.8' = 'M2_2_8'
        '2.9' = 'M2_2_9'
        '2.10' = 'M2_2_10'
    }

    for ($n = 1; $n -le 10; $n++) {
        $point = "2.$n"
        $root = Get-PointRoot $point

        Invoke-Checked -WorkingDirectory $root -FilePath 'dotnet' -ArgumentList @('restore','AceriaData.sln') -Context "$point/Paso 1 restore" | Out-Null
        Invoke-Build -Root $root -Context "$point/Paso 1 build"

        $list = Invoke-Checked -WorkingDirectory $root -FilePath 'dotnet' -ArgumentList @('ef','migrations','list','--configuration','Release') -Context "$point migrations list"
        Assert-TextContains -Text $list -Tokens @($expectedMigrations[$point]) -Context "$point historial"

        Invoke-Checked -WorkingDirectory $root -FilePath 'dotnet' -ArgumentList @('ef','migrations','has-pending-model-changes','--configuration','Release') -Context "$point cambios pendientes" | Out-Null

        if ($point -eq '2.1') {
            if ($list.Contains('M2_2_1')) {
                throw '2.1/Paso 4: existe una migración M2_2_1 y la práctica exige no crearla.'
            }
            $run = Invoke-Checked -WorkingDirectory $root -FilePath 'dotnet' -ArgumentList @('run','--project','AceriaData.Console.csproj','--configuration','Release','--no-build') -Context '2.1/Paso 5 run'
            Assert-TextContains -Text $run -Tokens @('--- MODELO EF CORE 2.1 ---','Entidad: OrdenFabricacion') -Context '2.1/Paso 5'
            Write-Host 'PASS operacional 2.1 / Pasos 1-6'
            continue
        }

        $previous = if ($point -eq '2.2') { Get-PointRoot '2.1' } else { Get-PointRoot ("2." + ($n - 1)) }
        $temp = New-PedagogicalCopy -Source $previous -Name ("m02-" + $point.Replace('.','-') + "-migration-rebuild")
        Copy-Item (Join-Path $root 'Program.cs') (Join-Path $temp 'Program.cs') -Force

        Invoke-Checked -WorkingDirectory $temp -FilePath 'dotnet' -ArgumentList @('restore','AceriaData.sln') -Context "$point/Paso 4 restore previo" | Out-Null
        Invoke-Build -Root $temp -Context "$point/Paso 4 build previo"
        Invoke-Checked -WorkingDirectory $temp -FilePath 'dotnet' -ArgumentList @('ef','migrations','add',$expectedMigrations[$point],'--configuration','Release') -Context "$point/Paso 4 migrations add" | Out-Null

        $generated = @(Get-ChildItem (Join-Path $temp 'Migrations') -Filter ("*_" + $expectedMigrations[$point] + ".cs") | Where-Object { $_.Name -notmatch '\.Designer\.cs$' })
        if ($generated.Count -ne 1) {
            throw "$point/Paso 4: no se generó exactamente una migración $($expectedMigrations[$point])."
        }

        Invoke-Checked -WorkingDirectory $temp -FilePath 'dotnet' -ArgumentList @('ef','database','drop','--force','--configuration','Release') -Context "$point/Paso 4 limpiar base" | Out-Null
        Invoke-Checked -WorkingDirectory $temp -FilePath 'dotnet' -ArgumentList @('ef','database','update','--configuration','Release') -Context "$point/Paso 4 database update" | Out-Null
        Invoke-Checked -WorkingDirectory $temp -FilePath 'dotnet' -ArgumentList @('ef','migrations','has-pending-model-changes','--configuration','Release') -Context "$point/Paso 4 snapshot reconstruido" | Out-Null

        $run = Invoke-Checked -WorkingDirectory $root -FilePath 'dotnet' -ArgumentList @('run','--project','AceriaData.Console.csproj','--configuration','Release','--no-build') -Context "$point/Paso 5 run"
        Assert-TextContains -Text $run -Tokens @("$point OK") -Context "$point/Paso 5"

        Write-Host "PASS operacional $point / Pasos 1-6"
    }
}

function Test-Point211 {
    Write-Section 'M02 · 2.11 ciclo completo Soft Delete'

    $root = Get-PointRoot '2.11'
    Invoke-Checked -WorkingDirectory $root -FilePath 'dotnet' -ArgumentList @('restore','AceriaData.sln') -Context '2.11/Paso 1 restore' | Out-Null
    Invoke-Build -Root $root -Context '2.11/Paso 1 build'

    $temp2 = New-PedagogicalCopy -Source $root -Name 'm02-2-11-step2'
    $program2 = Join-Path $temp2 'Program.cs'
    $markers2 = @(
        'FRAGMENTO PDF M02 2.11 - PASO 2 - OrdenFabricacion',
        'FRAGMENTO PDF M02 2.11 - PASO 2 - PlanchaAcero',
        'FRAGMENTO PDF M02 2.11 - PASO 2 - Aleacion',
        'FRAGMENTO PDF M02 2.11 - PASO 2 - EstadoOrden'
    )
    $regions2 = @(
        'ACTIVO FINAL M02 2.11 PASO 2 OrdenFabricacion',
        'ACTIVO FINAL M02 2.11 PASO 2 PlanchaAcero',
        'ACTIVO FINAL M02 2.11 PASO 2 Aleacion',
        'ACTIVO FINAL M02 2.11 PASO 2 EstadoOrden'
    )
    Enable-BlockFragment -Path $program2 -Markers $markers2 -ActiveRegions $regions2
    Invoke-Build -Root $temp2 -Context '2.11/Paso 2 build de variante'
    $out2 = Invoke-Checked -WorkingDirectory $temp2 -FilePath 'dotnet' -ArgumentList @('run','--project','AceriaData.Console.csproj','--configuration','Release','--no-build') -Context '2.11/Paso 2 run de variante'
    Assert-TextContains -Text $out2 -Tokens @('2.11 OK') -Context '2.11/Paso 2'
    Write-Host 'PASS 2.11/Paso 2 variante Soft Delete'

    $temp3 = New-PedagogicalCopy -Source (Get-PointRoot '2.10') -Name 'm02-2-11-step3'
    Copy-Item (Join-Path $root 'Program.cs') (Join-Path $temp3 'Program.cs') -Force
    Invoke-Build -Root $temp3 -Context '2.11/Paso 3 build previo'
    Invoke-Checked -WorkingDirectory $temp3 -FilePath 'dotnet' -ArgumentList @('ef','migrations','add','M2_2_11','--configuration','Release') -Context '2.11/Paso 3 migrations add' | Out-Null
    $generated = @(Get-ChildItem (Join-Path $temp3 'Migrations') -Filter '*_M2_2_11.cs' | Where-Object { $_.Name -notmatch '\.Designer\.cs$' })
    if ($generated.Count -ne 1) {
        throw '2.11/Paso 3: no se generó exactamente una migración M2_2_11.'
    }
    Write-Host 'PASS 2.11/Paso 3 migración incremental'

    $temp4 = New-PedagogicalCopy -Source $root -Name 'm02-2-11-step4'
    $migration4 = @(Get-ChildItem (Join-Path $temp4 'Migrations') -Filter '*_M2_2_11.cs' | Where-Object { $_.Name -notmatch '\.Designer\.cs$' })
    if ($migration4.Count -ne 1) {
        throw '2.11/Paso 4: no se encuentra una única migración canónica.'
    }
    Enable-LineCommentWholeFileCopy -Path $migration4[0].FullName -Marker 'FRAGMENTO PDF M02 2.11 - PASO 4'
    Invoke-Build -Root $temp4 -Context '2.11/Paso 4 migración descomentada'
    Invoke-Checked -WorkingDirectory $temp4 -FilePath 'dotnet' -ArgumentList @('ef','migrations','has-pending-model-changes','--configuration','Release') -Context '2.11/Paso 4 modelo' | Out-Null
    Write-Host 'PASS 2.11/Paso 4 migración comentada activada'

    Invoke-Checked -WorkingDirectory $root -FilePath 'dotnet' -ArgumentList @('ef','database','drop','--force','--configuration','Release') -Context '2.11 preparar base limpia' | Out-Null
    Invoke-Checked -WorkingDirectory $root -FilePath 'dotnet' -ArgumentList @('ef','database','update','--configuration','Release') -Context '2.11/Paso 5 database update' | Out-Null

    $list = Invoke-Checked -WorkingDirectory $root -FilePath 'dotnet' -ArgumentList @('ef','migrations','list','--configuration','Release') -Context '2.11/Paso 6 migrations list'
    Assert-TextContains -Text $list -Tokens @('M2_2_11') -Context '2.11/Paso 6'

    $sqlcmd = Get-Command sqlcmd -ErrorAction SilentlyContinue
    if (-not $sqlcmd) {
        throw '2.11/Paso 6: sqlcmd no está disponible.'
    }
    Push-Location $root
    try {
        $history = & sqlcmd -S '(localdb)\MSSQLLocalDB' -d 'AceriaDB' -Q 'SELECT MigrationId, ProductVersion FROM __EFMigrationsHistory ORDER BY MigrationId;' -W -h-1 2>&1
        if ($LASTEXITCODE -ne 0) {
            throw ('2.11/Paso 6: consulta __EFMigrationsHistory falla.' + [Environment]::NewLine + ($history | Out-String))
        }
        Assert-TextContains -Text ($history | Out-String) -Tokens @('M2_2_11') -Context '2.11/Paso 6 historial SQL'
    }
    finally {
        Pop-Location
    }
    Write-Host 'PASS 2.11/Pasos 5-6 aplicación e historial'

    Invoke-Checked -WorkingDirectory $root -FilePath 'dotnet' -ArgumentList @('ef','database','update','20260927204833_M2_2_10','--configuration','Release') -Context '2.11/Paso 7 rollback' | Out-Null
    $afterRollback = Invoke-Checked -WorkingDirectory $root -FilePath 'dotnet' -ArgumentList @('ef','migrations','list','--configuration','Release') -Context '2.11/Paso 7 list'
    $line211 = ($afterRollback -split "\r?\n" | Where-Object { $_ -match 'M2_2_11' }) -join ' '
    if ($line211 -notmatch 'Pending') {
        throw '2.11/Paso 7: M2_2_11 no queda Pending tras rollback.'
    }
    Write-Host 'PASS 2.11/Paso 7 rollback exacto'

    Invoke-Checked -WorkingDirectory $root -FilePath 'dotnet' -ArgumentList @('ef','database','update','--configuration','Release') -Context '2.11/Paso 8 reaplicar' | Out-Null
    Write-Host 'PASS 2.11/Paso 8 reaplicación'

    $temp9 = New-PedagogicalCopy -Source $root -Name 'm02-2-11-step9'
    $program9 = Join-Path $temp9 'Program.cs'
    $text9 = Get-Content $program9 -Raw
    $mainPattern = '(?m)^    public static void Main\(\)\r?\n    \{'
    $guard = '        if (Environment.GetEnvironmentVariable("ACERIA_EF_TOOLS_ONLY") == "1") return;'
    if ($text9 -notmatch $mainPattern) {
        throw '2.11/Paso 9: no se localiza Main para proteger migrations remove.'
    }
    $text9 = [regex]::Replace($text9,$mainPattern,{ param($m) $m.Value + [Environment]::NewLine + $guard },1)
    Set-Content $program9 -Value $text9 -Encoding utf8

    $oldToolsOnly = $env:ACERIA_EF_TOOLS_ONLY
    try {
        $env:ACERIA_EF_TOOLS_ONLY = '1'
        Invoke-Checked -WorkingDirectory $temp9 -FilePath 'dotnet' -ArgumentList @('ef','database','update','20260927204833_M2_2_10','--configuration','Release') -Context '2.11/Paso 9 rollback previo' | Out-Null
        Invoke-Checked -WorkingDirectory $temp9 -FilePath 'dotnet' -ArgumentList @('ef','migrations','remove','--configuration','Release') -Context '2.11/Paso 9 migrations remove' | Out-Null
    }
    finally {
        $env:ACERIA_EF_TOOLS_ONLY = $oldToolsOnly
    }
    if (Get-ChildItem (Join-Path $temp9 'Migrations') -Filter '*_M2_2_11.cs') {
        throw '2.11/Paso 9: M2_2_11 sigue presente en la copia desechable.'
    }
    Write-Host 'PASS 2.11/Paso 9 migrations remove aislado'

    $temp10 = New-PedagogicalCopy -Source $root -Name 'm02-2-11-step10'
    $snapshot10 = Join-Path $temp10 'Migrations\AceriaDbContextModelSnapshot.cs'
    Enable-SnapshotFragmentByExactReplacement -Path $snapshot10 -Marker 'FRAGMENTO PDF M02 2.11 - PASO 10'
    Invoke-Build -Root $temp10 -Context '2.11/Paso 10 snapshot descomentado'
    Invoke-Checked -WorkingDirectory $temp10 -FilePath 'dotnet' -ArgumentList @('ef','migrations','has-pending-model-changes','--configuration','Release') -Context '2.11/Paso 10 modelo' | Out-Null
    Write-Host 'PASS 2.11/Paso 10 snapshot comentado activado'

    $temp11 = New-PedagogicalCopy -Source $root -Name 'm02-2-11-step11'
    Invoke-Checked -WorkingDirectory $temp11 -FilePath 'dotnet' -ArgumentList @('ef','migrations','script','20260927204833_M2_2_10','20260927204841_M2_2_11','--configuration','Release','--output','softdelete.sql') -Context '2.11/Paso 11 delta SQL' | Out-Null
    Invoke-Checked -WorkingDirectory $temp11 -FilePath 'dotnet' -ArgumentList @('ef','migrations','script','--idempotent','--configuration','Release','--output','migraciones_idempotentes.sql') -Context '2.11/Paso 11 idempotente' | Out-Null

    $delta = Get-Content (Join-Path $temp11 'softdelete.sql') -Raw
    foreach ($table in @('OrdenesFabricacion','PlanchasAcero','Aleaciones','EstadosOrden')) {
        if ($delta -notmatch [regex]::Escape($table)) {
            throw "2.11/reto: softdelete.sql no contiene $table."
        }
    }
    if (([regex]::Matches($delta,'DeletedAt')).Count -lt 4 -or ([regex]::Matches($delta,'IsDeleted')).Count -lt 4) {
        throw '2.11/reto: el delta SQL no contiene las cuatro parejas DeletedAt/IsDeleted.'
    }

    $idempotent = Get-Content (Join-Path $temp11 'migraciones_idempotentes.sql') -Raw
    Assert-TextContains -Text $idempotent -Tokens @('__EFMigrationsHistory','M2_2_11') -Context '2.11/reto idempotente'
    Write-Host 'PASS 2.11/Paso 11 + reto SQL'

    Invoke-Build -Root $root -Context '2.11/Paso 12 build'
    $final211 = Invoke-Checked -WorkingDirectory $root -FilePath 'dotnet' -ArgumentList @('run','--project','AceriaData.Console.csproj','--configuration','Release','--no-build') -Context '2.11/Paso 12 run'
    Assert-TextContains -Text $final211 -Tokens @('2.11 OK','Tras borrar visibles: 0','Totales: 1','Restaurada: True') -Context '2.11/Paso 12'
    Write-Host 'PASS 2.11/Paso 12 E2E Soft Delete'
}

function Test-Point212 {
    Write-Section 'M02 · 2.12 arquitectura y variantes por paso'

    $root = Get-PointRoot '2.12'
    Invoke-Checked -WorkingDirectory $root -FilePath 'dotnet' -ArgumentList @('restore','AceriaData.sln') -Context '2.12/Paso 1 restore' | Out-Null
    Invoke-Build -Root $root -Context '2.12/Paso 1 build'

    $domainProject = Get-Content (Join-Path $root 'src\AceriaData.Domain\AceriaData.Domain.csproj') -Raw
    $applicationProject = Get-Content (Join-Path $root 'src\AceriaData.Application\AceriaData.Application.csproj') -Raw
    if ($domainProject -match 'EntityFrameworkCore|ProjectReference|PackageReference') {
        throw '2.12/Paso 2: Domain no es independiente.'
    }
    if ($applicationProject -match 'EntityFrameworkCore|AceriaData\.Infrastructure') {
        throw '2.12/Paso 3: Application depende de EF Core/Infrastructure.'
    }

    $groups = @(
        @{
            Name='Paso 2'
            Files=@(
                @{ Path='src\AceriaData.Domain\Entities.cs'; Marker='FRAGMENTO PDF M02 2.12 - PASO 2' }
            )
            CheckModel=$false
            Run=$false
        },
        @{
            Name='Paso 3'
            Files=@(
                @{ Path='src\AceriaData.Application\Interfaces.cs'; Marker='FRAGMENTO PDF M02 2.12 - PASO 3' }
            )
            CheckModel=$false
            Run=$false
        },
        @{
            Name='Paso 4'
            Files=@(
                @{ Path='src\AceriaData.Application\CrearOrdenUseCase.cs'; Marker='FRAGMENTO PDF M02 2.12 - PASO 4' }
            )
            CheckModel=$false
            Run=$false
        },
        @{
            Name='Paso 5'
            Files=@(
                @{ Path='src\AceriaData.Infrastructure\Persistence\AceriaDbContext.cs'; Marker='FRAGMENTO PDF M02 2.12 - PASO 5' },
                @{ Path='src\AceriaData.Infrastructure\Persistence\Configurations\OrdenFabricacionConfiguration.cs'; Marker='FRAGMENTO PDF M02 2.12 - PASO 5.1A' },
                @{ Path='src\AceriaData.Infrastructure\Persistence\Configurations\PlanchaAceroConfiguration.cs'; Marker='FRAGMENTO PDF M02 2.12 - PASO 5.1B' },
                @{ Path='src\AceriaData.Infrastructure\Persistence\Configurations\ModeloConfiguration.cs'; Marker='FRAGMENTO PDF M02 2.12 - PASO 5.1C' }
            )
            CheckModel=$true
            Run=$false
        },
        @{
            Name='Paso 6'
            Files=@(
                @{ Path='src\AceriaData.Infrastructure\Repositories\Repositories.cs'; Marker='FRAGMENTO PDF M02 2.12 - PASO 6A' },
                @{ Path='src\AceriaData.Infrastructure\DependencyInjection.cs'; Marker='FRAGMENTO PDF M02 2.12 - PASO 6B' }
            )
            CheckModel=$false
            Run=$false
        },
        @{
            Name='Paso 7'
            Files=@(
                @{ Path='src\AceriaData.Console\Program.cs'; Marker='FRAGMENTO PDF M02 2.12 - PASO 7' }
            )
            CheckModel=$false
            Run=$true
        },
        @{
            Name='Paso 8'
            Files=@(
                @{ Path='src\AceriaData.Infrastructure\Persistence\AceriaDesignTimeDbContextFactory.cs'; Marker='FRAGMENTO PDF M02 2.12 - PASO 8' }
            )
            CheckModel=$false
            Run=$true
            CheckMigrations=$true
        }
    )

    foreach ($group in $groups) {
        $temp = New-PedagogicalCopy -Source $root -Name ("m02-2-12-" + $group.Name.Replace(' ','-').ToLowerInvariant())
        foreach ($file in $group.Files) {
            Enable-LineCommentWholeFileCopy -Path (Join-Path $temp $file.Path) -Marker $file.Marker
        }

        Invoke-Build -Root $temp -Context ("2.12/" + $group.Name + " build de variante")

        if ($group.CheckModel) {
            Invoke-Checked -WorkingDirectory $temp -FilePath 'dotnet' -ArgumentList @('ef','migrations','has-pending-model-changes','--project','src/AceriaData.Infrastructure','--startup-project','src/AceriaData.Console','--configuration','Release') -Context ("2.12/" + $group.Name + " modelo") | Out-Null
        }

        if ($group.ContainsKey('CheckMigrations') -and $group.CheckMigrations) {
            $list = Invoke-Checked -WorkingDirectory $temp -FilePath 'dotnet' -ArgumentList @('ef','migrations','list','--project','src/AceriaData.Infrastructure','--startup-project','src/AceriaData.Console','--configuration','Release') -Context '2.12/Paso 8 migrations list'
            Assert-TextContains -Text $list -Tokens @('M2_2_11','M2_2_12_Architecture') -Context '2.12/Paso 8'
        }

        if ($group.Run) {
            $out = Invoke-Checked -WorkingDirectory $temp -FilePath 'dotnet' -ArgumentList @('run','--project','src/AceriaData.Console/AceriaData.Console.csproj','--configuration','Release','--no-build') -Context ("2.12/" + $group.Name + " run")
            Assert-TextContains -Text $out -Tokens @('2.12 OK') -Context ("2.12/" + $group.Name)
        }

        Write-Host "PASS 2.12/$($group.Name) variante descomentada"
    }

    $listFinal = Invoke-Checked -WorkingDirectory $root -FilePath 'dotnet' -ArgumentList @('ef','migrations','list','--project','src/AceriaData.Infrastructure','--startup-project','src/AceriaData.Console','--configuration','Release') -Context '2.12 estado final migrations list'
    Assert-TextContains -Text $listFinal -Tokens @('InitialCreate','AddAleacion','AddEstadoOrden','M2_2_2','M2_2_3','M2_2_4','M2_2_5','M2_2_6','M2_2_7','M2_2_8','M2_2_9','M2_2_10','M2_2_11','M2_2_12_Architecture') -Context '2.12 historial trasladado'

    Invoke-Checked -WorkingDirectory $root -FilePath 'dotnet' -ArgumentList @('ef','migrations','has-pending-model-changes','--project','src/AceriaData.Infrastructure','--startup-project','src/AceriaData.Console','--configuration','Release') -Context '2.12 modelo final' | Out-Null

    $final = Invoke-Checked -WorkingDirectory $root -FilePath 'dotnet' -ArgumentList @('run','--project','src/AceriaData.Console/AceriaData.Console.csproj','--configuration','Release','--no-build') -Context '2.12 estado final run'
    Assert-TextContains -Text $final -Tokens @('2.12 OK','OF-M2-HEX-0001','Cliente Arquitectura') -Context '2.12 estado final'
    Write-Host 'PASS 2.12/Pasos 1-8 y estado final'
}

if ($Suite -in @('all','coverage')) {
    Test-Coverage
}

if ($Suite -in @('all','variants')) {
    Test-ProgramFragments
    Test-Point211
    Test-Point212
}

if ($Suite -in @('all','retos')) {
    Test-Retos
}

if ($Suite -in @('all','operational')) {
    Test-MigrationsAndFinalStates21To210

    if ($Suite -eq 'operational') {
        Test-Point211
        Test-Point212
    }
}

Write-Section 'M02 · RESULTADO'
Write-Host "PASS suite '$Suite'."
