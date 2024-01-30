using Game.Companies;
using Game.Economy;
using Game.Prefabs;
using Game.Simulation;
using HarmonyLib;
using Unity.Entities;
using Unity.Mathematics;

namespace CompanyProfitFix.Patches
{
    [HarmonyPatch(typeof(ProcessingCompanySystem), "EstimateDailyProfit")]
    public static class ProcessingCompanySystemPatches
    {
        [HarmonyPatch([typeof(int), typeof(int), typeof(IndustrialProcessData), typeof(EconomyParameterData), typeof(DynamicBuffer<TradeCost>), typeof(ResourcePrefabs), typeof(ComponentLookup<ResourceData>)], [ArgumentType.Normal, ArgumentType.Normal, ArgumentType.Normal, ArgumentType.Ref, ArgumentType.Normal, ArgumentType.Normal, ArgumentType.Normal])]
        [HarmonyPrefix]
        public static bool Prefix(int production, int wages, IndustrialProcessData processData, ref EconomyParameterData economyParameters, DynamicBuffer<TradeCost> tradeCost, ResourcePrefabs resourcePrefabs, ComponentLookup<ResourceData> resourceDatas, ref int __result)
        {
            TradeCost tradeCost2 = EconomyUtils.GetTradeCost(processData.m_Output.m_Resource, tradeCost);
            float3 tradeCost3 = default;
            tradeCost3.x = 0f;
            if (float.IsNaN(tradeCost2.m_BuyCost))
            {
                tradeCost3.x = tradeCost2.m_SellCost - 0f;
            }
            else if (float.IsNaN(tradeCost2.m_SellCost))
            {
                tradeCost3.x = 0f - tradeCost2.m_BuyCost;
            }
            else
            {
                tradeCost3.x = tradeCost2.m_SellCost - tradeCost2.m_BuyCost;
            }
            tradeCost3.y = EconomyUtils.GetTradeCost(processData.m_Input1.m_Resource, tradeCost).m_BuyCost;
            tradeCost3.z = EconomyUtils.GetTradeCost(processData.m_Input2.m_Resource, tradeCost).m_BuyCost;

            // Fixed calculate profit
            __result = ProcessingCompanySystem.EstimateDailyProfit(production, wages, processData, ref economyParameters, tradeCost3, resourcePrefabs, resourceDatas);

            // Do not execute original method
            return false;
        }
    }

    [HarmonyPatch(typeof(TransportBoardingHelpers))]
    public static class TransferFixPatch
    {

    }
}
