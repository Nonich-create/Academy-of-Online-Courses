using System.ComponentModel.DataAnnotations;

namespace Students.DAL.Models.Base
{
    public class BaseModel
    {
        [Key]
        public int Id { get; set; }
    }
}
