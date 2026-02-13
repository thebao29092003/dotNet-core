using coreC_.Dtos.Stock;
using coreC_.Helpers;
using coreC_.Models;

namespace coreC_.Interfaces
{
    public interface IStockRepository
    {

        Task<List<Stock>> GetAllStocksAsync(); // Trả về Stock
        Task<List<Stock>> GetAllStocksCommentAsync(); // Trả về Stock + comments của nó
        Task<Stock?> GetStockByIdAsync(int id); // Trả về Stock
        Task<Stock?> GetStockCommentByIdAsync(int id); // Trả về Stock
        Task<Stock> CreateStockAsync(Stock stockModel); // Nhận vào Stock
        Task<Stock?> UpdateStockAsync(int id, UpdateStockRequestDto updateStock);
        Task<Stock?> DeleteStockAsync(int id);
        Task<bool> StockExistsAsync(int id);
        Task<List<Stock>> GetAllAsyncSearch(QueryObject query); 
    }
}
