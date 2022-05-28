using Microsoft.AspNetCore.Mvc.ViewFeatures;
using MyCuisine.Data.Web.Models;
using System.ComponentModel.DataAnnotations;
using static MyCuisine.Web.Models.TableViewModel;

namespace MyCuisine.Web.Models
{
    public class CuisineTypesViewModel
    {
        public List<CuisineType> Entries { get; set; } = new List<CuisineType>();

        public TableViewModel GetTableViewModel(ViewDataDictionary ViewData)
        {
            return new TableViewModel
            {
                Name = (string)ViewData["Title"],
                CreateUrl = () => "/Admin/CuisineTypeCreate",
                UpdateUrl = (id) => $"/Admin/CuisineTypes/{id}",
                Items = Entries ?? new List<CuisineType>(),
                Columns = new List<TableColumn>
                {
                    new TableColumn(nameof(CuisineType.Id))
                    {
                        IsIdentifier = true
                    },
                    new TableColumn(nameof(CuisineType.Name), "Название"),
                    new TableColumn(nameof(CuisineType.IsActive), "Статус"),
                    new TableColumn
                    {
                        IsEdit = true
                    }
                }
            };
        }
    }

    public class CuisineTypeCreateViewModel
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

    public class CuisineTypeUpdateViewModel
    {
        public FormModel Form { get; set; }
        public class FormModel: CuisineTypeCreateViewModel.FormModel
        {
        }
    }
}
