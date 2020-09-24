using System;
using System.ComponentModel.DataAnnotations;

namespace Frgo.Dohod.DbModel.ModelCrm.Core
{

    /// <summary>
    /// тип импорта: предварительный, основной, полный
    /// </summary>
    public enum ImportType
    {
        None=0,
        Preview=1, 
        Import = 2,
        FullData = 3
    }

    /// <summary>
    /// вид документа(записи)
    /// </summary>
    public enum VidDoc
    {   
        None = 0,
        Hand = 1,     // 1 - ручной ввод
        BD   = 2,     // 2 - закачан из BD
        Voz  = 3,     // 3 - Переброска возвратов 
        XL   = 4,     // 4 - закачан из МФ-xls файла
        ZF   = 5,     // 5 - закачан из ZF
        VT   = 6,     // 6 - закачан из VT - выписка
        SF   = 7,     // 7 - закачан из SF - справка
        Kor  = 8,     // 8 - Корректировка данных след.дня (по колонке OUT из XLS) 
        OneP = 9,     // 9 - Закачан из ФО-1P - файл для структурного подразделения
        NP   = 10,    // 10- закачан из NP
        IP   = 11,    // 11- закачан из IP
        CE   = 12,    // 12- закачан из CE
        SE   = 13     // 13- закачан из SE
    }



}
