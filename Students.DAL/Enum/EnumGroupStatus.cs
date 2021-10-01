using System.ComponentModel.DataAnnotations;

namespace Students.DAL.Enum
{
    public enum EnumGroupStatus
    {
        [Display(Name = "Набор")]
        Set,
        [Display(Name = "Закрыта")]
        Close,
        [Display(Name = "Отменена")]
        Cancelled,
        [Display(Name = "Обучение")]
        Training
    }
}
