using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using System.ComponentModel.DataAnnotations;
using static MyCuisine.Web.Models.TableViewModel;

namespace MyCuisine.Web.Models
{
    public class RecipeItemsViewModel
    {
        public int RecipeId { get; set; }
        public List<RecipeItemViewModel> Entries { get; set; } = new List<RecipeItemViewModel>();

        public class RecipeItemViewModel
        {
            public int Id { get; set; }
            public float Quantity { get; set; }
            public bool IsMain { get; set; }
            public int OrderNumber { get; set; }
            public string Ingredient { get; set; }
            public string QuantityType { get; set; }
        }

        public TableViewModel GetTableViewModel(ViewDataDictionary ViewData)
        {
            return new TableViewModel
            {
                Name = (string)ViewData["Title"],
                CreateUrl = () => $"/Admin/Recipes/{RecipeId}/ItemCreate",
                UpdateUrl = (id) => $"/Admin/Recipes/{RecipeId}/Items/{id}",
                DeleteUrl = (id) => $"/Admin/Recipes/{RecipeId}/Items/{id}/Remove",
                Items = Entries ?? new List<RecipeItemViewModel>(),
                Columns = new List<TableColumn>
                {
                    new TableColumn(nameof(RecipeItemViewModel.Id))
                    {
                        IsIdentifier = true
                    },
                    new TableColumn(nameof(RecipeItemViewModel.Ingredient), "Ингредиент"),
                    new TableColumn(nameof(RecipeItemViewModel.QuantityType), "Единицы измерения"),
                    new TableColumn(nameof(RecipeItemViewModel.Quantity), "Количество"),
                    new TableColumn(nameof(RecipeItemViewModel.OrderNumber), "Порядковый номер"),
                    new TableColumn(nameof(RecipeItemViewModel.IsMain), "Основной"),
                    new TableColumn
                    {
                        IsEdit = true
                    },
                    new TableColumn
                    {
                        IsRemove = true
                    }
                }
            };
        }
    }

    public class RecipeItemCreateViewModel
    {
        public int RecipeId { get; set; }
        public List<SelectListItem> Ingredients { get; set; } = new List<SelectListItem>();
        public List<SelectListItem> QuantityTypes { get; set; } = new List<SelectListItem>();
        public FormModel Form { get; set; }
        public class FormModel
        {
            public float Quantity { get; set; }
            public bool IsMain { get; set; }
            public int OrderNumber { get; set; }

            [Required(ErrorMessage = "Обязательное поле.")]
            public int IngredientId { get; set; }

            [Required(ErrorMessage = "Обязательное поле.")]
            public int QuantityTypeId { get; set; }
        }
    }

    public class RecipeItemUpdateViewModel
    {
        public int RecipeId { get; set; }
        public List<SelectListItem> Ingredients { get; set; } = new List<SelectListItem>();
        public List<SelectListItem> QuantityTypes { get; set; } = new List<SelectListItem>();
        public FormModel Form { get; set; }
        public class FormModel : RecipeItemCreateViewModel.FormModel
        {
        }
    }
}
