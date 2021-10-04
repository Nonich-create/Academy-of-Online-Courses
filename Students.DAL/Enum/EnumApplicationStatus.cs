using System.ComponentModel.DataAnnotations;

namespace Students.DAL.Enum
{
    public enum EnumApplicationStatus
    {
        [Display(Name = "Открыта")]
        Open,
        [Display(Name = "Закрыта")]
        Close,
        [Display(Name = "Отменена")]
        Cancelled
    }
}
