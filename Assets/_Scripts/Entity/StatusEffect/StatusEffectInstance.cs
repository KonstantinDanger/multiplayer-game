public class StatusEffectInstance
{
    public readonly StatusEffect Effect;
    public readonly IStatusEffectPresentationData Presentation;

    private ScriptableStatusEffect _scriptable;

    public StatusEffectInstance(ScriptableStatusEffect scriptableStatusEffect, StatusEffect instance)
    {
        Effect = instance;
        Presentation = scriptableStatusEffect;

        _scriptable = scriptableStatusEffect;
    }

    public override string ToString() => _scriptable.name + " | " + Effect;
}
