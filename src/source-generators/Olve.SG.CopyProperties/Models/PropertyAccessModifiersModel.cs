namespace Olve.SG.CopyProperties.Models;

public class PropertyAccessModifiersModel(
    PropertyAccessModifierModel? get,
    PropertyAccessModifierModel? set
)
{
    public PropertyAccessModifierModel? Get { get; } = get;
    public PropertyAccessModifierModel? Set { get; } = set;
}
