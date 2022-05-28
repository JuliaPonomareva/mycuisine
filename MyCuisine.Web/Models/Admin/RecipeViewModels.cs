using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using System.ComponentModel.DataAnnotations;
using static MyCuisine.Web.Models.TableViewModel;

namespace MyCuisine.Web.Models
{
    public class RecipesViewModel
    {
        public List<RecipeViewModel> Entries { get; set; } = new List<RecipeViewModel>();

        public class RecipeViewModel
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public int? PersonsCount { get; set; }
            public string Image { get; set; }
            public float Rate { get; set; }
            public int Votes { get; set; }
            public bool IsActive { get; set; }
            public string DishType { get; set; }
            public string CuisineType { get; set; }
            public int ItemsCount { get; set; }
            public int StepsCount { get; set; }
        }

        public TableViewModel GetTableViewModel(ViewDataDictionary ViewData)
        {
            return new TableViewModel
            {
                Name = (string)ViewData["Title"],
                CreateUrl = () => "/Admin/RecipeCreate",
                UpdateUrl = (id) => $"/Admin/Recipes/{id}",
                Items = Entries ?? new List<RecipeViewModel>(),
                Columns = new List<TableColumn>
                {
                    new TableColumn(nameof(RecipeViewModel.Id))
                    {
                        IsIdentifier = true
                    },
                    new TableColumn(nameof(RecipeViewModel.Name), "Название"),
                    new TableColumn(nameof(RecipeViewModel.Image), "Картинка")
                    {
                        IsImage = true
                    },
                    new TableColumn(nameof(RecipeViewModel.PersonsCount), "Персон"),
                    new TableColumn(nameof(RecipeViewModel.Rate), "Рейтинг"),
                    new TableColumn(nameof(RecipeViewModel.Votes), "Голосов"),
                    new TableColumn(nameof(RecipeViewModel.IsActive), "Статус"),
                    new TableColumn(nameof(RecipeViewModel.DishType), "Блюдо"),
                    new TableColumn(nameof(RecipeViewModel.CuisineType), "Кухня"),
                    new TableColumn(nameof(RecipeViewModel.ItemsCount), "Ингридиенты")
                    {
                        Url = (id) => $"/Admin/Recipes/{id}/Items"
                    },
                    new TableColumn(nameof(RecipeViewModel.StepsCount), "Шаги")
                    {
                        Url = (id) => $"/Admin/Recipes/{id}/CookingSteps"
                    },
                    new TableColumn
                    {
                        IsEdit = true
                    }
                }
            };
        }
    }

    public class RecipeCreateViewModel
    {
        public List<SelectListItem> DishTypes { get; set; } = new List<SelectListItem>();
        public List<SelectListItem> CuisineTypes { get; set; } = new List<SelectListItem>();
        public List<SelectListItem> OtherProperties { get; set; } = new List<SelectListItem>();
        public FormModel Form { get; set; }
        public class FormModel
        {
            [Required(ErrorMessage = "Обязательное поле.")]
            [StringLength(maximumLength: 200, ErrorMessage = "Максимальная длина 200 символов.")]
            public string Name { get; set; }
            public string Description { get; set; }
            public int? PersonsCount { get; set; }
            public IFormFile Image { get; set; }
            public bool IsActive { get; set; }

            [Required(ErrorMessage = "Обязательное поле.")]
            public int DishTypeId { get; set; }

            [Required(ErrorMessage = "Обязательное поле.")]
            public int CuisineTypeId { get; set; }
            public List<int> OtherPropertyIds { get; set; } = new List<int>();
        }
    }

    public class RecipeUpdateViewModel
    {
        public List<SelectListItem> DishTypes { get; set; } = new List<SelectListItem>();
        public List<SelectListItem> CuisineTypes { get; set; } = new List<SelectListItem>();
        public List<SelectListItem> OtherProperties { get; set; } = new List<SelectListItem>();
        public string Image { get; set; }
        public FormModel Form { get; set; }
        public class FormModel : RecipeCreateViewModel.FormModel
        {
            public bool RemoveImage { get; set; }
        }
    }
}
