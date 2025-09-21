# MottuFlow API

<div align="center">
  <img src="https://github.com/leomotalima/MottuFlow/blob/main/Assets/logo.png?raw=true" alt="MottuFlow" width="200"/>
  <h1>MottuFlow API</h1>
  <p>API RESTful para gerenciamento de funcionários, motos e pátios, com boas práticas REST, HATEOAS e Swagger.</p>
</div>

---

## Integrantes
- João Pedro Cancian Corrêa - RM: 555341  
- Léo Mota Lima - RM: 557851  
- Gustavo Paz Felipe - RM: 555277  
- Nicolas Gabriel Santos - RM: 554464  

---

## Justificativa da Arquitetura
O projeto segue a **arquitetura em camadas**, garantindo separação de responsabilidades:

- **Controllers** → recebem requisições HTTP e devolvem respostas REST.  
- **Services** → contêm a lógica de negócio.  
- **Repositories** → comunicação com o banco de dados (InMemory ou persistente).  
- **Models/DTOs** → definem os objetos de entrada e saída da API.  

Essa arquitetura facilita **testes, manutenção e escalabilidade**, permitindo evoluir a API sem impactar outras camadas.

---

## Justificativa do Domínio
O domínio foi definido com base na operação da Mottu, startup de locação e logística de motos:

- **Funcionário** → organiza e monitora as motos.  
- **Moto** → recurso principal, utilizado em entregas e locação.  
- **Pátio** → local físico de armazenamento e organização das motos.  

Essa modelagem reflete os processos centrais da Mottu: gestão de funcionários, organização de pátios e controle da frota de motos.

---

## Instruções de Execução

1. Clone o repositório:
```bash
git clone https://github.com/leomotalima/MottuFlow.git
```

2. Acesse a pasta do projeto:
```bash
cd MottuFlow
```

3. Abra no **Visual Studio 2022+** ou **VS Code**.

4. Restaure os pacotes NuGet:
```bash
dotnet restore
```

5. Execute a API:
```bash
dotnet run
```

6. Acesse o **Swagger UI** para explorar os endpoints:
```
http://localhost:5000/swagger
```
ou conforme a porta exibida no console.

---

## Exemplos de Endpoints

### 1️⃣ Moto

**Listar motos com paginação**
```
GET /api/motos?page=1&size=10
```
Resposta:
```json
{
  "page": 1,
  "size": 10,
  "totalCount": 35,
  "data": [
    {
      "id": 1,
      "placa": "ABC-1234",
      "marca": "Honda",
      "modelo": "CG 160 Titan",
      "links": [
        { "rel": "self", "href": "/api/motos/1" },
        { "rel": "update", "href": "/api/motos/1" },
        { "rel": "delete", "href": "/api/motos/1" }
      ]
    }
  ]
}
```

**Criar uma moto**
```
POST /api/motos
Content-Type: application/json

{
  "placa": "XYZ-5678",
  "marca": "Yamaha",
  "modelo": "Factor 150"
}
```
Resposta:
```
201 Created
Location: /api/motos/2
```

**Atualizar uma moto**
```
PUT /api/motos/2
Content-Type: application/json

{
  "id": 2,
  "placa": "XYZ-5678",
  "marca": "Yamaha",
  "modelo": "Factor 150 ED"
}
```

**Excluir uma moto**
```
DELETE /api/motos/2
```
Resposta:
```
204 No Content
```

---

### 2️⃣ Funcionário

**Listar funcionários**
```
GET /api/funcionarios?page=1&size=10
```

**Criar um funcionário**
```
POST /api/funcionarios
Content-Type: application/json

{
  "nome": "Léo Mota Lima",
  "cargo": "Operador de Pátio",
  "telefone": "11 99999-8888"
}
```

**Atualizar um funcionário**
```
PUT /api/funcionarios/1
Content-Type: application/json

{
  "id": 1,
  "nome": "Léo Mota Lima",
  "cargo": "Supervisor de Pátio",
  "telefone": "11 99999-8888"
}
```

**Excluir um funcionário**
```
DELETE /api/funcionarios/1
```
Resposta:
```
204 No Content
```

---

### 3️⃣ Pátio

**Listar pátios**
```
GET /api/patios?page=1&size=10
```

**Criar um pátio**
```
POST /api/patios
Content-Type: application/json

{
  "nome": "Pátio Central",
  "endereco": "Rua A, 123"
}
```

**Atualizar um pátio**
```
PUT /api/patios/1
Content-Type: application/json

{
  "id": 1,
  "nome": "Pátio Central",
  "endereco": "Rua A, 456"
}
```

**Excluir um pátio**
```
DELETE /api/patios/1
```
Resposta:
```
204 No Content
```

---

> ⚠️ Observação: Todos os endpoints possuem **HATEOAS**, **paginação** e retornam **status codes corretos**. Para testar e explorar, use o **Swagger UI**.

