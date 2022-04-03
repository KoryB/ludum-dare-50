using Godot;
using System;
using System.Collections.Generic;
using System.Linq;


public class Mouth : Spatial
{
    [Signal]
    public delegate void OnDeath();    

    private IList<Tooth> _teeth;
    
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {        
        SetupTeeth();
    }
    
    public override void _Process(float delta)
    {
        if (_teeth.All(t => t.IsDead()))
        {
            EmitSignal(nameof(OnDeath));
            GD.Print("Teeth dead!");
        }
    }
    
    private void SetupTeeth()
    {
        _teeth = GetTeethChildren();
    }
    
    private IList<Tooth> GetTeethChildren()
    {
        return GetNode<Spatial>("BottomTeeth")
            .GetChildren().Cast<Godot.Object>()
            .Where(x => x is Tooth).Cast<Tooth>()
            .ToList();
    }
}
