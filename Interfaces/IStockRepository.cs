using coreC_.Models;

namespace coreC_.Interfaces
{
    public interface IStockRepository
    {
        Task<List<Stock>> GetAllStocksAsync();
    }
}
