using Godot;
using System;
using System.Collections.Generic;
using System.Linq;


public class Mouth : Spatial
{
    private const int BottomJawlineIndex = 0;
    private const int TopJawlineIndex = 1;

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
        }
    }
    
    private IEnumerable<Tooth> GetTeethChildren()
    {
        var teeth_nodes = new List<Spatial>()
        {
            GetNode<Spatial>("TopTeeth"),
            GetNode<Spatial>("BottomTeeth")
        };
        
        var teeth = new List<Tooth>();
        
        foreach (Spatial t in teeth_nodes)
        {
            teeth.AddRange(
                t.GetChildren()
                    .Cast<Godot.Object>()
                    .Where(x => x is Tooth)
                    .Cast<Tooth>()
            );
        }
        
        return teeth;
    }
    
    public bool HasTopTeeth()
    {
        return GetNode<Spatial>("TopTeeth").GetChildCount() != 0;
    }
    
    public bool HasBottomTeeth()
    {
        return GetNode<Spatial>("BottomTeeth").GetChildCount() != 0;
    }
    
    public Node SpawnParticleRandom()
    {
        var jawline_index = _rng.RandiRange(0, 1);
    
        if (jawline_index == BottomJawlineIndex)
        {
            if (HasBottomTeeth())
            {
                return _jawlines[BottomJawlineIndex].SpawnParticleRandom();
            }
            else
            {
                if (HasTopTeeth())
                {
                    return _jawlines[TopJawlineIndex].SpawnParticleRandom();
                }
            }
        }
        else
        {
            if (HasTopTeeth())
            {
                return _jawlines[TopJawlineIndex].SpawnParticleRandom();
            }
            else
            {
                if (HasBottomTeeth())
                {
                    return _jawlines[BottomJawlineIndex].SpawnParticleRandom();
                }
            }
        }
    
        return null;
    }
    
    public bool IsDead()
    {
        return !GetTeethChildren().Any() || 
            GetNode<Spatial>("TopTeeth").GetChildren().Cast<Tooth>().All(t => t.IsDead()) ||
            GetNode<Spatial>("BottomTeeth").GetChildren().Cast<Tooth>().All(t => t.IsDead());
    }
    
    public void WhitenTeeth()
    {
        foreach(Tooth tooth in GetTeethChildren())
        {
            if (!tooth.IsDead())
            {
                tooth.Whiten();
            }
        }
    }

    public void PullTeeth()
    {
        foreach(Tooth tooth in GetTeethChildren())
        {
            if (tooth.IsDead())
            {
                tooth.QueueFree();
            }
        }
    }


    private void _on_GameManager_SpawnParticle()
    {
        SpawnParticleRandom();
    }
}
