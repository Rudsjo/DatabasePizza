USE [DB_A53DDD_grupp5]
GO
/****** Object:  Table [dbo].[BakerOrdersRelationship]    Script Date: 2020-02-12 20:36:30 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[BakerOrdersRelationship](
	[EmployeeID] [int] NOT NULL,
	[OrderID] [int] NOT NULL,
 CONSTRAINT [PK_BakerOrdersRelationship] PRIMARY KEY CLUSTERED 
(
	[EmployeeID] ASC,
	[OrderID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[CashierOrdersRelationship]    Script Date: 2020-02-12 20:36:30 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[CashierOrdersRelationship](
	[EmployeeID] [int] NOT NULL,
	[OrderID] [int] NOT NULL,
 CONSTRAINT [PK_CashierOrdersRelationship] PRIMARY KEY CLUSTERED 
(
	[EmployeeID] ASC,
	[OrderID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Condiments]    Script Date: 2020-02-12 20:36:30 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Condiments](
	[CondimentID] [int] IDENTITY(1,1) NOT NULL,
	[Type] [nvarchar](50) NOT NULL,
	[Price] [float] NOT NULL,
 CONSTRAINT [PK__Condimen__C4AFEF8E240731FF] PRIMARY KEY CLUSTERED 
(
	[CondimentID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY],
 CONSTRAINT [UQ__Condimen__C4AFEF8F8153165C] UNIQUE NONCLUSTERED 
(
	[CondimentID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY],
 CONSTRAINT [UQ__Condimen__F9B8A48B230817EF] UNIQUE NONCLUSTERED 
(
	[Type] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Employees]    Script Date: 2020-02-12 20:36:30 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Employees](
	[UserID] [int] IDENTITY(1,1) NOT NULL,
	[Password] [nvarchar](50) NOT NULL,
	[Role] [nvarchar](50) NOT NULL,
 CONSTRAINT [PK__Employee__1788CCACB5FEAE75] PRIMARY KEY CLUSTERED 
(
	[UserID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY],
 CONSTRAINT [UQ__Employee__1788CCAD4C27149E] UNIQUE NONCLUSTERED 
(
	[UserID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Extra]    Script Date: 2020-02-12 20:36:30 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Extra](
	[ProductID] [int] IDENTITY(1,1) NOT NULL,
	[Type] [nvarchar](50) NOT NULL,
	[Price] [float] NOT NULL,
 CONSTRAINT [PK__Extra__B40CC6EDBE68B90B] PRIMARY KEY CLUSTERED 
(
	[ProductID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY],
 CONSTRAINT [UQ__Extra__B40CC6EC541959B4] UNIQUE NONCLUSTERED 
(
	[ProductID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY],
 CONSTRAINT [UQ__Extra__F9B8A48BFADB9793] UNIQUE NONCLUSTERED 
(
	[Type] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Orders]    Script Date: 2020-02-12 20:36:30 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Orders](
	[OrderID] [int] IDENTITY(1,1) NOT NULL,
	[Pizzas] [nvarchar](max) NOT NULL,
	[Extras] [nvarchar](max) NOT NULL,
	[Price] [float] NOT NULL,
	[Status] [int] NOT NULL,
 CONSTRAINT [PK__Orders__C3905BAFED2F9B9D] PRIMARY KEY CLUSTERED 
(
	[OrderID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY],
 CONSTRAINT [UQ__Orders__C3905BAE879C0280] UNIQUE NONCLUSTERED 
(
	[OrderID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Pizzabase]    Script Date: 2020-02-12 20:36:30 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Pizzabase](
	[PizzabaseID] [int] IDENTITY(1,1) NOT NULL,
	[Type] [nvarchar](50) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[PizzabaseID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY],
UNIQUE NONCLUSTERED 
(
	[PizzabaseID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Pizzas]    Script Date: 2020-02-12 20:36:30 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Pizzas](
	[PizzaID] [int] IDENTITY(1,1) NOT NULL,
	[Type] [nvarchar](50) NOT NULL,
	[Price] [float] NOT NULL,
	[PizzabaseID] [int] NOT NULL,
 CONSTRAINT [PK_PizzasNew] PRIMARY KEY CLUSTERED 
(
	[PizzaID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY],
UNIQUE NONCLUSTERED 
(
	[Type] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[StandardIngredientsRelationships]    Script Date: 2020-02-12 20:36:30 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[StandardIngredientsRelationships](
	[PizzaID] [int] NOT NULL,
	[CondimentID] [int] NOT NULL
) ON [PRIMARY]
GO
ALTER TABLE [dbo].[BakerOrdersRelationship]  WITH CHECK ADD  CONSTRAINT [FK_BakerOrdersRelationship_Employees] FOREIGN KEY([EmployeeID])
REFERENCES [dbo].[Employees] ([UserID])
GO
ALTER TABLE [dbo].[BakerOrdersRelationship] CHECK CONSTRAINT [FK_BakerOrdersRelationship_Employees]
GO
ALTER TABLE [dbo].[BakerOrdersRelationship]  WITH CHECK ADD  CONSTRAINT [FK_OrdersBeingCooked_Orders] FOREIGN KEY([OrderID])
REFERENCES [dbo].[Orders] ([OrderID])
GO
ALTER TABLE [dbo].[BakerOrdersRelationship] CHECK CONSTRAINT [FK_OrdersBeingCooked_Orders]
GO
ALTER TABLE [dbo].[CashierOrdersRelationship]  WITH CHECK ADD  CONSTRAINT [FK_CashierOrdersRelationship_Employees] FOREIGN KEY([EmployeeID])
REFERENCES [dbo].[Employees] ([UserID])
GO
ALTER TABLE [dbo].[CashierOrdersRelationship] CHECK CONSTRAINT [FK_CashierOrdersRelationship_Employees]
GO
ALTER TABLE [dbo].[CashierOrdersRelationship]  WITH CHECK ADD  CONSTRAINT [FK_CashierOrdersRelationship_Orders] FOREIGN KEY([OrderID])
REFERENCES [dbo].[Orders] ([OrderID])
GO
ALTER TABLE [dbo].[CashierOrdersRelationship] CHECK CONSTRAINT [FK_CashierOrdersRelationship_Orders]
GO
ALTER TABLE [dbo].[Pizzas]  WITH CHECK ADD  CONSTRAINT [FK_PizzasNew_Pizzabase] FOREIGN KEY([PizzabaseID])
REFERENCES [dbo].[Pizzabase] ([PizzabaseID])
GO
ALTER TABLE [dbo].[Pizzas] CHECK CONSTRAINT [FK_PizzasNew_Pizzabase]
GO
ALTER TABLE [dbo].[StandardIngredientsRelationships]  WITH CHECK ADD  CONSTRAINT [fk_condimentconnection] FOREIGN KEY([CondimentID])
REFERENCES [dbo].[Condiments] ([CondimentID])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[StandardIngredientsRelationships] CHECK CONSTRAINT [fk_condimentconnection]
GO
ALTER TABLE [dbo].[StandardIngredientsRelationships]  WITH CHECK ADD  CONSTRAINT [fk_pizzaconnection] FOREIGN KEY([PizzaID])
REFERENCES [dbo].[Pizzas] ([PizzaID])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[StandardIngredientsRelationships] CHECK CONSTRAINT [fk_pizzaconnection]
GO
/****** Object:  StoredProcedure [dbo].[AddCondiment]    Script Date: 2020-02-12 20:36:30 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[AddCondiment]
@Type nvarchar(50),
@Price float

AS
BEGIN
SET NOCOUNT ON;

INSERT INTO Condiments ("Type",Price)
VALUES (@Type,@Price)
END
GO
/****** Object:  StoredProcedure [dbo].[AddEmployee]    Script Date: 2020-02-12 20:36:30 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[AddEmployee] 
	-- Add the parameters for the stored procedure here
	@Password nvarchar(50),
	@Role nvarchar(10)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	INSERT INTO Employees (Password, Role) VALUES (@Password, @Role)

END
GO
/****** Object:  StoredProcedure [dbo].[AddExtra]    Script Date: 2020-02-12 20:36:30 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[AddExtra]
@Type nvarchar(50),
@Price float

AS
BEGIN
SET NOCOUNT ON;

INSERT INTO Extra ("Type",Price)
VALUES (@Type,@Price)
END
GO
/****** Object:  StoredProcedure [dbo].[AddOrder]    Script Date: 2020-02-12 20:36:30 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[AddOrder]
	@PizzasJSON nvarchar(max),
	@ExtrasJSON nvarchar(max),
	@OrderPrice float
	
	AS BEGIN

		INSERT INTO Orders (Pizzas, Extras, Price, "Status") VALUES (@PizzasJSON, @ExtrasJSON, @OrderPrice, 0)
		SELECT SCOPE_IDENTITY()
	END
GO
/****** Object:  StoredProcedure [dbo].[AddPizza]    Script Date: 2020-02-12 20:36:30 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[AddPizza]
@Type nvarchar(50),
@Price float,
@PizzabaseID int

AS
--DECLARE @ThisPizzaID int
BEGIN
SET NOCOUNT ON;

INSERT INTO Pizzas ("Type",Price,PizzabaseID)
VALUES (@Type,@Price,@PizzabaseID)
--set @ThisPizzaID = (SELECT Pizzas.PizzaID FROM Pizzas WHERE Pizzas."Type" = @Type)
--return @ThisPizzaID

SELECT * FROM Pizzas WHERE "Type" = @Type

END
GO
/****** Object:  StoredProcedure [dbo].[AddStandardCondimentToPizza]    Script Date: 2020-02-12 20:36:30 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[AddStandardCondimentToPizza]
@PizzaID int,
@CondimentID int

AS
BEGIN

SET NOCOUNT ON;

INSERT INTO StandardIngredientsRelationships(PizzaID,CondimentID)
VALUES (@PizzaID,@CondimentID)

END
GO
/****** Object:  StoredProcedure [dbo].[BakerCancellingCooking]    Script Date: 2020-02-12 20:36:30 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[BakerCancellingCooking] 
	-- Add the parameters for the stored procedure here
		@EmployeeID int,
		@OrderID int
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	DELETE FROM BakerOrdersRelationship WHERE EmployeeID = @EmployeeID AND OrderID = @OrderID

		UPDATE Orders
		SET "Status" = 0
		WHERE Orders.OrderID = @OrderID
END
GO
/****** Object:  StoredProcedure [dbo].[BakerChoosingOrderToCook]    Script Date: 2020-02-12 20:36:30 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[BakerChoosingOrderToCook]
	-- Add the parameters for the stored procedure here
	@EmployeeID int,
	@OrderID int
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	INSERT INTO BakerOrdersRelationship (EmployeeID, OrderID)
	SELECT Employees.UserID, Orders.OrderID
	FROM Employees, Orders
	WHERE Orders.OrderID = @OrderID and Orders."Status" = 0 and Employees.UserID = @EmployeeID and (Employees.Role = 'bagare' or Employees.Role = 'admin')

	UPDATE Orders 
	SET Orders."Status" = 1
	WHERE Orders.OrderID = @OrderID

END
GO
/****** Object:  StoredProcedure [dbo].[CashierCancellingOrder]    Script Date: 2020-02-12 20:36:30 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[CashierCancellingOrder]
	-- Add the parameters for the stored procedure here
		@EmployeeID int,
		@OrderID int
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
		DELETE FROM CashierOrdersRelationship WHERE EmployeeID = @EmployeeID AND OrderID = @OrderID

		UPDATE Orders
		SET "Status" = 0
		WHERE Orders.OrderID = @OrderID
END
GO
/****** Object:  StoredProcedure [dbo].[CheckForExistingCondimentID]    Script Date: 2020-02-12 20:36:30 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[CheckForExistingCondimentID]
@CondimentID int

AS
BEGIN
SET NOCOUNT ON

IF EXISTS(SELECT CondimentID FROM Condiments WHERE CondimentID = @CondimentID)
	SELECT 'true' AS IDExists
ELSE
	SELECT 'false' AS IDExists

END
GO
/****** Object:  StoredProcedure [dbo].[CheckForExistingID]    Script Date: 2020-02-12 20:36:30 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[CheckForExistingID]
@UserID int

AS
BEGIN
SET NOCOUNT ON

	IF EXISTS(SELECT UserID FROM Employees WHERE UserID = @UserID)
		SELECT 'true' AS IDExists
	ELSE
		SELECT 'false' AS IDExists

END

GO
/****** Object:  StoredProcedure [dbo].[CheckForExistingPizzaID]    Script Date: 2020-02-12 20:36:30 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[CheckForExistingPizzaID]
@PizzaID int

AS
BEGIN
SET NOCOUNT ON

IF EXISTS(SELECT PizzaID FROM Pizzas WHERE PizzaID = @PizzaID)
	SELECT 'true' AS IDExists
ELSE
	SELECT 'false' AS IDExists

END
GO
/****** Object:  StoredProcedure [dbo].[CheckForExistingProductID]    Script Date: 2020-02-12 20:36:30 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[CheckForExistingProductID]
@ProductID int

AS
BEGIN
SET NOCOUNT ON

IF EXISTS(SELECT ProductID FROM Extra WHERE ProductID = @ProductID)
	SELECT 'true' AS IDExists
ELSE
	SELECT 'false' AS IDExists

END
GO
/****** Object:  StoredProcedure [dbo].[CheckPassword]    Script Date: 2020-02-12 20:36:30 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Joakim (Grupp5)
-- Create date: 2020-01-21
-- Description:	Kontrollerar om en användare med angivet lösenord finns i databasen och returnerar true/false
-- =============================================
CREATE PROCEDURE [dbo].[CheckPassword] 
	@id int,
	@pass nvarchar(50)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	IF EXISTS(SELECT * FROM Employees WHERE UserID = @id AND Password = @pass)
		SELECT 'true' AS UserExists
	ELSE
		SELECT 'false' AS UserExists
		
END
GO
/****** Object:  StoredProcedure [dbo].[CheckRole]    Script Date: 2020-02-12 20:36:30 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[CheckRole]
	-- Add the parameters for the stored procedure here
	@id int
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	SELECT Employees.Role
	FROM Employees
	WHERE Employees.UserID = @id
END
GO
/****** Object:  StoredProcedure [dbo].[DeleteCondimentByID]    Script Date: 2020-02-12 20:36:30 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[DeleteCondimentByID]
@CondimentID int

AS
BEGIN
SET NOCOUNT ON;

DELETE FROM Condiments
WHERE CondimentID = @CondimentID
END 
GO
/****** Object:  StoredProcedure [dbo].[DeleteCondimentFromPizza]    Script Date: 2020-02-12 20:36:30 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[DeleteCondimentFromPizza]
	-- Add the parameters for the stored procedure here
	@PizzaID int,
	@CondimentID int
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	DELETE FROM StandardIngredientsRelationships WHERE PizzaID = @PizzaID and CondimentID = @CondimentID
END
GO
/****** Object:  StoredProcedure [dbo].[DeleteEmployeeByID]    Script Date: 2020-02-12 20:36:30 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[DeleteEmployeeByID]
@UserID int

AS
BEGIN
SET NOCOUNT ON;

DELETE FROM Employees
WHERE UserID = @UserID
END
GO
/****** Object:  StoredProcedure [dbo].[DeleteExtraByID]    Script Date: 2020-02-12 20:36:30 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[DeleteExtraByID]
@ProductID int

AS
BEGIN
SET NOCOUNT ON;

DELETE FROM Extra
WHERE ProductID = @ProductID
END 
GO
/****** Object:  StoredProcedure [dbo].[DeleteOldOrderByID]    Script Date: 2020-02-12 20:36:30 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[DeleteOldOrderByID]
@OrderID int

AS
BEGIN
SET NOCOUNT ON;

DELETE FROM Orders
WHERE OrderID = @OrderID
END 
GO
/****** Object:  StoredProcedure [dbo].[DeletePizzaByID]    Script Date: 2020-02-12 20:36:30 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[DeletePizzaByID]
@PizzaID int

AS
BEGIN
SET NOCOUNT ON;

DELETE FROM Pizzas
WHERE PizzaID = @PizzaID
END 
GO
/****** Object:  StoredProcedure [dbo].[GetAllCondiments]    Script Date: 2020-02-12 20:36:30 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[GetAllCondiments]

AS
BEGIN
SET NOCOUNT ON;

SELECT * FROM Condiments
END
GO
/****** Object:  StoredProcedure [dbo].[GetAllEmployees]    Script Date: 2020-02-12 20:36:30 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[GetAllEmployees]

AS
BEGIN
SET NOCOUNT ON;

SELECT UserID, Role FROM Employees
END
GO
/****** Object:  StoredProcedure [dbo].[GetAllExtras]    Script Date: 2020-02-12 20:36:30 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[GetAllExtras]

AS
BEGIN
SET NOCOUNT ON;

SELECT * FROM Extra
END
GO
/****** Object:  StoredProcedure [dbo].[GetAllOrders]    Script Date: 2020-02-12 20:36:30 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[GetAllOrders] 
	-- Add the parameters for the stored procedure here
	-- NO PARAMETERS NEEDED
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	SELECT * FROM Orders

END
GO
/****** Object:  StoredProcedure [dbo].[GetAllPendingOrders]    Script Date: 2020-02-12 20:36:30 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[GetAllPendingOrders]  
	-- Add the parameters for the stored procedure here

AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
SELECT * 
FROM Orders
WHERE "Status" = 0
END
GO
/****** Object:  StoredProcedure [dbo].[GetAllPizzabases]    Script Date: 2020-02-12 20:36:30 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[GetAllPizzabases] 
	-- Add the parameters for the stored procedure here
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	SELECT * FROM Pizzabase
END
GO
/****** Object:  StoredProcedure [dbo].[GetAllPizzas]    Script Date: 2020-02-12 20:36:30 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[GetAllPizzas] 

AS
BEGIN
SET NOCOUNT ON;

SELECT Pizzas."type", Pizzas.Price, Pizzas.PizzaID, Pizzas.PizzabaseID, Pizzabase."Type" 
AS Pizzabase 
FROM Pizzabase, Pizzas 
WHERE Pizzabase.PizzabaseID = Pizzas.PizzabaseID

END
GO
/****** Object:  StoredProcedure [dbo].[GetIngredientsFromSpecificPizza]    Script Date: 2020-02-12 20:36:30 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[GetIngredientsFromSpecificPizza]
@PizzaID int

AS
BEGIN
SET NOCOUNT ON;

SELECT Condiments.CondimentID, Price, "Type"
FROM Condiments, StandardIngredientsRelationships
WHERE Condiments.CondimentID = StandardIngredientsRelationships.CondimentID 
AND StandardIngredientsRelationships.PizzaID = @PizzaID

END
GO
/****** Object:  StoredProcedure [dbo].[GetOrderByStatus]    Script Date: 2020-02-12 20:36:30 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[GetOrderByStatus]
@Status int

AS
BEGIN
SET NOCOUNT ON;

SELECT * FROM Orders
WHERE "Status" = @Status

END
GO
/****** Object:  StoredProcedure [dbo].[GetSingleCondiment]    Script Date: 2020-02-12 20:36:30 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[GetSingleCondiment]
@CondimentID int

AS
BEGIN
SET NOCOUNT ON;

SELECT * FROM Condiments
WHERE CondimentID = @CondimentID
END
GO
/****** Object:  StoredProcedure [dbo].[GetSingleEmployee]    Script Date: 2020-02-12 20:36:30 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[GetSingleEmployee]
@UserID int

AS
BEGIN
SET NOCOUNT ON;

SELECT * FROM Employees
WHERE UserID = @UserID
END
GO
/****** Object:  StoredProcedure [dbo].[GetSingleExtra]    Script Date: 2020-02-12 20:36:30 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[GetSingleExtra]
@ProductID int

AS
BEGIN
SET NOCOUNT ON;

SELECT * FROM Extra
WHERE ProductID = @ProductID
END
GO
/****** Object:  StoredProcedure [dbo].[GetSingleOrder]    Script Date: 2020-02-12 20:36:30 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[GetSingleOrder]
	-- Add the parameters for the stored procedure here
	@OrderID int
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	SELECT * FROM Orders WHERE OrderID = @OrderID
END
GO
/****** Object:  StoredProcedure [dbo].[GetSinglePizza]    Script Date: 2020-02-12 20:36:30 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[GetSinglePizza]
@PizzaID int

AS
BEGIN
SET NOCOUNT ON;

SELECT * FROM Pizzas
WHERE PizzaID = @PizzaID
END
GO
/****** Object:  StoredProcedure [dbo].[GetSpecificPizza]    Script Date: 2020-02-12 20:36:30 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[GetSpecificPizza]
@PizzaID int

AS
BEGIN
SET NOCOUNT ON;

SELECT Pizzas.PizzaID, Pizzas."Type", Pizzas.Price, Pizzas.PizzabaseID, Pizzabase."Type" 
AS Pizzabase 
FROM Pizzabase, Pizzas 
WHERE Pizzabase.PizzabaseID = Pizzas.PizzabaseID
AND Pizzas.PizzaID = @PizzaID

END
GO
/****** Object:  StoredProcedure [dbo].[UpdateCondimentByID]    Script Date: 2020-02-12 20:36:30 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[UpdateCondimentByID]
@CondimentID int,
@Type nvarchar(50),
@Price float

AS
BEGIN
SET NOCOUNT ON;

UPDATE Condiments
SET
"Type" = @Type,
Price = @Price
WHERE CondimentID = @CondimentID
END
GO
/****** Object:  StoredProcedure [dbo].[UpdateEmployeeByID]    Script Date: 2020-02-12 20:36:30 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[UpdateEmployeeByID]
@UserID int,
@Role nvarchar(50),
@Password nvarchar(50)

AS
BEGIN
SET NOCOUNT ON;

UPDATE Employees
SET
Role = @Role,
Password = @Password
WHERE UserID = @UserID
END
GO
/****** Object:  StoredProcedure [dbo].[UpdateExtraByID]    Script Date: 2020-02-12 20:36:30 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[UpdateExtraByID]
@ProductID int,
@Type nvarchar(50),
@Price float

AS
BEGIN
SET NOCOUNT ON;

UPDATE Extra
SET
"Type" = @Type,
Price = @Price
WHERE ProductID = @ProductID
END
GO
/****** Object:  StoredProcedure [dbo].[UpdateOrderStatusWhenOrderIsCooked]    Script Date: 2020-02-12 20:36:30 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[UpdateOrderStatusWhenOrderIsCooked] 
	-- Add the parameters for the stored procedure here
		@OrderID int
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
		UPDATE Orders
		SET "Status" = 2
		WHERE Orders.OrderID = @OrderID
END
GO
/****** Object:  StoredProcedure [dbo].[UpdateOrderStatusWhenOrderIsServed]    Script Date: 2020-02-12 20:36:30 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[UpdateOrderStatusWhenOrderIsServed]
	-- Add the parameters for the stored procedure here
		@EmployeeID int,
		@OrderID int
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
		INSERT INTO CashierOrdersRelationship(EmployeeID, OrderID)
		SELECT Employees.UserID, Orders.OrderID
		FROM Employees, Orders
		WHERE Orders.OrderID = @OrderID and Orders."Status" = 2 and Employees.UserID = @EmployeeID and (Employees.Role = 'kassör' or Employees.Role = 'admin')

		UPDATE Orders
		SET Orders."Status" = 3
		WHERE Orders.OrderID = @OrderID
END
GO
/****** Object:  StoredProcedure [dbo].[UpdatePizzaByID]    Script Date: 2020-02-12 20:36:30 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[UpdatePizzaByID]
@PizzaID int,
@Type nvarchar(50),
@Price float,
@PizzabaseID int

AS
BEGIN
SET NOCOUNT ON;

UPDATE Pizzas
SET
"Type" = @Type,
Price = @Price,
PizzabaseID = @PizzabaseID
WHERE PizzaID = @PizzaID
END
GO
