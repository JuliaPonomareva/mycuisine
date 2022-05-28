using Microsoft.AspNetCore.Mvc.ViewFeatures;
using System.ComponentModel.DataAnnotations;
using static MyCuisine.Web.Models.TableViewModel;

namespace MyCuisine.Web.Models
{
    public class CookingStepsViewModel
    {
        public int RecipeId { get; set; }
        public List<CookingStepViewModel> Entries { get; set; } = new List<CookingStepViewModel>();

        public class CookingStepViewModel
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public string Image { get; set; }
            public int OrderNumber { get; set; }
        }

        public TableViewModel GetTableViewModel(ViewDataDictionary ViewData)
        {
            return new TableViewModel
            {
                Name = (string)ViewData["Title"],
                CreateUrl = () => $"/Admin/Recipes/{RecipeId}/CookingStepCreate",
                UpdateUrl = (id) => $"/Admin/Recipes/{RecipeId}/CookingSteps/{id}",
                DeleteUrl = (id) => $"/Admin/Recipes/{RecipeId}/CookingSteps/{id}/Remove",
                Items = Entries ?? new List<CookingStepViewModel>(),
                Columns = new List<TableColumn>
                {
                    new TableColumn(nameof(CookingStepViewModel.Id))
                    {
                        IsIdentifier = true
                    },
                    new TableColumn(nameof(CookingStepViewModel.Name), "Название"),
                    new TableColumn(nameof(CookingStepViewModel.Image), "Картинка")
                    {
                        IsImage = true
                    },
                    new TableColumn(nameof(CookingStepViewModel.OrderNumber), "Порядковый номер"),
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

    public class CookingStepCreateViewModel
    {
        public int RecipeId { get; set; }
        public FormModel Form { get; set; }
        public class FormModel
        {
            [Required(ErrorMessage = "Обязательное поле.")]
            [StringLength(maximumLength: 100, ErrorMessage = "Максимальная длина 100 символов.")]
            public string Name { get; set; }
            public string Description { get; set; }
            public IFormFile Image { get; set; }
            public int OrderNumber { get; set; }
        }
    }

    public class CookingStepUpdateViewModel
    {
        public int RecipeId { get; set; }
        public string Image { get; set; }
        public FormModel Form { get; set; }
        public class FormModel : CookingStepCreateViewModel.FormModel
        {
            public bool RemoveImage { get; set; }
        }
    }
}
