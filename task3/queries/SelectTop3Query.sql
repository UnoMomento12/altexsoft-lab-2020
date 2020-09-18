/****** Select top 3 with ingredients  ******/
declare @targetNumber as int = 3;
with cte_cat as (
select Id, Name, ParentId from dbo.Category where Id=@targetNumber union all
select cat.Id, cat.Name, cat.ParentId from dbo.Category cat inner join cte_cat cte on cte.Id = cat.ParentId
)

select result.CategoryName, result.RecipeName, result.Description, ing.Name, ingD.Amount, den.DenominationName from
	(select top 3 cat.Name as CategoryName, rec.Name as RecipeName, rec.Description, rec.Id from Recipe rec 
	inner join cte_cat cat on rec.CategoryId = cat.Id) result
inner join IngredientDetail ingD on ingD.RecipeId = result.Id
inner join Ingredient ing on ingD.IngredientId = ing.Id
inner join Denomination den on ingD.DenominationId = den.Id 