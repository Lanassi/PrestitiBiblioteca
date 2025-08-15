#!/bin/bash

# Script per configurare le credenziali in Jenkins
# Eseguire dopo aver avviato Jenkins

echo "=== Configurazione Credenziali Jenkins ==="

# URL di Jenkins (modifica se necessario)
JENKINS_URL="http://localhost:8080"
JENKINS_USER="admin"

echo "🔧 Avvio Jenkins container..."
docker-compose up -d

echo "⏳ Attendo che Jenkins sia disponibile..."