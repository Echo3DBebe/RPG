using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStatsUi : IUi
{
    [SerializeField] private GameObject StatsUi;

    [SerializeField] private Transform StatsListHolder;

    [SerializeField] private Text StatTextPrefab;
    [SerializeField] private Text BannerPrefab;

    [SerializeField] private GameObject SkillTextPrefab;

    /*------When button pressed-------
    
    Call TurnItemDetailsOff()
    Clear Inventory holder
    Call StatsUi.SetActive(true)
    Call inventoryUi.SetActive(false)
    Call QuestsUi.SetActive(false)
    */

    public override void Set()
    {
        if (isActive)
        {
            return;
        }

        isActive = true;

        StatsUi.SetActive(true);

        Text text;
        Image bar;

        #region CraeteNameText
        text = Instantiate(StatTextPrefab, StatsListHolder);
        text.text = WorldStateTracker.Tracker.PlayerName;
        text.fontSize = 40;
        text.alignment = TextAnchor.MiddleCenter;

        text = Instantiate(SkillTextPrefab, StatsListHolder).GetComponent<Text>();
        bar = text.transform.GetChild(0).GetChild(0).GetComponent<Image>();
        #endregion

        #region CreateLevelText/LevelBar
        StringBuilder sb = new StringBuilder("Level: ");
        sb.Append(Player.player.GetLevel());

        text.text = sb.ToString();

        if (Player.player.GetStoredLevels() >= 1)
        {
            bar.fillAmount = 1;
        }
        else
        {
            bar.fillAmount = (float)Player.player.GetLevelProgress() * .1f;
        }

        sb.Clear();
        #endregion

        text = Instantiate(BannerPrefab, StatsListHolder).GetComponent<Text>();
        text.text = "Skills\n___________________________________________________";

        #region CreateSkillTexts/SkillBars
        for (int i = 0; i < 16; i++)
        {
            sb.Clear();

            text = Instantiate(SkillTextPrefab, StatsListHolder).GetComponent<Text>();
            bar = text.transform.GetChild(0).GetChild(0).GetComponent<Image>();

            string name = ((SkillType)i).ToString();

            for (int x = 0; x < name.Length; x++)
            {
                char c = name[x];

                if (char.IsUpper(c) && x != 0)
                {
                    name = name.Insert(x, " ");
                    x++;
                }
            }

            sb.Append(name);
            sb.Append(": ");
            sb.Append(Player.player.GetSkillLevel(i)); ;

            text.text = sb.ToString();
            bar.fillAmount = (float)Player.player.GetSkillExp(i) / Player.player.GetSkillRExp(i);
        }
        #endregion

        text = Instantiate(BannerPrefab, StatsListHolder);
        text.text = "Masteries\n___________________________________________________";

        #region CreateMasteryTexts/MasteryBars
        for (int i = 0; i < 10; i++)
        {
            sb.Clear();

            text = Instantiate(SkillTextPrefab, StatsListHolder).GetComponent<Text>();
            bar = text.transform.GetChild(0).GetChild(0).GetComponent<Image>();

            string name = ((MasteryType)i).ToString();

            for (int x = 0; x < name.Length; x++)
            {
                char c = name[x];

                if (char.IsUpper(c) && x != 0)
                {
                    name = name.Insert(x, " ");
                    x++;
                }
            }

            sb.Append(name);
            sb.Append(": ");
            sb.Append(Player.player.GetMasteryLevel(i));

            text.text = sb.ToString();
            bar.fillAmount = (float)Player.player.GetMasteryExp(i) / Player.player.GetMasteryRExp(i);
        }
        #endregion

        text = Instantiate(BannerPrefab, StatsListHolder);
        text.text = "Abilities\n___________________________________________________";

        #region CreateAbilityTexts
        for (int i = 0; i < 3; i++)
        {
            text = Instantiate(StatTextPrefab, StatsListHolder);

            sb.Clear();
            sb.Append(((Abilities)i).ToString());
            sb.Append(": ");
            sb.Append(Player.player.GetAbility(i));

            text.text = sb.ToString();
        }
        #endregion

        text = Instantiate(BannerPrefab, StatsListHolder);
        text.text = "Defenses\n___________________________________________________";

        #region CreateArmourText
        text = Instantiate(StatTextPrefab, StatsListHolder);

        sb.Clear();
        sb.Append("Armour: ");
        sb.Append(Player.player.GetArmour());

        text.text = sb.ToString();
        #endregion

        #region CreateFireResistenceText
        text = Instantiate(StatTextPrefab, StatsListHolder);

        sb.Clear();

        sb.Append("Fire Resistance: ");
        sb.Append(Player.player.GetFireResistence());
        sb.Append("%");

        text.text = sb.ToString();
        #endregion

        #region CreateLightningRessitenceText
        text = Instantiate(StatTextPrefab, StatsListHolder);

        sb.Clear();

        sb.Append("Lightning Resistance: ");
        sb.Append(Player.player.GetLightningResistence());
        sb.Append("%");

        text.text = sb.ToString();
        #endregion

        #region CreateIceResistenceText
        text = Instantiate(StatTextPrefab, StatsListHolder);

        sb.Clear();

        sb.Append("Ice Resistance: ");
        sb.Append(Player.player.GetIceResistence());
        sb.Append("%");

        text.text = sb.ToString();
        #endregion

        for (int HandType = 0; HandType < 2; HandType++)
        {
            sb.Clear();

            Item heldItem = Player.player.GetHeldItem(HandType);
            DamageStats stats = Player.player.GetStats(HandType);

            if (heldItem != null)
            {
                continue;
            }
            
            text = Instantiate(BannerPrefab, StatsListHolder);

            if (HandType == 0)
            {
                text.text = "Right Hand\n___________________________________________________";
            }
            else
            {
                if (Player.player.GetHeldItem(0) == Player.player.GetHeldItem(1))
                {
                    Destroy(text);
                    continue;
                }

                text.text = "Left Hand\n___________________________________________________";
            }

            switch (heldItem.tag)
            {
                case GlobalValues.WeaponTag:

                    WeaponHolder weapon = heldItem.GetComponent<WeaponHolder>();

                    for (int x = 0; x < weapon.GetDamageRangesCount(); x++)
                    {
                        text = Instantiate(StatTextPrefab, StatsListHolder);

                        sb.Append(weapon.GetDamageType(x).ToString());
                        sb.Append(": ");
                        sb.Append(weapon.GetLowerRange(x).ToString("n0"));
                        sb.Append(" to ");
                        sb.Append(weapon.GetUpperRange(x).ToString("n0"));

                        text.text = sb.ToString();
                        sb.Clear();
                    }

                    text = Instantiate(StatTextPrefab, StatsListHolder);

                    WeaponHolder tempWeapon = weapon as WeaponHolder;

                    sb.Append("Attacks Per Secound: ");
                    sb.Append(tempWeapon.GetAttackSpeed().ToString("0.00"));

                    text.text = sb.ToString();

                    sb.Clear();

                    text = Instantiate(StatTextPrefab, StatsListHolder);

                    sb.Append("Life Steal: ");
                    sb.Append(tempWeapon.GetLifeSteal());
                    sb.Append("%");

                    text.text = sb.ToString();

                    for (int i = 0; i < weapon.GetDamageRangesCount(); i++)
                    {
                        SetStatsStats(weapon, i, weapon.GetDamageType(i));
                    }

                    float[] multi;

                    text = Instantiate(BannerPrefab, StatsListHolder);
                    text.text = "Melee Damage Multipliers\n___________________________________________";

                    multi = Player.player.GetMeleeMultis(HandType);

                    for (int x = 0; x < weapon.GetDamageRangesCount(); x++)
                    {
                        text = Instantiate(StatTextPrefab, StatsListHolder);

                        sb.Clear();

                        sb.Append(weapon.GetDamageType(x).ToString());
                        sb.Append(" Damage: ");
                        sb.Append(multi[(int)weapon.GetDamageType(x)]);

                        text.text = sb.ToString();

                    }
                    break;
                case GlobalValues.SpellTag:

                    break;
                default:
                    break;
            }
        }
    }

    public override void Clear()
    {
        if (!isActive)
        {
            return;
        }

        isActive = false;

        int loops = StatsListHolder.childCount;

        for (int i = 0; i < loops; i++)
        {
            Destroy(StatsListHolder.GetChild(i).gameObject);
        }

        StatsUi.SetActive(false);
    }

    public override void Close()
    {
        StatsUi.SetActive(false);
    }

    private void SetStatsStats(WeaponHolder weapon, int id, DamageTypeEnum type)
    {
        switch (type)
        {
            case DamageTypeEnum.Physical:
                DisplayPhysStats(weapon.GetStatus(id), weapon.GetCrit());
                break;
            default:
                break;
        }
    }

    private void SetStatsStats(Spell spell, int id, DamageTypeEnum type)
    {
        int chance = 0;
        int damage = 0;

        switch (spell.GetSpellType())
        {
            case SpellType.DamageSpell:
                DamageSpell dSpell = spell as DamageSpell;
                damage = dSpell.GetCritDamage();
                chance = dSpell.GetStatusChance(id);
                break;
            default:
                break;
        }

        switch (type)
        {
            case DamageTypeEnum.Physical:
                DisplayPhysStats(chance, damage);
                break;
            case DamageTypeEnum.Fire:
                DisplayFireStats(chance);
                break;
            case DamageTypeEnum.Lightning:
                DisplayLightningStats(chance);
                break;
            case DamageTypeEnum.Ice:
                DisplayIceStats(chance);
                break;
            default:
                break;
        }

    }

    private void DisplayPhysStats(int statusChance, int crit)
    {
        StringBuilder sb = new StringBuilder();

        Text text = Instantiate(BannerPrefab, StatsListHolder);
        text.text = "Critical\n___________________________________________";

        text = Instantiate(StatTextPrefab, StatsListHolder);

        sb.Append("Critical Chance: ");
        sb.Append(statusChance);
        sb.Append("%");

        text.text = sb.ToString();

        text = Instantiate(StatTextPrefab, StatsListHolder);

        sb.Clear();

        sb.Append("Critital Damage: ");
        sb.Append(crit);
        sb.Append("%");

        text.text = sb.ToString();
    }

    private void DisplayFireStats(int statsChance)
    {
        StringBuilder sb = new StringBuilder();

        Text text = Instantiate(BannerPrefab, StatsListHolder);
        text.text = "Burn\n___________________________________________";

        text = Instantiate(StatTextPrefab, StatsListHolder);

        sb.Append("Burn Chance: ");
        sb.Append(statsChance);
        sb.Append("%");

        text.text = sb.ToString();

        text = Instantiate(StatTextPrefab, StatsListHolder);

        sb.Clear();

        sb.Append("Burn Damage: ");
        sb.Append(Player.player.GetBurnDamage());
        sb.Append("%");

        text.text = sb.ToString();

        text = Instantiate(StatTextPrefab, StatsListHolder);

        sb.Clear();

        sb.Append("Burn Ticks: ");
        sb.Append(Player.player.GetTicks());

        text.text = sb.ToString();

        text = Instantiate(StatTextPrefab, StatsListHolder);

        sb.Clear();

        sb.Append("Time Between Burn Ticks: ");
        sb.Append(Mathf.Round(Player.player.GetWaitTime() * 100) * .01f);
        sb.Append("s");

        text.text = sb.ToString();
    }

    private void DisplayLightningStats(int statsChance)
    {
        StringBuilder sb = new StringBuilder();
        Text text = Instantiate(BannerPrefab, StatsListHolder);
        text.text = "Chain\n___________________________________________";

        text = Instantiate(StatTextPrefab, StatsListHolder);

        sb.Append("Chain Chance: ");
        sb.Append(statsChance);
        sb.Append("%");

        text.text = sb.ToString();

        text = Instantiate(StatTextPrefab, StatsListHolder);

        sb.Clear();

        sb.Append("Chain Damage: ");
        sb.Append(Player.player.GetChainDamage());
        sb.Append("%");

        text.text = sb.ToString();

        text = Instantiate(StatTextPrefab, StatsListHolder);

        sb.Clear();

        sb.Append("Number Of Chains: ");
        sb.Append(Player.player.GetChains());

        text.text = sb.ToString();

        text = Instantiate(StatTextPrefab, StatsListHolder);

        sb.Clear();

        sb.Append("Chain Lenght: ");
        sb.Append(Player.player.GetChainLength());

        text.text = sb.ToString();
    }

    private void DisplayIceStats(int statsChance)
    {
        StringBuilder sb = new StringBuilder();

        Text text = Instantiate(BannerPrefab, StatsListHolder);
        text.text = "Chill\n___________________________________________";

        text = Instantiate(StatTextPrefab, StatsListHolder);

        sb.Append("Chill Chance: ");
        sb.Append(statsChance);
        sb.Append("%");

        text.text = sb.ToString();

        text = Instantiate(StatTextPrefab, StatsListHolder);

        sb.Clear();

        sb.Append("Chill Affect: ");
        sb.Append(Player.player.GetChillAffect());
        sb.Append("% Reduced Action Speed");

        text.text = sb.ToString();

        sb.Clear();

        text = Instantiate(StatTextPrefab, StatsListHolder);

        sb.Append("Chill Duration: ");
        sb.Append(Mathf.Round(Player.player.GetChillDuration() * 100) / 100);
        sb.Append("s");

        text.text = sb.ToString();

        sb.Clear();
    }
}
