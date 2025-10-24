<div align="center">
  <img src="https://github.com/thejaobiell/MottuFlowJava/blob/main/MottuFlow/src/main/resources/static/images/logo.png?raw=true" alt="MottuFlow Logo" width="180"/>
  
  <h1><i><b>MottuFlow</b></i> - Sprint 4 (FIAP)</h1>
  <p><b>Disciplina:</b> Advanced Business Development with .NET</p>
  <p><b>Professor Orientador:</b> Leonardo Gasparini Romão </p>
  <p>API RESTful desenvolvida em .NET 8 para o gerenciamento inteligente de frotas de motocicletas da empresa <b>Mottu</b>.</p>
</div>

---

## 🏷️ Etiquetas
[![.NET](https://img.shields.io/badge/.NET-8.0-blue.svg)](https://dotnet.microsoft.com/)
[![C#](https://img.shields.io/badge/C%23-ASP.NET_Core-green.svg)](https://learn.microsoft.com/aspnet/core)
[![Entity Framework](https://img.shields.io/badge/ORM-Entity%20Framework%20Core-purple.svg)](https://learn.microsoft.com/ef/)
[![License: MIT](https://img.shields.io/badge/License-MIT-green.svg)](https://choosealicense.com/licenses/mit/)
[![FIAP](https://img.shields.io/badge/FIAP-2TDSB-red.svg)](https://www.fiap.com.br/)

---

## 🎯 Visão Geral

O **MottuFlow** é uma **API RESTful** desenvolvida para otimizar o **gerenciamento inteligente de frotas de motocicletas** da empresa **Mottu**.

A solução oferece controle completo sobre **funcionários, pátios, motos, câmeras, ArUco Tags, registros de status e localidades**,  
proporcionando **eficiência operacional**, **automação de processos** e **monitoramento centralizado** por meio de boas práticas REST e integração moderna com banco de dados.

O projeto aplica **boas práticas REST**, **HATEOAS**, **autenticação JWT**, **Health Checks**, **versionamento de API**, e inclui **testes unitários e de integração com xUnit**.

> 💡 Este projeto foi desenvolvido como parte da disciplina **Advanced Business Development with .NET** da **FIAP**, aplicando conceitos modernos de arquitetura, segurança e testes em APIs RESTful corporativas.

---

## 🧠 Arquitetura do Sistema

O projeto segue uma arquitetura em camadas (Controller → Service → Repository → Data → Model), garantindo modularidade e manutenibilidade.

### 🧩 Diagrama C4 (Alto Nível)

C4Context
    title Diagrama de Contexto - MottuFlow API

    Person(gerente, "Gerente Mottu", "Acessa via interface web para gerenciar motos e pátios.")
    Person(funcionario, "Funcionário Mottu", "Usa aplicativo mobile para atualizar status e localização.")

    System_Boundary(mottuflow, "MottuFlow API (.NET 8)") {
        Container(web, "Interface Web / Swagger UI", "ASP.NET Core", "Interface para visualizar e testar os endpoints.")
        Container(rest, "API RESTful", "ASP.NET Core Web API", "Gerencia entidades como Moto, Pátio, Funcionário e Localidade.")
        ContainerDb(db, "Banco de Dados Oracle", "Oracle 19c / EF Core", "Armazena as informações das operações e cadastros.")
        Container(ml, "Módulo de Machine Learning", "ML.NET", "Prediz necessidade de manutenção de motos.")
        Container(jwt, "Serviço de Autenticação", "JWT Service", "Gera e valida tokens de autenticação.")
    }

    Rel(gerente, web, "Gerencia frotas e funcionários")
    Rel(funcionario, rest, "Atualiza status e localização das motos")
    Rel(rest, db, "CRUD completo via Entity Framework")
    Rel(rest, jwt, "Valida tokens de autenticação JWT")
    Rel(rest, ml, "Predição de manutenção preventiva")


## ⚙️ Funcionalidades Principais

- ✅ CRUD completo para todas as entidades (Funcionário, Pátio, Moto, etc.)
- 🔗 **HATEOAS** integrado em todas as respostas
- 🔒 **Autenticação via JWT Token**
- ❤️ **Health Check Endpoint**
- 🧩 **Versionamento de API** (v1, v2)
- 📊 **Swagger/OpenAPI** com descrições detalhadas
- 🧠 **Integração ML.NET** (classificação de status de motos)
- 🧪 **Testes com xUnit e WebApplicationFactory**

---

## 🧰 Tecnologias Utilizadas

- **.NET 8 / ASP.NET Core Web API**
- **Entity Framework Core**
- **Swagger / Swashbuckle**
- **ML.NET**
- **xUnit**
- **HATEOAS**
- **JWT Authentication**
- **Oracle / InMemory Database (EF Core)**

---

## 🧩 Documentação da API

### 🔹 Health Check
```http
GET /api/health/ping
```
**Resposta:**
```json
{
  "status": "API rodando 🚀"
}
```

---

### 🔹 Funcionários

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
  "id": 1,
  "nome": "João Silva",
  "cpf": "123.456.789-00",
  "links": [
    { "rel": "self", "href": "/api/funcionarios/1", "method": "GET" },
    { "rel": "update", "href": "/api/funcionarios/1", "method": "PUT" },
    { "rel": "delete", "href": "/api/funcionarios/1", "method": "DELETE" }
  ]
}
```

---

## 🧪 Testes Automatizados

### 🚦 Status dos Testes

![Tests](https://img.shields.io/badge/Testes%20de%20Integração-100%25%20Aprovados-brightgreen.svg)
![Build](https://img.shields.io/badge/Build-Sucesso-blue.svg)

Os testes foram executados com **xUnit** e **WebApplicationFactory**, garantindo:
- ✅ Banco InMemory criado e inicializado corretamente;
- ✅ Endpoints retornando status HTTP esperado (200 OK, 201 Created, etc.);
- ✅ Separação entre ambientes **Oracle (produção)** e **InMemory (testes)**;
- ✅ Integração contínua sem dependência de infraestrutura externa.

---

### 🔍 Executando os testes manualmente

```bash
dotnet clean
dotnet build
dotnet test
```

> 💡 Dica: todos os testes estão configurados para rodar com **banco InMemory**, não exigindo Oracle.

---

## 🏗️ Estrutura do Projeto

```
MottuFlow-Sprint4/
├── .idea/
├── bin/
├── obj/
├── Controllers/
├── Data/
├── DTOs/
├── Hateoas/
├── Migrations/
├── Models/
├── MottuFlow.Tests/
├── Properties/
├── Repositories/
├── Services/
├── static/
├── Swagger/
├── AppDbContextFactory.cs
├── appsettings.json
├── appsettings.Development.json
├── global.json
├── MottuFlow.csproj
├── MottuFlow.http
├── MottuFlow.sln
├── Program.cs
└── README.md

```
> Estrutura modular e testável — separando **camadas de domínio, infraestrutura e testes de integração**.

---

## 💻 Execução Local

### Clonar o projeto
```bash
git clone https://github.com/leomotalima/MottuFlow-Sprint4.git
```

### Entrar no diretório
```bash
cd MottuFlow-Sprint4
```

### Restaurar dependências
```bash
dotnet restore
```

### Rodar a aplicação
```bash
dotnet run
```

Acesse: http://localhost:5224

---

## ⚙️ Configuração do Banco de Dados

O projeto suporta **dois tipos de banco**: **InMemory (EF Core)** e **Oracle Database**.  

### 1️⃣ InMemory Database (para testes e desenvolvimento)
- Não requer configuração adicional.  
- Ideal para testes rápidos e desenvolvimento local.  
- Para usar InMemory, configure no `appsettings.json`:

```json
{
  "UseInMemoryDatabase": true
}
```

### 2️⃣ Oracle Database (recomendado para produção)
- Configure `UseInMemoryDatabase` como `false` e adicione a string de conexão no `appsettings.json` ou via **variáveis de ambiente**:

```json
{
  "UseInMemoryDatabase": false,
  "ConnectionStrings": {
    "OracleDb": "User Id=SEU_USUARIO;Password=SUA_SENHA;Data Source=(DESCRIPTION=(ADDRESS=(PROTOCOL=TCP)(HOST=SEU_HOST)(PORT=1521))(CONNECT_DATA=(SERVICE_NAME=SEU_SERVICO)))"
  }
}
```

- Certifique-se de que o **banco Oracle esteja rodando**.  
- Caso existam **migrations**, execute:

```bash
dotnet ef database update
```

### 🔹 Alternando via Variáveis de Ambiente

Você pode sobrescrever `UseInMemoryDatabase` sem alterar o `appsettings.json`:

- **Windows (PowerShell):**
```powershell
$env:UseInMemoryDatabase="false"
dotnet run
```

- **Linux / MacOS (bash/zsh):**
```bash
export UseInMemoryDatabase=false
dotnet run
```

---

## 🧠 Aprendizados

Durante o desenvolvimento, foram aplicadas práticas avançadas de:
- Arquitetura em camadas e injeção de dependência;
- Versionamento e documentação de APIs;
- Segurança com JWT e boas práticas REST;
- Testes automatizados e integração contínua.

---

## 📜 Licença

Distribuído sob a licença **MIT**.  
Veja [LICENSE](https://choosealicense.com/licenses/mit/) para mais detalhes.

---

## 👥 Autores

| Nome | RM | Responsabilidade |
|------|----|------------------|
| **Léo Mota Lima** | 557851 | API REST, Controllers, DTOs, Swagger, HATEOAS, Testes |
| **João Gabriel Boaventura** | 557854 | Lógica de negócio e integração ML.NET |
| **Lucas Leal das Chagas** | 551124 | Documentação, banco de dados e versionamento |

---

## 🔗 Referências

- [Microsoft Docs – ASP.NET Core Web API](https://learn.microsoft.com/aspnet/core/)
- [Awesome README Templates](https://awesomeopensource.com/project/elangosundar/awesome-README-templates)
- [Swagger Documentation Best Practices](https://swagger.io/resources/articles/best-practices-in-api-documentation/)
- [Mermaid C4 Diagrams](https://mermaid.js.org/syntax/c4.html)
