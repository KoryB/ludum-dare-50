using Godot;
using System;

public class GameManager : Node
{
    [Signal]
    public delegate void SpawnParticle();
    
    [Signal]
    public delegate void WaveComplete();
    
    [Export]
    private NodePath _progress_bar_path = "";
    private ProgressBar _progress_bar;
    
    [Export]
    private NodePath _split_screen_container_path = "";
    private Control _split_screen_container;
    
    [Export]
    private NodePath _shop_screen_container_path = "";
    private Control _shop_screen_container;

    private float _particle_spawn_timer = 0.0f;
    private float _wave_timer = 0.0f;
    private int _wave_counter = 1;
    private int _money = 5;

    private bool _is_active = true;
    
    
    public override void _Ready()
    {
        _progress_bar = GetNode<ProgressBar>(_progress_bar_path);
    }


    public override void _Process(float delta)
    {
        if (_is_active)
        {
            UpdateParticleSpawn(delta);
            UpdateWaveTimer(delta);
            UpdateProgressBar();
            UpdateLabels();
            UpdateGameOver();
        }
    }
    
    private void UpdateParticleSpawn(float delta)
    {      
        _particle_spawn_timer += delta;
        
        if (_particle_spawn_timer > GetWaveParticleDelay())
        {
            _particle_spawn_timer = 0.0f;
            var particle = GetNode<Mouth>("../Mouth").SpawnParticleRandom();
            
            if (particle != null)
            {
                AttachDeathListener(particle);
            }
        }
    }
    
    private void UpdateWaveTimer(float delta)
    {
        _wave_timer += delta;
        
        if (_wave_timer > GetWaveTime())
        {
            OnWaveComplete();
        }
    }
    
    private void UpdateLabels()
    {
        GetNode<Label>("../SplitScreenContainer/Score").Text = $"Score: {PlayerVariables.Score}";
        GetNode<Label>("../SplitScreenContainer/Score/Money").Text = $"Money: {_money}";
        GetNode<Label>("../SplitScreenContainer/Score/Money/Wave").Text = $"Wave: {_wave_counter}";
    }
    
    private void UpdateProgressBar()
    {
        _progress_bar.Value = 1.0f - GetWaveCompletionPercentage();
    }
    
    private void UpdateGameOver()
    {
        if (GetNode<Mouth>("../Mouth").IsDead())
        {
            GetTree().ChangeScene("res://level/end/End.tscn");
        }
    }
    
    private void OnWaveComplete()
    {
        _is_active = false;
        
        EmitSignal(nameof(WaveComplete));
    }
    
    private void StartNewWave()
    {
        _particle_spawn_timer = 0.0f;
        _wave_timer = 0.0f;
        
        _wave_counter += 1;
        _is_active = true;
        
        ClearFloatingParticles();
    }
    
    private void ClearFloatingParticles()
    {
        foreach (Node n in GetTree().GetNodesInGroup(Particle.Group))
        {
            if (n is Particle p)
            {
                if (p.IsFloating())
                {
                    p.QueueFree();
                }
            }
        }
    }
    
    private void AttachDeathListener(Node particle)
    {
        GD.Print("Particle Connected");
        particle.Connect("OnDeath", this, "OnParticleDeath");
    }
    
    public float GetWaveCompletionPercentage()
    {
        return _wave_timer / GetWaveTime();
    }
    
    public float GetWaveParticleDelay()
    {
        return 6.0f / (Mathf.Sqrt(_wave_counter) + 1);
    }
    
    public float GetWaveTime()
    {
        return 5 + 10 * Mathf.Log(Mathf.Sqrt(_wave_counter) + 1);
    }
    
    public int GetMoney()
    {
        return _money;
    }
    
    public bool CanSpendMoney(int amount)
    {
        return _money >= amount;
    }
    
    public void SpendMoney(int amount)
    {
        _money -= amount;
    }
    
    public void WhitenTeeth()
    {
        GetNode<Mouth>("../Mouth").WhitenTeeth();
    }
    
    public void PullTeeth()
    {
        GetNode<Mouth>("../Mouth").PullTeeth();
    }
    
    
    private void OnParticleDeath()
    {
        PlayerVariables.Score += 1;
        _money += 1;
    }


    private void _on_StoreManager_OnLeaveStore()
    {
        StartNewWave();
    }
}
