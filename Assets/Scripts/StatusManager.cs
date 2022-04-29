using UnityEngine;

public class StatusManager : MonoBehaviour
{
    [SerializeField] private FireStatusManager fireManager;

    [SerializeField] private LightningStatusManager lightningManager;


    [SerializeField] private LivingEntities self;

    public void OnEnable()
    {
        self = GetComponent<LivingEntities>();
    }

    public void RunCalculs()
    {
        fireManager.RunCalc(self.GetSkillLevel((int)SkillType.Pyromancy));
        lightningManager.RunCalc(self.GetSkillLevel((int)SkillType.Astromancy));
    }

    public void StartBurning(LivingEntities target, int damage)
    {
        //Debug.Log("Burning status called " + gameObject.ToString());
        fireManager.StartBurning(target, damage);
    }

    public void StartChain(LivingEntities target, int damage)
    {
        lightningManager.StartChain(target, damage);
    }
}
