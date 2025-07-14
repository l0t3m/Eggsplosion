using Fusion;
using System;
using TMPro;
using UnityEngine;

public class CharacterSelectionIndividualHandler : NetworkObject
{
    public event Action<int> NextPressed;
    public event Action<int> PreviousPressed;
    [HideInInspector] public int PlayerNumber;
    public TextMeshProUGUI PlayerText;
    
    [Networked]
    public string ColorName { get => default; set { ColorName = value; }}

    public void PressNext()
    {
        NextPressed?.Invoke(PlayerNumber);
    }

    public void PressPrevious()
    {
        PreviousPressed?.Invoke(PlayerNumber);
    }
}
