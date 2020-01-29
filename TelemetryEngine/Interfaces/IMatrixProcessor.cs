//Интерфейс для класса расчета комбинации 

// ---------------------------------------------------------------------------
// Авторское право © ООО "%CompanyName". Авторские права защищены.
// Copyright. JSC «%CompanyName» 2016. All rights reserved
// Компания: %CompanyName
// Подразделение: %Department
// Author: Oleg Vorontsov
// Description: Интерфейс определяет метод GetResult, который возвращает итоговоый блок данных, 
//              максимально очищенный от недостоверных данных 
// Rational: Отобразить основные функции класса-реализатора  
// ---------------------------------------------------------------------------

namespace TelemetryEngine.Interfaces
{
    public interface IMatrixProcessor
    {
       RawDataMatrix GetResult();
    }
}
