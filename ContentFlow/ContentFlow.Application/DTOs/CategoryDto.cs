﻿namespace ContentFlow.Application.DTOs;

public record CategoryDto(
    int Id,
    string Name,
    string Slug,
    string Description);