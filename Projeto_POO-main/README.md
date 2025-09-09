## Documentação da API FFCE
OBS: Os endpoints são case-sensitive. 
### RODANDO O PROJETO
`dotnet build`

`dotnet run`
### Fazendo Migrations
`dotnet ef migrations add NOMEDAMIGRATION`

`dotnet ef database update`

`dotnet build`

`dotnet run`
### Removendo Migrations
`dotnet ef migrations remove`

---

**Base URL:** `{{baseUrl}}/api`

---

### OBS:
NÃO É PARA O USUÁRIO COLOCAR O TOKEN JWT NA HORA DA AUTENTICAÇÃO E NEM VER O RETORNO DA API.