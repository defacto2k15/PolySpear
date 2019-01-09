namespace Assets.Scripts.Battle
{
    public enum EffectActivationCircumstances
    {
        DirectorBattle, StepBattle, Allways
    }

    public static class EffectActivationCircumstancesUtils
    {
        public static bool IsActivated(this EffectActivationCircumstances activationCircumstances, BattleCircumstances battleCircumstances)
        {
            if (activationCircumstances == EffectActivationCircumstances.Allways)
            {
                return true;
            }else if (activationCircumstances == EffectActivationCircumstances.StepBattle && battleCircumstances == BattleCircumstances.Step)
            {
                return true;
            }
            else if (activationCircumstances == EffectActivationCircumstances.DirectorBattle && battleCircumstances == BattleCircumstances.Director)
            {
                return true;
            }
            return false;
        }
    }
}