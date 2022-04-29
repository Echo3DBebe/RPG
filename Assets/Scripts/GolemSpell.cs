
public class GolemSpell : Spell
{
    public DamageTypeStruct DamageRange;

    public int Number;
    public int Alive;

    public bool Activated;

    public override bool Equals(Spell spell)
    {
        if (spell is GolemSpell gSpell)
        {
            if (DamageRange.Type == gSpell.DamageRange.Type &&
                DamageRange.LDamage == gSpell.DamageRange.LDamage &&
                DamageRange.HDamage == gSpell.DamageRange.HDamage &&
                Number == gSpell.Number &&
                Activated == false)
            {
                return base.Equals(spell);
            }
        }

        return false;
    }
}
