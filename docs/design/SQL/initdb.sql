/* =========================================================
   ParkingDB_Simple_Guid - Minimal schema with GUID keys
   - Tất cả PK/FK dùng UNIQUEIDENTIFIER (GUID)
   - DEFAULT NEWSEQUENTIALID() cho PK
   ========================================================= */

-- (Tuỳ chọn) tạo DB mới nếu cần
CREATE DATABASE ParkingDB;
GO
USE ParkingDB;
GO
SET ANSI_NULLS ON;
SET QUOTED_IDENTIFIER ON;
GO

/* ========== 1) Danh mục nhỏ gọn (GUID) ========== */
CREATE TABLE dbo.VehicleType(
    vehicleTypeId  UNIQUEIDENTIFIER NOT NULL 
        CONSTRAINT DF_VehicleType_Id DEFAULT NEWSEQUENTIALID()
        CONSTRAINT PK_VehicleType PRIMARY KEY,
    typeName       NVARCHAR(50) NOT NULL
);

CREATE TABLE dbo.SendType(
    sendTypeId     UNIQUEIDENTIFIER NOT NULL 
        CONSTRAINT DF_SendType_Id DEFAULT NEWSEQUENTIALID()
        CONSTRAINT PK_SendType PRIMARY KEY,
    sendName       NVARCHAR(50) NOT NULL
);

CREATE TABLE dbo.PaymentMethod(
    methodId       UNIQUEIDENTIFIER NOT NULL 
        CONSTRAINT DF_PaymentMethod_Id DEFAULT NEWSEQUENTIALID()
        CONSTRAINT PK_PaymentMethod PRIMARY KEY,
    methodName     NVARCHAR(50) NOT NULL
);

/* ========== 2) Người dùng (vai trò đơn giản, GUID) ========== */
CREATE TABLE dbo.[User](
    userId       UNIQUEIDENTIFIER NOT NULL 
        CONSTRAINT DF_User_Id DEFAULT NEWSEQUENTIALID()
        CONSTRAINT PK_User PRIMARY KEY,
    username     NVARCHAR(100) NOT NULL CONSTRAINT UQ_User UNIQUE,
    passwordHash NVARCHAR(256) NOT NULL,
    fullName     NVARCHAR(200) NOT NULL,
    roleName     NVARCHAR(20)  NOT NULL DEFAULT N'STAFF'
        CONSTRAINT CK_User_Role CHECK (roleName IN (N'ADMIN',N'STAFF',N'CUSTOMER')),
    status       TINYINT NOT NULL DEFAULT 1                -- 1: Active, 0: Locked
);

/* ========== 3) Khách hàng & Phương tiện (1–N sở hữu, GUID) ========== */
CREATE TABLE dbo.Customer(
    customerId   UNIQUEIDENTIFIER NOT NULL 
        CONSTRAINT DF_Customer_Id DEFAULT NEWSEQUENTIALID()
        CONSTRAINT PK_Customer PRIMARY KEY,
    userId       UNIQUEIDENTIFIER NULL 
        CONSTRAINT FK_Customer_User REFERENCES dbo.[User](userId),
    fullName     NVARCHAR(200) NOT NULL,
    phone        NVARCHAR(30) NULL,
    email        NVARCHAR(200) NULL,
    status       TINYINT NOT NULL DEFAULT 1
);
CREATE INDEX IX_Customer_Status_Name ON dbo.Customer(status, fullName);

CREATE TABLE dbo.Vehicle(
    vehicleId     UNIQUEIDENTIFIER NOT NULL 
        CONSTRAINT DF_Vehicle_Id DEFAULT NEWSEQUENTIALID()
        CONSTRAINT PK_Vehicle PRIMARY KEY,
    vehicleTypeId UNIQUEIDENTIFIER NOT NULL 
        CONSTRAINT FK_Vehicle_VehicleType REFERENCES dbo.VehicleType(vehicleTypeId),
    licensePlate  NVARCHAR(20) NULL,  -- cho phép NULL
    rfidTag       NVARCHAR(50) NULL,
    brand         NVARCHAR(100) NULL,
    color         NVARCHAR(50) NULL,
    status        TINYINT NOT NULL DEFAULT 1
);
-- Unique có lọc: cho phép nhiều NULL, nhưng duy nhất khi có giá trị
CREATE UNIQUE INDEX UQ_Vehicle_LicensePlate_NotNull
    ON dbo.Vehicle(licensePlate) WHERE licensePlate IS NOT NULL;
CREATE UNIQUE INDEX UQ_Vehicle_Rfid_NotNull
    ON dbo.Vehicle(rfidTag) WHERE rfidTag IS NOT NULL;

-- Bảng quan hệ sở hữu: 1 Customer có nhiều Vehicle; 1 Vehicle chỉ thuộc 1 Customer
CREATE TABLE dbo.CustomerVehicle(
    customerId   UNIQUEIDENTIFIER NOT NULL 
        CONSTRAINT FK_CustVeh_Customer REFERENCES dbo.Customer(customerId),
    vehicleId    UNIQUEIDENTIFIER NOT NULL 
        CONSTRAINT FK_CustVeh_Vehicle  REFERENCES dbo.Vehicle(vehicleId),
    assignedAt   DATETIME2(0) NOT NULL DEFAULT SYSUTCDATETIME(),
    note         NVARCHAR(200) NULL,
    CONSTRAINT PK_CustomerVehicle PRIMARY KEY(customerId, vehicleId),
    CONSTRAINT UQ_CustomerVehicle_UniqueVehicle UNIQUE(vehicleId)
);
CREATE INDEX IX_CustomerVehicle_Customer ON dbo.CustomerVehicle(customerId);

/* ========== 4) Khu bãi & phân khu (GUID) ========== */
CREATE TABLE dbo.ParkingLot(
    lotId        UNIQUEIDENTIFIER NOT NULL 
        CONSTRAINT DF_Lot_Id DEFAULT NEWSEQUENTIALID()
        CONSTRAINT PK_ParkingLot PRIMARY KEY,
    lotName      NVARCHAR(200) NOT NULL,
    address      NVARCHAR(300) NULL
);

CREATE TABLE dbo.Zone(
    zoneId        UNIQUEIDENTIFIER NOT NULL 
        CONSTRAINT DF_Zone_Id DEFAULT NEWSEQUENTIALID()
        CONSTRAINT PK_Zone PRIMARY KEY,
    lotId         UNIQUEIDENTIFIER NOT NULL 
        CONSTRAINT FK_Zone_Lot REFERENCES dbo.ParkingLot(lotId),
    zoneName      NVARCHAR(200) NOT NULL,
    vehicleTypeId UNIQUEIDENTIFIER NOT NULL 
        CONSTRAINT FK_Zone_VehicleType REFERENCES dbo.VehicleType(vehicleTypeId),
    capacity      INT NOT NULL DEFAULT 0 CONSTRAINT CK_Zone_Cap CHECK (capacity >= 0)
);

/* ========== 5) Vé vãng lai & Gói định kỳ (GUID) ========== */
CREATE TABLE dbo.Ticket(
    ticketId       UNIQUEIDENTIFIER NOT NULL 
        CONSTRAINT DF_Ticket_Id DEFAULT NEWSEQUENTIALID()
        CONSTRAINT PK_Ticket PRIMARY KEY,
    ticketCode     NVARCHAR(50) NOT NULL CONSTRAINT UQ_Ticket UNIQUE,
    issueAt        DATETIME2(0) NOT NULL DEFAULT SYSUTCDATETIME(),
    issuedBy       UNIQUEIDENTIFIER NOT NULL 
        CONSTRAINT FK_Ticket_User REFERENCES dbo.[User](userId),
    vehicleTypeId  UNIQUEIDENTIFIER NOT NULL 
        CONSTRAINT FK_Ticket_VehicleType REFERENCES dbo.VehicleType(vehicleTypeId),
    licensePlateAtEntry NVARCHAR(20) NULL,
    zoneId         UNIQUEIDENTIFIER NULL 
        CONSTRAINT FK_Ticket_Zone REFERENCES dbo.Zone(zoneId),
    status         TINYINT NOT NULL DEFAULT 0 
        CONSTRAINT CK_Ticket_Status CHECK (status IN (0,1,2)) -- 0 Open,1 Closed,2 Lost
);

CREATE TABLE dbo.Subscription(
    subscriptionId UNIQUEIDENTIFIER NOT NULL 
        CONSTRAINT DF_Subscription_Id DEFAULT NEWSEQUENTIALID()
        CONSTRAINT PK_Subscription PRIMARY KEY,
    customerId     UNIQUEIDENTIFIER NOT NULL 
        CONSTRAINT FK_Sub_Customer REFERENCES dbo.Customer(customerId),
    vehicleId      UNIQUEIDENTIFIER NOT NULL 
        CONSTRAINT FK_Sub_Vehicle  REFERENCES dbo.Vehicle(vehicleId),
    packageType    NVARCHAR(10) NOT NULL 
        CONSTRAINT CK_Sub_PackageType CHECK (packageType IN (N'MONTHLY',N'YEARLY')),
    startDate      DATE NOT NULL,
    endDate        DATE NOT NULL,
    status         TINYINT NOT NULL DEFAULT 1 
        CONSTRAINT CK_Sub_Status CHECK (status IN (0,1,2,3)) -- 1 Active; 0 NotEff; 2 Expiring; 3 Expired
);

/* ========== 6) Phiên gửi (vào/ra) ========== */
CREATE TABLE dbo.ParkingSession(
    sessionId      UNIQUEIDENTIFIER NOT NULL 
        CONSTRAINT DF_Session_Id DEFAULT NEWSEQUENTIALID()
        CONSTRAINT PK_ParkingSession PRIMARY KEY,
    openAt         DATETIME2(0) NOT NULL DEFAULT SYSUTCDATETIME(),
    closeAt        DATETIME2(0) NULL,
    lotId          UNIQUEIDENTIFIER NOT NULL 
        CONSTRAINT FK_Session_Lot REFERENCES dbo.ParkingLot(lotId),
    zoneId         UNIQUEIDENTIFIER NOT NULL 
        CONSTRAINT FK_Session_Zone REFERENCES dbo.Zone(zoneId),
    vehicleTypeId  UNIQUEIDENTIFIER NOT NULL 
        CONSTRAINT FK_Session_VehicleType REFERENCES dbo.VehicleType(vehicleTypeId),
    ticketId       UNIQUEIDENTIFIER NULL 
        CONSTRAINT FK_Session_Ticket REFERENCES dbo.Ticket(ticketId),
    subscriptionId UNIQUEIDENTIFIER NULL 
        CONSTRAINT FK_Session_Sub REFERENCES dbo.Subscription(subscriptionId),
    vehicleId      UNIQUEIDENTIFIER NULL 
        CONSTRAINT FK_Session_Vehicle REFERENCES dbo.Vehicle(vehicleId),
    openedBy       UNIQUEIDENTIFIER NOT NULL 
        CONSTRAINT FK_Session_UserOpen REFERENCES dbo.[User](userId),
    closedBy       UNIQUEIDENTIFIER NULL 
        CONSTRAINT FK_Session_UserClose REFERENCES dbo.[User](userId),
    status         TINYINT NOT NULL DEFAULT 0 
        CONSTRAINT CK_Session_Status CHECK (status IN (0,1,2)), -- 0 Open,1 Closed,2 Cancelled
    note           NVARCHAR(200) NULL,
    CONSTRAINT CK_Session_TicketOrSub CHECK (
        (ticketId IS NOT NULL AND subscriptionId IS NULL)
        OR (ticketId IS NULL AND subscriptionId IS NOT NULL)
    )
);
/* (Tuỳ chọn khuyến nghị) Tránh 1 xe mở 2 phiên cùng lúc */
-- CREATE UNIQUE INDEX UQ_OpenSession_Vehicle
-- ON dbo.ParkingSession(vehicleId) WHERE vehicleId IS NOT NULL AND status = 0;

/* ========== 7) Hoá đơn & Thanh toán ========== */
CREATE TABLE dbo.Invoice(
    invoiceId      UNIQUEIDENTIFIER NOT NULL 
        CONSTRAINT DF_Invoice_Id DEFAULT NEWSEQUENTIALID()
        CONSTRAINT PK_Invoice PRIMARY KEY,
    invoiceNo      NVARCHAR(50) NOT NULL CONSTRAINT UQ_Invoice UNIQUE,
    customerId     UNIQUEIDENTIFIER NULL 
        CONSTRAINT FK_Invoice_Customer REFERENCES dbo.Customer(customerId),
    sessionId      UNIQUEIDENTIFIER NULL 
        CONSTRAINT FK_Invoice_Session REFERENCES dbo.ParkingSession(sessionId),
    subscriptionId UNIQUEIDENTIFIER NULL 
        CONSTRAINT FK_Invoice_Sub REFERENCES dbo.Subscription(subscriptionId),
    totalAmount    DECIMAL(18,2) NOT NULL DEFAULT 0,
    status         TINYINT NOT NULL DEFAULT 1 
        CONSTRAINT CK_Invoice_Status CHECK (status IN (0,1,2,3)), -- 1 Issued, 2 Paid, 3 Voided, 0 Draft
    issuedAt       DATETIME2(0) NOT NULL DEFAULT SYSUTCDATETIME(),
    issuedBy       UNIQUEIDENTIFIER NOT NULL 
        CONSTRAINT FK_Invoice_User REFERENCES dbo.[User](userId),
    note           NVARCHAR(200) NULL
);

CREATE TABLE dbo.InvoiceLine(
    lineId      UNIQUEIDENTIFIER NOT NULL 
        CONSTRAINT DF_InvoiceLine_Id DEFAULT NEWSEQUENTIALID()
        CONSTRAINT PK_InvoiceLine PRIMARY KEY,
    invoiceId   UNIQUEIDENTIFIER NOT NULL 
        CONSTRAINT FK_InvoiceLine_Invoice REFERENCES dbo.Invoice(invoiceId),
    description NVARCHAR(200) NOT NULL,
    qty         DECIMAL(10,2) NOT NULL DEFAULT 1,
    unitPrice   DECIMAL(18,2) NOT NULL DEFAULT 0,
    lineAmount  AS (qty * unitPrice) PERSISTED
);

CREATE TABLE dbo.Payment(
    paymentId   UNIQUEIDENTIFIER NOT NULL 
        CONSTRAINT DF_Payment_Id DEFAULT NEWSEQUENTIALID()
        CONSTRAINT PK_Payment PRIMARY KEY,
    invoiceId   UNIQUEIDENTIFIER NOT NULL 
        CONSTRAINT FK_Payment_Invoice REFERENCES dbo.Invoice(invoiceId),
    methodId    UNIQUEIDENTIFIER NOT NULL 
        CONSTRAINT FK_Payment_Method REFERENCES dbo.PaymentMethod(methodId),
    amount      DECIMAL(18,2) NOT NULL,
    paidAt      DATETIME2(0) NOT NULL DEFAULT SYSUTCDATETIME(),
    status      TINYINT NOT NULL DEFAULT 1 
        CONSTRAINT CK_Payment_Status CHECK (status IN (0,1,2,3)) -- 1 Completed, 0 Pending, 2 Failed, 3 Refunded
);

/* ========== 8) Index cơ bản ========== */
CREATE INDEX IX_Session_Status_OpenAt ON dbo.ParkingSession(status, openAt);
CREATE INDEX IX_Invoice_Status_IssuedAt ON dbo.Invoice(status, issuedAt);
CREATE INDEX IX_Payment_Invoice ON dbo.Payment(invoiceId);

/* ========== 9) Seed dữ liệu danh mục ========== */
INSERT dbo.VehicleType(typeName) VALUES (N'Xe đạp'),(N'Xe máy'),(N'Ô tô');
INSERT dbo.SendType(sendName)   VALUES (N'Vãng lai'),(N'Gói tháng'),(N'Gói năm');
INSERT dbo.PaymentMethod(methodName) VALUES (N'Tiền mặt'),(N'Chuyển khoản');