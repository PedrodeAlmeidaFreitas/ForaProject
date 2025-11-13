using AutoMapper;
using ForaProject.Application.DTOs;
using ForaProject.Application.Interfaces;
using ForaProject.Domain.Entities;
using ForaProject.Domain.Interfaces;

namespace ForaProject.Application.Services;

/// <summary>
/// Service implementation for authentication and user management operations.
/// </summary>
public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly ITokenService _tokenService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public AuthService(
        IUserRepository userRepository,
        IPasswordHasher passwordHasher,
        ITokenService tokenService,
        IUnitOfWork unitOfWork,
        IMapper mapper)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
        _tokenService = tokenService;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<TokenResponseDto> LoginAsync(LoginDto loginDto)
    {
        // Find user by email
        var user = await _userRepository.GetByEmailAsync(loginDto.Email.ToLowerInvariant());

        if (user == null)
        {
            throw new UnauthorizedAccessException("Invalid email or password.");
        }

        // Check if user is active
        if (!user.IsActive)
        {
            throw new UnauthorizedAccessException("User account is inactive.");
        }

        // Verify password
        if (!_passwordHasher.VerifyPassword(loginDto.Password, user.PasswordHash))
        {
            throw new UnauthorizedAccessException("Invalid email or password.");
        }

        // Record login
        user.RecordLogin();
        await _unitOfWork.SaveChangesAsync();

        // Map to DTO and generate token
        var userDto = _mapper.Map<UserDto>(user);
        var token = _tokenService.GenerateToken(userDto);

        return new TokenResponseDto
        {
            AccessToken = token,
            TokenType = "Bearer",
            ExpiresIn = _tokenService.GetTokenExpirationSeconds(),
            User = userDto
        };
    }

    public async Task<TokenResponseDto> RegisterAsync(RegisterDto registerDto)
    {
        // Check if email already exists
        if (await _userRepository.ExistsByEmailAsync(registerDto.Email.ToLowerInvariant()))
        {
            throw new InvalidOperationException("Email address is already registered.");
        }

        // Check if username already exists
        if (await _userRepository.ExistsByUsernameAsync(registerDto.Username))
        {
            throw new InvalidOperationException("Username is already taken.");
        }

        // Hash password
        var passwordHash = _passwordHasher.HashPassword(registerDto.Password);

        // Create user entity
        var user = User.Create(
            registerDto.Username,
            registerDto.Email,
            passwordHash,
            registerDto.Roles ?? new List<string> { "ReadOnly" } // Default role
        );

        // Save user
        await _userRepository.AddAsync(user);
        await _unitOfWork.SaveChangesAsync();

        // Map to DTO and generate token
        var userDto = _mapper.Map<UserDto>(user);
        var token = _tokenService.GenerateToken(userDto);

        return new TokenResponseDto
        {
            AccessToken = token,
            TokenType = "Bearer",
            ExpiresIn = _tokenService.GetTokenExpirationSeconds(),
            User = userDto
        };
    }

    public async Task<UserDto> GetUserByIdAsync(Guid userId)
    {
        var user = await _userRepository.GetByIdAsync(userId);

        if (user == null)
        {
            throw new InvalidOperationException("User not found.");
        }

        return _mapper.Map<UserDto>(user);
    }

    public async Task ChangePasswordAsync(Guid userId, string currentPassword, string newPassword)
    {
        var user = await _userRepository.GetByIdAsync(userId);

        if (user == null)
        {
            throw new InvalidOperationException("User not found.");
        }

        // Verify current password
        if (!_passwordHasher.VerifyPassword(currentPassword, user.PasswordHash))
        {
            throw new UnauthorizedAccessException("Current password is incorrect.");
        }

        // Hash new password
        var newPasswordHash = _passwordHasher.HashPassword(newPassword);

        // Update password
        user.UpdatePassword(newPasswordHash);

        // Save changes
        await _unitOfWork.SaveChangesAsync();
    }
}
