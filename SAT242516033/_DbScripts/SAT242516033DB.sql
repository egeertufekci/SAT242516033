IF DB_ID(N'SAT242516033') IS NULL CREATE DATABASE SAT242516033;
GO
USE SAT242516033;
GO

IF OBJECT_ID('dbo.trg_SiparisDetaylari_AI','TR') IS NOT NULL DROP TRIGGER dbo.trg_SiparisDetaylari_AI;
IF OBJECT_ID('dbo.trg_SiparisDetaylari_AU','TR') IS NOT NULL DROP TRIGGER dbo.trg_SiparisDetaylari_AU;
IF OBJECT_ID('dbo.trg_SiparisDetaylari_AD','TR') IS NOT NULL DROP TRIGGER dbo.trg_SiparisDetaylari_AD;
IF OBJECT_ID('dbo.sp_SiparisOlustur_TVP','P') IS NOT NULL DROP PROCEDURE dbo.sp_SiparisOlustur_TVP;
IF OBJECT_ID('dbo.sp_SiparisEkle','P') IS NOT NULL DROP PROCEDURE dbo.sp_SiparisEkle;
IF OBJECT_ID('dbo.sp_UrunListele','P') IS NOT NULL DROP PROCEDURE dbo.sp_UrunListele;
IF OBJECT_ID('dbo.sp_UrunGuncelle','P') IS NOT NULL DROP PROCEDURE dbo.sp_UrunGuncelle;
IF OBJECT_ID('dbo.sp_UrunSil','P') IS NOT NULL DROP PROCEDURE dbo.sp_UrunSil;
IF OBJECT_ID('dbo.sp_UrunEkle','P') IS NOT NULL DROP PROCEDURE dbo.sp_UrunEkle;
GO
IF OBJECT_ID('dbo.vw_SiparisDetay','V') IS NOT NULL DROP VIEW dbo.vw_SiparisDetay;
IF OBJECT_ID('dbo.vw_SiparisOzet','V') IS NOT NULL DROP VIEW dbo.vw_SiparisOzet;
GO
IF OBJECT_ID('dbo.SiparisDetaylari','U') IS NOT NULL DROP TABLE dbo.SiparisDetaylari;
IF OBJECT_ID('dbo.Logs_Table','U') IS NOT NULL DROP TABLE dbo.Logs_Table;
IF OBJECT_ID('dbo.UrunKategorileri','U') IS NOT NULL DROP TABLE dbo.UrunKategorileri;
IF OBJECT_ID('dbo.Siparisler','U') IS NOT NULL DROP TABLE dbo.Siparisler;
IF OBJECT_ID('dbo.Urunler','U') IS NOT NULL DROP TABLE dbo.Urunler;
IF OBJECT_ID('dbo.Kategoriler','U') IS NOT NULL DROP TABLE dbo.Kategoriler;
IF OBJECT_ID('dbo.Musteriler','U') IS NOT NULL DROP TABLE dbo.Musteriler;
GO
IF TYPE_ID(N'dbo.tt_SiparisDetay') IS NOT NULL DROP TYPE dbo.tt_SiparisDetay;
IF TYPE_ID(N'dbo.DurumType') IS NOT NULL DROP TYPE dbo.DurumType;
GO

EXEC('CREATE TYPE dbo.DurumType FROM NVARCHAR(30) NOT NULL;');
EXEC('CREATE TYPE dbo.tt_SiparisDetay AS TABLE
(
    UrunId INT NOT NULL,
    Miktar INT NOT NULL CHECK (Miktar > 0),
    BirimFiyat DECIMAL(10,2) NOT NULL CHECK (BirimFiyat >= 0),
    UNIQUE(UrunId)
);');
GO

CREATE TABLE dbo.Musteriler
(
    MusteriId INT IDENTITY(1,1) CONSTRAINT PK_Musteriler PRIMARY KEY,
    Ad NVARCHAR(100) NOT NULL,
    Soyad NVARCHAR(100) NOT NULL,
    Email NVARCHAR(255) NOT NULL
);

CREATE TABLE dbo.Kategoriler
(
    KategoriId INT IDENTITY(1,1) CONSTRAINT PK_Kategoriler PRIMARY KEY,
    KategoriAdi NVARCHAR(150) NOT NULL
);

CREATE TABLE dbo.Urunler
(
    UrunId INT IDENTITY(1,1) CONSTRAINT PK_Urunler PRIMARY KEY,
    UrunAdi NVARCHAR(200) NOT NULL,
    SKU NVARCHAR(64) NOT NULL,
    BirimFiyat DECIMAL(10,2) NOT NULL,
    StokAdet INT NOT NULL CONSTRAINT DF_Urunler_StokAdet DEFAULT(0),
    AktifMi BIT NOT NULL CONSTRAINT DF_Urunler_AktifMi DEFAULT(1)
);

CREATE TABLE dbo.Siparisler
(
    SiparisId INT IDENTITY(1,1) CONSTRAINT PK_Siparisler PRIMARY KEY,
    MusteriId INT NOT NULL CONSTRAINT FK_Siparisler_Musteriler REFERENCES dbo.Musteriler(MusteriId) ON DELETE CASCADE,
    SiparisTarihi DATETIME2 NOT NULL CONSTRAINT DF_Siparisler_SiparisTarihi DEFAULT(SYSDATETIME()),
    Durum dbo.DurumType NOT NULL CONSTRAINT DF_Siparisler_Durum DEFAULT(N'Beklemede')
);

CREATE TABLE dbo.UrunKategorileri
(
    UrunId INT NOT NULL,
    KategoriId INT NOT NULL,
    CONSTRAINT PK_UrunKategorileri PRIMARY KEY (UrunId, KategoriId),
    CONSTRAINT FK_UrunKategorileri_Urunler FOREIGN KEY (UrunId) REFERENCES dbo.Urunler(UrunId) ON DELETE CASCADE,
    CONSTRAINT FK_UrunKategorileri_Kategoriler FOREIGN KEY (KategoriId) REFERENCES dbo.Kategoriler(KategoriId) ON DELETE CASCADE
);

CREATE TABLE dbo.SiparisDetaylari
(
    SiparisDetayId INT IDENTITY(1,1) CONSTRAINT PK_SiparisDetaylari PRIMARY KEY,
    SiparisId INT NOT NULL CONSTRAINT FK_SiparisDetaylari_Siparisler REFERENCES dbo.Siparisler(SiparisId) ON DELETE CASCADE,
    UrunId INT NOT NULL CONSTRAINT FK_SiparisDetaylari_Urunler REFERENCES dbo.Urunler(UrunId) ON DELETE CASCADE,
    Miktar INT NOT NULL CHECK (Miktar > 0),
    BirimFiyat DECIMAL(10,2) NOT NULL CHECK (BirimFiyat >= 0)
);

CREATE TABLE dbo.Logs_Table
(
    LogId INT IDENTITY(1,1) CONSTRAINT PK_Logs_Table PRIMARY KEY,
    LogLevel NVARCHAR(50) NOT NULL,
    Message NVARCHAR(500) NOT NULL,
    CreatedAt DATETIME2 NOT NULL CONSTRAINT DF_Logs_Table_CreatedAt DEFAULT(SYSDATETIME())
);
GO

CREATE VIEW dbo.vw_SiparisOzet AS
SELECT
    s.SiparisId,
    s.SiparisTarihi,
    m.Ad + N' ' + m.Soyad AS Musteri,
    s.Durum,
    SUM(sd.Miktar * sd.BirimFiyat) AS ToplamTutar
FROM dbo.Siparisler s
JOIN dbo.Musteriler m ON m.MusteriId = s.MusteriId
LEFT JOIN dbo.SiparisDetaylari sd ON sd.SiparisId = s.SiparisId
GROUP BY s.SiparisId, s.SiparisTarihi, m.Ad, m.Soyad, s.Durum;
GO

CREATE VIEW dbo.vw_SiparisDetay AS
SELECT
  sd.SiparisDetayId,
  sd.SiparisId,
  s.SiparisTarihi,
  m.Ad + N' ' + m.Soyad AS Musteri,
  sd.UrunId,
  u.UrunAdi,
  sd.Miktar,
  sd.BirimFiyat,
  sd.Miktar * sd.BirimFiyat AS SatirTutar
FROM dbo.SiparisDetaylari sd
JOIN dbo.Siparisler  s ON s.SiparisId = sd.SiparisId
JOIN dbo.Musteriler m ON m.MusteriId = s.MusteriId
JOIN dbo.Urunler   u ON u.UrunId    = sd.UrunId;
GO

CREATE OR ALTER PROCEDURE dbo.sp_UrunListele
AS
BEGIN
    SET NOCOUNT ON;
    SELECT UrunId, UrunAdi, SKU, BirimFiyat, StokAdet, AktifMi
    FROM dbo.Urunler
    ORDER BY UrunId;
END;
GO

@@ -111,77 +170,160 @@ BEGIN
    DELETE FROM dbo.Urunler WHERE UrunId = @UrunId;
END;
GO

CREATE OR ALTER PROCEDURE dbo.sp_SiparisEkle
    @MusteriId INT,
    @Durum dbo.DurumType = N'Beklemede'
AS
BEGIN
    SET NOCOUNT ON;
    INSERT INTO dbo.Siparisler(MusteriId,Durum) VALUES(@MusteriId,@Durum);
    DECLARE @YeniId INT = SCOPE_IDENTITY();
    SELECT @YeniId AS SiparisId;
END;
GO

CREATE OR ALTER PROCEDURE dbo.sp_SiparisOlustur_TVP
    @MusteriId INT,
    @Durum dbo.DurumType = N'Beklemede',
    @Detaylar dbo.tt_SiparisDetay READONLY
AS
BEGIN
    SET NOCOUNT ON;
    IF NOT EXISTS (SELECT 1 FROM @Detaylar)
    BEGIN
        RAISERROR (N'Detay listesi boþ olamaz.',16,1);
        RETURN;
    END
    BEGIN TRAN;
    BEGIN TRY
        INSERT INTO dbo.Siparisler(MusteriId,Durum) VALUES(@MusteriId,@Durum);
        DECLARE @SiparisId INT = SCOPE_IDENTITY();
        INSERT INTO dbo.SiparisDetaylari(SiparisId,UrunId,Miktar,BirimFiyat)
        SELECT @SiparisId,UrunId,Miktar,BirimFiyat FROM @Detaylar;
        COMMIT TRAN;
        SELECT @SiparisId AS SiparisId;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0 ROLLBACK TRAN;
        DECLARE @ErrMsg NVARCHAR(4000)=ERROR_MESSAGE(), @ErrSev INT=ERROR_SEVERITY(), @ErrSta INT=ERROR_STATE();
        RAISERROR(@ErrMsg,@ErrSev,@ErrSta);
        RETURN;
    END CATCH
END;
GO

CREATE OR ALTER TRIGGER dbo.trg_SiparisDetaylari_AI
ON dbo.SiparisDetaylari
AFTER INSERT
AS
BEGIN
    SET NOCOUNT ON;
    ;WITH cte AS (
        SELECT UrunId, SUM(Miktar) AS ToplamMiktar
        FROM inserted
        GROUP BY UrunId
    )
    IF EXISTS (
        SELECT 1
        FROM cte c
        JOIN dbo.Urunler u ON u.UrunId = c.UrunId
        WHERE u.StokAdet < c.ToplamMiktar
    )
    BEGIN
        RAISERROR(N'Yeterli stok bulunmuyor.',16,1);
        ROLLBACK TRANSACTION;
        RETURN;
    END

    UPDATE u
    SET StokAdet = u.StokAdet - c.ToplamMiktar
    FROM dbo.Urunler u
    JOIN cte c ON c.UrunId = u.UrunId;
END;
GO

CREATE OR ALTER TRIGGER dbo.trg_SiparisDetaylari_AU
ON dbo.SiparisDetaylari
AFTER UPDATE
AS
BEGIN
    SET NOCOUNT ON;
    ;WITH delta AS (
        SELECT COALESCE(i.UrunId, d.UrunId) AS UrunId,
               ISNULL(i.Miktar,0) - ISNULL(d.Miktar,0) AS MiktarDegisim
        FROM inserted i
        FULL JOIN deleted d ON i.SiparisDetayId = d.SiparisDetayId
    ), agg AS (
        SELECT UrunId, SUM(MiktarDegisim) AS ToplamDegisim
        FROM delta
        GROUP BY UrunId
    )
    IF EXISTS (
        SELECT 1
        FROM agg a
        JOIN dbo.Urunler u ON u.UrunId = a.UrunId
        WHERE a.ToplamDegisim > 0 AND u.StokAdet < a.ToplamDegisim
    )
    BEGIN
        RAISERROR(N'Yeterli stok bulunmuyor.',16,1);
        ROLLBACK TRANSACTION;
        RETURN;
    END

    UPDATE u
    SET StokAdet = u.StokAdet - a.ToplamDegisim
    FROM dbo.Urunler u
    JOIN agg a ON a.UrunId = u.UrunId;
END;
GO

CREATE OR ALTER TRIGGER dbo.trg_SiparisDetaylari_AD
ON dbo.SiparisDetaylari
AFTER DELETE
AS
BEGIN
    SET NOCOUNT ON;
    ;WITH cte AS (
        SELECT UrunId, SUM(Miktar) AS ToplamMiktar
        FROM deleted
        GROUP BY UrunId
    )
    UPDATE u
    SET StokAdet = u.StokAdet + c.ToplamMiktar
    FROM dbo.Urunler u
    JOIN cte c ON c.UrunId = u.UrunId;
END;
GO

INSERT INTO dbo.Musteriler(Ad,Soyad,Email) VALUES
(N'Ali',N'Koç',N'ali@example.com'),
(N'Veli',N'Boz',N'veli@example.com'),
(N'Melih',N'Coþkun',N'melih@example.com');

INSERT INTO dbo.Kategoriler(KategoriAdi) VALUES
(N'Elektronik'),(N'Kitap'),(N'Giyim');

EXEC dbo.sp_UrunEkle N'Kulaklýk',N'SKU-1001',299.90,50;
EXEC dbo.sp_UrunEkle N'Roman Kitabý',N'SKU-2001',89.90,120;
EXEC dbo.sp_UrunEkle N'Tiþört',N'SKU-3001',149.90,80;

INSERT INTO dbo.UrunKategorileri(UrunId,KategoriId) VALUES (1,1),(2,2),(3,3);

DECLARE @d1 dbo.tt_SiparisDetay;
INSERT INTO @d1(UrunId,Miktar,BirimFiyat) VALUES (1,1,299.90),(3,2,149.90);
DECLARE @Yeni1 TABLE(SiparisId INT);
INSERT INTO @Yeni1 EXEC dbo.sp_SiparisOlustur_TVP @MusteriId=1,@Durum=N'Beklemede',@Detaylar=@d1;

DECLARE @d2 dbo.tt_SiparisDetay;
INSERT INTO @d2(UrunId,Miktar,BirimFiyat) VALUES (2,3,89.90);
DECLARE @Yeni2 TABLE(SiparisId INT);
INSERT INTO @Yeni2 EXEC dbo.sp_SiparisOlustur_TVP @MusteriId=2,@Durum=N'Onaylandý',@Detaylar=@d2;

DECLARE @d3 dbo.tt_SiparisDetay;
INSERT INTO @d3(UrunId,Miktar,BirimFiyat) VALUES (3,1,149.90);
DECLARE @Yeni3 TABLE(SiparisId INT);
INSERT INTO @Yeni3 EXEC dbo.sp_SiparisOlustur_TVP @MusteriId=3,@Durum=N'Kargoda',@Detaylar=@d3;

SELECT * FROM dbo.vw_SiparisOzet ORDER BY SiparisId;
SELECT * FROM dbo.vw_SiparisDetay ORDER BY SiparisDetayId;