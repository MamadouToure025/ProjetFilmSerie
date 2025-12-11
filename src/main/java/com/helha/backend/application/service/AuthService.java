package com.helha.backend.application.service;

import com.helha.backend.domain.model.User;
import com.helha.backend.infrastructure.persistence.UserEntity;
import com.helha.backend.infrastructure.repository.UserRepository;
import lombok.RequiredArgsConstructor;
import org.springframework.security.crypto.bcrypt.BCryptPasswordEncoder;
import org.springframework.stereotype.Service;

import java.util.Optional;

@Service
@RequiredArgsConstructor
public class AuthService {

    private final UserRepository userRepository;
    private final BCryptPasswordEncoder passwordEncoder = new BCryptPasswordEncoder();

    public Optional<User> register(String username, String email, String plainPassword) {
        if (userRepository.findByEmail(email).isPresent()) {
            return Optional.empty();
        }

        UserEntity userEntity = new UserEntity();
        userEntity.setUsername(username);
        userEntity.setEmail(email);
        userEntity.setPassword(passwordEncoder.encode(plainPassword));
        userEntity.setRole("USER");

        UserEntity saved = userRepository.save(userEntity);

        User user = new User(saved.getId(), saved.getUsername(), saved.getEmail(), saved.getPassword(), saved.getRole());

        return Optional.of(user);
    }


    public Optional<User> login(String email, String plainPassword) {
        Optional<UserEntity> entityOpt = userRepository.findByEmail(email);

        if (entityOpt.isPresent()) {
            UserEntity entity = entityOpt.get();
            if (passwordEncoder.matches(plainPassword, entity.getPassword())) {
                return Optional.of(toDomain(entity));
            }
        }

        return Optional.empty();
    }

    private User toDomain(UserEntity entity) {
        return new User(
                entity.getId(),
                entity.getUsername(),
                entity.getEmail(),
                entity.getPassword(),
                entity.getRole()
        );
    }
}
