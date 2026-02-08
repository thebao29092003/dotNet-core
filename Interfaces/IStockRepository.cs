using coreC_.Dtos.Stock;
using coreC_.Models;

namespace coreC_.Interfaces
{
    public interface IStockRepository
    {

        Task<List<Stock>> GetAllStocksAsync(); // Trả về Stock
        Task<Stock?> GetStockByIdAsync(int id); // Trả về Stock
        Task<Stock> CreateStockAsync(Stock stockModel); // Nhận vào Stock
        Task<Stock?> UpdateStockAsync(int id, UpdateStockRequestDto updateStock);
        Task<Stock?> DeleteStockAsync(int id);
    }
}
