/****** Object:  Table [dbo].[IngredientDetail]    Script Date: 18-Sep-20 20:13:39 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[IngredientDetail](
	[RecipeId] [int] NOT NULL,
	[IngredientId] [int] NOT NULL,
	[DenominationId] [int] NOT NULL,
	[Amount] [float] NOT NULL,
 CONSTRAINT [PK_IngredientDetail_1] PRIMARY KEY CLUSTERED 
(
	[RecipeId] ASC,
	[IngredientId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[IngredientDetail]  WITH CHECK ADD  CONSTRAINT [FK_IngredientDetail_Denomination] FOREIGN KEY([DenominationId])
REFERENCES [dbo].[Denomination] ([Id])
GO

ALTER TABLE [dbo].[IngredientDetail] CHECK CONSTRAINT [FK_IngredientDetail_Denomination]
GO

ALTER TABLE [dbo].[IngredientDetail]  WITH CHECK ADD  CONSTRAINT [FK_IngredientDetail_Ingredient] FOREIGN KEY([IngredientId])
REFERENCES [dbo].[Ingredient] ([Id])
GO

ALTER TABLE [dbo].[IngredientDetail] CHECK CONSTRAINT [FK_IngredientDetail_Ingredient]
GO

ALTER TABLE [dbo].[IngredientDetail]  WITH CHECK ADD  CONSTRAINT [FK_IngredientDetail_Recipe] FOREIGN KEY([RecipeId])
REFERENCES [dbo].[Recipe] ([Id])
GO

ALTER TABLE [dbo].[IngredientDetail] CHECK CONSTRAINT [FK_IngredientDetail_Recipe]
GO


