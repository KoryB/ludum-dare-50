using Godot;
using System;

public class Particle : StaticBody, IBrushable
{
    [Signal]
    delegate void OnDeath();

    [Export]
    private float _max_health = 10.0f;
    private float _health;
    
    public override void _Ready()
    {
        _health = _max_health;
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
