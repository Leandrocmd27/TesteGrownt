# 🏢 Sistema de Gestão de Colaboradores e Departamentos

Sistema web desenvolvido em .NET 8 com Razor Pages para gerenciamento de colaboradores e departamentos com suporte a hierarquia organizacional.

## 📋 Sobre o Projeto

Este projeto foi desenvolvido como parte de um desafio técnico que implementa um sistema completo de gestão de colaboradores e departamentos, incluindo:

- ✅ CRUD completo de Colaboradores e Departamentos
- ✅ Hierarquia de departamentos (departamentos superiores e subordinados)
- ✅ Validações de integridade (CPF único, RG único, ciclos hierárquicos)
- ✅ Filtros avançados de busca
- ✅ Relacionamento entre colaboradores, departamentos e gerentes

## 🚀 Tecnologias Utilizadas

- **Back-end:** .NET 8
- **Front-end:** Razor Pages
- **Banco de Dados:** PostgreSQL 16
- **ORM:** Entity Framework Core
- **Containerização:** Docker & Docker Compose
- **Arquitetura:** Clean Architecture (Domain, Application, Infrastructure, Presentation)

## 📁 Estrutura do Projeto

```
TesteGrownt/
├── TesteGrownt.Domain/          # Entidades e regras de negócio
│   └── Entities/
│       ├── Colaborador.cs
│       └── Departamento.cs
├── TesteGrownt.Application/     # Lógica de aplicação e serviços
│   ├── Interfaces/
│   │   ├── IColaboradorService.cs
│   │   └── IDepartamentoService.cs
│   └── Services/
│       ├── ColaboradorService.cs
│       └── DepartamentoService.cs
├── TesteGrownt.Infrastructure/  # Acesso a dados e persistência
│   ├── Data/
│   │   └── AppDbContext.cs
│		└── Mappings/
│       └── ColaboradorMappings.cs
│		    └── CDepartamentoMappings.cs
└── TesteGrownt/                 # Camada de apresentação (Razor Pages)
    └── Pages/
        ├── Colaboradores/
        │   ├── Index.cshtml
        │   ├── Create.cshtml
        │   └── Edit.cshtml
        └── Departamentos/
            ├── Index.cshtml
            ├── Create.cshtml
            └── Edit.cshtml
```

## 🎯 Funcionalidades Implementadas

### 👥 Gestão de Colaboradores

- **Cadastro de Colaboradores**
  - Campos: Nome, CPF (único), RG (único, opcional), Departamento
  - Validações automáticas de duplicidade
  - Departamento obrigatório quando existem departamentos cadastrados

- **Listagem e Filtros**
  - Filtro por Nome
  - Filtro por CPF
  - Filtro por RG
  - Filtro por Departamento
  - Exibição do nome do gerente baseado no departamento

- **Edição de Colaboradores**
  - Atualização de dados com validações
  - Prevenção de CPF/RG duplicados

### 🏢 Gestão de Departamentos

- **Cadastro de Departamentos**
  - Campos: Nome, Gerente (obrigatório), Departamento Superior (opcional)
  - Validação de existência do gerente
  - Validação de departamento superior
  - Prevenção de auto-referência

- **Listagem e Filtros**
  - Filtro por Nome do Departamento
  - Filtro por Gerente
  - Filtro por Departamento Superior
  - Exibição da hierarquia organizacional

- **Edição de Departamentos**
  - Atualização com validações de hierarquia
  - Detecção e prevenção de ciclos hierárquicos
  - Exclusão automática do departamento atual na lista de superiores

### 🎖️ Funcionalidades Avançadas (Desafios)

1. **✅ Colaborador com Gerente**
   - Ao buscar um colaborador, o sistema retorna automaticamente o nome do gerente baseado no departamento

2. **✅ Árvore Hierárquica de Departamentos**
   - Método recursivo `ObterArvoreAsync` que retorna toda a estrutura de subdepartamentos
   - Carregamento lazy de subdepartamentos em múltiplos níveis

3. **✅ Colaboradores Subordinados ao Gerente**
   - Método `ObterColaboradoresDoGerenteAsync` que busca recursivamente todos os colaboradores de todos os departamentos subordinados a um gerente

## 🛠️ Pré-requisitos

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [Docker](https://www.docker.com/get-started)
- [Docker Compose](https://docs.docker.com/compose/install/)
- IDE recomendada: Visual Studio 2022 ou VS Code

## 🐳 Configuração do Banco de Dados (Docker)

O projeto utiliza PostgreSQL rodando em container Docker.

### docker-compose.yml

```yaml
version: '3.9'
services:
  postgres:
    image: postgres:16
    container_name: postgres_testegrownt
    environment:
      POSTGRES_DB: testegrownt
      POSTGRES_USER: postgres
      POSTGRES_PASSWORD: postgres
    ports:
      - "5432:5432"
    volumes:
      - postgres_data:/var/lib/postgresql/data
volumes:
  postgres_data:
```


## 🚀 Como Executar o Projeto

### 1️⃣ Clone o Repositório

```bash
git clone https://github.com/seu-usuario/TesteGrownt.git
cd TesteGrownt
```

### 2️⃣ Configure a String de Conexão

Verifique o arquivo `appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=testegrownt;Username=postgres;Password=postgres"
  }
}
```

### 3️⃣ Iniciar o Banco de Dados

```bash

Navegue até a pasta raiz do projeto (onde está o arquivo docker-compose.yml) e execute:

# Iniciar o container
docker-compose up -d

# Verificar se está rodando
docker ps

# Ver logs (se necessário)
docker-compose logs -f postgres

Nota: O comando deve ser executado no diretório que contém o arquivo docker-compose.yml.
```

### 4️⃣ Aplicar Migrations

```bash

dotnet ef database update --project TesteGrownt

# Adicionar migration inicial (apenas se necessário)
dotnet ef migrations add InitialCreate --project TesteGrownt.Infrastructure --startup-project TesteGrownt

```

**Estrutura de diretórios esperada:**
```
C:\Users\SeuUsuario\Desktop\TesteGrownt\     ← Execute os comandos aqui
├── docker-compose.yml                        ← Para docker-compose up -d
├── TesteGrownt.sln                          ← Para dotnet ef commands
└── TesteGrownt\
    ├── TesteGrownt.csproj
    ├── Program.cs
    └── appsettings.json
	


# Atualizar o banco de dados
dotnet ef database update --project TesteGrownt.Infrastructure --startup-project TesteGrownt

Importante:

O comando docker-compose up -d deve ser executado na pasta que contém docker-compose.yml
O comando dotnet ef database update deve ser executado na pasta que contém o arquivo .sln
Alguns avisos informativos podem aparecer durante a execução das migrations (como "Acquiring an exclusive lock"), mas isso é normal

```


### 5️⃣ Executar a Aplicação

```bash
# Navegar até o projeto web
cd TesteGrownt

# Executar
dotnet run
```

A aplicação estará disponível em: `https://localhost:7XXX` ou `http://localhost:5XXX`

## 📊 Modelo de Dados

### Entidade: Colaborador

```csharp
- Id (Guid) - Chave primária
- Nome (string) - Obrigatório, max 100 caracteres
- CPF (string) - Obrigatório, único
- RG (string) - Opcional, único se preenchido
- DepartamentoId (Guid) - Obrigatório quando existem departamentos
- Departamento (navegação) - Relacionamento com Departamento
```

### Entidade: Departamento

```csharp
- Id (Guid) - Chave primária
- Nome (string) - Obrigatório, max 100 caracteres
- GerenteId (Guid) - Obrigatório
- Gerente (navegação) - Relacionamento com Colaborador
- DepartamentoSuperiorId (Guid?) - Opcional
- DepartamentoSuperior (navegação) - Auto-relacionamento
- SubDepartamentos (coleção) - Departamentos subordinados
- Colaboradores (coleção) - Colaboradores do departamento
```

## 🔒 Validações Implementadas

### Colaboradores
- ✅ Nome obrigatório
- ✅ CPF obrigatório e único
- ✅ RG único (se informado)
- ✅ Departamento obrigatório quando existem departamentos cadastrados
- ✅ Validação de existência do departamento

### Departamentos
- ✅ Nome obrigatório (max 100 caracteres)
- ✅ Gerente obrigatório e deve existir
- ✅ Validação de existência do departamento superior
- ✅ Prevenção de auto-referência (departamento não pode ser superior a si mesmo)
- ✅ Detecção de ciclos na hierarquia (A → B → C → A)

## 🧪 Testando a Aplicação

### Fluxo Recomendado

1. **Criar Colaboradores**
   - Acesse `/Colaboradores/Create`
   - Crie pelo menos 2 colaboradores (que serão gerentes)

2. **Criar Departamento Raiz**
   - Acesse `/Departamentos/Create`
   - Crie um departamento sem superior (Ex: "Diretoria")
   - Selecione um gerente

3. **Criar Subdepartamentos**
   - Crie departamentos filhos (Ex: "TI", "RH", "Financeiro")
   - Associe ao departamento superior criado anteriormente

4. **Adicionar Colaboradores aos Departamentos**
   - Edite os colaboradores existentes
   - Associe-os aos departamentos criados

5. **Testar Filtros**
   - Use os filtros nas páginas de listagem
   - Verifique se o gerente aparece corretamente

## 📝 Comandos Úteis

```bash
# Parar o banco de dados
docker-compose down

# Parar e remover volumes (CUIDADO: apaga os dados)
docker-compose down -v

# Ver logs do PostgreSQL
docker-compose logs -f postgres

# Acessar o PostgreSQL via CLI
docker exec -it postgres_testegrownt psql -U postgres -d testegrownt

# Remover migration
dotnet ef migrations remove --project TesteGrownt.Infrastructure --startup-project TesteGrownt

# Criar nova migration
dotnet ef migrations add NomeDaMigration --project TesteGrownt.Infrastructure --startup-project TesteGrownt
```

## 🏗️ Arquitetura e Boas Práticas

- **Separation of Concerns:** Separação clara entre camadas (Domain, Application, Infrastructure, Presentation)
- **Dependency Injection:** Uso de injeção de dependência nativa do .NET
- **Repository Pattern:** Acesso a dados através de services
- **Eager Loading:** Uso de `Include` e `ThenInclude` para evitar N+1 queries
- **Validações:** Implementadas tanto no lado do servidor quanto com Data Annotations
- **Exception Handling:** Tratamento adequado de erros com mensagens amigáveis
- **Recursive Queries:** Implementação de métodos recursivos para hierarquias

## 🎨 Interface

- **Bootstrap 5:** Framework CSS para layout responsivo
- **Design Limpo:** Interface intuitiva e fácil de usar
- **Validações Client-Side:** Feedback imediato ao usuário
- **Mensagens de Feedback:** TempData para sucesso/erro

## 🐛 Troubleshooting

### Problema: Erro de conexão com o banco

```bash
# Verificar se o container está rodando
docker ps

# Reiniciar o container
docker-compose restart postgres
```

### Problema: Migration não aplicada

```bash
# Forçar update
dotnet ef database update --force --project TesteGrownt.Infrastructure --startup-project TesteGrownt
```

### Problema: Porta 5432 já em uso

```bash
# Parar outros serviços PostgreSQL
# No Linux/Mac
sudo systemctl stop postgresql

# Ou alterar a porta no docker-compose.yml
ports:
  - "5433:5432"  # Usar 5433 no host
```

## 📄 Licença

Este projeto foi desenvolvido para fins educacionais e de avaliação técnica.

## 👨‍💻 Autor

Desenvolvido como parte de um desafio técnico para demonstração de habilidades em:
- .NET 8
- Entity Framework Core
- Razor Pages
- PostgreSQL
- Docker
- Clean Architecture

---

**Observação:** Para dúvidas ou sugestões, abra uma issue no repositório.
