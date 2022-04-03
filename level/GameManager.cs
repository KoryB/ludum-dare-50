using Godot;
using System;

public class GameManager : Node
{
    [Signal]
    public delegate void SpawnParticle();

    [Export]
    private float _particle_delay = 3.0f;
    
    private float _particle_spawn_timer = 0.0f;


    public override void _Process(float delta)
    {
        _particle_spawn_timer += delta;
               
        if (_particle_spawn_timer > _particle_delay)
        {
            _particle_spawn_timer = 0.0f;
            EmitSignal(nameof(SpawnParticle));
        }
    }
}
