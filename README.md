# 🏠 NossoLar

![.NET](https://img.shields.io/badge/.NET-10-512BD4?logo=dotnet&logoColor=white)
![C#](https://img.shields.io/badge/C%23-14-239120?logo=csharp&logoColor=white)
![Status](https://img.shields.io/badge/status-em%20desenvolvimento-yellow)

Sistema de gestão doméstica para casais que estão se mudando para a primeira casa juntos. Ele resolve duas dores conectadas:

- **Encontrar o imóvel certo**: cadastrar e comparar imóveis candidatos dentro de critérios claros (bairro, características mínimas e teto de preço já com condomínio e IPTU).
- **Controlar as finanças do casal**: renda, gastos por grupo, reserva de emergência e a sobra do mês.

Os dois lados se encontram num ponto: o aluguel escolhido vira uma despesa no orçamento, e o orçamento define quanto de aluguel o casal pode pagar.

---

## 🧱 Arquitetura

O projeto segue **Clean Architecture** com **DDD tático**, num **monólito modular**: um único deploy, dividido em módulos de negócio bem isolados.

```
        ┌────────────────────────────┐
        │            API             │  Controllers, Program.cs
        │  ┌──────────────────────┐  │
        │  │     Application      │  │  Casos de uso, DTOs, interfaces
        │  │  ┌────────────────┐  │  │
        │  │  │     Domain     │  │  │  Entidades, value objects, regras
        │  │  └────────────────┘  │  │
        │  └──────────────────────┘  │
        │       Infrastructure       │  EF Core, repositórios
        └────────────────────────────┘
     As dependências apontam sempre para dentro.
```

**Princípios adotados:**

- **Domínio rico**: as regras de negócio vivem nas entidades, não em services. Uma entidade nunca entra em estado inválido.
- **Encapsulamento**: nada de `public set`. Toda mudança passa por um método com nome de ação (`AddExpense`, `ChangeRentalCosts`...).
- **Value objects imutáveis** (`sealed record`) para conceitos definidos pelos seus valores, como `Address` e `RentalCosts`.
- **Agregados**: a raiz controla seus filhos. Um gasto só entra no orçamento por `budget.AddExpense(...)`.
- **Valores calculados, nunca guardados**: totais e saldos são propriedades calculadas, então nunca ficam desatualizados.
- **`decimal` para dinheiro**, sempre arredondado a 2 casas.

---

## 📦 Módulos

| Módulo | Responsabilidade | Status |
| --- | --- | --- |
| **Residentials** | Imóveis candidatos: endereço, custos (aluguel, condomínio, IPTU), tipo, imobiliária e link do anúncio | 🟡 Domínio em construção |
| **MonthlyBudgets** | Orçamento mensal: renda, gastos por grupo, reserva de emergência e sobra | 🟡 Domínio em construção |
| **Identity** | Usuários, autenticação e o household (o casal) | ⚪ Planejado |

### Modelo de domínio atual

**Residentials**

- `Residential` (entidade): `Address`, `RentalCosts`, `ResidentialType`, `RealEstateAgency?`, `Notes?`, `Url?`
- `Address` (value object): rua, número, bairro, cidade, UF, CEP, complemento e ponto de referência
- `RentalCosts` (value object): aluguel, condomínio, IPTU e o **total calculado**
- `RealEstateAgency` (value object): nome, endereço, CNPJ e telefone

**MonthlyBudgets**

- `MonthlyBudget` (raiz do agregado): renda total, percentual da reserva e a lista de gastos
  - Calculados: `TotalExpenses`, `EmergencyReserveAmount`, `Balance`, `HasNegativeBalance`
  - Consultas: `PercentageOfIncome(...)`, `TotalsByGroup()`
- `Expense` (entidade filha): grupo, descrição e valor mensal
- `ExpenseGroup` (enum): Housing, Bills, GroceriesAndHome, Transportation, Health, LeisureAndPersonal, Other

---

## 📏 Regras de negócio (exemplos)

| Regra | Descrição |
| --- | --- |
| Custo total do imóvel | `Aluguel + Condomínio + IPTU`, sempre calculado pelo domínio |
| Valores válidos | Aluguel > 0; condomínio, IPTU e gastos não podem ser negativos |
| Sobra do mês | `Renda − Total de gastos − Reserva de emergência` |
| Reserva de emergência | Percentual da renda, entre 0% e 100% (padrão 10%) |
| Alerta de sobra negativa | Se a sobra ficar abaixo de zero, o orçamento sinaliza (avisa, não bloqueia) |
| Link do anúncio | Opcional; quando informado, precisa ser um endereço http(s) válido |

---

## 🗂️ Estrutura do projeto

```
NossoLar/
├── NossoLar.slnx
├── NossoLar.Domain/
│   ├── Common/              # Entity e classes base
│   ├── Exceptions/          # DomainException
│   ├── Residentials/
│   │   ├── Entities/
│   │   ├── Enums/
│   │   └── ValueObjects/
│   └── MonthlyBudgets/
│       ├── Entities/
│       └── Enums/
└── NossoLar.Domain.Test/
    └── Residentials/        # Testes do domínio
```

---

## 🚀 Como rodar

**Pré-requisito:** [.NET 10 SDK](https://dotnet.microsoft.com/download)

```bash
# clonar o repositório
git clone <url-do-repositorio>
cd NossoLar

# compilar
dotnet build

# rodar os testes
dotnet test
```

---

## 🧪 Testes

Os testes ficam em `NossoLar.Domain.Test` e usam **xUnit**. O foco está no domínio: como as regras vivem nas entidades, elas podem ser testadas sem banco de dados nem HTTP.

Convenção de nomes: `Método_Cenário_ResultadoEsperado`

```csharp
[Fact]
public void ChangeUrl_WithInvalidValue_KeepsCurrentUrl()
```

---

## 🛣️ Roadmap

- [x] Estrutura da solução e camada de domínio
- [x] Value objects de imóveis (`Address`, `RentalCosts`, `RealEstateAgency`)
- [x] Entidades `Residential`, `MonthlyBudget` e `Expense`
- [x] Testes de domínio do `Residential`
- [ ] Testes de domínio do `MonthlyBudget`
- [ ] Ciclo de vida do imóvel (candidato → visitado → contratado / descartado)
- [ ] Mês de referência no orçamento (histórico mês a mês)
- [ ] Módulo de identidade: usuários, household e autenticação JWT
- [ ] Persistência com EF Core + PostgreSQL
- [ ] Camada de aplicação (casos de uso) e API REST
- [ ] Integração: contratar um imóvel gera a despesa de aluguel no orçamento

---

## 🛠️ Tecnologias

- **.NET 10** / **C#**
- **xUnit** para testes
- Planejado: **EF Core**, **PostgreSQL**, **FluentValidation**, **Serilog**, **Swagger/OpenAPI**

---

## 👤 Autor

**Ramon Frias**

Projeto de estudo de Clean Architecture e DDD aplicado a um problema real.
