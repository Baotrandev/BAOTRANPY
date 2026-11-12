/* BOOKNEST - SQL Server schema generated from Models/DataWeb.edmx
   Target database: bookstore_db
   Safe to run on an empty bookstore_db database.
   Existing tables are not dropped or overwritten. */

USE [bookstore_db];
GO

IF OBJECT_ID(N'dbo.authors', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.[authors] (
        [author_id] int IDENTITY(1,1) NOT NULL,
        [author_name] nvarchar(150) NOT NULL,
        [biography] nvarchar(max) NULL,
        [nationality] nvarchar(100) NULL,
        [date_of_birth] date NULL,
        [created_at] datetime2(7) NOT NULL CONSTRAINT [DF_authors_created_at] DEFAULT (SYSDATETIME()),
        [author_image_url] nvarchar(500) NULL,
        CONSTRAINT [PK_authors] PRIMARY KEY ([author_id])
    );
END;
GO

IF OBJECT_ID(N'dbo.book_authors', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.[book_authors] (
        [book_id] int NOT NULL,
        [author_id] int NOT NULL,
        [author_order] int NOT NULL,
        CONSTRAINT [PK_book_authors] PRIMARY KEY ([book_id], [author_id])
    );
END;
GO

IF OBJECT_ID(N'dbo.book_excerpts', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.[book_excerpts] (
        [excerpt_id] int IDENTITY(1,1) NOT NULL,
        [book_id] int NOT NULL,
        [excerpt_text] nvarchar(max) NOT NULL,
        [source_note] nvarchar(300) NULL,
        CONSTRAINT [PK_book_excerpts] PRIMARY KEY ([excerpt_id])
    );
END;
GO

IF OBJECT_ID(N'dbo.books', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.[books] (
        [book_id] int IDENTITY(1,1) NOT NULL,
        [category_id] int NOT NULL,
        [publisher_id] int NULL,
        [ISBN] varchar(20) NOT NULL,
        [book_title] nvarchar(250) NOT NULL,
        [description] nvarchar(max) NULL,
        [publication_date] date NULL,
        [page_count] int NULL,
        [language] nvarchar(50) NOT NULL,
        [book_format] nvarchar(50) NOT NULL,
        [cover_image_url] nvarchar(500) NULL,
        [original_price] decimal(12,2) NOT NULL,
        [sale_price] decimal(12,2) NOT NULL,
        [status] nvarchar(30) NOT NULL CONSTRAINT [DF_books_status] DEFAULT (N'Đang bán'),
        [created_at] datetime2(7) NOT NULL CONSTRAINT [DF_books_created_at] DEFAULT (SYSDATETIME()),
        [updated_at] datetime2(7) NULL,
        CONSTRAINT [PK_books] PRIMARY KEY ([book_id])
    );
END;
GO

IF OBJECT_ID(N'dbo.cart_items', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.[cart_items] (
        [cart_item_id] int IDENTITY(1,1) NOT NULL,
        [cart_id] int NOT NULL,
        [book_id] int NOT NULL,
        [quantity] int NOT NULL,
        [added_at] datetime2(7) NOT NULL CONSTRAINT [DF_cart_items_added_at] DEFAULT (SYSDATETIME()),
        CONSTRAINT [PK_cart_items] PRIMARY KEY ([cart_item_id])
    );
END;
GO

IF OBJECT_ID(N'dbo.carts', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.[carts] (
        [cart_id] int IDENTITY(1,1) NOT NULL,
        [customer_id] int NOT NULL,
        [cart_status] nvarchar(30) NOT NULL CONSTRAINT [DF_carts_cart_status] DEFAULT (N'Active'),
        [created_at] datetime2(7) NOT NULL CONSTRAINT [DF_carts_created_at] DEFAULT (SYSDATETIME()),
        [updated_at] datetime2(7) NULL,
        CONSTRAINT [PK_carts] PRIMARY KEY ([cart_id])
    );
END;
GO

IF OBJECT_ID(N'dbo.categories', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.[categories] (
        [category_id] int IDENTITY(1,1) NOT NULL,
        [category_name] nvarchar(120) NOT NULL,
        [description] nvarchar(500) NULL,
        [parent_category_id] int NULL,
        [is_active] bit NOT NULL CONSTRAINT [DF_categories_is_active] DEFAULT ((1)),
        [created_at] datetime2(7) NOT NULL CONSTRAINT [DF_categories_created_at] DEFAULT (SYSDATETIME()),
        CONSTRAINT [PK_categories] PRIMARY KEY ([category_id])
    );
END;
GO

IF OBJECT_ID(N'dbo.customer_addresses', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.[customer_addresses] (
        [address_id] int IDENTITY(1,1) NOT NULL,
        [customer_id] int NOT NULL,
        [recipient_name] nvarchar(150) NOT NULL,
        [phone] varchar(20) NOT NULL,
        [province] nvarchar(100) NOT NULL,
        [district] nvarchar(100) NOT NULL,
        [ward] nvarchar(100) NULL,
        [street_address] nvarchar(255) NOT NULL,
        [is_default] bit NOT NULL CONSTRAINT [DF_customer_addresses_is_default] DEFAULT ((0)),
        [created_at] datetime2(7) NOT NULL CONSTRAINT [DF_customer_addresses_created_at] DEFAULT (SYSDATETIME()),
        CONSTRAINT [PK_customer_addresses] PRIMARY KEY ([address_id])
    );
END;
GO

IF OBJECT_ID(N'dbo.customers', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.[customers] (
        [customer_id] int IDENTITY(1,1) NOT NULL,
        [full_name] nvarchar(150) NOT NULL,
        [email] varchar(150) NOT NULL,
        [password_hash] varchar(255) NOT NULL,
        [phone] varchar(20) NULL,
        [gender] nvarchar(20) NULL,
        [date_of_birth] date NULL,
        [status] nvarchar(30) NOT NULL CONSTRAINT [DF_customers_status] DEFAULT (N'Hoat dong'),
        [created_at] datetime2(7) NOT NULL CONSTRAINT [DF_customers_created_at] DEFAULT (SYSDATETIME()),
        [updated_at] datetime2(7) NULL,
        CONSTRAINT [PK_customers] PRIMARY KEY ([customer_id])
    );
END;
GO

IF OBJECT_ID(N'dbo.inventory', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.[inventory] (
        [inventory_id] int IDENTITY(1,1) NOT NULL,
        [book_id] int NOT NULL,
        [quantity_in_stock] int NOT NULL,
        [quantity_reserved] int NOT NULL CONSTRAINT [DF_inventory_quantity_reserved] DEFAULT ((0)),
        [reorder_level] int NOT NULL CONSTRAINT [DF_inventory_reorder_level] DEFAULT ((5)),
        [last_stock_update] datetime2(7) NOT NULL CONSTRAINT [DF_inventory_last_stock_update] DEFAULT (SYSDATETIME()),
        CONSTRAINT [PK_inventory] PRIMARY KEY ([inventory_id])
    );
END;
GO

IF OBJECT_ID(N'dbo.order_items', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.[order_items] (
        [order_item_id] int IDENTITY(1,1) NOT NULL,
        [order_id] int NOT NULL,
        [book_id] int NOT NULL,
        [quantity] int NOT NULL,
        [unit_price] decimal(12,2) NOT NULL,
        [discount_amount] decimal(12,2) NOT NULL CONSTRAINT [DF_order_items_discount_amount] DEFAULT ((0)),
        [line_total] decimal(12,2) NOT NULL,
        CONSTRAINT [PK_order_items] PRIMARY KEY ([order_item_id])
    );
END;
GO

IF OBJECT_ID(N'dbo.orders', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.[orders] (
        [order_id] int IDENTITY(1,1) NOT NULL,
        [customer_id] int NOT NULL,
        [address_id] int NULL,
        [promotion_id] int NULL,
        [order_code] varchar(30) NOT NULL,
        [order_date] datetime2(7) NOT NULL CONSTRAINT [DF_orders_order_date] DEFAULT (SYSDATETIME()),
        [subtotal] decimal(12,2) NOT NULL CONSTRAINT [DF_orders_subtotal] DEFAULT ((0)),
        [discount_amount] decimal(12,2) NOT NULL CONSTRAINT [DF_orders_discount_amount] DEFAULT ((0)),
        [shipping_fee] decimal(12,2) NOT NULL CONSTRAINT [DF_orders_shipping_fee] DEFAULT ((0)),
        [total_amount] decimal(12,2) NOT NULL CONSTRAINT [DF_orders_total_amount] DEFAULT ((0)),
        [order_status] nvarchar(40) NOT NULL CONSTRAINT [DF_orders_order_status] DEFAULT (N'Chờ xử lý'),
        [payment_status] nvarchar(40) NOT NULL CONSTRAINT [DF_orders_payment_status] DEFAULT (N'Chưa thanh toán'),
        [customer_note] nvarchar(500) NULL,
        CONSTRAINT [PK_orders] PRIMARY KEY ([order_id])
    );
END;
GO

IF OBJECT_ID(N'dbo.payments', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.[payments] (
        [payment_id] int IDENTITY(1,1) NOT NULL,
        [order_id] int NOT NULL,
        [payment_method] nvarchar(40) NOT NULL,
        [transaction_code] varchar(100) NULL,
        [amount] decimal(12,2) NOT NULL,
        [payment_status] nvarchar(40) NOT NULL CONSTRAINT [DF_payments_payment_status] DEFAULT (N'Chưa thanh toán'),
        [payment_date] datetime2(7) NULL,
        [created_at] datetime2(7) NOT NULL CONSTRAINT [DF_payments_created_at] DEFAULT (SYSDATETIME()),
        CONSTRAINT [PK_payments] PRIMARY KEY ([payment_id])
    );
END;
GO

IF OBJECT_ID(N'dbo.promotions', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.[promotions] (
        [promotion_id] int IDENTITY(1,1) NOT NULL,
        [promotion_code] varchar(40) NOT NULL,
        [promotion_name] nvarchar(150) NOT NULL,
        [discount_type] nvarchar(20) NOT NULL,
        [discount_value] decimal(12,2) NOT NULL,
        [minimum_order_value] decimal(12,2) NOT NULL,
        [maximum_discount] decimal(12,2) NULL,
        [start_date] datetime2(7) NOT NULL,
        [end_date] datetime2(7) NOT NULL,
        [usage_limit] int NULL,
        [is_active] bit NOT NULL CONSTRAINT [DF_promotions_is_active] DEFAULT ((1)),
        CONSTRAINT [PK_promotions] PRIMARY KEY ([promotion_id])
    );
END;
GO

IF OBJECT_ID(N'dbo.publishers', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.[publishers] (
        [publisher_id] int IDENTITY(1,1) NOT NULL,
        [publisher_name] nvarchar(150) NOT NULL,
        [email] varchar(150) NULL,
        [phone] varchar(20) NULL,
        [address] nvarchar(255) NULL,
        [website] varchar(255) NULL,
        [description] nvarchar(500) NULL,
        [created_at] datetime2(7) NOT NULL CONSTRAINT [DF_publishers_created_at] DEFAULT (SYSDATETIME()),
        CONSTRAINT [PK_publishers] PRIMARY KEY ([publisher_id])
    );
END;
GO

IF OBJECT_ID(N'dbo.reviews', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.[reviews] (
        [review_id] int IDENTITY(1,1) NOT NULL,
        [customer_id] int NOT NULL,
        [book_id] int NOT NULL,
        [order_id] int NULL,
        [rating] int NOT NULL,
        [review_title] nvarchar(200) NULL,
        [review_content] nvarchar(1000) NULL,
        [review_date] datetime2(7) NOT NULL CONSTRAINT [DF_reviews_review_date] DEFAULT (SYSDATETIME()),
        [is_approved] bit NOT NULL CONSTRAINT [DF_reviews_is_approved] DEFAULT ((0)),
        CONSTRAINT [PK_reviews] PRIMARY KEY ([review_id])
    );
END;
GO

IF OBJECT_ID(N'dbo.shipments', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.[shipments] (
        [shipment_id] int IDENTITY(1,1) NOT NULL,
        [order_id] int NOT NULL,
        [shipping_provider] nvarchar(100) NULL,
        [tracking_number] varchar(100) NULL,
        [shipping_status] nvarchar(40) NOT NULL CONSTRAINT [DF_shipments_shipping_status] DEFAULT (N'Chờ xử lý'),
        [shipped_at] datetime2(7) NULL,
        [delivered_at] datetime2(7) NULL,
        [shipping_note] nvarchar(500) NULL,
        CONSTRAINT [PK_shipments] PRIMARY KEY ([shipment_id])
    );
END;
GO

IF OBJECT_ID(N'dbo.sysdiagrams', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.[sysdiagrams] (
        [name] nvarchar(128) NOT NULL,
        [principal_id] int NOT NULL,
        [diagram_id] int IDENTITY(1,1) NOT NULL,
        [version] int NULL,
        [definition] varbinary(max) NULL,
        CONSTRAINT [PK_sysdiagrams] PRIMARY KEY ([diagram_id])
    );
END;
GO

IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = N'FK__book_auth__autho__464A023C')
BEGIN
    ALTER TABLE dbo.[book_authors] WITH CHECK ADD CONSTRAINT [FK__book_auth__autho__464A023C] FOREIGN KEY ([author_id]) REFERENCES dbo.[authors] ([author_id]);
    ALTER TABLE dbo.[book_authors] CHECK CONSTRAINT [FK__book_auth__autho__464A023C];
END;
GO

IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = N'FK__book_auth__book___4555DE03')
BEGIN
    ALTER TABLE dbo.[book_authors] WITH CHECK ADD CONSTRAINT [FK__book_auth__book___4555DE03] FOREIGN KEY ([book_id]) REFERENCES dbo.[books] ([book_id]);
    ALTER TABLE dbo.[book_authors] CHECK CONSTRAINT [FK__book_auth__book___4555DE03];
END;
GO

IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = N'FK__books__category___3EA8E074')
BEGIN
    ALTER TABLE dbo.[books] WITH CHECK ADD CONSTRAINT [FK__books__category___3EA8E074] FOREIGN KEY ([category_id]) REFERENCES dbo.[categories] ([category_id]);
    ALTER TABLE dbo.[books] CHECK CONSTRAINT [FK__books__category___3EA8E074];
END;
GO

IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = N'FK__books__publisher__3F9D04AD')
BEGIN
    ALTER TABLE dbo.[books] WITH CHECK ADD CONSTRAINT [FK__books__publisher__3F9D04AD] FOREIGN KEY ([publisher_id]) REFERENCES dbo.[publishers] ([publisher_id]);
    ALTER TABLE dbo.[books] CHECK CONSTRAINT [FK__books__publisher__3F9D04AD];
END;
GO

IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = N'FK__cart_item__book___5A50FAE9')
BEGIN
    ALTER TABLE dbo.[cart_items] WITH CHECK ADD CONSTRAINT [FK__cart_item__book___5A50FAE9] FOREIGN KEY ([book_id]) REFERENCES dbo.[books] ([book_id]);
    ALTER TABLE dbo.[cart_items] CHECK CONSTRAINT [FK__cart_item__book___5A50FAE9];
END;
GO

IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = N'FK__cart_item__cart___595CD6B0')
BEGIN
    ALTER TABLE dbo.[cart_items] WITH CHECK ADD CONSTRAINT [FK__cart_item__cart___595CD6B0] FOREIGN KEY ([cart_id]) REFERENCES dbo.[carts] ([cart_id]);
    ALTER TABLE dbo.[cart_items] CHECK CONSTRAINT [FK__cart_item__cart___595CD6B0];
END;
GO

IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = N'FK__carts__customer___53A3FD5A')
BEGIN
    ALTER TABLE dbo.[carts] WITH CHECK ADD CONSTRAINT [FK__carts__customer___53A3FD5A] FOREIGN KEY ([customer_id]) REFERENCES dbo.[customers] ([customer_id]);
    ALTER TABLE dbo.[carts] CHECK CONSTRAINT [FK__carts__customer___53A3FD5A];
END;
GO

IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = N'FK__categorie__paren__3707BEAC')
BEGIN
    ALTER TABLE dbo.[categories] WITH CHECK ADD CONSTRAINT [FK__categorie__paren__3707BEAC] FOREIGN KEY ([parent_category_id]) REFERENCES dbo.[categories] ([category_id]);
    ALTER TABLE dbo.[categories] CHECK CONSTRAINT [FK__categorie__paren__3707BEAC];
END;
GO

IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = N'FK__customer___custo__2C8A3039')
BEGIN
    ALTER TABLE dbo.[customer_addresses] WITH CHECK ADD CONSTRAINT [FK__customer___custo__2C8A3039] FOREIGN KEY ([customer_id]) REFERENCES dbo.[customers] ([customer_id]);
    ALTER TABLE dbo.[customer_addresses] CHECK CONSTRAINT [FK__customer___custo__2C8A3039];
END;
GO

IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = N'FK__inventory__book___4DEB2404')
BEGIN
    ALTER TABLE dbo.[inventory] WITH CHECK ADD CONSTRAINT [FK__inventory__book___4DEB2404] FOREIGN KEY ([book_id]) REFERENCES dbo.[books] ([book_id]);
    ALTER TABLE dbo.[inventory] CHECK CONSTRAINT [FK__inventory__book___4DEB2404];
END;
GO

IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = N'FK__order_ite__book___731CA8B3')
BEGIN
    ALTER TABLE dbo.[order_items] WITH CHECK ADD CONSTRAINT [FK__order_ite__book___731CA8B3] FOREIGN KEY ([book_id]) REFERENCES dbo.[books] ([book_id]);
    ALTER TABLE dbo.[order_items] CHECK CONSTRAINT [FK__order_ite__book___731CA8B3];
END;
GO

IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = N'FK__order_ite__order__7228847A')
BEGIN
    ALTER TABLE dbo.[order_items] WITH CHECK ADD CONSTRAINT [FK__order_ite__order__7228847A] FOREIGN KEY ([order_id]) REFERENCES dbo.[orders] ([order_id]);
    ALTER TABLE dbo.[order_items] CHECK CONSTRAINT [FK__order_ite__order__7228847A];
END;
GO

IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = N'FK__orders__address___6C6FAB24')
BEGIN
    ALTER TABLE dbo.[orders] WITH CHECK ADD CONSTRAINT [FK__orders__address___6C6FAB24] FOREIGN KEY ([address_id]) REFERENCES dbo.[customer_addresses] ([address_id]);
    ALTER TABLE dbo.[orders] CHECK CONSTRAINT [FK__orders__address___6C6FAB24];
END;
GO

IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = N'FK__orders__customer__6B7B86EB')
BEGIN
    ALTER TABLE dbo.[orders] WITH CHECK ADD CONSTRAINT [FK__orders__customer__6B7B86EB] FOREIGN KEY ([customer_id]) REFERENCES dbo.[customers] ([customer_id]);
    ALTER TABLE dbo.[orders] CHECK CONSTRAINT [FK__orders__customer__6B7B86EB];
END;
GO

IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = N'FK__orders__promotio__6D63CF5D')
BEGIN
    ALTER TABLE dbo.[orders] WITH CHECK ADD CONSTRAINT [FK__orders__promotio__6D63CF5D] FOREIGN KEY ([promotion_id]) REFERENCES dbo.[promotions] ([promotion_id]);
    ALTER TABLE dbo.[orders] CHECK CONSTRAINT [FK__orders__promotio__6D63CF5D];
END;
GO

IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = N'FK__payments__order___78D58209')
BEGIN
    ALTER TABLE dbo.[payments] WITH CHECK ADD CONSTRAINT [FK__payments__order___78D58209] FOREIGN KEY ([order_id]) REFERENCES dbo.[orders] ([order_id]);
    ALTER TABLE dbo.[payments] CHECK CONSTRAINT [FK__payments__order___78D58209];
END;
GO

IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = N'FK__reviews__book_id__044734B5')
BEGIN
    ALTER TABLE dbo.[reviews] WITH CHECK ADD CONSTRAINT [FK__reviews__book_id__044734B5] FOREIGN KEY ([book_id]) REFERENCES dbo.[books] ([book_id]);
    ALTER TABLE dbo.[reviews] CHECK CONSTRAINT [FK__reviews__book_id__044734B5];
END;
GO

IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = N'FK__reviews__custome__0353107C')
BEGIN
    ALTER TABLE dbo.[reviews] WITH CHECK ADD CONSTRAINT [FK__reviews__custome__0353107C] FOREIGN KEY ([customer_id]) REFERENCES dbo.[customers] ([customer_id]);
    ALTER TABLE dbo.[reviews] CHECK CONSTRAINT [FK__reviews__custome__0353107C];
END;
GO

IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = N'FK__reviews__order_i__053B58EE')
BEGIN
    ALTER TABLE dbo.[reviews] WITH CHECK ADD CONSTRAINT [FK__reviews__order_i__053B58EE] FOREIGN KEY ([order_id]) REFERENCES dbo.[orders] ([order_id]);
    ALTER TABLE dbo.[reviews] CHECK CONSTRAINT [FK__reviews__order_i__053B58EE];
END;
GO

IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = N'FK__shipments__order__7E8E5B5F')
BEGIN
    ALTER TABLE dbo.[shipments] WITH CHECK ADD CONSTRAINT [FK__shipments__order__7E8E5B5F] FOREIGN KEY ([order_id]) REFERENCES dbo.[orders] ([order_id]);
    ALTER TABLE dbo.[shipments] CHECK CONSTRAINT [FK__shipments__order__7E8E5B5F];
END;
GO

IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = N'FK_book_excerpts_books')
BEGIN
    ALTER TABLE dbo.[book_excerpts] WITH CHECK ADD CONSTRAINT [FK_book_excerpts_books] FOREIGN KEY ([book_id]) REFERENCES dbo.[books] ([book_id]);
    ALTER TABLE dbo.[book_excerpts] CHECK CONSTRAINT [FK_book_excerpts_books];
END;
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'UX_customers_email' AND object_id = OBJECT_ID(N'dbo.customers'))
    CREATE UNIQUE INDEX [UX_customers_email] ON dbo.[customers] ([email]);
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'UX_books_ISBN' AND object_id = OBJECT_ID(N'dbo.books'))
    CREATE UNIQUE INDEX [UX_books_ISBN] ON dbo.[books] ([ISBN]);
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'UX_orders_order_code' AND object_id = OBJECT_ID(N'dbo.orders'))
    CREATE UNIQUE INDEX [UX_orders_order_code] ON dbo.[orders] ([order_code]);
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'UX_promotions_promotion_code' AND object_id = OBJECT_ID(N'dbo.promotions'))
    CREATE UNIQUE INDEX [UX_promotions_promotion_code] ON dbo.[promotions] ([promotion_code]);
GO

PRINT N'BOOKNEST database tables and relationships are ready.';
GO
