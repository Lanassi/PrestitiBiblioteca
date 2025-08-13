
vpipeline { 
    agent any

   // Variabili di ambiente globali
    environment {

        JINKINS_DOCKER_FILE = 'https://github.com/Lanassi/CI-CD-PrestitiBiblioteca.git'
        CODE_REPO = 'https://github.com/Lanassi/PrestitiBiblioteca.git'  // URL del repository del codice .NET
        BRANCH = 'main'  // Branch da buildare

        // Nome del progetto e immagine Docker
        APP_NAME = 'prestiti-biblioteca'
        DOCKER_IMAGE = "${DOCKER_HUB_USERNAME}/${APP_NAME}"
        // Versione basata sul numero di build
        DOCKER_TAG = "${env.BUILD_NUMBER ?: 'latest'}"
        
        // Recupera credenziali da Jenkins
        DOCKER_HUB_CREDENTIALS = credentials('dockerhub-credentials')  // ID delle credenziali DockerHub
        KUBECONFIG_CREDENTIALS = credentials('kubeconfig-file')        // File kubeconfig per Kubernetes
        
        // Connection string per il database (da Jenkins secrets)
        DB_CONNECTION_STRING = credentials('db-connection-string')
    }
    
    // Strumenti necessari
    tools {
        // Specifica la versione di .NET se configurata in Jenkins
        dotnetsdk 'dotnet-9.0'
    }
    
    stages {
        echo "Iniza tutto!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!"
        // Clona il repository contenente il Jenkinsfile e Dockerfile
        stage('Clonazione Repository CI/CD') {
            steps {
                script {
                    echo "Clonazione del repository CI/CD..."
                    dir("ci-cd-PrestitiBiblioteca") {
                        git(
                            url: "${JINKINS_DOCKER_FILE}",
                            branch: "${BRANCH}",
                            credentialsId: "github-token"
                        )
                    }
                    echo "Fine clonazione del repository CI/CD."
                }
            }
        }


        
        // Clona il repository del progetto .NET da buildare
        stage('📥 Checkout del Codice Sorgente') {
            steps {
                script {
                    echo "Clonazione del repository del codice sorgente .NET..."
                    dir("ci-cd-PrestitiBiblioteca") {
                        git(
                            url: "${CODE_REPO}",
                            branch: "${BRANCH}",
                            credentialsId: "github-token"
                        )
                    }
                    echo "Fine clonazione del codice sorgente .NET..."
                }
            }
        }

        stage('Debug Environment') {
            steps {
                sh '''
                    echo "Shell: $SHELL"
                    echo "PATH: $PATH"
                    echo "Working Directory: $(pwd)"
                    ls -la
                    which dotnet || echo "dotnet non trovato nel PATH"
                '''
            }
        }
        
        echo "Finisce tutto!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!"
        stage('🔧 Restore Dependencies') {
            steps {
                echo '=== Ripristino delle dipendenze NuGet ==='
                dir("dotnet-project") {
                    sh '''
                        # Verifica la presenza di dotnet
                        dotnet --version
                        
                        # Mostra i file del progetto
                        find . -name "*.csproj" -o -name "*.sln"
                        
                        # Ripristina tutti i pacchetti NuGet necessari
                        dotnet restore --verbosity normal
                    '''
                }
            }
        }
        echo "Finisce tutto!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!"
        stage('🏗️ Build') {
            steps {
                echo '=== Compilazione del progetto ==='
                dir("dotnet-project") {
                    sh '''
                        # Compila il progetto in modalità Release
                        dotnet build --configuration Release --no-restore --verbosity normal
                        
                        # Verifica che la build sia riuscita
                        if [ $? -eq 0 ]; then
                            echo "✅ Build completata con successo"
                        else
                            echo "❌ Build fallita"
                            exit 1
                        fi
                    '''
                }
            }
        }
        
        stage('🧪 Test') {
            steps {
                echo '=== Esecuzione dei test ==='
                dir("dotnet-project") {
                    sh '''
                        # Verifica se esistono file di test
                        if find . -name "*Test*.csproj" -o -name "*Tests*.csproj" | grep -q .; then
                            echo "✅ Test trovati, esecuzione in corso..."
                            # Esegue tutti i test del progetto
                            dotnet test --configuration Release --no-build --verbosity normal --logger trx --results-directory ./TestResults/
                        else
                            echo "⚠️ Nessun progetto di test trovato, saltando i test"
                        fi
                    '''
                }
            }
            post {
                always {
                    // Pubblica i risultati dei test se esistono
                    script {
                        if (fileExists('dotnet-project/TestResults/*.trx')) {
                            publishTestResults testResultsPattern: 'dotnet-project/TestResults/*.trx'
                        }
                    }
                }
            }
        }

        // Pubblica l'output della build
        stage('📦 Publish') {
            steps {
                echo "=== Pubblicazione dell'applicazione ==="
                dir("dotnet-project") {
                    sh '''
                        # Trova il file di progetto principale
                        PROJECT_FILE=$(find . -name "*.csproj" | head -1)
                        echo "Pubblicazione progetto: $PROJECT_FILE"
                        
                        # Pubblica l'applicazione
                        dotnet publish "$PROJECT_FILE" -c Release -f net9.0 -o ../publish
                        
                        # Verifica i file pubblicati
                        ls -la ../publish/
                    '''
                }
            }
        }

        // Verifica Docker
        stage('🐳 Verifica Docker') {
            steps {
                echo "=== Verifica Docker ==="
                sh '''
                    docker --version
                    docker ps
                    echo "✅ Docker funzionante"
                '''
            }
        }
        
        // Costruzione immagine Docker (opzionale)
        stage('🔨 Build Docker Image') {
            when {
                // Esegui solo se esiste un Dockerfile
                expression {
                    return fileExists('ci-cd-files/Dockerfile') || fileExists('dotnet-project/Dockerfile')
                }
            }
            steps {
                script {
                    echo "=== Costruzione immagine Docker ==="
                    def dockerfilePath = fileExists('ci-cd-files/Dockerfile') ? 'ci-cd-files/Dockerfile' : 'dotnet-project/Dockerfile'
                    
                    sh """
                        # Copia i file pubblicati se necessario
                        if [ -f "ci-cd-files/Dockerfile" ]; then
                            cp -r publish ci-cd-files/
                            cd ci-cd-files
                        else
                            cd dotnet-project
                        fi
                        
                        # Costruisci l'immagine
                        docker build -t prestiti-biblioteca:latest .
                        docker images | grep prestiti-biblioteca
                    """
                }
            }
        }
    
        
    }
    

         
    // Blocchi post (esecuzione dopo il build):
    post {
        always {
            echo "Pulizia finale dello spazio di lavoro"
            cleanWs()  // Pulisce tutta la workspace alla fine del build
        }
        success {
            // Messaggio di successo visibile nella console Jenkins
            echo '✅ Deploy completato con successo!'
        }
        failure {
            // Messaggio di errore in caso di problemi
            echo '❌ Errore nel deploy!'
        }
    }
}




