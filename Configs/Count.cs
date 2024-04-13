using System;
using System.ComponentModel;
using SpikysLib.Configs;
using Terraria.ModLoader.Config;

namespace SPIC.Configs;

public class ToFromStringConverterFix<T> : ToFromStringConverter<T> {}

[TypeConverter(typeof(ToFromStringConverterFix<Count>))]
public class Count : MultiChoice<int> {

    public Count() : base() {}
    public Count(int value) : base(value) {}

    [Choice] public Text Disabled => new();
    [Choice, Range(1, 9999)] public int Amount { get; set; } = 999;

    public override int Value {
        get => Choice == nameof(Disabled) ? 0 : Amount;
        set {
            if (value != 0) {
                Choice = nameof(Amount);
                Amount = value;
            } else Choice = nameof(Disabled);
        }
    }

    public static implicit operator Count(int count) => new(count);
    public static Count FromString(string s) => new(int.Parse(s));

    public override bool Equals(object? obj) => obj is Count c && this == c;
    public override int GetHashCode() => Value.GetHashCode();

    public static bool operator ==(Count a, Count b) => a.Value == b.Value;
    public static bool operator !=(Count a, Count b) => !(a == b);
}

public class Count<TCategory> : Count where TCategory : struct, System.Enum {
    public Count() : base() { }
    public Count(int value) : base(value) { }
    public Count(TCategory category) : base(-System.Convert.ToInt32(category)) { }

    [Choice]
    public TCategory Category { get; set; } = default;

    public override int Value {
        get => Choice == nameof(Category) ? -System.Convert.ToInt32(Category) : base.Value;
        set {
            if (value < 0) {
                Choice = nameof(Category);
                Category = System.Enum.Parse<TCategory>((-value).ToString());
            } else base.Value = value;
        }
    }
}