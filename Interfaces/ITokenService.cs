using coreC_.Models;

namespace coreC_.Interfaces
{
    public interface ITokenService
    {
        string CreateToken(AppUser user);
    }
}
