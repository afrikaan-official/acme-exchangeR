using AcmeExchangeR.Utils.Models.Requests;
using FluentValidation;

namespace AcmeExchangeR.API.Validators;

public class TradeValidator : AbstractValidator<TradeRequest>
{
    public TradeValidator()
    {
        RuleFor(x => x.Amount).GreaterThan(0).WithMessage("Amount should be great than 0");
        RuleFor(x => x.From).Length(3, 3).WithMessage("From Exchange length must be 3 chars");
        RuleFor(x => x.To).Length(3, 3).WithMessage("To Exchange length must be 3 chars");
    }
}