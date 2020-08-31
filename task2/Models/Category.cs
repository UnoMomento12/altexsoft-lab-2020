
using Newtonsoft.Json;
namespace task2.Models
{
    class Category : BaseModel
    {

        public string ParentId { get; set; }

        [JsonIgnore]
        public Category Parent { get; set; }
    }
}
