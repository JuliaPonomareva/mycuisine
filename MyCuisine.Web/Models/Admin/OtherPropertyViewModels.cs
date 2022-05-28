using Microsoft.AspNetCore.Mvc.ViewFeatures;
using MyCuisine.Data.Web.Models;
using System.ComponentModel.DataAnnotations;
using static MyCuisine.Web.Models.TableViewModel;

namespace MyCuisine.Web.Models
{
    public class OtherPropertiesViewModel
    {
        public List<OtherProperty> Entries { get; set; } = new List<OtherProperty>();

        public TableViewModel GetTableViewModel(ViewDataDictionary ViewData)
        {
            return new TableViewModel
            {
                Name = (string)ViewData["Title"],
                CreateUrl = () => "/Admin/OtherPropertyCreate",
                UpdateUrl = (id) => $"/Admin/OtherProperties/{id}",
                Items = Entries ?? new List<OtherProperty>(),
                Columns = new List<TableColumn>
                {
                    new TableColumn(nameof(OtherProperty.Id))
                    {
                        IsIdentifier = true
                    },
                    new TableColumn(nameof(OtherProperty.Name), "Название"),
                    new TableColumn(nameof(OtherProperty.IsActive), "Статус"),
                    new TableColumn
                    {
                        IsEdit = true
                    }
                }
            };
        }
    }

    public class OtherPropertyCreateViewModel
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

    public class OtherPropertyUpdateViewModel
    {
        public FormModel Form { get; set; }
        public class FormModel: OtherPropertyCreateViewModel.FormModel
        {
        }
    }
}
