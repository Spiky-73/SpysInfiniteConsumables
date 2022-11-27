namespace SPIC.ConsumableGroup;

// ? add generic type
public interface IRequirement {
    ICount NextRequirement(ICount count);
    Infinity Infinity(ICount count);
}

public sealed class NoRequirement : IRequirement {
    public Infinity Infinity(ICount count) => new(count.None, 0);
    public ICount NextRequirement(ICount count) => count.None;
}

public abstract class FixedRequirement : IRequirement {
    public FixedRequirement(ICount root, float multiplier) {
        Multiplier = multiplier;
        Root = root;
    }

    public float Multiplier { get; init; }
    public ICount Root { get; init; }

    public abstract ICount EffectiveRequirement(ICount count);
    public Infinity Infinity(ICount count) => new(EffectiveRequirement(count), Multiplier);
    public ICount NextRequirement(ICount count) => Root.CompareTo(count) > 0 ? Root.AdaptTo(count) : count.None;
}

public abstract class RecursiveRequirement : IRequirement {
    protected RecursiveRequirement(ICount root, float multiplier) {
        Multiplier = multiplier;
        Root = root;
    }

    public float Multiplier { get; init; }
    public ICount Root { get; init; }

    public ICount EffectiveRequirement(ICount count){
        ICount effective = count.None;
        ICount next = Root.AdaptTo(count);
        while(next.CompareTo(count) <= 0){
            effective = next;
            next = NextValue(next);
        }
        return effective;
    }

    public Infinity Infinity(ICount count) => new(EffectiveRequirement(count), Multiplier);
    protected abstract ICount NextValue(ICount value);
    public ICount NextRequirement(ICount count) => count.IsNone ? Root.AdaptTo(count) : NextValue(count);
}

