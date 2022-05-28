using Microsoft.AspNetCore.Mvc.ViewFeatures;
using MyCuisine.Data.Web.Models;
using static MyCuisine.Web.Models.TableViewModel;

namespace MyCuisine.Web.Models
{
    public class UsersViewModel
    {
        public List<User> Entries { get; set; } = new List<User>();

        public TableViewModel GetTableViewModel(ViewDataDictionary ViewData)
        {
            return new TableViewModel
            {
                Name = (string)ViewData["Title"],
                //CreateUrl = () => "/Admin/CreateUser",
                UpdateUrl = (id) => $"/Admin/Users/{id}",
                //DeleteUrl = (id) => $"/Admin/Users/{id}",
                Items = Entries ?? new List<User>(),
                Columns = new List<TableColumn>
                {
                    new TableColumn(nameof(User.Id))
                    {
                        IsIdentifier = true
                    },
                    new TableColumn(nameof(User.Email), "Почта"),
                    new TableColumn(nameof(User.Name), "Имя"),
                    new TableColumn(nameof(User.IsAdmin), "Администратор"),
                    new TableColumn(nameof(User.IsActive), "Статус"),
                    //new TableColumn(nameof(User.DateCreated), "Дата создания"),
                    //new TableColumn(nameof(User.DateModified), "Дата обновления"),
                    new TableColumn
                    {
                        IsEdit = true
                    },
                    //new TableColumn
                    //{
                    //    IsRemove = true
                    //}
                }
            };
        }
    }

    public class UserUpdateViewModel
    {
        public string Email { get; set; }
        public string Name { get; set; }
        public bool IsActive { get; set; }
        public bool IsAdmin { get; set; }
        public DateTimeOffset DateCreated { get; set; }
        public DateTimeOffset DateModified { get; set; }
        public FormModel Form { get; set; }
        public class FormModel
        {
            public int Id { get; set; }
        }
    }
}
