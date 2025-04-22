using System;
using System.Collections.Generic;
using Leopotam.Ecs;
using Settings;
using TMPro;
using UI;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace Systems
{
    public class BusinessSpawnSystem : IEcsInitSystem
    {
        private EcsWorld _ecsWorld;
        private ConfigValues _configValues;
        private SavedKeys _savedKeys;
        private SpawnData _spawnData;

        public void Init()
        {
            for (int i = 0; i < _configValues.Businesses.Count; i++)
            {
                var businessEntity = CreateBusinessEntity(i);
                ref var business = ref businessEntity.Get<Business>();
                InitializeUpgradeLists(ref business, i);
                var businessGO = SpawnBusinessGameObject();
                if (businessGO == null) continue;
                SetupTextComponents(ref business, businessGO);
                SetupSlider(ref business, businessGO);
                SetupButtonComponents(ref business, businessGO);
            }
        }
        
        private EcsEntity CreateBusinessEntity(int index)
        {
            var entity = _ecsWorld.NewEntity();
            ref var business = ref entity.Get<Business>();
            business.Level = PlayerPrefs.GetInt(string.Format(_savedKeys.LevelKey, index), index == 0 ? 1 : 0);
            return entity;
        }
        
        private void InitializeUpgradeLists(ref Business business, int index)
        {
            business.Multiplier = new List<float>();
            business.UpgradeButton = new List<Button>();
            business.UpgradeText = new List<TextMeshProUGUI>();
            business.UpgradeString = new List<string>();
            business.UpgradeCost = new List<float>();

            foreach (var upgrade in _configValues.Businesses[index].Upgrades)
            {
                business.Multiplier.Add(0f);
                business.UpgradeButton.Add(default);
                business.UpgradeText.Add(default);
                business.UpgradeString.Add(default);
                business.UpgradeCost.Add(upgrade.Cost);
            }
        }
        
        private GameObject SpawnBusinessGameObject()
        {
            var businessGO = Object.Instantiate(_spawnData.BuisnessPrefab, _spawnData.BuisnessTransform);
            if (businessGO == null)
            {
                Debug.LogError("Failed to instantiate BuisnessPrefab!");
            }
            return businessGO;
        }
        
        private void SetupTextComponents(ref Business business, GameObject businessGO)
        {
            foreach (var text in businessGO.GetComponentsInChildren<TextMeshProUGUI>())
            {
                var textController = text.GetComponent<TextController>();
                if (textController == null)
                {
                    Debug.LogWarning($"TextMeshProUGUI on {text.gameObject.name} has no TextController component!");
                    continue;
                }

                switch (textController.Type)
                {
                    case TextType.Name:
                        business.Name = text;
                        break;
                    case TextType.Level:
                        business.LevelText = text;
                        business.LevelString = text.text;
                        break;
                    case TextType.Income:
                        business.IncomeText = text;
                        business.IncomeString = business.IncomeText.text;
                        break;
                    case TextType.LevelUp:
                        business.LevelUpText = text;
                        business.LevelUpString = business.LevelUpText.text;
                        break;
                    case TextType.Upgrade1:
                        business.UpgradeText[0] = text;
                        business.UpgradeString[0] = business.UpgradeText[0].text;
                        break;
                    case TextType.Upgrade2:
                        business.UpgradeText[1] = text;
                        business.UpgradeString[1] = business.UpgradeText[1].text;
                        break;
                    default:
                        Debug.LogWarning($"Unknown TextType: {textController.Type} on {text.gameObject.name}");
                        break;
                }
            }
        }
        
        private void SetupSlider(ref Business business, GameObject businessGO)
        {
            business.Progress = businessGO.GetComponentInChildren<Slider>();
            if (business.Progress == null)
            {
                Debug.LogError("Slider component not found in BuisnessPrefab!");
            }
        }
        
        private void SetupButtonComponents(ref Business business, GameObject businessGO)
        {
            foreach (var button in businessGO.GetComponentsInChildren<Button>())
            {
                var buttonController = button.GetComponent<ButtonController>();
                if (buttonController == null)
                {
                    Debug.LogWarning($"Button on {button.gameObject.name} has no ButtonController component!");
                    continue;
                }

                switch (buttonController.Type)
                {
                    case ButtonType.LevelUp:
                        business.LevelUpButton = button;
                        break;
                    case ButtonType.Upgrade1:
                        business.UpgradeButton[0] = button;
                        break;
                    case ButtonType.Upgrade2:
                        business.UpgradeButton[1] = button;
                        break;
                    default:
                        Debug.LogWarning($"Unknown ButtonType: {buttonController.Type} on {button.gameObject.name}");
                        break;
                }
            }
            
        }
    }
}