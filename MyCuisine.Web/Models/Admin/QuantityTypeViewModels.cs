using Microsoft.AspNetCore.Mvc.ViewFeatures;
using MyCuisine.Data.Web.Models;
using System.ComponentModel.DataAnnotations;
using static MyCuisine.Web.Models.TableViewModel;

namespace MyCuisine.Web.Models
{
    public class QuantityTypesViewModel
    {
        public List<QuantityType> Entries { get; set; } = new List<QuantityType>();

        public TableViewModel GetTableViewModel(ViewDataDictionary ViewData)
        {
            return new TableViewModel
            {
                Name = (string)ViewData["Title"],
                CreateUrl = () => "/Admin/QuantityTypeCreate",
                UpdateUrl = (id) => $"/Admin/QuantityTypes/{id}",
                Items = Entries ?? new List<QuantityType>(),
                Columns = new List<TableColumn>
                {
                    new TableColumn(nameof(QuantityType.Id))
                    {
                        IsIdentifier = true
                    },
                    new TableColumn(nameof(QuantityType.Name), "Название"),
                    new TableColumn(nameof(QuantityType.IsActive), "Статус"),
                    new TableColumn
                    {
                        IsEdit = true
                    }
                }
            };
        }
    }

    public class QuantityTypeCreateViewModel
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

    public class QuantityTypeUpdateViewModel
    {
        public FormModel Form { get; set; }
        public class FormModel: QuantityTypeCreateViewModel.FormModel
        {
        }
    }
}
