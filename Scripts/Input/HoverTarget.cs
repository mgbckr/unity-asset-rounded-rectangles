using UnityEngine;

/// <summary>
/// Hover as a material property: the shader property <c>_Hovered</c> (0 or 1) on a control or
/// key. Quest sets it while a pointer ray rests on the object. visionOS never calls this and
/// leaves it at 0, because there the system draws hover through the PolySpatial hover state.
/// </summary>
public static class HoverTarget
{
    static readonly int HoveredId = Shader.PropertyToID("_Hovered");

    // Reused by every call; GetPropertyBlock overwrites its contents
    static MaterialPropertyBlock block;

    /// <summary>
    /// Sets <c>_Hovered</c> to 1 (on) or 0 on the first Renderer found on <paramref name="target"/>
    /// or its parents. Writes a MaterialPropertyBlock, so no material instance is created.
    /// Does nothing when the target is missing or destroyed, no renderer is found, or the
    /// renderer's material has no <c>_Hovered</c>.
    /// </summary>
    public static void SetHovered(GameObject target, bool on)
    {
        // Unity's == is also true for a destroyed object
        if (target == null)
        {
            return;
        }

        Renderer renderer = FindRenderer(target.transform);
        if (renderer == null)
        {
            return;
        }

        Material material = renderer.sharedMaterial;
        if (material == null || !material.HasProperty(HoveredId))
        {
            return;
        }

        block ??= new MaterialPropertyBlock();
        renderer.GetPropertyBlock(block);
        block.SetFloat(HoveredId, on ? 1f : 0f);
        renderer.SetPropertyBlock(block);
    }

    // The renderer on the object itself, else on the nearest parent that has one, up to the scene root
    static Renderer FindRenderer(Transform transform)
    {
        for (Transform t = transform; t != null; t = t.parent)
        {
            if (t.TryGetComponent(out Renderer renderer))
            {
                return renderer;
            }
        }

        return null;
    }
}
