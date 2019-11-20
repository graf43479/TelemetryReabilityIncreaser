//Интерфейс для классаб формирующего комбинации ВХД и рассчитывающего итоговое значение 

// ---------------------------------------------------------------------------
// Авторское право © ООО "%CompanyName". Авторские права защищены.
// Copyright. JSC «%CompanyName» 2016. All rights reserved
// Компания: %CompanyName
// Подразделение: %Department
// Author: Oleg Vorontsov
// Description: Интерфейс определяет методы:
//              - GetFilteredCombinations - возвращает количество возможных комбинаций блоков данных 
//                  (в зависимости от количества исходных каналов каждая комбинация может быть 3,4 или 5-значной)
//              - InitializeMatrixes - десереализирует исходные данные из JSON файлов
//              - PerformCombination - применяет основной алгоритм к выбранной комбинации и возвращает нормализованный блок данных
// Rational: Отобразить основные функции класса-реализатора  
// ---------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Text;

namespace TelemetryEngine.Interfaces
{
    public interface IEngine
    {
        IEnumerable<Items> GetFilteredCombinations();
        void InitializeMatrixes();
        RawDataMatrix PerformCombination(string testCase);
    }
}
