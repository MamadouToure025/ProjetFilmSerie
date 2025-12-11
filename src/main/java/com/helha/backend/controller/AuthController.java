package com.helha.backend.controller;

import com.helha.backend.application.service.AuthService;
import com.helha.backend.domain.model.User;
import com.helha.backend.infrastructure.persistence.AuthResponse;
import com.helha.backend.infrastructure.persistence.UserResponse;
import com.helha.backend.util.JwtUtil;
import jakarta.servlet.http.Cookie;
import jakarta.servlet.http.HttpServletResponse;
import lombok.RequiredArgsConstructor;
import org.springframework.http.ResponseEntity;
import org.springframework.web.bind.annotation.*;

import java.util.Map;

@RestController
@RequestMapping("/api/auth")
@RequiredArgsConstructor
public class AuthController {

    private final AuthService authService;
    private final JwtUtil jwtUtil;

    record RegisterRequest(String username, String email, String password) {}
    record LoginRequest(String email, String password) {}

    @PostMapping("/register")
    public ResponseEntity<?> register(@RequestBody RegisterRequest req) {
        var userOpt = authService.register(req.username(), req.email(), req.password());
        if (userOpt.isEmpty()) {
            return ResponseEntity.badRequest().body(Map.of("error", "Email déjà utilisé"));
        }
        return ResponseEntity.ok(generateTokenResponse(userOpt.get()));
    }

    @PostMapping("/login")
    public ResponseEntity<?> login(@RequestBody LoginRequest req, HttpServletResponse response) {
        var userOpt = authService.login(req.email(), req.password());
        if (userOpt.isEmpty()) {
            return ResponseEntity.status(401).body(Map.of("error", "Identifiants incorrects"));
        }

        var user = userOpt.get();
        String token = jwtUtil.generateToken(user);

        // Set JWT as an HttpOnly cookie
        Cookie cookie = new Cookie("jwt", token);
        cookie.setHttpOnly(true); // Prevents JS from accessing it
        cookie.setSecure(true);   // Only send over HTTPS in production
        cookie.setPath("/");      // Cookie accessible for the entire domain
        cookie.setMaxAge(24 * 60 * 60); // 1 day expiration
        response.addCookie(cookie);

        // Also return a DTO without the password
        UserResponse safeUser = new UserResponse(
                user.getId(),
                user.getUsername(),
                user.getEmail(),
                user.getRole()
        );

        return ResponseEntity.ok(new AuthResponse(token, safeUser));
    }

    private AuthResponse generateTokenResponse(User user) {
        String token = jwtUtil.generateToken(user);

        UserResponse safeUser = new UserResponse(
                user.getId(),
                user.getUsername(),
                user.getEmail(),
                user.getRole()
        );

        return new AuthResponse(token, safeUser);
    }
}
