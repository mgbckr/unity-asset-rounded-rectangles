using UnityEngine;

/// <summary>
/// Hover as a material property: the shader property <c>_Hovered</c> (0 to 1) on a control or
/// key. Quest sets it while a pointer ray rests on the object and fades it in and out, as the
/// visionOS hover intensity does. visionOS never calls this and leaves it at 0, because there the
/// system draws hover through the PolySpatial hover state.
/// </summary>
public static class HoverTarget
{
    static readonly int HoveredId = Shader.PropertyToID("_Hovered");

    // Reused by every call; GetPropertyBlock overwrites its contents
    static MaterialPropertyBlock block;

    /// <summary>
    /// Sets <c>_Hovered</c> to 1 (on) or 0 at once on the hover renderer of <paramref name="target"/>
    /// (see <see cref="FindHoverRenderer"/>). Does nothing when it has none.
    /// </summary>
    public static void SetHovered(GameObject target, bool on)
    {
        SetHoverValue(target, on ? 1f : 0f);
    }

    /// <summary>
    /// Sets <c>_Hovered</c> to <paramref name="value"/> on the hover renderer of
    /// <paramref name="target"/> (see <see cref="FindHoverRenderer"/>): 0 not hovered, 1 hovered,
    /// values between for a fade. Does nothing when it has none.
    /// </summary>
    public static void SetHoverValue(GameObject target, float value)
    {
        SetHoverValue(FindHoverRenderer(target), value);
    }

    /// <summary>
    /// Sets <c>_Hovered</c> to <paramref name="value"/> on <paramref name="renderer"/>, for a renderer
    /// found with <see cref="FindHoverRenderer"/>. Writes a MaterialPropertyBlock, so no material
    /// instance is created; at 0 (not hovered) it removes the renderer's block instead, because a
    /// renderer with a block leaves URP's SRP Batcher. That assumes nothing else writes a block to
    /// a hover renderer (in this project only Vuplex sets blocks, on its own web view renderers),
    /// and that the material's own <c>_Hovered</c> is 0 (true for all of them). Does nothing when
    /// the renderer is missing or destroyed.
    /// </summary>
    public static void SetHoverValue(Renderer renderer, float value)
    {
        // Unity's == is also true for a destroyed object
        if (renderer == null)
        {
            return;
        }

        if (value <= 0f)
        {
            renderer.SetPropertyBlock(null);
            return;
        }

        block ??= new MaterialPropertyBlock();
        renderer.GetPropertyBlock(block);
        block.SetFloat(HoveredId, value);
        renderer.SetPropertyBlock(block);
    }

    /// <summary>
    /// The renderer whose <c>_Hovered</c> the setters write for <paramref name="target"/>: the first
    /// Renderer on it or else on its nearest parent that has one. Null when the target is missing
    /// or destroyed, no renderer is found, or the renderer's material has no <c>_Hovered</c>.
    /// </summary>
    public static Renderer FindHoverRenderer(GameObject target)
    {
        // Unity's == is also true for a destroyed object
        if (target == null)
        {
            return null;
        }

        Renderer renderer = FindRenderer(target.transform);
        if (renderer == null)
        {
            return null;
        }

        Material material = renderer.sharedMaterial;
        return material != null && material.HasProperty(HoveredId) ? renderer : null;
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
