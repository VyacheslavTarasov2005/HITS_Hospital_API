using System.ComponentModel.DataAnnotations;

namespace HITS_API_1.Application.DTOs;

public record LoginRequest(
    String password,
    String email);