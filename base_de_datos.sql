IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = N'SegurosDB')
BEGIN
    CREATE DATABASE SegurosDB;
END


-- TABLAS
-- se han obtenido las tablas desde el gestor de sql server management studio

USE [SegurosDB]
GO

/****** Object:  Table [dbo].[Seguros]    Script Date: 10/24/2024 10:29:36 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[Seguros](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Nombre] [varchar](100) NOT NULL,
	[Codigo] [varchar](20) NOT NULL,
	[SumaAsegurada] [decimal](18, 2) NOT NULL,
	[Prima] [decimal](18, 2) NOT NULL,
	[EdadMinima] [int] NULL,
	[EdadMaxima] [int] NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY],
UNIQUE NONCLUSTERED 
(
	[Codigo] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO




--
USE [SegurosDB]
GO

/****** Object:  Table [dbo].[Asegurados]    Script Date: 10/24/2024 10:28:06 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[Asegurados](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Cedula] [varchar](20) NOT NULL,
	[Nombre] [varchar](100) NOT NULL,
	[Telefono] [varchar](20) NULL,
	[Edad] [int] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY],
UNIQUE NONCLUSTERED 
(
	[Cedula] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

--
USE [SegurosDB]
GO

/****** Object:  Table [dbo].[AseguradosSeguros]    Script Date: 10/24/2024 10:30:12 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[AseguradosSeguros](
	[AseguradoId] [int] NOT NULL,
	[SeguroId] [int] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[AseguradoId] ASC,
	[SeguroId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[AseguradosSeguros]  WITH CHECK ADD FOREIGN KEY([AseguradoId])
REFERENCES [dbo].[Asegurados] ([Id])
ON DELETE CASCADE
GO

ALTER TABLE [dbo].[AseguradosSeguros]  WITH CHECK ADD FOREIGN KEY([SeguroId])
REFERENCES [dbo].[Seguros] ([Id])
ON DELETE CASCADE
GO




-- STORED PROCEDURES
USE [SegurosDB]
GO

CREATE PROCEDURE [dbo].[sp_ActualizarAsegurado]
    @Id INT,
    @Cedula VARCHAR(20),
    @Nombre VARCHAR(100),
    @Telefono VARCHAR(20),
    @Edad INT
AS
BEGIN
    IF EXISTS (SELECT 1 FROM Asegurados WHERE Cedula = @Cedula AND Id != @Id)
        THROW 50001, 'La cédula ya existe en el sistema.', 1;
    
    UPDATE Asegurados
    SET Cedula = @Cedula,
        Nombre = @Nombre,
        Telefono = @Telefono,
        Edad = @Edad
    WHERE Id = @Id
END


CREATE PROCEDURE [dbo].[sp_ActualizarSeguro]
    @Id INT,
    @Nombre VARCHAR(100),
    @Codigo VARCHAR(20),
    @SumaAsegurada DECIMAL(18,2),
    @Prima DECIMAL(18,2),
	@EdadMinima INT = NULL,
	@EdadMaxima INT = NULL
AS
BEGIN
    IF EXISTS (SELECT 1 FROM Seguros WHERE Codigo = @Codigo AND Id != @Id)
        THROW 50001, 'El código del seguro ya existe.', 1;
	
	-- tomar valores actuales de edad si no se proporciona el rango de edad adecuado
	IF @EdadMinima IS NULL
    BEGIN
        SELECT 
            @EdadMinima = COALESCE(@EdadMinima, EdadMinima)
        FROM Seguros 
        WHERE Id = @Id
    END

	IF @EdadMaxima IS NULL
	BEGIN
        SELECT
            @EdadMaxima = COALESCE(@EdadMaxima, EdadMaxima)
        FROM Seguros 
        WHERE Id = @Id
    END

    UPDATE Seguros
    SET Nombre = @Nombre,
        Codigo = @Codigo,
        SumaAsegurada = @SumaAsegurada,
        Prima = @Prima,
		EdadMinima = @EdadMinima,
		EdadMaxima = @EdadMaxima
    WHERE Id = @Id
END


--
CREATE PROCEDURE [dbo].[sp_AsignarSeguroAAsegurado]
    @AseguradoId INT,
    @SeguroId INT
AS
BEGIN
    IF NOT EXISTS (SELECT 1 FROM AseguradosSeguros 
                  WHERE AseguradoId = @AseguradoId AND SeguroId = @SeguroId)
    BEGIN
        INSERT INTO AseguradosSeguros (AseguradoId, SeguroId)
        VALUES (@AseguradoId, @SeguroId)
    END
END


--
CREATE PROCEDURE [dbo].[sp_BorrarSegurosDeAsegurado]
    @AseguradoId INT
AS
BEGIN
    IF EXISTS (SELECT 1 FROM AseguradosSeguros 
                  WHERE AseguradoId = @AseguradoId)
    BEGIN
        delete from AseguradosSeguros where AseguradoId=@AseguradoId
    END
END

--
CREATE PROCEDURE [dbo].[sp_BuscarAseguradoPorCedula]
    @Cedula VARCHAR(20)
AS
BEGIN
    SELECT a.Id, a.Cedula, a.Nombre, a.Telefono, a.Edad,
           s.Id as SeguroId, s.Nombre as SeguroNombre, s.Codigo as SeguroCodigo
    FROM Asegurados a
    LEFT JOIN AseguradosSeguros ass ON a.Id = ass.AseguradoId
    LEFT JOIN Seguros s ON ass.SeguroId = s.Id
    WHERE a.Cedula = @Cedula
END


--
CREATE PROCEDURE [dbo].[sp_BuscarAseguradosPorSeguro]
    @CodigoSeguro VARCHAR(20)
AS
BEGIN
    SELECT a.Id, a.Cedula, a.Nombre, a.Telefono, a.Edad
    FROM Asegurados a
    INNER JOIN AseguradosSeguros ass ON a.Id = ass.AseguradoId
    INNER JOIN Seguros s ON ass.SeguroId = s.Id
    WHERE s.Codigo = @CodigoSeguro
END


--
CREATE PROCEDURE [dbo].[sp_CrearAsegurado]
    @Cedula VARCHAR(20),
    @Nombre VARCHAR(100),
    @Telefono VARCHAR(20),
    @Edad INT,
    @Id INT OUTPUT  -- agregar output parameter para id
AS
BEGIN
    -- revisar si la identificacion ya existe
    IF EXISTS (SELECT 1 FROM Asegurados WHERE Cedula = @Cedula)
        THROW 50001, 'La cédula ya existe en el sistema.', 1;

    -- insertar en nuevo registro
    INSERT INTO Asegurados (Cedula, Nombre, Telefono, Edad)
    VALUES (@Cedula, @Nombre, @Telefono, @Edad);

    -- obtener el id de la fila recien ingresada
    SET @Id = SCOPE_IDENTITY();  -- asignar el id obtenido al param de salida
END


--
CREATE PROCEDURE [dbo].[sp_CrearSeguro]
    @Nombre VARCHAR(100),
    @Codigo VARCHAR(20),
    @SumaAsegurada DECIMAL(18,2),
    @Prima DECIMAL(18,2),
	@EdadMinima int = 0,
	@EdadMaxima int = 120,
    @Id INT OUTPUT  -- Agregar parametro de salida de ID
AS
BEGIN
    -- revisar si el código del seguro ya existe
    IF EXISTS (SELECT 1 FROM Seguros WHERE Codigo = @Codigo)
        THROW 50001, 'El código del seguro ya existe.', 1;
	
    -- insertar el nuevo registro
    INSERT INTO Seguros (Nombre, Codigo, SumaAsegurada, Prima, EdadMinima, EdadMaxima)
    VALUES (@Nombre, @Codigo, @SumaAsegurada, @Prima, @EdadMinima, @EdadMaxima);

    -- obtener el Id del registro recién ingresado
    SET @Id = SCOPE_IDENTITY();  -- asigna dicho id al parametro de salida
END


--
CREATE PROCEDURE [dbo].[sp_EliminarAsegurado]
    @Id INT
AS
BEGIN
    BEGIN TRANSACTION
        DELETE FROM AseguradosSeguros WHERE AseguradoId = @Id
        DELETE FROM Asegurados WHERE Id = @Id
    COMMIT
END


--
CREATE PROCEDURE [dbo].[sp_EliminarSeguro]
    @Id INT
AS
BEGIN
    IF EXISTS (SELECT 1 FROM AseguradosSeguros WHERE SeguroId = @Id)
        THROW 50001, 'No se puede eliminar el seguro porque tiene asegurados asociados.', 1;
    
    DELETE FROM Seguros WHERE Id = @Id
END


--
CREATE PROCEDURE [dbo].[sp_ListarSeguros]
AS
BEGIN
    SELECT Id, Nombre, Codigo, SumaAsegurada, Prima, EdadMinima, EdadMaxima
    FROM Seguros
END


--
CREATE PROCEDURE [dbo].[sp_ObtenerAsegurado]
    @Id INT
AS
BEGIN
    SELECT Id, Cedula, Nombre, Telefono, Edad
    FROM Asegurados
    WHERE Id = @Id
END

--
CREATE PROCEDURE [dbo].[sp_ObtenerSeguro]
    @Id INT
AS
BEGIN
    SELECT Id, Nombre, Codigo, SumaAsegurada, Prima
    FROM Seguros
    WHERE Id = @Id
END


--
ALTER PROCEDURE [dbo].[sp_ObtenerSegurosDeAseguradoId]
    @AseguradoId INT
AS
BEGIN
    SET NOCOUNT ON;
	-- TODO revisar
    -- revisar si el asegurado existe
    --IF NOT EXISTS (SELECT 1 FROM Asegurados WHERE Id = @AseguradoId)
    --BEGIN
    --    RAISERROR('El asegurado con ID %d no existe.', 16, 1, @AseguradoId);
    --    RETURN;
    --END;

    ---- Revisar si el asegurado tiene algun seguro asociado
    --IF NOT EXISTS (SELECT 1 FROM AseguradosSeguros WHERE AseguradoId = @AseguradoId)
    --BEGIN
    --    RAISERROR('El asegurado con ID %d no tiene seguros asociados.', 16, 1, @AseguradoId);
    --    RETURN;
    --END;

    -- si todas las validaciones pasan, obtener los datos
    SELECT 
        s.Id AS SeguroId,
        s.Nombre AS NombreSeguro,
        s.Codigo,
        s.SumaAsegurada,
        s.Prima,
        a.Id AS AseguradoId,
        a.Cedula,
        a.Nombre AS NombreAsegurado,
        a.Telefono,
        a.Edad
    FROM Seguros s
    INNER JOIN AseguradosSeguros ass ON s.Id = ass.SeguroId
    INNER JOIN Asegurados a ON ass.AseguradoId = a.Id
    WHERE ass.AseguradoId = @AseguradoId;
END;


--
CREATE PROCEDURE [dbo].[sp_sp_BorrarSegurosDeAsegurado]
    @AseguradoId INT
AS
BEGIN
    IF EXISTS (SELECT 1 FROM AseguradosSeguros 
                  WHERE AseguradoId = @AseguradoId)
    BEGIN
        delete from AseguradosSeguros where AseguradoId=@AseguradoId
    END
END