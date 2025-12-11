package com.helha.backend.application.service;

import com.helha.backend.domain.model.Serie;
import com.helha.backend.infrastructure.repository.SerieRepository;
import lombok.RequiredArgsConstructor;
import org.springframework.stereotype.Service;

import java.util.List;

@Service
@RequiredArgsConstructor
public class SerieService {
    private final SerieRepository serieRepository;
}