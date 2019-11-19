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
