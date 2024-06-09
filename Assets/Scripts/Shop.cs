using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Shop : MonoBehaviour
{
    public GameManager manager;

    public List<float> depthUpgrade = new List<float>();
    public List<int> depthCost = new List<int>();
    public int depthTier = 0;

    public List<float> cdUpgrade = new List<float>();
    public List<int> cdCost = new List<int>();
    public int cdTier = 0;

    public List<float> speedUpgrade = new List<float>();
    public List<int> speedCost = new List<int>();
    public int speedTier = 0;

    public TMP_Text text_depth_upgrade;
    public TMP_Text text_depth_cost;

    public TMP_Text text_cd_upgrade;
    public TMP_Text text_cd_cost;

    public TMP_Text text_speed_upgrade;
    public TMP_Text text_speed_cost;

    void Start()
    {
        ShopUI(manager.wellCamera.GetComponent<CameraControls>().maxDepth, depthTier, depthUpgrade, depthCost, text_depth_upgrade, text_depth_cost);
        ShopUI(manager.fishingRod.GetComponent<FishingRodMovement>().dropSpeed, speedTier, speedUpgrade, speedCost, text_speed_upgrade, text_speed_cost);
        ShopUI(manager.wellCamera.GetComponent<CameraControls>().flashCd, cdTier, cdUpgrade, cdCost, text_cd_upgrade, text_cd_cost);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void UpgradeStat(ref float stat, ref int tier, ref List<float> upgrade, ref List<int> cost, ref TMP_Text textUprade, ref TMP_Text textCost)
    {
        if(tier < 3)
        {
            if (manager.currency >= cost[tier])
            {
                manager.a_button.Play();
                manager.EventChangeCurrency(-cost[tier]);
                stat += upgrade[tier];
                tier++;
            }

            if (tier == 3)
            {
                textUprade.text = "Cannot be improved further";
                textCost.text = "ERROR";
            }
            else
            {
                ShopUI(stat, tier, upgrade, cost, textUprade, textCost);
            }
        }
    }

    private static void ShopUI(float stat, int tier, List<float> upgrade, List<int> cost, TMP_Text textUprade, TMP_Text textCost)
    {
        textCost.text = cost[tier].ToString();
        textUprade.text = stat.ToString() + " + " + upgrade[tier].ToString();
    }

    public void UpgradeDepth()
    {
        UpgradeStat(ref manager.wellCamera.GetComponent<CameraControls>().maxDepth, ref depthTier, ref depthUpgrade, ref depthCost, ref text_depth_upgrade, ref text_depth_cost);
    }

    public void UpgradeRodSpeed()
    {
        UpgradeStat(ref manager.fishingRod.GetComponent<FishingRodMovement>().dropSpeed, ref speedTier, ref speedUpgrade, ref speedCost, ref text_speed_upgrade, ref text_speed_cost);
    }

    public void UpgradeCD()
    {
        UpgradeStat(ref manager.wellCamera.GetComponent<CameraControls>().flashCd, ref cdTier, ref cdUpgrade, ref cdCost, ref text_cd_upgrade, ref text_cd_cost); 
    }

    
}
