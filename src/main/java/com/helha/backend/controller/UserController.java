package com.helha.backend.controller;

import com.helha.backend.application.service.UserService;
import com.helha.backend.domain.model.User;
import com.helha.backend.util.JwtUtil;
import jakarta.servlet.http.HttpServletRequest;
import lombok.RequiredArgsConstructor;
import org.springframework.http.ResponseEntity;
import org.springframework.web.bind.annotation.*;

import java.util.List;

@RestController
@RequestMapping("/api/users")
@RequiredArgsConstructor
public class UserController {

    private final UserService userService;

    // Récupère l'ID de l'utilisateur connecté depuis le JWT (injecté par le filtre)
    private Long getCurrentUserId(HttpServletRequest request) {
        return (Long) request.getAttribute("userId");
    }

    private String getCurrentUserRole(HttpServletRequest request) {
        return (String) request.getAttribute("userRole");
    }

    // Liste tous les utilisateurs (ADMIN uniquement)
    @GetMapping
    public ResponseEntity<List<User>> getAllUsers(HttpServletRequest request) {
        if (!"ADMIN".equals(getCurrentUserRole(request))) {
            return ResponseEntity.status(403).build();
        }
        return ResponseEntity.ok(userService.getAllUsers());
    }

    // Détail d'un utilisateur
    @GetMapping("/{id}")
    public ResponseEntity<User> getUser(@PathVariable Long id, HttpServletRequest request) {
        if (!"ADMIN".equals(getCurrentUserRole(request))) {
            return ResponseEntity.status(403).build();
        }

        User user = userService.getUserById(id);
        return user != null ? ResponseEntity.ok(user) : ResponseEntity.notFound().build();
    }

    // Changer le rôle d'un utilisateur (ADMIN uniquement)
    @PutMapping("/{id}/role")
    public ResponseEntity<?> updateUserRole(
            @PathVariable Long id,
            @RequestBody UpdateRoleRequest request,
            HttpServletRequest httpRequest) {

        if (!"ADMIN".equals(getCurrentUserRole(httpRequest))) {
            return ResponseEntity.status(403).body("Accès refusé");
        }

        User user = userService.getUserById(id);
        if (user == null) {
            return ResponseEntity.notFound().build();
        }

        // Un admin ne peut pas se rétrograder lui-même
        if (id.equals(getCurrentUserId(httpRequest)) && !"ADMIN".equals(request.role())) {
            return ResponseEntity.badRequest()
                    .body("Un administrateur ne peut pas se retirer son propre rôle ADMIN");
        }

        user.setRole(request.role());
        User updated = userService.updateUser(user);
        return ResponseEntity.ok(updated);
    }

    // Supprimer un utilisateur (ADMIN uniquement)
    @DeleteMapping("/{id}")
    public ResponseEntity<Void> deleteUser(
            @PathVariable Long id,
            HttpServletRequest request) {

        if (!"ADMIN".equals(getCurrentUserRole(request))) {
            return ResponseEntity.status(403).build();
        }

        User user = userService.getUserById(id);
        if (user == null) {
            return ResponseEntity.notFound().build();
        }

        // Un admin ne peut pas se supprimer lui-même
        if (id.equals(getCurrentUserId(request))) {
            return ResponseEntity.badRequest()
                    .body(null);
        }

        userService.deleteUser(id);
        return ResponseEntity.noContent().build();
    }
}

// Record pour la requête de changement de rôle
record UpdateRoleRequest(String role) {}