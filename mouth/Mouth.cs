using Godot;
using System;
using System.Collections.Generic;
using System.Linq;


public class Mouth : Spatial
{
    [Signal]
    public delegate void OnDeath();
    
    public override void _Process(float delta)
    {
        if (IsDead())
        {
            EmitSignal(nameof(OnDeath));
            GD.Print("Teeth dead!");
        }
    }
    
    private IEnumerable<Tooth> GetTeethChildren()
    {
        return GetNode<Spatial>("BottomTeeth")
            .GetChildren().Cast<Godot.Object>()
            .Where(x => x is Tooth).Cast<Tooth>();
    }
    
    public bool IsDead()
    {
        return GetTeethChildren().All(t => t.IsDead());
    }
}
