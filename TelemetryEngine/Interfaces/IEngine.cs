//Интерфейс для классаб формирующего комбинации ВХД и рассчитывающего итоговое значение 

// ---------------------------------------------------------------------------
// Авторское право © ООО "%CompanyName". Авторские права защищены.
// Copyright. JSC «%CompanyName» 2016. All rights reserved
// Компания: %CompanyName
// Подразделение: %Department
// Author: Oleg Vorontsov
// Description: Интерфейс определяет методы:
//              - InitializeMatrixes - десереализирует исходные данные из JSON файлов
//              - GetFilteredCombinations - возвращает количество возможных комбинаций блоков данных 
//                  (в зависимости от количества исходных каналов каждая комбинация может быть 3,4 или 5-значной)
//              - PerformCombination - применяет основной алгоритм к выбранной комбинации и возвращает нормализованный блок данных
//              - GetMatrixDifference - возвращает блоки итоговой матрицы, несоответствующие эталонной
//              - Etalon - доступ к проверочной матрице
//              - Gamma - показатель эффективности примененного алгоритма
// Rational: Отобразить основные функции класса-реализатора  
// ---------------------------------------------------------------------------

using System.Collections.Generic;

namespace TelemetryEngine.Interfaces
{
    public interface IEngine
    {
        IEnumerable<Items> GetFilteredCombinations();
        void InitializeMatrixes();
        RawDataMatrix PerformCombination(string testCase);

        List<MismatchesCoordList> GetMatrixDifference();
        Matrix Etalon { get; }
        string Gamma { get;  }
    }
}
