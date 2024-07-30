using System;
using LibraryAPI6.Models;

namespace LibraryAPI6.Services
{
	public interface ITokenService
	{
        Task<string> GenerateToken(ApplicationUser user);
    }
}
