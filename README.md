<div align="center">
  <img src="https://github.com/thejaobiell/MottuFlowJava/blob/main/MottuFlow/src/main/resources/static/images/logo.png?raw=true" alt="MottuFlow Logo" width="200"/>
  
  <h1><i><b>MottuFlow</b></i> - Sprint 4</h1>
  <p><b>Disciplina:</b> Advanced Business Development with .NET</p>
  <p><b>Professor Orientador:</b> Leonardo Gasparini Romão</p>
  <p>API RESTful desenvolvida em <b>.NET 8</b> para o gerenciamento inteligente de frotas de motocicletas da empresa <b>Mottu</b>.</p>
</div>

[![.NET](https://img.shields.io/badge/.NET-8.0-blue.svg)](https://dotnet.microsoft.com/)
[![C#](https://img.shields.io/badge/C%23-ASP.NET_Core-green.svg)](https://learn.microsoft.com/aspnet/core)
[![Entity Framework](https://img.shields.io/badge/ORM-Entity%20Framework%20Core-purple.svg)](https://learn.microsoft.com/ef/)
[![License: MIT](https://img.shields.io/badge/License-MIT-green.svg)](https://choosealicense.com/licenses/mit/)
[![FIAP](https://img.shields.io/badge/FIAP-2TDSB-red.svg)](https://www.fiap.com.br/)

---

## Visão Geral

O **MottuFlow** é uma **API RESTful** desenvolvida para otimizar o **gerenciamento inteligente de frotas de motocicletas** da empresa **Mottu**.  
A solução oferece controle completo sobre **funcionários, pátios, motos, câmeras, ArUco Tags, registros de status e localidades**, proporcionando **eficiência operacional**, **automação de processos** e **monitoramento centralizado** com boas práticas REST e integração com **Oracle**.

A aplicação implementa **autenticação JWT**, **HATEOAS**, **Health Checks**, **versionamento de API**, e testes automatizados com **xUnit** e **WebApplicationFactory**.

---

## Arquitetura do Sistema

O sistema segue arquitetura em camadas (**Controller → Service → Repository → Data → Model**), garantindo modularidade e manutenibilidade.

### 1. Container Diagram

```mermaid
graph TB
    subgraph MottuFlow["Software System: MottuFlow"]
        api["API ASP.NET Core Web API"]
        service["Services (Lógica de Negócio)"]
        repo["Repositories (Acesso a Dados)"]
        db[("Database (Oracle / InMemory)")]

        swagger["Swagger UI (OpenAPI)"]
        health["Health Checks"]
        ml["ML.NET Engine (Previsão de Manutenção)"]
    end

    user["Front-End Web/Mobile"]
    idp["Provedor de Identidade JWT"]

    user --> api
    api --> service
    service --> repo
    repo --> db
    api --> swagger
    api --> health
    service --> ml
    api --> idp
```

---

### 3. Component Diagram

```mermaid
graph LR
    ctrl["MotoController – Endpoints REST"]
    svc["MotoService – Regras de Negócio"]
    repo["MotoRepository – Acesso a Dados"]
    mapper["MotoMapper – DTO ⇄ Entidade"]
    validator["MotoValidator – Validação de Dados"]
    db[(Banco de Dados Oracle / InMemory)]

    ctrl --> svc
    svc --> repo
    svc --> mapper
    svc --> validator
    repo --> db
```

---

## Funcionalidades Principais

- CRUD completo para Funcionário, Pátio, Moto, etc.  
- Autenticação JWT e proteção de endpoints  
- HATEOAS em todas as respostas  
- Versionamento de API (v1, v2)  
- Health Check (`/api/health/ping`)  
- Swagger/OpenAPI documentado com anotações  
- Integração ML.NET (classificação de status de motos)  
- Testes automatizados com xUnit

---

## Tecnologias Utilizadas

| Tecnologia | Descrição |
|-------------|------------|
| **.NET 8 / ASP.NET Core** | Framework principal da API |
| **Entity Framework Core** | ORM para Oracle e InMemory |
| **Swagger / Swashbuckle** | Documentação interativa da API |
| **JWT Bearer** | Autenticação e segurança |
| **ML.NET** | Previsão de manutenção |
| **xUnit** | Testes de unidade e integração |
| **HATEOAS** | Navegação via links semânticos |
| **Oracle / InMemory** | Suporte a múltiplos bancos de dados |

---

## Pré-requisitos

Antes de executar o projeto, certifique-se de ter instalado:

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [Oracle Database](https://www.oracle.com/database/technologies/oracle-database-software-downloads.html) (ou Docker com [oracle-free](https://hub.docker.com/r/gvenzl/oracle-free))

---

## Execução Local

### 1️⃣ Clonar o repositório

```bash
git clone https://github.com/leomotalima/MottuFlow-Sprint4.git
cd MottuFlow-Sprint4
```

---

### 2️⃣ Configurar as credenciais do banco de dados

Crie um arquivo `.env` na raiz do projeto e configure as credenciais do Oracle:

```env
ORACLE_USER_ID=Joao
ORACLE_PASSWORD=112305
ORACLE_DATA_SOURCE=localhost:1521/FREEPDB1
ConnectionStrings__OracleConnection=User Id=${ORACLE_USER_ID};Password=${ORACLE_PASSWORD};Data Source=${ORACLE_DATA_SOURCE}
```

> **⚠️ IMPORTANTE:** Altere os valores de `ORACLE_USER_ID`, `ORACLE_PASSWORD` e `ORACLE_DATA_SOURCE` conforme seu ambiente Oracle local.

---

### 3️⃣ Instalar ferramentas e dependências

Execute os seguintes comandos no terminal:

```bash
# Instalar Entity Framework CLI globalmente
dotnet tool install --global dotnet-ef

# Restaurar pacotes NuGet
dotnet restore

# Compilar o projeto
dotnet build

# Aplicar migrations no banco de dados
dotnet ef database update
```

---

### 4️⃣ Executar a aplicação

```bash
dotnet run
```

A API estará disponível em: **[http://localhost:5224/swagger/index.html](http://localhost:5224/swagger/index.html)**

---

### 5️⃣ Configurar a versão da API no Swagger

No Swagger UI, selecione a versão da API:

- **Campo "API Version":** Digite `1` (versão v1)

> **⚠️ IMPORTANTE:** Todos os endpoints devem usar a **versão v1**. No Swagger, basta inserir o número **1** no campo de versionamento.

---

### 6️⃣ Autenticar na API

Antes de utilizar os endpoints protegidos, faça login no endpoint de autenticação:

**Endpoint:**
```http
POST /api/auth/login
```

**Corpo da requisição:**
```json
{
  "username": "admin@mottu.com",
  "password": "adminmottu"
}
```

**Resposta:**
```json
{
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiJhZG1pbkBtb3R0dS5jb20iLCJodHRwOi8vc2NoZW1hcy5taWNyb3NvZnQuY29tL3dzLzIwMDgvMDYvaWRlbnRpdHkvY2xhaW1zL3JvbGUiOiJBZG1pbmlzdHJhZG9yIiwianRpIjoiNmM3MDE4ZDEtNmU4MC00YTAyLTk4MjgtMTg2Yzg0ZmI4ODUzIiwiZXhwIjoxNzYxODM1NDkwLCJpc3MiOiJNb3R0dUZsb3ciLCJhdWQiOiJNb3R0dUZsb3dBcGlVc2VycyJ9.1kQV4c9rf2TzEaHHnn1PJ5F_0xjtTCEzgdkHjj6SIS0",
  "role": "Administrador",
  "expiresIn": "2h"
}
```

> **💡 Recomendação:** Utilize o Swagger UI para testar os endpoints. Clique no botão **"Authorize"** e cole o **token JWT** retornado (copie apenas o valor do campo `token`).

---

## Estrutura do Projeto

```
MottuFlow-Sprint4/
├── Controllers/           # Endpoints da API
├── Data/                 # DbContext e configurações EF
├── DTOs/                 # Data Transfer Objects
├── Hateoas/              # Implementação HATEOAS
├── Models/               # Entidades do domínio
├── Repositories/         # Acesso a dados
├── Services/             # Lógica de negócio
├── Swagger/              # Configurações Swagger
├── MottuFlow.Tests/      # Testes automatizados
├── Program.cs            # Ponto de entrada da aplicação
├── .env                  # Variáveis de ambiente (criar manualmente)
└── README.md
```

---

## Documentação da API

### Health Check
```http
GET /api/health/ping
```
**Resposta:**
```json
{
  "success": true,
  "message": "API rodando com sucesso 🚀",
  "data": {
    "status": "Healthy",
    "version": "1.0.0",
    "uptime": "00:00:00",
    "environment": "Development",
    "host": "<nome do host>",
    "timestampUtc": "2025-10-30T12:50:01.5172445Z"
  },
  "statusCode": 200,
  "timestampUtc": "2025-10-30T12:50:01.5173065Z"
}
```

---

### Funcionários

| Método | Endpoint | Descrição |
|--------|-----------|-----------|
| `GET` | `/api/funcionarios` | Lista todos os funcionários |
| `GET` | `/api/funcionarios/{id}` | Retorna um funcionário específico |
| `POST` | `/api/funcionarios` | Cria um novo funcionário |
| `PUT` | `/api/funcionarios/{id}` | Atualiza dados de um funcionário |
| `DELETE` | `/api/funcionarios/{id}` | Remove um funcionário |

**Exemplo de resposta com HATEOAS:**
```json
{
  "success": true,
  "message": "Funcionários listados com sucesso.",
  "data": {
    "meta": {
      "totalItems": 1,
      "page": 1,
      "pageSize": 10,
      "totalPages": 1
    },
    "funcionarios": [
      {
        "nome": "Admin",
        "cpf": "000.000.000-00",
        "cargo": "Administrador",
        "telefone": "(00)00000-0000",
        "email": "admin@mottu.com",
        "dataCadastro": "2025-10-29T12:48:25",
        "id": 1,
        "links": []
      }
    ]
  },
  "statusCode": 200,
  "timestampUtc": "2025-10-30T12:48:25.2077607Z"
}
```

---

## Testes Automatizados

![Tests](https://img.shields.io/badge/Testes%20de%20Integração-100%25%20Aprovados-brightgreen.svg)
![Build](https://img.shields.io/badge/Build-Sucesso-blue.svg)

- Banco InMemory criado e inicializado corretamente  
- Endpoints retornando status HTTP esperados (200, 201, 404 etc.)  
- Separação entre ambientes Oracle (produção) e InMemory (testes)  
- Testes de autenticação JWT validados com sucesso  

**Executando os testes manualmente:**
```bash
dotnet clean
dotnet build
dotnet test
```
> Todos os testes rodam com banco **InMemory**, sem necessidade do Oracle local.


---

## Equipe de Desenvolvimento

<table align="center">
<tr>
<td align="center">
<a href="https://github.com/thejaobiell">
<img src="https://github.com/thejaobiell.png" width="100px;" alt="João Gabriel"/><br>
<sub><b>João Gabriel Boaventura</b></sub><br>
<sub>RM554874 • 2TDSB2025</sub><br>
</a>
</td>
<td align="center">
<a href="https://github.com/leomotalima">
<img src="https://github.com/leomotalima.png" width="100px;" alt="Léo Mota"/><br>
<sub><b>Léo Mota Lima</b></sub><br>
<sub>RM557851 • 2TDSB2025</sub><br>
</a>
</td>
<td align="center">
<a href="https://github.com/LucasLDC">
<img src="https://github.com/LucasLDC.png" width="100px;" alt="Lucas Leal"/><br>
<sub><b>Lucas Leal das Chagas</b></sub><br>
<sub>RM551124 • 2TDSB2025</sub><br>
</a>
</td>
</tr>
</table>

---

## Licença

Distribuído sob a licença **MIT**.  
Consulte [LICENSE](https://choosealicense.com/licenses/mit/).
