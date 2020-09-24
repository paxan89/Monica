namespace Monica.Core.DbModel.Extension
{
    /// <summary>
    /// Список ошибок обращения к БД
    /// </summary>
    public enum ErrorCode
    {
        NotUser = 1,
        NoCode =2
    }
    /// <summary>
    /// Тип компонентов
    /// </summary>
    public enum TypeControl
    {
        TextEdit=1,
        DateEdit=2,
        ComboBox=3,
        MultiLine=4,
        NumericEdit=5,
        FormSelect=6,
        CheckBox=7,
        Details=8
    }
    /// <summary>
    /// Тип уровня доступа к элементам
    /// </summary>
    public enum TypeAccec
    {
        Full = 3,
        ReadOnly = 2,
        NoAccess = 1
    }
    /// <summary>
    /// Тип профиля для полей модели. Обязателно или не обязательно
    /// </summary>
    public enum TypeProfileForm
    {
        Required =2,
        Advisably =1
    }
    /// <summary>
    /// Тип группы. 0 - нет группы, 1 - группа, 2 - табгруппа
    /// </summary>
    public enum TypeGroup { None=0, Group=1, Tab=2 }

    public enum TypeField { List = 1, Edit = 2, ListAndEdit = 3 }

    public enum TypeBtn
    {
        Normal =1, Success = 2, Danger=3
    }

    public enum StylingMode { Contained =1, Outlined =2 }
    public enum Location { Before = 1, Center= 2, After=3 }
    public enum TypeValidation { Sql = 1, Component = 2 }
    public enum TypeReturnValidation { Boolean = 1, Model = 2  }

}
