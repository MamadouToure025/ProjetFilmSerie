package com.helha.backend.infrastructure.persistence;

public record UserResponse(Long id, String username, String email, String role) {}
