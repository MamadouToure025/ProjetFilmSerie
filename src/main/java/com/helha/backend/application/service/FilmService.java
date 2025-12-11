package com.helha.backend.application.service;

import com.helha.backend.domain.model.Film;
import com.helha.backend.infrastructure.repository.FilmRepository;
import lombok.RequiredArgsConstructor;
import org.springframework.stereotype.Service;

import java.util.List;

@Service
@RequiredArgsConstructor
public class FilmService {
    private final FilmRepository filmRepository;
}