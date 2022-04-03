using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public class Food : Particle
{
    private const float DamageScaleFactor = 0.13f; // 30s for one food to deal 10 damage

    [Export]
    private float _strength = 0.5f;
    private float _lifetime = 0.0f;

    private int _current_frame = 0;
    private IEnumerable<Tooth> _touching_teeth = new List<Tooth>();

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(float delta)
    {
        _lifetime += delta;
        
        foreach (Tooth tooth in _touching_teeth)
        {
            Damage(tooth, delta);
        }
    }
    
    public override void _PhysicsProcess(float delta)
    {
        _current_frame += 1;
        
        // 10 because it needs a few frames to activate
        if (_current_frame == 10)
        {   
            _touching_teeth = _area.GetOverlappingBodies()
                .Cast<Godot.Object>()
                .Where(x => x is Tooth)
                .Cast<Tooth>()
                .ToList();
                
            GD.Print(_area.GetOverlappingBodies());
            GD.Print(String.Join(", ", _touching_teeth));
        }
    }
    
    public void Damage(Tooth tooth, float delta)
    {
        var damage = CalculateDamage();
        tooth.TakeDamage(damage * delta);
    }
    
    public float CalculateDamage()
    {
        return GetHealthPercent() * DamageScaleFactor * (float) Math.Log(_lifetime + 1.0f);
    }
}
