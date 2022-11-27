using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using Microsoft.Xna.Framework.Graphics;
using Terraria.UI;
using Terraria.GameContent.UI.States;
using Terraria.GameContent.UI.Elements;
using Terraria.ModLoader.Config;
using Terraria.ModLoader.Config.UI;
using System.ComponentModel;
using System.Reflection;
using Newtonsoft.Json;

namespace SPIC.Configs.UI;



public interface IDictionaryEntryWrapper {
    [JsonIgnore] public PropertyInfo ValueProp { get; }
    object Key { get; set; }
    object? Value { get; set; }
}

public class DictionaryEntryWrapper<Tkey, Tvalue> : IDictionaryEntryWrapper where Tkey : notnull{
    public PropertyInfo ValueProp => typeof(DictionaryEntryWrapper<Tkey, Tvalue>).GetProperty(nameof(Value), BindingFlags.Instance | BindingFlags.Public)!;

    public Tkey Key {
        get => _key;
        set {
            if (_dict is IOrderedDictionary ordered) {
                int index = 0;
                foreach (DictionaryEntry entry in ordered) {
                    if (entry.Key.Equals(_key)) break;
                    index++;
                }
                ordered.RemoveAt(index);
                ordered.Insert(index, value, _dict[_key]);
            } else {
                _dict.Remove(_key);
                _dict.Add(value, _dict[_key]);
            }

            _key = value;
        }
    }

    [ColorNoAlpha, ColorHSLSlider]
    public Tvalue? Value {
        get => (Tvalue?)_dict[_key];
        set {
            _dict[_key] = value;
        }
    }

    object IDictionaryEntryWrapper.Key {
        get => Key;
        set => Key = (Tkey)value;
    }
    object? IDictionaryEntryWrapper.Value {
        get => Value;
        set => Value = (Tvalue?)value;
    }

    public DictionaryEntryWrapper(IDictionary dict, Tkey key) {
        _key = key;
        _dict = dict;
    }

    private readonly IDictionary _dict;
    private Tkey _key;
}


[AttributeUsage(AttributeTargets.Class | AttributeTargets.Property | AttributeTargets.Field)]
public class ConstantKeys : Attribute { }

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Property | AttributeTargets.Field)]
public class ValuesAsConfigItemsAttribute : Attribute { }

public class CustomDictionaryElement : ConfigElement<IDictionary> {

    // TODO implement attributes to finish class
    // private ConstantKeys _constKeys; // - add, clear, remove
    // private ValuesAsConfigItemsAttribute _asConfigItems; // - collapse, size button, (add, clear, remove)

    // private FieldInfo _dictWrappersField = typeof(CustomDictionaryUI).GetField(nameof(dictWrappers), BindingFlags.Instance | BindingFlags.Public);

    // private Attribute[] _memberAttributes;

    private static readonly FieldInfo s_dummyField = typeof(CustomDictionaryElement).GetField(nameof(_dummy), BindingFlags.NonPublic | BindingFlags.Instance)!;
    private readonly EmptyClass _dummy = new();


    public List<IDictionaryEntryWrapper> dictWrappers = new();
    private readonly UIList _dataList = new();
    public override void OnBind() {
        base.OnBind();

        object value = Value;
        if(value is null) throw new ArgumentNullException("This config element only supports IDictionaries");

        // _memberAttributes = Attribute.GetCustomAttributes(MemberInfo.MemberInfo).Where(attrib => attrib is not CustomModConfigItemAttribute).ToArray();

        // _hideUnloaded = ConfigManager.GetCustomAttribute<HideUnloadedAttribute>(MemberInfo, value.GetType());
        // _asConfigItems = ConfigManager.GetCustomAttribute<ValuesAsConfigItemsAttribute>(MemberInfo, value.GetType());
        // _constKeys = ConfigManager.GetCustomAttribute<ConstantKeys>(MemberInfo, value.GetType());
        // _sortable = ConfigManager.GetCustomAttribute<SortableAttribute>(MemberInfo, value.GetType());

        _dataList.Top = new(0, 0f);
        _dataList.Left = new(0, 0f);
        _dataList.Height = new(-5, 1f);
        _dataList.Width = new(0, 1f);
        _dataList.ListPadding = 5f;
        _dataList.PaddingBottom = -5f;

        Append(_dataList);

        SetupList();
    }
 
    // TODO Add attributes to childrens
    public void SetupList(){
        _dataList.Clear();
        dictWrappers.Clear();

        int unloaded = 0;

        IDictionary dict = Value;
        int top = 0;
        int i = -1;
        foreach (DictionaryEntry entry in dict) {
            i++;
            (object key, object? value) = entry;
            if(value is null) continue;
            if(key is ConsumableTypeDefinition entity && entity.IsUnloaded){
                unloaded++;
                continue;
            }
            Type genericType = typeof(DictionaryEntryWrapper<,>).MakeGenericType(key.GetType(), value.GetType());
            IDictionaryEntryWrapper wrapper = (IDictionaryEntryWrapper)Activator.CreateInstance(genericType, dict, key)!;

            dictWrappers.Add(wrapper);
            (UIElement container, UIElement element) = ConfigManager.WrapIt(_dataList, ref top, new(wrapper.ValueProp), wrapper, i);
                // ConfigManager.WrapIt(_dataList, ref top, new(_dictWrappersField), this, 0, _dictWrappers, genericType, i);
                
            if(dict is IOrderedDictionary){
                
                element.Width.Pixels -= 25;
                element.Left.Pixels += 25;

                if (element.GetType() == ReflectionHelper.ObjectElement) {
                    ReflectionHelper.ObjectElement_expanded.SetValue(element, false);
                    ReflectionHelper.ObjectElement_pendindChanges.SetValue(element, false);
                }

                int index = i;

                HoverImageSplit moveButton = new(UpDownTexture, "Move up", "Move down") {
                    VAlign = 0.5f,
                    Left = new(2, 0f)
                };
                moveButton.OnClick += (UIMouseEvent a, UIElement b) => {
                    IOrderedDictionary ordered = (IOrderedDictionary)Value;
                    if (moveButton.HoveringUp ? index <= 0 : index >= dict.Count - 1) return;
                    ordered.Move(index, index + (moveButton.HoveringUp ? -1 : 1));
                    SetupList();
                    ConfigManager.SetPendingChanges();
                };
                container.Append(moveButton);
            }

            string? name = key switch {
                ConsumableTypeDefinition type => type.Label(),
                ItemDefinition item => $"[i:{item.Type}] {item.Name}",
                EntityDefinition def => def.Name,
                _ => key.ToString()
            };
            ReflectionHelper.ConfigElement_TextDisplayFunction.SetValue(element, () => name);
        }
        if(unloaded > 0){
            (UIElement container, UIElement element) = ConfigManager.WrapIt(_dataList, ref top, new(s_dummyField), this, i);
            string text = $"{unloaded} unloaded consumable types";
            element.RemoveAllChildren();
            ReflectionHelper.ObjectElement_pendindChanges.SetValue(element, false);
            ReflectionHelper.ConfigElement_TextDisplayFunction.SetValue(element, () => text);

        }
        Recalculate();
    }

    public override void Recalculate() {
        base.Recalculate();
        int defaultHeight = _dataList.Count > 1 ? -5 : 0;
        float h = (_dataList.Parent != null) ? (_dataList.GetTotalHeight() + defaultHeight) : defaultHeight;
        Height.Set(h, 0f);
        if (Parent != null && Parent is UISortableElement) Parent.Height.Set(h, 0f);
        MaxHeight.Pixels = int.MaxValue;
    }

    public override void Draw(SpriteBatch spriteBatch) {
        DrawChildren(spriteBatch);
    }
}