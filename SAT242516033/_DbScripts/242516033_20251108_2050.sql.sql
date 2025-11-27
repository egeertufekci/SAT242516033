IF DB_ID(N'SAT242516033') IS NULL CREATE DATABASE SAT242516033;
GO
USE SAT242516033;
GO

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
@@ -91,111 +94,151 @@ SELECT
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

CREATE OR ALTER PROCEDURE dbo.sp_UrunEkle
    @UrunAdi NVARCHAR(200),
    @SKU NVARCHAR(64),
    @BirimFiyat DECIMAL(10,2),
    @StokAdet INT = 0,
    @AktifMi BIT = 1
AS
BEGIN
    SET NOCOUNT ON;
    INSERT INTO dbo.Urunler(UrunAdi,SKU,BirimFiyat,StokAdet,AktifMi)
    VALUES(@UrunAdi,@SKU,@BirimFiyat,ISNULL(@StokAdet,0),ISNULL(@AktifMi,1));
END;
GO

CREATE OR ALTER PROCEDURE dbo.sp_UrunGuncelle
    @UrunId INT,
    @UrunAdi NVARCHAR(200),
    @SKU NVARCHAR(64),
    @BirimFiyat DECIMAL(10,2),
    @StokAdet INT,
    @AktifMi BIT
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE dbo.Urunler
    SET UrunAdi = @UrunAdi,
        SKU = @SKU,
        BirimFiyat = @BirimFiyat,
        StokAdet = ISNULL(@StokAdet, 0),
        AktifMi = ISNULL(@AktifMi, 1)
    WHERE UrunId = @UrunId;
END;
GO

CREATE OR ALTER PROCEDURE dbo.sp_UrunSil
    @UrunId INT
AS
BEGIN
    SET NOCOUNT ON;
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
        RAISERROR (N'Detay listesi bo olamaz.',16,1);
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

INSERT INTO dbo.Musteriler(Ad,Soyad,Email) VALUES
(N'Ali',N'Ko',N'ali@example.com'),
(N'Veli',N'Bo',N'veli@example.com'),
(N'Melih',N'Co',N'melih@example.com');

INSERT INTO dbo.Kategoriler(KategoriAdi) VALUES
(N'Elektronik'),(N'Kitap'),(N'Giyim');

EXEC dbo.sp_UrunEkle N'Kulaklk',N'SKU-1001',299.90,50;
EXEC dbo.sp_UrunEkle N'Roman Kitab',N'SKU-2001',89.90,120;
EXEC dbo.sp_UrunEkle N'Tirt',N'SKU-3001',149.90,80;

INSERT INTO dbo.UrunKategorileri(UrunId,KategoriId) VALUES (1,1),(2,2),(3,3);

DECLARE @d1 dbo.tt_SiparisDetay;
INSERT INTO @d1(UrunId,Miktar,BirimFiyat) VALUES (1,1,299.90),(3,2,149.90);
DECLARE @Yeni1 TABLE(SiparisId INT);
INSERT INTO @Yeni1 EXEC dbo.sp_SiparisOlustur_TVP @MusteriId=1,@Durum=N'Beklemede',@Detaylar=@d1;

DECLARE @d2 dbo.tt_SiparisDetay;
INSERT INTO @d2(UrunId,Miktar,BirimFiyat) VALUES (2,3,89.90);
DECLARE @Yeni2 TABLE(SiparisId INT);
INSERT INTO @Yeni2 EXEC dbo.sp_SiparisOlustur_TVP @MusteriId=2,@Durum=N'Onayland',@Detaylar=@d2;

DECLARE @d3 dbo.tt_SiparisDetay;
INSERT INTO @d3(UrunId,Miktar,BirimFiyat) VALUES (3,1,149.90);
DECLARE @Yeni3 TABLE(SiparisId INT);
INSERT INTO @Yeni3 EXEC dbo.sp_SiparisOlustur_TVP @MusteriId=3,@Durum=N'Kargoda',@Detaylar=@d3;

SELECT * FROM dbo.vw_SiparisOzet ORDER BY SiparisId;
SELECT * FROM dbo.vw_SiparisDetay ORDER BY SiparisDetayId;