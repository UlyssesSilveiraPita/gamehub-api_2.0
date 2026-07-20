using GameHub.API.Entities;
using GameHub.API.Enums;

namespace GameHub.Tests.Builders;

public class GameProductBuilder
{
    private readonly GameProduct _product;

    public GameProductBuilder()
    {
        _product = new GameProduct
        {
            Id = 1,
            GameId = 1,
            Name = "Wild Hunter",
            Description = "Default Description",
            ProductType = GameProductType.BaseGame,
            Price = 59.90m,
            Currency = "BRL",
            IsActive = true
        };
    }

    public GameProductBuilder WithId(int id)
    {
        _product.Id = id;
        return this;
    }

    public GameProductBuilder WithPrice(decimal price)
    {
        _product.Price = price;
        return this;
    }

    public GameProductBuilder WithName(string name)
    {
        _product.Name = name;
        return this;
    }

    public GameProductBuilder WithCurrency(string currency)
    {
        _product.Currency = currency;
        return this;
    }

    public GameProductBuilder Active()
    {
        _product.IsActive = true;
        return this;
    }

    public GameProductBuilder Inactive()
    {
        _product.IsActive = false;
        return this;
    }

    public GameProduct Build()
    {
        return _product;
    }
}
