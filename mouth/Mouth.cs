using Godot;
using System;
using System.Collections.Generic;
using System.Linq;


public class Mouth : Spatial
{
    [Signal]
    public delegate void OnDeath();
    
    private IList<Jawline> _jawlines;
    private RandomNumberGenerator _rng = new RandomNumberGenerator();
    
    
    public override void _Ready()
    {
        _jawlines = new List<Jawline> 
        {
            GetNode<Jawline>("BottomJawline"),
            GetNode<Jawline>("TopJawline")
        };
    }
    
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
    
    public Node SpawnParticleRandom()
    {
        return _jawlines[_rng.RandiRange(0, 1)].SpawnParticleRandom();
    }
    
    public bool IsDead()
    {
        return GetTeethChildren().All(t => t.IsDead());
    }


    private void _on_GameManager_SpawnParticle()
    {
        SpawnParticleRandom();
    }
}
