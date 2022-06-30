using WASM_Reatime_D3.Models;

namespace WASM_Reatime_D3.Contracts
{

    public interface ILogRepository
    {
        Task<tbllog> GetLog();
        Task CallChartEndpoint();
    }
}