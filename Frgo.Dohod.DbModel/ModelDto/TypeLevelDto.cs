using Monica.CrmDbModel.ModelDto.Core;

namespace Frgo.Dohod.DbModel.ModelDto
{
    /// <summary>
    /// модель данных для передачи типов уровней орг
    /// </summary>
    public class TypeLevelDto : BaseModelDto
    {
        public TypeLevelDto()
        {
            IsDeleted = false;
        }
        public string Caption { get; set; }
    }

}
