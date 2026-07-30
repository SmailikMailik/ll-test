using LL.Composition.Installers;
using LL.Game.Cards.Configuration;
using LL.Game.Promotions.Configuration;
using LL.Game.Quests.Configuration;
using LL.Game.Ranks.Configuration;
using LL.Game.Rewards.Configuration;
using LL.Presentation.Icons.Configuration;
using LL.UI.Windows.Configuration;
using LL.User.Configuration;
using UnityEngine;
using UnityEngine.Scripting.APIUpdating;
using VContainer;
using VContainer.Unity;

namespace LL.Composition.Scopes
{
    [MovedFrom(true, "LL.Composition")]
    [DisallowMultipleComponent]
    internal sealed class ProjectLifetimeScope : LifetimeScope
    {
        [Header("Windows")]
        [SerializeField] private WindowCatalogConfig _windowCatalogConfig;

        [Header("Game Data")]
        [SerializeField] private RankCatalogConfig _rankCatalogConfig;
        [SerializeField] private CardCatalogConfig _cardCatalogConfig;
        [SerializeField] private QuestCatalogConfig _questCatalogConfig;
        [SerializeField] private RankPromotionCatalogConfig _rankPromotionCatalogConfig;
        [SerializeField] private RewardCatalogConfig _rewardCatalogConfig;

        [Header("Presentation")]
        [SerializeField] private ItemIconCatalogConfig _itemIconCatalogConfig;

        [Header("User")]
        [SerializeField] private UserDefaultsConfig _userDefaultsConfig;

        protected override void Configure(IContainerBuilder builder)
        {
            new WindowInstaller(_windowCatalogConfig).Install(builder);
            new PresentationInstaller(_itemIconCatalogConfig).Install(builder);
            new ValidationReportingInstaller().Install(builder);
            new GameDataInstaller(
                    _rankCatalogConfig,
                    _cardCatalogConfig,
                    _questCatalogConfig,
                    _rankPromotionCatalogConfig,
                    _rewardCatalogConfig)
                .Install(builder);
            new UserInstaller(_userDefaultsConfig).Install(builder);
            new GameServicesInstaller().Install(builder);
        }
    }
}