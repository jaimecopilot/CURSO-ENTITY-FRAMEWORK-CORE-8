param(
    [Parameter(Mandatory = $true)]
    [ValidateSet("M1","M2","M3","M4","M5")]
    [string]$Module
)

$ErrorActionPreference = "Stop"
if ($PSVersionTable.PSVersion.Major -ge 7) {
    $PSNativeCommandUseErrorActionPreference = $true
}

function Assert-Contains([string]$Text, [string]$Pattern, [string]$Message) {
    if ($Text -notmatch $Pattern) { throw $Message }
}

function Build-Solution([string]$Path) {
    Write-Host "== BUILD $Path =="
    dotnet restore $Path
    dotnet build $Path --configuration Release --no-restore
}

function Run-Project([string]$Path) {
    Write-Host "== RUN $Path =="
    return (dotnet run --project $Path --configuration Release --no-build 2>&1 | Out-String)
}

switch ($Module) {
    "M1" {
        python scripts/sync_m1_project_states.py
        $syncChanges = git status --porcelain -- M01/PROYECTO/1.5 M01/PROYECTO/1.6 M01/PROYECTO/1.7 M01/PROYECTO/1.8 M01/PROYECTO/1.9 M01/PROYECTO/1.10
        if ($syncChanges) { throw "M1: la práctica y los estados acumulativos no están sincronizados" }

        foreach ($n in 1..12) {
            Build-Solution "M01/PROYECTO/1.$n/AceriaData.sln"
        }

        Push-Location M01/PROYECTO/1.10
        $list = dotnet ef migrations list --no-build --configuration Release | Out-String
        @("InitialCreate","AddAleacion","AddEstadoOrden") | ForEach-Object {
            Assert-Contains $list $_ "M1 1.10: falta migración $_"
        }
        dotnet ef database update --no-build --configuration Release
        dotnet ef database update 20260927000200_AddAleacion --no-build --configuration Release
        dotnet ef database update --no-build --configuration Release
        dotnet ef migrations script --no-build --configuration Release --output migrations-1.10.sql
        if (-not (Test-Path migrations-1.10.sql)) { throw "M1: no se generó migrations-1.10.sql" }
        Pop-Location

        Push-Location M01/PROYECTO/1.12
        dotnet ef migrations list --no-build --configuration Release
        dotnet ef database update --no-build --configuration Release
        Pop-Location

        foreach ($n in 1..12) {
            $output = Run-Project "M01/PROYECTO/1.$n/AceriaData.Console.csproj"
            $output | Write-Host
        }
        python scripts/audit_m1_traceability.py
        Write-Host "COURSE FINAL M1 PASS"
    }

    "M2" {
        python scripts/audit_m2_traceability.py

        foreach ($n in 1..11) {
            Build-Solution "M02/PROYECTO/2.$n/AceriaData.sln"
        }

        Push-Location M02/PROYECTO/2.11
        $list = dotnet ef migrations list --no-build --configuration Release | Out-String
        Assert-Contains $list "M2_2_11" "M2: 2.11 no aparece en migrations list"
        dotnet ef database update --no-build --configuration Release
        dotnet ef database update 20260927204833_M2_2_10 --no-build --configuration Release
        dotnet ef database update --no-build --configuration Release
        dotnet ef migrations script 20260927204833_M2_2_10 20260927204841_M2_2_11 --idempotent --no-build --configuration Release --output softdelete-idempotent.sql
        $sql = Get-Content softdelete-idempotent.sql -Raw
        @("__EFMigrationsHistory","IsDeleted","DeletedAt") | ForEach-Object {
            Assert-Contains $sql $_ "M2: script 2.11 incompleto: $_"
        }
        Pop-Location

        foreach ($n in 1..11) {
            $output = Run-Project "M02/PROYECTO/2.$n/AceriaData.Console.csproj"
            $output | Write-Host
        }

        Build-Solution "M02/PROYECTO/2.12/AceriaData.sln"
        Push-Location M02/PROYECTO/2.12
        $list = dotnet ef migrations list --project src/AceriaData.Infrastructure --startup-project src/AceriaData.Console --configuration Release | Out-String
        Assert-Contains $list "M2_2_12_Architecture" "M2: falta migración final de arquitectura"
        $output = dotnet run --project src/AceriaData.Console/AceriaData.Console.csproj --configuration Release --no-build 2>&1 | Out-String
        $output | Write-Host
        Assert-Contains $output "2\.12 OK" "M2: falta marcador 2.12 OK"
        Pop-Location
        Write-Host "COURSE FINAL M2 PASS"
    }

    "M3" {
        python scripts/audit_m3_traceability.py
        foreach ($n in 1..12) {
            Build-Solution "M03/PROYECTO/3.$n/AceriaData.sln"
        }
        Push-Location M03/PROYECTO/3.12
        $list = dotnet ef migrations list --project src/AceriaData.Infrastructure --startup-project src/AceriaData.Console --configuration Release | Out-String
        Assert-Contains $list "M2_2_12_Architecture" "M3: no conserva migración final de M2"
        dotnet ef database update --project src/AceriaData.Infrastructure --startup-project src/AceriaData.Console --configuration Release
        Pop-Location
        foreach ($n in 1..12) {
            $output = Run-Project "M03/PROYECTO/3.$n/src/AceriaData.Console/AceriaData.Console.csproj"
            $output | Write-Host
            Assert-Contains $output "3\.$n OK" "M3: falta marcador 3.$n OK"
        }
        $app = Get-ChildItem M03/PROYECTO/3.12/src/AceriaData.Application -Filter *.cs | Get-Content -Raw
        if ($app -match "Microsoft.EntityFrameworkCore") { throw "M3: Application depende de EF Core" }
        Write-Host "COURSE FINAL M3 PASS"
    }

    "M4" {
        python scripts/audit_m4_traceability.py
        foreach ($n in 1..12) {
            Build-Solution "M04/PROYECTO/4.$n/AceriaData.sln"
        }
        Push-Location M04/PROYECTO/4.12
        $list = dotnet ef migrations list --project src/AceriaData.Infrastructure --startup-project src/AceriaData.Console --configuration Release | Out-String
        Assert-Contains $list "M2_2_12_Architecture" "M4: no conserva migración final heredada"
        dotnet ef database update --project src/AceriaData.Infrastructure --startup-project src/AceriaData.Console --configuration Release
        Pop-Location
        foreach ($n in 1..12) {
            $output = Run-Project "M04/PROYECTO/4.$n/src/AceriaData.Console/AceriaData.Console.csproj"
            $output | Write-Host
            Assert-Contains $output "4\.$n OK" "M4: falta marcador 4.$n OK"
        }
        $app = Get-ChildItem M04/PROYECTO/4.12/src/AceriaData.Application -Filter *.cs | Get-Content -Raw
        if ($app -match "Microsoft.EntityFrameworkCore") { throw "M4: Application depende de EF Core" }
        Write-Host "COURSE FINAL M4 PASS"
    }

    "M5" {
        foreach ($n in 1..12) {
            Build-Solution "M05/PROYECTO/5.$n/AceriaData.sln"
        }

        Push-Location M05/PROYECTO/5.12
        $list = dotnet ef migrations list --project src/AceriaData.Infrastructure --startup-project src/AceriaData.Console --configuration Release | Out-String
        Assert-Contains $list "M5_5_2_ConcurrencyTokens" "M5: falta migración M5_5_2_ConcurrencyTokens"
        dotnet ef migrations has-pending-model-changes --project src/AceriaData.Infrastructure --startup-project src/AceriaData.Console --configuration Release
        Pop-Location

        foreach ($n in 1..12) {
            $output = Run-Project "M05/PROYECTO/5.$n/src/AceriaData.Console/AceriaData.Console.csproj"
            $output | Write-Host
            Assert-Contains $output "5\.$n OK" "M5: falta marcador 5.$n OK"

            if ($n -eq 10) {
                Assert-Contains $output "5\.10 DIAGNOSTIC EVENTS: [1-9]" "M5: DiagnosticSource sin evidencia"
                Assert-Contains $output "5\.10 EVENT COUNTERS: [1-9]" "M5: EventCounters sin evidencia"
                Assert-Contains $output "5\.10 TELEMETRY ITEMS: [1-9]" "M5: telemetría sin evidencia"
            }
            if ($n -eq 12) {
                Assert-Contains $output "5\.12 N\+1 RESULTADOS EQUIVALENTES: True" "M5: refactor N+1 no equivalente"
                Assert-Contains $output "5\.12 OVERFETCH RESULTADOS EQUIVALENTES: True" "M5: refactor over-fetch no equivalente"
                Assert-Contains $output "5\.12 PROJECTION EXCLUDES ROWVERSION: True" "M5: proyección no reduce columnas"
                Assert-Contains $output "5\.12 METODO WHERE FALLA TRADUCCION: True" "M5: no se demostró fallo de traducción"
            }
        }

        Push-Location M05/PROYECTO/5.7
        ./deployment/validate-idempotent-scripts.ps1
        Pop-Location

        Push-Location M05/PROYECTO/5.8
        ./team-migrations/validate-team-migrations.ps1
        Pop-Location

        Push-Location M05/PROYECTO/5.12
        dotnet test tests/AceriaData.Tests/AceriaData.Tests.csproj --configuration Release --no-build --logger "console;verbosity=normal"
        Pop-Location

        python scripts/validate_m5_traceability.py

        foreach ($n in 1..12) {
            $app = Get-ChildItem "M05/PROYECTO/5.$n/src/AceriaData.Application" -Filter *.cs | Get-Content -Raw
            if ($app -match "Microsoft.EntityFrameworkCore") { throw "M5 $n: Application depende de EF Core" }
            if ($app -match "AceriaData.Infrastructure") { throw "M5 $n: Application depende de Infrastructure" }
        }
        Write-Host "COURSE FINAL M5 PASS"
    }
}
