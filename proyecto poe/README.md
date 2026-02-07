# 🏆 Sistema de Elección Reina FIEC - Versión Mejorada 2.0

## 📋 Descripción del Proyecto

Sistema completo de gestión de elecciones para Reina de la Facultad y Miss Fotogenia, desarrollado en C# con Windows Forms y SQL Server. Esta versión incluye mejoras críticas de seguridad, arquitectura robusta y funcionalidades completas.

---

## ✨ Características Principales

### 🔒 Seguridad
- ✅ **Autenticación segura** con hash de contraseñas (PBKDF2)
- ✅ **Consultas parametrizadas** para prevenir inyección SQL
- ✅ **Sistema de roles** (Administrador/Estudiante)
- ✅ **Auditoría completa** de todas las votaciones
- ✅ **Validación de entrada** en todos los formularios

### 🏗️ Arquitectura
- ✅ **Patrón Repository** para acceso a datos
- ✅ **Capa de Servicios** para lógica de negocio
- ✅ **Separación de responsabilidades** (SRP)
- ✅ **Manejo robusto de excepciones**
- ✅ **Código limpio y mantenible**

### 📊 Funcionalidades

#### Módulo Administrador
- Inscripción completa de candidatas con validaciones
- Gestión de álbumes de fotos (crear, editar, eliminar)
- Consulta de resultados en tiempo real con estadísticas
- Panel de control con métricas generales
- Gestión de usuarios y seguridad

#### Módulo Estudiante
- Visualización de candidatas con portafolios completos
- Sistema de votación seguro (una vez por categoría)
- Votación para Reina FCMF
- Votación para Miss Fotogenia
- Sistema de comentarios en fotos
- Consulta de resultados

---

## 🚀 Requisitos del Sistema

### Software Necesario
- **Windows 7 o superior** (recomendado Windows 10/11)
- **Visual Studio 2019 o superior**
- **.NET Framework 4.7.2 o superior**
- **SQL Server 2012 o superior** (Express, Developer o Enterprise)
- **SQL Server Management Studio** (SSMS)

### Librerías Adicionales
- System.Data.SqlClient (incluida en .NET Framework)
- System.Configuration (incluida en .NET Framework)

---

## 📦 Instalación Paso a Paso

### Paso 1: Configurar SQL Server

1. **Instalar SQL Server** (si no lo tienes):
   - Descargar SQL Server Express: https://www.microsoft.com/sql-server/sql-server-downloads
   - Instalar con configuración por defecto
   - Anotar el nombre del servidor (generalmente: `localhost` o `.\SQLEXPRESS`)

2. **Instalar SQL Server Management Studio (SSMS)**:
   - Descargar: https://docs.microsoft.com/sql/ssms/download-sql-server-management-studio-ssms
   - Instalar con configuración por defecto

### Paso 2: Crear la Base de Datos

1. Abrir **SQL Server Management Studio**

2. Conectarse al servidor local:
   - Server name: `localhost` o `.\SQLEXPRESS`
   - Authentication: Windows Authentication
   - Click en "Connect"

3. Ejecutar el script de base de datos:
   - Abrir el archivo: `Documentation/ScriptBaseDatos.sql`
   - Presionar **F5** o click en "Execute"
   - Verificar que aparezca el mensaje de éxito

4. Verificar que se creó la base de datos:
   ```sql
   USE ReinaFIECDB;
   GO
   SELECT * FROM Usuarios; -- Debe mostrar el usuario admin
   ```

### Paso 3: Configurar el Proyecto

1. **Abrir el proyecto en Visual Studio**:
   - Abrir Visual Studio
   - File → Open → Project/Solution
   - Seleccionar el archivo `ReinaFIEC.csproj` o `ReinaFIEC.sln`

2. **Configurar la cadena de conexión**:
   - Abrir el archivo `App.config`
   - Modificar la conexión según tu configuración:

   ```xml
   <connectionStrings>
     <!-- Si usas autenticación de Windows (recomendado): -->
     <add name="ReinaFIECDB" 
          connectionString="Server=localhost;Database=ReinaFIECDB;Integrated Security=true;" 
          providerName="System.Data.SqlClient" />
     
     <!-- Si usas usuario y contraseña de SQL Server: -->
     <!-- 
     <add name="ReinaFIECDB" 
          connectionString="Server=localhost;Database=ReinaFIECDB;User Id=tu_usuario;Password=tu_contraseña;" 
          providerName="System.Data.SqlClient" />
     -->
   </connectionStrings>
   ```

3. **Restaurar referencias** (si es necesario):
   - Click derecho en el proyecto → "Restore NuGet Packages"
   - Build → Rebuild Solution

### Paso 4: Compilar y Ejecutar

1. **Compilar el proyecto**:
   - Presionar **F6** o Build → Build Solution
   - Verificar que no haya errores

2. **Ejecutar la aplicación**:
   - Presionar **F5** o Debug → Start Debugging
   - Debe aparecer la ventana de login

3. **Iniciar sesión como administrador**:
   - Usuario: `admin`
   - Contraseña: `Admin123!`
   - ⚠️ **IMPORTANTE**: Cambiar esta contraseña después del primer login

---

## 👥 Uso de la Aplicación

### Primer Uso - Administrador

1. **Cambiar contraseña del administrador**:
   - Ir a Configuración → Cambiar Contraseña
   - Ingresar contraseña actual: `Admin123!`
   - Ingresar nueva contraseña segura

2. **Registrar candidatas**:
   - Ir a Candidatas → Nueva Candidata
   - Completar todos los campos requeridos
   - Cargar foto principal (JPG, PNG, GIF máx 5MB)
   - Click en "Guardar"

3. **Crear álbumes de fotos**:
   - Seleccionar una candidata
   - Click en "Gestionar Álbumes"
   - "Nuevo Álbum" → Ingresar nombre y descripción
   - Agregar fotos al álbum

4. **Consultar resultados**:
   - Ir a Resultados → Ver Resultados
   - Seleccionar categoría (Reina o Miss Fotogenia)
   - Ver estadísticas en tiempo real

### Registro de Estudiantes

1. **Los estudiantes se registran desde la pantalla principal**:
   - Click en "Registrarse"
   - Completar formulario:
     - Username (mínimo 4 caracteres)
     - Password (mínimo 6 caracteres, con mayúscula y número)
     - Email válido
     - Nombre completo
     - Matrícula (mínimo 6 caracteres)
   - Click en "Registrar"

2. **Iniciar sesión como estudiante**:
   - Usar el username y password registrados

### Votación - Estudiantes

1. **Ver candidatas**:
   - Explorar el portafolio de cada candidata
   - Ver fotos en álbumes
   - Leer información personal y académica

2. **Votar por Reina FCMF**:
   - Ir a Votación → Reina FCMF
   - Seleccionar candidata favorita
   - Click en "Votar"
   - Confirmar voto
   - ✅ Solo se puede votar una vez

3. **Votar por Miss Fotogenia**:
   - Ir a Votación → Miss Fotogenia
   - Seleccionar candidata más fotogénica
   - Click en "Votar"
   - Confirmar voto
   - ✅ Solo se puede votar una vez

4. **Comentar en fotos**:
   - Abrir álbum de una candidata
   - Seleccionar foto
   - Click en "Agregar Comentario"
   - Escribir comentario (máx 500 caracteres)
   - Click en "Publicar"

---

## 🗂️ Estructura del Proyecto

```
ReinaFIEC_Mejorado/
├── Models/                    # Modelos de datos (Entidades)
│   ├── Usuario.cs            # Usuario del sistema
│   ├── Candidata.cs          # Candidata a reina
│   ├── Album.cs              # Álbumes, Fotos y Comentarios
│   └── Voto.cs               # Votos y auditoría
│
├── Data/                      # Capa de acceso a datos
│   ├── IRepository.cs        # Interfaz genérica
│   └── DatabaseContext.cs    # Contexto de base de datos
│
├── Repositories/              # Implementación de repositorios
│   ├── UsuarioRepository.cs
│   ├── CandidataRepository.cs
│   └── VotoRepository.cs
│
├── Services/                  # Lógica de negocio
│   ├── AuthService.cs        # Autenticación
│   ├── VotacionService.cs    # Gestión de votaciones
│   └── CandidataService.cs   # Gestión de candidatas
│
├── Views/                     # Interfaz de usuario (Forms)
│   ├── FormLogin.cs
│   ├── FormPrincipal.cs
│   ├── FormCandidatas.cs
│   └── FormVotacion.cs
│
├── Utils/                     # Utilidades
│   ├── PasswordHasher.cs     # Hash de contraseñas
│   └── Validadores.cs        # Validaciones
│
├── Documentation/             # Documentación
│   ├── ScriptBaseDatos.sql   # Script SQL completo
│   └── ManualUsuario.pdf
│
└── App.config                 # Configuración
```

---

## 🔒 Seguridad Implementada

### 1. Prevención de Inyección SQL
```csharp
// ❌ ANTES (Vulnerable):
string sql = "SELECT * FROM Usuarios WHERE Username = '" + username + "'";

// ✅ AHORA (Seguro):
string sql = "SELECT * FROM Usuarios WHERE Username = @Username";
cmd.Parameters.AddWithValue("@Username", username);
```

### 2. Hash de Contraseñas
- Algoritmo: PBKDF2 con 10,000 iteraciones
- Salt aleatorio de 16 bytes por contraseña
- Hash de 20 bytes
- Las contraseñas NUNCA se guardan en texto plano

### 3. Control de Acceso
- Sistema de roles (Administrador/Estudiante)
- Verificación de permisos en cada acción
- Sesiones controladas

### 4. Auditoría
- Registro completo de todos los votos
- Tracking de IP y timestamp
- Log de todas las acciones críticas

---

## 📊 Base de Datos

### Tablas Principales

1. **Usuarios**: Gestión de usuarios con autenticación
2. **Candidatas**: Información de candidatas
3. **Votos**: Registro de votos emitidos
4. **AuditoriaVotos**: Log de auditoría
5. **Albumes**: Álbumes de fotos
6. **Fotos**: Fotos de candidatas
7. **Comentarios**: Comentarios en fotos
8. **Carreras**: Catálogo de carreras

### Relaciones
- Usuario → Votos (1:N)
- Candidata → Votos (1:N)
- Candidata → Albumes (1:N)
- Album → Fotos (1:N)
- Foto → Comentarios (1:N)
- Usuario → Comentarios (1:N)

---

## 🐛 Solución de Problemas

### Error: "No se puede conectar a la base de datos"

**Solución**:
1. Verificar que SQL Server esté ejecutándose:
   - Services → SQL Server (SQLEXPRESS) → Start

2. Verificar la cadena de conexión en `App.config`

3. Probar conexión en SSMS con las mismas credenciales

### Error: "Login fallido"

**Solución**:
1. Verificar que ejecutaste el script de base de datos
2. Usuario por defecto: `admin` / `Admin123!`
3. Verificar en SSMS:
   ```sql
   SELECT * FROM Usuarios WHERE Username = 'admin'
   ```

### Error: "No se pueden cargar fotos"

**Solución**:
1. Verificar permisos de escritura en la carpeta del proyecto
2. Crear carpeta `Candidatas` en el directorio raíz
3. Verificar tamaño de archivo (máx 5MB)
4. Verificar formato (JPG, PNG, GIF)

### Error de compilación

**Solución**:
1. Verificar .NET Framework 4.7.2 instalado
2. Restaurar paquetes NuGet
3. Limpiar y recompilar:
   - Build → Clean Solution
   - Build → Rebuild Solution

---

## 📈 Mejoras Implementadas vs Versión Anterior

| Aspecto | Versión Anterior | Versión Nueva |
|---------|------------------|---------------|
| **Base de Datos** | Access (.mdb) | SQL Server |
| **Inyección SQL** | ❌ Vulnerable | ✅ Protegido |
| **Contraseñas** | ❌ Sin hash | ✅ PBKDF2 |
| **Arquitectura** | ❌ Monolítica | ✅ Por capas |
| **Validaciones** | ❌ Básicas | ✅ Completas |
| **Auditoría** | ❌ Ninguna | ✅ Completa |
| **Manejo Errores** | ❌ Limitado | ✅ Robusto |
| **Escalabilidad** | ❌ Baja | ✅ Alta |
| **Mantenibilidad** | ❌ Difícil | ✅ Fácil |

---

## 🎯 Funcionalidades Adicionales

### Validaciones Implementadas

**Candidatas:**
- Cédula ecuatoriana válida (10 dígitos)
- Edad entre 17-25 años
- Semestre entre 1-12
- Promedio académico 0-10
- Email válido
- Cédula única

**Usuarios:**
- Username mínimo 4 caracteres
- Password mínimo 6 caracteres con mayúscula y número
- Email válido
- Matrícula mínima 6 caracteres
- Matrícula única

**Comentarios:**
- Máximo 500 caracteres
- Sin contenido ofensivo (opcional: implementar filtro)
- Moderación por administrador

### Reportes Disponibles

1. **Resultados por Categoría**
   - Lista completa de candidatas
   - Votos recibidos
   - Porcentaje de votación
   - Posición en ranking

2. **Estadísticas Generales**
   - Total de candidatas
   - Total de votantes
   - Participación por categoría
   - Ganadoras

3. **Auditoría**
   - Log de todos los votos
   - IP y timestamp
   - Acciones de usuarios

---

## 🔄 Actualizaciones Futuras

### Próximas Mejoras Planeadas
- [ ] Interfaz web con ASP.NET Core
- [ ] App móvil (Android/iOS)
- [ ] Gráficos interactivos con Chart.js
- [ ] Exportación de resultados a PDF/Excel
- [ ] Sistema de notificaciones por email
- [ ] Panel de control con dashboards
- [ ] Votación en tiempo real con SignalR
- [ ] Backup automático de base de datos

---

## 📞 Soporte y Contacto

### Información del Proyecto
- **Versión**: 2.0
- **Fecha**: Enero 2026
- **Desarrollador**: [Tu Nombre]
- **Institución**: FIEC

### Reportar Problemas
Si encuentras algún error o tienes sugerencias:
1. Documenta el problema claramente
2. Incluye capturas de pantalla
3. Describe los pasos para reproducir el error
4. Incluye el mensaje de error completo

---

## 📜 Licencia

Este proyecto es de uso académico y educativo.

---

## ⚠️ Notas Importantes

1. **CAMBIAR CONTRASEÑA DEL ADMIN**: Después del primer login, cambiar inmediatamente la contraseña por defecto.

2. **BACKUP REGULAR**: Realizar backups periódicos de la base de datos:
   ```sql
   BACKUP DATABASE ReinaFIECDB 
   TO DISK = 'C:\Backups\ReinaFIEC_backup.bak'
   ```

3. **SEGURIDAD**: Nunca compartir las credenciales del administrador.

4. **PRUEBAS**: Probar todas las funcionalidades antes de usar en producción.

5. **SOPORTE SQL SERVER**: Asegurarse de que SQL Server esté actualizado y configurado correctamente.

---

## 🎓 Créditos

**Docente**: Ph.D. Franklin Parrales Bravo  
**Materia**: Programación Orientada a Eventos  
**Institución**: FIEC  

---

**¡Sistema listo para usar! 🚀**

Para más información, consultar la documentación adicional en la carpeta `Documentation/`.
