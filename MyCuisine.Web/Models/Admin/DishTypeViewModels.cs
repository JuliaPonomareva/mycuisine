using Microsoft.AspNetCore.Mvc.ViewFeatures;
using MyCuisine.Data.Web.Models;
using System.ComponentModel.DataAnnotations;
using static MyCuisine.Web.Models.TableViewModel;

namespace MyCuisine.Web.Models
{
    public class DishTypesViewModel
    {
        public List<DishType> Entries { get; set; } = new List<DishType>();

        public TableViewModel GetTableViewModel(ViewDataDictionary ViewData)
        {
            return new TableViewModel
            {
                Name = (string)ViewData["Title"],
                CreateUrl = () => "/Admin/DishTypeCreate",
                UpdateUrl = (id) => $"/Admin/DishTypes/{id}",
                Items = Entries ?? new List<DishType>(),
                Columns = new List<TableColumn>
                {
                    new TableColumn(nameof(DishType.Id))
                    {
                        IsIdentifier = true
                    },
                    new TableColumn(nameof(DishType.Name), "Название"),
                    new TableColumn(nameof(DishType.IsActive), "Статус"),
                    new TableColumn
                    {
                        IsEdit = true
                    }
                }
            };
        }
    }

    public class DishTypeCreateViewModel
    {
        public FormModel Form { get; set; }
        public class FormModel
        {
            [Required(ErrorMessage = "Обязательное поле.")]
            [StringLength(maximumLength: 100, ErrorMessage = "Максимальная длина 100 символов.")]
            public string Name { get; set; }
            public bool IsActive { get; set; }
        }
    }

    public class DishTypeUpdateViewModel
    {
        public FormModel Form { get; set; }
        public class FormModel: DishTypeCreateViewModel.FormModel
        {
        }
    }
}
