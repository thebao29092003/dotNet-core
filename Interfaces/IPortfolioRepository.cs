using coreC_.Models;

namespace coreC_.Interfaces
{
    public interface IPortfolioRepository
    {
        Task<List<Stock>> GetUserPortfolio(AppUser user);
        Task<Portfolio> createPortfolio(Portfolio portfolio);
        Task<Portfolio?> DeletePortfolio(AppUser appUser, Stock stock);
    }
}
