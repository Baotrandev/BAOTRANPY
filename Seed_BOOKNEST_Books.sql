/* BOOKNEST - Du lieu mau sach cho website
   Chay SAU Create_BOOKNEST_Database.sql trong SSMS.
   Script co the chay lai: khong chen trung ISBN/category/publisher. */
USE [bookstore_db];
GO

/* Danh muc */
IF NOT EXISTS (SELECT 1 FROM dbo.categories WHERE category_name = N'Văn học Việt Nam')
    INSERT dbo.categories(category_name, description) VALUES (N'Văn học Việt Nam', N'Tác phẩm văn học Việt Nam');
IF NOT EXISTS (SELECT 1 FROM dbo.categories WHERE category_name = N'Văn học nước ngoài')
    INSERT dbo.categories(category_name, description) VALUES (N'Văn học nước ngoài', N'Tác phẩm văn học nước ngoài');
IF NOT EXISTS (SELECT 1 FROM dbo.categories WHERE category_name = N'Kỹ năng sống')
    INSERT dbo.categories(category_name, description) VALUES (N'Kỹ năng sống', N'Sách phát triển bản thân và kỹ năng');
IF NOT EXISTS (SELECT 1 FROM dbo.categories WHERE category_name = N'Công nghệ - Lập trình')
    INSERT dbo.categories(category_name, description) VALUES (N'Công nghệ - Lập trình', N'Sách công nghệ thông tin, lập trình và dữ liệu');
IF NOT EXISTS (SELECT 1 FROM dbo.categories WHERE category_name = N'Thiếu nhi')
    INSERT dbo.categories(category_name, description) VALUES (N'Thiếu nhi', N'Sách dành cho thiếu nhi');
GO

/* Nhà xuất bản dùng cho dữ liệu demo */
IF NOT EXISTS (SELECT 1 FROM dbo.publishers WHERE publisher_name = N'BOOKNEST Demo')
    INSERT dbo.publishers(publisher_name, description) VALUES (N'BOOKNEST Demo', N'Nhà xuất bản mẫu dùng cho dữ liệu trình diễn');
GO

DECLARE @LiteratureVN int = (SELECT TOP 1 category_id FROM dbo.categories WHERE category_name = N'Văn học Việt Nam' ORDER BY category_id);
DECLARE @LiteratureWorld int = (SELECT TOP 1 category_id FROM dbo.categories WHERE category_name = N'Văn học nước ngoài' ORDER BY category_id);
DECLARE @Skills int = (SELECT TOP 1 category_id FROM dbo.categories WHERE category_name = N'Kỹ năng sống' ORDER BY category_id);
DECLARE @Tech int = (SELECT TOP 1 category_id FROM dbo.categories WHERE category_name = N'Công nghệ - Lập trình' ORDER BY category_id);
DECLARE @Kids int = (SELECT TOP 1 category_id FROM dbo.categories WHERE category_name = N'Thiếu nhi' ORDER BY category_id);
DECLARE @Publisher int = (SELECT TOP 1 publisher_id FROM dbo.publishers WHERE publisher_name = N'BOOKNEST Demo' ORDER BY publisher_id);

DECLARE @Books TABLE (
    ISBN varchar(20) NOT NULL,
    book_title nvarchar(250) NOT NULL,
    category_id int NOT NULL,
    description nvarchar(max) NULL,
    image_file nvarchar(500) NOT NULL,
    original_price decimal(12,2) NOT NULL,
    sale_price decimal(12,2) NOT NULL,
    page_count int NULL
);

INSERT INTO @Books(ISBN, book_title, category_id, description, image_file, original_price, sale_price, page_count) VALUES
('BN0000000001', N'Thiên Thần Nhỏ Của Tôi', @LiteratureVN, N'Câu chuyện nhẹ nhàng về tuổi trẻ và những rung động đầu đời.', N'~/Images/thienthannhocuatoi.jpg', 95000, 79000, 220),
('BN0000000002', N'Lập Trình Cơ Bản', @Tech, N'Tài liệu nhập môn lập trình dành cho người mới bắt đầu.', N'~/Images/laptrinhbook.jpg', 150000, 129000, 320),
('BN0000000003', N'Tư Duy Phản Biện', @Skills, N'Rèn luyện khả năng phân tích, đánh giá và lập luận.', N'~/Images/tuduyphanbien.jpg', 120000, 99000, 260),
('BN0000000004', N'Mùa Hè Không Tên', @LiteratureVN, N'Tác phẩm văn học về ký ức, tuổi trẻ và tình cảm.', N'~/Images/muahekhongtenvaqua.jpg', 110000, 89000, 240),
('BN0000000005', N'Java Cơ Bản', @Tech, N'Giới thiệu ngôn ngữ Java và các kiến thức lập trình nền tảng.', N'~/Images/javabook.jpg', 180000, 155000, 420),
('BN0000000006', N'Trại Hoa Vàng', @LiteratureVN, N'Tác phẩm văn học Việt Nam dành cho bạn đọc yêu truyện.', N'~/Images/traihoavang.jpg', 105000, 89000, 280),
('BN0000000007', N'Hoàng Tử Bé', @LiteratureWorld, N'Câu chuyện kinh điển về tình bạn, tình yêu và cách nhìn thế giới.', N'~/Images/hoangtube.jpg', 85000, 69000, 96),
('BN0000000008', N'Tiệm Sách Của Nàng', @LiteratureVN, N'Một câu chuyện dành cho những người yêu sách và sự dịu dàng.', N'~/Images/tiemsachcuanang.jpg', 115000, 95000, 250),
('BN0000000009', N'Đắc Nhân Tâm', @Skills, N'Những nguyên tắc giao tiếp và xây dựng mối quan hệ tích cực.', N'~/Images/dacnhantam.jpg', 110000, 89000, 320),
('BN0000000010', N'Sách Thiếu Nhi - Tập 1', @Kids, N'Tuyển tập truyện và nội dung đọc dành cho thiếu nhi.', N'~/Images/sachthieunhi1.jpg', 65000, 55000, 96),
('BN0000000011', N'Lập Trình Và Cuộc Sống', @Tech, N'Những câu chuyện và kiến thức xoay quanh nghề lập trình.', N'~/Images/laptrinhvacuocsong.jpg', 145000, 125000, 300),
('BN0000000012', N'Tôi Thấy Hoa Vàng Trên Cỏ Xanh', @LiteratureVN, N'Tác phẩm văn học về tuổi thơ và miền quê Việt Nam.', N'~/Images/toithayhoavangtrencoxanh.jpg', 125000, 105000, 380),
('BN0000000013', N'Giàn Diếp Mặn', @LiteratureVN, N'Tác phẩm văn học Việt Nam.', N'~/Images/giandiepmang.jpg', 90000, 75000, 210),
('BN0000000014', N'Data Driven', @Tech, N'Nhập môn tư duy và ứng dụng dữ liệu trong công việc.', N'~/Images/datadriven.jpg', 190000, 165000, 360),
('BN0000000015', N'Chí Phèo', @LiteratureVN, N'Tác phẩm văn học kinh điển của Nam Cao.', N'~/Images/chipheo.jpg', 70000, 59000, 160),
('BN0000000016', N'Cấu Trúc Dữ Liệu Và Giải Thuật', @Tech, N'Kiến thức nền tảng về cấu trúc dữ liệu và giải thuật.', N'~/Images/datagiaithuat.jpg', 200000, 175000, 440),
('BN0000000017', N'Lập Trình Di Động', @Tech, N'Giới thiệu các kiến thức phát triển ứng dụng di động.', N'~/Images/laptrinhdidong.jpg', 185000, 159000, 380),
('BN0000000018', N'Khoa Học Khám Phá', @LiteratureWorld, N'Sách khoa học phổ thông dành cho bạn đọc ham tìm hiểu.', N'~/Images/khoahoc.png', 130000, 109000, 280),
('BN0000000019', N'Ngày Xưa Có Một Chuyện Tình', @LiteratureVN, N'Câu chuyện tình cảm và ký ức qua năm tháng.', N'~/Images/ngayxuacomotchuyentinh.jpg', 120000, 99000, 300),
('BN0000000020', N'Ngồi Khóc Trên Cây', @LiteratureVN, N'Tác phẩm văn học về tuổi trẻ và những cảm xúc trong trẻo.', N'~/Images/ngoikhoctrencay.jpg', 115000, 95000, 290),
('BN0000000021', N'Trí Tuệ Nhân Tạo', @Tech, N'Tổng quan về trí tuệ nhân tạo và các ứng dụng hiện đại.', N'~/Images/ai.jpg', 210000, 185000, 400),
('BN0000000022', N'Phân Tích Dữ Liệu', @Tech, N'Kiến thức cơ bản về phân tích và khai thác dữ liệu.', N'~/Images/phantichdata.jpg', 195000, 169000, 360),
('BN0000000023', N'Mindset - Tâm Lý Học Thành Công', @Skills, N'Khám phá tư duy phát triển và cách học hỏi từ thử thách.', N'~/Images/mindset.jpg', 135000, 115000, 300),
('BN0000000024', N'Khởi Nghiệp', @Skills, N'Những kiến thức nhập môn về ý tưởng và hành trình khởi nghiệp.', N'~/Images/khoinghiep.jpg', 145000, 125000, 320),
('BN0000000025', N'Big Data', @Tech, N'Giới thiệu dữ liệu lớn và các khái niệm liên quan.', N'~/Images/bigdata.jpg', 220000, 189000, 420),
('BN0000000026', N'Sách Thiếu Nhi - Tập 2', @Kids, N'Tuyển tập truyện đọc dành cho thiếu nhi.', N'~/Images/sachthieunhi2.jpg', 65000, 55000, 100),
('BN0000000027', N'Sách Thiếu Nhi - Tập 3', @Kids, N'Tuyển tập truyện đọc dành cho thiếu nhi.', N'~/Images/sachthieunhi3.jpg', 65000, 55000, 100);

INSERT INTO dbo.books(category_id, publisher_id, ISBN, book_title, description, publication_date, page_count, language, book_format, cover_image_url, original_price, sale_price, status)
SELECT b.category_id, @Publisher, b.ISBN, b.book_title, b.description, '2024-01-01', b.page_count, N'Tiếng Việt', N'Bìa mềm', b.image_file, b.original_price, b.sale_price, N'Đang bán'
FROM @Books b
WHERE NOT EXISTS (SELECT 1 FROM dbo.books existing WHERE existing.ISBN = b.ISBN);
GO

/* Tạo tồn kho demo cho sách vừa thêm hoặc sách đã có nhưng chưa có tồn kho */
INSERT INTO dbo.inventory(book_id, quantity_in_stock, quantity_reserved, reorder_level)
SELECT b.book_id, 30, 0, 5
FROM dbo.books b
WHERE b.ISBN LIKE 'BN00000000%'
  AND NOT EXISTS (SELECT 1 FROM dbo.inventory i WHERE i.book_id = b.book_id);
GO

SELECT COUNT(*) AS SoLuongSach FROM dbo.books;
SELECT book_id, book_title, cover_image_url, sale_price FROM dbo.books ORDER BY book_id;
GO
