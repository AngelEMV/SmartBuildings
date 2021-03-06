﻿using Bogus;
using GuestActionsProcessor.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GuestActionsProcessor.EventGenerator.Helpers
{
    public interface IGuestModelsGenerator
    {
        BuildingInfo GenerateBuildingInfo();
        UserInfo GenerateUserInfo();
        CheckinInfo GenerateCheckinInfo();
        IEnumerable<GuestActionInfo> GenerateActionInfo(int amount, BuildingInfo buildingSeed, UserInfo userSeed);
        public CheckoutInfo GenerateCheckoutInfo();
    }

    public class GuestModelsGenerator : IGuestModelsGenerator
    {
        public BuildingInfo GenerateBuildingInfo()
        {
            return new Faker<BuildingInfo>()
                .RuleFor(i => i.BuildingId, (fake) => Guid.NewGuid())
                .RuleFor(i => i.City, (f, u) => f.Address.City())
                .RuleFor(i => i.Address, (f, u) => f.Address.StreetAddress())
                .GenerateLazy(1).FirstOrDefault();
        }

        public UserInfo GenerateUserInfo()
        {
            return new Faker<UserInfo>()
                .RuleFor(i => i.UserId, (fake) => Guid.NewGuid())
                .RuleFor(u => u.FirstName, (f, u) => f.Name.FirstName())
                .RuleFor(u => u.LastName, (f, u) => f.Name.LastName())
                .RuleFor(u => u.Email, (f, u) => f.Internet.Email(u.FirstName, u.LastName))
                .GenerateLazy(1).FirstOrDefault();
        }

        public CheckinInfo GenerateCheckinInfo()
        {
            return new Faker<CheckinInfo>()
                .RuleFor(i => i.CheckinDateTime, DateTime.UtcNow)
                .GenerateLazy(1).FirstOrDefault();
        }

        public IEnumerable<GuestActionInfo> GenerateActionInfo(
            int amount, 
            BuildingInfo buildingSeed,
            UserInfo userSeed)
        {
            var areas = new[] { "Room 210", "Room 211", "Room 212", "Gym", "Swimming Pool", "Restaurant", "Parking Lot" };

            return new Faker<GuestActionInfo>()
                .RuleFor(i => i.BuildingInfo, buildingSeed)
                .RuleFor(i => i.UserInfo, userSeed)
                .RuleFor(i => i.id, (fake) => Guid.NewGuid())
                .RuleFor(i => i.AreaName, (fake) => fake.PickRandom(areas))
                .RuleFor(i => i.ActionDateTime, DateTime.UtcNow)
                .GenerateLazy(amount);
        }

        public CheckoutInfo GenerateCheckoutInfo()
        {
            return new Faker<CheckoutInfo>()
                .RuleFor(i => i.CheckoutDateTime, DateTime.UtcNow)
                .GenerateLazy(1).FirstOrDefault();
        }
    }
}
