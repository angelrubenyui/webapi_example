## EXAMPLE REDARBOR 
## 2026/9/8
## Version 1.00 Beta
--

## Endpoints REST

```
GET    /api/employees              → Obtener todos los empleados
GET    /api/employees/{id}         → Obtener empleado por ID
POST   /api/employees              → Crear nuevo empleado
PUT    /api/employees/{id}         → Actualizar empleado
DELETE /api/employees/{id}         → Eliminar empleado (Soft Delete)
```

### **Swagger**
 `http://localhost:5000/swagger`

---

## Campos de Employee

### **Obligatorios**
- `CompanyId` (int)
- `Email` (string)
- `Password` (string)
- `PortalId` (int)
- `RoleId` (int)
- `StatusId` (int)
- `Username` (string)

### **Opcionales**
- `Name` (string)
- `Telephone` (string)
- `Fax` (string)
- `LastLogin` (DateTime)

---

## Test

### **1. Crear Employee**
```bash
curl -X POST http://localhost:5000/api/employees \
  -H "Content-Type: application/json" \
  -d '{
    "companyId": 1,
    "email": "test1@test.com",
    "password": "test",
    "portalId": 1,
    "roleId": 1,
    "statusId": 1,
    "username": "test1",
    "name": "Test User"
  }'
```
### **1.1 Crear Employee con JSON**
curl.exe -X POST http://localhost:5000/api/employees -H "Content-Type: application/json" -d `@employee.json


### **2. Obtener Todos**
```bash
curl http://localhost:5000/api/employees
```

### **3. Obtener por ID**
```bash
curl http://localhost:5000/api/employees/1
```

### **4. Actualizar**
```bash
curl -X PUT http://localhost:5000/api/employees/3 \
  -H "Content-Type: application/json" \
  -d '{
    "email": "updated@test.com",
    "roleId": 2,
    "statusId": 1,
    "name": "Updated User"
  }'
```

### **5. Eliminar**
```bash
curl -X DELETE http://localhost:5000/api/employees/1
```

---

## Docker

### **Servicios**

| Servicio | Imagen | Puerto | Contenedor |
|----------|--------|--------|-----------|
| SQL Server | mssql/server:2019 | 1433 | mi-sqlserver |
| API | .NET 9 Custom | 5000 | mi-api-exampleapi |

---

## Test  Unitarios

```powershell
# Ejecutar todos los tests
dotnet test

# Ejecutar con verbose
dotnet test -v detailed

# Tests disponibles: 28
# - Command Handlers: 6
# - Query Handlers: 5
# - Validators: 17
```
