using Godot;
using System;
using System.Linq;

public class Particle : StaticBody, IBrushable
{
    public const String Group = "Particle";

    [Signal]
    public delegate void OnDeath();

    [Export]
    protected float _max_health = 10.0f;
    protected float _health;
    
    protected Area _area;
    
    public override void _Ready()
    {
        _health = _max_health;
        _area = GetNode<Area>("Area");
    }
    
    public void Brush(IBrush brush)
    {
        _health -= brush.GetStrength();
        
        var pct = GetHealthPercent();
        this.Transform = new Transform(
            new Vector3(pct, pct, pct).ToDiagonalMatrix(),
            this.Translation
        );
        
        if (_health <= 0)
        {
            Die();
        }
    }
    
    public bool IsFloating()
    {
        var is_touching_tooth = _area.GetOverlappingBodies()
            .Cast<Godot.Object>()
            .Where(x => x is Tooth)
            .Any();
            
        return !is_touching_tooth;
    }
    
    public void Die()
    {
        EmitSignal(nameof(OnDeath));
        QueueFree();
    }
    
    public float GetHealthPercent()
    {
        return _health / _max_health;
    }
}
