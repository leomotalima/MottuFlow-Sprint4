<div align="center">
  <img src="https://github.com/thejaobiell/MottuFlowJava/blob/main/MottuFlow/src/main/resources/static/images/logo.png?raw=true" alt="MottuFlow" width="200"/>
  
  <h1><i><b>MottuFlow</b></i> - Sprint 4 (FIAP)</h1>
  <p><b>Disciplina:</b> Advanced Business Development with .NET</p>
  <p><b>Professor Orientador:</b> Leonardo Gasparini Romão </p>
  <p>API RESTful desenvolvida em .NET 8 para o gerenciamento inteligente de frotas de motocicletas da empresa <b>Mottu</b>.</p>
</div>

---

## Etiquetas
[![.NET](https://img.shields.io/badge/.NET-8.0-blue.svg)](https://dotnet.microsoft.com/)
[![C#](https://img.shields.io/badge/C%23-ASP.NET_Core-green.svg)](https://learn.microsoft.com/aspnet/core)
[![Entity Framework](https://img.shields.io/badge/ORM-Entity%20Framework%20Core-purple.svg)](https://learn.microsoft.com/ef/)
[![License: MIT](https://img.shields.io/badge/License-MIT-green.svg)](https://choosealicense.com/licenses/mit/)
[![FIAP](https://img.shields.io/badge/FIAP-2TDSB-red.svg)](https://www.fiap.com.br/)

---

## Visão Geral

O **MottuFlow** é uma **API RESTful** desenvolvida para otimizar o **gerenciamento inteligente de frotas de motocicletas** da empresa **Mottu**.

A solução oferece controle completo sobre **funcionários, pátios, motos, câmeras, ArUco Tags, registros de status e localidades**,  
proporcionando **eficiência operacional**, **automação de processos** e **monitoramento centralizado** por meio de boas práticas REST e integração moderna com banco de dados.

O projeto aplica **boas práticas REST**, **HATEOAS**, **autenticação JWT**, **Health Checks**, **versionamento de API**, e inclui **testes unitários e de integração com xUnit**.

> Este projeto foi desenvolvido como parte da disciplina **Advanced Business Development with .NET** da **FIAP**, aplicando conceitos modernos de arquitetura, segurança e testes em APIs RESTful corporativas.

---

## Arquitetura do Sistema

O projeto segue uma arquitetura em camadas (Controller → Service → Repository → Data → Model), garantindo modularidade e manutenibilidade.

---

### 1.System Context (Visão de Contexto)

```mermaid
graph TB
    user["Person: Usuário (Funcionário/Gerente)"]

    extPay["External System: Sistema de Pagamentos"]
    extIdP["External System: Provedor de Identidade (JWT)"]

    subgraph s1["Software System: MottuFlow"]
        api["API REST .NET 8"]
    end

    user -->|HTTP/JSON| api
    api -->|JWT Authentication| extIdP
    api -->|Financial Integration| extPay

```

> Mostra o relacionamento entre o usuário e os sistemas externos que interagem com o MottuFlow.

---

### 2.Container (Visão de Contêineres)

```mermaid
graph TB
    subgraph MottuFlow ["Software System: MottuFlow"]
        api["Container: API .NET 8 (ASP.NET Core)"]
        service["Container: Services (Lógica de Negócio)"]
        repo["Container: Repositories (Acesso a Dados)"]
        db[("Container: Database (Oracle / InMemory)")]
        swagger["Container: Swagger UI (OpenAPI)"]
        health["Container: Health Checks"]
        ml["Container: ML.NET Engine (Previsão de Manutenção)"]
    end

    user["Container Externo: Front-End Web/Mobile"]
    idp["Container Externo: Provedor de Identidade JWT"]

    user -->|HTTP/JSON| api
    api --> service
    service --> repo
    repo -->|EF Core| db
    api --> swagger
    api --> health
    service --> ml
    api -->|Autenticação| idp
```

> Representa os principais contêineres internos e suas relações de comunicação dentro do sistema MottuFlow.

---

### 3.Component (Visão de Componentes da API - Exemplo Domínio “Moto”)

```mermaid
graph LR
    ctrl["Component: MotoController - Endpoints REST"]
    svc["Component: MotoService - Regras de Negócio"]
    repo["Component: MotoRepository - Acesso a Dados"]
    mapper["Component: MotoMapper - Conversão DTO ⇄ Entidade"]
    validator["Component: MotoValidator - Validação de Dados"]
    db[(Container: Banco de Dados Oracle / InMemory)]

    ctrl -->|Chama| svc
    svc -->|Usa| repo
    svc -->|Usa| mapper
    svc -->|Usa| validator
    repo -->|CRUD / Queries| db
```

> Mostra os principais componentes internos do container da API para o domínio de **Moto**.

---

## Funcionalidades Principais

- CRUD completo para todas as entidades (Funcionário, Pátio, Moto, etc.)
- **HATEOAS** integrado em todas as respostas
- **Autenticação via JWT Token**
- **Health Check Endpoint**
- **Versionamento de API** (v1, v2)
- **Swagger/OpenAPI** com descrições detalhadas
- **Integração ML.NET** (classificação de status de motos)
- **Testes com xUnit e WebApplicationFactory**

---

## Tecnologias Utilizadas

- **.NET 8 / ASP.NET Core Web API**
- **Entity Framework Core**
- **Swagger / Swashbuckle**
- **ML.NET**
- **xUnit**
- **HATEOAS**
- **JWT Authentication**
- **Oracle / InMemory Database (EF Core)**

---

## Documentação da API

### Health Check
```http
GET /api/health/ping
```
**Resposta:**
```json
{
  "status": "API rodando"
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

## Testes Automatizados

![Tests](https://img.shields.io/badge/Testes%20de%20Integração-100%25%20Aprovados-brightgreen.svg)
![Build](https://img.shields.io/badge/Build-Sucesso-blue.svg)

Os testes foram executados com **xUnit** e **WebApplicationFactory**, garantindo:
- Banco InMemory criado e inicializado corretamente;
- Endpoints retornando status HTTP esperado (200 OK, 201 Created, etc.);
- Separação entre ambientes **Oracle (produção)** e **InMemory (testes)**;
- Integração contínua sem dependência de infraestrutura externa.

### Executando os testes manualmente

```bash
dotnet clean
dotnet build
dotnet test
```

> Dica: todos os testes estão configurados para rodar com **banco InMemory**, não exigindo Oracle.

---

## Estrutura do Projeto

```
MottuFlow-Sprint4/
├── Controllers/
├── Data/
├── DTOs/
├── Hateoas/
├── Migrations/
├── Models/
├── MottuFlow.Tests/
├── Repositories/
├── Services/
├── Swagger/
├── Program.cs
└── README.md
```

> Estrutura modular e testável — separando **camadas de domínio, infraestrutura e testes de integração**.

---

## Execução Local

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

Acesse: [http://localhost:5224/swagger](http://localhost:5224/swagger)

---

## Aprendizados

Durante o desenvolvimento, foram aplicadas práticas avançadas de:
- Arquitetura em camadas e injeção de dependência;
- Versionamento e documentação de APIs;
- Segurança com JWT e boas práticas REST;
- Testes automatizados e integração contínua.

---

## Licença

Distribuído sob a licença **MIT**.  
Veja [LICENSE](https://choosealicense.com/licenses/mit/) para mais detalhes.

---

## Autores

| Nome | RM | Responsabilidade |
|------|----|------------------|
| **Léo Mota Lima** | 557851 | API REST, Controllers, DTOs, Swagger, HATEOAS, Testes |
| **João Gabriel Boaventura** | 557854 | Lógica de negócio e integração ML.NET |
| **Lucas Leal das Chagas** | 551124 | Documentação, banco de dados e versionamento |

---

## Referências

- [Microsoft Docs – ASP.NET Core Web API](https://learn.microsoft.com/aspnet/core/)
- [C4 Model Official Website](https://c4model.com/diagrams)
- [Swagger Documentation Best Practices](https://swagger.io/resources/articles/best-practices-in-api-documentation/)
- [Mermaid C4 Diagrams](https://mermaid.js.org/syntax/c4.html)
