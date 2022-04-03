using Godot;
using System;

public class End : Control
{
    public override void _Ready()
    {
        GetNode<Label>("LblScore").Text = $"Final Score: {PlayerVariables.Score}";
    }

    private void _on_BtnStart_pressed()
    {
        PlayerVariables.Score = 0;
        GetTree().ChangeScene("res://level/TestLevel.tscn");
    }

    private void _on_BtnQuit_pressed()
    {
        GetTree().Quit();
    }
}
