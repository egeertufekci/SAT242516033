USE [SAT242516075]
GO

CREATE TABLE [dbo].[Kullanicilar] (
    [KullaniciId] INT IDENTITY(1,1) NOT NULL,
    [KullaniciAdi] NVARCHAR(100) NOT NULL,
    [Sifre] NVARCHAR(200) NOT NULL,
    [Ad] NVARCHAR(100) NULL,
    [Soyad] NVARCHAR(100) NULL,
    [Rol] NVARCHAR(50) NULL,
    [AktifMi] BIT NOT NULL CONSTRAINT [DF_Kullanicilar_AktifMi] DEFAULT (1),
    CONSTRAINT [PK_Kullanicilar] PRIMARY KEY CLUSTERED ([KullaniciId] ASC)
);
GO

CREATE TABLE [dbo].[UretimHatlari] (
    [UretimHattiId] INT IDENTITY(1,1) NOT NULL,
    [HatAdi] NVARCHAR(150) NOT NULL,
    [Aciklama] NVARCHAR(500) NULL,
    [AktifMi] BIT NOT NULL CONSTRAINT [DF_UretimHatlari_AktifMi] DEFAULT (1),
    CONSTRAINT [PK_UretimHatlari] PRIMARY KEY CLUSTERED ([UretimHattiId] ASC)
);
GO

CREATE TABLE [dbo].[HataTipleri] (
    [HataTipiId] INT IDENTITY(1,1) NOT NULL,
    [HataAdi] NVARCHAR(150) NOT NULL,
    [Aciklama] NVARCHAR(500) NULL,
    [AktifMi] BIT NOT NULL CONSTRAINT [DF_HataTipleri_AktifMi] DEFAULT (1),
    CONSTRAINT [PK_HataTipleri] PRIMARY KEY CLUSTERED ([HataTipiId] ASC)
);
GO

CREATE TABLE [dbo].[HataKayitlari] (
    [HataKaydiId] INT IDENTITY(1,1) NOT NULL,
    [UretimHattiId] INT NOT NULL,
    [HataTipiId] INT NOT NULL,
    [KullaniciId] INT NULL,
    [Aciklama] NVARCHAR(1000) NULL,
    [KayitTarihi] DATETIME2(7) NOT NULL CONSTRAINT [DF_HataKayitlari_KayitTarihi] DEFAULT (SYSUTCDATETIME()),
    [Durum] NVARCHAR(50) NULL,
    CONSTRAINT [PK_HataKayitlari] PRIMARY KEY CLUSTERED ([HataKaydiId] ASC),
    CONSTRAINT [FK_HataKayitlari_UretimHatlari] FOREIGN KEY ([UretimHattiId]) REFERENCES [dbo].[UretimHatlari]([UretimHattiId]),
    CONSTRAINT [FK_HataKayitlari_HataTipleri] FOREIGN KEY ([HataTipiId]) REFERENCES [dbo].[HataTipleri]([HataTipiId]),
    CONSTRAINT [FK_HataKayitlari_Kullanicilar] FOREIGN KEY ([KullaniciId]) REFERENCES [dbo].[Kullanicilar]([KullaniciId])
);
GO

CREATE TABLE [dbo].[LogsTable] (
    [LogId] INT IDENTITY(1,1) NOT NULL,
    [LogLevel] NVARCHAR(50) NOT NULL,
    [Message] NVARCHAR(MAX) NOT NULL,
    [Exception] NVARCHAR(MAX) NULL,
    [LoggedAt] DATETIME2(7) NOT NULL CONSTRAINT [DF_LogsTable_LoggedAt] DEFAULT (SYSUTCDATETIME()),
    CONSTRAINT [PK_LogsTable] PRIMARY KEY CLUSTERED ([LogId] ASC)
);
GO
