using Godot;
using System;

public class Tooth : StaticBody
{
    private Gradient _tooth_decay_gradient;
    
    protected float _health = 1.0f;
    
    public override void _Ready()
    {
        SetupGradient();
        SetupMaterial();
    }
    
    public override void _Process(float delta)
    {
        UpdateColor();
    }
    
    private void SetupGradient()
    {
        // Workaround becuase it seems like C# gradient exports are broken
        _tooth_decay_gradient = new Gradient();
        
        _tooth_decay_gradient.SetColor(0, new Color("ffffff"));
        _tooth_decay_gradient.SetOffset(0, 0.0f);
        
        _tooth_decay_gradient.SetColor(1, new Color("a8a62b"));
        _tooth_decay_gradient.SetOffset(1, 0.627f);
        
        _tooth_decay_gradient.AddPoint(1.0f, new Color("73600c"));
    }
    
    private void SetupMaterial()
    {
        var model = GetNode<CSGSphere>("Model");
        var material = model.Material as SpatialMaterial;
        var new_material = new SpatialMaterial();
        
        new_material = new SpatialMaterial();
        new_material.ParamsDiffuseMode = material.ParamsDiffuseMode;
        new_material.ParamsSpecularMode = material.ParamsSpecularMode;
        new_material.Roughness = material.Roughness;
        
        model.Material = new_material;
    }
    
    public Color GetColor()
    {
        return _tooth_decay_gradient.Interpolate(1.0f - _health);
    }
    
    public void UpdateColor()
    {
        SpatialMaterial sm = GetNode<CSGSphere>("Model").Material as SpatialMaterial;
        sm.AlbedoColor = GetColor();
    }
}
