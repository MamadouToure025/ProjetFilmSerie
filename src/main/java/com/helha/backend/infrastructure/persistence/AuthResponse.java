package com.helha.backend.infrastructure.persistence;

public record AuthResponse(String token, UserResponse user) {}
