using System.Collections.Concurrent;
using System.Linq.Expressions;

namespace MyCuisine.Web.Models
{
    public class TableViewModel
    {
        private static ConcurrentDictionary<string, Func<object, object>> _buffer = new ConcurrentDictionary<string, Func<object, object>>();
        public string Name { get; set; }
        public Func<string> CreateUrl { get; set; }
        public Func<object, string> UpdateUrl { get; set; }
        public Func<object, string> DeleteUrl { get; set; }
        public List<TableColumn> Columns { get; set; } = new List<TableColumn>();
        public IEnumerable<object> Items { get; set; } = new List<object>();

        public class TableColumn
        {
            public TableColumn()
            {

            }
            public TableColumn(string modelField, string columnName = null)
            {
                ModelField = modelField;
                ColumnName = columnName ?? modelField;
            }
            public string ModelField { get; set; }
            public string ColumnName { get; set; }
            public Func<object, string> Url { get; set; }
            public bool IsIdentifier { get; set; }
            public bool IsImage { get; set; }
            public bool IsEdit { get; set; }
            public bool IsRemove { get; set; }
            public Func<object, string, object> Value { get; set; } = (item, modelField) =>
            {
                var type = item.GetType();
                var key = $"{type}_{modelField}";
                Func<object, object> lambda = default;
                if (_buffer.ContainsKey(key))
                {
                    lambda = _buffer[key];
                }
                else
                {
                    var lambdaParameter = Expression.Parameter(typeof(object));
                    var itemType = Expression.TypeAs(lambdaParameter, type);
                    var property = Expression.Property(itemType, modelField);
                    var convert = Expression.TypeAs(property, typeof(object));
                    lambda = Expression.Lambda<Func<object, object>>(convert, lambdaParameter).Compile();
                    _buffer.TryAdd(key, lambda);
                }

                return lambda(item);
            };
        }
    }
}
