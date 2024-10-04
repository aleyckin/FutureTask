using Contracts.Dtos.UserDtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Abstractions
{
    public interface IUserService 
    {
        Task<List<UserDto>> GetAllAsync(CancellationToken cancellationToken = default);
        Task<UserDto> GetUserById(Guid userId, CancellationToken cancellationToken = default);
        Task<UserDto> GetUserByEmail(string email, CancellationToken cancellationToken = default);
        Task<UserDto> ValidateUserCredentials(string email, string password, CancellationToken cancellationToken = default);
        Task<UserDto> CreateAsync(UserDtoForCreate userDtoForCreate, CancellationToken cancellationToken = default);
        Task UpdateAsync(Guid userId, UserDtoForUpdate userDtoForUpdate, CancellationToken cancellationToken = default);
        Task DeleteAsync(Guid userId, CancellationToken cancellationToken = default);
        string GenerateJwtToken(UserDto userDto);
    }
}
