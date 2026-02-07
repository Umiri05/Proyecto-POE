# 🚀 GUÍA RÁPIDA DE INICIO - Sistema Reina FIEC

## ⏱️ Instalación en 10 Minutos

### 1️⃣ Instalar SQL Server (5 min)
```
1. Descargar SQL Server Express: bit.ly/sqlserver2019
2. Ejecutar instalador → Basic Installation
3. Aceptar términos → Install
4. Anotar el nombre del servidor mostrado
```

### 2️⃣ Instalar SSMS (3 min)
```
1. Descargar SSMS: bit.ly/ssms-download
2. Ejecutar instalador
3. Siguiente → Install
```

### 3️⃣ Crear Base de Datos (1 min)
```
1. Abrir SSMS
2. Conectar a localhost
3. Abrir archivo: Documentation/ScriptBaseDatos.sql
4. Presionar F5 (Execute)
5. ✅ Listo
```

### 4️⃣ Configurar Proyecto (1 min)
```
1. Abrir Visual Studio
2. Abrir ReinaFIEC.csproj
3. Abrir App.config
4. Verificar línea de conexión:
   Server=localhost;Database=ReinaFIECDB;Integrated Security=true;
5. Presionar F5 para ejecutar
```

---

## 🔑 Credenciales por Defecto

```
Usuario: admin
Contraseña: Admin123!
```

⚠️ **CAMBIAR INMEDIATAMENTE DESPUÉS DEL PRIMER LOGIN**

---

## ✅ Checklist de Verificación

- [ ] SQL Server instalado y ejecutándose
- [ ] SSMS instalado
- [ ] Base de datos ReinaFIECDB creada
- [ ] Proyecto abierto en Visual Studio
- [ ] App.config configurado correctamente
- [ ] Aplicación compila sin errores (F6)
- [ ] Login exitoso con credenciales por defecto
- [ ] Contraseña del admin cambiada

---

## 📞 Ayuda Rápida

### No puedo conectar a la base de datos
```
1. Verificar que SQL Server esté corriendo:
   Services → SQL Server (SQLEXPRESS) → Start
   
2. Verificar nombre del servidor en SSMS
3. Actualizar App.config con el nombre correcto
```

### Error al compilar
```
1. Build → Clean Solution
2. Build → Rebuild Solution
3. Si persiste: Tools → NuGet → Restore Packages
```

### Olvidé la contraseña del admin
```
Ejecutar en SSMS:
UPDATE Usuarios 
SET PasswordHash = 'HAkdO7/mRkP7QqKZ9w+JkA==+gZdV8s1QqB7RTIE5/Lz58uQ='
WHERE Username = 'admin';

Nueva contraseña: Admin123!
```

---

## 🎯 Próximos Pasos

1. ✅ Cambiar contraseña del admin
2. ✅ Registrar primera candidata
3. ✅ Crear álbum de fotos
4. ✅ Registrar estudiantes de prueba
5. ✅ Realizar votación de prueba
6. ✅ Consultar resultados

---

## 📚 Documentación Completa

Ver archivo: **README.md**

---

**¡Sistema listo en 10 minutos! 🎉**
