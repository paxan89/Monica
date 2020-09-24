namespace Frgo.Dohod.DbModel.Models.Settings
{
    /// <summary>
    /// Класс режима для настройки доступа
    /// </summary>
    public class ModeItem
    {
        public int Id { get; set; }
        public int ModeId { get; set; }
        public int TypeId { get; set; }
        public int ParentId { get; set; }
        public string Text { get; set; }
        public bool IsMode { get; set; } = true;
    }
}
