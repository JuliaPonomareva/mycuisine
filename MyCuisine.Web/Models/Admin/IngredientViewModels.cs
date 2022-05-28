using Microsoft.AspNetCore.Mvc.ViewFeatures;
using MyCuisine.Data.Web.Models;
using System.ComponentModel.DataAnnotations;
using static MyCuisine.Web.Models.TableViewModel;

namespace MyCuisine.Web.Models
{
    public class IngredientsViewModel
    {
        public List<Ingredient> Entries { get; set; } = new List<Ingredient>();

        public TableViewModel GetTableViewModel(ViewDataDictionary ViewData)
        {
            return new TableViewModel
            {
                Name = (string)ViewData["Title"],
                CreateUrl = () => "/Admin/IngredientCreate",
                UpdateUrl = (id) => $"/Admin/Ingredients/{id}",
                Items = Entries ?? new List<Ingredient>(),
                Columns = new List<TableColumn>
                {
                    new TableColumn(nameof(Ingredient.Id))
                    {
                        IsIdentifier = true
                    },
                    new TableColumn(nameof(Ingredient.Name), "Название"),
                    new TableColumn(nameof(Ingredient.Image), "Картинка")
                    {
                        IsImage = true
                    },
                    new TableColumn(nameof(Ingredient.IsActive), "Статус"),
                    new TableColumn
                    {
                        IsEdit = true
                    }
                }
            };
        }
    }

    public class IngredientCreateViewModel
    {
        public FormModel Form { get; set; }
        public class FormModel
        {
            [Required(ErrorMessage = "Обязательное поле.")]
            [StringLength(maximumLength: 100, ErrorMessage = "Максимальная длина 100 символов.")]
            public string Name { get; set; }
            public bool IsActive { get; set; }
            public IFormFile Image { get; set; }
        }
    }

    public class IngredientUpdateViewModel
    {
        public string Image { get; set; }
        public FormModel Form { get; set; }
        public class FormModel: IngredientCreateViewModel.FormModel
        {
            public bool RemoveImage { get; set; }
        }
    }
}
