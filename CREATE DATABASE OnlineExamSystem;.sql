CREATE DATABASE OnlineExamSystem;
GO

USE OnlineExamSystem;
GO

-- Bảng User (thay tblNguoiDung)
CREATE TABLE tblUser (
    UserId INT IDENTITY(1,1) PRIMARY KEY,       -- Primary key
    UserCode NVARCHAR(50) NOT NULL UNIQUE,      -- Random unique code (like FB ID)
    FullName NVARCHAR(255) NOT NULL,
    BirthDate DATE,
    Gender NVARCHAR(10) NOT NULL,
    Username NVARCHAR(100) NOT NULL UNIQUE,     -- Unique username
    PasswordHash NVARCHAR(255) NOT NULL,
    PhoneNumber CHAR(10),
    Email NVARCHAR(255) UNIQUE, -- Unique email
    Avatar NVARCHAR(255),
    Balance DECIMAL(18,2) DEFAULT 0 CHECK (Balance >= 0),
    LastLogin DATETIME,                               -- Last login time
    IsEmailVerified BIT DEFAULT 0,                    -- Email verified?
    IsPhoneVerified BIT DEFAULT 0,                    -- Phone verified?                   
    Role TINYINT NOT NULL CHECK (Role IN (1,2)),-- 1: Admin, 2: User
    CreatedAt DATETIME DEFAULT GETDATE(),
    Status BIT DEFAULT 1,                       -- 1: Active, 0: Inactive
    CreatedBy NVARCHAR(100) NOT NULL,
    UpdatedAt DATETIME,
    UpdatedBy NVARCHAR(100)
);
GO

INSERT INTO tblUser 
(UserCode, FullName, BirthDate, Gender, Username, PasswordHash, PhoneNumber, Email, Avatar, Balance, LastLogin, IsEmailVerified, IsPhoneVerified, Role, CreatedBy, CreatedAt, Status)
VALUES
-- 1. Admin
('USR0001', N'Nguyễn Bá Thái', '2002-03-15', N'Nam', 'admin_thai', 'Admin1234', '0912345678', 'thai.admin@example.com', '/images/avatars/admin1.png', 0, GETDATE(), 1, 1, 1, N'System', GETDATE(), 1),

-- 2. User
('USR0002', N'Trần Văn Hùng', '2001-08-12', N'Nam', 'hung_tv', 'Hung1234', '0987654321', 'hungtv@example.com', '/images/avatars/user1.png', 50000, NULL, 1, 1, 2, N'admin_thai', GETDATE(), 1),

-- 3. User
('USR0003', N'Lê Thị Mai', '2003-01-22', N'Nữ', 'lemai_03', 'Mai2024A', '0905123456', 'lemai@example.com', '/images/avatars/user2.png', 100000, NULL, 0, 1, 2, N'admin_thai', GETDATE(), 1),

-- 4. User
('USR0004', N'Phạm Anh Tuấn', '2000-10-05', N'Nam', 'tuan_anh', 'TuanAnh99', '0977123456', 'tuanpa@example.com', '/images/avatars/user3.png', 20000, NULL, 1, 0, 2, N'admin_thai', GETDATE(), 1),

-- 5. User
('USR0005', N'Ngô Minh Thư', '2002-05-30', N'Nữ', 'minh_thu', 'Thu2002A', '0938123456', 'minhthu@example.com', '/images/avatars/user4.png', 75000, NULL, 1, 1, 2, N'admin_thai', GETDATE(), 1),

-- 6. User
('USR0006', N'Lưu Đức Long', '2001-11-18', N'Nam', 'luu_long', 'Long1234A', '0918456123', 'luudlong@example.com', '/images/avatars/user5.png', 0, NULL, 0, 0, 2, N'admin_thai', GETDATE(), 1),

-- 7. User
('USR0007', N'Phan Hồng Hạnh', '2004-09-09', N'Nữ', 'hong_hanh', 'HanhA2024', '0967345123', 'honghanh@example.com', '/images/avatars/user6.png', 120000, NULL, 1, 1, 2, N'admin_thai', GETDATE(), 1),

-- 8. User
('USR0008', N'Đặng Văn Lợi', '2002-07-02', N'Nam', 'dang_loi', 'Loi2024aA', '0945123456', 'dangloi@example.com', '/images/avatars/user7.png', 30000, NULL, 1, 0, 2, N'admin_thai', GETDATE(), 1),

-- 9. User
('USR0009', N'Hoàng Thị Tuyết', '2003-12-10', N'Nữ', 'hoang_tuyet', 'Tuyet2024', '0983123456', 'hoangtuyet@example.com', '/images/avatars/user8.png', 60000, NULL, 1, 1, 2, N'admin_thai', GETDATE(), 1),

-- 10. User
('USR0010', N'Bùi Quốc Khánh', '2001-04-28', N'Nam', 'bui_khanh', 'KhanhA12', '0921345678', 'buikhanh@example.com', '/images/avatars/user9.png', 25000, NULL, 0, 0, 2, N'admin_thai', GETDATE(), 1);


CREATE TABLE tblAdminMenu (
    AdminMenuID BIGINT IDENTITY(1,1) PRIMARY KEY,
    ItemName NVARCHAR(50) NOT NULL,
    ItemLevel INT NULL,
    ParentLevel INT NULL,
    ItemOrder INT NULL,
    ItemTarget NVARCHAR(50) NULL,
    IsActive BIT NOT NULL DEFAULT 1,
    AreaName NVARCHAR(20) NULL,
    ControllerName NVARCHAR(20) NULL,
    ActionName NVARCHAR(20) NULL,
    Icon NVARCHAR(50) NULL,
    IdName NVARCHAR(50) NULL
);

GO

INSERT INTO tblAdminMenu (ItemName, ItemLevel, ParentLevel, ItemOrder, ItemTarget, IsActive, AreaName, ControllerName, ActionName, Icon, IdName)
VALUES 
(N'Subject management', 1, 0, 2, NULL, 1, N'Admin', N'Subject', N'Index', NULL, NULL),
(N'Exam management', 1, 0, 3, NULL, 1, N'Admin', N'Exam', N'Index', NULL, NULL),
(N'Question & Answer management', 1, 0, 4, NULL, 1, N'Admin', N'QuestionAndAnswer', N'Index', NULL, NULL),
(N'User management', 1, 0, 1, NULL, 1, N'Admin', N'User', N'Index', NULL, NULL);

GO

CREATE TABLE tblSubject (
    SubjectId NVARCHAR(10) PRIMARY KEY,
    SubjectName NVARCHAR(255) NOT NULL,
    Description NVARCHAR(MAX) NULL,
    Image NVARCHAR(500) NULL,
    Status BIT NOT NULL DEFAULT 1,
    CreatedAt DATETIME DEFAULT GETDATE(),
    CreatedBy NVARCHAR(255) NOT NULL,
    UpdatedAt DATETIME NULL,
    UpdatedBy NVARCHAR(255) NULL
);

GO

INSERT INTO tblSubject (SubjectId, SubjectName, Description, Image, Status, CreatedAt, CreatedBy)
VALUES 
('IT101', N'Lập trình C cơ bản', N'Môn học giới thiệu lập trình C, cấu trúc điều khiển, hàm, mảng.', '/uploads/subject/c.png', 1, GETDATE(), N'admin'),

('IT102', N'Lập trình Java', N'Môn học lập trình hướng đối tượng với Java, class, object, kế thừa, interface.', '/uploads/subject/java.png', 1, GETDATE(), N'admin'),

('IT103', N'Cơ sở dữ liệu', N'Môn học về SQL, thiết kế cơ sở dữ liệu quan hệ, truy vấn nâng cao.', '/uploads/subject/sql.png', 1, GETDATE(), N'admin'),

('IT104', N'Mạng máy tính', N'Kiến thức cơ bản về mạng máy tính, TCP/IP, LAN, WAN.', '/uploads/subject/network.png', 1, GETDATE(), N'admin'),

('IT105', N'Trí tuệ nhân tạo', N'Giới thiệu về AI, Machine Learning, thuật toán tìm kiếm và học máy.', '/uploads/subject/ai.png', 1, GETDATE(), N'admin'),

('IT106', N'Lập trình .Net', N'Môn học giới thiệu lập trình .Net, mô hình MVC.', '/uploads/subject/asp.png', 1, GETDATE(), N'admin');

GO

CREATE TABLE tblExam
(
    ExamId INT IDENTITY(1,1) PRIMARY KEY,          -- Mã đề thi, tự tăng
    ExamName NVARCHAR(255) NOT NULL,               -- Tên đề thi
    Description NVARCHAR(MAX) NULL,                -- Mô tả đề thi, optional
    QuestionCount INT NOT NULL,                    -- Số câu hỏi, bắt buộc
    DurationMinutes INT NOT NULL,                  -- Thời gian làm bài (phút), bắt buộc
    ExamType NVARCHAR(100) NOT NULL,              -- Loại đề thi: Thi thử, Chính thức…
    ExamFee DECIMAL(10,2) NOT NULL DEFAULT 0.00,  -- Phí thi, mặc định 0
    Image NVARCHAR(500) NULL,                      -- Ảnh minh họa, optional
    SubjectId NVARCHAR(10) NOT NULL,              -- Khóa ngoại tới môn học
    Status BIT NOT NULL DEFAULT 1,                 -- Trạng thái: 1 = hoạt động, 0 = khóa
    CreatedAt DATETIME NOT NULL DEFAULT GETDATE(), -- Ngày tạo, mặc định hiện tại
    CreatedBy NVARCHAR(255) NOT NULL,             -- Người tạo, bắt buộc
    UpdatedAt DATETIME NULL,                       -- Ngày cập nhật
    UpdatedBy NVARCHAR(255) NULL,                 -- Người cập nhật
    CONSTRAINT FK_tblExam_Subject FOREIGN KEY (SubjectId)
        REFERENCES dbo.tblSubject(SubjectId)
        ON DELETE CASCADE
        ON UPDATE CASCADE
);

GO

INSERT INTO tblExam 
    (ExamName, Description, QuestionCount, DurationMinutes, ExamType, ExamFee, Image, SubjectId, Status, CreatedAt, CreatedBy)
VALUES
-- Lập trình C cơ bản
(N'Thử thách lập trình C cơ bản', N'Đề thi tổng hợp các kiến thức cơ bản về lập trình C.', 20, 60, N'Thử nghiệm', 0.00, '/uploads/exam/c_basic_1.png', 'IT101', 1, GETDATE(), N'admin'),
(N'Kiểm tra giữa kỳ C', N'Đề thi giữa kỳ môn Lập trình C.', 30, 90, N'Chính thức', 50.00, '/uploads/exam/c_midterm.png', 'IT101', 1, GETDATE(), N'admin'),

-- Lập trình Java
(N'Thử thách Java OOP', N'Đề thi tổng hợp kiến thức lập trình hướng đối tượng Java.', 25, 75, N'Thử nghiệm', 0.00, '/uploads/exam/java_oop_1.png', 'IT102', 1, GETDATE(), N'admin'),
(N'Kiểm tra cuối kỳ Java', N'Đề thi chính thức cuối kỳ môn Java.', 40, 120, N'Chính thức', 100.00, '/uploads/exam/java_final.png', 'IT102', 1, GETDATE(), N'admin'),

-- Cơ sở dữ liệu
(N'Thử thách SQL cơ bản', N'Đề thi kiểm tra kiến thức SQL cơ bản và truy vấn.', 20, 60, N'Thử nghiệm', 0.00, '/uploads/exam/sql_basic.png', 'IT103', 1, GETDATE(), N'admin'),
(N'Kiểm tra CSDL nâng cao', N'Đề thi chính thức về cơ sở dữ liệu nâng cao.', 35, 90, N'Chính thức', 75.00, '/uploads/exam/sql_advanced.png', 'IT103', 1, GETDATE(), N'admin'),

-- Mạng máy tính
(N'Thử thách Mạng LAN/WAN', N'Đề thi kiến thức cơ bản về mạng máy tính.', 15, 45, N'Thử nghiệm', 0.00, '/uploads/exam/network_basic.png', 'IT104', 1, GETDATE(), N'admin'),
(N'Kiểm tra mạng máy tính', N'Đề thi chính thức về lý thuyết mạng máy tính.', 25, 60, N'Chính thức', 50.00, '/uploads/exam/network_final.png', 'IT104', 1, GETDATE(), N'admin'),

-- Trí tuệ nhân tạo
(N'Thử thách AI cơ bản', N'Đề thi kiểm tra kiến thức cơ bản về AI và Machine Learning.', 20, 60, N'Thử nghiệm', 0.00, '/uploads/exam/ai_basic.png', 'IT105', 1, GETDATE(), N'admin'),
(N'Kiểm tra AI nâng cao', N'Đề thi chính thức về AI, học máy và thuật toán tìm kiếm.', 30, 90, N'Chính thức', 100.00, '/uploads/exam/ai_advanced.png', 'IT105', 1, GETDATE(), N'admin'),

-- Lập trình .Net
(N'Thử thách .Net MVC', N'Đề thi tổng hợp kiến thức lập trình .Net MVC.', 25, 75, N'Thử nghiệm', 0.00, '/uploads/exam/dotnet_basic.png', 'IT106', 1, GETDATE(), N'admin'),
(N'Kiểm tra .Net nâng cao', N'Đề thi chính thức môn .Net, MVC, và C# nâng cao.', 35, 90, N'Chính thức', 100.00, '/uploads/exam/dotnet_advanced.png', 'IT106', 1, GETDATE(), N'admin');

GO

CREATE TABLE tblQuestion
(
    QuestionId INT IDENTITY(1,1) PRIMARY KEY,     -- Mã câu hỏi, tự tăng
    SubjectId NVARCHAR(10) NOT NULL,              -- Thuộc môn học nào
    QuestionText NVARCHAR(MAX) NOT NULL,          -- Nội dung câu hỏi
    Level TINYINT NOT NULL DEFAULT 1,             -- Độ khó: 1=Dễ, 2=Trung bình, 3=Khó
    Status BIT NOT NULL DEFAULT 1,                -- Trạng thái: 1=Hoạt động, 0=Ẩn
    CreatedAt DATETIME NOT NULL DEFAULT GETDATE(),-- Ngày tạo
    CreatedBy NVARCHAR(255) NOT NULL,             -- Người tạo
    UpdatedAt DATETIME NULL,                      -- Ngày cập nhật
    UpdatedBy NVARCHAR(255) NULL,                 -- Người cập nhật
    CONSTRAINT FK_tblQuestion_Subject FOREIGN KEY (SubjectId)
        REFERENCES dbo.tblSubject(SubjectId)
        ON DELETE CASCADE
        ON UPDATE CASCADE
);

GO

CREATE TABLE tblAnswer
(
    AnswerId INT IDENTITY(1,1) PRIMARY KEY,      -- Mã đáp án, tự tăng
    QuestionId INT NOT NULL,                      -- Thuộc câu hỏi nào
    AnswerText NVARCHAR(MAX) NOT NULL,           -- Nội dung đáp án
    IsCorrect BIT NOT NULL DEFAULT 0,            -- Đáp án đúng hay sai
    Status BIT NOT NULL DEFAULT 1,               -- Trạng thái: 1=Hoạt động, 0=Ẩn
    CreatedAt DATETIME NOT NULL DEFAULT GETDATE(),-- Ngày tạo
    CreatedBy NVARCHAR(255) NOT NULL,            -- Người tạo
    UpdatedAt DATETIME NULL,                      -- Ngày cập nhật
    UpdatedBy NVARCHAR(255) NULL,                -- Người cập nhật
    CONSTRAINT FK_tblAnswer_Question FOREIGN KEY (QuestionId)
        REFERENCES dbo.tblQuestion(QuestionId)
        ON DELETE CASCADE
        ON UPDATE CASCADE
);

GO

CREATE TABLE tblQuestionInExam
(
    QuestionInExamId INT IDENTITY(1,1) PRIMARY KEY,             
    ExamId INT NOT NULL,                           
    QuestionId INT NOT NULL,                                
    CreatedAt DATETIME NOT NULL DEFAULT GETDATE(), 
    CreatedBy NVARCHAR(255) NOT NULL,             
    UpdatedAt DATETIME NULL,                       
    UpdatedBy NVARCHAR(255) NULL,                 
    CONSTRAINT FK_QuestionInExam_Exam FOREIGN KEY (ExamId)
        REFERENCES dbo.tblExam(ExamId)
        ON DELETE CASCADE,
    CONSTRAINT FK_QuestionInExam_Question FOREIGN KEY (QuestionId)
        REFERENCES dbo.tblQuestion(QuestionId)
        ON DELETE NO ACTION,
    CONSTRAINT UQ_QuestionInExam UNIQUE (ExamId, QuestionId)
);

GO

CREATE TABLE tblPayment (
    PaymentId BIGINT IDENTITY(1,1) PRIMARY KEY,
    UserId INT NOT NULL,
    Amount DECIMAL(18,2) NOT NULL,
    PaymentMethod NVARCHAR(50) NOT NULL,      -- 'VNPay', 'Momo', etc.
    PaymentStatus NVARCHAR(50) NOT NULL,             -- 'Pending', 'Success', 'Failed'
    TransactionCode NVARCHAR(100),            -- Code returned from gateway
    PaymentDate DATETIME DEFAULT GETDATE(),
    Note NVARCHAR(255) NULL,
    IsProcessed BIT NOT NULL DEFAULT(0),
    CreatedBy NVARCHAR(255),
    CreatedDate DATETIME DEFAULT GETDATE(),
    UpdatedBy NVARCHAR(255),
    UpdatedDate DATETIME NULL,

    CONSTRAINT FK_Payment_User FOREIGN KEY (UserId)
    REFERENCES tblUser(UserId)
);

GO

CREATE TABLE tblExamAttempt
(
    AttemptId        INT IDENTITY(1,1) PRIMARY KEY,         -- Mã lượt thi (tự tăng)
    ExamId           INT NOT NULL,                          -- FK -> tblExam
    UserId           INT NULL,                              -- FK -> tblUser (nếu có)

    Score            DECIMAL(5,2) NULL,                     -- Điểm (nếu có)
    CorrectCount     INT NULL,                              -- Số câu đúng
    WrongCount       INT NULL,                              -- Số câu sai
    UnansweredCount  INT NULL,                              -- Số câu bỏ trống
    PercentScore     DECIMAL(5,2) NULL,                     -- % điểm đạt được (đã tính sẵn)

    StartedAt        DATETIME2(7) NOT NULL,                 -- UTC: thời điểm bắt đầu làm
    FinishedAt       DATETIME2(7) NULL,                     -- UTC: thời điểm kết thúc (nếu nộp)
    DurationSeconds  INT NULL,                              -- Thời gian làm thực tế (giây)
    TimeLimitSeconds INT NULL,                              -- Giới hạn thời gian làm bài (giây)
    IsCompleted      BIT NOT NULL DEFAULT 0,                -- 1 = hoàn thành, 0 = bỏ dở

    CreatedAt        DATETIME2(7) NOT NULL DEFAULT SYSUTCDATETIME(), -- Thời điểm tạo bản ghi (UTC)
    CreatedBy        NVARCHAR(255) NULL,                    -- Người tạo (tên user hoặc hệ thống)
    UpdatedAt        DATETIME2(7) NULL,                     -- Thời điểm cập nhật
    UpdatedBy        NVARCHAR(255) NULL,                    -- Người cập nhật

    CONSTRAINT FK_tblExamAttempt_Exam FOREIGN KEY (ExamId)
        REFERENCES dbo.tblExam(ExamId)
        ON DELETE CASCADE,
    CONSTRAINT FK_tblExamAttempt_User FOREIGN KEY (UserId)
        REFERENCES dbo.tblUser(UserId)
        ON DELETE SET NULL
);

GO

CREATE INDEX IX_tblExamAttempt_ExamId_CreatedAt ON dbo.tblExamAttempt(ExamId, CreatedAt);

GO

CREATE INDEX IX_tblExamAttempt_UserId_CreatedAt ON dbo.tblExamAttempt(UserId, CreatedAt);

GO

CREATE INDEX IX_tblExamAttempt_IsCompleted ON dbo.tblExamAttempt(IsCompleted);

GO

INSERT INTO dbo.tblExamAttempt
(ExamId, UserId, Score, CorrectCount, WrongCount, UnansweredCount, PercentScore,
 StartedAt, FinishedAt, DurationSeconds, TimeLimitSeconds, IsCompleted,
 CreatedAt, CreatedBy, UpdatedAt, UpdatedBy)
VALUES
(2, 1, 85.00, 26, 3, 1, 86.67,
 '2025-11-01 02:15:00.0000000', '2025-11-01 03:45:00.0000000', 5400, 5400, 1,
 '2025-11-01 03:46:00.0000000', 'NguyenBaThai', '2025-11-01 03:46:00.0000000', 'NguyenBaThai'),

(4, 4, 72.50, 29, 9, 2, 72.50,
 '2025-10-20 08:00:00.0000000', '2025-10-20 10:02:00.0000000', 7320, 7200, 1,
 '2025-10-20 10:05:00.0000000', 'admin_thai', '2025-10-20 10:05:00.0000000', 'admin_thai'),

(5, 5, 95.00, 19, 0, 1, 95.00,
 '2025-11-03 13:10:00.0000000', '2025-11-03 13:50:00.0000000', 2400, 3600, 1,
 '2025-11-03 13:51:00.0000000', 'admin_thai', '2025-11-03 13:51:00.0000000', 'admin_thai'),

(6, 6, NULL, NULL, NULL, NULL, NULL,
 '2025-11-04 09:00:00.0000000', NULL, NULL, 5400, 0,
 '2025-11-04 09:00:00.0000000', 'admin_thai', NULL, NULL),

(7, 7, 88.00, 13, 1, 1, 86.67,
 '2025-10-30 15:20:00.0000000', '2025-10-30 15:50:00.0000000', 1800, 2700, 1,
 '2025-10-30 15:52:00.0000000', 'admin_thai', '2025-10-30 15:52:00.0000000', 'admin_thai'),

(8, 8, 70.00, 18, 7, 0, 72.00,
 '2025-11-02 10:00:00.0000000', '2025-11-02 11:02:00.0000000', 3720, 3600, 1,
 '2025-11-02 11:03:00.0000000', 'admin_thai', '2025-11-02 11:03:00.0000000', 'admin_thai'),

(9, 9, 60.00, 12, 5, 3, 60.00,
 '2025-10-28 14:30:00.0000000', '2025-10-28 15:10:00.0000000', 2400, 3600, 1,
 '2025-10-28 15:11:00.0000000', 'admin_thai', '2025-10-28 15:11:00.0000000', 'admin_thai'),

(10, 10, 92.00, 28, 2, 0, 93.33,
 '2025-11-05 09:00:00.0000000', '2025-11-05 10:32:00.0000000', 5520, 5400, 1,
 '2025-11-05 10:33:00.0000000', 'admin_thai', '2025-11-05 10:33:00.0000000', 'admin_thai'),

(1, 2, 100.00, 2, 0, 0, 100.00,
 '2025-11-06 20:00:00.0000000', '2025-11-06 20:20:00.0000000', 1200, 3600, 1,
 '2025-11-06 20:21:00.0000000', 'NguyenBaThai', '2025-11-06 20:21:00.0000000', 'NguyenBaThai'),

(2, 1, NULL, NULL, NULL, NULL, NULL,
 '2025-11-06 07:30:00.0000000', NULL, NULL, 5400, 0,
 '2025-11-06 07:30:00.0000000', 'admin_thai', NULL, NULL),

(5, 2, 78.00, 15, 3, 2, 78.00,
 '2025-10-25 16:00:00.0000000', '2025-10-25 16:50:00.0000000', 3000, 3600, 1,
 '2025-10-25 16:52:00.0000000', 'user', '2025-10-25 16:52:00.0000000', 'user'),

(6, 8, 82.50, 29, 6, 0, 82.86,
 '2025-10-22 09:30:00.0000000', '2025-10-22 10:58:00.0000000', 4680, 5400, 1,
 '2025-10-22 10:59:00.0000000', 'admin_thai', '2025-10-22 10:59:00.0000000', 'admin_thai');

-- 5 đề thi được thì nhiều nhất trong tuần
/*
SELECT TOP 5 
    e.ExamId,
    e.ExamName,
    COUNT(a.AttemptId) AS AttemptCount
FROM dbo.tblExamAttempt AS a
JOIN dbo.tblExam AS e ON a.ExamId = e.ExamId
WHERE a.StartedAt >= DATEADD(DAY, 1 - DATEPART(WEEKDAY, SYSUTCDATETIME()), CAST(SYSUTCDATETIME() AS DATE))
      AND a.IsCompleted = 1
GROUP BY e.ExamId, e.ExamName
ORDER BY AttemptCount DESC;
*/

-- 5 dề thì được thi nhiều nhất
/*
SELECT TOP 5 
    e.ExamId,
    e.ExamName,
    COUNT(a.AttemptId) AS AttemptCount
FROM dbo.tblExamAttempt AS a
JOIN dbo.tblExam AS e ON a.ExamId = e.ExamId
WHERE a.IsCompleted = 1                -- chỉ tính các bài đã hoàn thành
GROUP BY e.ExamId, e.ExamName
ORDER BY AttemptCount DESC;            -- sắp xếp giảm dần theo số lượt thi
*/
GO

INSERT INTO tblQuestion (SubjectId, QuestionText, Level, Status, CreatedAt, CreatedBy)
VALUES
('IT101', N'Ngôn ngữ lập trình C được phát triển bởi ai?', 1, 1, GETDATE(), N'admin'),
('IT101', N'Tập tin có phần mở rộng nào thường dùng cho mã nguồn C?', 1, 1, GETDATE(), N'admin'),
('IT101', N'Hàm main() trong chương trình C có vai trò gì?', 1, 1, GETDATE(), N'admin'),
('IT101', N'Câu lệnh nào dùng để in ra màn hình trong C?', 1, 1, GETDATE(), N'admin'),
('IT101', N'Trong C, cú pháp để khai báo biến nguyên là gì?', 1, 1, GETDATE(), N'admin'),
('IT101', N'Hàm nào dùng để nhập dữ liệu từ bàn phím trong C?', 1, 1, GETDATE(), N'admin'),
('IT101', N'Trong C, ký tự nào dùng để kết thúc một câu lệnh?', 1, 1, GETDATE(), N'admin'),
('IT101', N'Trong C, kiểu dữ liệu “float” dùng để lưu trữ gì?', 1, 1, GETDATE(), N'admin'),
('IT101', N'Trong C, phép toán “%” dùng để làm gì?', 1, 1, GETDATE(), N'admin'),
('IT101', N'Trong C, cấu trúc điều kiện nào được sử dụng để kiểm tra điều kiện?', 1, 1, GETDATE(), N'admin'),
('IT101', N'Trong C, vòng lặp nào được sử dụng khi biết trước số lần lặp?', 1, 1, GETDATE(), N'admin'),
('IT101', N'Trong C, từ khóa nào dùng để thoát khỏi vòng lặp ngay lập tức?', 1, 1, GETDATE(), N'admin'),
('IT101', N'Hàm nào trong C được dùng để lấy giá trị tuyệt đối của một số nguyên?', 1, 1, GETDATE(), N'admin'),
('IT101', N'Trong C, cú pháp khai báo mảng 1 chiều đúng là gì?', 2, 1, GETDATE(), N'admin'),
('IT101', N'Hàm nào được sử dụng để tính độ dài chuỗi ký tự trong C?', 2, 1, GETDATE(), N'admin'),
('IT101', N'Trong C, ký tự kết thúc chuỗi là gì?', 2, 1, GETDATE(), N'admin'),
('IT101', N'Trong C, con trỏ là gì?', 2, 1, GETDATE(), N'admin'),
('IT101', N'Trong C, toán tử “&” dùng để làm gì?', 2, 1, GETDATE(), N'admin'),
('IT101', N'Hàm nào được dùng để cấp phát bộ nhớ động trong C?', 3, 1, GETDATE(), N'admin');

GO

-- Câu 1: Ngôn ngữ lập trình C được phát triển bởi ai?
INSERT INTO tblAnswer (QuestionId, AnswerText, IsCorrect, CreatedBy) VALUES
(1, N'Dennis Ritchie', 1, N'admin'),
(1, N'Bjarne Stroustrup', 0, N'admin'),
(1, N'James Gosling', 0, N'admin'),
(1, N'Bill Gates', 0, N'admin');
GO
-- Câu 2: Tập tin có phần mở rộng nào thường dùng cho mã nguồn C?
INSERT INTO tblAnswer (QuestionId, AnswerText, IsCorrect, CreatedBy) VALUES
(2, N'.c', 1, N'admin'),
(2, N'.cpp', 0, N'admin'),
(2, N'.java', 0, N'admin'),
(2, N'.py', 0, N'admin');
GO
-- Câu 3: Hàm main() trong chương trình C có vai trò gì?
INSERT INTO tblAnswer (QuestionId, AnswerText, IsCorrect, CreatedBy) VALUES
(3, N'Điểm bắt đầu của chương trình', 1, N'admin'),
(3, N'Kết thúc chương trình', 0, N'admin'),
(3, N'In ra màn hình', 0, N'admin'),
(3, N'Nhập dữ liệu từ bàn phím', 0, N'admin');
GO
-- Câu 4: Câu lệnh nào dùng để in ra màn hình trong C?
INSERT INTO tblAnswer (QuestionId, AnswerText, IsCorrect, CreatedBy) VALUES
(4, N'printf()', 1, N'admin'),
(4, N'cout', 0, N'admin'),
(4, N'System.out.println()', 0, N'admin'),
(4, N'print()', 0, N'admin');
GO
-- Câu 5: Trong C, cú pháp để khai báo biến nguyên là gì?
INSERT INTO tblAnswer (QuestionId, AnswerText, IsCorrect, CreatedBy) VALUES
(5, N'int ten_bien;', 1, N'admin'),
(5, N'integer ten_bien;', 0, N'admin'),
(5, N'var ten_bien;', 0, N'admin'),
(5, N'string ten_bien;', 0, N'admin');
GO
-- Câu 6: Hàm nào dùng để nhập dữ liệu từ bàn phím trong C?
INSERT INTO tblAnswer (QuestionId, AnswerText, IsCorrect, CreatedBy) VALUES
(6, N'scanf()', 1, N'admin'),
(6, N'cin', 0, N'admin'),
(6, N'input()', 0, N'admin'),
(6, N'readline()', 0, N'admin');
GO
-- Câu 7: Trong C, ký tự nào dùng để kết thúc một câu lệnh?
INSERT INTO tblAnswer (QuestionId, AnswerText, IsCorrect, CreatedBy) VALUES
(7, N';', 1, N'admin'),
(7, N'.', 0, N'admin'),
(7, N':', 0, N'admin'),
(7, N',', 0, N'admin');
GO
-- Câu 8: Trong C, kiểu dữ liệu "float" dùng để lưu trữ gì?
INSERT INTO tblAnswer (QuestionId, AnswerText, IsCorrect, CreatedBy) VALUES
(8, N'Số thực', 1, N'admin'),
(8, N'Số nguyên', 0, N'admin'),
(8, N'Chuỗi ký tự', 0, N'admin'),
(8, N'Ký tự', 0, N'admin');
GO
-- Câu 9: Trong C, phép toán "%" dùng để làm gì?
INSERT INTO tblAnswer (QuestionId, AnswerText, IsCorrect, CreatedBy) VALUES
(9, N'Chia lấy phần dư', 1, N'admin'),
(9, N'Chia lấy phần nguyên', 0, N'admin'),
(9, N'Phép nhân', 0, N'admin'),
(9, N'Phép lũy thừa', 0, N'admin');
GO
-- Câu 10: Trong C, cấu trúc điều kiện nào được sử dụng để kiểm tra điều kiện?
INSERT INTO tblAnswer (QuestionId, AnswerText, IsCorrect, CreatedBy) VALUES
(10, N'if-else', 1, N'admin'),
(10, N'for', 0, N'admin'),
(10, N'while', 0, N'admin'),
(10, N'switch', 0, N'admin');
GO
-- Câu 11: Trong C, vòng lặp nào được sử dụng khi biết trước số lần lặp?
INSERT INTO tblAnswer (QuestionId, AnswerText, IsCorrect, CreatedBy) VALUES
(11, N'for', 1, N'admin'),
(11, N'while', 0, N'admin'),
(11, N'do-while', 0, N'admin'),
(11, N'if', 0, N'admin');
GO
-- Câu 12: Trong C, từ khóa nào dùng để thoát khỏi vòng lặp ngay lập tức?
INSERT INTO tblAnswer (QuestionId, AnswerText, IsCorrect, CreatedBy) VALUES
(12, N'break', 1, N'admin'),
(12, N'continue', 0, N'admin'),
(12, N'return', 0, N'admin'),
(12, N'exit', 0, N'admin');
GO
-- Câu 13: Hàm nào trong C được dùng để lấy giá trị tuyệt đối của một số nguyên?
INSERT INTO tblAnswer (QuestionId, AnswerText, IsCorrect, CreatedBy) VALUES
(13, N'abs()', 1, N'admin'),
(13, N'fabs()', 0, N'admin'),
(13, N'absolute()', 0, N'admin'),
(13, N'mod()', 0, N'admin');
GO
-- Câu 14: Trong C, cú pháp khai báo mảng 1 chiều đúng là gì?
INSERT INTO tblAnswer (QuestionId, AnswerText, IsCorrect, CreatedBy) VALUES
(14, N'int arr[10];', 1, N'admin'),
(14, N'array arr[10];', 0, N'admin'),
(14, N'int arr = [10];', 0, N'admin'),
(14, N'arr[10] int;', 0, N'admin');
GO
-- Câu 15: Hàm nào được sử dụng để tính độ dài chuỗi ký tự trong C?
INSERT INTO tblAnswer (QuestionId, AnswerText, IsCorrect, CreatedBy) VALUES
(15, N'strlen()', 1, N'admin'),
(15, N'sizeof()', 0, N'admin'),
(15, N'length()', 0, N'admin'),
(15, N'strlength()', 0, N'admin');
GO
-- Câu 16: Trong C, ký tự kết thúc chuỗi là gì?
INSERT INTO tblAnswer (QuestionId, AnswerText, IsCorrect, CreatedBy) VALUES
(16, N'\0', 1, N'admin'),
(16, N'\n', 0, N'admin'),
(16, N'\t', 0, N'admin'),
(16, N'NULL', 0, N'admin');
GO
-- Câu 17: Trong C, con trỏ là gì?
INSERT INTO tblAnswer (QuestionId, AnswerText, IsCorrect, CreatedBy) VALUES
(17, N'Biến lưu địa chỉ của biến khác', 1, N'admin'),
(17, N'Biến lưu giá trị số nguyên', 0, N'admin'),
(17, N'Kiểu dữ liệu mới', 0, N'admin'),
(17, N'Hàm đặc biệt', 0, N'admin');

-- Câu 18: Trong C, toán GOtử "&" dùng để làm gì?
INSERT INTO tblAnswer (QuestionId, AnswerText, IsCorrect, CreatedBy) VALUES
(18, N'Lấy địa chỉ của biến', 1, N'admin'),
(18, N'Phép toán AND bit', 0, N'admin'),
(18, N'Phép cộng', 0, N'admin'),
(18, N'Phép nhân', 0, N'admin');
GO
-- Câu 19: Hàm nào được dùng để cấp phát bộ nhớ động trong C?
INSERT INTO tblAnswer (QuestionId, AnswerText, IsCorrect, CreatedBy) VALUES
(19, N'malloc()', 1, N'admin'),
(19, N'alloc()', 0, N'admin'),
(19, N'new', 0, N'admin'),
(19, N'create()', 0, N'admin');

GO

CREATE TABLE tblAttemptAnswer (
    AttemptAnswerId INT IDENTITY(1,1) PRIMARY KEY,

    AttemptId INT NOT NULL,                    -- FK to tblAttempt
    QuestionId INT NOT NULL,                   -- FK to tblQuestion
    AnswerId INT NULL,                         -- nullable because user may skip

    IsCorrect BIT NOT NULL DEFAULT 0,          -- store correct/wrong

    -- System fields (as required in your project)
    CreatedAt DATETIME NOT NULL DEFAULT GETDATE(),
    CreatedBy NVARCHAR(255) NULL,
    UpdatedAt DATETIME NULL,
    UpdatedBy NVARCHAR(255) NULL,

    CONSTRAINT FK_AttemptAnswer_Attempt FOREIGN KEY (AttemptId)
        REFERENCES tblExamAttempt(AttemptId),

    CONSTRAINT FK_AttemptAnswer_Question FOREIGN KEY (QuestionId)
        REFERENCES tblQuestion(QuestionId),

    CONSTRAINT FK_AttemptAnswer_Answer FOREIGN KEY (AnswerId)
        REFERENCES tblAnswer(AnswerId)
);

-- use master
-- ALTER DATABASE test SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
-- DROP DATABASE test;
