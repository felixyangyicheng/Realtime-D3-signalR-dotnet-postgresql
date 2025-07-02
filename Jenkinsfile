pipeline {
    agent any

    environment {
        COMPOSE_PROJECT_DIR = 'Realtime-D3-signalR-dotnet-postgresql'
    }

    stages {
        stage('拉取代码') {
            steps {
                echo "--------------- 拉取 Git 仓库代码 ---------------"
                checkout scm
            }
        }

        stage('获取 Git 提交哈希') {
            steps {
                script {
                    dir(env.COMPOSE_PROJECT_DIR) {
                        GITHASH = sh(
                            script: 'git rev-parse --short HEAD',
                            returnStdout: true
                        ).trim()
                        echo "Git 哈希: ${GITHASH}"
                    }
                }
            }
        }

        // [3] 新增：构建 Docker 镜像（不启动服务）
        stage('Build Images') {
            steps {
                echo "--------------- 构建 Docker 镜像 ---------------"
                sh 'docker compose build'
            }
        }

        // [4] 新增：运行测试（关键质量门禁）
        stage('Tests') {
            steps {
                echo "--------------- 运行单元测试 ---------------"
                sh 'docker compose run realtime_d3_api dotnet test'
                
                // 可选：添加测试报告处理
                post {
                    always {
                        junit '**/test-results.xml' // 如果测试生成 JUnit 报告
                    }
                }
            }
        }

        // [5] 新增：数据库迁移
        stage('Migrations') {
            steps {
                echo "--------------- 执行数据库迁移 ---------------"
                sh 'docker compose run realtime_d3_api dotnet ef database update'
            }
        }

        // [6] 启动服务
        stage('启动容器服务') {
            steps {
                echo "--------------- 启动服务 ---------------"
                sh 'docker compose down --remove-orphans || true'
                sh 'docker compose up -d --force-recreate'
            }
        }

        stage('清理旧镜像') {
            steps {
                echo "--------------- 清理未使用的 Docker 镜像 ---------------"
                sh 'docker image prune -f'
            }
        }
    }

    post {
        always {
            echo "--------------- 显示当前 Docker 镜像状态 ---------------"
            sh 'docker images'
            echo "--------------- 显示当前运行的容器 ---------------"
            sh 'docker ps -a'
        }
    }
}
