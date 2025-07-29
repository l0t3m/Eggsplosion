using Fusion;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class CharacterSelection : NetworkBehaviour
{
    public Material[] UIColors;
    public int[] selectedColors;

    private Image myImage;
    private int mySelectedColor;
    public override void Spawned()
    {
        base.Spawned();
        StartCoroutine(SpawnLogic());
    }

    private System.Collections.IEnumerator SpawnLogic()
    {
        selectedColors = new int[Runner.SessionInfo.MaxPlayers];
        myImage = GetComponentInChildren<Image>();
        NextButton();
        myImage.material = UIColors[mySelectedColor];
        yield return null;
    }

    [Rpc]
    public void RPC_SelectColor(int playerIndex, int color, RpcInfo info = default)
    {
        selectedColors[playerIndex] = color;
    }

    public void NextButton()
    {
        do
        {
            mySelectedColor++;
            if (mySelectedColor == UIColors.Length)
                mySelectedColor = 0;
        } while (selectedColors.Contains(mySelectedColor));

        RPC_SelectColor(Runner.LocalPlayer.PlayerId - 1, mySelectedColor);
        myImage.material = UIColors[mySelectedColor];
    }

    public void PreviousButton()
    {
        do
        {
            mySelectedColor--;
            if (mySelectedColor == -1)
                mySelectedColor = UIColors.Length-1;
        } while (selectedColors.Contains(mySelectedColor));

        RPC_SelectColor(Runner.LocalPlayer.PlayerId-1, mySelectedColor);
        myImage.material = UIColors[mySelectedColor];
    }
}
