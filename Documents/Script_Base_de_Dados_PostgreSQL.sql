CREATE EXTENSION IF NOT EXISTS "pgcrypto";

CREATE TABLE IF NOT EXISTS Categories (
    Id UUID NOT NULL DEFAULT gen_random_uuid(),
    Name VARCHAR(100) NOT NULL,
    CreatedAt TIMESTAMP NOT NULL DEFAULT NOW(),
    PRIMARY KEY (Id)
);

CREATE TABLE IF NOT EXISTS Users (
    Id UUID NOT NULL DEFAULT gen_random_uuid(),
    Name VARCHAR(150) NOT NULL,
    Email VARCHAR(200) NOT NULL,
    PasswordHash VARCHAR(500) NOT NULL,
    IsActive BOOLEAN NOT NULL DEFAULT TRUE,
    CreatedAt TIMESTAMP NOT NULL DEFAULT NOW(),
    PRIMARY KEY (Id),
    UNIQUE (Email)
);

CREATE TABLE IF NOT EXISTS Stores (
    Id UUID NOT NULL DEFAULT gen_random_uuid(),
    Name VARCHAR(200) NOT NULL,
    TradeName VARCHAR(200),
    DocumentNumber VARCHAR(20),
    StateRegistration VARCHAR(30),
    City VARCHAR(100),
    State VARCHAR(2),
    CreatedAt TIMESTAMP NOT NULL DEFAULT NOW(),
    PRIMARY KEY (Id)
);

CREATE TABLE IF NOT EXISTS Products (
    Id UUID NOT NULL DEFAULT gen_random_uuid(),
    NormalizedName VARCHAR(300) NOT NULL,
    Brand VARCHAR(150),
    CategoryId UUID,
    CreatedAt TIMESTAMP NOT NULL DEFAULT NOW(),
    PRIMARY KEY (Id),
    FOREIGN KEY (CategoryId) REFERENCES Categories(Id)
);

CREATE TABLE IF NOT EXISTS Purchases (
    Id UUID NOT NULL DEFAULT gen_random_uuid(),
    UserId UUID NOT NULL,
    StoreId UUID NOT NULL,
    AccessKey VARCHAR(60),
    PurchaseDate TIMESTAMP,
    TotalValue NUMERIC(18,2) NOT NULL,
    XmlFileName VARCHAR(300),
    XmlContent TEXT,
    CreatedAt TIMESTAMP NOT NULL DEFAULT NOW(),
    PRIMARY KEY (Id),
    FOREIGN KEY (UserId) REFERENCES Users(Id),
    FOREIGN KEY (StoreId) REFERENCES Stores(Id)
);

CREATE TABLE IF NOT EXISTS PurchaseItems (
    Id UUID NOT NULL DEFAULT gen_random_uuid(),
    PurchaseId UUID NOT NULL,
    ProductId UUID,
    OriginalDescription VARCHAR(500) NOT NULL,
    ProductCode VARCHAR(100),
    NcmCode VARCHAR(20),
    Ean VARCHAR(50),
    Quantity NUMERIC(18,3) NOT NULL,
    Unit VARCHAR(20),
    UnitPrice NUMERIC(18,4) NOT NULL,
    TotalPrice NUMERIC(18,2) NOT NULL,
    CreatedAt TIMESTAMP NOT NULL DEFAULT NOW(),
    PRIMARY KEY (Id),
    FOREIGN KEY (PurchaseId) REFERENCES Purchases(Id),
    FOREIGN KEY (ProductId) REFERENCES Products(Id)
);

CREATE TABLE IF NOT EXISTS ImportLogs (
    Id UUID NOT NULL DEFAULT gen_random_uuid(),
    UserId UUID NOT NULL,
    FileName VARCHAR(300) NOT NULL,
    Status VARCHAR(50) NOT NULL,
    Message TEXT,
    ImportedAt TIMESTAMP NOT NULL DEFAULT NOW(),
    PRIMARY KEY (Id),
    FOREIGN KEY (UserId) REFERENCES Users(Id)
);

CREATE TABLE IF NOT EXISTS ProductNormalizationMappings (
    Id UUID NOT NULL DEFAULT gen_random_uuid(),
    OriginalText VARCHAR(500) NOT NULL,
    ReplacementText VARCHAR(500) NOT NULL,
    MatchType VARCHAR(20) NOT NULL DEFAULT 'Exact',
    IsActive BOOLEAN NOT NULL DEFAULT TRUE,
    CreatedAt TIMESTAMP NOT NULL DEFAULT NOW(),
    UpdatedAt TIMESTAMP,
    PRIMARY KEY (Id)
);

CREATE TABLE IF NOT EXISTS ProductKeywordSubstitutions (
    Id UUID NOT NULL DEFAULT gen_random_uuid(),
    OriginalTerm VARCHAR(200) NOT NULL,
    ReplacementTerm VARCHAR(200) NOT NULL,
    MatchType VARCHAR(20) NOT NULL DEFAULT 'WholeWord',
    IsActive BOOLEAN NOT NULL DEFAULT TRUE,
    CreatedAt TIMESTAMP NOT NULL DEFAULT NOW(),
    UpdatedAt TIMESTAMP,
    PRIMARY KEY (Id)
);

CREATE INDEX IF NOT EXISTS idx_purchaseitems_productid ON PurchaseItems(ProductId);
CREATE INDEX IF NOT EXISTS idx_purchaseitems_originaldescription ON PurchaseItems(OriginalDescription);
CREATE INDEX IF NOT EXISTS idx_products_normalizedname ON Products(NormalizedName);
CREATE INDEX IF NOT EXISTS idx_purchases_storeid ON Purchases(StoreId);
