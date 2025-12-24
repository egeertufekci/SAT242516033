USE [SAT242516033];
GO

IF OBJECT_ID('dbo.Kullanici', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.Kullanici (
        KullaniciId INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        KullaniciAdi NVARCHAR(100) NOT NULL,
        AdSoyad NVARCHAR(150) NULL,
        Email NVARCHAR(255) NULL,
        SifreHash NVARCHAR(200) NULL,
        AktifMi BIT NOT NULL CONSTRAINT DF_Kullanici_AktifMi DEFAULT 1,
        KayitTarihi DATETIME2 NOT NULL CONSTRAINT DF_Kullanici_KayitTarihi DEFAULT SYSUTCDATETIME()
    );
END
GO

IF OBJECT_ID('dbo.UretimHatti', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.UretimHatti (
        UretimHattiId INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        HatKodu NVARCHAR(50) NOT NULL,
        HatAdi NVARCHAR(150) NOT NULL,
        Lokasyon NVARCHAR(150) NULL,
        AktifMi BIT NOT NULL CONSTRAINT DF_UretimHatti_AktifMi DEFAULT 1
    );
END
GO

IF OBJECT_ID('dbo.HataTipi', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.HataTipi (
        HataTipiId INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        HataKodu NVARCHAR(50) NOT NULL,
        HataAdi NVARCHAR(150) NOT NULL,
        Aciklama NVARCHAR(250) NULL,
        KritikSeviye INT NOT NULL CONSTRAINT DF_HataTipi_KritikSeviye DEFAULT 1
    );
END
GO

IF OBJECT_ID('dbo.HataKaydi', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.HataKaydi (
        HataKaydiId INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        UretimHattiId INT NOT NULL,
        HataTipiId INT NOT NULL,
        KullaniciId INT NULL,
        Durum NVARCHAR(30) NOT NULL CONSTRAINT DF_HataKaydi_Durum DEFAULT N'Açık',
        BildirimTarihi DATETIME2 NOT NULL CONSTRAINT DF_HataKaydi_BildirimTarihi DEFAULT SYSUTCDATETIME(),
        Aciklama NVARCHAR(500) NULL,
        CONSTRAINT FK_HataKaydi_UretimHatti FOREIGN KEY (UretimHattiId) REFERENCES dbo.UretimHatti(UretimHattiId),
        CONSTRAINT FK_HataKaydi_HataTipi FOREIGN KEY (HataTipiId) REFERENCES dbo.HataTipi(HataTipiId),
        CONSTRAINT FK_HataKaydi_Kullanici FOREIGN KEY (KullaniciId) REFERENCES dbo.Kullanici(KullaniciId)
    );
END
GO

IF OBJECT_ID('dbo.LogsTable', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.LogsTable (
        LogId INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        LogLevel NVARCHAR(20) NOT NULL CONSTRAINT DF_LogsTable_LogLevel DEFAULT N'Info',
        LogMessage NVARCHAR(MAX) NOT NULL,
        Kaynak NVARCHAR(150) NULL,
        LogTarihi DATETIME2 NOT NULL CONSTRAINT DF_LogsTable_LogTarihi DEFAULT SYSUTCDATETIME(),
        KullaniciId INT NULL,
        HataKaydiId INT NULL,
        CONSTRAINT FK_LogsTable_Kullanici FOREIGN KEY (KullaniciId) REFERENCES dbo.Kullanici(KullaniciId),
        CONSTRAINT FK_LogsTable_HataKaydi FOREIGN KEY (HataKaydiId) REFERENCES dbo.HataKaydi(HataKaydiId)
    );
END
GO

CREATE OR ALTER PROCEDURE dbo.sp_Kullanici_List
AS
BEGIN
    SET NOCOUNT ON;
    SELECT KullaniciId, KullaniciAdi, AdSoyad, Email, SifreHash, AktifMi, KayitTarihi
    FROM dbo.Kullanici;
END
GO

CREATE OR ALTER PROCEDURE dbo.sp_Kullanici_Login
    @kullaniciAdi NVARCHAR(100),
    @sifre NVARCHAR(200)
AS
BEGIN
    SET NOCOUNT ON;
    SELECT TOP 1 KullaniciId, KullaniciAdi, AdSoyad, Email, SifreHash, AktifMi, KayitTarihi
    FROM dbo.Kullanici
    WHERE KullaniciAdi = @kullaniciAdi AND SifreHash = @sifre AND AktifMi = 1;
END
GO

CREATE OR ALTER PROCEDURE dbo.sp_Kullanici_Add_Update_Remove
    @operation NVARCHAR(20),
    @jsonvalues NVARCHAR(MAX)
AS
BEGIN
    SET NOCOUNT ON;

    IF @operation = 'add'
    BEGIN
        INSERT INTO dbo.Kullanici (KullaniciAdi, AdSoyad, Email, SifreHash, AktifMi, KayitTarihi)
        SELECT KullaniciAdi, AdSoyad, Email, SifreHash, AktifMi, KayitTarihi
        FROM OPENJSON(@jsonvalues)
        WITH (
            KullaniciAdi NVARCHAR(100),
            AdSoyad NVARCHAR(150),
            Email NVARCHAR(255),
            SifreHash NVARCHAR(200),
            AktifMi BIT,
            KayitTarihi DATETIME2
        );
    END

    IF @operation = 'update'
    BEGIN
        UPDATE k
        SET k.KullaniciAdi = j.KullaniciAdi,
            k.AdSoyad = j.AdSoyad,
            k.Email = j.Email,
            k.SifreHash = j.SifreHash,
            k.AktifMi = j.AktifMi
        FROM dbo.Kullanici k
        CROSS APPLY OPENJSON(@jsonvalues)
        WITH (
            KullaniciId INT,
            KullaniciAdi NVARCHAR(100),
            AdSoyad NVARCHAR(150),
            Email NVARCHAR(255),
            SifreHash NVARCHAR(200),
            AktifMi BIT
        ) j
        WHERE k.KullaniciId = j.KullaniciId;
    END

    IF @operation = 'remove'
    BEGIN
        DELETE k
        FROM dbo.Kullanici k
        CROSS APPLY OPENJSON(@jsonvalues)
        WITH (KullaniciId INT) j
        WHERE k.KullaniciId = j.KullaniciId;
    END

    SELECT 'KullaniciIslem' AS [Key], 1 AS [Value];
END
GO

CREATE OR ALTER PROCEDURE dbo.sp_UretimHatti_List
AS
BEGIN
    SET NOCOUNT ON;
    SELECT UretimHattiId, HatKodu, HatAdi, Lokasyon, AktifMi
    FROM dbo.UretimHatti;
END
GO

CREATE OR ALTER PROCEDURE dbo.sp_UretimHatti_Add_Update_Remove
    @operation NVARCHAR(20),
    @jsonvalues NVARCHAR(MAX)
AS
BEGIN
    SET NOCOUNT ON;

    IF @operation = 'add'
    BEGIN
        INSERT INTO dbo.UretimHatti (HatKodu, HatAdi, Lokasyon, AktifMi)
        SELECT HatKodu, HatAdi, Lokasyon, AktifMi
        FROM OPENJSON(@jsonvalues)
        WITH (
            HatKodu NVARCHAR(50),
            HatAdi NVARCHAR(150),
            Lokasyon NVARCHAR(150),
            AktifMi BIT
        );
    END

    IF @operation = 'update'
    BEGIN
        UPDATE u
        SET u.HatKodu = j.HatKodu,
            u.HatAdi = j.HatAdi,
            u.Lokasyon = j.Lokasyon,
            u.AktifMi = j.AktifMi
        FROM dbo.UretimHatti u
        CROSS APPLY OPENJSON(@jsonvalues)
        WITH (
            UretimHattiId INT,
            HatKodu NVARCHAR(50),
            HatAdi NVARCHAR(150),
            Lokasyon NVARCHAR(150),
            AktifMi BIT
        ) j
        WHERE u.UretimHattiId = j.UretimHattiId;
    END

    IF @operation = 'remove'
    BEGIN
        DELETE u
        FROM dbo.UretimHatti u
        CROSS APPLY OPENJSON(@jsonvalues)
        WITH (UretimHattiId INT) j
        WHERE u.UretimHattiId = j.UretimHattiId;
    END

    SELECT 'UretimHattiIslem' AS [Key], 1 AS [Value];
END
GO

CREATE OR ALTER PROCEDURE dbo.sp_HataTipi_List
AS
BEGIN
    SET NOCOUNT ON;
    SELECT HataTipiId, HataKodu, HataAdi, KritikSeviye, Aciklama
    FROM dbo.HataTipi;
END
GO

CREATE OR ALTER PROCEDURE dbo.sp_HataTipi_Add_Update_Remove
    @operation NVARCHAR(20),
    @jsonvalues NVARCHAR(MAX)
AS
BEGIN
    SET NOCOUNT ON;

    IF @operation = 'add'
    BEGIN
        INSERT INTO dbo.HataTipi (HataKodu, HataAdi, KritikSeviye, Aciklama)
        SELECT HataKodu, HataAdi, KritikSeviye, Aciklama
        FROM OPENJSON(@jsonvalues)
        WITH (
            HataKodu NVARCHAR(50),
            HataAdi NVARCHAR(150),
            KritikSeviye INT,
            Aciklama NVARCHAR(250)
        );
    END

    IF @operation = 'update'
    BEGIN
        UPDATE h
        SET h.HataKodu = j.HataKodu,
            h.HataAdi = j.HataAdi,
            h.KritikSeviye = j.KritikSeviye,
            h.Aciklama = j.Aciklama
        FROM dbo.HataTipi h
        CROSS APPLY OPENJSON(@jsonvalues)
        WITH (
            HataTipiId INT,
            HataKodu NVARCHAR(50),
            HataAdi NVARCHAR(150),
            KritikSeviye INT,
            Aciklama NVARCHAR(250)
        ) j
        WHERE h.HataTipiId = j.HataTipiId;
    END

    IF @operation = 'remove'
    BEGIN
        DELETE h
        FROM dbo.HataTipi h
        CROSS APPLY OPENJSON(@jsonvalues)
        WITH (HataTipiId INT) j
        WHERE h.HataTipiId = j.HataTipiId;
    END

    SELECT 'HataTipiIslem' AS [Key], 1 AS [Value];
END
GO

CREATE OR ALTER PROCEDURE dbo.sp_HataKaydi_List
AS
BEGIN
    SET NOCOUNT ON;
    SELECT HataKaydiId, UretimHattiId, HataTipiId, KullaniciId, Durum, BildirimTarihi, Aciklama
    FROM dbo.HataKaydi;
END
GO

CREATE OR ALTER PROCEDURE dbo.sp_HataKaydi_Add_Update_Remove
    @operation NVARCHAR(20),
    @jsonvalues NVARCHAR(MAX)
AS
BEGIN
    SET NOCOUNT ON;

    IF @operation = 'add'
    BEGIN
        INSERT INTO dbo.HataKaydi (UretimHattiId, HataTipiId, KullaniciId, Durum, BildirimTarihi, Aciklama)
        SELECT UretimHattiId, HataTipiId, KullaniciId, Durum, BildirimTarihi, Aciklama
        FROM OPENJSON(@jsonvalues)
        WITH (
            UretimHattiId INT,
            HataTipiId INT,
            KullaniciId INT,
            Durum NVARCHAR(30),
            BildirimTarihi DATETIME2,
            Aciklama NVARCHAR(500)
        );
    END

    IF @operation = 'update'
    BEGIN
        UPDATE h
        SET h.UretimHattiId = j.UretimHattiId,
            h.HataTipiId = j.HataTipiId,
            h.KullaniciId = j.KullaniciId,
            h.Durum = j.Durum,
            h.BildirimTarihi = j.BildirimTarihi,
            h.Aciklama = j.Aciklama
        FROM dbo.HataKaydi h
        CROSS APPLY OPENJSON(@jsonvalues)
        WITH (
            HataKaydiId INT,
            UretimHattiId INT,
            HataTipiId INT,
            KullaniciId INT,
            Durum NVARCHAR(30),
            BildirimTarihi DATETIME2,
            Aciklama NVARCHAR(500)
        ) j
        WHERE h.HataKaydiId = j.HataKaydiId;
    END

    IF @operation = 'remove'
    BEGIN
        DELETE h
        FROM dbo.HataKaydi h
        CROSS APPLY OPENJSON(@jsonvalues)
        WITH (HataKaydiId INT) j
        WHERE h.HataKaydiId = j.HataKaydiId;
    END

    SELECT 'HataKaydiIslem' AS [Key], 1 AS [Value];
END
GO

CREATE OR ALTER PROCEDURE dbo.sp_Logs_List
AS
BEGIN
    SET NOCOUNT ON;
    SELECT LogId, LogLevel, LogMessage, Kaynak, LogTarihi, KullaniciId, HataKaydiId
    FROM dbo.LogsTable;
END
GO
