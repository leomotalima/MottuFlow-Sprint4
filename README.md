<div align="center">
  <img src="https://github.com/thejaobiell/MottuFlowJava/blob/main/MottuFlow/src/main/resources/static/images/logo.png?raw=true" alt="MottuFlow" width="200"/>
  <h1>𝙈𝙤𝙩𝙩𝙪𝙁𝙡𝙤𝙬</h1>
</div>

![Java](https://img.shields.io/badge/Java-21-orange.svg)
![Spring Boot](https://img.shields.io/badge/Spring%20Boot-3.x-brightgreen.svg)
![MySQL](https://img.shields.io/badge/MySQL-8.0-blue.svg)
![Thymeleaf](https://img.shields.io/badge/Thymeleaf-3.x-lightgreen.svg)



## 🚀 Sobre o Projeto

**MottuFlow** é uma solução completa para gerenciamento de frotas de motocicletas, desenvolvida com arquitetura híbrida que combina **API REST** moderna com interface web intuitiva. O sistema utiliza **visão computacional** e **ArUco Tags** para identificação automática de veículos, oferecendo controle total sobre funcionários, pátios, motos, câmeras e localização de ativos.

--- 

O sistema integra uma **API REST moderna** com interface web e utiliza **visão computacional** para automação na identificação dos veículos, facilitando o controle de:

- Funcionários  
- Pátios  
- Motos  
- Câmeras
- ArUco Tags
- Status e Localização em tempo real

---

## 👥 Integrantes

| Nome | RM | Turma |
|------|----|-------|
| João Gabriel Boaventura | 557854 | 2TDSB2025 |
| Léo Mota Lima | 557851 | 2TDSB2025 |
| Lucas Leal das Chagas | 551124 | 2TDSB2025 |

---

## 📌 Justificativa da Arquitetura

O MottuFlow .NET segue **arquitetura em camadas**, separando responsabilidades para facilitar **manutenção**, **escalabilidade** e **testes unitários**:

| Camada | Função |
|--------|--------|
| **Controller** | Recebe requisições HTTP e retorna respostas |
| **Service** | Contém regras de negócio e processamento de dados |
| **Repository** | Gerencia o acesso ao banco de dados |
| **Data/DbContext** | Conecta e gerencia operações no banco de dados |

**DTOs (Data Transfer Objects)** são usados para padronizar dados entre camadas, garantindo que apenas informações necessárias sejam expostas ou recebidas pela API.

---

## 📌 Justificativa do Domínio

As entidades refletem a operação da Mottu, startup especializada em locação e logística de motos:

| Entidade | Função |
|----------|-------|
| **Funcionário** | Organiza e monitora motos no pátio |
| **Moto** | Principal recurso para entregas e locação |
| **Pátio** | Local físico para armazenamento e organização das motos |

---

## 🛠 Tecnologias

- **Backend:** ASP.NET Core 8  
- **Banco de Dados:** Oracle 19c  
- **Controle de Versão:** GitHub  

---

## 🏢 Módulos Principais

| Módulo | Descrição | Funcionalidades |
|--------|-----------|----------------|
| **👥 Funcionários** | Gestão de pessoas | CRUD, controle de acessos, histórico |
| **🏪 Pátios** | Gerenciamento de locais | Cadastro, monitoramento e capacidade |
| **🏍️ Motos** | Controle da frota | Registro, status, localização e manutenção |
| **📹 Câmeras** | Monitoramento visual | Configuração e status das câmeras |
| **🏷️ ArUco Tags** | Identificação automática | Cadastro e rastreamento via visão computacional |
| **📍 Status & Localização** | Rastreamento em tempo real | Monitoramento de posição, disponibilidade e alertas |

---

## 📂 Estrutura do Projeto

```
MottuFlow/
├── .idea/                       # Configurações do IDE (opcional)
├── Controllers/                 # Endpoints da API
├── DTOs/                        # Data Transfer Objects
├── Data/                        # DbContext e configuração do banco
├── Hateoas/                      # Classes HATEOAS
├── Helpers/                      # Serviços auxiliares, interfaces e utilitários
├── Migrations/                   # Scripts de migração do banco de dados
├── Models/                       # Entidades e classes de domínio
├── Properties/                   # Propriedades do projeto (AssemblyInfo)
├── Repositories/                 # Acesso a dados
├── Services/                     # Regras de negócio
├── .gitignore                     # Ignorar arquivos do Git
├── AppDbContextFactory.cs         # Factory para contexto do banco
├── MottuFlow.csproj               # Projeto .NET
├── MottuFlow.http                 # Coleção HTTP para testes
├── Program.cs                     # Configuração e inicialização da aplicação
├── README.md                      # Documentação do projeto
├── appsettings.Development.json   # Configurações de desenvolvimento
├── appsettings.json               # Configurações gerais
└── post.txt                       # Arquivo auxiliar/teste
```

---

## 🚀 Execução da API

1. **Clone o repositório:**

```bash
git clone https://github.com/leomotalima/MottuFlow.git
cd MottuFlow
```

2. **Restaure pacotes e execute:**

```bash
dotnet restore
dotnet run
```

3. **Acesse a API:**

- Navegador ou Postman: [http://localhost:5224](http://localhost:5224)  
- Swagger (OpenAPI): [http://localhost:5224/swagger](http://localhost:5224/swagger)

---

## 🖼 Exemplos de Endpoints

### Funcionários
```bash
GET /api/funcionarios
POST /api/funcionarios
PUT /api/funcionarios/{id}
DELETE /api/funcionarios/{id}
```

### Pátios
```bash
GET /api/patios
POST /api/patios
PUT /api/patios/{id}
DELETE /api/patios/{id}
```

### Motos
```bash
GET /api/motos
POST /api/motos
PUT /api/motos/{id}
DELETE /api/motos/{id}
```

### Câmeras
```bash
GET /api/cameras
POST /api/cameras
PUT /api/cameras/{id}
DELETE /api/cameras/{id}
```

### ArUco Tags
```bash
GET /api/aruco-tags
POST /api/aruco-tags
PUT /api/aruco-tags/{id}
DELETE /api/aruco-tags/{id}
```

### Localidades
```bash
GET /api/localidades
POST /api/localidades
```

### Registro de Status
```bash
GET /api/registro-status
POST /api/registro-status
```

