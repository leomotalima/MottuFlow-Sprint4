
<div align="center">
  <img src="https://github.com/thejaobiell/MottuFlowJava/blob/main/MottuFlow/src/main/resources/static/images/logo.png?raw=true" alt="MottuFlow" width="200"/>
  <h1>𝙈𝙤𝙩𝙩𝙪𝙁𝙡𝙤𝙬</h1>
</div>

<div align="center">
  <img src="https://github.com/leomotalima/MottuFlow/raw/main/Assets/logo.png" alt="MottuFlow" width="200"/>
  <h1>𝙈𝙤𝙩𝙩𝙪𝙁𝙡𝙤𝙬</h1>
</div>

![.NET](https://img.shields.io/badge/.NET-8-blue.svg)
![ASP.NET Core](https://img.shields.io/badge/ASP.NET%20Core-8.0-green.svg)
![Oracle](https://img.shields.io/badge/Oracle-19c-red.svg)
![xUnit](https://img.shields.io/badge/xUnit-2.5-orange.svg)

---

## 👥 Integrantes

* João Gabriel Boaventura RM557854 - 2TDSB2025
* Léo Mota Lima RM557851 - 2TDSB2025
* Lucas Leal das Chagas RM551124 - 2TDSB2025

---

## 📌 Justificativa da Arquitetura

O MottuFlow .NET foi desenvolvido com **arquitetura em camadas**, separando responsabilidades para maior manutenção e escalabilidade:

* **Controller:** Recebe requisições HTTP e retorna respostas.
* **Service:** Contém regras de negócio e processamento de dados.
* **Repository:** Gerencia o acesso ao banco de dados.

O uso de **DTOs** garante segurança, padronização e separação entre dados de entrada e saída.

---

## 📌 Justificativa do Domínio

As entidades foram definidas com base na operação da Mottu, startup de locação e logística de motos:

* **Funcionário:** Responsável por organizar e monitorar motos no pátio.
* **Moto:** Recurso principal utilizado em entregas e locação.
* **Pátio:** Local físico de armazenamento e organização das motos.

Essa modelagem garante rastreabilidade, controle da frota e operação eficiente da empresa.

---

## 🛠 Tecnologias

* **Backend:** ASP.NET Core 8
* **Banco de Dados:** Oracle 19c
* **Testes:** xUnit
* **Controle de Versão:** GitHub

---

## 📂 Estrutura do Projeto

```
MottuFlow/
│
├─ Controllers/        # Endpoints da API
├─ Models/             # Entidades e DTOs
├─ Repositories/       # Acesso a dados
├─ Services/           # Regras de negócio
├─ appsettings.json    # Configurações do projeto
├─ Tests/              # Projetos de teste (xUnit)
└─ Program.cs          # Configuração da aplicação
```

---

## 🚀 Execução da API

1. Clone o repositório:

```bash
git clone https://github.com/leomotalima/MottuFlow.git
```

2. Acesse a pasta do projeto:

```bash
cd MottuFlow
```

3. Restaure pacotes e execute:

```bash
dotnet restore
dotnet run
```

4. Acesse a API no navegador ou Postman:

```
http://localhost:5224
```

5. Para explorar os endpoints com Swagger (OpenAPI):

```
http://localhost:5224/swagger
```

---

## 🖼 Exemplos de Endpoints

### Funcionários

```
GET /api/funcionarios
POST /api/funcionarios
PUT /api/funcionarios/{id}
DELETE /api/funcionarios/{id}
```

### Pátios

```
GET /api/patios
POST /api/patios
PUT /api/patios/{id}
DELETE /api/patios/{id}
```

### Motos

```
GET /api/motos
POST /api/motos
PUT /api/motos/{id}
DELETE /api/motos/{id}
```

### Câmeras

```
GET /api/cameras
POST /api/cameras
PUT /api/cameras/{id}
DELETE /api/cameras/{id}
```

### ArUco Tags

```
GET /api/aruco-tags
POST /api/aruco-tags
PUT /api/aruco-tags/{id}
DELETE /api/aruco-tags/{id}
```

### Localidades

```
GET /api/localidades
POST /api/localidades
```

### Registro de Status

```
GET /api/registro-status
POST /api/registro-status
```

---

## 🧪 Testes Unitários

Para executar todos os testes com **xUnit**:

```bash
dotnet test
```


Para executar todos os testes com **xUnit**:

```bash
dotnet test
```
