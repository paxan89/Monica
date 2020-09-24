using Monica.Core.DbModel.Core;

namespace Monica.Core.DbModel.ModelDto.Core
{
    /// <summary>
    /// Базовая модель для Dto классов
    /// </summary>
    public class BaseModelDto : IBaseModel
    {
        public int Id { get; set; }
        public bool IsDeleted { get; set; }
    }
}
