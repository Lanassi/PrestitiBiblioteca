pipeline { 
    agent any

    parameters {
        choice(
            name: 'BG',
            choices: ['blue', 'green'],
            description: 'Seleziona l\'ambiente di deploy'
        )
    }
    
    environment {
        JINKINS_DOCKER_FILE = 'https://github.com/Lanassi/ProgettoSpindox.git'
        CODE_REPO = 'https://github.com/Lanassi/MyProgetto1.git'  // URL del repository del codice .NET
        BRANCH = 'main'  // Branch da buildare

        //DOCKER_HOST = "unix:///Users/lansanacamara/.colima/default/docker.sock"
        DOCKER_IMAGE = "itslansana/progettospindox"
        DOCKER_TAG = "${env.BUILD_NUMBER ?: 'latest'}"

        DOTNET_ROOT = "/opt/homebrew/bin/dotnet"
        //PATH = "/opt/homebrew/bin:${env.PATH}"
        PATH = "/opt/homebrew/bin:/usr/bin:/bin:/usr/sbin:/sbin:${env.PATH}"
        
    }
    
    stages {
        
        // Clona il repository contenente il Jenkinsfile (e presumibilmente il Dockerfile)
        stage('Clonazione Repository Jenkinsfile') {
            steps {
                dir("MyProgetto1") {
                    script {
                        echo "Clonazione del repository Jenkinsfile..."
                        dir("MyProgetto1") {
                            git(
                                url: "${JINKINS_DOCKER_FILE}",
                                branch: "${BRANCH}",
                                credentialsId: "github"
                            )
                            echo " Fine clonazione del Jenkinsfile."
                        }
                    }
                }
            }
        }
        
        // Clona il repository del progetto .NET da buildare
        stage('Checkout del Codice Sorgente') {
            steps {
                dir("MyProgetto1") {
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

        // Pulisce l’intero workspace Jenkins alla fine della pipeline
        stage('Cleanup') {
            steps {
                cleanWs()
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