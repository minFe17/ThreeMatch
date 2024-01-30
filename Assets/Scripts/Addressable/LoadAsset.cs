using System.Threading.Tasks;
using UnityEngine;
using Utils;

public class LoadAsset : MonoBehaviour
{
    // �̱���
    public async Task Init()
    {
        await LoadLobbyAsset();
    }

    async Task LoadLobbyAsset()
    {
        await GenericSingleton<SpriteManager>.Instance.Init();
    }
}