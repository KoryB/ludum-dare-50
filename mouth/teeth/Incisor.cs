using Godot;
using System;

public class Incisor : Tooth
{    
    public override void _Process(float delta)
    {
        base._Process(delta);
        
        _health -= delta;
        _health = Math.Max(_health, 0.0f);
    }
}
