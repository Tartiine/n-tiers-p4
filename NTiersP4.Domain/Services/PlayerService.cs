using NTiersP4.Domain.Repositories;
using NTiersP4.Domain.Model;
using NTiersP4.Domain.Model.Dtos;

namespace NTiersP4.Domain.Services;

public class PlayerService {
    private readonly IPlayerRepository _playerRepository;

    public PlayerService(IPlayerRepository playerRepository)
    {
        _playerRepository = playerRepository;
    }

    public async Task<string> GetLoginById(int id)
    {
        var player = await _playerRepository.GetByIdAsync(id);
        
        if (player == null)
        {
            return null;
        }

        return player.Login;
    }
    
    public async Task<ServiceResponse<Player>> AuthenticateAsync(string login, string password)
    {
        var player = await _playerRepository.GetPlayerByCredentialsAsync(login, password);
        if (player == null)
        {
            return new ServiceResponse<Player>
            {
                Success = false,
                Message = "Invalid credentials"
            };
        }

        return new ServiceResponse<Player>
        {
            Success = true,
            Data = player,
            Message = "Login successful"
        };
    }
}