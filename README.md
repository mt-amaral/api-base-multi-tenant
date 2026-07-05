# API base multi-tenant .net 9.0

API em ASP.NET Core para servir como base white label, com autenticação, multi-tenant, **ASP.NET Identity**, **cookie auth** e **refresh token**.

O projeto reaproveita o mesmo núcleo em mais de uma aplicação, mantendo banco, entidades e configurações principais em um lugar só:

- **DTOs com `record`** para entrada e saída
- **Response padrão** para quase todas as respostas da API
- **PagedResponse** para consultas paginadas
- **Validação** na controller
- **Services** focados em regra de negócio
- **Identity customizado** com entidades próprias e `long` como chave
- **Mappings do Identity** separados para controlar tamanho de campos, nomes de tabela e afins
- **Api.Core** com o que é compartilhado entre as APIs: contexto, entidades, mappings e migrations
- **Api** para a parte admin/recrutador
- **Api.Client** para a parte cliente
- **ConfigApp** centralizando parâmetros estáticos da aplicação
- **ExceptionMiddleware** para capturar erro inesperado e devolver no padrão da API

![API Swagger overview](docs/images/zzz.png)

![API Swagger resources](docs/images/zzz1.png)

## Multi tenant

O multi-tenant permite que a mesma base atenda mais de uma empresa/cliente, usando um único banco e mantendo a separação por tenant.

Esse formato ajuda a criar APIs white label: a regra principal fica no núcleo e cada aplicação expõe apenas o que precisa.

Hoje a base já tem:

- `Company` representando a empresa/tenant
- `AdminUser` vinculado a um usuário e uma empresa
- `ClientUser` vinculado a um usuário e uma empresa
- `UserType` no usuário para separar `Admin` e `Client`
- claims com referências como `user_type`, `company_id` e `tenant_id`

O `Api` aceita somente usuário admin.
O `Api.Client` aceita somente usuário cliente.

Os dois usam o mesmo `Api.Core`, o mesmo `ApplicationDbContext` e o mesmo banco.

As próximas entidades de negócio devem considerar o tenant/empresa quando a regra exigir isolamento.

## Autenticação

A autenticação usa **cookie do Identity** como login principal.
Além disso, existe um **refresh token** salvo no banco e enviado por cookie `HttpOnly`.

Fluxo:

1. Usuário faz login
2. API cria o cookie de autenticação
3. API gera e salva refresh token
4. Quando precisar renovar a sessão, usa o refresh token
5. No logout, encerra sessão e revoga o refresh token

## Padrão de retorno

A API trabalha com um objeto `Response<T>`:

- `data`
- `message`

Para paginação, usa `PagedResponse<T>` com:

- `data`
- `message`
- `currentPage`
- `pageSize`
- `totalCount`
- `totalPages`

Mantenha esse padrão nas respostas da API.

## Seed inicial

Se você iniciar o `Api` como Staging, ele irá gerar as migrations e popular os dados iniciais.
A seed fica somente no `Api`, não no `Api.Client`.

Hoje a seed cria uma empresa base, usuário admin, usuário cliente, roles e permissões do admin.

Usuário admin:

- nome: `Admin`
- email: `admin@teste.com`
- Vai encontrar a senha do Admin senha dentro de `Configurations/Seed/AdminUserSeed`

Usuário cliente:

- nome: `Client`
- email: `client@teste.com`
- Vai encontrar a senha do cliente dentro de `Configurations/Seed/ClientUserSeed`

Lembre-se de trocar usuário/senha em produção.

### Api.UnitTests (xUnit)

Os testes estão focados na validação dos requests das controllers (Input Model Validation).
Conforme as regras crescerem, vale cobrir também os principais métodos de services.

Rodar testes:
**dotnet test**

## Observações

o objetivo é manter a base simples e pratica:

- controller simples
- service com regra de negócio
- configurações centralizada
- retorno padronizado
- núcleo compartilhado no `Api.Core`
- coisas especificas ficam em cada API
- isolamento por tenant onde a regra de negócio precisar

## Banco

O projeto não deve depender de banco local para rodar.

As duas APIs usam a connection string via **user-secrets**, apontando para o banco de homologação.

Projetos que precisam estar com secret configurado:

- `Api`
- `Api.Client`

Chave usada:

- `ConnectionStrings:DefaultConnection`

Se aparecer erro tentando conectar em `localhost:5432`, provavelmente a API foi iniciada sem carregar os secrets ou está em um ambiente errado.
