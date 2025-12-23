USE [master]
GO

-- Eski veritabanı varsa temizle (Temiz kurulum için)
IF EXISTS (SELECT name FROM sys.databases WHERE name = N'SAT242516046')
BEGIN
    ALTER DATABASE [SAT242516046] SET SINGLE_USER WITH ROLLBACK IMMEDIATE
    DROP DATABASE [SAT242516046]
END
GO

CREATE DATABASE [SAT242516046]
GO

USE [SAT242516046]
GO

-- =============================================
-- 1. KULLANICI TANIMLI TİPLER (Madde 7)
-- =============================================
CREATE TYPE [dbo].[Type_Dictionary_String_String] AS TABLE(
	[Key] [nvarchar](100) NULL,
	[Value] [nvarchar](max) NULL
)
GO

-- =============================================
-- 2. TABLOLAR VE İLİŞKİLER (Madde 5, 8, 12)
-- =============================================

-- ROL TABLOSU
CREATE TABLE [dbo].[Rol](
	[RolID] [int] IDENTITY(1,1) NOT NULL PRIMARY KEY,
	[RolAdi] [nvarchar](50) NOT NULL UNIQUE
)
GO

-- KULLANICI TABLOSU (Identity için)
CREATE TABLE [dbo].[Kullanici](
	[KullaniciID] [int] IDENTITY(1,1) NOT NULL PRIMARY KEY,
	[KullaniciAdi] [nvarchar](50) NOT NULL UNIQUE,
	[SifreHash] [nvarchar](256) NOT NULL,
	[RolID] [int] NOT NULL,
    FOREIGN KEY ([RolID]) REFERENCES [dbo].[Rol] ([RolID])
)
GO

-- PERSONEL TABLOSU
CREATE TABLE [dbo].[Personel](
	[PersonelID] [int] IDENTITY(1,1) NOT NULL PRIMARY KEY,
	[Ad] [nvarchar](50) NOT NULL,
	[Soyad] [nvarchar](50) NOT NULL,
	[Departman] [nvarchar](50) NULL,
	[BrutMaas] [decimal](10, 2) NOT NULL DEFAULT 0,
	[Email] [nvarchar](100) NULL,
	[Telefon] [nvarchar](20) NULL
)
GO

-- MAAŞ HESAPLAMA TABLOSU
CREATE TABLE [dbo].[MaasHesaplama](
	[HesapID] [int] IDENTITY(1,1) NOT NULL PRIMARY KEY,
	[PersonelID] [int] NOT NULL,
	[Donem] [nvarchar](10) NOT NULL,
	[Prim] [decimal](10, 2) DEFAULT 0,
	[Kesinti] [decimal](10, 2) DEFAULT 0,
	[NetMaas] [decimal](10, 2) NOT NULL,
	[HesapTarihi] [datetime] DEFAULT GETDATE(),
    FOREIGN KEY ([PersonelID]) REFERENCES [dbo].[Personel] ([PersonelID]) ON DELETE CASCADE
)
GO

-- İZİN TABLOSU
CREATE TABLE [dbo].[Izin](
	[IzinID] [int] IDENTITY(1,1) NOT NULL PRIMARY KEY,
	[PersonelID] [int] NOT NULL,
	[BaslangicTarihi] [date] NOT NULL,
	[BitisTarihi] [date] NOT NULL,
	[IzinTuru] [nvarchar](50) NULL,
    FOREIGN KEY ([PersonelID]) REFERENCES [dbo].[Personel] ([PersonelID]) ON DELETE CASCADE
)
GO

-- SİSTEM LOGLARI (DbLogger)
CREATE TABLE [dbo].[Logs](
	[Id] [int] IDENTITY(1,1) NOT NULL PRIMARY KEY,
	[Timestamp] [datetime] DEFAULT GETDATE(),
	[Level] [nvarchar](50) NULL,
	[Category] [nvarchar](200) NULL,
	[Message] [nvarchar](max) NULL,
	[Exception] [nvarchar](max) NULL
)
GO

-- CRUD LOGLARI (Table Trigger) (Madde 14)
CREATE TABLE [dbo].[Logs_Table](
	[Id] [int] IDENTITY(1,1) NOT NULL PRIMARY KEY,
	[TableName] [nvarchar](100) NULL,
	[RowId] [nvarchar](50) NULL,
	[OldValue] [nvarchar](max) NULL,
	[NewValue] [nvarchar](max) NULL,
	[ActionType] [nvarchar](50) NULL,
	[ActionDateTime] [datetime] DEFAULT GETDATE()
)
GO

-- =============================================
-- 3. INDEXLER (Madde 9)
-- =============================================
CREATE NONCLUSTERED INDEX [IX_Personel_AdSoyad] ON [dbo].[Personel] ([Ad] ASC, [Soyad] ASC);
CREATE NONCLUSTERED INDEX [IX_Personel_Departman] ON [dbo].[Personel] ([Departman] ASC);
CREATE NONCLUSTERED INDEX [IX_Maas_Personel] ON [dbo].[MaasHesaplama] ([PersonelID] ASC);
CREATE NONCLUSTERED INDEX [IX_Izin_Personel] ON [dbo].[Izin] ([PersonelID] ASC);
GO

-- =============================================
-- 4. VIEW (GÖRÜNÜM) (Madde 10)
-- =============================================
-- Önce View'i şematik bağlamalıyız (Indexed View şartı)
GO
ALTER VIEW [dbo].[v_PersonelBordroOzeti] WITH SCHEMABINDING
AS
SELECT 
    p.PersonelID,
    p.Ad,
    p.Soyad,
    p.Email,
    p.Telefon,
    p.Departman,
    p.BrutMaas
    -- Not: Subquery'ler Indexed View'da desteklenmez, o yüzden basit tutuyoruz
FROM dbo.Personel p;
GO

-- Şimdi Index'i Çakıyoruz
CREATE UNIQUE CLUSTERED INDEX [IX_v_PersonelBordroOzeti_PersonelID] 
ON [dbo].[v_PersonelBordroOzeti] ([PersonelID]);
GO

-- =============================================
-- 5. TRIGGERLAR (Madde 14 & 23a)
-- =============================================
-- Personel Trigger
CREATE TRIGGER [dbo].[Trg_Personel_Takip_JSON] ON [dbo].[Personel] AFTER INSERT, UPDATE, DELETE AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @table NVARCHAR(100)='Personel';
    INSERT INTO Logs_Table (TableName, RowId, ActionType, OldValue, NewValue)
    SELECT @table, COALESCE(i.PersonelID, d.PersonelID),
    CASE WHEN i.PersonelID IS NOT NULL AND d.PersonelID IS NULL THEN 'Insert' WHEN i.PersonelID IS NOT NULL AND d.PersonelID IS NOT NULL THEN 'Update' ELSE 'Delete' END,
    (SELECT * FROM deleted d2 WHERE d2.PersonelID=d.PersonelID FOR JSON PATH, WITHOUT_ARRAY_WRAPPER),
    (SELECT * FROM inserted i2 WHERE i2.PersonelID=i.PersonelID FOR JSON PATH, WITHOUT_ARRAY_WRAPPER)
    FROM inserted i FULL OUTER JOIN deleted d ON i.PersonelID=d.PersonelID;
END
GO

-- Maaş Trigger
CREATE TRIGGER [dbo].[Trg_Maas_Takip_JSON] ON [dbo].[MaasHesaplama] AFTER INSERT, UPDATE, DELETE AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @table NVARCHAR(100)='MaasHesaplama';
    INSERT INTO Logs_Table (TableName, RowId, ActionType, OldValue, NewValue)
    SELECT @table, COALESCE(i.HesapID, d.HesapID),
    CASE WHEN i.HesapID IS NOT NULL AND d.HesapID IS NULL THEN 'Insert' WHEN i.HesapID IS NOT NULL AND d.HesapID IS NOT NULL THEN 'Update' ELSE 'Delete' END,
    (SELECT * FROM deleted d2 WHERE d2.HesapID=d.HesapID FOR JSON PATH, WITHOUT_ARRAY_WRAPPER),
    (SELECT * FROM inserted i2 WHERE i2.HesapID=i.HesapID FOR JSON PATH, WITHOUT_ARRAY_WRAPPER)
    FROM inserted i FULL OUTER JOIN deleted d ON i.HesapID=d.HesapID;
END
GO

-- İzin Trigger
CREATE TRIGGER [dbo].[Trg_Izin_Takip_JSON] ON [dbo].[Izin] AFTER INSERT, UPDATE, DELETE AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @table NVARCHAR(100)='Izin';
    INSERT INTO Logs_Table (TableName, RowId, ActionType, OldValue, NewValue)
    SELECT @table, COALESCE(i.IzinID, d.IzinID),
    CASE WHEN i.IzinID IS NOT NULL AND d.IzinID IS NULL THEN 'Insert' WHEN i.IzinID IS NOT NULL AND d.IzinID IS NOT NULL THEN 'Update' ELSE 'Delete' END,
    (SELECT * FROM deleted d2 WHERE d2.IzinID=d.IzinID FOR JSON PATH, WITHOUT_ARRAY_WRAPPER),
    (SELECT * FROM inserted i2 WHERE i2.IzinID=i.IzinID FOR JSON PATH, WITHOUT_ARRAY_WRAPPER)
    FROM inserted i FULL OUTER JOIN deleted d ON i.IzinID=d.IzinID;
END
GO

-- =============================================
-- 6. STORED PROCEDURES (Madde 13)
-- =============================================
-- SP: PERSONEL LİSTELE
CREATE PROCEDURE [dbo].[sp_Personel_Listele]
    @pagination Type_Dictionary_String_String READONLY, @where Type_Dictionary_String_String READONLY
AS
BEGIN
    SELECT * FROM v_PersonelBordroOzeti ORDER BY PersonelID DESC
END
GO

-- SP: PERSONEL EKLE/GÜNCELLE/SİL
CREATE PROCEDURE [dbo].[sp_Personel_EkleGuncelleSil]
    @operation VARCHAR(10), @jsonvalues NVARCHAR(MAX)
AS
BEGIN
    SELECT * INTO #tmpP FROM OPENJSON(@jsonvalues) WITH (PersonelID INT '$.PersonelID', Ad NVARCHAR(50) '$.Ad', Soyad NVARCHAR(50) '$.Soyad', Departman NVARCHAR(50) '$.Departman', BrutMaas DECIMAL(10,2) '$.BrutMaas', Email NVARCHAR(100) '$.Email', Telefon NVARCHAR(20) '$.Telefon');
    DECLARE @rc INT = 0;
    IF @operation='add' BEGIN INSERT INTO Personel (Ad,Soyad,Departman,BrutMaas,Email,Telefon) SELECT Ad,Soyad,Departman,BrutMaas,Email,Telefon FROM #tmpP; SET @rc=@@ROWCOUNT; END
    IF @operation='update' BEGIN UPDATE p SET p.Ad=t.Ad, p.Soyad=t.Soyad, p.Departman=t.Departman, p.BrutMaas=t.BrutMaas, p.Email=t.Email, p.Telefon=t.Telefon FROM Personel p JOIN #tmpP t ON p.PersonelID=t.PersonelID; SET @rc=@@ROWCOUNT; END
    IF @operation='remove' BEGIN DELETE FROM Personel WHERE PersonelID IN (SELECT PersonelID FROM #tmpP); SET @rc=@@ROWCOUNT; END
    SELECT @operation AS [Key], IIF(@rc>0, 1, 0) AS [Value];
END
GO

-- SP: MAAŞ LİSTELE
CREATE PROCEDURE [dbo].[sp_Maas_Listele]
    @pagination Type_Dictionary_String_String READONLY, @where Type_Dictionary_String_String READONLY
AS
BEGIN
    SELECT m.*, p.Ad + ' ' + p.Soyad AS PersonelAdSoyad, p.BrutMaas AS GuncelBrutMaas 
    FROM MaasHesaplama m JOIN Personel p ON m.PersonelID = p.PersonelID ORDER BY m.Donem DESC
END
GO

-- SP: MAAŞ EKLE (Otomatik Hesapla)
CREATE PROCEDURE [dbo].[sp_Maas_EkleGuncelleSil]
    @operation VARCHAR(10), @jsonvalues NVARCHAR(MAX)
AS
BEGIN
    SELECT * INTO #tmpM FROM OPENJSON(@jsonvalues) WITH (HesapID INT '$.HesapID', PersonelID INT '$.PersonelID', Donem NVARCHAR(10) '$.Donem', Prim DECIMAL(10,2) '$.Prim', Kesinti DECIMAL(10,2) '$.Kesinti');
    DECLARE @NetMaas DECIMAL(10,2), @Brut DECIMAL(10,2);
    IF @operation IN ('add','update') BEGIN
        SELECT @Brut = BrutMaas FROM Personel WHERE PersonelID = (SELECT PersonelID FROM #tmpM);
        SELECT @NetMaas = (@Brut + ISNULL(Prim,0) - ISNULL(Kesinti,0)) FROM #tmpM;
    END
    DECLARE @rc INT = 0;
    IF @operation='add' BEGIN INSERT INTO MaasHesaplama (PersonelID, Donem, Prim, Kesinti, NetMaas) SELECT PersonelID, Donem, ISNULL(Prim,0), ISNULL(Kesinti,0), @NetMaas FROM #tmpM; SET @rc=@@ROWCOUNT; END
    IF @operation='update' BEGIN UPDATE m SET m.PersonelID=t.PersonelID, m.Donem=t.Donem, m.Prim=ISNULL(t.Prim,0), m.Kesinti=ISNULL(t.Kesinti,0), m.NetMaas=@NetMaas FROM MaasHesaplama m JOIN #tmpM t ON m.HesapID=t.HesapID; SET @rc=@@ROWCOUNT; END
    IF @operation='remove' BEGIN DELETE FROM MaasHesaplama WHERE HesapID IN (SELECT HesapID FROM #tmpM); SET @rc=@@ROWCOUNT; END
    SELECT @operation AS [Key], IIF(@rc>0, 1, 0) AS [Value];
END
GO

-- SP: İZİN LİSTELE
CREATE PROCEDURE [dbo].[sp_Izin_Listele]
    @pagination Type_Dictionary_String_String READONLY, @where Type_Dictionary_String_String READONLY
AS
BEGIN
    SELECT i.*, p.Ad + ' ' + p.Soyad AS PersonelAdSoyad, DATEDIFF(day, i.BaslangicTarihi, i.BitisTarihi) + 1 AS GunSayisi
    FROM Izin i JOIN Personel p ON i.PersonelID = p.PersonelID ORDER BY i.BaslangicTarihi DESC
END
GO

-- SP: İZİN EKLE
CREATE PROCEDURE [dbo].[sp_Izin_EkleGuncelleSil]
    @operation VARCHAR(10), @jsonvalues NVARCHAR(MAX)
AS
BEGIN
    SELECT * INTO #tmpI FROM OPENJSON(@jsonvalues) WITH (IzinID INT '$.IzinID', PersonelID INT '$.PersonelID', BaslangicTarihi DATE '$.BaslangicTarihi', BitisTarihi DATE '$.BitisTarihi', IzinTuru NVARCHAR(50) '$.IzinTuru');
    DECLARE @rc INT = 0;
    IF @operation='add' BEGIN INSERT INTO Izin (PersonelID, BaslangicTarihi, BitisTarihi, IzinTuru) SELECT PersonelID, BaslangicTarihi, BitisTarihi, IzinTuru FROM #tmpI; SET @rc=@@ROWCOUNT; END
    IF @operation='update' BEGIN UPDATE i SET i.PersonelID=t.PersonelID, i.BaslangicTarihi=t.BaslangicTarihi, i.BitisTarihi=t.BitisTarihi, i.IzinTuru=t.IzinTuru FROM Izin i JOIN #tmpI t ON i.IzinID=t.IzinID; SET @rc=@@ROWCOUNT; END
    IF @operation='remove' BEGIN DELETE FROM Izin WHERE IzinID IN (SELECT IzinID FROM #tmpI); SET @rc=@@ROWCOUNT; END
    SELECT @operation AS [Key], IIF(@rc>0, 1, 0) AS [Value];
END
GO

-- SP: ROL LİSTELE/EKLE
CREATE PROCEDURE [dbo].[sp_Rol_Listele] @p1 Type_Dictionary_String_String READONLY, @p2 Type_Dictionary_String_String READONLY AS BEGIN SELECT * FROM Rol ORDER BY RolAdi END
GO
CREATE PROCEDURE [dbo].[sp_Rol_EkleGuncelleSil] @operation VARCHAR(10), @jsonvalues NVARCHAR(MAX) AS BEGIN 
    SELECT * INTO #tmp FROM OPENJSON(@jsonvalues) WITH (RolID INT '$.RolID', RolAdi NVARCHAR(50) '$.RolAdi');
    DECLARE @rc INT=0;
    IF @operation='add' INSERT INTO Rol SELECT RolAdi FROM #tmp; 
    IF @operation='update' UPDATE r SET r.RolAdi=t.RolAdi FROM Rol r JOIN #tmp t ON r.RolID=t.RolID;
    IF @operation='remove' DELETE r FROM Rol r JOIN #tmp t ON r.RolID=t.RolID;
    SET @rc=@@ROWCOUNT; SELECT @operation, IIF(@rc>0,1,0);
END
GO

-- SP: KULLANICI LİSTELE/EKLE
CREATE PROCEDURE [dbo].[sp_Kullanici_Listele] @p1 Type_Dictionary_String_String READONLY, @p2 Type_Dictionary_String_String READONLY AS BEGIN 
    SELECT k.KullaniciID, k.KullaniciAdi, k.RolID, r.RolAdi, '***' AS SifreHash FROM Kullanici k JOIN Rol r ON k.RolID=r.RolID ORDER BY k.KullaniciAdi 
END
GO
CREATE PROCEDURE [dbo].[sp_Kullanici_EkleGuncelleSil] @operation VARCHAR(10), @jsonvalues NVARCHAR(MAX) AS BEGIN 
    SELECT * INTO #tmp FROM OPENJSON(@jsonvalues) WITH (KullaniciID INT '$.KullaniciID', KullaniciAdi NVARCHAR(50) '$.KullaniciAdi', SifreHash NVARCHAR(256) '$.SifreHash', RolID INT '$.RolID');
    DECLARE @rc INT=0;
    IF @operation='add' INSERT INTO Kullanici (KullaniciAdi, SifreHash, RolID) SELECT KullaniciAdi, SifreHash, RolID FROM #tmp; 
    IF @operation='update' UPDATE k SET k.KullaniciAdi=t.KullaniciAdi, k.RolID=t.RolID, k.SifreHash=CASE WHEN t.SifreHash IS NOT NULL AND t.SifreHash<>'' THEN t.SifreHash ELSE k.SifreHash END FROM Kullanici k JOIN #tmp t ON k.KullaniciID=t.KullaniciID;
    IF @operation='remove' DELETE k FROM Kullanici k JOIN #tmp t ON k.KullaniciID=t.KullaniciID;
    SET @rc=@@ROWCOUNT; SELECT @operation, IIF(@rc>0,1,0);
END
GO

-- SP: SİSTEM LOGLARI LİSTELE (Son 1000)
CREATE PROCEDURE [dbo].[sp_SystemLogs_Listele] @p1 Type_Dictionary_String_String READONLY, @p2 Type_Dictionary_String_String READONLY AS BEGIN 
    SELECT TOP 1000 Id, [Timestamp], [Level], [Category], [Message], [Exception] FROM Logs ORDER BY [Timestamp] DESC 
END
GO

-- SP: LOGS_TABLE LİSTELE (CRUD LOGLARI)
CREATE PROCEDURE [dbo].[sp_Logs_Table_Listele] @p1 Type_Dictionary_String_String READONLY, @p2 Type_Dictionary_String_String READONLY AS BEGIN 
    SELECT TOP 500 Id, TableName, ActionType, ActionDateTime, LEFT(OldValue,50) OldValue, LEFT(NewValue,50) NewValue FROM Logs_Table ORDER BY ActionDateTime DESC 
END
GO