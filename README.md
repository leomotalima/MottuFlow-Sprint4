
<div align="center">
  <img src="https://github.com/thejaobiell/MottuFlowJava/blob/main/MottuFlow/src/main/resources/static/images/logo.png?raw=true" alt="MottuFlow" width="200"/>
  <h1>𝙈𝙤𝙩𝙩𝙪𝙁𝙡𝙤𝙬</h1>
</div>

![Java](https://img.shields.io/badge/Java-21-orange.svg)
![Spring Boot](https://img.shields.io/badge/Spring%20Boot-3.x-brightgreen.svg)
![MySQL](https://img.shields.io/badge/MySQL-8.0-blue.svg)
![Thymeleaf](https://img.shields.io/badge/Thymeleaf-3.x-lightgreen.svg)

---

## 👥 Integrantes

- João Gabriel Boaventura RM557854 - 2TDSB2025  
- Léo Mota Lima RM557851 - 2TDSB2025  
- Lucas Leal das Chagas RM551124 - 2TDSB2025  

---

## 📌 Justificativa da Arquitetura

O MottuFlow .NET foi desenvolvido com **arquitetura em camadas**, separando responsabilidades para maior manutenção e escalabilidade:  

- **Controller:** Recebe requisições e retorna respostas.  
- **Service:** Contém regras de negócio e processamento de dados.  
- **Repository:** Gerencia o acesso ao banco de dados.  

O uso de **DTOs** garante segurança, padronização e separação entre dados de entrada e saída.  

O modelo de dados permite gerenciar frotas de motos, incluindo **Funcionários, Pátios, Motos, Câmeras, ArUco Tags, Localidades e Registro de Status**.

---

## 🛠 Tecnologias

- **Backend:** ASP.NET Core 8  
- **Banco de Dados:** Oracle  
- **Testes:** xUnit  
- **Controle de Versão:** GitHub  

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

---

## 🖼 Exemplos de Endpoints

### Funcionários

```
GET /api/funcionarios
POST /api/funcionarios
PUT /api/funcionarios/{id}
DELETE /api/funcionarios/{id}
```

![Funcionários](https://github.com/leomotalima/MottuFlow/blob/main/Screenshots/FuncionarioEndpoint.png?raw=true)

### Pátios

```
GET /api/patios
POST /api/patios
PUT /api/patios/{id}
DELETE /api/patios/{id}
```

![Pátios](https://github.com/leomotalima/MottuFlow/blob/main/Screenshots/PatioEndpoint.png?raw=true)

### Motos

```
GET /api/motos
POST /api/motos
PUT /api/motos/{id}
DELETE /api/motos/{id}
```

![Motos](https://github.com/leomotalima/MottuFlow/blob/main/Screenshots/MotoEndpoint.png?raw=true)

### Câmeras

```
GET /api/cameras
POST /api/cameras
PUT /api/cameras/{id}
DELETE /api/cameras/{id}
```

![Câmeras](https://github.com/leomotalima/MottuFlow/blob/main/Screenshots/CameraEndpoint.png?raw=true)

### ArUco Tags

```
GET /api/aruco-tags
POST /api/aruco-tags
PUT /api/aruco-tags/{id}
DELETE /api/aruco-tags/{id}
```

![ArUco Tags](https://github.com/leomotalima/MottuFlow/blob/main/Screenshots/ArucoEndpoint.png?raw=true)

### Localidades

```
GET /api/localidades
POST /api/localidades
```

![Localidades](https://github.com/leomotalima/MottuFlow/blob/main/Screenshots/LocalidadeEndpoint.png?raw=true)

### Registro de Status

```
GET /api/registro-status
POST /api/registro-status
```

![Registro de Status](https://github.com/leomotalima/MottuFlow/blob/main/Screenshots/RegistroStatusEndpoint.png?raw=true)

---

## 🧪 Testes Unitários

Para executar todos os testes com **xUnit**:

```bash
dotnet test
```
