using Bogus;
using GuestActionsProcessor.Models.Models;
using System;
using System.Linq;

namespace GuestActionsProcessor.Test.Helpers
{
    public static class GuestModelsGenerator
    {
        public static CheckinInfo GenerateCheckinInfo(int amount)
        {
            return new Faker<CheckinInfo>()
                .RuleFor(i => i.BuildingId, (fake) => Guid.NewGuid().ToString())
                .RuleFor(i => i.UserId, (fake) => Guid.NewGuid().ToString())
                .RuleFor(i => i.CheckinDateTime, DateTime.UtcNow)
                .RuleFor(u => u.FirstName, (f, u) => f.Name.FirstName())
                .RuleFor(u => u.LastName, (f, u) => f.Name.LastName())
                .RuleFor(u => u.Email, (f, u) => f.Internet.Email(u.FirstName, u.LastName))
                .GenerateLazy(amount).FirstOrDefault();
        }

        public static CheckoutInfo GenerateCheckoutInfo(int amount)
        {
            return new Faker<CheckoutInfo>()
                .RuleFor(i => i.BuildingId, (fake) => Guid.NewGuid().ToString())
                .RuleFor(i => i.UserId, (fake) => Guid.NewGuid().ToString())
                .RuleFor(i => i.CheckoutDateTime, DateTime.UtcNow)
                .GenerateLazy(amount).FirstOrDefault();
        }
    }
}
