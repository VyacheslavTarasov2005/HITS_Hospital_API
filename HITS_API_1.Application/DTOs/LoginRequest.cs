using System.ComponentModel.DataAnnotations;

namespace HITS_API_1.Application.DTOs;

public record LoginRequest(
    [MinLength(1)]
    String password,
    
    [MinLength(1)]
    String email);