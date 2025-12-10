# **Prova Prática \- CatalogHub API**

### **📌 Descrição do Projeto**

O ***CatalogHub API*** é uma API RESTful desenvolvida em .NET 8, projetada como uma solução robusta e escalável para o gerenciamento de um catálogo de produtos e categorias. O projeto demonstra as melhores práticas de desenvolvimento de software, incluindo a implementação de uma Arquitetura Limpa (Clean Architecture), persistência de dados com Entity Framework Core, integração com a AWS para armazenamento de arquivos e uma suíte de testes unitários para garantir a qualidade do código.  
A API proporciona funcionalidades completas para o ciclo de vida dos produtos, incluindo operações de CRUD, filtros e upload de imagens.

### **🚀 Objetivos do Projeto**

* Implementar uma **Arquitetura Limpa** desacoplada, com clara separação entre as camadas de Domínio, Aplicação, Infraestrutura e Apresentação.  
* Aplicar práticas recomendadas para uso de **Entity Framework Core**, incluindo mapeamento com Fluent API.  
* Garantir a qualidade e a confiabilidade do código através de **testes unitários** (xUnit e Moq).  
* Integrar com serviços externos de forma desacoplada, como a **AWS S3** para armazenamento de imagens.  
* Fornecer uma documentação de API clara e interativa com **Swagger/OpenAPI**, seguindo as convenções REST.  
* Apresentar um código limpo, organizado e facilmente extensível.

### **🛠️ Tecnologias Utilizadas**

**Backend:**

* .NET 8  
* ASP.NET Core  
* Entity Framework Core 8

**Banco de Dados:**

* PostgreSQL

**Testes:**

* xUnit  
* Moq

**Infraestrutura e Cloud:**

* AWS S3 (SDK para .NET)
* Docker

**Documentação:**

* Swagger (Swashbuckle)

### **✨ Arquitetura do Projeto**

O projeto segue a abordagem de **Clean Architecture**, que promove a separação de responsabilidades e garante que a lógica de negócio seja independente de detalhes de implementação.

  ```mermaid

graph TD
    subgraph MainLayers
        direction TB
        A1[Cliente HTTP] --> B1[API Layer - Apresentacao]
        B1 --> C1[Application Layer]
        C1 --> D1[Domain Layer]
        E1[Infrastructure Layer] --> D1
        C1 -- usa --> F1[Interfaces de Repositorio]
        E1 -- implementa --> F1
    end

    subgraph Legenda
        direction LR
        B1 --- B2[Controllers, DI, Configs]
        C1 --- C2[Services, Validators]
        D1 --- D2[Entities, Interfaces de Repositorio]
        E1 --- E2[DbContext, Repositories, Migrations]
    end

    subgraph Dependencias Externas
        direction TB
        E1 --> G1[PostgreSQL]
        E1 --> H1[AWS S3]
    end

```


### **📁 Estrutura de Pastas**

/  
├── 📄 CatalogHub.sln  
│  
├── 🚀 CatalogHub.Api/  
│   ├── Configuration/ 
│   ├── Controllers/  
│   └── Program.cs  
│  
├── ⚙️ CatalogHub.Application/  
│   ├── DTOs/  
│   ├── Services/  
│   ├── Interfaces/  
│   └── Validators/  
│  
├── 💾 CatalogHub.Infrastructure/  
│   ├── Data/  
│   ├── Repository/  
│   └── Migrations/  
│  
├── 🧱 CatalogHub.Domain/  
│   ├── Models/  
│   └── Interfaces/  
│  
└── 🧪 CatalogHub.Tests/  
    └── Service/

### **📌 Como Rodar o Projeto**

Siga os passos abaixo para configurar e executar a aplicação em seu ambiente local.

#### **✅ Pré-requisitos**

* [Docker Desktop](https://www.docker.com/products/docker-desktop): Essencial para executar a aplicação de forma simples. É crucial que esteja rodando antes de executar os comandos.
* [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0) <- Caso use o Docker, não é necessário instalar o SDK localmente. 
* [Ambiente de Desenvolvimento da sua preferência](https://visualstudio.microsoft.com/pt-br/downloads/) <- Recomendado Visual Studio

#### **🛠️ Configuração**

#### 🐳 Como Rodar o Projeto com Docker (Recomendado)

Esta é a forma mais simples e garantida de executar o projeto, pois o ambiente inteiro (API + Base de Dados) é gerido pelo Docker.

1.  Clone o repositório:

        git clone: https://github.com/alysonsz/CatalogHub.git
        OBS: abra o projeto através do arquivo CatalogHub.sln

2.  Configure os Segredos:

        Na raiz do projeto, crie um novo arquivo chamado docker-compose.override.yml.

        Copie e cole o conteúdo abaixo neste novo arquivo, substituindo com as suas credenciais reais da AWS:

        version: '3.8'

        services:
          api:
            environment:
              # As suas credenciais reais da AWS ficam aqui
              - AWS__BucketName=cataloghub-db
              - AWS__AWSAccessKey=SUA_ACCESS_KEY_AQUI
              - AWS__AWSSecretKey=SUA_SECRET_KEY_AQUI
              - AWS__Region=us-east-2

        Este arquivo é ignorado pelo Git e fornece as suas credenciais de forma segura para o contentor da API.

3.  Inicie os Contentores:

        Abra um terminal na raiz do projeto.

        Execute o comando:

        docker-compose up --build

4.  Acesse a API:    

        Na primeira vez que executar, o Docker irá construir a imagem da API, o que pode demorar alguns minutos. Nas próximas vezes, será muito mais rápido.
        A própria API irá aplicar as migrations automaticamente ao iniciar, criando as tabelas na base de dados.

        Após os logs estabilizarem, a aplicação estará rodando em: http://localhost:8000.
        A documentação interativa do Swagger estará disponível em:
        http://localhost:8000/swagger

#### 🔧 Rodando o Projeto Manualmente (Alternativa):

       Base de Dados: Garanta que tem uma instância do PostgreSQL a correr localmente.

       Pré-requisitos: É necessário ter o .NET 8 SDK instalado.

       Configure as variáveis de ambiente: No arquivo appsettings.json, ajuste a DefaultConnection e as credenciais da AWS.

       Aplique as Migrations: dotnet ef database update --project CatalogHub.Infrastructure --startup-project CatalogHub.Api

       Inicie a API: dotnet run --project CatalogHub.Api

       Acesse a Documentação da API: http://localhost:5222/swagger (a porta pode variar).

#### 🧪 Executando os Testes Unitários

       Abra um terminal na raiz da solução e execute o comando:

       dotnet test