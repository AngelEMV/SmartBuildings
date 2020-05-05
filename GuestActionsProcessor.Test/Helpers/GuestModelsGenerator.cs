using Bogus;
using GuestActionsProcessor.Models.Models;
using System;
using System.Collections.Generic;
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

        public static IEnumerable<GuestActionInfo> GenerateActionInfo(int amount, CheckinInfo seed)
        {
            var areas = new[] { "Room 210", "Room 211", "Room 212", "Gym", "Swimming Pool", "Restaurant", "Parking Lot" };

            return new Faker<GuestActionInfo>()
                .RuleFor(i => i.BuildingId, seed.BuildingId)
                .RuleFor(i => i.UserId, seed.UserId)
                .RuleFor(u => u.FirstName, seed.FirstName)
                .RuleFor(u => u.LastName, seed.LastName)
                .RuleFor(i => i.AreaName, (fake) => fake.PickRandom(areas))
                .RuleFor(i => i.ActionDateTime, DateTime.UtcNow)
                .GenerateLazy(amount);
        }

        public static CheckoutInfo GenerateCheckoutInfo(int amount, CheckinInfo seed)
        {
            return new Faker<CheckoutInfo>()
                .RuleFor(i => i.BuildingId, seed.BuildingId)
                .RuleFor(i => i.UserId, seed.UserId)
                .RuleFor(u => u.FirstName, seed.FirstName)
                .RuleFor(u => u.LastName, seed.LastName)
                .RuleFor(i => i.CheckoutDateTime, DateTime.UtcNow)
                .GenerateLazy(amount).FirstOrDefault();
        }
    }
}
