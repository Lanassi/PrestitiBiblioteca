pipeline { 
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
        
        // Clona il repository contenente il Jenkinsfile (e presumibilmente il Dockerfile)
        stage('Clonazione Repository Jenkinsfile') {
            steps {
                dir("CI-CD-PrestitiBiblioteca") {
                    script {
                        echo "Clonazione del repository Jenkinsfile..."
                        dir("CI-CD-PrestitiBiblioteca") {
                            git(
                                url: "${JINKINS_DOCKER_FILE}",
                                branch: "${BRANCH}",
                                credentialsId: "github-token"
                            )
                            echo " Fine clonazione del Jenkinsfile."
                        }
                    }
                }
            }
        }
        
        // Clona il repository del progetto .NET da buildare
        stage('📥 Checkout del Codice Sorgente') {
            steps {
                dir("PrestitiBiblioteca") {
                    script {
                        echo "Clonazione del repository del codice sorgente .NET..."
                        git(
                            url: "${CODE_REPO}",
                            branch: "${BRANCH}",
                            credentialsId: "github"
                        )
                        echo "Fine clonazione del codice sorgente .NET..."
                    }
                }
            }
        }


        // Ripristina le dipendenze del progetto .NET usando `dotnet restore`
        /*stage('Restore delle Dipendenze') {
            steps {
                dir("MyProgetto1") {
                    echo "Inizio Restore Dependencies"
                    script {
                        try {
                            sh 'export PATH=${DOTNET_ROOT}:$PATH && ${DOTNET_ROOT} restore'
                        } catch (Exception e) {
                            error "Errore nel restore delle dipendenze: ${e.message}"
                        }
                    }
                    echo "Fine Restore Dependencies"
                }
            }
        }*/
        
        
        stage('Debug Environment') {
          steps {
            sh '''
              echo "Shell: $SHELL"
              echo "PATH: $PATH"
              which sh
              which bash
            '''
          }
        }

        /*stage('📥 Checkout') {
            steps {
                echo '=== Checkout del codice dal repository ==='
                // Jenkins fa automaticamente il checkout se il Jenkinsfile è nel repo
                checkout scm
                
                // Mostra informazioni sul commit corrente
                sh '''
                    echo "Commit corrente: $(git rev-parse HEAD)"
                    echo "Branch: $(git branch --show-current)"
                    echo "Ultimo commit: $(git log -1 --pretty=format:'%h - %s (%an, %ar)')"
                '''
            }
        }*/
        
        stage('🔧 Restore Dependencies') {
            steps {
                echo '=== Ripristino delle dipendenze NuGet ==='
                sh '''
                    # Ripristina tutti i pacchetti NuGet necessari
                    dotnet restore --verbosity normal
                    
                    # Mostra informazioni sul progetto
                    dotnet --version
                    echo "Progetto: $(find . -name '*.csproj' | head -1)"
                '''
            }
        }
        
        stage('🏗️ Build') {
            steps {
                echo '=== Compilazione del progetto ==='
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
        
        stage('🧪 Test') {
            steps {
                echo '=== Esecuzione dei test ==='
                sh '''
                    # Esegue tutti i test del progetto
                    dotnet test --configuration Release --no-build --verbosity normal --logger trx --results-directory ./TestResults/
                    
                    # Verifica se esistono file di test
                    if find . -name "*Test*.csproj" -o -name "*Tests*.csproj" | grep -q .; then
                        echo "✅ Test trovati ed eseguiti"
                    else
                        echo "⚠️ Nessun progetto di test trovato, saltando i test"
                    fi
                '''
            }
            post {
                always {
                    // Pubblica i risultati dei test se esistono
                    script {
                        if (fileExists('TestResults/*.trx')) {
                            publishTestResults testResultsPattern: 'TestResults/*.trx'
                        }
                    }
                }
            }
        }

        // Pubblica l’output della build in una directory per il packaging
        stage('Publish') {
            steps {
                echo "Inizio Publish"
                
                sh "dotnet publish -c Release -f net9.0 -o publish"
                
                echo "Fine Publish"
            }
        }

        // Verifica che Docker sia attivo e funzionante eseguendo `docker ps`
        stage('Verifica Docker') {
            steps {
                echo "Inizio Test Docker!"
                //sh 'docker -H unix:///Users/lansanacamara/.colima/default/docker.sock ps'
                sh 'docker ps'
                echo "Fine Test Docker!"
            }
        }


        // Pulisce l’intero workspace Jenkins alla fine della pipeline
        stage('Cleanup') {
            steps {
                cleanWs()
            }
        }
    }
    // token:   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
         
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