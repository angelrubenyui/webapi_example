param(
    [string]$Action = "up",
    [switch]$Build = $false
)

# Configuracion
$SqlContainer = "mi-sqlserver"
$ApiContainer = "mi-api-exampleapi"
$ApiPort = 5000
$SqlPort = 1433
$SqlPassword = "YourStrongPassword123!"
$SqlUser = "sa"

Write-Host "exampleAPI - Docker Manager"

# Funcion para ejecutar comandos
function Execute-Command {
    param([string]$Command)
    Write-Host ">> $Command"
    Invoke-Expression $Command
}

# Funcion: Iniciar servicios
function Start-Services {
    Write-Host ""
    Write-Host "Iniciando servicios..."
    
    if ($Build) {
        Write-Host "Construyendo imagen..."
        Execute-Command "docker-compose build"
        Write-Host ""
    }
    
    Execute-Command "docker-compose up -d"
    
    Write-Host ""
    Write-Host "Esperando a que SQL Server .."
    Start-Sleep -Seconds 15
    
    Write-Host ""
    Write-Host "Servicios iniciados correctamente!"
    Write-Host ""
    Write-Host "URLs disponibles:"
    Write-Host "  API:        http://localhost:$ApiPort"
    Write-Host "  Swagger:    http://localhost:$ApiPort/swagger"
    Write-Host "  SQL Server: localhost,$SqlPort"
    Write-Host "  Usuario SQL: $SqlUser"
    Write-Host "  Password SQL: $SqlPassword"
    Write-Host ""
}

# Funcion: Detener servicios
function Stop-Services {
    Write-Host ""
    Write-Host "Deteniendo servicios..."
    Execute-Command "docker-compose down"
    Write-Host "Servicios detenidos."
    Write-Host ""
}

# Funcion: Limpiar todo
function Clean-All {
    Write-Host ""
    Write-Host "Limpiando imagenes, contenedores y volumenes..."
    Execute-Command "docker-compose down -v"
    Write-Host "Limpieza completada."
    Write-Host ""
}

function Show-Logs {
    param([string]$Container = "")
    
    Write-Host ""
    if ($Container) {
        Write-Host "Logs de $Container (Ctrl+C para salir):"
        Execute-Command "docker logs -f $Container"
    } else {
        Write-Host "Logs de todos los servicios (Ctrl+C para salir):"
        Execute-Command "docker-compose logs -f"
    }
}


function Show-Status {
    Write-Host ""
    Write-Host "Estado de los contenedores:"
    Write-Host ""
    Execute-Command "docker-compose ps"
    Write-Host ""
    Write-Host "Volumenes:"
    Execute-Command "docker volume ls | findstr mssql_data"
    Write-Host ""
}

function Run-Migrations {
    Write-Host ""
    Write-Host "Ejecutando migraciones..."
    
    $apiContainer = docker ps -aqf "name=$ApiContainer"
    if ($apiContainer) {
        Execute-Command "docker exec $apiContainer dotnet ef database update --project exampleAPI.Infrastructure --startup-project exampleAPI.Api"
        Write-Host "Migraciones ejecutadas."
    } else {
        Write-Host "ERROR: Contenedor API no encontrado. Inicia los servicios primero."
    }
    Write-Host ""
}

function Test-Connection {
    Write-Host ""
    Write-Host "Probando conexiones..."
    Write-Host ""
    
    Write-Host "API:"
    try {
        $response = Invoke-WebRequest -Uri "http://localhost:$ApiPort/swagger" -UseBasicParsing -ErrorAction Stop
        if ($response.StatusCode -eq 200) {
            Write-Host "OK - API respondiendo en http://localhost:$ApiPort"
        }
    } catch {
        Write-Host "ERROR - API no responde"
    }
    
    Write-Host ""
    Write-Host "SQL Server:"
    $sqlContainer = docker ps -aqf "name=$SqlContainer"
    if ($sqlContainer) {
        Write-Host "OK - SQL Server ejecutando"
        Write-Host "  Host: localhost,$SqlPort"
        Write-Host "  Usuario: $SqlUser"
    } else {
        Write-Host "ERROR - SQL Server no ejecutando"
    }
    Write-Host ""
}

# Menu principal
switch ($Action.ToLower()) {
    "up" {
        Start-Services
    }
    "down" {
        Stop-Services
    }
    "restart" {
        Stop-Services
        Start-Sleep -Seconds 2
        Start-Services
    }
    "clean" {
        Clean-All
    }
    "logs" {
        Show-Logs
    }
    "logs-api" {
        Show-Logs $ApiContainer
    }
    "logs-sql" {
        Show-Logs $SqlContainer
    }
    "status" {
        Show-Status
    }
    "migrations" {
        Run-Migrations
    }
    "test" {
        Test-Connection
    }
    "help" {
        Write-Host ""
        Write-Host "Comandos disponibles:"
        Write-Host ""
        Write-Host "  .\run-docker.ps1 up               - Iniciar servicios"
        Write-Host "  .\run-docker.ps1 up -Build        - Iniciar con rebuild de imagen"
        Write-Host "  .\run-docker.ps1 down             - Detener servicios"
        Write-Host "  .\run-docker.ps1 restart          - Reiniciar servicios"
        Write-Host "  .\run-docker.ps1 clean            - Limpiar todo (volumenes incluidos)"
        Write-Host "  .\run-docker.ps1 status           - Ver estado de contenedores"
        Write-Host "  .\run-docker.ps1 logs             - Ver logs de todos"
        Write-Host "  .\run-docker.ps1 logs-api         - Ver logs de la API"
        Write-Host "  .\run-docker.ps1 logs-sql         - Ver logs de SQL Server"
        Write-Host "  .\run-docker.ps1 migrations       - Ejecutar migraciones EF Core"
        Write-Host "  .\run-docker.ps1 test             - Probar conexiones"
        Write-Host "  .\run-docker.ps1 help             - Mostrar esta ayuda"
        Write-Host ""
    }
    default {
        Write-Host "ERROR: Accion desconocida: $Action"
        Write-Host "Usa: .\run-docker.ps1 help"
        Write-Host ""
    }
}
