using TinyUrlProto.Models;
using TinyUrlProto.Repositories;

namespace TinyUrlProto.Services;

public class UserService
{
    readonly UserRepository _userRepository;
    public UserService(UserRepository userRepository) => _userRepository = userRepository;

    public async Task<User> GetUser(Guid userId) => await _userRepository.GetUser(userId);
    public async Task<User> UpdateUser(Guid userId, User user) => await _userRepository.UpdateUser(userId, user);
}