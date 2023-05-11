using TaleWorlds.CampaignSystem.ViewModelCollection.Inventory;
using HarmonyLib;
using TaleWorlds.Core;
using static TaleWorlds.Core.ItemObject.ItemTypeEnum;
using TaleWorlds.Localization;
using System;
using TaleWorlds.CampaignSystem.ViewModelCollection;
using TaleWorlds.Core.ViewModelCollection.Generic;
using System.Runtime.CompilerServices;
using TaleWorlds.Core.ViewModelCollection;

namespace ShieldUiMod
{
    [HarmonyPatch(typeof(ItemMenuVM), "SetWeaponComponentTooltip")]
    public class ShieldUi_patch
    {
        static bool Prefix(in EquipmentElement targetWeapon, int targetWeaponUsageIndex, EquipmentElement comparedWeapon, int comparedWeaponUsageIndex, ItemMenuVM __instance,bool isInit, ItemVM ____comparedItem,TextObject ____classText, TextObject ____weaponTierText,TextObject ____swingSpeedText,TextObject ____swingDamageText,TextObject ____thrustSpeedText,TextObject ____thrustDamageText,TextObject ____lengthText,TextObject ____handlingText, TextObject ____weaponLengthText, TextObject ____damageText, TextObject ____missileSpeedText, TextObject ____accuracyText, TextObject ____stackAmountText, TextObject ____speedText, TextObject ____hitPointsText,TextObject ____ammoLimitText,TextObject ____noneText, TextObject ____bannerEffectText, TextObject ____bodyArmorText) {
            EquipmentElement equipmentElement = targetWeapon;
            WeaponComponentData weaponWithUsageIndex = equipmentElement.Item.GetWeaponWithUsageIndex(targetWeaponUsageIndex);
            if (__instance.IsComparing && ____comparedItem != null && comparedWeapon.IsEmpty)
            {
                var getComparedWeapon = AccessTools.Method(typeof(ItemMenuVM), "GetComparedWeapon");
                object[] parameters = new object[] { weaponWithUsageIndex.WeaponDescriptionId,  comparedWeapon,  comparedWeaponUsageIndex };
                getComparedWeapon.Invoke(__instance, parameters);
                comparedWeapon = (EquipmentElement)parameters[1];
                comparedWeaponUsageIndex = (int)parameters[2];
            }
            WeaponComponentData weaponComponentData = comparedWeapon.IsEmpty ? null : comparedWeapon.Item.GetWeaponWithUsageIndex(comparedWeaponUsageIndex);
            if (isInit)
            {
                var addWeaponItemFlags = AccessTools.Method(typeof(ItemMenuVM), "AddWeaponItemFlags");
                addWeaponItemFlags.Invoke(__instance, new object[] { __instance.TargetItemFlagList, weaponWithUsageIndex });
                if (__instance.IsComparing)
                {
                    var addWeaponItemFlag = AccessTools.Method(typeof(ItemMenuVM), "AddWeaponItemFlags");
                    addWeaponItemFlag.Invoke(__instance, new object[] { __instance.ComparedItemFlagList, weaponComponentData });
                }
                if (targetWeaponUsageIndex == 0)
                {
                    __instance.AlternativeUsageIndex = -1;
                }
                equipmentElement = targetWeapon;
                foreach (WeaponComponentData weaponComponentData2 in equipmentElement.Item.Weapons)
                {
                    if (CampaignUIHelper.IsItemUsageApplicable(weaponComponentData2))
                    {
                        __instance.AlternativeUsages.Add(new StringItemWithHintVM(GameTexts.FindText("str_weapon_usage", weaponComponentData2.WeaponDescriptionId).ToString(), GameTexts.FindText("str_inventory_alternative_usage_hint", null)));
                    }
                }
                __instance.AlternativeUsageIndex = targetWeaponUsageIndex;
            }
            var createProperty = AccessTools.Method(typeof(ItemMenuVM), "CreateProperty");
            createProperty.Invoke(__instance, new object[] { __instance.TargetItemProperties, ____classText.ToString(), GameTexts.FindText("str_inventory_weapon", ((int)weaponWithUsageIndex.WeaponClass).ToString()).ToString(), 0, null });
            if (!comparedWeapon.IsEmpty)
            {
                createProperty.Invoke(__instance, new object[] {__instance.ComparedItemProperties, " ", GameTexts.FindText("str_inventory_weapon", ((int)weaponWithUsageIndex.WeaponClass).ToString()).ToString(), 0, null});
            }
            else if (__instance.IsComparing)
            {
                createProperty.Invoke(__instance, new object[] { __instance.ComparedItemProperties, "", "", 0, null });
            }
            equipmentElement = targetWeapon;

            var addIntProperty = AccessTools.Method(typeof(ItemMenuVM), "AddIntProperty");
            var addSwingDamageProperty = AccessTools.Method(typeof(ItemMenuVM), "AddSwingDamageProperty");
            var addThrustDamageProperty = AccessTools.Method(typeof(ItemMenuVM), "AddThrustDamageProperty");
            var addMissileDamageProperty = AccessTools.Method(typeof(ItemMenuVM), "AddMissileDamageProperty");

            if (equipmentElement.Item.BannerComponent == null)
            {
                int value = 0;
                if (!comparedWeapon.IsEmpty)
                {
                    value = (int)(comparedWeapon.Item.Tier + 1);
                }
                TextObject weaponTierText = ____weaponTierText;
                equipmentElement = targetWeapon;
                addIntProperty.Invoke(__instance, new object[] { weaponTierText, (int)(equipmentElement.Item.Tier + 1), new int?(value) });
            }
            ItemObject.ItemTypeEnum itemTypeFromWeaponClass = WeaponComponentData.GetItemTypeFromWeaponClass(weaponWithUsageIndex.WeaponClass);
            ItemObject.ItemTypeEnum itemTypeEnum = (!comparedWeapon.IsEmpty) ? WeaponComponentData.GetItemTypeFromWeaponClass(weaponWithUsageIndex.WeaponClass) : ItemObject.ItemTypeEnum.Invalid;
                if (itemTypeFromWeaponClass == ItemObject.ItemTypeEnum.OneHandedWeapon || itemTypeFromWeaponClass == ItemObject.ItemTypeEnum.TwoHandedWeapon || itemTypeFromWeaponClass == ItemObject.ItemTypeEnum.Polearm || itemTypeEnum == ItemObject.ItemTypeEnum.OneHandedWeapon || itemTypeEnum == ItemObject.ItemTypeEnum.TwoHandedWeapon || itemTypeEnum == ItemObject.ItemTypeEnum.Polearm)
                {
                    if (weaponWithUsageIndex.SwingDamageType != DamageTypes.Invalid)
                    {
                        TextObject swingSpeedText = ____swingSpeedText;
                        equipmentElement = targetWeapon;
                        addIntProperty.Invoke(__instance, new object[] { swingSpeedText, equipmentElement.GetModifiedSwingSpeedForUsage(targetWeaponUsageIndex), comparedWeapon.IsEmpty ? null : new int?(comparedWeapon.GetModifiedSwingSpeedForUsage(comparedWeaponUsageIndex)) });
                        addSwingDamageProperty.Invoke(__instance, new object[] { ____swingDamageText, targetWeapon, targetWeaponUsageIndex, comparedWeapon, comparedWeaponUsageIndex });
                    }
                    if (weaponWithUsageIndex.ThrustDamageType != DamageTypes.Invalid)
                    {
                        TextObject thrustSpeedText = ____thrustSpeedText;
                        equipmentElement = targetWeapon;
                        addIntProperty.Invoke(__instance, new object[] { thrustSpeedText, equipmentElement.GetModifiedThrustSpeedForUsage(targetWeaponUsageIndex), comparedWeapon.IsEmpty ? null : new int?(comparedWeapon.GetModifiedThrustSpeedForUsage(comparedWeaponUsageIndex)) });
                        addThrustDamageProperty.Invoke(__instance, new object[] { ____thrustDamageText, targetWeapon, targetWeaponUsageIndex, comparedWeapon, comparedWeaponUsageIndex });
                    }
                addIntProperty.Invoke(__instance, new object[] { ____lengthText, weaponWithUsageIndex.WeaponLength, (weaponComponentData != null) ? new int?(weaponComponentData.WeaponLength) : null});
                TextObject handlingText = ____handlingText;
                equipmentElement = targetWeapon;
                addIntProperty.Invoke(__instance, new object[] { handlingText, equipmentElement.GetModifiedHandlingForUsage(targetWeaponUsageIndex), comparedWeapon.IsEmpty ? null : new int?(comparedWeapon.GetModifiedHandlingForUsage(comparedWeaponUsageIndex)) });
            }
                if (itemTypeFromWeaponClass == ItemObject.ItemTypeEnum.Thrown || itemTypeEnum == ItemObject.ItemTypeEnum.Thrown)
                {
                addIntProperty.Invoke(__instance, new object[] { ____weaponLengthText, weaponWithUsageIndex.WeaponLength, (weaponComponentData != null) ? new int?(weaponComponentData.WeaponLength) : null});
                addMissileDamageProperty.Invoke(__instance, new object[] { ____damageText, targetWeapon, targetWeaponUsageIndex, comparedWeapon, comparedWeaponUsageIndex });
                TextObject missileSpeedText = ____missileSpeedText;
                equipmentElement = targetWeapon;
                addIntProperty.Invoke(__instance, new object[] { missileSpeedText, equipmentElement.GetModifiedMissileSpeedForUsage(targetWeaponUsageIndex), comparedWeapon.IsEmpty ? null : new int?(comparedWeapon.GetModifiedMissileSpeedForUsage(comparedWeaponUsageIndex)) });
                addIntProperty.Invoke(__instance, new object[] { ____accuracyText, weaponWithUsageIndex.Accuracy, (weaponComponentData != null) ? new int?(weaponComponentData.Accuracy) : null });
                TextObject stackAmountText = ____stackAmountText;
                equipmentElement = targetWeapon;
                addIntProperty.Invoke(__instance, new object[] { stackAmountText, (int)equipmentElement.GetModifiedStackCountForUsage(targetWeaponUsageIndex), comparedWeapon.IsEmpty ? null : new int?((int)comparedWeapon.GetModifiedStackCountForUsage(comparedWeaponUsageIndex)) });
            }
            if (itemTypeFromWeaponClass == ItemObject.ItemTypeEnum.Shield || itemTypeEnum == ItemObject.ItemTypeEnum.Shield)
            {
                TextObject speedText = ____speedText;
                equipmentElement = targetWeapon;
                addIntProperty.Invoke(__instance, new object[] { speedText, equipmentElement.GetModifiedSwingSpeedForUsage(targetWeaponUsageIndex), comparedWeapon.IsEmpty ? null : new int?(comparedWeapon.GetModifiedSwingSpeedForUsage(comparedWeaponUsageIndex)) });
                TextObject hitPointsText = ____hitPointsText;
                equipmentElement = targetWeapon;
                addIntProperty.Invoke(__instance, new object[] { hitPointsText, (int)equipmentElement.GetModifiedMaximumHitPointsForUsage(targetWeaponUsageIndex), comparedWeapon.IsEmpty ? null : new int?((int)comparedWeapon.GetModifiedMaximumHitPointsForUsage(comparedWeaponUsageIndex)) });
                TextObject armorText = ____bodyArmorText;
                equipmentElement = targetWeapon;
                addIntProperty.Invoke(__instance, new object[] { armorText, (int)equipmentElement.GetModifiedBodyArmor(), comparedWeapon.IsEmpty ? null : new int?((int)comparedWeapon.GetModifiedBodyArmor()) });
            }
            if (itemTypeFromWeaponClass == ItemObject.ItemTypeEnum.Bow || itemTypeFromWeaponClass == ItemObject.ItemTypeEnum.Crossbow || itemTypeEnum == ItemObject.ItemTypeEnum.Bow || itemTypeEnum == ItemObject.ItemTypeEnum.Crossbow)
            {
                TextObject speedText2 = ____speedText;
                equipmentElement = targetWeapon;
                addIntProperty.Invoke(__instance, new object[] { speedText2, equipmentElement.GetModifiedSwingSpeedForUsage(targetWeaponUsageIndex), comparedWeapon.IsEmpty ? null : new int?(comparedWeapon.GetModifiedSwingSpeedForUsage(comparedWeaponUsageIndex)) });
                addThrustDamageProperty.Invoke(__instance, new object[] { ____damageText, targetWeapon, targetWeaponUsageIndex, comparedWeapon, comparedWeaponUsageIndex });
                addIntProperty.Invoke(__instance, new object[] { ____accuracyText, weaponWithUsageIndex.Accuracy, (weaponComponentData != null) ? new int?(weaponComponentData.Accuracy) : null });
                TextObject missileSpeedText2 = ____missileSpeedText;
                equipmentElement = targetWeapon;
                addIntProperty.Invoke(__instance, new object[] { missileSpeedText2, equipmentElement.GetModifiedMissileSpeedForUsage(targetWeaponUsageIndex), comparedWeapon.IsEmpty ? null : new int?(comparedWeapon.GetModifiedMissileSpeedForUsage(comparedWeaponUsageIndex)) });
                if (itemTypeFromWeaponClass == ItemObject.ItemTypeEnum.Crossbow || itemTypeEnum == ItemObject.ItemTypeEnum.Crossbow)
                {
                    TextObject ammoLimitText = ____ammoLimitText;
                    int maxDataValue = (int)weaponWithUsageIndex.MaxDataValue;
                    short? num = (weaponComponentData != null) ? new short?(weaponComponentData.MaxDataValue) : null;
                    addIntProperty.Invoke(__instance, new object[] { ammoLimitText, maxDataValue, (num != null) ? new int?((int)num.GetValueOrDefault()) : null });
                }
            }
            if (weaponWithUsageIndex.IsAmmo || (weaponComponentData != null && weaponComponentData.IsAmmo))
            {
                if ((itemTypeFromWeaponClass != ItemObject.ItemTypeEnum.Arrows && itemTypeFromWeaponClass != ItemObject.ItemTypeEnum.Bolts) || (weaponComponentData != null && itemTypeEnum != ItemObject.ItemTypeEnum.Arrows && itemTypeEnum != ItemObject.ItemTypeEnum.Bolts))
                {
                     addIntProperty.Invoke(__instance, new object[] { ____accuracyText, weaponWithUsageIndex.Accuracy, (weaponComponentData != null) ? new int?(weaponComponentData.Accuracy) : null });
                }
                addThrustDamageProperty.Invoke(__instance, new object[] { ____damageText, targetWeapon, targetWeaponUsageIndex, comparedWeapon, comparedWeaponUsageIndex });
                TextObject stackAmountText2 = ____stackAmountText;
                equipmentElement = targetWeapon;
                addIntProperty.Invoke(__instance, new object[] { stackAmountText2, (int)equipmentElement.GetModifiedStackCountForUsage(targetWeaponUsageIndex), comparedWeapon.IsEmpty ? null : new int?((int)comparedWeapon.GetModifiedStackCountForUsage(comparedWeaponUsageIndex)) });
            }
            equipmentElement = targetWeapon;
            ItemObject item = equipmentElement.Item;
            if (item == null || !item.HasBannerComponent)
            {
                ItemObject item2 = comparedWeapon.Item;
                if (item2 == null || !item2.HasBannerComponent)
                {
                    goto IL_717;
                }
            }
            Func<EquipmentElement, string> valueAsStringFunc = delegate (EquipmentElement x)
            {
                ItemObject item3 = x.Item;
                bool flag;
                if (item3 == null)
                {
                    flag = (null != null);
                }
                else
                {
                    BannerComponent bannerComponent = item3.BannerComponent;
                    flag = (((bannerComponent != null) ? bannerComponent.BannerEffect : null) != null);
                }
                if (flag)
                {
                    GameTexts.SetVariable("RANK", x.Item.BannerComponent.BannerEffect.Name);
                    string content = string.Empty;
                    if (x.Item.BannerComponent.BannerEffect.IncrementType == BannerEffect.EffectIncrementType.AddFactor)
                    {
                        TextObject obj = GameTexts.FindText("str_NUMBER_percent", null).SetTextVariable("NUMBER", ((int)Math.Abs(x.Item.BannerComponent.GetBannerEffectBonus() * 100f)).ToString());
                        content = obj.ToString();
                    }
                    else if (x.Item.BannerComponent.BannerEffect.IncrementType == BannerEffect.EffectIncrementType.Add)
                    {
                        content = x.Item.BannerComponent.GetBannerEffectBonus().ToString();
                    }
                    GameTexts.SetVariable("NUMBER", content);
                    return GameTexts.FindText("str_RANK_with_NUM_between_parenthesis", null).ToString();
                }
                return ____noneText.ToString();
            };
            var addComparableStringProperty = AccessTools.Method(typeof(ItemMenuVM), "AddComparableStringProperty");
            addComparableStringProperty.Invoke(__instance, new object[] { ____bannerEffectText, valueAsStringFunc, (EquipmentElement x) => 0 });
        IL_717:
            var addDonationXpTooltip = AccessTools.Method(typeof(ItemMenuVM), "AddDonationXpTooltip");
            addDonationXpTooltip.Invoke(__instance, new object[] { });
            return false;
        }
    }
}
