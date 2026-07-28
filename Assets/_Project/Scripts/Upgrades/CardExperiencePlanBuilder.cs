using System;
using System.Collections.Generic;
using LL.Game.Cards;
using LL.User.Core.Cards;

namespace LL.Upgrades
{
    internal static class CardExperiencePlanBuilder
    {
        private const int MinimumAmount = 0;

        internal static IReadOnlyList<CardAmount> Build(
            IReadOnlyList<ExperienceCardOption> cards,
            int requiredExperience)
        {
            if (cards == null)
                throw new ArgumentNullException(nameof(cards));

            if (CanReach(cards, requiredExperience) is false)
                return Array.Empty<CardAmount>();

            var experienceLimit = GetExperienceLimit(cards, requiredExperience);
            var plans = CreatePlans(cards.Count, experienceLimit);

            for (var cardIndex = 0; cardIndex < cards.Count; cardIndex++)
                AddCardOptions(plans, cards[cardIndex], cardIndex, experienceLimit);

            var bestPlan = FindBestPlan(plans, requiredExperience);
            return CreateCardAmounts(cards, bestPlan);
        }

        internal static bool CanReach(
            IReadOnlyList<ExperienceCardOption> cards,
            int requiredExperience)
        {
            if (cards == null)
                throw new ArgumentNullException(nameof(cards));

            if (requiredExperience <= MinimumAmount)
                return false;

            var missingExperience = requiredExperience;

            foreach (var card in cards)
            {
                if (card.AvailableAmount <= MinimumAmount ||
                    card.ExperienceAmount <= MinimumAmount)
                {
                    continue;
                }

                var requiredAmount =
                    (missingExperience - 1) / card.ExperienceAmount + 1;

                if (card.AvailableAmount >= requiredAmount)
                    return true;

                missingExperience -=
                    card.AvailableAmount * card.ExperienceAmount;
            }

            return false;
        }

        private static int GetExperienceLimit(
            IReadOnlyList<ExperienceCardOption> cards,
            int requiredExperience)
        {
            var maximumCardExperience = MinimumAmount;

            foreach (var card in cards)
            {
                if (card.AvailableAmount > MinimumAmount)
                {
                    maximumCardExperience = Math.Max(
                        maximumCardExperience,
                        card.ExperienceAmount);
                }
            }

            var overflowAllowance = maximumCardExperience - 1;

            return requiredExperience <= int.MaxValue - overflowAllowance
                ? requiredExperience + overflowAllowance
                : int.MaxValue;
        }

        private static Plan[] CreatePlans(int cardTypesCount, int experienceLimit)
        {
            var plans = new Plan[experienceLimit + 1];
            plans[MinimumAmount] = new Plan(cardTypesCount);

            return plans;
        }

        private static void AddCardOptions(
            Plan[] plans,
            ExperienceCardOption card,
            int cardIndex,
            int experienceLimit)
        {
            if (card.AvailableAmount <= MinimumAmount ||
                card.ExperienceAmount <= MinimumAmount)
            {
                return;
            }

            var plansWithoutCurrentCard = (Plan[])plans.Clone();

            for (var experience = MinimumAmount; experience <= experienceLimit; experience++)
            {
                var currentPlan = plansWithoutCurrentCard[experience];

                if (currentPlan == null)
                    continue;

                AddPossibleAmounts(
                    plans,
                    currentPlan,
                    card,
                    cardIndex,
                    experience,
                    experienceLimit);
            }
        }

        private static void AddPossibleAmounts(
            Plan[] plans,
            Plan currentPlan,
            ExperienceCardOption card,
            int cardIndex,
            int currentExperience,
            int experienceLimit)
        {
            var availableExperience = experienceLimit - currentExperience;
            var maximumAmount = Math.Min(
                card.AvailableAmount,
                availableExperience / card.ExperienceAmount);

            for (var amount = 1; amount <= maximumAmount; amount++)
            {
                var targetExperience =
                    currentExperience + amount * card.ExperienceAmount;
                TryStoreBetterPlan(
                    plans,
                    targetExperience,
                    currentPlan,
                    cardIndex,
                    amount);
            }
        }

        private static void TryStoreBetterPlan(
            Plan[] plans,
            int experience,
            Plan currentPlan,
            int cardIndex,
            int amount)
        {
            var storedPlan = plans[experience];
            var totalCards = currentPlan.TotalCards + amount;

            if (storedPlan != null && storedPlan.TotalCards <= totalCards)
                return;

            plans[experience] = currentPlan.Add(cardIndex, amount);
        }

        private static Plan FindBestPlan(
            IReadOnlyList<Plan> plans,
            int requiredExperience)
        {
            for (var experience = requiredExperience;
                 experience < plans.Count;
                 experience++)
            {
                if (plans[experience] != null)
                    return plans[experience];
            }

            return null;
        }

        private static IReadOnlyList<CardAmount> CreateCardAmounts(
            IReadOnlyList<ExperienceCardOption> cards,
            Plan plan)
        {
            if (plan == null)
                return Array.Empty<CardAmount>();

            var cardAmounts = new List<CardAmount>(cards.Count);

            for (var index = 0; index < cards.Count; index++)
            {
                var amount = plan.Amounts[index];

                if (amount > MinimumAmount)
                    cardAmounts.Add(new CardAmount(cards[index].Id, amount));
            }

            return cardAmounts;
        }

        private sealed class Plan
        {
            internal int TotalCards { get; }
            internal IReadOnlyList<int> Amounts => _amounts;

            private readonly int[] _amounts;

            internal Plan(int cardTypesCount)
            {
                _amounts = new int[cardTypesCount];
            }

            private Plan(int[] amounts, int totalCards)
            {
                _amounts = amounts;
                TotalCards = totalCards;
            }

            internal Plan Add(int cardIndex, int amount)
            {
                var amounts = (int[])_amounts.Clone();
                amounts[cardIndex] += amount;

                return new Plan(amounts, TotalCards + amount);
            }
        }
    }

    internal readonly struct ExperienceCardOption
    {
        internal CardId Id { get; }
        internal int AvailableAmount { get; }
        internal int ExperienceAmount { get; }

        internal ExperienceCardOption(
            CardId id,
            int availableAmount,
            int experienceAmount)
        {
            Id = id;
            AvailableAmount = availableAmount;
            ExperienceAmount = experienceAmount;
        }
    }
}