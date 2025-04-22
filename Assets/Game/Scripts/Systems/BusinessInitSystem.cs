using System.Linq;
using Leopotam.Ecs;
using Settings;
using UnityEngine;

namespace Systems
{
    public class BusinessInitSystem : IEcsInitSystem
    {
        private ConfigNames _configNames;
        private ConfigValues _configValues;
        private SavedKeys _savedKeys;
        private EcsFilter<Business> _businessFilter;
        private EcsFilter<Balance> _balanceFilter;

        public void Init()
        {
            foreach (var idx in _businessFilter)
            {
                ref var business = ref _businessFilter.Get1(idx);
                var data = _configValues.Businesses[idx];
                
                InitializeBusiness(ref business, idx, data, _configNames);
                
                for (int i = 0; i < business.UpgradeText.Count; i++)
                {
                    if (PlayerPrefs.GetInt(string.Format(_savedKeys.UpgradeKey, idx, i)) == 0)
                    {
                        var cost = string.Format(_configNames.Cost, business.UpgradeCost[i]);
                        business.UpgradeText[i].text = string.Format(business.UpgradeString[i],
                            _configNames.Upgrade[i], data.Upgrades[i].Multiplier, cost);
                    }
                    else
                    {
                        ApplyUpgrade(ref business, idx, data, _configNames);
                    }
                }
                
                business.LevelUpButton.onClick.AddListener(() =>
                {
                    ref var businessC = ref _businessFilter.Get1(idx);
                    ref var balance = ref _balanceFilter.Get1(0);
                    
                    if (businessC.LevelUpCost <= balance.BalanceSum)
                    {
                        LevelUpBusiness(ref balance, ref businessC, idx, data, _savedKeys);
                    }
                });
                
                for (int i = 0; i < business.UpgradeButton.Count; i++)
                {
                    var j = i;
                    
                    business.UpgradeButton[i].onClick.AddListener(() =>
                    {
                        ref var businessC = ref _businessFilter.Get1(idx);
                        ref var balance = ref _balanceFilter.Get1(0);
                        if (businessC.UpgradeCost[j] <= balance.BalanceSum)
                        { 
                            PurchaseUpgrade(ref balance, ref businessC, idx, j, data, _configNames, _savedKeys);
                        }
                    });
                }
            }
        }
        
        private void InitializeBusiness(ref Business business, int index, ConfigValues.BusinessData data, ConfigNames configNames)
        {
            business.Name.text = configNames.Business[index];
            
            business.LevelText.text = string.Format(business.LevelString, business.Level);
            
            business.Income = business.Level * data.BaseIncome;
            business.IncomeText.text = string.Format(business.IncomeString, business.Income);
            
            business.LevelUpCost = (business.Level + 1) * data.BaseCost;
            business.LevelUpText.text = string.Format(business.LevelUpString, business.LevelUpCost);
        }

        private void ApplyUpgrade(ref Business business, int index, ConfigValues.BusinessData data, ConfigNames configNames)
        {
            business.Multiplier[index] = data.Upgrades[index].Multiplier / 100f;
            
            business.Income = business.Level * data.BaseIncome * (1 + business.Multiplier.Sum());
            business.IncomeText.text = string.Format(business.IncomeString, business.Income);
            
            business.UpgradeText[index].text = string.Format(business.UpgradeString[index], configNames.Upgrade[index], data.Upgrades[index].Multiplier, configNames.Bought);
            
            business.UpgradeButton[index].interactable = false;
        }
        
        private void LevelUpBusiness(ref Balance balance, ref Business businessC, int index, ConfigValues.BusinessData data, SavedKeys savedKeys)
        {
            balance.BalanceSum -= businessC.LevelUpCost;
            balance.BalanceText.text = string.Format(balance.BalanceString, balance.BalanceSum);
            PlayerPrefs.SetFloat(savedKeys.BalanceKey, balance.BalanceSum);
            
            businessC.Level++;
            businessC.LevelText.text = string.Format(businessC.LevelString, businessC.Level);
            PlayerPrefs.SetInt(string.Format(savedKeys.LevelKey, index), businessC.Level);
            
            businessC.Income = businessC.Level * data.BaseIncome * (1 + businessC.Multiplier.Sum());
            businessC.IncomeText.text = string.Format(businessC.IncomeString, businessC.Income);
            
            businessC.LevelUpCost = (businessC.Level + 1) * data.BaseCost;
            businessC.LevelUpText.text = string.Format(businessC.LevelUpString, businessC.LevelUpCost);
        }
        
        private void PurchaseUpgrade(ref Balance balance, ref Business businessC, int index, int j, ConfigValues.BusinessData data, ConfigNames configNames, SavedKeys savedKeys)
        {
            balance.BalanceSum -= businessC.UpgradeCost[j];
            balance.BalanceText.text = string.Format(balance.BalanceString, balance.BalanceSum);
            PlayerPrefs.SetFloat(savedKeys.BalanceKey, balance.BalanceSum);

            businessC.Multiplier[j] = data.Upgrades[j].Multiplier / 100f;
            
            businessC.Income = businessC.Level * data.BaseIncome * (1 + businessC.Multiplier.Sum());
            businessC.IncomeText.text = string.Format(businessC.IncomeString, businessC.Income);
            
            businessC.UpgradeText[j].text = string.Format(businessC.UpgradeString[j], configNames.Upgrade[j], data.Upgrades[j].Multiplier, configNames.Bought);
            
            PlayerPrefs.SetInt(string.Format(savedKeys.UpgradeKey, index, j), 1);
            
            businessC.UpgradeButton[j].interactable = false;
        }
    }
}
