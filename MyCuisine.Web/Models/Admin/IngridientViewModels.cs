using Microsoft.AspNetCore.Mvc.ViewFeatures;
using MyCuisine.Data.Web.Models;
using System.ComponentModel.DataAnnotations;
using static MyCuisine.Web.Models.TableViewModel;

namespace MyCuisine.Web.Models
{
    public class IngridientsViewModel
    {
        public List<Ingridient> Entries { get; set; } = new List<Ingridient>();

        public TableViewModel GetTableViewModel(ViewDataDictionary ViewData)
        {
            return new TableViewModel
            {
                Name = (string)ViewData["Title"],
                CreateUrl = () => "/Admin/IngridientCreate",
                UpdateUrl = (id) => $"/Admin/Ingridients/{id}",
                Items = Entries ?? new List<Ingridient>(),
                Columns = new List<TableColumn>
                {
                    new TableColumn(nameof(Ingridient.Id))
                    {
                        IsIdentifier = true
                    },
                    new TableColumn(nameof(Ingridient.Name), "Название"),
                    new TableColumn(nameof(Ingridient.Image), "Картинка")
                    {
                        IsImage = true
                    },
                    new TableColumn(nameof(Ingridient.IsActive), "Статус"),
                    new TableColumn
                    {
                        IsEdit = true
                    }
                }
            };
        }
    }

    public class IngridientCreateViewModel
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

    public class IngridientUpdateViewModel
    {
        public string Image { get; set; }
        public FormModel Form { get; set; }
        public class FormModel: IngridientCreateViewModel.FormModel
        {
            public bool RemoveImage { get; set; }
        }
    }
}
