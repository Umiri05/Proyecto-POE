-- =============================================
-- Script de Base de Datos - Sistema Reina FIEC Mejorado
-- Base de datos: SQL Server 2012+
-- Versión: 2.0
-- Fecha: Enero 2026
-- =============================================

USE [master]
GO

-- Crear la base de datos
IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = N'ReinaFIECDB')
BEGIN
    CREATE DATABASE [ReinaFIECDB]
END
GO

USE [ReinaFIECDB]
GO

-- =============================================
-- TABLA: Usuarios (con autenticación)
-- =============================================
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Usuarios]'))
BEGIN
    CREATE TABLE [dbo].[Usuarios] (
        [Id] INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        [Username] NVARCHAR(50) NOT NULL UNIQUE,
        [PasswordHash] NVARCHAR(255) NOT NULL,
        [Rol] INT NOT NULL CHECK ([Rol] IN (1, 2)), -- 1=Admin, 2=Estudiante
        [Matricula] NVARCHAR(20) NULL UNIQUE,
        [Email] NVARCHAR(100) NOT NULL,
        [NombreCompleto] NVARCHAR(200) NOT NULL,
        [FechaCreacion] DATETIME NOT NULL DEFAULT GETDATE(),
        [UltimoAcceso] DATETIME NULL,
        [Activo] BIT NOT NULL DEFAULT 1,
        [HaVotadoReina] BIT NOT NULL DEFAULT 0,
        [HaVotadoFotogenia] BIT NOT NULL DEFAULT 0,
        [FechaVotoReina] DATETIME NULL,
        [FechaVotoFotogenia] DATETIME NULL,
        INDEX [IX_Username] ([Username]),
        INDEX [IX_Matricula] ([Matricula]),
        INDEX [IX_Rol] ([Rol])
    )
END
GO

-- =============================================
-- TABLA: Candidatas (mejorada)
-- =============================================
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Candidatas]'))
BEGIN
    CREATE TABLE [dbo].[Candidatas] (
        [Id] INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        [NombreCompleto] NVARCHAR(200) NOT NULL,
        [Cedula] NVARCHAR(10) NOT NULL UNIQUE,
        [FechaNacimiento] DATE NOT NULL,
        [Carrera] NVARCHAR(100) NOT NULL,
        [Semestre] INT NOT NULL CHECK ([Semestre] BETWEEN 1 AND 12),
        [Email] NVARCHAR(100) NOT NULL UNIQUE,
        [Telefono] NVARCHAR(15) NULL,
        [PromedioAcademico] DECIMAL(4,2) NOT NULL CHECK ([PromedioAcademico] BETWEEN 0 AND 10),
        [Pasatiempos] NVARCHAR(MAX) NULL,
        [Habilidades] NVARCHAR(MAX) NULL,
        [Intereses] NVARCHAR(MAX) NULL,
        [AspiracionesFuturo] NVARCHAR(MAX) NULL,
        [FotoPrincipalUrl] NVARCHAR(500) NULL,
        [FechaRegistro] DATETIME NOT NULL DEFAULT GETDATE(),
        [RegistradoPor] INT NOT NULL,
        [Activo] BIT NOT NULL DEFAULT 1,
        CONSTRAINT [FK_Candidatas_Usuarios] FOREIGN KEY ([RegistradoPor]) 
            REFERENCES [dbo].[Usuarios] ([Id]),
        INDEX [IX_Cedula] ([Cedula]),
        INDEX [IX_Carrera] ([Carrera]),
        INDEX [IX_Email] ([Email])
    )
END
GO

-- =============================================
-- TABLA: Votos (con auditoría)
-- =============================================
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Votos]'))
BEGIN
    CREATE TABLE [dbo].[Votos] (
        [Id] INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        [UsuarioId] INT NOT NULL,
        [CandidataId] INT NOT NULL,
        [TipoVoto] INT NOT NULL CHECK ([TipoVoto] IN (1, 2)), -- 1=Reina, 2=Fotogenia
        [FechaVoto] DATETIME NOT NULL DEFAULT GETDATE(),
        [IpAddress] NVARCHAR(45) NULL,
        [UserAgent] NVARCHAR(500) NULL,
        CONSTRAINT [FK_Votos_Usuarios] FOREIGN KEY ([UsuarioId]) 
            REFERENCES [dbo].[Usuarios] ([Id]),
        CONSTRAINT [FK_Votos_Candidatas] FOREIGN KEY ([CandidataId]) 
            REFERENCES [dbo].[Candidatas] ([Id]),
        CONSTRAINT [UQ_Usuario_TipoVoto] UNIQUE ([UsuarioId], [TipoVoto]),
        INDEX [IX_CandidataId_TipoVoto] ([CandidataId], [TipoVoto]),
        INDEX [IX_FechaVoto] ([FechaVoto])
    )
END
GO

-- =============================================
-- TABLA: AuditoriaVotos
-- =============================================
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[AuditoriaVotos]'))
BEGIN
    CREATE TABLE [dbo].[AuditoriaVotos] (
        [Id] INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        [VotoId] INT NULL,
        [Accion] NVARCHAR(50) NOT NULL, -- INSERT, UPDATE, DELETE
        [UsuarioId] INT NOT NULL,
        [CandidataId] INT NOT NULL,
        [TipoVoto] INT NOT NULL,
        [FechaAccion] DATETIME NOT NULL DEFAULT GETDATE(),
        [DetallesJson] NVARCHAR(MAX) NULL,
        [IpAddress] NVARCHAR(45) NULL,
        INDEX [IX_FechaAccion] ([FechaAccion]),
        INDEX [IX_UsuarioId] ([UsuarioId])
    )
END
GO

-- =============================================
-- TABLA: Albumes
-- =============================================
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Albumes]'))
BEGIN
    CREATE TABLE [dbo].[Albumes] (
        [Id] INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        [Nombre] NVARCHAR(100) NOT NULL,
        [Descripcion] NVARCHAR(500) NULL,
        [CandidataId] INT NOT NULL,
        [FechaCreacion] DATETIME NOT NULL DEFAULT GETDATE(),
        CONSTRAINT [FK_Albumes_Candidatas] FOREIGN KEY ([CandidataId]) 
            REFERENCES [dbo].[Candidatas] ([Id]) ON DELETE CASCADE,
        INDEX [IX_CandidataId] ([CandidataId])
    )
END
GO

-- =============================================
-- TABLA: Fotos
-- =============================================
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Fotos]'))
BEGIN
    CREATE TABLE [dbo].[Fotos] (
        [Id] INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        [RutaArchivo] NVARCHAR(500) NOT NULL,
        [Titulo] NVARCHAR(200) NOT NULL,
        [Descripcion] NVARCHAR(500) NULL,
        [AlbumId] INT NOT NULL,
        [FechaSubida] DATETIME NOT NULL DEFAULT GETDATE(),
        CONSTRAINT [FK_Fotos_Albumes] FOREIGN KEY ([AlbumId]) 
            REFERENCES [dbo].[Albumes] ([Id]) ON DELETE CASCADE,
        INDEX [IX_AlbumId] ([AlbumId])
    )
END
GO

-- =============================================
-- TABLA: Comentarios
-- =============================================
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Comentarios]'))
BEGIN
    CREATE TABLE [dbo].[Comentarios] (
        [Id] INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        [Texto] NVARCHAR(500) NOT NULL,
        [Autor] NVARCHAR(100) NOT NULL,
        [UsuarioId] INT NOT NULL,
        [FotoId] INT NOT NULL,
        [FechaComentario] DATETIME NOT NULL DEFAULT GETDATE(),
        [Aprobado] BIT NOT NULL DEFAULT 1,
        CONSTRAINT [FK_Comentarios_Usuarios] FOREIGN KEY ([UsuarioId]) 
            REFERENCES [dbo].[Usuarios] ([Id]),
        CONSTRAINT [FK_Comentarios_Fotos] FOREIGN KEY ([FotoId]) 
            REFERENCES [dbo].[Fotos] ([Id]) ON DELETE CASCADE,
        INDEX [IX_FotoId] ([FotoId]),
        INDEX [IX_UsuarioId] ([UsuarioId])
    )
END
GO

-- =============================================
-- PROCEDIMIENTOS ALMACENADOS
-- =============================================

-- Obtener resultados de votación
IF OBJECT_ID('sp_ObtenerResultados', 'P') IS NOT NULL
    DROP PROCEDURE sp_ObtenerResultados
GO

CREATE PROCEDURE sp_ObtenerResultados
    @TipoVoto INT
AS
BEGIN
    SET NOCOUNT ON;
    
    WITH VotacionCTE AS (
        SELECT 
            c.Id as CandidataId,
            c.NombreCompleto,
            c.Carrera,
            c.FotoPrincipalUrl,
            COUNT(v.Id) as TotalVotos,
            RANK() OVER (ORDER BY COUNT(v.Id) DESC) as Posicion
        FROM Candidatas c
        LEFT JOIN Votos v ON c.Id = v.CandidataId AND v.TipoVoto = @TipoVoto
        WHERE c.Activo = 1
        GROUP BY c.Id, c.NombreCompleto, c.Carrera, c.FotoPrincipalUrl
    )
    SELECT *, 
           CASE WHEN TotalVotos > 0 THEN 
                CAST(TotalVotos AS DECIMAL(10,2)) / 
                (SELECT SUM(TotalVotos) FROM VotacionCTE) * 100 
           ELSE 0 END as PorcentajeVotos,
           CASE WHEN Posicion = 1 THEN 1 ELSE 0 END as EsGanadora
    FROM VotacionCTE
    ORDER BY TotalVotos DESC;
END
GO

-- Verificar si usuario puede votar
IF OBJECT_ID('sp_PuedeVotar', 'P') IS NOT NULL
    DROP PROCEDURE sp_PuedeVotar
GO

CREATE PROCEDURE sp_PuedeVotar
    @UsuarioId INT,
    @TipoVoto INT
AS
BEGIN
    SET NOCOUNT ON;
    
    IF EXISTS (SELECT 1 FROM Votos WHERE UsuarioId = @UsuarioId AND TipoVoto = @TipoVoto)
    BEGIN
        SELECT 0 as PuedeVotar, 'Ya ha votado en esta categoría' as Mensaje
    END
    ELSE
    BEGIN
        SELECT 1 as PuedeVotar, 'Puede votar' as Mensaje
    END
END
GO

-- Estadísticas generales
IF OBJECT_ID('sp_ObtenerEstadisticasGenerales', 'P') IS NOT NULL
    DROP PROCEDURE sp_ObtenerEstadisticasGenerales
GO

CREATE PROCEDURE sp_ObtenerEstadisticasGenerales
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        (SELECT COUNT(*) FROM Candidatas WHERE Activo = 1) as TotalCandidatas,
        (SELECT COUNT(*) FROM Usuarios WHERE Rol = 2 AND Activo = 1) as TotalEstudiantes,
        (SELECT COUNT(DISTINCT UsuarioId) FROM Votos WHERE TipoVoto = 1) as VotantesReina,
        (SELECT COUNT(DISTINCT UsuarioId) FROM Votos WHERE TipoVoto = 2) as VotantesFotogenia,
        (SELECT COUNT(*) FROM Votos WHERE TipoVoto = 1) as TotalVotosReina,
        (SELECT COUNT(*) FROM Votos WHERE TipoVoto = 2) as TotalVotosFotogenia,
        (SELECT TOP 1 NombreCompleto FROM Candidatas c 
         INNER JOIN Votos v ON c.Id = v.CandidataId 
         WHERE v.TipoVoto = 1 
         GROUP BY c.Id, c.NombreCompleto 
         ORDER BY COUNT(v.Id) DESC) as LiderReina,
        (SELECT TOP 1 NombreCompleto FROM Candidatas c 
         INNER JOIN Votos v ON c.Id = v.CandidataId 
         WHERE v.TipoVoto = 2 
         GROUP BY c.Id, c.NombreCompleto 
         ORDER BY COUNT(v.Id) DESC) as LiderFotogenia
END
GO

-- =============================================
-- DATOS INICIALES
-- =============================================

-- Insertar usuario administrador por defecto
-- Usuario: admin, Contraseña: Admin123! (cambiar en producción)
IF NOT EXISTS (SELECT 1 FROM Usuarios WHERE Username = 'admin')
BEGIN
    INSERT INTO Usuarios (Username, PasswordHash, Rol, Email, NombreCompleto, Activo)
    VALUES ('admin', 'HAkdO7/mRkP7QqKZ9w+JkA==+gZdV8s1QqB7RTIE5/Lz58uQ=', 1, 
            'admin@fiec.edu.ec', 'Administrador del Sistema', 1)
END
GO

-- Insertar carreras de ejemplo
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Carreras]'))
BEGIN
    CREATE TABLE [dbo].[Carreras] (
        [Id] INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        [Nombre] NVARCHAR(100) NOT NULL UNIQUE
    )
    
    INSERT INTO Carreras (Nombre) VALUES
    ('Ingeniería en Sistemas Computacionales'),
    ('Ingeniería en Electrónica y Automatización'),
    ('Ingeniería en Electricidad'),
    ('Ingeniería en Telecomunicaciones'),
    ('Ingeniería en Ciencias de la Computación')
END
GO

PRINT '=============================================';
PRINT 'Base de datos creada exitosamente';
PRINT 'Tablas: Usuarios, Candidatas, Votos, AuditoriaVotos, Albumes, Fotos, Comentarios, Carreras';
PRINT 'Usuario admin creado - Username: admin, Password: Admin123!';
PRINT 'IMPORTANTE: Cambiar la contraseña del administrador';
PRINT '=============================================';
GO
