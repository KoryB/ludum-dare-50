using Godot;
using System;

public class StoreManager : Node
{
    [Signal]
    public delegate void OnLeaveStore();


    [Export]
    private Godot.Collections.Array _toothbrush_costs;
    private int _toothbrush_level = 0;
    
    [Export]
    private int _teeth_whitening_cost = 10;
    
    [Export]
    private int _pull_cost = 25;
    
    private GameManager _game_manager;
    
    
    public override void _Ready()
    {
        _game_manager = GetNode<GameManager>("../GameManager");
    }
    
    
    public void ActivateStore()
    {
        // Do this first, before triggering the update with visibility change 
        UpdateStoreCosts();
        
        GetNode<Control>("../SplitScreenContainer").Visible = false;
        GetNode<Control>("../ShopScreenContainer").Visible = true;
    }
    
    public void LeaveStore()
    {
        GetNode<Control>("../SplitScreenContainer").Visible = true;
        GetNode<Control>("../ShopScreenContainer").Visible = false;
        
        EmitSignal(nameof(OnLeaveStore));
    }
    
    
    public void UpdateStoreCosts()
    {
        GetNode<Label>("../ShopScreenContainer/VBoxContainer/BtnToothbrush/Cost").Text = GetToothbrushCost().ToString();
        GetNode<Label>("../ShopScreenContainer/VBoxContainer/BtnWhiten/Cost").Text = _teeth_whitening_cost.ToString();
        GetNode<Label>("../ShopScreenContainer/VBoxContainer/BtnPull/Cost").Text = _pull_cost.ToString();
        
        GetNode<Label>("../ShopScreenContainer/VBoxContainer/LblMoney/Amount").Text = _game_manager.GetMoney().ToString();
    }
    
    
    private int GetToothbrushCost()
    {
        return (int) _toothbrush_costs[_toothbrush_level];
    }

    
    private void ShowNotEnoughMoney()
    {
        GD.Print("timeout");
        
        GetNode<Timer>("HideTextTimer").Start();
        GetNode<Label>("../ShopScreenContainer/Title/LblNotEnoughMoney").Visible = true;
    }


    private void _on_BtnToothbrush_pressed()
    {
        if (_game_manager.CanSpendMoney(GetToothbrushCost()))
        {
            _game_manager.SpendMoney(GetToothbrushCost());
        
            _toothbrush_level = Math.Min(_toothbrush_level + 1, 3);
            GetNode<Toothbrush>("../Toothbrush").LevelUp();
            
            UpdateStoreCosts();
        }
        else
        {
            ShowNotEnoughMoney();
        }
    }

    private void _on_BtnWhiten_pressed()
    {
        if (_game_manager.CanSpendMoney(_teeth_whitening_cost))
        {
            _game_manager.SpendMoney(_teeth_whitening_cost);
            _game_manager.WhitenTeeth();
            
            UpdateStoreCosts();
        }
        else
        {
            ShowNotEnoughMoney();
        }
    }

    private void _on_BtnPull_pressed()
    {
        if (_game_manager.CanSpendMoney(_pull_cost))
        {
            _game_manager.SpendMoney(_pull_cost);
            _game_manager.PullTeeth();
            
            UpdateStoreCosts();
        }
        else
        {
            ShowNotEnoughMoney();
        }
    }


    private void _on_GameManager_WaveComplete()
    {
        ActivateStore();
    }
    

    private void _on_HideTextTimer_timeout()
    {
        GetNode<Label>("../ShopScreenContainer/Title/LblNotEnoughMoney").Visible = false;
    }


    private void _on_BtnLeave_pressed()
    {
        LeaveStore();
    }
}
