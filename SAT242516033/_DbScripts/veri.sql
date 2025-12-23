USE [SAT242516046]
GO

-- =============================================
-- 1. ROLLERİ OLUŞTUR
-- =============================================
INSERT INTO Rol (RolAdi) VALUES ('Admin');
INSERT INTO Rol (RolAdi) VALUES ('Yonetici');
INSERT INTO Rol (RolAdi) VALUES ('Personel');
GO

-- =============================================
-- 2. ADMIN KULLANICISI (Kullanıcı Adı: taha, Şifre: 123)
-- =============================================
DECLARE @AdminRolID INT = (SELECT RolID FROM Rol WHERE RolAdi = 'Admin');
INSERT INTO Kullanici (KullaniciAdi, SifreHash, RolID)
VALUES ('taha', '123', @AdminRolID);
GO

-- =============================================
-- 3. PERSONELLERİ EKLE (20 ADET)
-- =============================================
INSERT INTO Personel (Ad, Soyad, Departman, BrutMaas, Email, Telefon) VALUES 
('Ahmet', 'Yılmaz', 'Yazılım', 55000, 'ahmet.yilmaz@sirket.com', '555-101-0001'),
('Ayşe', 'Kaya', 'İnsan Kaynakları', 42000, 'ayse.kaya@sirket.com', '555-101-0002'),
('Mehmet', 'Demir', 'Muhasebe', 40000, 'mehmet.demir@sirket.com', '555-101-0003'),
('Fatma', 'Çelik', 'Pazarlama', 38000, 'fatma.celik@sirket.com', '555-101-0004'),
('Mustafa', 'Şahin', 'IT Destek', 35000, 'mustafa.sahin@sirket.com', '555-101-0005'),
('Zeynep', 'Arslan', 'Yazılım', 60000, 'zeynep.arslan@sirket.com', '555-101-0006'),
('Emre', 'Yıldız', 'Satış', 45000, 'emre.yildiz@sirket.com', '555-101-0007'),
('Selin', 'Öztürk', 'Yönetim', 95000, 'selin.ozturk@sirket.com', '555-101-0008'),
('Burak', 'Aydın', 'Yazılım', 48000, 'burak.aydin@sirket.com', '555-101-0009'),
('Esra', 'Tekin', 'İnsan Kaynakları', 36000, 'esra.tekin@sirket.com', '555-101-0010'),
('Caner', 'Erkin', 'Muhasebe', 39000, 'caner.erkin@sirket.com', '555-101-0011'),
('Gamze', 'Polat', 'Pazarlama', 41000, 'gamze.polat@sirket.com', '555-101-0012'),
('Hakan', 'Çalhanoğlu', 'Yazılım', 75000, 'hakan.calhanoglu@sirket.com', '555-101-0013'),
('Merve', 'Boluğur', 'Satış', 43000, 'merve.bolugur@sirket.com', '555-101-0014'),
('Oğuzhan', 'Koç', 'IT Destek', 34000, 'oguzhan.koc@sirket.com', '555-101-0015'),
('Pelin', 'Karahan', 'Yönetim', 80000, 'pelin.karahan@sirket.com', '555-101-0016'),
('Serkan', 'Çayoğlu', 'Yazılım', 52000, 'serkan.cayoglu@sirket.com', '555-101-0017'),
('Hande', 'Erçel', 'Pazarlama', 40000, 'hande.ercel@sirket.com', '555-101-0018'),
('Kerem', 'Bürsin', 'Satış', 46000, 'kerem.bursin@sirket.com', '555-101-0019'),
('Barış', 'Arduç', 'Yazılım', 58000, 'baris.arduc@sirket.com', '555-101-0020');
GO

-- =============================================
-- 4. BAZI PERSONELLERE MAAŞ GİRİŞİ YAP (Dolu görünsün)
-- =============================================
-- Ahmet (Yazılım) - Primli
INSERT INTO MaasHesaplama (PersonelID, Donem, Prim, Kesinti, NetMaas)
VALUES ((SELECT PersonelID FROM Personel WHERE Ad='Ahmet'), '2025-01', 5000, 0, 60000);

-- Zeynep (Yazılım) - Standart
INSERT INTO MaasHesaplama (PersonelID, Donem, Prim, Kesinti, NetMaas)
VALUES ((SELECT PersonelID FROM Personel WHERE Ad='Zeynep'), '2025-01', 0, 0, 60000);

-- Selin (Yönetim) - Yüksek Prim
INSERT INTO MaasHesaplama (PersonelID, Donem, Prim, Kesinti, NetMaas)
VALUES ((SELECT PersonelID FROM Personel WHERE Ad='Selin'), '2025-01', 10000, 0, 105000);

-- Mustafa (IT) - Geç Kaldığı İçin Kesintili
INSERT INTO MaasHesaplama (PersonelID, Donem, Prim, Kesinti, NetMaas)
VALUES ((SELECT PersonelID FROM Personel WHERE Ad='Mustafa'), '2025-01', 0, 2000, 33000);

-- Hakan (Yazılım)
INSERT INTO MaasHesaplama (PersonelID, Donem, Prim, Kesinti, NetMaas)
VALUES ((SELECT PersonelID FROM Personel WHERE Ad='Hakan'), '2025-01', 2500, 0, 77500);
GO

-- =============================================
-- 5. BAZI PERSONELLERE İZİN EKLE
-- =============================================
-- Ayşe (İK) Haftaya İzinli
INSERT INTO Izin (PersonelID, BaslangicTarihi, BitisTarihi, IzinTuru)
VALUES ((SELECT PersonelID FROM Personel WHERE Ad='Ayşe'), DATEADD(day, 7, GETDATE()), DATEADD(day, 14, GETDATE()), 'Yıllık İzin');

-- Mehmet (Muhasebe) Hasta
INSERT INTO Izin (PersonelID, BaslangicTarihi, BitisTarihi, IzinTuru)
VALUES ((SELECT PersonelID FROM Personel WHERE Ad='Mehmet'), GETDATE(), DATEADD(day, 2, GETDATE()), 'Raporlu');

-- Esra (İK) Ücretsiz İzin
INSERT INTO Izin (PersonelID, BaslangicTarihi, BitisTarihi, IzinTuru)
VALUES ((SELECT PersonelID FROM Personel WHERE Ad='Esra'), '2025-02-01', '2025-02-15', 'Ücretsiz İzin');

-- Kerem (Satış) Mazeret
INSERT INTO Izin (PersonelID, BaslangicTarihi, BitisTarihi, IzinTuru)
VALUES ((SELECT PersonelID FROM Personel WHERE Ad='Kerem'), DATEADD(day, 1, GETDATE()), DATEADD(day, 1, GETDATE()), 'Mazeret İzni');
GO