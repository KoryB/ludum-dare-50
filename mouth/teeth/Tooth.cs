using Godot;
using System;
using System.Linq;

public class Tooth : StaticBody
{
    public static float RotStrength = 0.25f;

    [Signal]
    public delegate void OnDeath();

    private Gradient _tooth_decay_gradient;
    
    [Export]
    protected float _max_health = 10.0f;
    protected float _health;
    
    protected bool _is_dead = false;
    
    protected Area _area;
    
    public override void _Ready()
    {
        _health = _max_health;
        _area = GetNode<Area>("Area");
    
        SetupGradient();
        SetupMaterial();
    }
    
    public override void _Process(float delta)
    {
        UpdateColor();
        UpdateDeath();
        RotNeighbors(delta);
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
    
    private void UpdateDeath()
    {
        if (_health <= 0.0)
        {
            EmitSignal(nameof(OnDeath));
            _is_dead = true;
        }
    }
    
    private void RotNeighbors(float delta)
    {
        if (_is_dead)
        {
            var neighbors = _area.GetOverlappingBodies()
                .Cast<Godot.Object>()
                .Where(x => x is Tooth)
                .Cast<Tooth>();
                
            foreach (Tooth neighbor in neighbors)
            {
                neighbor.TakeDamage(RotStrength * delta);
            }
        }
    }
    
    public bool IsDead()
    {
        return _is_dead;
    }
    
    public void Whiten()
    {
        _health = _max_health;
    }
    
    public void TakeDamage(float damage)
    {
        _health -= damage;
    }
    
    public Color GetColor()
    {
        var i = (_max_health - _health) / _max_health;
        return _tooth_decay_gradient.Interpolate(i);
    }
    
    public void UpdateColor()
    {
        SpatialMaterial sm = GetNode<CSGSphere>("Model").Material as SpatialMaterial;
        sm.AlbedoColor = GetColor();
    }
}
