USE [master]
GO
/****** Object:  Database [Magazzino]    Script Date: 10/04/2021 00:42:51 ******/
CREATE DATABASE [Magazzino]
 CONTAINMENT = NONE
 ON  PRIMARY 
( NAME = N'Magazzino', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL15.MSSQLSERVER\MSSQL\DATA\Magazzino.mdf' , SIZE = 8192KB , MAXSIZE = UNLIMITED, FILEGROWTH = 65536KB )
 LOG ON 
( NAME = N'Magazzino_log', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL15.MSSQLSERVER\MSSQL\DATA\Magazzino_log.ldf' , SIZE = 8192KB , MAXSIZE = 2048GB , FILEGROWTH = 65536KB )
 WITH CATALOG_COLLATION = DATABASE_DEFAULT
GO
ALTER DATABASE [Magazzino] SET COMPATIBILITY_LEVEL = 150
GO
IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
begin
EXEC [Magazzino].[dbo].[sp_fulltext_database] @action = 'enable'
end
GO
ALTER DATABASE [Magazzino] SET ANSI_NULL_DEFAULT OFF 
GO
ALTER DATABASE [Magazzino] SET ANSI_NULLS OFF 
GO
ALTER DATABASE [Magazzino] SET ANSI_PADDING OFF 
GO
ALTER DATABASE [Magazzino] SET ANSI_WARNINGS OFF 
GO
ALTER DATABASE [Magazzino] SET ARITHABORT OFF 
GO
ALTER DATABASE [Magazzino] SET AUTO_CLOSE OFF 
GO
ALTER DATABASE [Magazzino] SET AUTO_SHRINK OFF 
GO
ALTER DATABASE [Magazzino] SET AUTO_UPDATE_STATISTICS ON 
GO
ALTER DATABASE [Magazzino] SET CURSOR_CLOSE_ON_COMMIT OFF 
GO
ALTER DATABASE [Magazzino] SET CURSOR_DEFAULT  GLOBAL 
GO
ALTER DATABASE [Magazzino] SET CONCAT_NULL_YIELDS_NULL OFF 
GO
ALTER DATABASE [Magazzino] SET NUMERIC_ROUNDABORT OFF 
GO
ALTER DATABASE [Magazzino] SET QUOTED_IDENTIFIER OFF 
GO
ALTER DATABASE [Magazzino] SET RECURSIVE_TRIGGERS OFF 
GO
ALTER DATABASE [Magazzino] SET  DISABLE_BROKER 
GO
ALTER DATABASE [Magazzino] SET AUTO_UPDATE_STATISTICS_ASYNC OFF 
GO
ALTER DATABASE [Magazzino] SET DATE_CORRELATION_OPTIMIZATION OFF 
GO
ALTER DATABASE [Magazzino] SET TRUSTWORTHY OFF 
GO
ALTER DATABASE [Magazzino] SET ALLOW_SNAPSHOT_ISOLATION OFF 
GO
ALTER DATABASE [Magazzino] SET PARAMETERIZATION SIMPLE 
GO
ALTER DATABASE [Magazzino] SET READ_COMMITTED_SNAPSHOT OFF 
GO
ALTER DATABASE [Magazzino] SET HONOR_BROKER_PRIORITY OFF 
GO
ALTER DATABASE [Magazzino] SET RECOVERY FULL 
GO
ALTER DATABASE [Magazzino] SET  MULTI_USER 
GO
ALTER DATABASE [Magazzino] SET PAGE_VERIFY CHECKSUM  
GO
ALTER DATABASE [Magazzino] SET DB_CHAINING OFF 
GO
ALTER DATABASE [Magazzino] SET FILESTREAM( NON_TRANSACTED_ACCESS = OFF ) 
GO
ALTER DATABASE [Magazzino] SET TARGET_RECOVERY_TIME = 60 SECONDS 
GO
ALTER DATABASE [Magazzino] SET DELAYED_DURABILITY = DISABLED 
GO
EXEC sys.sp_db_vardecimal_storage_format N'Magazzino', N'ON'
GO
ALTER DATABASE [Magazzino] SET QUERY_STORE = OFF
GO
USE [Magazzino]
GO
/****** Object:  Table [dbo].[Magazzino]    Script Date: 10/04/2021 00:42:51 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Magazzino](
	[IDProdotto] [char](13) NOT NULL,
	[Nome Prodotto] [char](30) NULL,
	[Quantità] [int] NULL,
	[Note] [char](30) NULL,
PRIMARY KEY CLUSTERED 
(
	[IDProdotto] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[OperazioniPossibili]    Script Date: 10/04/2021 00:42:51 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[OperazioniPossibili](
	[IDOperazione] [int] IDENTITY(1,1) NOT NULL,
	[Carico] [bit] NULL,
	[Descrizione] [varchar](64) NULL,
	[Note] [varchar](64) NULL,
PRIMARY KEY CLUSTERED 
(
	[IDOperazione] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[StoricoOperazioni]    Script Date: 10/04/2021 00:42:51 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[StoricoOperazioni](
	[IDEffettuazioniOperazioni] [int] IDENTITY(1,1) NOT NULL,
	[BarCode] [char](13) NULL,
	[CodOperazione] [int] NULL,
	[CodUtente] [char](13) NULL,
	[Quantità] [int] NULL,
PRIMARY KEY CLUSTERED 
(
	[IDEffettuazioniOperazioni] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Utenti]    Script Date: 10/04/2021 00:42:51 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Utenti](
	[Utente] [char](13) NOT NULL,
	[Nome Utente] [char](30) NULL,
	[Password] [char](64) NULL,
	[Ruolo] [int] NULL,
PRIMARY KEY CLUSTERED 
(
	[Utente] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
SET IDENTITY_INSERT [dbo].[OperazioniPossibili] ON 

INSERT [dbo].[OperazioniPossibili] ([IDOperazione], [Carico], [Descrizione], [Note]) VALUES (1, 1, N'Acquistato da fornitore', NULL)
INSERT [dbo].[OperazioniPossibili] ([IDOperazione], [Carico], [Descrizione], [Note]) VALUES (2, 1, N'Acquistato da privato', NULL)
INSERT [dbo].[OperazioniPossibili] ([IDOperazione], [Carico], [Descrizione], [Note]) VALUES (3, 1, N'Restituito per sbaglio acquisto', NULL)
INSERT [dbo].[OperazioniPossibili] ([IDOperazione], [Carico], [Descrizione], [Note]) VALUES (4, 1, N'Produzione interna', NULL)
INSERT [dbo].[OperazioniPossibili] ([IDOperazione], [Carico], [Descrizione], [Note]) VALUES (5, 1, N'Omaggio', NULL)
INSERT [dbo].[OperazioniPossibili] ([IDOperazione], [Carico], [Descrizione], [Note]) VALUES (6, 0, N'Venduto ad Azienda', NULL)
INSERT [dbo].[OperazioniPossibili] ([IDOperazione], [Carico], [Descrizione], [Note]) VALUES (7, 0, N'Venduto a privato', NULL)
INSERT [dbo].[OperazioniPossibili] ([IDOperazione], [Carico], [Descrizione], [Note]) VALUES (8, 0, N'Prodotto difettoso', NULL)
INSERT [dbo].[OperazioniPossibili] ([IDOperazione], [Carico], [Descrizione], [Note]) VALUES (9, 0, N'Rottura del prodotto', NULL)
INSERT [dbo].[OperazioniPossibili] ([IDOperazione], [Carico], [Descrizione], [Note]) VALUES (10, 0, N'Omaggio pubblicitario', NULL)
INSERT [dbo].[OperazioniPossibili] ([IDOperazione], [Carico], [Descrizione], [Note]) VALUES (11, 1, N'Altra motivazione', NULL)
INSERT [dbo].[OperazioniPossibili] ([IDOperazione], [Carico], [Descrizione], [Note]) VALUES (12, 0, N'Altra motivazione', NULL)
INSERT [dbo].[OperazioniPossibili] ([IDOperazione], [Carico], [Descrizione], [Note]) VALUES (13, NULL, N'Aggiungi utente', NULL)
INSERT [dbo].[OperazioniPossibili] ([IDOperazione], [Carico], [Descrizione], [Note]) VALUES (14, NULL, N'Aggiungi prodotto', NULL)
INSERT [dbo].[OperazioniPossibili] ([IDOperazione], [Carico], [Descrizione], [Note]) VALUES (15, NULL, N'Elimina utente', NULL)
INSERT [dbo].[OperazioniPossibili] ([IDOperazione], [Carico], [Descrizione], [Note]) VALUES (16, NULL, N'Elimina prodotto', NULL)
INSERT [dbo].[OperazioniPossibili] ([IDOperazione], [Carico], [Descrizione], [Note]) VALUES (17, NULL, N'Visualizza storico', NULL)
SET IDENTITY_INSERT [dbo].[OperazioniPossibili] OFF
GO
INSERT [dbo].[Utenti] ([Utente], [Nome Utente], [Password], [Ruolo]) VALUES (N'0123456789ABC', N'Mario                         ', N'57906b2660e4e2e227e96b86ec676dbac58f2fbc9673c2e03a63ceb29f94b220', 0)
INSERT [dbo].[Utenti] ([Utente], [Nome Utente], [Password], [Ruolo]) VALUES (N'PD32I5589OP1L', N'Mag. Sergio                   ', N'5c60fe0d492919d196e3a57352d609c34ba12ddc21f9a69ef442b210d6ed5153', 1)
GO
ALTER TABLE [dbo].[StoricoOperazioni]  WITH CHECK ADD FOREIGN KEY([BarCode])
REFERENCES [dbo].[Magazzino] ([IDProdotto])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[StoricoOperazioni]  WITH CHECK ADD FOREIGN KEY([CodOperazione])
REFERENCES [dbo].[OperazioniPossibili] ([IDOperazione])
GO
ALTER TABLE [dbo].[StoricoOperazioni]  WITH CHECK ADD FOREIGN KEY([CodUtente])
REFERENCES [dbo].[Utenti] ([Utente])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Magazzino]  WITH CHECK ADD CHECK  (([Quantità]>=(0)))
GO
ALTER TABLE [dbo].[StoricoOperazioni]  WITH CHECK ADD CHECK  (([Quantità]>=(0)))
GO
USE [master]
GO
ALTER DATABASE [Magazzino] SET  READ_WRITE 
GO
